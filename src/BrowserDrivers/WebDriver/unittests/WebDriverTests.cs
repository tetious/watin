using System;
using NUnit.Framework;
using OpenQA.Selenium;
using WatiN.Core;
using OpenQA.Selenium.Chrome;

namespace WatiN.BrowserDrivers.WebDriver.UnitTests
{
    [TestFixture, RequiresSTA]
    public class WebdriverBrowserTests
    {
        private Browser _browser;

        [TearDown]
        public void TearDown()
        {
            if (_browser != null) _browser.Close();
        }

        [Test]
        public void Should_navigate_to_google()
        {
            var ieDriver = new ChromeDriver(); //@"C:\Users\Jeroen van Menen\AppData\Local\Google\Chrome\Application");
            ieDriver.Navigate().GoToUrl("http://www.google.com");
            ieDriver.FindElement(By.Name("q")).SendKeys("WatiN meets WebDriver");
            ieDriver.FindElement(By.Name("btnG")).Click();
            ieDriver.Quit();
        }

        [Test]
        public void Should_navigate_to_google_3()
        {
            _browser = new IE("www.google.com");
            var q = _browser.TextField(Find.ByName("q"));
            q.TypeText("WatiN meets WebDriver");
//            browser.TextField(Find.ByName("q")).Blur();
            //browser.Button(Find.ByName("btnG")).Click();
        }

        [Test]
        public void Should_navigate_to_google_2()
        {
            Settings.WaitForCompleteTimeOut = 5;
            Settings.WaitUntilExistsTimeOut = 5;

            _browser = new Watin.BrowserDrivers.WebDriver.WebDriver(new ChromeDriver(), new Uri("http://www.google.com"));
            _browser.TextField(Find.ByName("q")).TypeText("WatiN meets WebDriver");
            _browser.Button(Find.ByName("btnG")).Click();

            Assert.That(_browser.Url, Is.StringContaining("q=WatiN+meets+WebDriver"));
        }

        
    }
}
