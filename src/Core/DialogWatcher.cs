#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright

using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

using WatiN.Core.Exceptions;

namespace WatiN.Core.DialogHandlers
{
  /// <summary>
  /// This class handles alert/popup dialogs. Every second it checks if a dialog
  /// is shown. If so, it stores it's message in the alertQueue and closses the dialog
  /// by clicking the close button in the title bar.  
  /// </summary>
  public class DialogWatcher : IDisposable
  {
    private int ieProcessId;
    private bool keepRunning = true;
    private ArrayList handlers = new ArrayList();
    private Thread watcherThread;
    private bool closeUnhandledDialogs = true;
    
    private static ArrayList dialogWatchers = new ArrayList();
    
    public static DialogWatcher GetDialogWatcherForProcess(int ieProcessId)
    {     
      CleanupDialogWatchers();
      
      // Loop through already created dialogwatchers and
      // return a dialogWatcher if one exists for the given processid
      foreach (DialogWatcher dialogWatcher in dialogWatchers)
      {
        if (dialogWatcher.ProcessId == ieProcessId)
        {
          return dialogWatcher;
        }
      }

      // If no dialogwatcher exists for the ieprocessid then 
      // create a new one, store it and return it.
      DialogWatcher newDialogWatcher = new DialogWatcher(ieProcessId);
      
      dialogWatchers.Add(newDialogWatcher);
      
      return newDialogWatcher;
    }

    private static void CleanupDialogWatchers()
    {
      ArrayList cleanedupDialogWatchers = new ArrayList();
      
      foreach (DialogWatcher dialogWatcher in dialogWatchers)
      {
        if (!dialogWatcher.ProcessStillExists)
        {
          dialogWatcher.Dispose();
        }
        else
        {
          cleanedupDialogWatchers.Add(dialogWatcher);
        }
      }
      
      dialogWatchers = cleanedupDialogWatchers;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogWatcher"/> class.
    /// You are encouraged to use the Factory method DialogWatcher.GetDialogWatcherForProcess
    /// instead.
    /// </summary>
    /// <param name="ieProcessId">The ie process id.</param>
    internal DialogWatcher(int ieProcessId)
    {
      this.ieProcessId = ieProcessId;
      
      handlers = new ArrayList();

      // Create thread to watch windows
      watcherThread = new Thread(new ThreadStart(Start));
      // Start the thread.
      watcherThread.Start();
    }

    public void Add(IDialogHandler handler)
    {
      lock (this)
      {
        handlers.Add(handler);
      }
    }
    
    public void Remove(IDialogHandler handler)
    {
      lock (this)
      {
        handlers.Remove(handler);
      }
    }
    
    public void RemoveAll(IDialogHandler handler)
    {
      while (Contains(handler))
      {
        Remove(handler);
      }
    }

    public void Clear()
    {
      lock (this)
      {
        handlers.Clear();
      }
    }
    
    public bool Contains(object handler)
    {
      lock (this)
      {
        return handlers.Contains(handler);
      }
    }
    
    public int Count
    {
      get
      {
        lock (this)
        {
          return handlers.Count;
        }
      }
    }

    public bool CloseUnhandledDialogs
    {
      get
      {
        lock (this)
        {
          return closeUnhandledDialogs;
        }
      }
      set
      {
        lock (this)
        {
          closeUnhandledDialogs = value;
        }
      }
    }

    public int ProcessId
    {
      get { return ieProcessId; }
    }

    /// <summary>
    /// Called by the constructor to start watching popups
    /// on a separate thread.
    /// </summary>
    private void Start()
    {
      while (keepRunning)
      {
        Process process = getProcess(ProcessId);

        if (process != null)
        {
          foreach (ProcessThread t in process.Threads)
          {
            int threadId = t.Id;

            NativeMethods.EnumThreadProc callbackProc = new NativeMethods.EnumThreadProc(myEnumThreadWindowsProc);
            NativeMethods.EnumThreadWindows(threadId, callbackProc, IntPtr.Zero);
          }
        }

        Thread.Sleep(1000);
      }
    }

    public bool ProcessStillExists
    {
      get
      {
        return (getProcess(ProcessId) != null);
      }
    }

    private Process getProcess(int processId)
    {
      Process process;
      try
      {
        process = Process.GetProcessById(processId);
      }
      catch(ArgumentException)
      {
        // Thrown when the ieProcessId is not running (anymore)
        process = null;
      }
      return process;
    }

    private bool myEnumThreadWindowsProc(IntPtr hwnd, IntPtr lParam)
    {
      // Create a window wrapper class
      HandleWindow(new Window(hwnd));
      
      // Always return true so all windows in all threads will
      // be enumerated.
      return true;
    }

    private void HandleWindow(Window window)
    {
      if (window.IsDialog())
      {
        // Wait untill window is visible so all properties
        // of the window class (like Style and StyleInHex)
        // will return valid values.
        while (window.Visible == false)
        {
          Thread.Sleep(50);
        }
        
        // Lock the thread and see if a handler will handle
        // this dialog window
        lock (this)
        {
          foreach (IDialogHandler dialogHandler in handlers)
          {
            if (dialogHandler.HandleDialog(window))
            {
              return;
            }
          }

          // If no handler handled the dialog, see if it
          // should be close automatically.
          if (CloseUnhandledDialogs)
          {
            window.ForceClose();
          }
        }
      }
    }

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or
    /// resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      lock (this)
      {
        keepRunning = false;
        watcherThread.Join();
      }
    }

