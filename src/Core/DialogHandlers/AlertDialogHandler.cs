namespace WatiN.Core.DialogHandlers
{
  public class AlertDialogHandler : JavaDialogHandler
  {
    public override bool CanHandleDialog(Window window)
    {      
      return (window.StyleInHex == "94C801C5" && !ButtonWithId1Exists(window.Hwnd));
    }

    protected override int getOKButtonID()
    {
      return 2;
    }
  }
}