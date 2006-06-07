using System;
using System.IO;
using System.Threading;

using NUnit.Framework;

using WatiN.Core;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class IEAndMainDocument
  {
    private static Uri htmlTestBaseURI ;
    private static Uri mainURI;
    private static Uri indexURI;
    private static Uri popUpURI;
    private static Uri googleURI;

    [TestFixtureSetUp]
    public void Setup()
    {
      Thread.CurrentThread.ApartmentState = ApartmentState.STA;

      Logger.LogWriter = new DebugLogWriter();

      string htmlLocation = string.Format(@"{0}\html\", new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName);
            
      htmlTestBaseURI = new Uri(htmlLocation);
      mainURI = new Uri(htmlTestBaseURI, "main.html");
      indexURI = new Uri(htmlTestBaseURI, "Index.html");
      popUpURI = new Uri(htmlTestBaseURI, "popup.html");
      googleURI = new Uri("http://www.google.com");
    }

    [Test]
    public void NUnitGUI()
    {
      using (new IE(mainURI.ToString(), true))
      {
      }
    }

    [Test]
    public void Google()
    {
      // Instantiate a new DebugLogger to output "user" events to
      // the debug window in VS
      Logger.LogWriter = new DebugLogWriter();

      using (IE ie = new IE(googleURI.ToString(), true))
      {
        ie.MainDocument.TextField(Find.ByName("q")).TypeText("WatiN");
        ie.MainDocument.Button(Find.ByName("btnG")).Click();

        Assert.IsTrue(ie.MainDocument.ContainsText("WatiN"));
      }
    }
    
    [Test]
    public void GoogleFormSubmit()
    {
      // Instantiate a new DebugLogger to output "user" events to
      // the debug window in VS
      Logger.LogWriter = new DebugLogWriter();

      using (IE ie = new IE(googleURI.ToString(), true))
      {
        ie.MainDocument.TextField(Find.ByName("q")).TypeText("WatiN");
        ie.MainDocument.Form(Find.ByName("f")).Submit();

        Assert.IsTrue(ie.MainDocument.ContainsText("WatiN"));
      }
    }

    [Test]
    public void ModelessDialog()
    {
      using (IE ie = new IE(mainURI.ToString(), true))
      {
        ie.MainDocument.Button("popupid").Click();
        Document dialog = ie.HtmlDialogs[0].MainDocument;

        Assert.AreEqual("47", dialog.TextField("dims").Value);
      }
    }

    [Test]
    public void ContainsText()
    {
      using (IE ie = new IE(mainURI.ToString(), true))
      {
        Assert.IsTrue(ie.MainDocument.ContainsText("Contains text in DIV"), "Text not found");
        Assert.IsFalse(ie.MainDocument.ContainsText("abcde"), "Text incorrectly found");
      }
    }
    [Test]
    public void Alert()
    {
      using (IE ie = new IE(mainURI.ToString(), true))
      {
        ie.MainDocument.Button("helloid").Click();

        // getting alert text
        Assert.AreEqual("hello", ie.PopAlert());

        // expected alert was missing
        try
        {
          ie.PopAlert();
          Assert.Fail("Expected MissingAlertException");
        }
        catch (MissingAlertException)
        {
          // expected; ignore
        }
      }
    }

    [Test]
    public void URL()
    {
      using (IE ie = new IE(mainURI.ToString(), true))
      {
        Assert.AreEqual(mainURI.ToString(), ie.Url);
      }
    }

    [Test]
    public void GoTo()
    {
      using (IE ie = new IE())
      {
        Uri URL = new Uri(mainURI.ToString());
        ie.GoTo(URL.ToString());
        Assert.AreEqual(URL.ToString(), ie.Url);
      }
    }

    [Test]
    public void BackAndForward()
    {
      using (IE ie = new IE())
      {
        ie.GoTo(mainURI.ToString());
        Assert.AreEqual(mainURI.ToString(), ie.Url);
        
        ie.MainDocument.Link(Find.ByUrl(indexURI.ToString())).Click();
        Assert.AreEqual(indexURI.ToString(), ie.Url);

        ie.Back();
        Assert.AreEqual(mainURI.ToString(), ie.Url);

        ie.Forward();
        Assert.AreEqual(indexURI.ToString(), ie.Url);
      }      
    }

    [Test]
    public void AttachToIEByParialTitle()
    {
      using (IE ie = new IE(mainURI.ToString()))
      {
        Assert.IsFalse(IsGoogleIEWindowOpen(), "An Internet Explorer with 'gOo' in it's title allready exists. AttachToIEByParialTitle can't be correctly tested. Please close the window.");
        
        ie.MainDocument.Link("testlinkid").Click();
        using (IE ieGoogle = IE.AttachToIE(Find.ByTitle("gOo")))
        {
          Assert.AreEqual(googleURI.ToString(), ieGoogle.Url);
        }
        
        Assert.IsFalse(IsGoogleIEWindowOpen(), "The Internet Explorer with 'gOo' in it's title should be closed.");
      }
    }

    [Test]
    public void AttachToIEByURL()
    {
      
      using (IE ie = new IE(mainURI.ToString()))
      {
        Assert.IsFalse(IsGoogleIEWindowOpen(), "An Internet Explorer with url " + googleURI.ToString() + " is allready open. AttachToIEByURL can't be correctly tested. Please close the window.");
        
        ie.MainDocument.Link("testlinkid").Click();
        using (IE ieGoogle = IE.AttachToIE(Find.ByUrl(googleURI.ToString())))
        {
          Assert.AreEqual(googleURI.ToString(), ieGoogle.Url);
        }
        
        Assert.IsFalse(IsGoogleIEWindowOpen(), "The Internet Explorer with 'gOo' in it's title should be closed.");
      }
    }

    [Test]
    public void IENotFoundException()
    {
      DateTime startTime = DateTime.Now;
      const int timeoutTime = 5;
      string expectedMessage = "Could not find an IE window by title with value 'Non Excisting IE Title'. (Search expired after '5' seconds)";
      
      try
      {
        // Time out after timeoutTime seconds
        startTime = DateTime.Now;
        IE.AttachToIE(Find.ByTitle("Non Excisting IE Title"),timeoutTime);
        Assert.Fail("Index.html should not be found");
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
      IE ie = new IE(mainURI.ToString());

      ie.MainDocument.Button("modalid").ClickNoWait();

      HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest"));
  
      Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
      Assert.AreEqual("PopUpTest", htmlDialog.MainDocument.Title, "Unexpected title");
  
      htmlDialog.MainDocument.TextField("name").TypeText("Textfield in HTMLDialog");
      htmlDialog.MainDocument.Button("hello").Click();

      htmlDialog.Close();

      ie.WaitForComplete();
      ie.Close();
    }

    [Test]
    public void HTMLDialogFindByTitle()
    {
      IE ie = new IE(mainURI.ToString());

      ie.MainDocument.Button("modalid").ClickNoWait();

      HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest"));
  
      Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
      Assert.AreEqual("PopUpTest", htmlDialog.MainDocument.Title, "Unexpected title");
  
      htmlDialog.Close();

      ie.WaitForComplete();
      ie.Close();
    }

    [Test]
    public void HTMLDialogFindByUrl()
    {
      IE ie = new IE(mainURI.ToString());

      ie.MainDocument.Button("modalid").ClickNoWait();

      HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByUrl(popUpURI.ToString()));
  
      Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
      Assert.AreEqual("PopUpTest", htmlDialog.MainDocument.Title, "Unexpected title");
  
      htmlDialog.Close();

      ie.WaitForComplete();
      ie.Close();
    }

    [Test]
    public void HTMLDialogNotFoundException()
    {
      using (IE ie = new IE(mainURI.ToString()))
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

    [Test]
    public void NewUri()
    {
      Uri uri = new Uri("about:blank");
      Assert.AreEqual("about:blank", uri.ToString());
    }

    private static bool IsGoogleIEWindowOpen()
    {
      try
      {
        IE.AttachToIE(Find.ByTitle("gOo"), 5);
      }
      catch (IENotFoundException)
      {
        return false;
      }
      return true;
    }
  }
}