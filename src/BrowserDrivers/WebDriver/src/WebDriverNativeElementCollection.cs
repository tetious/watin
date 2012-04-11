using System.Collections.Generic;
using WatiN.Core.Native;
using OpenQA.Selenium;
using WatiN.Core;

namespace Watin.BrowserDrivers.WebDriver
{
    public class WebDriverNativeElementCollection : INativeElementCollection2
    {
        private readonly ISearchContext _context;

        public WebDriverNativeElementCollection(ISearchContext searchContext)
        {
            _context = searchContext;
        }

        public IEnumerable<INativeElement> GetElements()
        {
            return FindElements(By.TagName("*"));
        }

        public IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            return FindElements(By.TagName(tagName));
        }

        public IEnumerable<INativeElement> GetElementsById(string id)
        {
            return FindElements(By.Id(id));
        }

        private IEnumerable<INativeElement> FindElements(By by)
        {
            var elements = _context.FindElements(by);
            foreach (var element in elements)
                yield return new WebDriverNativeElement(element);

        }

        public IEnumerable<INativeElement> GetElementsWithQuerySelector(ICssSelector selector, DomContainer domContainer)
        {
            return FindElements(By.CssSelector(selector.Selector(false)));
        }
    }
}
