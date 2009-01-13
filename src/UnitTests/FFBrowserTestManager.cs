using System;
using WatiN.Core.UnitTests.Interfaces;

namespace WatiN.Core.UnitTests
{
    public class FFBrowserTestManager : IBrowserTestManager
    {
        private FireFox firefox;

        public Browser GetBrowser(Uri uri)
        {
            if (firefox == null)
            {
                firefox = new FireFox(uri);
            }

            return firefox;
        }

        public void CloseBrowser()
        {
            if (firefox == null) return;
            firefox.Dispose();
            firefox = null;
        }
    }
}