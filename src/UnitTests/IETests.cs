#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using mshtml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.Core.UnitTests
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
			Settings.Instance.Reset();
		}

		[Test]
		public void TestRunnerApartmentStateMustBeSTA()
		{
#if NET11
			// Code for .Net 1.1
			Assert.IsTrue(Thread.CurrentThread.ApartmentState == ApartmentState.STA);

#else
            // Code for .Net 2.0 and higher
            Assert.IsTrue(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
#endif
		}

		[Test]
		public void IEIsIEAndDomContainer()
		{
			using (IE ie = new IE())
			{
				Assert.IsInstanceOfType(typeof (IE), ie, "Should be an IE instance");
				Assert.IsInstanceOfType(typeof (DomContainer), ie, "Should be a DomContainer instance");
			}
		}

		[Test, Category("InternetConnectionNeeded")]
		public void Google()
		{
			// Instantiate a new DebugLogger to output "user" events to
			// the debug window in VS
			Logger.LogWriter = new DebugLogWriter();

			using (IE ie = new IE(GoogleUrl))
			{
				ie.TextField(Find.ByName("q")).TypeText("WatiN");
				ie.Button(Find.ByName("btnG")).Click();

				Assert.IsTrue(ie.ContainsText("WatiN"));
			}
		}

		[Test, Category("InternetConnectionNeeded")]
		public void GoogleSearchWithEncodedQueryString()
		{
			// Instantiate a new DebugLogger to output "user" events to
			// the debug window in VS
			Logger.LogWriter = new DebugLogWriter();

            string url = string.Format("http://www.google.com/search?q={0}", HttpUtility.UrlEncode("a+b"));

            using (IE ie = new IE(url))
			{
                Assert.That(ie.TextField(Find.ByName("q")).Value, Is.EqualTo("a+b"));
			}
            using (IE ie = new IE())
			{
                ie.GoTo(url);
                Assert.That(ie.TextField(Find.ByName("q")).Value, Is.EqualTo("a+b"));
			}
		}

		[Test, Ignore("Second assert fails in nunit console mode.")]
		public void PressTabAndActiveElement()
		{
			using (IE ie = new IE(MainURI))
			{
				ie.TextField("name").Focus();

				Element element = ie.ActiveElement;
				Assert.AreEqual("name", element.Id);

				ie.PressTab();

				element = ie.ActiveElement;
				Assert.AreEqual("popupid", element.Id);
			}
		}

		[Test, Category("InternetConnectionNeeded")]
		public void AddProtocolToUrlIfOmmitted()
		{
			using (IE ie = new IE("www.google.com"))
			{
				Assert.That(ie.Url, NUnit.Framework.SyntaxHelpers.Text.StartsWith("http://"));
			}
		}

		[Test]
		public void WindowStyle()
		{
			using (IE ie = new IE(MainURI))
			{
				NativeMethods.WindowShowStyle currentStyle = ie.GetWindowStyle();

				ie.ShowWindow(NativeMethods.WindowShowStyle.Maximize);
				Assert.AreEqual(NativeMethods.WindowShowStyle.Maximize.ToString(), ie.GetWindowStyle().ToString(), "Not maximized");

				ie.ShowWindow(NativeMethods.WindowShowStyle.Restore);
				Assert.AreEqual(currentStyle.ToString(), ie.GetWindowStyle().ToString(), "Not Restored");

				ie.ShowWindow(NativeMethods.WindowShowStyle.Minimize);
				Assert.AreEqual(NativeMethods.WindowShowStyle.ShowMinimized.ToString(), ie.GetWindowStyle().ToString(), "Not Minimize");

				ie.ShowWindow(NativeMethods.WindowShowStyle.ShowNormal);
				Assert.AreEqual(NativeMethods.WindowShowStyle.ShowNormal.ToString(), ie.GetWindowStyle().ToString(), "Not ShowNormal");
			}
		}

		[Test]
		public void AutoMoveMousePointerToTopLeft()
		{
			Point notTopLeftPoint = new Point(50, 50);
			Cursor.Position = notTopLeftPoint;
			Settings.Instance.AutoMoveMousePointerToTopLeft = false;

			using (new IE())
			{
				Assert.That(Cursor.Position, NUnit.Framework.SyntaxHelpers.Is.EqualTo(notTopLeftPoint));
			}

			Settings.Instance.AutoMoveMousePointerToTopLeft = true;
			using (new IE())
			{
				Assert.That(Cursor.Position, NUnit.Framework.SyntaxHelpers.Is.EqualTo(new Point(0, 0)));
			}
		}

		[Test]
		public void MakeNewIeInstanceVisible()
		{
			Settings.Instance.MakeNewIeInstanceVisible = true;
			Assert.That(Settings.Instance.MakeNewIeInstanceVisible, "Default should be true");

			using (IE ie = new IE())
			{
				Assert.That(((SHDocVw.InternetExplorer) ie.InternetExplorer).Visible, "IE Should be visible");
			}

			Settings.Instance.MakeNewIeInstanceVisible = false;
			Assert.That(Settings.Instance.MakeNewIeInstanceVisible, NUnit.Framework.SyntaxHelpers.Is.EqualTo(false), "should be false");

			using (IE ie = new IE())
			{
				Assert.That(((SHDocVw.InternetExplorer) ie.InternetExplorer).Visible, NUnit.Framework.SyntaxHelpers.Is.EqualTo(false), "IE Should be visible");
			}
		}

		[Test]
		public void DocumentShouldBeDisposedSoHTMLDialogGetsDisposedAndReferenceCountIsOK()
		{
			DialogWatcher dialogWatcher;
			int ReferenceCount;

			using (IE ie = new IE(MainURI))
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

            string url = MainURI.AbsoluteUri;
			LogonDialogHandler logon = new LogonDialogHandler("y", "z");

			using (IE ie = new IE(url, logon))
			{
				Assert.AreEqual(MainURI, new Uri(ie.Url));
				Assert.IsTrue(ie.DialogWatcher.Contains(logon));
				Assert.AreEqual(1, ie.DialogWatcher.Count);

				using (IE ie1 = new IE(url, logon))
				{
					Assert.AreEqual(MainURI, new Uri(ie1.Url));
					Assert.IsTrue(ie.DialogWatcher.Contains(logon));
					Assert.AreEqual(1, ie.DialogWatcher.Count);
				}
			}

			Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer should be closed by IE.Dispose");
		}


		[Test]
		public void GoToUrl()
		{
			using (IE ie = new IE())
			{
                string url = MainURI.AbsoluteUri;

				ie.GoTo(url);

				Assert.AreEqual(MainURI, new Uri(ie.Url));
			}
		}

		[Test]
		public void GoToUrlNoWait()
		{
			using (IE ie = new IE())
			{
                string url = MainURI.AbsoluteUri;

                ie.GoToNoWait(url);

                Assert.That(ie.Url, Is.EqualTo("about:blank"));
                
                ie.WaitForComplete();
                Assert.AreEqual(MainURI, new Uri(ie.Url));
            }
		}

		[Test]
		public void GoToUri()
		{
			using (IE ie = new IE())
			{
				ie.GoTo(MainURI);
				Assert.AreEqual(MainURI, new Uri(ie.Url));
			}
		}

		[Test]
		public void NewIEInSameProcess()
		{
			using (IE ie1 = new IE())
			{
				using (IE ie2 = new IE())
				{
					Assert.AreEqual(ie1.ProcessID, ie2.ProcessID);
				}
			}
		}

		[Test]
		public void NewIEInNewProcess()
		{
			using (IE ie1 = new IE())
			{
				using (IE ie2 = new IE(true))
				{
					Assert.IsNotNull(ie2, "create ie in new process returned null");
					Assert.AreNotEqual(ie1.ProcessID, ie2.ProcessID, "process id problem");
				}
			}
		}

		[Test]
		public void NewIE()
		{
			using (IE ie = new IE())
			{
				Assert.AreEqual("about:blank", ie.Url);
			}
		}

		[Test]
		public void NewIEWithUri()
		{
			using (IE ie = new IE(MainURI))
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

			using (IE ie = new IE(MainURI))
			{
				Assert.IsTrue(ie.AutoClose);
				ie.AutoClose = false;
			}

			Assert.IsTrue(IsIEWindowOpen("main"), "Internet Explorer should NOT be closed by IE.Dispose");

			IE.AttachToIE(Find.ByTitle("main"), 3).Close();
		}

		[Test]
		public void NewIEWithUrl()
		{
			FailIfIEWindowExists("main", "NewIEWithUrl");

            string url = MainURI.AbsoluteUri;

			using (IE ie = new IE(url))
			{
				Assert.AreEqual(MainURI, new Uri(ie.Url));
				Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");
			}

			Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer should be closed by IE.Dispose");
		}

		[Test]
		public void Reopen()
		{
			FailIfIEWindowExists("main", "Reopen");

            string url = MainURI.AbsoluteUri;
			using (IE ie = new IE(url))
			{
				Assert.AreEqual(MainURI, new Uri(ie.Url));
				object oldIEObj = ie.InternetExplorer;

				ie.Reopen();
				Assert.AreNotSame(oldIEObj, ie.InternetExplorer, "Reopen should create a new browser.");

				Assert.AreEqual("about:blank", ie.Url);
			}
		}

		[Test]
		public void ReopenWithUrlAndLogonDialogHandlerInNewProcess()
		{
			FailIfIEWindowExists("main", "ReopenWithUrlAndLogonDialogHandler");

			LogonDialogHandler logon = new LogonDialogHandler("y", "z");

			using (IE ie1 = new IE())
			{
				using (IE ie2 = new IE())
				{
					Assert.AreEqual(ie1.ProcessID, ie2.ProcessID, "process id problem");

					Assert.AreEqual("about:blank", ie2.Url);
					object oldIEObj = ie2.InternetExplorer;

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
		public void RefreshWithNeverExpiredPage()
		{
			using (IE ie = new IE(MainURI))
			{
				ie.TextField("name").TypeText("refresh test");

				ie.Refresh();

				Assert.AreEqual("refresh test", ie.TextField("name").Text);
			}
		}

		[Test]
		public void Cookies()
		{
			using (IE ie = new IE())
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

				// Clear cookies under one subdomain.
				ie.ClearCookies("http://1.watin.com/");

				// Ensure just the cookie of the first subdomain was deleted.
				Assert.IsNull(ie.GetCookie("http://1.watin.com/", "test-cookie"));
				Assert.AreEqual("test-cookie=def", ie.GetCookie("http://2.watin.com/", "test-cookie"));

				// Clear cookies under master domain.
				ie.ClearCookies("http://watin.com/");

				// Ensure the second subdomain's cookie was deleted this time.
				Assert.IsNull(ie.GetCookie("http://2.watin.com/", "test-cookie"));
			}
		}

		[Test, Ignore("Experiencing problems. Test fails and following tests also fail. Need to look into ti")]
		public void ClearCache()
		{
			using (IE ie = new IE(GoogleUrl))
			{
				// Testing cache clearing directly is a little difficult because we cannot
				// easily enumerate its contents without using system APIs.
				// We could create a sample page that includes a nonce but says it's cacheable
				// forever.  Then we should only observe the change on refresh or cache clearing.
				// Fortunately Google has already done it for us.

				// Save the original page html.
				string oldHtml = GetHtmlSource(ie);

				// If we navigate to the page again, we should see identical Html.
				ie.GoTo(GoogleUrl);
				Assert.AreEqual(oldHtml, GetHtmlSource(ie), "HTML should be identical when pulling from the cache.");

				// But after clearing the cache, things are different.
				ie.ClearCache();
				ie.GoTo(GoogleUrl);
				Assert.AreNotEqual(oldHtml, GetHtmlSource(ie), "HTML should differ after cache has been cleared.");
			}
		}

		private static string GetHtmlSource(IE ie)
		{
			return ie.HtmlDocument.body.parentElement.outerHTML;
		}

		[Test, Category("InternetConnectionNeeded")]
		public void RefreshWithImmediatelyExpiredPage()
		{
			using (IE ie = new IE(GoogleUrl))
			{
				ie.TextField(Find.ByName("q")).TypeText("refresh test");

				ie.Refresh();

				Assert.AreEqual(null, ie.TextField(Find.ByName("q")).Text);
			}
		}

		[Test]
		public void BackAndForward()
		{
			using (IE ie = new IE())
			{
				ie.GoTo(MainURI);
				Assert.AreEqual(MainURI, new Uri(ie.Url));

				ie.Link(Find.ByUrl(IndexURI)).Click();
				Assert.AreEqual(IndexURI, new Uri(ie.Url));

				bool wentBack = ie.Back();
				Assert.AreEqual(MainURI, new Uri(ie.Url));
                Assert.That(wentBack, "Expected went back");

				bool wentFoward = ie.Forward();
				Assert.AreEqual(IndexURI, new Uri(ie.Url));
				Assert.That(wentFoward, "Expected went forward");
			}
		}

        [Test]
		public void BackShouldIgnoreExceptionsWhenNoHistoryIsAvailable()
		{
			using (IE ie = new IE())
			{
			    try
			    {
			        bool wentBack = ie.Back();
                    Assert.That(wentBack, Is.False, "Expected no navigation back");
			    }
			    catch (Exception e)
			    {
			        Assert.Fail("Shouldn't throw exception: " + e);
			    }
			}
		}

        [Test]
		public void ForwardShouldIgnoreExceptionsWhenNoHistoryIsAvailable()
		{
			using (IE ie = new IE())
			{
			    try
			    {
			        bool wentForward = ie.Forward();
                    Assert.That(wentForward, Is.False, "Expected no navigation forward");
                }
			    catch (Exception e)
			    {
			        Assert.Fail("Shouldn't throw exception: " + e);
			    }
			}
		}

		[Test]
		public void IEExistsByHWND()
		{
			string hwnd = "0";
			Assert.IsFalse(IE.Exists(Find.By("hwnd", hwnd)), "hwnd = 0 should not be found");

			using (IE ie = new IE(MainURI))
			{
				hwnd = ie.hWnd.ToString();
				Assert.IsTrue(IE.Exists(Find.By("hwnd", hwnd)), "hwnd of ie instance should be found");
			}

			Assert.IsFalse(IE.Exists(Find.By("hwnd", hwnd)), "hwnd of closed ie instance should not be found");
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

		private static void IEExistsAsserts(BaseConstraint findByUrl)
		{
			Assert.IsFalse(IE.Exists(findByUrl));

			using (new IE(MainURI))
			{
				Assert.IsTrue(IE.Exists(findByUrl));
			}

			Assert.IsFalse(IE.Exists(findByUrl));
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
				DateTime startTime = DateTime.Now;
				using (IE.AttachToIE(Find.ByUrl(MainURI), 0))
				{
					// Should return (within 1 second).
					Assert.Greater(1, DateTime.Now.Subtract(startTime).TotalSeconds);
				}
			}
		}

		[Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void AttachToIEWithNegativeTimeoutNotAllowed()
		{
			IE.AttachToIE(Find.ByTitle("Bogus title"), -1);
		}

		[Test]
		public void AttachToIEByPartialTitle()
		{
			FailIfIEWindowExists("Ai", "AttachToIEByPartialTitle");

			using (new IE(MainURI))
			{
				using (IE ieMainByTitle = IE.AttachToIE(Find.ByTitle("Ai")))
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
				using (IE ieMainByUri = IE.AttachToIE(Find.ByUrl(MainURI)))
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
				using (IE ie = IE.AttachToIE(Find.ByTitle("main")))
				{
					Assert.AreEqual(MainURI, new Uri(ie.Url));
				}
			}

			Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer not closed by IE.Dispose");
		}

		[Test]
		public void IENotFoundException()
		{
			DateTime startTime = DateTime.Now;
			const int timeoutTime = 5;
			const string ieTitle = "Non Existing IE Title";
			const string expectedMessage = "Could not find an IE window matching constraint: Attribute 'title' with value 'non existing ie title'. Search expired after '5' seconds.";

			try
			{
				// Time out after timeoutTime seconds
				startTime = DateTime.Now;
				using (IE.AttachToIE(Find.ByTitle(ieTitle), timeoutTime)) {}

				Assert.Fail(string.Format("Internet Explorer with title '{0}' should not be found", ieTitle));
			}
			catch (Exception e)
			{
				Assert.IsInstanceOfType(typeof (IENotFoundException), e);
				// add 1 second to give it some slack.
				Assert.Greater(timeoutTime + 1, DateTime.Now.Subtract(startTime).TotalSeconds);
				Console.WriteLine(e.Message);
				Assert.AreEqual(expectedMessage, e.Message, "Unexpected exception message");
			}
		}


		[Test]
		public void NewUriAboutBlank()
		{
			Uri uri = new Uri("about:blank");
            Assert.AreEqual("about:blank", uri.AbsoluteUri);
		}

		[Test]
		public void CallingIEDisposeAfterIECloseShouldNotThrowAnExeption()
		{
			IE ie = new IE();
			ie.Close();
			ie.Dispose();
		}

		[Test, ExpectedException(typeof (ObjectDisposedException))]
		public void CallingIEForceCloseAfterIECloseShouldThrowAnExeption()
		{
			IE ie = new IE();
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
			ProcessStartInfo info = new ProcessStartInfo("explorer.exe");
			info.UseShellExecute = false;
			info.CreateNoWindow = true;

			Process p = Process.Start(info);

			try
			{
				Thread.Sleep(2000);

				// Create an IE window so we know there's at least one of them.
				new IE();

				// Iterate over internet explorer windows.
				// Previously this would pick up a reference to the Windows Explorer
				// window which would timeout while waiting for the main document to
				// become available.
				Assert.GreaterOrEqual(IE.InternetExplorers().Length, 1);

				foreach (IE ie in IE.InternetExplorers())
					ie.Close();
			}
			finally
			{
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

			return IE.Exists(Find.ByTitle(partialTitle));
		}

		[Test]
		public void TestWatiNWithInjectedHTMLCode()
		{
			string html = "<HTML><input name=txtSomething><input type=button name=btnSomething value=Click></HTML>";

			using(IE ie = new IE())
			{
				ie.HtmlDocument.writeln(html);

				Assert.That(ie.Button(Find.ByName("btnSomething")).Exists);
			}
		}
	}

	[TestFixture]
	public class NewAttachToIeImplementation
	{

		[Test]
		public void Test()
		{
			ItterateProcesses();
		}

		public void ItterateProcesses()
		{
			IntPtr hWnd = IntPtr.Zero;

			Process[] processes = Process.GetProcesses();
			Console.WriteLine("#processes = " + processes.Length);

			foreach (Process process in processes)
			{

				foreach (ProcessThread t in process.Threads)
				{
					int threadId = t.Id;

					NativeMethods.EnumThreadProc callbackProc = new NativeMethods.EnumThreadProc(EnumChildForTridentDialogFrame);
					NativeMethods.EnumThreadWindows(threadId, callbackProc, hWnd);
				}
			}
		}

		private bool EnumChildForTridentDialogFrame(IntPtr hWnd, IntPtr lParam)
		{
//			Console.WriteLine(NativeMethods.GetClassName(hWnd));

			if (IsIEFrame(hWnd))
			{
				Console.WriteLine("Is IE Window: " + ((long)hWnd).ToString("X"));

				Console.WriteLine(IEDOMFromhWnd(hWnd).title);
			}

			return false;
		}

		internal static IHTMLDocument2 IEDOMFromhWnd(IntPtr hWnd)
		{
			Guid IID_IHTMLDocument2 = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");

			Int32 lRes = 0;
			Int32 lMsg;
			Int32 hr;

			//if (IsIETridentDlgFrame(hWnd))
			//{

			while (!IsIEServerWindow(hWnd))
			{
				// Get 1st child IE server window
				hWnd = NativeMethods.GetChildWindowHwnd(hWnd, "Internet Explorer_Server");
			}

			if (IsIEServerWindow(hWnd))
			{
				// Register the message
				lMsg = NativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");

				// Get the object
				NativeMethods.SendMessageTimeout(hWnd, lMsg, 0, 0, NativeMethods.SMTO_ABORTIFHUNG, 1000, ref lRes);

				if (lRes != 0)
				{
					// Get the object from lRes
					IHTMLDocument2 ieDOMFromhWnd = null;

					hr = NativeMethods.ObjectFromLresult(lRes, ref IID_IHTMLDocument2, 0, ref ieDOMFromhWnd);

					if (hr != 0)
					{
						throw new COMException("ObjectFromLresult has thrown an exception", hr);
					}

					return ieDOMFromhWnd;
				}
			}

			return null;
		}

		internal static bool IsIETridentDlgFrame(IntPtr hWnd)
		{
			return UtilityClass.CompareClassNames(hWnd, "Internet Explorer_TridentDlgFrame");
		}

		internal static bool IsIEFrame(IntPtr hWnd)
		{
			return UtilityClass.CompareClassNames(hWnd, "IEFrame");
		}

		private static bool IsIEServerWindow(IntPtr hWnd)
		{
			return UtilityClass.CompareClassNames(hWnd, "Internet Explorer_Server");
		}

	}


}