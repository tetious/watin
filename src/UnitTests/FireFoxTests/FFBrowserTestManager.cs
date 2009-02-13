using System;
using WatiN.Core.UnitTests.Interfaces;

namespace WatiN.Core.UnitTests.FireFoxTests
{
    public class FFBrowserTestManager : IBrowserTestManager
    {
        private FireFox firefox;

        public Browser CreateBrowser(Uri uri)
        {
            return new FireFox(uri);
        }

        public Browser GetBrowser(Uri uri)
        {
            if (firefox == null)
            {
                firefox = (FireFox) CreateBrowser(uri);
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