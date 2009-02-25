using System;
using System.Drawing;
using System.Web;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class BrowserTests : BaseWithBrowserTests
    {
        [Test, Category("InternetConnectionNeeded")]
        public void Google()
        {
            ExecuteTest(browser =>
                            {
                                browser.GoTo(GoogleUrl);

                                browser.TextField(Find.ByName("q")).TypeText("WatiN");
                                browser.Button(Find.ByName("btnG")).Click();

                                Assert.IsTrue(browser.ContainsText("WatiN"));
                            });
        }


        [Test, Category("InternetConnectionNeeded")]
        public void GoogleSearchWithEncodedQueryStringInGoTo()
        {

            ExecuteTest(browser =>
                            {
                                var url = string.Format("http://www.google.com/search?q={0}", HttpUtility.UrlEncode("a+b"));

                                browser.GoTo(url);
                                Assert.That(browser.TextField(Find.ByName("q")).Value, Is.EqualTo("a+b"));
                            });
        }

        [Test]
        public void WindowStyle()
        {
            ExecuteTest(browser =>
                            {
                                var currentStyle = browser.GetWindowStyle();

                                browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);
                                Assert.AreEqual(NativeMethods.WindowShowStyle.Maximize.ToString(), browser.GetWindowStyle().ToString(), "Not maximized");

                                browser.ShowWindow(NativeMethods.WindowShowStyle.Restore);
                                Assert.AreEqual(currentStyle.ToString(), browser.GetWindowStyle().ToString(), "Not Restored");

                                browser.ShowWindow(NativeMethods.WindowShowStyle.Minimize);
                                Assert.AreEqual(NativeMethods.WindowShowStyle.ShowMinimized.ToString(), browser.GetWindowStyle().ToString(), "Not Minimize");

                                browser.ShowWindow(NativeMethods.WindowShowStyle.ShowNormal);
                                Assert.AreEqual(NativeMethods.WindowShowStyle.ShowNormal.ToString(), browser.GetWindowStyle().ToString(), "Not ShowNormal");
                            });
        }

        [Test]
        public void AutoMoveMousePointerToTopLeft()
        {
            BrowsersToTestWith.ForEach(manager =>
                                           {
                                                manager.CloseBrowser();

                                                var notTopLeftPoint = new Point(50, 50);
                                                Cursor.Position = notTopLeftPoint;
                                                Settings.AutoMoveMousePointerToTopLeft = false;

                                                using (manager.CreateBrowser(TestPageUri))
                                                {
                                                    Assert.That(Cursor.Position, Is.EqualTo(notTopLeftPoint));
                                                }

                                                Settings.AutoMoveMousePointerToTopLeft = true;
                                                using (manager.CreateBrowser(TestPageUri))
                                                {
                                                    Assert.That(Cursor.Position, Is.EqualTo(new Point(0, 0)));
                                                }
                                            });
        }

        [Test]
        public void GoToUrl()
        {
            ExecuteTest(browser =>
                            {
                                var url = MainURI.AbsoluteUri;

                                browser.GoTo(url);

                                Assert.AreEqual(MainURI, new Uri(browser.Url));
                            });
        }

        [Test]
        public void GoToUrlNoWait()
        {
            ExecuteTest(browser =>
                            {
                                Assert.That(browser.Url, Is.EqualTo(AboutBlank));

                                var url = MainURI.AbsoluteUri;

                                browser.GoToNoWait(url);

                                Assert.That(browser.Url, Is.EqualTo(AboutBlank));

                                TryActionUntilTimeOut.Try(3, () => browser.Uri == MainURI);
                                browser.WaitForComplete();

                                Assert.AreEqual(MainURI, new Uri(browser.Url));
                            });
        }

        [Test]
        public void GoToUri()
        {
            ExecuteTest(browser =>
                            {
                                browser.GoTo(MainURI);
                                Assert.AreEqual(MainURI, browser.Uri);
                            });

        }

        [Test]
        public void BackAndForward()
        {
            ExecuteTest(browser =>
                            {
                                browser.GoTo(MainURI);
                                Assert.AreEqual(MainURI, new Uri(browser.Url));

                                browser.Link(Find.ByUrl(IndexURI)).Click();
                                Assert.AreEqual(IndexURI, new Uri(browser.Url));

                                var wentBack = browser.Back();
                                Assert.AreEqual(MainURI, new Uri(browser.Url));
                                Assert.That(wentBack, "Expected went back");

                                var wentFoward = browser.Forward();
                                Assert.AreEqual(IndexURI, new Uri(browser.Url));
                                Assert.That(wentFoward, "Expected went forward");

                            });
        }


        [Test]
        public void BackShouldNotBePossibleOnBrowserWithNoHistory()
        {
            BrowsersToTestWith.ForEach(manager =>
                            {
                                manager.CloseBrowser();

                                var browser = manager.GetBrowser(AboutBlank);

                                var wentBack = browser.Back();
                                Assert.That(wentBack, Is.False, "Expected no navigation back");
                            });
        }

        [Test]
        public void ForwardShouldNotBePossibleOnBrowserWithNoHistory()
        {
            BrowsersToTestWith.ForEach(manager =>
                            {
                                manager.CloseBrowser();

                                var browser = manager.GetBrowser(AboutBlank);

                                var wentForward = browser.Forward();
                                Assert.That(wentForward, Is.False, "Expected no navigation back");
                            });
        }

        [Test]
        public void Reopen()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                var oldBrowserHwnd = browser.hWnd;

                                browser.GoTo(MainURI);
                                Assert.AreEqual(MainURI, new Uri(browser.Url));

                                // WHEN
                                browser.Reopen();

                                // THEN
                                Assert.AreNotSame(oldBrowserHwnd, browser.hWnd, "Reopen should create a new browser.");
                                Assert.AreEqual("about:blank", browser.Url, "Unexpected url after reopen");
                            });

        }

        [Test]
        public void RefreshWithNeverExpiredPage()
        {
            ExecuteTest(browser =>
                            {
                                browser.GoTo(MainURI);
                                browser.TextField("name").TypeText("refresh test");

                                browser.Refresh();

                                Assert.AreEqual("refresh test", browser.TextField("name").Text);
                            });

        }

        [Test, Category("InternetConnectionNeeded")]
        public void RefreshWithImmediatelyExpiredPage()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                browser.GoTo(GoogleUrl);
                                browser.TextField(Find.ByName("q")).TypeText("refresh test");

                                // WHEN
                                browser.Refresh();

                                // THEN
                                Assert.AreEqual(null, browser.TextField(Find.ByName("q")).Text);
                            });

        }

        [Test]
        public void ActiveElementShouldBeCorrectWhenFocusIsSetOnElement()
        {
            ExecuteTest(browser =>
                            {
                                browser.GoTo(MainURI);

                                var element = browser.ActiveElement;
                                Assert.That(element.Id, Is.Not.EqualTo("popupid"), "pre-condition: expected popupid hasn't the focus");

                                browser.Button("popupid").Focus();

                                element = browser.ActiveElement;
                                Assert.That(element.Id,Is.EqualTo("popupid"), "Unexpected ActiveElement");
                            });
        }


        public override Uri TestPageUri
        {
            get { return AboutBlank; }
        }
    }
}
