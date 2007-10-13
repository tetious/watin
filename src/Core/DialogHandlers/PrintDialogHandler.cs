namespace WatiN.Core.DialogHandlers
{
  /// <summary>
  /// This class handles the print dialog clicking the Print, 
  /// Cancel or Apply button.
  /// </summary>
  /// <example>
  /// The following code shows the use of this dialog handler
  /// <code>
  /// IE ie = new IE();
  ///
  /// ie.DialogWatcher.Add(new PrintDialogHandler(PrintDialogHandler.ButtonsEnum.Cancel));
  ///
  /// ie.GoTo("http://www.someprintdialog.com");
  /// </code>
  /// </example>
  public class PrintDialogHandler : BaseDialogHandler
  {
    public enum ButtonsEnum
    {
      Print = 1,
      Cancel = 2,
    }

    // need to be checked whether this is valid for other operating 
    // systems - chcecked on WinXP and Win2003
    private const string printWarningDialogStyle = "96C820C4";

    private ButtonsEnum buttonToPush;

    public PrintDialogHandler(ButtonsEnum 
                                buttonToPush)
    {
      this.buttonToPush = buttonToPush;
    }

    public override bool HandleDialog(Window window)
    {
      if (IsPrintDialog(window))
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
    /// Determines whether the specified window is a print dialog.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns>
    /// <c>true</c> if the specified window is a print dialog; otherwise, <c>false</c>.
    /// </returns>
    public bool IsPrintDialog(Window window)
    {
      return (window.StyleInHex == printWarningDialogStyle);
    }
  }
}