#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using System.Drawing;
using System.Web;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Exceptions;
using WatiN.Core.Native.Windows;
using WatiN.Core.UnitTests.TestUtils;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class BrowserTests : BaseWithBrowserTests
    {
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

        [Test]
        public void AutoMoveMousePointerToTopLeft()
        {
            BrowsersToTestWith.ForEach(manager =>
                                           {
                                               // GIVEN
                                               manager.CloseBrowser();
                                                
                                               var notTopLeftPoint = new Point(50, 50);
                                               Cursor.Position = notTopLeftPoint;
                                               
                                               // This test won't work if mousepointer isn't moved
                                               // Happens if system is locked or remote desktop with no UI
                                               if (Cursor.Position != notTopLeftPoint)
                                                   return;


                                               // WHEN not moving the mousepointer to top left
                                               // when creating a new browser instance
                                               Settings.AutoMoveMousePointerToTopLeft = false;
                                               using (manager.CreateBrowser(TestPageUri))
                                               {
                                                   // THEN cursor should still be on 50,50
                                                   Assert.That(Cursor.Position, Is.EqualTo(notTopLeftPoint));
                                               }

                                               // WHEN we set to the mousepointer to top left
                                               // when creating a new browser instance
                                               Settings.AutoMoveMousePointerToTopLeft = true;
                                               using (manager.CreateBrowser(TestPageUri))
                                               {
                                                   // THEN cursor should be on 0,0
                                                   Assert.That(Cursor.Position, Is.EqualTo(new Point(0, 0)));
                                               }
                                           });
        }

        [Test]
        public void BackAndForward()
        {
            ExecuteTest(browser =>
                            {
                                browser.GoTo(MainURI);
                                Assert.That(MainURI, Is.EqualTo(new Uri(browser.Url)), "Unexpected start Url");

                                browser.Link(Find.ByUrl(IndexURI)).Click();
                                Assert.That(IndexURI, Is.EqualTo(new Uri(browser.Url)), "Unexpected url after clicking on link");

                                var wentBack = browser.Back();
                                Assert.That(MainURI, Is.EqualTo(new Uri(browser.Url)), "Should went back to start Url");
                                Assert.That(wentBack, "Expected went back");

                                var wentFoward = browser.Forward();
                                Assert.That(IndexURI, Is.EqualTo(new Uri(browser.Url)), "Should gone forward to second page");
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

        [Test, Category("InternetConnectionNeeded")]
        public void GoogleFindSearchButtonAndClick()
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
        public void GoToUri()
        {
            ExecuteTest(browser =>
                            {
                                browser.GoTo(MainURI);
                                Assert.AreEqual(MainURI, browser.Uri);
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
                                // GIVEN
                                Assert.That(browser.Url, Is.EqualTo(AboutBlank), "Pre-condition");

                                var url = MainURI.AbsoluteUri;

                                // WHEN
                                browser.GoToNoWait(url);

                                // THEN
                                // This might fail in FireFox, although the check is essentail,
                                // a constant failing test is even worse, so skip it for FireFox
                                if (browser.GetType().IsInstanceOfType(typeof(IE)))
                                    Assert.That(browser.Url, Is.EqualTo(AboutBlank), "Right after GoToNoWait");

                                TryFuncUntilTimeOut.Try(TimeSpan.FromSeconds(3), () => browser.Uri == MainURI);
                                browser.WaitForComplete();

                                Assert.AreEqual(MainURI, new Uri(browser.Url), "Final check");
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
        public void Should_attach_to_browser()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                browser.GoTo(NewWindowUri);
                                browser.Link(Find.First()).Click();

                                // WHEN
                                using (var newWindow = Browser.AttachTo(browser.GetType(), Find.ByTitle("New Window Target Page")))
                                {
                                    // THEN
                                    Assert.That(newWindow.Text.Trim() == "Welcome to the new window.");
                                }
                            });
        }

        [Test]
        public void Should_have_dialogwatcher_set_when_instantiated_by_calling_AttachTo()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                browser.GoTo(NewWindowUri);
                                browser.Link(Find.First()).Click();

                                // WHEN
                                using (var newWindow = Browser.AttachTo(browser.GetType(), Find.ByTitle("New Window Target Page")))
                                {
                                    // THEN
                                    Assert.That(newWindow.DialogWatcher, Is.Not.Null, "Should have a dialogwatcher");
                                    Assert.That(newWindow.DialogWatcher.ReferenceCount, Is.EqualTo(1), "Unexpected dialogwatcher referencecount");
                                }
                            });
        }

        [Test]
        public void Should_not_close_already_open_browser_if_attach_to_doesnt_succeed()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                Assert.That(browser.hWnd, Is.Not.Null, "pre-condition: browser should exist");

                                try
                                {
                                    // WHEN
                                    Browser.AttachTo(browser.GetType(), Find.ByTitle("New Window Target Page"), 3);
                                    Assert.Fail("Should throw BrowserNotFoundException");
                                }
                                catch(BrowserNotFoundException)
                                {
                                    // THEN browser should still exist
                                    Assert.That(browser.hWnd, Is.Not.Null, "browser should still exist");
                                }
                                catch(Exception e)
                                {
                                    Assert.Fail("Unexpected exception: " + e);
                                }
                            });
        }

        [Test]
        public void Should_handle_special_characters()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                browser.GoTo(TheAppUri);

                                var divWithSpecialCharacter = browser.Div(div => div.Text != null && div.Text.Contains("ÅLAND ISLANDS"));

                                // WHEN
                                var exists = divWithSpecialCharacter.Exists;

                                // THEN
                                Assert.That(exists, Is.True);
                            }
                );
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
        public void Should_resize_browser_window()
        {
            ExecuteTest(browser => browser.SizeWindow(800, 600));
        }


        public override Uri TestPageUri
        {
            get { return AboutBlank; }
        }
    }
}
