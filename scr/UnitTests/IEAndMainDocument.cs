using System;
using System.Diagnostics;
using System.IO;

using NUnit.Framework;

using WatiN.Exceptions;
using WatiN.Logging;

namespace WatiN.Tests
{
  [TestFixture]
  public class IEAndMainDocument
  {
    private static Uri testDataBaseURI ;
    private static Uri mainURI;
    private static Uri indexURI;
    private static Uri popUpURI;
    private static Uri googleURI;

    [TestFixtureSetUp]
    public void Setup()
    {
      System.Threading.Thread.CurrentThread.ApartmentState = System.Threading.ApartmentState.STA;

      Logger.LogWriter = new DebugLogWriter();

      string testDataLocation = string.Format(@"{0}\testdata\", new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName);
            
      testDataBaseURI = new Uri(testDataLocation);
      mainURI = new Uri(testDataBaseURI, "main.html");
      indexURI = new Uri(testDataBaseURI, "Index.html");
      popUpURI = new Uri(testDataBaseURI, "popup.html");
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
        Debug.WriteLine("Before click");
        ie.MainDocument.Button(Find.ByName("btnG")).Click();
        Debug.WriteLine("After click");

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
        Document dialog = ie.HTMLDialogs[0].MainDocument;

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
    public void FindIEByPartialTitle()
    {
      using (IE ie = new IE(mainURI.ToString()))
      {
        ie.MainDocument.Link("testlinkid").Click();
        IE ieGoogle = IE.AttachToIE(Find.ByTitle("gOo"));
        Assert.AreEqual(googleURI.ToString(), ieGoogle.Url);
        ieGoogle.Close();
      }
    }

    [Test]
    public void FindIEByURL()
    {
      using (IE ie = new IE(mainURI.ToString()))
      {
        ie.MainDocument.Link("testlinkid").Click();
        IE ieGoogle = IE.AttachToIE(Find.ByUrl(googleURI.ToString()));
        Assert.AreEqual(googleURI.ToString(), ieGoogle.Url);
        ieGoogle.Close();
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

      HTMLDialog htmlDialog = ie.HTMLDialog(Find.ByTitle("PopUpTest"));
  
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

      HTMLDialog htmlDialog = ie.HTMLDialog(Find.ByTitle("PopUpTest"));
  
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

      HTMLDialog htmlDialog = ie.HTMLDialog(Find.ByUrl(popUpURI.ToString()));
  
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
          ie.HTMLDialog(Find.ByTitle("PopUpTest"), timeoutTime);
          Assert.Fail("PopUpTest should not be found");
        }
        catch (Exception e)
        {
          Assert.IsInstanceOfType(typeof(HTMLDialogNotFoundException), e);
          // add 1 second to give it some slack.
          Assert.Greater(timeoutTime + 1, DateTime.Now.Subtract(startTime).TotalSeconds);
          Assert.AreEqual(expectedMessage, e.Message, "Unexpected exception message");
        }
      }
    }
  }
}