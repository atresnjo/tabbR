using FlaUI.Core.AutomationElements;

namespace tabbR.Application.Browser.Models
{
    public class SearchResultItemModel
    {
        public string Title { get; }
        public string Subtitle { get; }
        public byte[] ImagePayload { get; }
        public AutomationElement Instance { get; }
        public AutomationElement TabItem { get; }

        public SearchResultItemModel(string title, string subtitle, byte[] imagePayload, AutomationElement instance,
            AutomationElement tabItem)
        {
            Title = title;
            Subtitle = subtitle;
            ImagePayload = imagePayload;
            Instance = instance;
            TabItem = tabItem;
        }
    }
}
