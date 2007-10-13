namespace WatiN.Core.DialogHandlers
{
  public class ConfirmDialogHandler : JavaDialogHandler
  {
    public WinButton CancelButton
    {
      get
      {
        ThrowExceptionIfDialogDoesNotExist();

        return new WinButton(2, window.Hwnd);
      }
    }

    public override bool CanHandleDialog(Window window)
    {
      return (window.StyleInHex == "94C801C5" && ButtonWithId1Exists(window.Hwnd));
    }

    protected override int getOKButtonID()
    {
      return 1;
    }
  }
}