using System.Collections.Generic;
using System.Threading.Tasks;
using FlaUI.Core.AutomationElements;
using tabbR.Application.Browser.Models;

namespace tabbR.Application.Browser.Core
{
    public interface ITabFinder
    {
        List<List<ResultDataModel>> FindBrowserTabElement(AutomationElement element);
        Task<IReadOnlyCollection<SearchResultItemModel>> GenerateResult(AutomationElement instance, List<ResultDataModel> results, string query);
    }
}