    #endregion
  }
  
  public interface IDialogHandler
  {
    bool HandleDialog(Window window);
  }
  
  public class Window
  {
    private IntPtr hwnd;
    
    public Window(IntPtr hwnd)
    {
      this.hwnd = hwnd;
    }
    
    public IntPtr Hwnd
    {
      get
      {
        return hwnd;
      }
    }
    
    public string Title
    {
      get
      {
        return NativeMethods.GetWindowText(Hwnd);
      }
    }

    public string ClassName
    {
      get
      {
        return NativeMethods.GetClassName(Hwnd);
      }
    }

    public Int64 Style
    {
      get
      {
        return NativeMethods.GetWindowStyle(Hwnd);
      }
    }
    public string StyleInHex
    {
      get
      {
        return NativeMethods.GetWindowStyle(Hwnd).ToString("X");
      }
    }
    
    public bool IsDialog()
    {
      return (ClassName == "#32770");
    }

    public void ForceClose()
    {
      NativeMethods.SendMessage(Hwnd, NativeMethods.WM_CLOSE, 0, 0);
    }
    
    public bool Exists()
    {
      return NativeMethods.IsWindow(Hwnd);
    }
    
    public bool Visible
    {
      get
      {
        return NativeMethods.IsWindowVisible(Hwnd);
      }
    }
  }
  
  public class AlertAndConfirmDialogHandler : IDialogHandler
  {
    private Queue alertQueue;

    public AlertAndConfirmDialogHandler()
    {
      alertQueue = new Queue();
    }

    /// <summary>
    /// Gets the count of the messages in the que of displayed alert and confirm windows.
    /// </summary>
    /// <value>The count of the alert and confirm messages in the que.</value>
    public int Count
    {
      get
      {
        return alertQueue.Count;
      }
    }

    /// <summary>
    /// Pops the most recent message from a que of displayed alert and confirm windows.
    /// Use this method to get the displayed message.
    /// </summary>
    /// <returns>The displayed message.</returns>
    public string Pop()
    {
      if (alertQueue.Count == 0)
      {
        throw new MissingAlertException();
      }

      return (string) alertQueue.Dequeue();
    }
    
