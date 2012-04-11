using WatiN.Core.Native;
using WatiN.Core;
using OpenQA.Selenium;

namespace Watin.BrowserDrivers.WebDriver
{
    public class WebDriverSelectAction : ISelectAction
    {
        private readonly Option _option;

        public WebDriverSelectAction(Option option)
        {
            _option = option;
        }

        public void Deselect(bool waitForComplete)
        {
            WebElement.Click();
        }

        public void Select(bool waitForComplete)
        {
            WebElement.Click();
        }

        public IWebElement WebElement
        {
            get
            {
                var nativeElement = (WebDriverNativeElement)_option.NativeElement;
                return nativeElement.WebElement;
            }
        }

    }
}
