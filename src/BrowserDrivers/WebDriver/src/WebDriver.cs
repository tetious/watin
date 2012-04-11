using System;
using OpenQA.Selenium;
using WatiN.Core;
using WatiN.Core.Native;
using WatiN.Core.UtilityClasses;

namespace Watin.BrowserDrivers.WebDriver
{
    public class WebDriver : Browser
    {
        private readonly WebdriverBrowser _nativeBrowser;

        public WebDriver(IWebDriver ieDriver, Uri uri)
        {
            UtilityClass.MoveMousePoinerToTopLeft(Settings.AutoMoveMousePointerToTopLeft);

            _nativeBrowser = new WebdriverBrowser(ieDriver);
            _nativeBrowser.NavigateTo(uri);
        }
        public override void WaitForComplete(int waitForCompleteTimeOut)
        {
            //throw new NotImplementedException();
        }

        public override INativeBrowser NativeBrowser
        {
            get { return _nativeBrowser; }
        }
    }
}
