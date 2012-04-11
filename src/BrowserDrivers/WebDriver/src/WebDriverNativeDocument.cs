using System;
using System.Collections.Generic;
using WatiN.Core.Native;
using OpenQA.Selenium;
using System.Drawing;

namespace Watin.BrowserDrivers.WebDriver
{
    public class WebDriverNativeDocument : INativeDocument
    {
        private readonly IWebDriver _driver;
        private readonly IWebElement _containingFrameElement;
        
        public WebDriverNativeDocument(IWebDriver driver) : this (driver, null) { }

        public WebDriverNativeDocument(IWebDriver driver, IWebElement containingFrameElement)
        {
            _driver = driver;
            _containingFrameElement = containingFrameElement;
        }

        public INativeElementCollection AllElements
        {
            get { return new WebDriverNativeElementCollection(_driver); }
        }

        public INativeElement ContainingFrameElement
        {
            get { return new WebDriverNativeElement(_containingFrameElement); }
        }

        public INativeElement Body
        {
            get
            {
                var context = (_driver as ISearchContext);
                var webElement = context.FindElement(By.TagName("body"));
                return new WebDriverNativeElement(webElement);
            }
        }

        public string Url
        {
            get { return _driver.Url; }
        }

        public string Title
        {
            get { return _driver.Title; }
        }

        public INativeElement ActiveElement
        {
            get { return new WebDriverNativeElement(_driver.SwitchTo().ActiveElement()); }
        }

        public string JavaScriptVariableName
        {
            get { return "document"; }
        }

        public IList<INativeDocument> Frames
        {
            get 
            {
                var frames = new List<INativeDocument>();

                var nativeFrames = _driver.FindElements(By.TagName("frame"));
                foreach (var nativeFrame in nativeFrames)
                {
                    var driver = WebDriverNativeElement.CastElementToDriver(nativeFrame);
                    var frame = new WebDriverNativeDocument(driver, nativeFrame);
                    frames.Add(frame);
                }

                return frames;
            }
        }

        public void RunScript(string scriptCode, string language)
        {
            var executor = _driver as IJavaScriptExecutor;
            executor.ExecuteScript(scriptCode);
        }

        public string GetPropertyValue(string propertyName)
        {
            return "TODO"; // TODO
        }

        public IEnumerable<Rectangle> GetTextBounds(string text)
        {
            throw new NotImplementedException();
        }

        public bool ContainsText(string text)
        {
            var source = _driver.PageSource;
            if (source == null) return false;

            return source.IndexOf(text, StringComparison.Ordinal) >= 0;

        }
    }
}
