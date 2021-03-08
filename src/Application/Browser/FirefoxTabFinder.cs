using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using tabbR.Application.Browser.Core;
using tabbR.Application.Browser.Models;
using tabbR.Application.Extensions;

namespace tabbR.Application.Browser
{
    public class FirefoxTabFinder : ITabFinder
    {
        public List<List<ResultDataModel>> FindBrowserTabElement(AutomationElement element)
        {
            var children = element.FindAll(TreeScope.Children, TrueCondition.Default);
            var tabBar = children.FirstOrDefault(x => x.Name != null && x.Name.ToLowerInvariant().Contains("tabs"));
            // doesn't work for tabs that are full screen videos
            if (tabBar == null)
                return null;

            return new List<List<ResultDataModel>>
            {
                GetBrowserTabData(tabBar)
            };
        }

        public List<ResultDataModel> GetBrowserTabData(AutomationElement element)
        {
            var browserTabData = element.FindFirstChild()?.FindAllChildren()?
                .Where(match => match.ControlType == ControlType.TabItem);

            return browserTabData == null
                ? new List<ResultDataModel>()
                : browserTabData.Select(foundTabs => new ResultDataModel(foundTabs, foundTabs.Name)).ToList();
        }

        public async Task<IReadOnlyCollection<SearchResultItemModel>> GenerateResult(AutomationElement instance, List<ResultDataModel> results, string query)
        {
            var result = new List<SearchResultItemModel>();
            var path = "firefox.png".BuildIconPath();
            var imagePayload = await File.ReadAllBytesAsync(path);

            foreach (var actualTab in results)
            {
                if (!actualTab.Name.ToLowerInvariant().Contains(query.ToLowerInvariant()))
                    continue;

                var queryResult = new SearchResultItemModel(actualTab.Name, "Firefox", imagePayload, instance, actualTab.TabElement);
                result.Add(queryResult);
            }

            return result;
        }
    }
}