    /// <summary>
    /// Gets the alert and confirm messages in the que of displayed alert and confirm windows.
    /// </summary>
    /// <value>The alert and confirm messages in the que.</value>
    public string[] Alerts
    {
      get
      {
        string[] result = new string[alertQueue.Count];
        Array.Copy(alertQueue.ToArray(), result, alertQueue.Count);
        return result;
      }
    }

    /// <summary>
    /// Clears all the messages from the que of displayed alert and confirm windows.
    /// </summary>
    public void Clear()
    {
      alertQueue.Clear();
    }

    public bool HandleDialog(Window window)
    {
      IntPtr handle = NativeMethods.GetDlgItem(window.Hwnd, 0xFFFF);

      if (handle != IntPtr.Zero)
      {
        alertQueue.Enqueue(NativeMethods.GetWindowText(handle));
        
        window.ForceClose();
      
        return true;
      }

      return false;
    }
  }
  
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
  public class LogonDialogHandler : IDialogHandler
  {
    private string userName;
    private string password;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LogonDialogHandler"/> class.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    public LogonDialogHandler(string userName, string password)
    {
      checkArgument("Username must be specified", userName, "username");
      checkArgument("Password must be specified", password, "password");

      this.userName = userName;
      this.password = password;
    }

    public bool HandleDialog(Window window)
    {
      if (IsLogonDialog(window))
      {
        NativeMethods.SetForegroundWindow(window.Hwnd);
        NativeMethods.SetActiveWindow(window.Hwnd);

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
  
  public class CertificateWarningHandler : IDialogHandler
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
       
    public bool HandleDialog(Window window)
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
    protected virtual bool IsCertificateDialog(Window window)
    {     
      return window.StyleInHex == certificateWarningDialogStyle;
    }
  }
  
  public class FileUploadDialogHandler : IDialogHandler
  {
    private String fileName;
    
    public FileUploadDialogHandler(String fileName)
    {
      this.fileName = fileName;  
    }
    
    #region IDialogHandler Members

    public bool HandleDialog(Window window)
    {
      Debug.WriteLine(Environment.TickCount + " Enter handler: " + window.Title);
      
      if (IsFileUploadDialog(window))
      {
        NativeMethods.SetForegroundWindow(window.Hwnd);
        NativeMethods.SetActiveWindow(window.Hwnd);

        System.Windows.Forms.SendKeys.SendWait(fileName + "{ENTER}");
        return true;
      }
        
      return false;
    }

    #endregion

    public bool IsFileUploadDialog(Window window)
    {
      // "96CC20C4" is valid for Windows XP, Win 2000 and Win 2003
      // and probably Vista
      bool returnValue = (window.StyleInHex == "96CC20C4");
      return returnValue;
    }
  }
  
  public class WinButton
  {
    private IntPtr hWnd;
    
    public WinButton(IntPtr Hwnd)
    {
      hWnd = Hwnd;
    }

    public WinButton(int buttonid, IntPtr parentHwnd)
    {
      hWnd = NativeMethods.GetDlgItem(parentHwnd, buttonid);
    }
    
    public void Click()
    {
      if (Exists())
      {
        NativeMethods.SendMessage(hWnd, NativeMethods.BM_CLICK, 0, 0);
      }
    }
    
    public bool Exists()
    {
      return NativeMethods.IsWindow(hWnd);
    }
    
    public string Title
    {
      get
      {
        return NativeMethods.GetWindowText(hWnd);
      }
    }
    
    public bool Enabled
    {
      get
      {
        return NativeMethods.IsWindowEnabled(hWnd);
      }
    }
    
    public bool Visible
    {
      get
      {
        return NativeMethods.IsWindowVisible(hWnd);
      }
    }
  }
  
  public abstract class JavaDialog
  {
    internal Window window;
    internal abstract bool CanHandleDialog(Window window);

    public bool Exists()
    {
      if (window == null) return false;
      
      return window.Exists();
    }

    public string Title
    {
      get
      {
        ThrowExceptionIfDialogDoesNotExist();
        
        return window.Title;
      }
    }

    public string Message
    {
      get
      {
        ThrowExceptionIfDialogDoesNotExist();
        
        IntPtr messagehWnd = NativeMethods.GetDlgItem(window.Hwnd, 65535);
        return NativeMethods.GetWindowText(messagehWnd);
      }
    }

    public WinButton OKButton
    {
      get
      {
        ThrowExceptionIfDialogDoesNotExist();
        
        return new WinButton(getOKButtonID(), window.Hwnd );
      }
    }

    protected abstract int getOKButtonID();

    protected bool ButtonWithId1Exists(IntPtr windowHwnd)
    {
      WinButton button = new WinButton(1, windowHwnd);
      return button.Exists();
    }

    protected WinButton createCancelButton(IntPtr windowHwnd)
    {
      return new WinButton(2, windowHwnd );
    }

    protected void ThrowExceptionIfDialogDoesNotExist()
    {
      if (!Exists())
      {
        throw new WatiNException("Operation not available. Dialog doesn't exist.");
      }
    }
  }

  public abstract class JavaDialogHandler : JavaDialog, IDialogHandler
  {
    public bool HandleDialog(Window window)
    {
      if (CanHandleDialog(window))
      {
        this.window = window;
      
        while(window.Exists())
        {
          Thread.Sleep(200);
        }
        return true;
        
      }
      return false;
    }

    public void WaitUntilExists()
    {
      WaitUntilExists(30);
    }
    
    public void WaitUntilExists(int waitDurationInSeconds)
    {
      DateTime startWaitUntilExists = DateTime.Now;

      bool dialogNotAvailable = !Exists();
      
      while (dialogNotAvailable)
      {
        Thread.Sleep(200);
        dialogNotAvailable = !UtilityClass.IsTimedOut(startWaitUntilExists, waitDurationInSeconds) & !Exists();
      }
      
      if (dialogNotAvailable)
      {
        throw new WatiNException(string.Format("Dialog not available within {0} seconds.", waitDurationInSeconds.ToString()));
      }
    }
  }

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

    internal override bool CanHandleDialog(Window window)
    {
      return (window.StyleInHex == "94C801C5" && ButtonWithId1Exists(window.Hwnd));
    }

    protected override int getOKButtonID()
    {
      return 1;
    }
  }
  
