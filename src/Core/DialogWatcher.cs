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

namespace WatiN.Core
{
  /// <summary>
  /// This class handles alert/popup dialogs. Every second it checks if a dialog
  /// is shown. If so, it stores it's message in the alertQueue and closses the dialog
  /// by clicking the close button in the title bar.  
  /// </summary>
  public class DialogWatcher
  {
    private int ieProcessId;
    private bool keepRunning = true;
    private DefaultDialogHandler defaultHandler = new DefaultDialogHandler();
    private ArrayList handlers = new ArrayList();
    private Thread watcherThread;
    private bool closeUnhandledDialogs = true;
    
    private static ArrayList dialogWatchers = new ArrayList();
    
    public static DialogWatcher GetDialogWatcherForProcess(int ieProcessId)
    {     
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
    
    internal DialogWatcher(int ieProcessId)
    {
      this.ieProcessId = ieProcessId;
      
      defaultHandler = new DefaultDialogHandler();
      handlers = new ArrayList();

      // Create thread to watch windows
      watcherThread = new Thread(new ThreadStart(Start));
      // Start the thread.
      watcherThread.Start();
    }

    public int AlertCount()
    {
      lock (this)
      {
        return DefaultHandler.AlertCount;
      }
    }

    public string PopAlert()
    {
      lock (this)
      {
        return DefaultHandler.PopAlert();
      }
    }
    
    public string[] Alerts
    {
      get
      {
        lock (this)
        {
          return DefaultHandler.Alerts;
        }
      }
    }

    public DefaultDialogHandler DefaultHandler
    {
      get
      {
        lock (this)
        {
          return defaultHandler;
        }
      }
      set
      {
        lock (this)
        {
          defaultHandler = value;
        }
      }
    }

    public void FlushAlerts()
    {
      lock (this)
      {
        DefaultHandler.FlushAlerts();
      }
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
    
    public void Clear()
    {
      lock (this)
      {
        handlers.Clear();
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
        return closeUnhandledDialogs;
      }
      set
      {
        closeUnhandledDialogs = value;
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
        lock (this)
        {
          Process process = getProcess();

          if (process != null)
          {
            foreach (ProcessThread t in process.Threads)
            {
              int threadId = t.Id;
	
              NativeMethods.EnumThreadProc callbackProc = new NativeMethods.EnumThreadProc(myEnumThreadWindowsProc);
              NativeMethods.EnumThreadWindows(threadId, callbackProc, IntPtr.Zero);
            }
          }
        }
        Thread.Sleep(1000);
      }
    }

    public void Stop()
    {
      lock (this)
      {
        keepRunning = false;
        watcherThread.Join();
      }
    }

    private Process getProcess()
    {
      Process process;
      try
      {
        process = Process.GetProcessById(ProcessId);
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
      Window window = new Window(hwnd);
      
      if (window.IsDialog())
      {
        foreach (IDialogHandler dialogHandler in handlers)
        {
          if (dialogHandler.HandleDialog(window))
          {
            return true;
          }
        }

        if (CloseUnhandledDialogs)
        {
          // using defaultHandler should be removed and this 
          // line of code should be all.
//          window.ForceClose();
          
          // If no dialogHandler handled the dialog, the
          // defaultHandler will close the dialog.
          if (defaultHandler != null)
          {
            DefaultHandler.HandleDialog(window);
          }
        }
      }

      return true;
    }
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
      NativeMethods.SendMessage(hwnd, NativeMethods.WM_CLOSE, 0, 0);
    }
  }
  
  public class DefaultDialogHandler : IDialogHandler
  {
    private Queue alertQueue;

    public DefaultDialogHandler()
    {
      alertQueue = new Queue();
    }

    public int AlertCount
    {
      get
      {
        return alertQueue.Count;
      }
    }

    public string PopAlert()
    {
      if (alertQueue.Count == 0)
      {
        throw new MissingAlertException();
      }

      return (string) alertQueue.Dequeue();
    }
    
    public string[] Alerts
    {
      get
      {
        string[] result = new string[alertQueue.Count];
        Array.Copy(alertQueue.ToArray(), result, alertQueue.Count);
        return result;
      }
    }

    public void FlushAlerts()
    {
      alertQueue.Clear();
    }

    public bool HandleDialog(Window window)
    {
      IntPtr handle = NativeMethods.GetDlgItem(window.Hwnd, 0xFFFF);

      alertQueue.Enqueue(NativeMethods.GetWindowText(handle));
      
      window.ForceClose();
      
      return true;
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
      IntPtr hWnd = IntPtr.Zero;
      
      // Go throught the child windows of the dialog window
      NativeMethods.EnumChildProc childProc = new NativeMethods.EnumChildProc(enumChildForSysCredentials);
      NativeMethods.EnumChildWindows(window.Hwnd, childProc, ref hWnd);
      
      // If a logon dialog window is found hWnd will be set.
      return hWnd != IntPtr.Zero;
    }

    private static void checkArgument(string message, string parameter, string parameterName)
    {
      if (UtilityClass.IsNullOrEmpty(parameter))
      {
        throw new ArgumentNullException(message, parameterName);
      }
    }

    private bool enumChildForSysCredentials(IntPtr hWnd, ref IntPtr lParam)
    {
      if (classNameIsSysCredential(hWnd))
      {
        lParam = hWnd;
        return false;
      }

      return true;
    }

    private static bool classNameIsSysCredential(IntPtr hWnd)
    {
      return UtilityClass.CompareClassNames(hWnd, "SysCredential");
    }
  }
  
  public class CertificateWarningHandler : IDialogHandler
  {
    public enum ButtonsEnum
    {
      Yes = 1,
      No = 2
    }
    
    private const string certificateDialogStyle = "94C808C4";

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
        NativeMethods.SetActiveWindow(window.Hwnd);

        NativeMethods.ClickDialogButton((int)buttonToPush, window.Hwnd);
      
        return true;
      }
      
      return false;
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
      return window.StyleInHex == certificateDialogStyle;
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
      if (IsFileUploadDialog(window))
      {
        NativeMethods.SetForegroundWindow(window.Hwnd);
        NativeMethods.SetActiveWindow(window.Hwnd);

        System.Windows.Forms.SendKeys.SendWait(fileName + "{ENTER}");
        return true;
      }
        
      return false;
    }

    public bool IsFileUploadDialog(Window window)
    {
      // "96CC20C4" is valid for Windows XP, Win 2000 and Win 2003
      // and probably Vista
      return window.StyleInHex == "96CC20C4";
    }

    #endregion
  }
}