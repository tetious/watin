using System;
using WatiN.Core.UnitTests.Interfaces;

namespace WatiN.Core.UnitTests
{
    public class IEBrowserTestManager : IBrowserTestManager
    {
        private IE ie;

        public Browser GetBrowser(Uri uri)
        {
            if (ie == null)
            {
                ie = new IE(uri);
            }

            return ie;
        }

        public void CloseBrowser()
        {
            if (ie == null) return;
            ie.Close();
            ie = null;
        }
    }
}