namespace WatiN.Core.DialogHandlers
{
  public class ReturnDialogHandler : ConfirmDialogHandler
  {
    public override bool CanHandleDialog(Window window)
    {
      return (window.StyleInHex == "94C803C5" && ButtonWithId1Exists(window.Hwnd));
    }
  }
}