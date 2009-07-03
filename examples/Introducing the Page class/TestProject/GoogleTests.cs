using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;

namespace TestProject
{
    [TestClass]
    public class GoogleTests
    {
        [TestMethod]
        public void Search_for_watin_on_google_the_old_way()
        {
            using (var browser = new IE("http://www.google.com"))
            {
                browser.TextField(Find.ByName("q")).TypeText("WatiN");
                browser.Button(Find.ByName("btnG")).Click();

                Assert.IsTrue(browser.ContainsText("WatiN"));
            }
        }

        [TestMethod]
        public void Search_for_watin_on_google_using_page_class()
        {
            using (var browser = new IE("http://www.google.com"))
            {
                var searchPage = browser.Page<GoogleSearchPage>();
                searchPage.SearchCriteria.TypeText("WatiN");
                searchPage.SearchButton.Click();

                Assert.IsTrue(browser.ContainsText("WatiN"));
            }
        }

        [TestMethod]
        public void Page_with_an_action()
        {
            using (var browser = new IE("http://www.google.com"))
            {
                browser.Page<GoogleSearchPage>().SearchFor("WatiN");

                Assert.IsTrue(browser.ContainsText("WatiN"));
            }
        }

        [Page(UrlRegex = "www.google.*")]
        public class GoogleSearchPage : Page
        {
            [FindBy(Name = "q")] 
            public TextField SearchCriteria;

            [FindBy(Name = "btnG")] 
            public Button SearchButton;

            public void SearchFor(string searchCriteria)
            {
                SearchCriteria.TypeText("WatiN");
                SearchButton.Click();
            }
        }

//        [Page(UrlRegex = "www.google.*")]
//        public class GoogleSearchPage : Page
//        {
//            public TextField SearchCriteria
//            {
//                get { return Document.TextField(Find.ByName("q")); }
//            }
//
//            public Button SearchButton
//            {
//                get { return Document.Button(Find.ByName("btnG")); }
//            }
//
//            public void SearchFor(string searchCriteria)
//            {
//                SearchCriteria.TypeText("WatiN");
//                SearchButton.Click();
//            }
//        }
    }
}
