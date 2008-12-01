using System;
using mshtml;
using WatiN.Core.Interfaces;

namespace WatiN.Core.InternetExplorer
{
    public class IEDocument : INativeDocument
    {
        public IEDocument(object document)
        {
            if (!(document is IHTMLDocument)) throw new ArgumentException("document should be of type IHTMLDocument");
        }
    }
}
