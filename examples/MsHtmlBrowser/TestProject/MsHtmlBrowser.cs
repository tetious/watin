using System;
using WatiN.Core;
using WatiN.Core.Native;
using WatiN.Core.Native.InternetExplorer;

namespace WatiN.Examples.MsHtmlBrowser
{
    public class MsHtmlBrowser : Browser
    {
        private readonly MsHtmlNativeBrowser _nativeBrowser;

        public MsHtmlBrowser()
        {
            _nativeBrowser = new MsHtmlNativeBrowser();
        }

        public override void WaitForComplete(int waitForCompleteTimeOut)
        {
            var msHtmlNativeBrowser = (MsHtmlNativeBrowser)NativeBrowser;
            var ieDocument = (IEDocument)msHtmlNativeBrowser.NativeDocument;

            var wait = new IEWaitForComplete(ieDocument, waitForCompleteTimeOut);
            wait.DoWait();
        }

        public override INativeBrowser NativeBrowser
        {
            get { return _nativeBrowser; }
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }
    }
}