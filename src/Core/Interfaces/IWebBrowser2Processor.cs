using mshtml;
using SHDocVw;

namespace WatiN.Core.Interfaces
{
    internal interface IWebBrowser2Processor
    {
        HTMLDocument HTMLDocument();
        void Process(IWebBrowser2 webBrowser2);
        bool Continue();
    }
}