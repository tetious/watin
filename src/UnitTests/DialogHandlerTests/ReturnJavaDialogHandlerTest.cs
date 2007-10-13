namespace WatiN.Core.UnitTests
{
  using System;
  using System.Threading;
  using NUnit.Framework;
  using SHDocVw;
  using WatiN.Core.DialogHandlers;

  [TestFixture]
  public class ReturnJavaDialogHandlerTest : WatiNTest
  {
    [Test]
    public void WhenOnBeforeUnloadReturnJavaDialogIsShown_ClickingOnOkShouldCloseIE()
    {
      using (IE ie = new IE(WatiNTest.OnBeforeUnloadJavaDialogURI))
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
        Assert.IsFalse(IE.Exists(new AttributeConstraint("hwnd", hWnd.ToString())));
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
        Assert.IsTrue(IE.Exists(new AttributeConstraint("hwnd", hWnd.ToString())));

        // finally close the ie instance
        internetExplorer.Quit();
        returnDialogHandler.WaitUntilExists();
        returnDialogHandler.OKButton.Click();
      }
    }
  }
}