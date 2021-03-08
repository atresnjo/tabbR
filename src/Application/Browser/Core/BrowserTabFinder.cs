using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using tabbR.Application.Browser.Models;
using tabbR.Application.Extensions;
using tabbR.ViewModels;

namespace tabbR.Application.Browser.Core
{
    public class BrowserTabFinder : IBrowserTabFinder
    {
        private readonly AutomationElement _desktopElement;
        private readonly ITabFactory _tabFactory;

        public BrowserTabFinder(ITabFactory tabFactory)
        {
            _tabFactory = tabFactory;
            _desktopElement = new UIA3Automation().GetDesktop();
        }

        public async Task<IReadOnlyCollection<SearchResultItemModel>> Search(string searchTerm)
        {
            var instances = GetBrowserInstances();
            var finalResults = new List<SearchResultItemModel>();

            foreach (var (tabFinder, automationElement) in instances.Where(x => !string.IsNullOrEmpty(x.Value?.Name)))
            {
                var tabs = tabFinder.FindBrowserTabElement(automationElement);
                if (tabs == null || !tabs.Any())
                    continue;

                foreach (var resultDataModels in tabs)
                {
                    var result = await tabFinder.GenerateResult(automationElement, resultDataModels, searchTerm);
                    finalResults.AddRange(result);
                }
            }

            return finalResults;
        }

        public Dictionary<ITabFinder, AutomationElement> GetBrowserInstances()
        {
            var result = new Dictionary<ITabFinder, AutomationElement>();
            foreach (var child in _desktopElement.FindAll(TreeScope.Children, new BoolCondition(true)))
            {
                var finder = Create(child);
                if (finder != null)
                    result.Add(finder, child);
            }

            return result;
        }

        private ITabFinder Create(AutomationElement child)
        {
            if (Bootstrapper.Tracker.GetValue<bool>(nameof(SettingsViewModel.IsFirefoxEnabled)))
                if (child.ClassName.Equals("MozillaWindowClass"))
                    return _tabFactory.Create("Firefox");

            if (Bootstrapper.Tracker.GetValue<bool>(nameof(SettingsViewModel.IsChromeEnabled)))
                if (child.ClassName.Equals("Chrome_WidgetWin_1") && child.ControlType.Equals(ControlType.Pane))
                    return _tabFactory.Create("Chrome");

            return null;
        }
    }
}