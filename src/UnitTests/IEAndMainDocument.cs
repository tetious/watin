#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright

using System;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;

using WatiN.Core;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using Attribute=WatiN.Core.Attribute;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class IeTests : WatiNTest
  {
    [TestFixtureSetUp]
    public void Setup()
    {
      Logger.LogWriter = new DebugLogWriter();
    }

    [Test]
    public void ApartmentStateMustBeSTA()
    {
#if NET11
      // Code for .Net 1.1
      Assert.IsTrue(Thread.CurrentThread.ApartmentState == ApartmentState.STA);

#elif NET20
      // Code for .Net 2.0
      Assert.IsTrue(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
#endif
    }
    
    [Test]
    public void NUnitGUI()
    {
      using (new IE(MainURI))
      {
      }
    }

    [Test]
    public void Google()
    {
      // Instantiate a new DebugLogger to output "user" events to
      // the debug window in VS
      Logger.LogWriter = new DebugLogWriter();

      using (IE ie = new IE(googleUrl))
      {
        ie.TextField(Find.ByName("q")).TypeText("WatiN");
        ie.Button(Find.ByName("btnG")).Click();

        Assert.IsTrue(ie.ContainsText("WatiN"));
      }
    }
       
    [Test,Ignore("Second assert fails in nunit console mode.")]
    public void PressTabAndActiveElement()
    {
      using (IE ie = new IE(MainURI))
      {              
        ie.TextField("name").Focus();
        
        Element element = ie.ActiveElement;
        Assert.AreEqual("name",element.Id);

        ie.PressTab();
       
        element = ie.ActiveElement;
        Assert.AreEqual("popupid", element.Id);
      }
    }
        
    [Test]
    public void WindowStyle()
    {
      using(IE ie = new IE(MainURI))
      {
        NativeMethods.WindowShowStyle currentStyle = ie.GetWindowStyle();
        
        ie.ShowWindow(NativeMethods.WindowShowStyle.Maximize);
        Assert.AreEqual(NativeMethods.WindowShowStyle.Maximize.ToString(),ie.GetWindowStyle().ToString(),"Not maximized");

        ie.ShowWindow(NativeMethods.WindowShowStyle.Restore);
        Assert.AreEqual(currentStyle.ToString(),ie.GetWindowStyle().ToString(),"Not Restored");

        ie.ShowWindow(NativeMethods.WindowShowStyle.Minimize);
        Assert.AreEqual(NativeMethods.WindowShowStyle.ShowMinimized.ToString(),ie.GetWindowStyle().ToString(),"Not Minimize");
        
        ie.ShowWindow(NativeMethods.WindowShowStyle.ShowNormal);
        Assert.AreEqual(NativeMethods.WindowShowStyle.ShowNormal.ToString(),ie.GetWindowStyle().ToString(),"Not ShowNormal");
      }
    }
    
    [Test]
    public void GoogleFormSubmit()
    {
      using (IE ie = new IE(googleUrl))
      {
        ie.TextField(Find.ByName("q")).TypeText("WatiN");
        ie.Form(Find.ByName("f")).Submit();

        Assert.IsTrue(ie.ContainsText("WatiN"));
      }
    }

    [Test]
    public void ModelessDialog()
    {
      using (IE ie = new IE(MainURI))
      {
        ie.Button("popupid").Click();
        Document dialog = ie.HtmlDialogs[0];

        Assert.AreEqual("47", dialog.TextField("dims").Value);
      }
    }
    
    [Test]
    public void ContainsText()
    {
      using (IE ie = new IE(MainURI))
      {
        Assert.IsTrue(ie.ContainsText("Contains text in DIV"), "Text not found");
        Assert.IsFalse(ie.ContainsText("abcde"), "Text incorrectly found");
                
        Assert.IsTrue(ie.ContainsText(new Regex("Contains text in DIV")), "Regex: Text not found");
        Assert.IsFalse(ie.ContainsText(new Regex("abcde")), "Regex: Text incorrectly found");
      }
    }
    
    [Test]
    public void Alert()
    {
      using (IE ie = new IE(MainURI))
      {
        ie.Button("helloid").Click();

        // getting alert text
        Assert.AreEqual("hello", ie.PopAlert());
      }
    }
    
    [Test, ExpectedException(typeof(MissingAlertException))]
    public void MissingAlertException()
    {
      using (IE ie = new IE(MainURI))
      {
        ie.PopAlert();
      }
    }

    [Test, Ignore()]
    public void LogonDialogTest()
    {
      IE ie = new IE();
            
      ie.DialogWatcher.Add(new LogonDialogHandler(@"username", "password"));

      ie.GoTo("https://www.somesecuresite.com");

      ie.DialogWatcher.Clear();

    }
    
//    [Test]
//    public void DialogTestSpike1()
//    {
//      IE ie = new IE("http://www.ergens.nl");
//      
//      ConfirmDialog confirmDialog = new ConfirmDialog();
//      ie.DialogWatcher.Add(confirmDialog);
//      
//      ie.Button(Find.ByText("Show confirm dialog")).ClickNoWait();
//      
//      confirmDialog.WaitUntilExists();
//      Assert.AreEqual("Microsoft Internet Explorer", confirmDialog.Title);
//      
//      confirmDialog.Button("OK").Click();
//      ie.DialogWatcher.Remove(confirmDialog);
//      
//      ie.Close();
//    }
//    
//    [Test]
//    public void DialogTestSpike2()
//    {
//      IE ie = new IE("http://www.ergens.nl");
//
//      ie.DialogWatcher.Add(new AlertDialog("OK"));
//      
//      ie.Button(Find.ByText("Show confirm dialog")).Click();
//            
//      ie.DialogWatcher.Remove(new Dialog());
//
//      ie.PopAlert();
//      ie.Close();
//    }
//    
//    [Test]
//    public void DialogTestSpike3()
//    {
//      IE ie = new IE("http://www.ergens.nl");
//      
//      ie.DialogWatcher.CloseUnhandledDialogs = false;
//      
//      ie.Button(Find.ByText("Show confirm dialog")).ClickNoWait();
//      
//      Dialog confirmDialog = ie.Dialog;
//      Assert.AreEqual("Microsoft Internet Explorer", confirmDialog.Title);
//      
//      confirmDialog.Button("OK").Click();
//      
//      ie.DialogWatcher.CloseUnhandledDialogs = true;
//      
//      ie.Close();
//    }
    
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
    public void NewIE()
    {
      using (IE ie = new IE(true))
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
    public void NewIEWithUriAndAutoClose()
    {
      FailIfIEWindowExists("main", "NewIEWithUriNotAutoClose");

      using (new IE(MainURI, true))
      {
      }
      
      Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer should be closed by IE.Dispose");
    }
    
    [Test]
    public void NewIEWithUriNotAutoClose()
    {
      FailIfIEWindowExists("main", "NewIEWithUriNotAutoClose");

      using (new IE(MainURI, false))
      {
      }
      
      Assert.IsTrue(IsIEWindowOpen("main"), "Internet Explorer should NOT be closed by IE.Dispose");

      IE.AttachToIE(Find.ByTitle("main"), 3).Close();
    }
    
    [Test]
    public void NewIEWithUrlAndAutoClose()
    {
      FailIfIEWindowExists("main", "NewIEWithUriNotAutoClose");

      using (new IE(MainURI.ToString(), true))
      {
      }
      
      Assert.IsFalse(IsIEWindowOpen("main"), "Internet Explorer should be closed by IE.Dispose");
    }
    
    [Test]
    public void NewIEWithUrlNotAutoClose()
    {
      FailIfIEWindowExists("main", "NewIEWithUriNotAutoClose");

      using (new IE(MainURI.ToString(), false))
      {
      }
      
      Assert.IsTrue(IsIEWindowOpen("main"), "Internet Explorer should NOT be closed by IE.Dispose");

      IE.AttachToIE(Find.ByTitle("main"), 3).Close();
    }

    [Test]
    public void NewIEWithUrl()
    {
      string url = MainURI.ToString();
      
      using (IE ie = new IE(url))
      {
        Assert.AreEqual(MainURI, new Uri(ie.Url));
      }
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
    public void RefreshWithImmediatelyExpiredPage()
    {
      using (IE ie = new IE(googleUrl))
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

    /// <summary>
    /// Attaches to IE with a zero timeout interval. Allthough the timeout
    /// interval is zero the existing IE instance should be found.
    /// </summary>
    [Test]
    public void AttachToIEWithZeroTimeout()
    {
      // Create a new IE instance so we can find it.
      using(new IE(MainURI))
      {
        DateTime startTime = DateTime.Now;
        IE.AttachToIE(new Url(MainURI), 0);

        // Should return (within 1 second).
        Assert.Greater(1, DateTime.Now.Subtract(startTime).TotalSeconds);       
      }
    }

    [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void AttachToIEWithNegativeTimeoutNotAllowed()
    {
      IE.AttachToIE(Find.ByTitle("Bogs title"), -1);
    }

    [Test]
    public void AttachToIEByPartialTitleAndByUrl()
    {
      FailIfIEWindowExists("Ai", "AttachToIEByPartialTitleAndByUrl");

      using (new IE(MainURI))
      {
        IE ieMain = IE.AttachToIE(Find.ByTitle("Ai"));
        Assert.AreEqual(MainURI, ieMain.Uri);
        
        ieMain = IE.AttachToIE(Find.ByUrl(MainURI));
        Assert.AreEqual(MainURI, ieMain.Uri);
      }
    }
    
    [Test]
    public void NewIEClosedByDispose()
    {
      FailIfIEWindowExists("main", "IEClosedByDispose");

      using (new IE(MainURI))
      {
        IE ie = IE.AttachToIE(Find.ByTitle("main"));
        Assert.AreEqual(MainURI, new Uri(ie.Url));
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
        IE.AttachToIE(Find.ByTitle(ieTitle),timeoutTime);
        Assert.Fail(string.Format("Internet Explorer with title '{0}' should not be found", ieTitle));
      }
      catch (Exception e)
      {
        Assert.IsInstanceOfType(typeof(IENotFoundException), e);
        // add 1 second to give it some slack.
        Assert.Greater(timeoutTime + 1, DateTime.Now.Subtract(startTime).TotalSeconds);
        Assert.AreEqual(expectedMessage, e.Message, "Unexpected exception message");
      }
    }

    [Test]
    public void HTMLDialog()
    {
      IE ie = new IE(MainURI);

      ie.Button("modalid").ClickNoWait();

      HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest"));
  
      Assert.IsInstanceOfType(typeof(DomContainer), htmlDialog);
      
      Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
      Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");
  
      htmlDialog.TextField("name").TypeText("Textfield in HTMLDialog");
      htmlDialog.Button("hello").Click();

      htmlDialog.Close();

      ie.WaitForComplete();
      ie.Close();
    }

    [Test]
    public void HTMLDialogFindByTitle()
    {
      HTMLDialogWithFindBy(Find.ByTitle("PopUpTest"), false);
    }

    [Test]
    public void HTMLDialogFindByUrl()
    {
      HTMLDialogWithFindBy(Find.ByUrl(PopUpURI),false);
    }
    
    [Test]
    public void HTMLDialogFindByTitleAndWithTimeout()
    {
      HTMLDialogWithFindBy(Find.ByTitle("PopUpTest"), true);
    }

    [Test]
    public void HTMLDialogFindByUrlAndWithTimeout()
    {
      HTMLDialogWithFindBy(Find.ByUrl(PopUpURI),true);
    }
    
    private static void HTMLDialogWithFindBy(Attribute attribute, bool withTimeout)
    {
      using (IE ie = new IE(MainURI))
      {
        TestHTMLDialog(attribute, withTimeout, ie);
      }
    }
    
    private static void TestHTMLDialog(Attribute attribute, bool withTimeout, IE ie)
    {
      ie.Button("modalid").ClickNoWait();

      HtmlDialog htmlDialog = GetHtmlDialog(attribute, withTimeout, ie);

      Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
      Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");
  
      htmlDialog.Close();

      ie.WaitForComplete();
    }

    private static HtmlDialog GetHtmlDialog(Attribute attribute, bool withTimeout, IE ie)
    {
      if (!withTimeout)
      {
        if (attribute is Title)
        {
          return ie.HtmlDialog((Title)attribute);
        }
        return ie.HtmlDialog((Url)attribute);
      }
      if (attribute is Title)
      {
        return ie.HtmlDialog((Title)attribute, 10);
      }
      return ie.HtmlDialog((Url)attribute, 10);      
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
          Assert.IsInstanceOfType(typeof(HtmlDialogNotFoundException), e);
          // add 1 second to give it some slack.
          Assert.Greater(timeoutTime + 1, DateTime.Now.Subtract(startTime).TotalSeconds);
          Assert.AreEqual(expectedMessage, e.Message, "Unexpected exception message");
        }
      }
    }

    [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void HTMLDialogGettingWithNegativeTimeoutNotAllowed()
    {
      using (IE ie = new IE(MainURI))
      {
        ie.HtmlDialog(Find.ByUrl(PopUpURI), -1);
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
        Assert.IsNull(report.Text, "Report not empty");
        
        Button button = ie.Button(Find.ByValue("Button"));
        Assert.IsNull(button.Id, "Button id not null before click event");
        
        button.KeyDown();
        
        Assert.IsNotNull(report.Text, "No keydown event fired (report is empty )");
        StringAssert.StartsWith("button.id = ", report.Text, "Report should start with 'button.id = '");
        Assert.Greater(report.Text.Length, "button.id = ".Length, "No assigned id report");
        
        Assert.IsNull(button.Id, "Button id not null after click event");
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
      try
      {
        IE.AttachToIE(Find.ByTitle(partialTitle), 1);
      }
      catch (IENotFoundException)
      {
        return false;
      }
      return true;
    }
  }
}