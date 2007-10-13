namespace WatiN.Core.DialogHandlers
{
  public class CertificateWarningHandler : BaseDialogHandler
  {
    public enum ButtonsEnum
    {
      Yes = 1,
      No = 2
    }
    
    private const string certificateWarningDialogStyle = "94C808C4";

    private ButtonsEnum buttonToPush;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CertificateWarningHandler"/> class.
    /// This handler will click the "Yes" button at the certificate warning dialog.
    /// </summary>
    public CertificateWarningHandler()
    {
      buttonToPush = ButtonsEnum.Yes;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CertificateWarningHandler"/> class.
    /// </summary>
    /// <param name="buttonToPush">The button to push.</param>
    public CertificateWarningHandler(ButtonsEnum buttonToPush)
    {
      this.buttonToPush = buttonToPush;
    }
       
    public override bool HandleDialog(Window window)
    {      
      if (IsCertificateDialog(window))
      {
        NativeMethods.SetForegroundWindow(window.Hwnd);
        NativeMethods.SetActiveWindow(window.Hwnd);

        ButtonToPush(window).Click();
      
        return true;
      }
      
      return false;
    }

    private WinButton ButtonToPush(Window window)
    {
      return new WinButton((int)buttonToPush, window.Hwnd);
    }

    /// <summary>
    /// Determines whether the specified window is a certificate dialog.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns>
    /// 	<c>true</c> if the specified window is a certificate dialog; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool IsCertificateDialog(Window window)
    {     
      return window.StyleInHex == certificateWarningDialogStyle;
    }
  }
}