#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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

namespace WatiN.Core.UnitTests
{
  using System;
  using System.Drawing;
  using System.IO;
  using System.Threading;
  using System.Windows.Forms;
  using NUnit.Framework;
  using Rhino.Mocks;
  using SHDocVw;
  using WatiN.Core.DialogHandlers;
  using WatiN.Core.Exceptions;
  using WatiN.Core.Interfaces;
  using WatiN.Core.Logging;

  [TestFixture]
  public class IeTests : WatiNTest
  {
    [TestFixtureSetUp]
    public void Setup()
    {
      Logger.LogWriter = new DebugLogWriter();
    }

    [SetUp]
    public void SetUp()
    {
      IE.Settings.Reset();
    }

    [Test]
    public void TestRunnerApartmentStateMustBeSTA()
    {
#if NET11
      // Code for .Net 1.1
      Assert.IsTrue(Thread.CurrentThread.ApartmentState == ApartmentState.STA);

#elif NET20
  // Code for .Net 2.0
      Assert.IsTrue(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
#endif
    }

    [Test, Ignore("Because of timeout issues, run this test manually and not automated"), Category("InternetConnectionNeeded")]
    public void DownloadOpen()
    {
      WatiN.Core.DialogHandlers.FileDownloadHandler dhdl = new WatiN.Core.DialogHandlers.FileDownloadHandler(WatiN.Core.DialogHandlers.FileDownloadOption.Open);

      IE ie = new IE();
      ie.AddDialogHandler(dhdl);
      ie.WaitForComplete();
      ie.GoTo("http://watin.sourceforge.net/WatiNRecorder.zip");

      dhdl.WaitUntilFileDownloadDialogIsHandled(5);
      dhdl.WaitUntilDownloadCompleted(20);
      ie.Close();
    }

    [Test, Ignore("Because of timeout issues, run this test manually and not automated"), Category("InternetConnectionNeeded")]
    public void DownloadSave()
    {
      FileInfo file = new FileInfo(@"c:\temp\test.zip");
      file.Directory.Create();
      file.Delete();

      FileDownloadHandler fileDownloadHandler = new FileDownloadHandler(file.FullName);

      using (IE ie = new IE())
      {
        ie.AddDialogHandler(fileDownloadHandler);

        ie.GoTo("http://watin.sourceforge.net/WatiN-1.0.0.4000-net-1.1.msi");
//        ie.GoTo("http://watin.sourceforge.net/WatiNRecorder.zip");

        fileDownloadHandler.WaitUntilFileDownloadDialogIsHandled(15);
        fileDownloadHandler.WaitUntilDownloadCompleted(200);
      }

      Assert.IsTrue(file.Exists, file.FullName + " file does not exist after download");
    }

    [Test, Ignore("Because of timeout issues, run this test manually and not automated"), Category("InternetConnectionNeeded")]
    public void DownloadRun()
    {
      WatiN.Core.DialogHandlers.FileDownloadHandler dhdl = new WatiN.Core.DialogHandlers.FileDownloadHandler(WatiN.Core.DialogHandlers.FileDownloadOption.Run);
      IE ie = new IE();
      ie.AddDialogHandler(dhdl);
      ie.WaitForComplete();
      ie.GoTo("http://watin.sourceforge.net/WatiN-1.0.0.4000-net-1.1.msi");

      dhdl.WaitUntilFileDownloadDialogIsHandled(5);
      dhdl.WaitUntilDownloadCompleted(20);
      ie.Close();
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
      using(IE ie = new IE("www.google.com"))
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
      IE.Settings.AutoMoveMousePointerToTopLeft = false;

      using(new IE())
      {
        Assert.That(Cursor.Position, NUnit.Framework.SyntaxHelpers.Is.EqualTo(notTopLeftPoint));
      }

      IE.Settings.Reset();
      using(new IE())
      {
        Assert.That(Cursor.Position, NUnit.Framework.SyntaxHelpers.Is.EqualTo(new Point(0,0)));
      }
    }

    [Test]
    public void HTMLDialogModeless()
    {
      using (IE ie = new IE(MainURI))
      {
        ie.Button("popupid").Click();
        using (Document dialog = ie.HtmlDialogs[0])
        {
          string value = dialog.TextField("dims").Value;
          Assert.AreEqual("47", value);
        }
      }
    }

    [Test]
    public void AlertAndConfirmDialogHandler()
    {
      DialogWatcher dialogWatcher;

      using (IE ie = new IE(MainURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero before test");

        // Create handler for Alert dialogs and register it.
        AlertAndConfirmDialogHandler dialogHandler = new AlertAndConfirmDialogHandler();
        using (new UseDialogOnce(ie.DialogWatcher, dialogHandler))
        {
          Assert.AreEqual(0, dialogHandler.Count);

          ie.Button("helloid").Click();

          Assert.AreEqual(1, dialogHandler.Count);
          Assert.AreEqual("hello", dialogHandler.Alerts[0]);

          // getting alert text
          Assert.AreEqual("hello", dialogHandler.Pop());

          Assert.AreEqual(0, dialogHandler.Count);

          // Test Clear
          ie.Button("helloid").Click();

          Assert.AreEqual(1, dialogHandler.Count);

          dialogHandler.Clear();

          Assert.AreEqual(0, dialogHandler.Count);

          dialogWatcher = ie.DialogWatcher;
        }
      }

      Assert.AreEqual(0, dialogWatcher.Count, "DialogWatcher count should be zero after test");
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

        using (Document document = ie.HtmlDialogs[0])
        {
          Assert.AreEqual(ReferenceCount + 1, ie.DialogWatcher.ReferenceCount, "DialogWatcher reference count");
        }

        dialogWatcher = ie.DialogWatcher;
      }

      Assert.AreEqual(ReferenceCount - 1, dialogWatcher.ReferenceCount, "DialogWatcher reference count should be zero after test");
    }

    [Test, ExpectedException(typeof (MissingAlertException))]
    public void MissingAlertException()
    {
      using (IE ie = new IE(MainURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero before test");

        AlertAndConfirmDialogHandler dialogHandler = new AlertAndConfirmDialogHandler();
        using (new UseDialogOnce(ie.DialogWatcher, dialogHandler))
        {
          dialogHandler.Pop();
        }
      }
    }

    [Test, Ignore()]
    public void LogonDialogTest()
    {
      using (IE ie = new IE())
      {
        LogonDialogHandler logonDialogHandler = new LogonDialogHandler(@"username", "password");
        using (new UseDialogOnce(ie.DialogWatcher, logonDialogHandler))
        {
          ie.GoTo("https://www.somesecuresite.com");
        }
      }
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void LogonDialogWithUserNameNullShouldThrowArgumentNullException()
    {
      new LogonDialogHandler(null, "pwd");
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void LogonDialogWithUserNameStringEmptyShouldThrowArgumentNullException()
    {
      new LogonDialogHandler(String.Empty, "pwd");
    }

    [Test]
    public void LogonDialogValidConstructorArguments()
    {
      new LogonDialogHandler("username", "pwd");
      new LogonDialogHandler("username", "");
      new LogonDialogHandler("username", null);
    }

    //    [Test]
    //    public void DialogTestSpike3()
    //    {
    //      IE ie = new IE("http://www.ergens.nl");
    //      
    //      ie.ExpectConfirmDialog;
    //      
    //      ie.Button(Find.ByText("Show confirm dialog")).ClickNoWait();
    //      
    //      ConfirmDialog confirmDialog = ie.ConfirmDialog;
    //      Assert.AreEqual("Microsoft Internet Explorer", confirmDialog.Title);
    //      Assert.AreEqual("This is a message.", confirmDialog.Message);
    //      
    //      confirmDialog.OKButton.Click();
    //      
    //      ie.Close();
    //    }

    [Test]
    public void AlertDialogHandler()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

        AlertDialogHandler alertDialogHandler = new AlertDialogHandler();
        using (new UseDialogOnce(ie.DialogWatcher, alertDialogHandler))
        {
          ie.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

          alertDialogHandler.WaitUntilExists();

          string message = alertDialogHandler.Message;
          alertDialogHandler.OKButton.Click();

          ie.WaitForComplete();

          Assert.AreEqual("This is an alert!", message, "Unexpected message");
          Assert.IsFalse(alertDialogHandler.Exists(), "Alert Dialog should be closed.");
        }
      }
    }

    [Test]
    public void AlertDialogSimpleJavaDialogHandler()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

        SimpleJavaDialogHandler dialogHandler = new SimpleJavaDialogHandler();

        Assert.IsFalse(dialogHandler.HasHandledDialog, "Alert Dialog should not be handled.");
        Assert.IsNull(dialogHandler.Message, "Message should be null");

        using (new UseDialogOnce(ie.DialogWatcher, dialogHandler))
        {
          ie.Button(Find.ByValue("Show alert dialog")).Click();

          Assert.IsTrue(dialogHandler.HasHandledDialog, "Alert Dialog should be handled.");
          Assert.AreEqual("This is an alert!", dialogHandler.Message, "Unexpected message");
        }
      }
    }

