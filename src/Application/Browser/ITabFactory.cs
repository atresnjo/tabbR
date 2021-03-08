using StyletIoC;
using tabbR.Application.Browser.Core;

namespace tabbR.Application.Browser
{
    public interface ITabFactory
    {
        [Inject("Key")]
        ITabFinder Create(string key);
    }
}