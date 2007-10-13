namespace WatiN.Core.DialogHandlers
{
  using System;
  using System.Threading;

  /// <summary>
  /// This class handles the logon dialog by passing in the username and password
  /// and clicking the OK button.
  /// </summary>
  /// <example>
  /// The following code shows the use of this dialog handler
  /// <code>
  /// IE ie = new IE();
  ///
  /// ie.DialogWatcher.Add(new LogonDialogHandler(@"domain\username", "password"));
  ///
  /// ie.GoTo("https://www.somesecuresite.com");
  /// </code>
  /// </example>
  public class LogonDialogHandler : BaseDialogHandler
  {
    private string userName;
    private string password;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LogonDialogHandler"/> class.
    /// </summary>
    /// <param name="userName">Name of the user. Is required.</param>
    /// <param name="password">The password. If no password is required, it can be left blank (<c>null</c> or <c>String.Empty</c>). </param>
    public LogonDialogHandler(string userName, string password)
    {
      checkArgument("Username must be specified", userName, "username");

      this.userName = userName;
      
      if (password == null)
      {
        this.password = String.Empty;
      }
      else
      {
        this.password = password;
      }
    }

    public override bool HandleDialog(Window window)
    {
      if (IsLogonDialog(window))
      {
        // Find Handle of the "Frame" and then the combo username entry box inside the frame
        IntPtr inputFrameHandle = NativeMethods.GetChildWindowHwnd(window.Hwnd, "SysCredential");
        IntPtr usernameControlHandle = NativeMethods.GetChildWindowHwnd(inputFrameHandle, "ComboBoxEx32");

        NativeMethods.SetActiveWindow(usernameControlHandle);
        Thread.Sleep(50);

        NativeMethods.SetForegroundWindow(usernameControlHandle);
        Thread.Sleep(50);

        System.Windows.Forms.SendKeys.SendWait(userName + "{TAB}");
        Thread.Sleep(500);

        System.Windows.Forms.SendKeys.SendWait(password + "{ENTER}");
        
        return true;
      }
      
      return false;
    }

    /// <summary>
    /// Determines whether the specified window is a logon dialog.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns>
    /// 	<c>true</c> if the specified window is a logon dialog; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool IsLogonDialog(Window window)
    {
      // If a logon dialog window is found hWnd will be set.
      return NativeMethods.GetChildWindowHwnd(window.Hwnd, "SysCredential") != IntPtr.Zero;
    }

    private static void checkArgument(string message, string parameter, string parameterName)
    {
      if (UtilityClass.IsNullOrEmpty(parameter))
      {
        throw new ArgumentNullException(message, parameterName);
      }
    }
  }
}