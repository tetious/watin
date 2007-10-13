namespace WatiN.Core.UnitTests.DialogHandlerTests
{
  using NUnit.Framework;
  using WatiN.Core.DialogHandlers;
  using WatiN.Core.Exceptions;

  [TestFixture]
  public class AlertAndConfirmDialogHandlerTests : WatiNTest
  {
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

    [Test, ExpectedException(typeof (MissingAlertException))]
    public void MissingAlertExceptionTest()
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
  }
}