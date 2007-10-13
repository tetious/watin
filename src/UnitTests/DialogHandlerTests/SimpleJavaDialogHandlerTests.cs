namespace WatiN.Core.UnitTests.DialogHandlerTests
{
  using NUnit.Framework;
  using WatiN.Core.DialogHandlers;

  [TestFixture]
  public class SimpleJavaDialogHandlerTests : WatiNTest
  {
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
  }
}