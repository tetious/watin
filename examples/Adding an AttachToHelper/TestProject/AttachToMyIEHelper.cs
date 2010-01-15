using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;

namespace WatiN.Examples.Tests
{
    public class AttachToMyIEHelper : AttachToIeHelper
    {
        protected override IE CreateBrowserInstance(IEBrowser browser)
        {
            return new MyIE(browser);
        }
    }
}