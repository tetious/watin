#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SHDocVw;
using WatiN.Core.Constraints;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Logging;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.IETests
{
    [TestFixture]
    public class IeTests : BaseWatiNTest
    {
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();
            Logger.LogWriter = new DebugLogWriter();
        }

        [SetUp]
        public void SetUp()
        {
            Settings.Reset();
        }

        [Test]
        public void TestRunnerApartmentStateMustBeSTA()
        {
            Assert.IsTrue(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
        }

        [Test]
        public void ShouldBeAbleToGetTheMetaTags()
        {
            // GIVEN
            using (var browser = new IE(MainURI))
            {
                // WHEN
                var metaTags = browser.ElementsWithTag("meta");
                
                // THEN
                Assert.That(metaTags.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void CheckIEIsInheritingProperTypes()
        {
            using (var ie = new IE())
            {
                Assert.IsInstanceOfType(typeof (IE), ie, "Should be an IE instance");
                Assert.IsInstanceOfType(typeof (Browser), ie, "Should be a Browser instance");
                Assert.IsInstanceOfType(typeof (DomContainer), ie, "Should be a DomContainer instance");
            }
        }

        [Test, Category("InternetConnectionNeeded")]
        public void GoogleSearchWithEncodedQueryStringInConstructor()
        {
            var url = string.Format("http://www.google.com/search?q={0}", HttpUtility.UrlEncode("a+b"));

            using (var ie = new IE(url))
            {
                Assert.That(ie.TextField(Find.ByName("q")).Value, Is.EqualTo("a+b"));
            }
        }

        [Test, Ignore("Second assert fails in nunit console mode.")]
        public void PressTabAndActiveElement()
        {
            using (var ie = new IE(MainURI))
            {
                ie.TextField("name").Focus();

                var element = ie.ActiveElement;
                Assert.AreEqual("name", element.Id);

                ie.PressTab();

                element = ie.ActiveElement;
                Assert.AreEqual("popupid", element.Id);
            }
        }


        [Test, Category("InternetConnectionNeeded")]
        public void AddProtocolToUrlIfOmmitted()
        {
            using (var ie = new IE("www.google.com"))
            {
                Assert.That(ie.Url, Text.StartsWith("http://"));
            }
        }

        [Test]
        public void NewIeInstanceShouldTake_Settings_MakeNewIeInstanceVisible_IntoAccount()
        {
            Settings.MakeNewIeInstanceVisible = true;
            Assert.That(Settings.MakeNewIeInstanceVisible, "Default should be true");

            using (var ie = new IE())
            {
                Assert.That(((IWebBrowser2)ie.InternetExplorer).Visible, "IE Should be visible");
            }

            Settings.MakeNewIeInstanceVisible = false;
            Assert.That(Settings.MakeNewIeInstanceVisible, Is.EqualTo(false), "should be false");

            using (var ie = new IE())
            {
                Assert.That(((IWebBrowser2)ie.InternetExplorer).Visible, Is.EqualTo(false), "IE Should be visible");
            }
        }

        // TODO: move to BrowserTests when HTMLDialogs are supported by FireFox
        [Test]
        public void DocumentShouldBeDisposedSoHTMLDialogGetsDisposedAndReferenceCountIsOK()
        {
            DialogWatcher dialogWatcher;
            int ReferenceCount;

            using (var ie = new IE(MainURI))
            {
                ReferenceCount = ie.DialogWatcher.ReferenceCount;

                ie.Button("popupid").Click();

                using (Document document = ie.HtmlDialog(Find.ByIndex(0)))
                {
                    Assert.AreEqual(ReferenceCount + 1, ie.DialogWatcher.ReferenceCount, "DialogWatcher reference count");
                }

                dialogWatcher = ie.DialogWatcher;
            }

            Assert.AreEqual(ReferenceCount - 1, dialogWatcher.ReferenceCount, "DialogWatcher reference count should be zero after test");
        }

        [Test]
        public void NewIEWithUrlAndLogonDialogHandler()
        {
            FailIfIEWindowExists("main", "NewIEWithUrlAndLogonDialogHandler");

            var url = MainURI.AbsoluteUri;
            var logon = new LogonDialogHandler("y", "z");

            using (var ie = new IE(url, logon))
            {
                Assert.AreEqual(MainURI, new Uri(ie.Url));
                Assert.IsTrue(ie.DialogWatcher.Contains(logon));
                Assert.AreEqual(1, ie.DialogWatcher.Count);

                using (var ie1 = new IE(url, logon))
                {
                    Assert.AreEqual(MainURI, new Uri(ie1.Url));
                    Assert.IsTrue(ie.DialogWatcher.Contains(logon));
                    Assert.AreEqual(1, ie.DialogWatcher.Count);
                }
            }

            Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer should be closed by IE.Dispose");
        }


        [Test]
        public void NewIEInSameProcess()
        {
            using (var ie1 = new IE())
            {
                using (var ie2 = new IE())
                {
                    Assert.AreEqual(ie1.ProcessID, ie2.ProcessID);
                }
            }
        }

        [Test]
        public void NewIEInNewProcess()
        {
            using (var ie1 = new IE())
            {
                using (var ie2 = new IE(true))
                {
                    Assert.IsNotNull(ie2, "create ie in new process returned null");
                    Assert.AreNotEqual(ie1.ProcessID, ie2.ProcessID, "process id problem");
                }
            }
        }

        [Test]
        public void NewIEWithoutUrlShouldStartAtAboutBlank()
        {
            using (var ie = new IE())
            {
                Assert.AreEqual("about:blank", ie.Url);
            }
        }

        [Test]
        public void NewIEWithUri()
        {
            using (var ie = new IE(MainURI))
            {
                Assert.AreEqual(MainURI, new Uri(ie.Url));
            }
        }

        [Test]
        public void NewIEWithUriShouldAutoClose()
        {
            FailIfIEWindowExists("main", "NewIEWithUriShouldAutoClose");

            using (new IE(MainURI)) {}

            Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer should be closed by IE.Dispose");
        }

        [Test]
        public void NewIEWithUriNotAutoClose()
        {
            FailIfIEWindowExists("main", "NewIEWithUriNotAutoClose");

            using (var ie = new IE(MainURI))
            {
                Assert.IsTrue(ie.AutoClose);
                ie.AutoClose = false;
            }

            Assert.IsTrue(IsIEWindowOpen("main"), "Internet Explorer should NOT be closed by IE.Dispose");

            Browser.AttachTo<IE>(Find.ByTitle("main"), 3).Close();
        }

        [Test]
        public void NewIEWithUrl()
        {
            FailIfIEWindowExists("main", "NewIEWithUrl");

            var url = MainURI.AbsoluteUri;

            using (var ie = new IE(url))
            {
                Assert.AreEqual(MainURI, new Uri(ie.Url));
                Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");
            }

            Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer should be closed by IE.Dispose");
        }

        // TODO: Should this also become a multi browser test? 
        //       Logon dialog not currently supported by FireFox implementation
        [Test]
        public void ReopenWithUrlAndLogonDialogHandlerInNewProcess()
        {
            FailIfIEWindowExists("main", "ReopenWithUrlAndLogonDialogHandler");

            var logon = new LogonDialogHandler("y", "z");

            using (var ie1 = new IE())
            {
                using (var ie2 = new IE())
                {
                    Assert.AreEqual(ie1.ProcessID, ie2.ProcessID, "process id problem");

                    Assert.AreEqual("about:blank", ie2.Url);
                    var oldIEObj = ie2.InternetExplorer;

                    ie2.Reopen(MainURI, logon, true);
                    Assert.AreNotSame(oldIEObj, ie2.InternetExplorer, "Reopen should create a new browser.");

                    Assert.AreNotEqual(ie1.ProcessID, ie2.ProcessID, "process id problem");

                    Assert.AreEqual(MainURI, new Uri(ie2.Url));
                    Assert.IsTrue(ie2.DialogWatcher.Contains(logon));
                    Assert.AreEqual(1, ie2.DialogWatcher.Count);
                }
            }

            Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer should be closed by IE.Dispose");
        }

        [Test]
        public void Should_set_and_get_cookies()
        {
            using (var ie = new IE())
            {
                // Clear all cookies.
                ie.ClearCookies();

                // Ensure our test cookies don't exist from a previous run.
                Assert.IsNull(ie.GetCookie("http://1.watin.com/", "test-cookie"));
                Assert.IsNull(ie.GetCookie("http://2.watin.com/", "test-cookie"));

                // Create cookies for a pair of domains.
                ie.SetCookie("http://1.watin.com/", "test-cookie=abc; expires=Wed, 01-Jan-2020 00:00:00 GMT");
                Assert.AreEqual("test-cookie=abc", ie.GetCookie("http://1.watin.com/", "test-cookie"));

                ie.SetCookie("http://2.watin.com/", "test-cookie=def; expires=Wed, 01-Jan-2020 00:00:00 GMT");
                Assert.AreEqual("test-cookie=def", ie.GetCookie("http://2.watin.com/", "test-cookie"));

                CookieCollection collection = ie.GetCookiesForUrl(new Uri("http://2.watin.com/"));
                Assert.AreEqual(1, collection.Count);
                Assert.AreEqual(collection[0].Name, "test-cookie");
                Assert.AreEqual(collection[0].Value, "def");
            }
        }

        [Test]
        public void Should_clear_cookies()
        {
            using (var ie = new IE())
            {
                // Clear all cookies.
                ie.ClearCookies();

                // Ensure our test cookies don't exist from a previous run.
                Assert.IsNull(ie.GetCookie("http://1.watin.com/", "test-cookie"), "Expected no cookies for 1.watin.com");
                Assert.IsNull(ie.GetCookie("http://2.watin.com/", "test-cookie"), "Expected no cookies for 2.watin.com");

                // Create cookies for a pair of domains.
                ie.SetCookie("http://1.watin.com/", "test-cookie=abc; expires=Wed, 01-Jan-2020 00:00:00 GMT");
                ie.SetCookie("http://2.watin.com/", "test-cookie=def; expires=Wed, 01-Jan-2020 00:00:00 GMT");

                // Clear cookies under one subdomain.
                ie.ClearCookies("http://1.watin.com/");

                // Ensure just the cookie of the first subdomain was deleted.
                Assert.IsNull(ie.GetCookie("http://1.watin.com/", "test-cookie"), "Expected no cookies for 1.watin.com after clear");
                Assert.AreEqual("test-cookie=def", ie.GetCookie("http://2.watin.com/", "test-cookie"), "Expected cookie for 2.watin.com");

                // Clear cookies under master domain.
                ie.ClearCookies("http://watin.com/");

                // Ensure the second subdomain's cookie was deleted this time.
                Assert.IsNull(ie.GetCookie("http://2.watin.com/", "test-cookie"), "Expected no cookies for 2.watin.com after ClearCookies");
            }
        }

        [Test, Ignore("Experiencing problems. Test fails and following tests also fail. Need to look into it")]
        public void ClearCache()
        {
            using (var ie = new IE(GoogleUrl))
            {
                // Testing cache clearing directly is a little difficult because we cannot
                // easily enumerate its contents without using system APIs.
                // We could create a sample page that includes a nonce but says it's cacheable
                // forever.  Then we should only observe the change on refresh or cache clearing.
                // Fortunately Google has already done it for us.

                // Save the original page html.
                var oldHtml = GetHtmlSource(ie);

                // If we navigate to the page again, we should see identical Html.
                ie.GoTo(GoogleUrl);
                Assert.AreEqual(oldHtml, GetHtmlSource(ie), "HTML should be identical when pulling from the cache.");

                // But after clearing the cache, things are different.
                ie.ClearCache();
                ie.GoTo(GoogleUrl);
                Assert.AreNotEqual(oldHtml, GetHtmlSource(ie), "HTML should differ after cache has been cleared.");
            }
        }

        private static string GetHtmlSource(Document ie)
        {
            var document = ((IEDocument)ie.NativeDocument).HtmlDocument;
            return document.body.parentElement.outerHTML;
        }

        [Test]
        public void IEExistsByHWND()
        {
            var hwnd = "0";
            Assert.IsFalse(Browser.Exists<IE>(Find.By("hwnd", hwnd)), "hwnd = 0 should not be found");

            using (var ie = new IE(MainURI))
            {
                hwnd = ie.hWnd.ToString();
                Assert.IsTrue(Browser.Exists<IE>(Find.By("hwnd", hwnd)), "hwnd of ie instance should be found");
            }

            Assert.IsFalse(Browser.Exists<IE>(Find.By("hwnd", hwnd)), "hwnd of closed ie instance should not be found");
        }

        [Test]
        public void IEExistsByUrl()
        {
            IEExistsAsserts(Find.ByUrl(MainURI));
        }

        [Test]
        public void IEExistsByTitle()
        {
            IEExistsAsserts(Find.ByTitle("Ai"));
        }

        private static void IEExistsAsserts(Constraint findByUrl)
        {
            Assert.IsFalse(Browser.Exists<IE>(findByUrl));

            using (new IE(MainURI))
            {
                Assert.IsTrue(Browser.Exists<IE>(findByUrl));
            }

            Assert.IsFalse(Browser.Exists<IE>(findByUrl));
        }

        /// <summary>
        /// Attaches to IE with a zero timeout interval. Allthough the timeout
        /// interval is zero the existing IE instance should be found.
        /// </summary>
        [Test]
        public void AttachToIEWithZeroTimeout()
        {
            // Create a new IE instance so we can find it.
            using (new IE(MainURI))
            {
                var startTime = DateTime.Now;
                using (Browser.AttachTo<IE>(Find.ByUrl(MainURI), 0))
                {
                    // Should return (within 1 second).
                    Assert.Greater(1, DateTime.Now.Subtract(startTime).TotalSeconds);
                }
            }
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void AttachToIEWithNegativeTimeoutNotAllowed()
        {
            Browser.AttachTo<IE>(Find.ByTitle("Bogus title"), -1);
        }

        [Test]
        public void AttachToIEByPartialTitle()
        {
            FailIfIEWindowExists("Ai", "AttachToIEByPartialTitle");

            using (new IE(MainURI))
            {
                using (var ieMainByTitle = IE.AttachTo<IE>(Find.ByTitle("Ai")))
                {
                    Assert.AreEqual(MainURI, ieMainByTitle.Uri);
                }
            }
        }

        [Test]
        public void AttachToIEByUrl()
        {
            FailIfIEWindowExists("Ai", "AttachToIEByUrl");

            using (new IE(MainURI))
            {
                using (var ieMainByUri = Browser.AttachTo<IE>(Find.ByUrl(MainURI)))
                {
                    Assert.AreEqual(MainURI, ieMainByUri.Uri);
                }
            }
        }

        [Test]
        public void NewIEClosedByDispose()
        {
            FailIfIEWindowExists("main", "IEClosedByDispose");

            using (new IE(MainURI))
            {
                using (var ie = Browser.AttachTo<IE>(Find.ByTitle("main")))
                {
                    Assert.AreEqual(MainURI, new Uri(ie.Url));
                }
            }

            Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer not closed by IE.Dispose");
        }

        [Test]
        public void IENotFoundException()
        {
            var startTime = DateTime.Now;
            const int timeoutTime = 5;
            const string ieTitle = "Non Existing IE Title";
            const string expectedMessage = "Could not find an IE window matching constraint: Attribute 'title' contains 'Non Existing IE Title' ignoring case. Search expired after '5' seconds.";

            try
            {
                // Time out after timeoutTime seconds
                startTime = DateTime.Now;
                using (Browser.AttachTo<IE>(Find.ByTitle(ieTitle), timeoutTime)) {}

                Assert.Fail(string.Format("Internet Explorer with title '{0}' should not be found", ieTitle));
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(typeof (BrowserNotFoundException), e);
                // add 1 second to give it some slack.
                Assert.Greater(timeoutTime + 1, DateTime.Now.Subtract(startTime).TotalSeconds);
                Console.WriteLine(e.Message);
                Assert.AreEqual(expectedMessage, e.Message, "Unexpected exception message");
            }
        }


        [Test]
        public void NewUriAboutBlank()
        {
            Assert.AreEqual("about:blank", AboutBlank.AbsoluteUri);
        }

        [Test]
        public void CallingIEDisposeAfterIECloseShouldNotThrowAnExeption()
        {
            var ie = new IE();
            ie.Close();
            ie.Dispose();
        }

        [Test, ExpectedException(typeof (ObjectDisposedException))]
        public void CallingIEForceCloseAfterIECloseShouldThrowAnExeption()
        {
            var ie = new IE();
            ie.Close();
            ie.ForceClose();
        }

        /// <summary>
        /// This test addresses a problem in the IE collection where Windows Explorer
        /// windows may get included in the list of shell windows which is a bit
        /// problematic for methods like ForceClose since the test will timeout.
        /// </summary>
        [Test]
        public void IECollectionExcludesWindowsExplorerWindows()
        {
            // Bring up an Explorer window and wait for it to become visible.
            var info = new ProcessStartInfo("explorer.exe") {UseShellExecute = false, CreateNoWindow = true};

            var p = Process.Start(info);

            try
            {
                Thread.Sleep(2000);

                // Create an IE window so we know there's at least one of them.
                new IE();

                // Iterate over internet explorer windows.
                // Previously this would pick up a reference to the Windows Explorer
                // window which would timeout while waiting for the main document to
                // become available.
                Assert.GreaterOrEqual(IE.InternetExplorers().Count, 1);

                foreach (var ie in IE.InternetExplorers())
                    ie.Close();
            }
            finally
            {
                if (p != null)
                    if (! p.HasExited)
                        p.Kill();
            }
        }

        private static void FailIfIEWindowExists(string partialTitle, string testName)
        {
            if (IsIEWindowOpen(partialTitle))
            {
                Assert.Fail(string.Format("An Internet Explorer with '{0}' in it's title already exists. Test '{1}' can't be correctly tested. Close all IE windows and run this test again.", partialTitle, testName));
            }
        }

        private static bool IsIEWindowOpen(string partialTitle)
        {
            // Give windows some time to do work before checking
            Thread.Sleep(1000);

            return Browser.Exists<IE>(Find.ByTitle(partialTitle));
        }

        [Test]
        public void TestWatiNWithInjectedHTMLCode()
        {
            var html = "<HTML><input name=txtSomething><input type=button name=btnSomething value=Click></HTML>";

            using(var ie = new IE())
            {
                var document = ((IEDocument)ie.NativeDocument).HtmlDocument;
                document.writeln(html);

                Assert.That(ie.Button(Find.ByName("btnSomething")).Exists);
            }
        }

        [Test]
        public void ProcessShouldBe_iexplore()
        {
            // GIVEN
            using(var ie = new IE())
            {
                var processId = ie.ProcessID;

                // WHEN
                var process = Process.GetProcessById(processId);

                // THEN
                Assert.That(process.ProcessName, Text.Contains("iexplore"));
            }
        }
        
        [Test, Category("IE8 and later"), Category("InternetConnectionNeeded")]
        public void SettingNoMerge()
        {
            if (IE.GetMajorIEVersion() != 8) return;

            // GIVEN
            Settings.MakeNewIe8InstanceNoMerge = true;

            // url put up by MSDN blogger, may need to add this to our files
            const string testUrl = "http://www.enhanceie.com/test/sessions/";
            using (var ie1 = new IE(testUrl))
            {
                ie1.SelectList(Find.ById("selColor")).Select("Green");

                // WHEN
                using (var ie2 = new IE(testUrl))
                {
                    // THEN
                    var actual = ie2.Div(Find.ById("divSessStore")).InnerHtml;
                    StringAssert.Contains("undefined",actual,"session cookie used");
                }
            }
        }

        [Test, Ignore("Currently attaching to IE which shows PDF in Acrobat Reader is not supported")]
        public void Should_find_browser_with_opened_pdf_document()
        {
            // GIVEN
            using (var ie = new IE(@"C:\Development\watin\trunk\src\UnitTests\html\WatiN.pdf"))
            {
//                var ie1 = IE.AttachToIENoWait(Find.ByTitle("watin.pdf"));
                Assert.That(Browser.Exists<IE>(Find.Any));
            }

            // WHEN


            // THEN

        }

    }
}
