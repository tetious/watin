namespace WatiN.Core.UnitTests.DialogHandlerTests
{
  using NUnit.Framework;
  using WatiN.Core.DialogHandlers;

  [TestFixture]
  public class AlertDialogHandlerTests : WatiNTest
  {
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
  }
}