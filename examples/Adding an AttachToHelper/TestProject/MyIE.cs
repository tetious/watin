using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;

namespace WatiN.Examples.Tests
{
    public class MyIE : IE
    {
        public MyIE(string url) : base(url) {}
        
        public MyIE(IEBrowser browser) : base(browser){}

        public override string ToString()
        {
            return  Title + " at " + Url;
        }
    }
}