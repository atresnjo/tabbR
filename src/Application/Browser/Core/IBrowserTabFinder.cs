using System.Collections.Generic;
using System.Threading.Tasks;
using tabbR.Application.Browser.Models;

namespace tabbR.Application.Browser.Core
{
    public interface IBrowserTabFinder
    {
        public Task<IReadOnlyCollection<SearchResultItemModel>> Search(string searchTerm);
    }
}