  public class AlertDialogHandler : JavaDialogHandler
  {
    internal override bool CanHandleDialog(Window window)
    {      
      return (window.StyleInHex == "94C801C5" && !ButtonWithId1Exists(window.Hwnd));
    }

    protected override int getOKButtonID()
    {
      return 2;
    }
  }
  
  public class SimpleJavaDialogHandler : IDialogHandler
  {
    JavaDialog dialogHandler;
    bool clickCancelButton = false;
    private bool hasHandledDialog = false;
    private string message;
    
    public SimpleJavaDialogHandler()
    {
      dialogHandler = new AlertDialogHandler();  
    }
    
    public SimpleJavaDialogHandler(bool clickCancelButton)
    {
      this.clickCancelButton = clickCancelButton;
      dialogHandler = new ConfirmDialogHandler();
    }

    public string Message
    {
      get { return message; }
    }

    public bool HasHandledDialog
    {
      get { return hasHandledDialog; }
    }

    #region IDialogHandler Members

    public bool HandleDialog(Window window)
    {
      if (dialogHandler.CanHandleDialog(window))
      {
        dialogHandler.window = window;
        
        message = dialogHandler.Message;
        
        ConfirmDialogHandler confirmDialogHandler = dialogHandler as ConfirmDialogHandler;
        
        if (confirmDialogHandler != null && clickCancelButton)
        {
          confirmDialogHandler.CancelButton.Click();
        }
        else
        {
          dialogHandler.OKButton.Click();
        }
        
        hasHandledDialog = true;
        
        return true;
      }
      
      return false;
    }

    #endregion
  }
}