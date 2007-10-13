namespace WatiN.Core.UnitTests.DialogHandlerTests
{
  using NUnit.Framework;
  using WatiN.Core.DialogHandlers;

  [TestFixture]
  public class ConfirmDialogHandlerTests : WatiNTest
  {
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
  }
}