    [Test]
    public void AlertDialogSimpleJavaDialogHandler2()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        SimpleJavaDialogHandler dialogHandler = new SimpleJavaDialogHandler();

        using (new UseDialogOnce(ie.DialogWatcher, dialogHandler))
        {
          ie.Button(Find.ByValue("Show alert dialog")).Click();

          Assert.AreEqual("This is an alert!", dialogHandler.Message, "Unexpected message");
        }
      }
    }

    [Test]
    public void IEUseOnceDialogHandler()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

        SimpleJavaDialogHandler dialogHandler = new SimpleJavaDialogHandler();

        using (new UseDialogOnce(ie.DialogWatcher, dialogHandler))
        {
          ie.Button(Find.ByValue("Show alert dialog")).Click();

          Assert.IsTrue(dialogHandler.HasHandledDialog, "Alert Dialog should be handled.");
          Assert.AreEqual("This is an alert!", dialogHandler.Message, "Unexpected message");
        }
      }
    }

    [Test]
    public void AlertDialogHandlerWithoutAutoCloseDialogs()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

        ie.DialogWatcher.CloseUnhandledDialogs = false;

        ie.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

        AlertDialogHandler alertDialogHandler = new AlertDialogHandler();

        using (new UseDialogOnce(ie.DialogWatcher, alertDialogHandler))
        {
          alertDialogHandler.WaitUntilExists();

          string message = alertDialogHandler.Message;
          alertDialogHandler.OKButton.Click();

          ie.WaitForComplete();

          Assert.AreEqual("This is an alert!", message, "Unexpected message");
          Assert.IsFalse(alertDialogHandler.Exists(), "Alert Dialog should be closed.");
        }
      }
    }

    [Test]
    public void ConfirmDialogHandlerOK()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

        ConfirmDialogHandler confirmDialogHandler = new ConfirmDialogHandler();

        using (new UseDialogOnce(ie.DialogWatcher, confirmDialogHandler))
        {
          ie.Button(Find.ByValue("Show confirm dialog")).ClickNoWait();

          confirmDialogHandler.WaitUntilExists();

          string message = confirmDialogHandler.Message;
          confirmDialogHandler.OKButton.Click();

          ie.WaitForComplete();

          Assert.AreEqual("Do you want to do xyz?", message, "Unexpected message");
          Assert.AreEqual("OK", ie.TextField("ReportConfirmResult").Text, "OK button expected.");
        }
      }
    }

    [Test]
    public void ConfirmDialogHandlerCancel()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

        ConfirmDialogHandler confirmDialogHandler = new ConfirmDialogHandler();

        using (new UseDialogOnce(ie.DialogWatcher, confirmDialogHandler))
        {
          ie.Button(Find.ByValue("Show confirm dialog")).ClickNoWait();

          confirmDialogHandler.WaitUntilExists();

          string message = confirmDialogHandler.Message;
          confirmDialogHandler.CancelButton.Click();

          ie.WaitForComplete();

          Assert.AreEqual("Do you want to do xyz?", message, "Unexpected message");
          Assert.AreEqual("Cancel", ie.TextField("ReportConfirmResult").Text, "Cancel button expected.");
        }
      }
    }

    [Test]
    public void ConfirmDialogSimpleJavaDialogHandlerCancel()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

        SimpleJavaDialogHandler dialogHandler = new SimpleJavaDialogHandler(true);
        using (new UseDialogOnce(ie.DialogWatcher, dialogHandler))
        {
          ie.Button(Find.ByValue("Show confirm dialog")).Click();

          Assert.IsTrue(dialogHandler.HasHandledDialog, "Confirm Dialog should be handled.");
          Assert.AreEqual("Do you want to do xyz?", dialogHandler.Message);
          Assert.AreEqual("Cancel", ie.TextField("ReportConfirmResult").Text, "Cancel button expected.");
        }
      }
    }

    [Test]
    public void NewIEWithUrlAndLogonDialogHandler()
    {
      FailIfIEWindowExists("main", "NewIEWithUrlAndLogonDialogHandler");

      string url = MainURI.ToString();
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
    public void DocumentUrlandUri()
    {
      string url = MainURI.ToString();

      using (IE ie = new IE(url))
      {
        Uri uri = new Uri(ie.Url);
        Assert.AreEqual(MainURI, uri);
        Assert.AreEqual(ie.Uri, uri);
      }
    }

    [Test]
    public void GoToUrl()
    {
      using (IE ie = new IE())
      {
        string url = MainURI.ToString();

        ie.GoTo(url);

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

      using (new IE(MainURI))
      {
      }

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

      string url = MainURI.ToString();

      using (IE ie = new IE(url))
      {
        Assert.AreEqual(MainURI, new Uri(ie.Url));
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");
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

        ie.Back();
        Assert.AreEqual(MainURI, new Uri(ie.Url));

        ie.Forward();
        Assert.AreEqual(IndexURI, new Uri(ie.Url));
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

    private static void IEExistsAsserts(Core.Attribute findByUrl)
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
        using (IE.AttachToIE(new Url(MainURI), 0))
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
      const string expectedMessage = "Could not find an IE window by title with value '" + ieTitle + "'. (Search expired after '5' seconds)";

      try
      {
        // Time out after timeoutTime seconds
        startTime = DateTime.Now;
        using (IE.AttachToIE(Find.ByTitle(ieTitle), timeoutTime))
        {
        }

        Assert.Fail(string.Format("Internet Explorer with title '{0}' should not be found", ieTitle));
      }
      catch (Exception e)
      {
        Assert.IsInstanceOfType(typeof (IENotFoundException), e);
        // add 1 second to give it some slack.
        Assert.Greater(timeoutTime + 1, DateTime.Now.Subtract(startTime).TotalSeconds);
        Assert.AreEqual(expectedMessage, e.Message, "Unexpected exception message");
      }
    }

    [Test]
    public void HTMLDialogModalByTitle()
    {
      using (IE ie = new IE(MainURI))
      {
        ie.Button("modalid").ClickNoWait();

        using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest")))
        {
          Assert.IsInstanceOfType(typeof (DomContainer), htmlDialog);

          Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
          Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");

          htmlDialog.TextField("name").TypeText("Textfield in HTMLDialog");
          htmlDialog.Button("hello").Click();
        }
      }
    }

    [Test]
    public void HTMLDialogModalByUrl()
    {
      using (IE ie = new IE(MainURI))
      {
        ie.Button("modalid").ClickNoWait();

        using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByUrl(PopUpURI)))
        {
          Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
          Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");
        }
      }
    }

    [Test]
    public void HTMLDialogsExists()
    {
      using (IE ie = new IE(MainURI))
      {
        Url findBy = Find.ByUrl(PopUpURI);
        Assert.IsFalse(ie.HtmlDialogs.Exists(findBy));

        ie.Button("modalid").ClickNoWait();

        Thread.Sleep(1000);

        Assert.IsTrue(ie.HtmlDialogs.Exists(findBy));
      }
    }

    [Test]
    public void HTMLDialogNotFoundException()
    {
      using (IE ie = new IE(MainURI))
      {
        DateTime startTime = DateTime.Now;
        const int timeoutTime = 5;
        string expectedMessage = "Could not find a HTMLDialog by title with value 'PopUpTest'. (Search expired after '5' seconds)";

        try
        {
          // Time out after timeoutTime seconds
          startTime = DateTime.Now;
          ie.HtmlDialog(Find.ByTitle("PopUpTest"), timeoutTime);
          Assert.Fail("PopUpTest should not be found");
        }
        catch (Exception e)
        {
          Assert.IsInstanceOfType(typeof (HtmlDialogNotFoundException), e);
          // add 1 second to give it some slack.
          Assert.Greater(timeoutTime + 1, DateTime.Now.Subtract(startTime).TotalSeconds);
          Assert.AreEqual(expectedMessage, e.Message, "Unexpected exception message");
        }
      }
    }

    [Test]
    public void NewUriAboutBlank()
    {
      Uri uri = new Uri("about:blank");
      Assert.AreEqual("about:blank", uri.ToString());
    }

    [Test]
    public void FireKeyDownEventOnElementWithNoId()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        TextField report = ie.TextField("Report");
        Core.Button button = ie.Button(Find.ByValue("Button without id"));

        Assert.IsNull(button.Id, "Button id not null before click event");
        Assert.IsNull(report.Text, "Report not empty");

        button.KeyDown();

        Assert.IsNotNull(report.Text, "No keydown event fired (report is empty )");
        Assert.AreEqual("button.id = ", report.Text, "Report should start with 'button.id = '");

        Assert.IsNull(button.Id, "Button id not null after click event");
      }
    }

    [Test]
    public void FireEventAlwaysSetsLeftMouseOnEventObject()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        // test in standard IE window
        ie.Button(Find.ByValue("Button without id")).KeyDown();

        Assert.AreEqual("1", ie.TextField("eventButtonValue").Value, "Event.button not left");

        // test in HTMLDialog window
        ie.Button("modalid").ClickNoWait();

        using (HtmlDialog htmlDialog = ie.HtmlDialogs[0])
        {
          htmlDialog.Button(Find.ByValue("Button without id")).KeyDown();

          Assert.AreEqual("1", htmlDialog.TextField("eventButtonValue").Value, "Event.button not left on modal dialog");
        }
      }
    }

    [Test]
    public void FireEventAlwaysSetsSrcElementOnEventObject()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        // test in standard IE window
        ie.Button(Find.ByValue("Button without id")).KeyDown();

        Assert.AreEqual("Button without id", ie.TextField("eventScrElement").Value, "Unexpected Event.scrElement.value");

        // test in HTMLDialog window
        ie.Button("modalid").ClickNoWait();

        using (HtmlDialog htmlDialog = ie.HtmlDialogs[0])
        {
          htmlDialog.Button(Find.ByValue("Button without id")).KeyDown();

          Assert.AreEqual("Button without id", htmlDialog.TextField("eventScrElement").Value, "Unexpected Event.scrElement.value");
        }
      }
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

    [Test, Category("InternetConnectionNeeded")]
    public void SecurityAlertDialogHandler()
    {
      SecurityAlertDialogHandlerMock securityAlertDialogHandlerMock = new SecurityAlertDialogHandlerMock();

      using (IE ie = new IENoWaitForComplete("http://sourceforge.net"))
      {
        ie.AddDialogHandler(securityAlertDialogHandlerMock);
        ie.Link(Find.ByText("Log in")).Click();
        
        ie.TextField(Find.ByName("form_loginname")).WaitUntilExists();

        Assert.IsTrue(securityAlertDialogHandlerMock.HasHandledSecurityAlertDialog);
      }
    }

    private class IENoWaitForComplete : IE
    {
      public IENoWaitForComplete(string url) : base(url) {}

      public override void WaitForComplete()
      {
        // Skip Wait logic
      }
    }

    private class SecurityAlertDialogHandlerMock : SecurityAlertDialogHandler
    {
      private bool _hasHandledSecurityAlertDialog;

      public bool HasHandledSecurityAlertDialog
      {
        get { return _hasHandledSecurityAlertDialog; }
      }

      public override bool HandleDialog(Window window)
      {
        bool handled = base.HandleDialog(window);

        if (handled && !HasHandledSecurityAlertDialog)
        {
          _hasHandledSecurityAlertDialog = true;
        }
        return handled;
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
  }


  [TestFixture]
  public class DialogWatcherTest : WatiNTest
  {
    private ILogWriter saveLoggerInstance;

    [SetUp]
    public void SetUp()
    {
      saveLoggerInstance = Logger.LogWriter;
    }

    [TearDown]
    public void TearDown()
    {
      Logger.LogWriter = saveLoggerInstance;
    }

    [Test]
    public void DialogWatcherShouldKeepRunningWhenClosingOneOfTwoInstancesInSameProcess()
    {
      using (IE ie1 = new IE(), ie2 = new IE())
      {
        Assert.AreEqual(ie1.ProcessID, ie2.ProcessID, "Processids should be the same");

        DialogWatcher dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie1.ProcessID);

        Assert.IsNotNull(dialogWatcher, "dialogWatcher should not be null");
        Assert.AreEqual(ie1.ProcessID, dialogWatcher.ProcessId, "Processids of ie1 and dialogWatcher should be the same");
        Assert.AreEqual(2, dialogWatcher.ReferenceCount, "Expected 2 as reference count");
        Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist");
        Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should be running");

        ie2.Close();

        Assert.AreEqual(1, dialogWatcher.ReferenceCount, "Expected 1 as reference count");
        Assert.IsTrue(dialogWatcher.ProcessExists, "Process should still exist");
        Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should still be running");

        ie1.Close();

        Assert.AreEqual(0, dialogWatcher.ReferenceCount, "Expected 0 as reference count");
        Assert.IsFalse(dialogWatcher.IsRunning, "dialogWatcher should not be running");
      }
    }

    [Test]
    public void DialogWatcherShouldTerminateWhenNoWatiNCoreIEInstancesExistButProcessDoesExist()
    {
      int ieProcessId;

      // Create running Internet Explorer instance but no longer referenced by 
      // an instance of WatiN.Core.IE.
      using (IE ie = new IE(MainURI))
      {
        ie.AutoClose = false;
        ieProcessId = ie.ProcessID;
      }

      // Create IE instances and see if DialogWatcher behaves as expected
      using (IE ie1 = new IE())
      {
        Assert.AreEqual(ieProcessId, ie1.ProcessID, "ProcessIds ie and ie1 should be the same");

        using (IE ie2 = new IE())
        {
          Assert.AreEqual(ie1.ProcessID, ie2.ProcessID, "Processids ie1 and ie2 should be the same");
        }

        DialogWatcher dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie1.ProcessID);

        Assert.IsNotNull(dialogWatcher, "dialogWatcher should not be null");
        Assert.AreEqual(ie1.ProcessID, dialogWatcher.ProcessId, "Processids of ie1 and dialogWatcher should be the same");
        Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist");
        Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should be running");

        ie1.Close();

        Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist after ie1.close");
        Assert.IsFalse(dialogWatcher.IsRunning, "dialogWatcher should not be running");
      }

      // Find created but not referenced Internet Explorer instance and close it.
      IE.AttachToIE(Find.ByUrl(MainURI)).Close();
    }

    [Test]
    public void DialogWatcherOfIEAndHTMLDialogShouldNotBeNull()
    {
      using (IE ie = new IE(MainURI))
      {
        Assert.IsNotNull(ie.DialogWatcher, "ie.DialogWatcher should not be null");

        ie.Button("modalid").ClickNoWait();

        using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest")))
        {
          Assert.IsNotNull(htmlDialog.DialogWatcher, "htmlDialog.DialogWatcher should not be null");
        }
      }
    }

    [Test]
    public void DialogWatcherShouldKeepRunningWhenClosingHTMLDialog()
    {
      using (IE ie = new IE(MainURI))
      {
        ie.Button("modalid").ClickNoWait();

        DialogWatcher dialogWatcher;
        using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest")))
        {
          Assert.AreEqual(ie.ProcessID, htmlDialog.ProcessID, "Processids should be the same");

          dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie.ProcessID);
          Assert.IsNotNull(dialogWatcher, "dialogWatcher should not be null");
          Assert.AreEqual(ie.ProcessID, dialogWatcher.ProcessId, "Processids of ie and dialogWatcher should be the same");
          Assert.AreEqual(2, dialogWatcher.ReferenceCount, "Expected 2 as reference count");
          Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist");
          Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should be running");
        }

        Assert.AreEqual(1, dialogWatcher.ReferenceCount, "Expected 1 as reference count");
        Assert.IsTrue(dialogWatcher.ProcessExists, "Process should still exist");
        Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should still be running");

        ie.WaitForComplete();
        ie.Close();

        Assert.AreEqual(0, dialogWatcher.ReferenceCount, "Expected 0 as reference count");
        Assert.IsFalse(dialogWatcher.IsRunning, "dialogWatcher should not be running");
      }
    }

    [Test]
    public void ThrowReferenceCountException()
    {
      using (IE ie = new IE())
      {
        DialogWatcher dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie.ProcessID);
        Assert.AreEqual(1, dialogWatcher.ReferenceCount);

        dialogWatcher.DecreaseReferenceCount();

        Assert.AreEqual(0, dialogWatcher.ReferenceCount);

        try
        {
          dialogWatcher.DecreaseReferenceCount();
          Assert.Fail("ReferenceCountException expected");
        }
        catch (ReferenceCountException)
        {
        }
        catch
        {
          Assert.Fail("ReferenceCountException expected");
        }
        finally
        {
          dialogWatcher.IncreaseReferenceCount();
        }
      }
    }

    [Test]
    public void ExceptionsInDialogHandlersShouldBeLoggedAndNeglected()
    {
      MockRepository mocks = new MockRepository();

      //Make the mocks
      ILogWriter mockLogWriter = (ILogWriter) mocks.CreateMock(typeof (ILogWriter));
      IDialogHandler buggyDialogHandler = (IDialogHandler) mocks.CreateMock(typeof (IDialogHandler));
      IDialogHandler nextDialogHandler = (IDialogHandler) mocks.CreateMock(typeof (IDialogHandler));
      Window dialog = (Window) mocks.DynamicMock(typeof (Window), IntPtr.Zero);

      // Handle window does check if window IsDialog and Visible
      Expect.Call(dialog.IsDialog()).Return(true);
      Expect.Call(dialog.Visible).Return(true);

      // If this HandleDialog is called throw an exception
      Expect.Call(buggyDialogHandler.HandleDialog(dialog)).Throw(new Exception());
      // Expect Logger will be called with the exception text and stack trace
      mockLogWriter.LogAction("");
      LastCall.Constraints(Rhino.Mocks.Text.Like("Exception was thrown while DialogWatcher called HandleDialog:"));
      mockLogWriter.LogAction("");
      LastCall.Constraints(Rhino.Mocks.Text.StartsWith("System.Exception:"));
      // Expect the next dialogHandler will be called even do an exception
      // has been thrown by the previous handler
      Expect.Call(nextDialogHandler.HandleDialog(dialog)).Return(true);

      mocks.ReplayAll();

      // Set Logger
      Logger.LogWriter = mockLogWriter;

      // Add dialogHandlers
      DialogWatcher dialogWatcher = new DialogWatcher(0);
      dialogWatcher.Add(buggyDialogHandler);
      dialogWatcher.Add(nextDialogHandler);

      Assert.IsNull(dialogWatcher.LastException, "LastException should be null");

      // Call HandleDialog
      dialogWatcher.HandleWindow(dialog);

      Assert.IsNotNull(dialogWatcher.LastException, "LastException should not be null");

      mocks.VerifyAll();
    }

    [Test]
    public void StartingDialogWatcherShouldAdhereToSetting()
    {
      IE.Settings.Reset();

      Assert.That(IE.Settings.AutoStartDialogWatcher, "Unexpected value for AutoStartDialogWatcher");

      IE.Settings.AutoStartDialogWatcher = false;
      using (IE ie = new IE())
      {
        Assert.That(ie.DialogWatcher, NUnit.Framework.SyntaxHelpers.Is.Null);
      }
    }
  }

  [TestFixture]
  public class HTMLDialogFindByTests : WatiNTest
  {
    private IE ie = new IE(MainURI);

    [TestFixtureSetUp]
    public void FixtureSetUp()
    {
      ie.Button("modalid").ClickNoWait();
    }

    [TestFixtureTearDown]
    public void FixtureTearDown()
    {
      foreach (HtmlDialog dialog in ie.HtmlDialogs)
      {
        dialog.Close();
      }

      ie.WaitForComplete();
      ie.Close();
    }

    [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
    public void HTMLDialogGettingWithNegativeTimeoutNotAllowed()
    {
      ie.HtmlDialog(Find.ByUrl(PopUpURI), -1);
    }

    [Test]
    public void HTMLDialogFindByTitle()
    {
      AssertHTMLDialog(ie.HtmlDialog(Find.ByTitle("PopUpTest")));
    }

    [Test]
    public void HTMLDialogFindByUrl()
    {
      AssertHTMLDialog(ie.HtmlDialog(Find.ByUrl(PopUpURI)));
    }

    [Test]
    public void HTMLDialogFindByTitleAndWithTimeout()
    {
      AssertHTMLDialog(ie.HtmlDialog(Find.ByTitle("PopUpTest"), 10));
    }

    [Test]
    public void HTMLDialogFindByUrlAndWithTimeout()
    {
      AssertHTMLDialog(ie.HtmlDialog(Find.ByUrl(PopUpURI), 10));
    }

    private static void AssertHTMLDialog(HtmlDialog htmlDialog)
    {
      Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
      Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");
    }
  }

  public class UseDialogOnce : IDisposable
  {
    private DialogWatcher dialogWatcher;
    private IDialogHandler dialogHandler;

    public UseDialogOnce(DialogWatcher dialogWatcher, IDialogHandler dialogHandler)
    {
      if (dialogWatcher == null)
      {
        throw new ArgumentNullException("dialogWatcher");
      }

      if (dialogHandler == null)
      {
        throw new ArgumentNullException("dialogHandler");
      }

      this.dialogWatcher = dialogWatcher;
      this.dialogHandler = dialogHandler;

      dialogWatcher.Add(dialogHandler);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
      return;
    }

    protected virtual void Dispose(bool managedAndNative)
    {
      dialogWatcher.Remove(dialogHandler);

      dialogWatcher = null;
      dialogHandler = null;
    }
  }

  [TestFixture]
  public class SettingsTests
  {
    [Test]
    public void Properties()
    {
      Settings settings = new Settings();

      settings.AttachToIETimeOut = 111;
      bool autoCloseDialogs = !settings.AutoCloseDialogs;
      settings.AutoCloseDialogs = autoCloseDialogs;
      settings.HighLightColor = "strange color";
      bool highLightElement = !settings.HighLightElement;
      settings.HighLightElement = highLightElement;
      settings.WaitForCompleteTimeOut = 222;
      settings.WaitUntilExistsTimeOut = 333;

      Assert.AreEqual(111, settings.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
      Assert.AreEqual(autoCloseDialogs, settings.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
      Assert.AreEqual("strange color", settings.HighLightColor, "Unexpected HighLightColor");
      Assert.AreEqual(highLightElement, settings.HighLightElement, "Unexpected HighLightElement");
      Assert.AreEqual(222, settings.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
      Assert.AreEqual(333, settings.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
    }

    [Test]
    public void Clone()
    {
      Settings settings = new Settings();

      settings.AttachToIETimeOut = 111;
      bool autoCloseDialogs = !settings.AutoCloseDialogs;
      settings.AutoCloseDialogs = autoCloseDialogs;
      settings.HighLightColor = "strange color";
      bool highLightElement = !settings.HighLightElement;
      settings.HighLightElement = highLightElement;
      settings.WaitForCompleteTimeOut = 222;
      settings.WaitUntilExistsTimeOut = 333;

      Settings settingsClone = settings.Clone();
      Assert.AreEqual(111, settingsClone.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
      Assert.AreEqual(autoCloseDialogs, settingsClone.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
      Assert.AreEqual("strange color", settingsClone.HighLightColor, "Unexpected HighLightColor");
      Assert.AreEqual(highLightElement, settingsClone.HighLightElement, "Unexpected HighLightElement");
      Assert.AreEqual(222, settingsClone.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
      Assert.AreEqual(333, settingsClone.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
    }

    [Test]
    public void Defaults()
    {
      Settings settings = new Settings();

      AssertDefaults(settings);
    }

    [Test]
    public void Reset()
    {
      Settings settings = new Settings();

      settings.AttachToIETimeOut = 111;
      bool autoCloseDialogs = !settings.AutoCloseDialogs;
      settings.AutoCloseDialogs = autoCloseDialogs;
      settings.HighLightColor = "strange color";
      bool highLightElement = !settings.HighLightElement;
      settings.HighLightElement = highLightElement;
      settings.WaitForCompleteTimeOut = 222;
      settings.WaitUntilExistsTimeOut = 333;

      Settings settingsClone = settings.Clone();
      Assert.AreEqual(111, settingsClone.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
      Assert.AreEqual(autoCloseDialogs, settingsClone.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
      Assert.AreEqual("strange color", settingsClone.HighLightColor, "Unexpected HighLightColor");
      Assert.AreEqual(highLightElement, settingsClone.HighLightElement, "Unexpected HighLightElement");
      Assert.AreEqual(222, settingsClone.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
      Assert.AreEqual(333, settingsClone.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");

      settingsClone.Reset();
      AssertDefaults(settingsClone);
    }

    [Test]
    public void ChangeSettingInCloneShouldNotChangeOriginalSetting()
    {
      Settings settings = new Settings();

      settings.AttachToIETimeOut = 111;

      Settings settingsClone = settings.Clone();
      Assert.AreEqual(111, settingsClone.AttachToIETimeOut, "Unexpected clone 1");

      settingsClone.AttachToIETimeOut = 222;

      Assert.AreEqual(111, settings.AttachToIETimeOut, "Unexpected original");
      Assert.AreEqual(222, settingsClone.AttachToIETimeOut, "Unexpected clone 2");
    }

    private static void AssertDefaults(Settings settings)
    {
      Assert.AreEqual(30, settings.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
      Assert.AreEqual(true, settings.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
      Assert.AreEqual("yellow", settings.HighLightColor, "Unexpected HighLightColor");
      Assert.AreEqual(true, settings.HighLightElement, "Unexpected HighLightElement");
      Assert.AreEqual(30, settings.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
      Assert.AreEqual(30, settings.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void IESettingsSetToNullShouldThrowArgumentNullException()
    {
      IE.Settings = null;
    }

    [Test]
    public void SetIESettings()
    {
      Settings settings = new Settings();
      Assert.AreNotEqual(111, IE.Settings.AttachToIETimeOut);
      settings.AttachToIETimeOut = 111;

      IE.Settings = settings;
      Assert.AreEqual(111, IE.Settings.AttachToIETimeOut);
    }
  }

  [TestFixture]
  public class LoggerTests
  {
    private const string LogMessage = "Call LogAction on mock";

    private MockRepository mocks;
    private ILogWriter mockLogWriter;
    private ILogWriter originalLogWriter;

    [TestFixtureSetUp]
    public void FixtureSetUp()
    {
      originalLogWriter = Logger.LogWriter;
    }

    [TestFixtureTearDown]
    public void FixtureTearDown()
    {
      Logger.LogWriter = originalLogWriter;
    }

    [SetUp]
    public void SetUp()
    {
      mocks = new MockRepository();
      mockLogWriter = (ILogWriter) mocks.CreateMock(typeof (ILogWriter));
    }

    [Test]
    public void SettingLogWriterToNullShouldReturnNoLogClass()
    {
      Logger.LogWriter = null;
      Assert.IsInstanceOfType(typeof (NoLog), Logger.LogWriter);
    }

    [Test]
    public void SettingLogWriterShouldReturnThatLogWriter()
    {
      Logger.LogWriter = new DebugLogWriter();
      Assert.IsInstanceOfType(typeof (DebugLogWriter), Logger.LogWriter);
    }

    [Test]
    public void LogActionShouldCallLogActionOnLogWriterInstance()
    {
      mockLogWriter.LogAction(LogMessage);

      mocks.ReplayAll();

      Logger.LogWriter = mockLogWriter;
      Logger.LogAction(LogMessage);

      mocks.VerifyAll();
    }

    [Test]
    public void LogActionShouldCallLogActionOnLogWriterInstance2()
    {
      Logger.LogWriter = mockLogWriter;
      mockLogWriter.LogAction(LogMessage);

      mocks.ReplayAll();

      Logger.LogAction(LogMessage);

      mocks.VerifyAll();
    }
  }

  [TestFixture]
  public class ReturnJavaDialogHandlerTest : WatiNTest
  {
    [Test]
    public void WhenOnBeforeUnloadReturnJavaDialogIsShown_ClickingOnOkShouldCloseIE()
    {
      using (IE ie = new IE(OnBeforeUnloadJavaDialogURI))
      {
        ReturnDialogHandler returnDialogHandler = new ReturnDialogHandler();
        ie.AddDialogHandler(returnDialogHandler);

        IntPtr hWnd = ie.hWnd;
        // can't use ie.Close() here cause this will cleanup the registered
        // returnDialogHandler which leads to a timeout on the WaitUntilExists
        InternetExplorer internetExplorer = (InternetExplorer) ie.InternetExplorer;
        internetExplorer.Quit();

        returnDialogHandler.WaitUntilExists();
        returnDialogHandler.OKButton.Click();

        Thread.Sleep(2000);
        Assert.IsFalse(IE.Exists(new WatiN.Core.Attribute("hwnd", hWnd.ToString())));
      }
    }

    [Test]
    public void WhenOnBeforeUnloadReturnJavaDialogIsShown_ClickingOnCancelShouldKeepIEOpen()
    {
      using (IE ie = new IE(OnBeforeUnloadJavaDialogURI))
      {
        ReturnDialogHandler returnDialogHandler = new ReturnDialogHandler();
        ie.AddDialogHandler(returnDialogHandler);

        IntPtr hWnd = ie.hWnd;

        // can't use ie.Close() here cause this will cleanup the registered
        // returnDialogHandler which leads to a timeout on the WaitUntilExists
        InternetExplorer internetExplorer = (InternetExplorer) ie.InternetExplorer;
        internetExplorer.Quit();

        returnDialogHandler.WaitUntilExists();
        returnDialogHandler.CancelButton.Click();

        Thread.Sleep(2000);
        Assert.IsTrue(IE.Exists(new WatiN.Core.Attribute("hwnd", hWnd.ToString())));

        // finally close the ie instance
        internetExplorer.Quit();
        returnDialogHandler.WaitUntilExists();
        returnDialogHandler.OKButton.Click();
      }
    }
  }
}