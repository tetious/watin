namespace WatiN.Core.DialogHandlers
{
  /// <summary>
  /// This class handles the Security Warning dialog and does press
  /// the OK button when the dialog shows up.
  /// </summary>
  public class SecurityAlertDialogHandler : BaseDialogHandler
  {
    private const string securityAlertDialogStyle = "94C80AC4";

    /// <summary>
    /// Handles the dialog if the <paramref name="window" /> is a
    /// security alert dialog.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns></returns>
    public override bool HandleDialog(Window window)
    {
      if (IsSecurityAlertDialog(window))
      {
        NativeMethods.SetForegroundWindow(window.Hwnd);
        NativeMethods.SetActiveWindow(window.Hwnd);

        WinButton buttonOk = new WinButton(1, window.Hwnd);
        buttonOk.Click();
      
        return true;
      }
      
      return false;
    }

    /// <summary>
    /// Determines whether the specified window is a security alert dialog.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns>
    /// 	<c>true</c> if the specified window is a security alert dialog; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool IsSecurityAlertDialog(Window window)
    {     
      return window.StyleInHex == securityAlertDialogStyle;
    }

  }
}