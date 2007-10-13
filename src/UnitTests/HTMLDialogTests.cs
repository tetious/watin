namespace WatiN.Core.UnitTests
{
  using System;
  using System.Threading;
  using NUnit.Framework;
  using WatiN.Core.Exceptions;

  [TestFixture]
  public class HTMLDialogTests : WatiNTest
  {
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
        AttributeConstraint findBy = Find.ByUrl(PopUpURI);
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
        string expectedMessage = "Could not find a HTMLDialog by title with value 'popuptest'. (Search expired after '5' seconds)";

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
  }
}