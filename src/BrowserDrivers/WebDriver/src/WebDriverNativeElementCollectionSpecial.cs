using System;
using System.Collections.Generic;
using WatiN.Core.Native;
using OpenQA.Selenium;

namespace Watin.BrowserDrivers.WebDriver
{
    public class WebDriverNativeElementCollectionSpecial : INativeElementCollection2
    {
        private readonly Func<IEnumerable<IWebElement>> _getWebElements;

        public WebDriverNativeElementCollectionSpecial(Func<IEnumerable<IWebElement>> getWebElements)
        {
            _getWebElements = getWebElements;
        }

        public IEnumerable<INativeElement> GetElements()
        {
            foreach (var element in GetWebElements())
                yield return new WebDriverNativeElement(element);
        }

        public IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            foreach (var element in GetWebElements())
            {
                if (AreEqual(tagName, element.TagName))
                    yield return new WebDriverNativeElement(element);
            }
        }

        public IEnumerable<INativeElement> GetElementsById(string id)
        {
            foreach (var element in GetWebElements())
            {
                if (AreEqual(id, element.GetAttribute("id")))
                    yield return new WebDriverNativeElement(element);
            }
        }

        private bool AreEqual(string lhs, string rhs)
        {
            return WatiN.Core.Comparers.StringComparer.AreEqual(lhs, rhs);
        }

        private IEnumerable<IWebElement> GetWebElements()
        {
            var children = _getWebElements.Invoke();

            foreach (var child in children)
            {
                yield return child;
            }
        }

        public IEnumerable<INativeElement> GetElementsWithQuerySelector(ICssSelector selector, WatiN.Core.DomContainer domContainer)
        {
            throw new NotImplementedException();
        }
    }
}
