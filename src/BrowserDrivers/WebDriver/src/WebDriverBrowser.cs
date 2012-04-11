using System;
using WatiN.Core.Native;
using OpenQA.Selenium;

namespace Watin.BrowserDrivers.WebDriver
{
    public class WebdriverBrowser : INativeBrowser
    {
        public IWebDriver Driver;

        public WebdriverBrowser(IWebDriver ieDriver)
        {
            Driver = ieDriver;
        }

        public void Close()
        {
            Driver.Quit();
        }

        public void NavigateTo(Uri url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public void NavigateToNoWait(Uri url)
        {
            Driver.Navigate().GoToUrl(url); // TODO: is this good enough? async?
        }

        public bool GoBack()
        {
            Driver.Navigate().Back();
            return true; // TODO: Should return False when not navigated back (first entry).
        }

        public bool GoForward()
        {
            Driver.Navigate().Forward();
            return true; // TODO: Should return False when not navigated forward (most recent entry).
        }

        public void Reopen()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            Driver.Navigate().Refresh();
        }

        public IntPtr hWnd
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        public INativeDocument NativeDocument
        {
            get { return new WebDriverNativeDocument(Driver); }
        }
    }
}
