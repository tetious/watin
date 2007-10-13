namespace WatiN.Core.UnitTests.DialogHandlerTests
{
  using NUnit.Framework;
  using WatiN.Core.DialogHandlers;

  [TestFixture]
  public class SecurityDialogHandlerTests
  {
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
      public IENoWaitForComplete(string url) : base(url)
      {
      }

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
  }
}