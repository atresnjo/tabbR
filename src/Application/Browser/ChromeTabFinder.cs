using System;
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
    public class ChromeTabFinder : ITabFinder
    {
        public List<List<ResultDataModel>> FindBrowserTabElement(AutomationElement element)
        {
            try
            {
                var children = element.FindAll(TreeScope.Children, TrueCondition.Default);
                var browserToolbars = children
                    .Where(match => match.ControlType == ControlType.Pane && match.Name.Equals("Google Chrome"))
                    .ToList();

                var chrome = browserToolbars.FirstOrDefault();
                if (chrome == null) // extra check to ignore apps made with electron
                    return null;

                var isMinimized = WinApi.IsIconic(element.Properties.NativeWindowHandle);
                var windowWasShown = false;

                if (isMinimized)
                {
                    windowWasShown = true;
                    WinApi.ShowWindowAsync(element.Properties.NativeWindowHandle, WinApi.SW_SHOWNORMAL);
                }

                var firstPane = chrome.FindChildAt(1);
                var finalPane = firstPane.FindFirstChild().FindFirstChild();
                // doesn't work for tabs that are full screen videos
                if (finalPane == null)
                    return null;

                var chromeWindow = finalPane.FindChildAt(0);
                var tabItems = chromeWindow.FindAll(TreeScope.Children, TrueCondition.Default)
                    .Where(match => match.ControlType == ControlType.TabItem).ToList();

                var actualTabs = tabItems.Select(foundTabs => new ResultDataModel(foundTabs, foundTabs.Name)).ToList();
                var tabCollection = new List<List<ResultDataModel>> {actualTabs};

                if (windowWasShown)
                    WinApi.ShowWindowAsync(element.Properties.NativeWindowHandle, WinApi.SW_FORCEMINIMIZE);

                return tabCollection;
            }
            catch (Exception)
            {
                return new List<List<ResultDataModel>>();
            }
        }

        public async Task<IReadOnlyCollection<SearchResultItemModel>> GenerateResult(AutomationElement instance, List<ResultDataModel> results,
            string query)
        {
            var result = new List<SearchResultItemModel>();
            var path = "chrome.png".BuildIconPath();
            var imagePayload = await File.ReadAllBytesAsync(path);

            foreach (var actualTab in results)
            {
                if (!actualTab.Name.ToLowerInvariant().Contains(query.ToLowerInvariant()))
                    continue;

                var queryResult = new SearchResultItemModel(actualTab.Name, "Chrome", imagePayload, instance, actualTab.TabElement);
                result.Add(queryResult);
            }

            return result;
        }
    }
}
