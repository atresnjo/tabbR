using FlaUI.Core.AutomationElements;

namespace tabbR.Application.Browser.Models
{
    public class ResultDataModel
    {
        public string Name { get; }
        public AutomationElement TabElement { get; }

        public ResultDataModel(AutomationElement element, string name)
        {
            TabElement = element;
            Name = name;
        }
    }
}