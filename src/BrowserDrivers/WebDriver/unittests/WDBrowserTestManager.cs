using System;
using WatiN.Core.UnitTests.TestUtils;
using WatiN.Core;
using OpenQA.Selenium.IE;

namespace Watin.BrowserDrivers.WebDriver.Tests
{
    public class WDBrowserTestManager : IBrowserTestManager
    {
        private WebDriver _webDriver;

        public Browser CreateBrowser(Uri uri)
        {
            return new WebDriver(new InternetExplorerDriver(), uri);
        }

        public Browser GetBrowser(Uri uri)
        {
            return _webDriver ?? (_webDriver = (WebDriver) CreateBrowser(uri));
        }

        public void CloseBrowser()
        {
            if (_webDriver == null) return;
            _webDriver.Close();
            _webDriver = null;
        }
    }

}
