#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

using WatiN.Core.Exceptions;

namespace WatiN.Core.DialogHandlers
{
  using WatiN.Core.Logging;

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
    private bool closeUnhandledDialogs = IE.Settings.AutoCloseDialogs;
    private int referenceCount = 0;
    
    private static ArrayList dialogWatchers = new ArrayList();
    private Exception lastException;

    /// <summary>
    /// Gets the dialog watcher for the specified process. It creates
    /// a new instance if no dialog watcher for the specified process 
    /// exists.
    /// </summary>
    /// <param name="ieProcessId">The ie process id.</param>
    /// <returns></returns>
    public static DialogWatcher GetDialogWatcherForProcess(int ieProcessId)
    {     
      CleanupDialogWatcherCache();

      DialogWatcher dialogWatcher = GetDialogWatcherFromCache(ieProcessId);

      // If no dialogwatcher exists for the ieprocessid then 
      // create a new one, store it and return it.
      if (dialogWatcher == null)
      {
        dialogWatcher = new DialogWatcher(ieProcessId);
      
        dialogWatchers.Add(dialogWatcher);
      }
      
      return dialogWatcher;
    }

    public static DialogWatcher GetDialogWatcherFromCache(int ieProcessId)
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
      
      return null;
    }

    public static void CleanupDialogWatcherCache()
    {
      ArrayList cleanedupDialogWatcherCache = new ArrayList();
      
      foreach (DialogWatcher dialogWatcher in dialogWatchers)
      {
        if (!dialogWatcher.IsRunning)
        {
          dialogWatcher.Dispose();
        }
        else
        {
          cleanedupDialogWatcherCache.Add(dialogWatcher);
        }
      }
      
      dialogWatchers = cleanedupDialogWatcherCache;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogWatcher"/> class.
    /// You are encouraged to use the Factory method DialogWatcher.GetDialogWatcherForProcess
    /// instead.
    /// </summary>
    /// <param name="ieProcessId">The ie process id.</param>
    public DialogWatcher(int ieProcessId)
    {
      this.ieProcessId = ieProcessId;
      
      handlers = new ArrayList();

      // Create thread to watch windows
      watcherThread = new Thread(new ThreadStart(Start));
      // Start the thread.
      watcherThread.Start();
    }

    /// <summary>
    /// Increases the reference count of this DialogWatcher instance with 1.
    /// </summary>
    public void IncreaseReferenceCount()
    {
      referenceCount++;
    }
    
    /// <summary>
    /// Decreases the reference count of this DialogWatcher instance with 1.
    /// When reference count becomes zero, the Dispose method will be 
    /// automatically called. This method will throw an <see cref="ReferenceCountException"/>
    /// if the reference count is zero.
    /// </summary>
    public void DecreaseReferenceCount()
    {
      
      if (ReferenceCount > 0)
      {
        referenceCount--;
      }
      else
      {
        throw new ReferenceCountException();
      }      

      if (ReferenceCount == 0)
      {
        Dispose();
      }
    }
    
    /// <summary>
    /// Adds the specified handler.
    /// </summary>
    /// <param name="handler">The handler.</param>
    public void Add(IDialogHandler handler)
    {
      lock (this)
      {
        handlers.Add(handler);
      }
    }
    
    /// <summary>
    /// Removes the specified handler.
    /// </summary>
    /// <param name="handler">The handler.</param>
    public void Remove(IDialogHandler handler)
    {
      lock (this)
      {
        handlers.Remove(handler);
      }
    }
    
    /// <summary>
    /// Removes all instances that match <paramref name="handler"/>.
    /// This method determines equality by calling Object.Equals.
    /// </summary>
    /// <param name="handler">The object implementing IDialogHandler.</param>
    /// <example>
    /// If you want to use RemoveAll with your custom dialog handler to
    /// remove all instances of your dialog handler from a DialogWatcher instance,
    /// you should override the Equals method in your custom dialog handler class 
    /// like this:
    /// <code>
    /// public override bool Equals(object obj)
    /// {
    ///   if (obj == null) return false;
    ///   
    ///   return (obj is YourDialogHandlerClassNameGoesHere);
    /// }                               
    /// </code>
    /// You could also inherit from <see cref="BaseDialogHandler"/> instead of implementing
    /// <see cref="IDialogHandler"/> in your custom dialog handler. <see cref="BaseDialogHandler"/> provides
    /// overrides for Equals and GetHashCode that work with RemoveAll.
    /// </example>
    public void RemoveAll(IDialogHandler handler)
    {
      while (Contains(handler))
      {
        Remove(handler);
      }
    }

    /// <summary>
    /// Removes all registered dialog handlers.
    /// </summary>
    public void Clear()
    {
      lock (this)
      {
        handlers.Clear();
      }
    }
    
    /// <summary>
    /// Determines whether this <see cref="DialogWatcher"/> contains the specified dialog handler.
    /// </summary>
    /// <param name="handler">The dialog handler.</param>
    /// <returns>
    /// 	<c>true</c> if [contains] [the specified handler]; otherwise, <c>false</c>.
    /// </returns>
    public bool Contains(object handler)
    {
      lock (this)
      {
        return handlers.Contains(handler);
      }
    }
    
    /// <summary>
    /// Gets the count of registered dialog handlers.
    /// </summary>
    /// <value>The count.</value>
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

    /// <summary>
    /// Gets or sets a value indicating whether unhandled dialogs should be closed automaticaly.
    /// The initial value is set to the value of <paramref name="IE.Settings.AutoCloseDialogs" />.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if unhandled dialogs should be closed automaticaly; otherwise, <c>false</c>.
    /// </value>
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

    /// <summary>
    /// Gets the process id this dialog watcher watches.
    /// </summary>
    /// <value>The process id.</value>
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
          
          // Keep DialogWatcher responsive during 1 second sleep period
          int count = 0;
          while(keepRunning && count < 5)
          {
            Thread.Sleep(200);
            count++;
          }
        }
        else
        {
          keepRunning = false;
        }
      }
    }

    public bool IsRunning
    {
      get
      {
        return watcherThread.IsAlive;
      }
    }
    /// <summary>
    /// Gets a value indicating whether the process this dialog watcher
    /// watches (still) exists.
    /// </summary>
    /// <value><c>true</c> if process exists; otherwise, <c>false</c>.</value>
    public bool ProcessExists
    {
      get
      {
        return (getProcess(ProcessId) != null);
      }
    }

    public int ReferenceCount
    {
      get { return referenceCount; }
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

    /// <summary>
    /// Get the last stored exception thrown by a dialog handler while 
    /// calling the <see cref="IDialogHandler.HandleDialog"/> method of the
    /// dialog handler.
    /// </summary>
    /// <value>The last exception.</value>
    public Exception LastException
    {
      get { return lastException; }
    }

    /// <summary>
    /// If the window is a dialog and visible, it will be passed to
    /// the registered dialog handlers. I none if these can handle
    /// it, it will be closed if <see cref="CloseUnhandledDialogs"/>
    /// is <c>true</c>.
    /// </summary>
    /// <param name="window">The window.</param>
    public void HandleWindow(Window window)
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
            try
            {
              if (dialogHandler.HandleDialog(window))
              {
                return;
              }
            }
            catch (Exception e)
            {
              lastException = e;

              Logger.LogAction("Exception was thrown while DialogWatcher called HandleDialog:");
              Logger.LogAction(e.ToString());
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
        if (IsRunning)
        {
          watcherThread.Join();
        }
        Clear();
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
    
	  public virtual IntPtr Hwnd
	  {
		  get
		  {
			  return hwnd;
		  }
	  }
    
    public virtual IntPtr ParentHwnd
    {
      get
      {
        return NativeMethods.GetParent(Hwnd);
      }
    }

    public virtual string Title
    {
      get
      {
        return NativeMethods.GetWindowText(Hwnd);
      }
    }

    public virtual string ClassName
    {
      get
      {
        return NativeMethods.GetClassName(Hwnd);
      }
    }

    public virtual Int64 Style
    {
      get
      {
        return NativeMethods.GetWindowStyle(Hwnd);
      }
    }
    public virtual string StyleInHex
    {
      get
      {
        return Style.ToString("X");
      }
    }
    
    public virtual bool IsDialog()
    {
      return (ClassName == "#32770");
    }

    public virtual void ForceClose()
    {
      NativeMethods.SendMessage(Hwnd, NativeMethods.WM_CLOSE, 0, 0);
    }
    
    public virtual bool Exists()
    {
      return NativeMethods.IsWindow(Hwnd);
    }
    
    public virtual bool Visible
    {
      get
      {
        return NativeMethods.IsWindowVisible(Hwnd);
      }
    }

    public virtual void ToFront()
    {
      NativeMethods.SetForegroundWindow(Hwnd);
    }

    public virtual void SetActivate()
    {
      NativeMethods.SetActiveWindow(Hwnd);
    }
  }
  
  public class AlertAndConfirmDialogHandler : BaseDialogHandler
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

    public override bool HandleDialog(Window window)
    {
      // See if the dialog has a static control with a controlID 
      // of 0xFFFF. This is unique for alert and confirm dialogboxes.
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

  public class FileUploadDialogHandler : BaseDialogHandler
  {
    private String fileName;
    
    public FileUploadDialogHandler(String fileName)
    {
      this.fileName = fileName;  
    }
    
    public override bool HandleDialog(Window window)
    {      
      if (IsFileUploadDialog(window))
      {
        IntPtr usernameControlHandle = NativeMethods.GetChildWindowHwnd(window.Hwnd, "Edit");

        NativeMethods.SetForegroundWindow(usernameControlHandle);
        NativeMethods.SetActiveWindow(usernameControlHandle);

        System.Windows.Forms.SendKeys.SendWait(fileName + "{ENTER}");
        return true;
      }
        
      return false;
    }

    public bool IsFileUploadDialog(Window window)
    {
      // "96CC20C4" is valid for Windows XP, Win 2000 and Win 2003
      // "96CC02C4" is valid for Windows Vista
      return (window.StyleInHex == "96CC20C4") || (window.StyleInHex == "96CC02C4");
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
        NativeMethods.SendMessage(hWnd, NativeMethods.WM_ACTIVATE, NativeMethods.MA_ACTIVATE, 0);
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
  
  public abstract class JavaDialogHandler : BaseDialogHandler
  {
    internal Window window;

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

    public override bool HandleDialog(Window window)
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
      SimpleTimer timeoutTimer = new SimpleTimer(waitDurationInSeconds);

      while (!Exists() && !timeoutTimer.Elapsed)
      {
        Thread.Sleep(200);
      }
      
      if (!Exists())
      {
        throw new WatiNException(string.Format("Dialog not available within {0} seconds.", waitDurationInSeconds.ToString()));
      }
    }

    public abstract bool CanHandleDialog(Window window);

    public bool Exists()
    {
      if (window == null) return false;
      
      return window.Exists();
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
  
  public class ReturnDialogHandler : ConfirmDialogHandler
  {
    public override bool CanHandleDialog(Window window)
    {
      return (window.StyleInHex == "94C803C5" && ButtonWithId1Exists(window.Hwnd));
    }
  }

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

	public enum FileDownloadOption {Run,Save,Open,Cancel}

  public class FileDownloadHandler : BaseDialogHandler
  {
	  private Window downloadProgressDialog;
	  private bool hasHandledFileDownloadDialog = false;
	  private FileDownloadOption option;
    private string saveAsFilename = String.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDownloadHandler"/> class.
    /// Use this constructor if you want to Run, Open or Cancel the download.
    /// </summary>
    /// <param name="option">The option to choose on the File Download dialog.</param>
	  public FileDownloadHandler(FileDownloadOption option)
	  {
      if(option == FileDownloadOption.Save)
      {
        throw new ArgumentException("When using FileDownloadOption.Save call the constructor which accepts a filename as an argument");
      }

      this.option = option;
	  }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDownloadHandler"/> class.
    /// Use this contructor if you want to download and save a file.
    /// </summary>
    /// <param name="filename">The filename.</param>
	  public FileDownloadHandler(string filename)
	  {
      if(UtilityClass.IsNullOrEmpty(filename))
      {
        throw new ArgumentNullException("filename", "Not a valid value");
      }

		  option = FileDownloadOption.Save;
      saveAsFilename = filename;
	  }

    /// <summary>
    /// Gets a value indicating whether this instance has handled a file download dialog.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has handled a file download dialog; otherwise, <c>false</c>.
    /// </value>
	  public bool HasHandledFileDownloadDialog
	  {
		  get { return hasHandledFileDownloadDialog; }
	  }

    /// <summary>
    /// Gets the full save as filename used when the downloaded file will be saved to disk.
    /// </summary>
    /// <value>The save as filename.</value>
    public string SaveAsFilename
    {
      get { return saveAsFilename; }
    }

    /// <summary>
    /// Handles the dialogs to download (and save) a file
    /// Mainly used internally by WatiN.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns></returns>
    public override bool HandleDialog(Window window)
	  {
//      Logger.LogAction(">> HandleDialog");
//      Logger.LogAction("hwnd = " + window.Hwnd.ToString());
//      Logger.LogAction("style = " + window.Style.ToString());
//      Logger.LogAction("stylehex = " + window.StyleInHex);
//      Logger.LogAction("<< HandleDialog");

      // This if handles the File download dialog
      if (!HasHandledFileDownloadDialog && IsFileDownloadDialog(window))
		  {

        window.ToFront();
        window.SetActivate();

		    DownloadProgressDialog = new Window(window.ParentHwnd);
			  
			  int buttonid = 0;

			  switch (option)
			  {
				  case FileDownloadOption.Run: buttonid = 4426; break;
				  case FileDownloadOption.Open: buttonid = 4426; break;
				  case FileDownloadOption.Save: buttonid = 4427; break;
				  case FileDownloadOption.Cancel: buttonid = 2; break;
			  }

			  WinButton btn = new WinButton(buttonid, window.Hwnd);
			  btn.Click();

			  hasHandledFileDownloadDialog = !Exists(window);

        if (HasHandledFileDownloadDialog)
        {
          Logger.LogAction("Download started at " + DateTime.Now.ToLongTimeString());
          Logger.LogAction("Clicked " + option.ToString());
        }

			  return true;
		  }
		  
      // This if handles the download progress dialog
      if (IsDownloadProgressDialog(window))
		  {
        DownloadProgressDialog = window;

        return true;
		  }
		  
      // This if handles the File save as dialog
      if (IsFileSaveDialog(window))
		  {
			  Logger.LogAction("Saving Download file as " + saveAsFilename);
			  
        DownloadProgressDialog = new Window(window.ParentHwnd);

        HandleFileSaveDialog(window);
        
        return true;
		  }

      // Window is not a dialog this handler can handle.
		  return false;
	  }

    /// <summary>
    /// Determines whether the specified window is a file download dialog by
    /// checking the style property of the window. It should match
    /// <c>window.StyleInHex == "94C80AC4"</c>
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns>
    /// 	<c>true</c> if the specified window is a file download dialog; otherwise, <c>false</c>.
    /// </returns>
    public bool IsFileDownloadDialog(Window window)
    {
      return (window.StyleInHex == "94C80AC4");
    }

    /// <summary>
    /// Determines whether the specified window is a download progress dialog by
    /// checking the style property of the window. It should match
    /// <c>(window.StyleInHex == "9CCA0BC4") || (window.StyleInHex == "94CA0BC4")</c>
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns>
    /// 	<c>true</c> if the specified window is a download progress dialog; otherwise, <c>false</c>.
    /// </returns>
    public bool IsDownloadProgressDialog(Window window)
    {
      // "9CCA0BC4" is valid before downloading the file has started
      // "94CA0BC4" is valid during and after the download
      return (window.StyleInHex == "9CCA0BC4") || (window.StyleInHex == "94CA0BC4");
    }

    /// <summary>
    /// Determines whether the specified window is a file save as dialog by
    /// checking the style property of the window. It should match
    /// <c>(window.StyleInHex == "96CC20C4") || (window.StyleInHex == "96CC02C4")</c>
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns>
    /// 	<c>true</c> if the specified window is a file save as dialog; otherwise, <c>false</c>.
    /// </returns>
    public bool IsFileSaveDialog(Window window)
    {
      // "96CC20C4" is valid for Windows XP, Win 2000 and Win 2003
      // "96CC02C4" is valid for Windows Vista
      return (window.StyleInHex == "96CC20C4") || (window.StyleInHex == "96CC02C4");
    }

    /// <summary>
    /// Determines if a dialog still exists by checking the the existance of the 
    /// window.Hwnd and checking if the window is visible.
    /// </summary>
    /// <param name="dialog">The dialog.</param>
    /// <returns><c>true</c> if exists and visible, otherwise <c>false</c></returns>
	  public bool Exists(Window dialog)
	  {
      return dialog.Exists() && dialog.Visible;

//	    bool exists = dialog.Exists();
//      Logger.LogAction("Exists: exists == " + exists.ToString());
//
//	    bool visible = dialog.Visible;
//      Logger.LogAction("Exists: visible == " + visible.ToString());
//      
//      return exists && visible;
	  }

    /// <summary>
    /// Checks if window is null or <see cref="Exists"/>.
    /// </summary>
    /// <param name="dialog">The dialog.</param>
    /// <returns><c>true</c> if null or exists, otherwise <c>false</c></returns>
	  public bool ExistsOrNull(Window dialog)
	  {
	    if (dialog == null)
	    {
//	      Logger.LogAction("ExistsOrNull: dialog == null");
        return true;
	    }

	    return Exists(dialog);
	  }

    /// <summary>
	  /// Wait until the save/open/run dialog opens.
	  /// This exists because some web servers are slower to start a file than others.
	  /// </summary>
	  /// <param name="waitDurationInSeconds">duration in seconds to wait</param>
	  public void WaitUntilFileDownloadDialogIsHandled(int waitDurationInSeconds)
	  {
		  SimpleTimer timeoutTimer = new SimpleTimer(waitDurationInSeconds);

		  while (!HasHandledFileDownloadDialog && !timeoutTimer.Elapsed)
		  {
//        Logger.LogAction("WaitUntilFileDownloadDialogIsHandled");
			  Thread.Sleep(200);
		  }
      
		  if (!HasHandledFileDownloadDialog)
		  {
			  throw new WatiNException(string.Format("Has not shown dialog after {0} seconds.", waitDurationInSeconds.ToString()));
		  }
	  }

    /// <summary>
    /// Wait until the download progress window does not exist any more
    /// </summary>
    /// <param name="waitDurationInSeconds">duration in seconds to wait</param>
    public void WaitUntilDownloadCompleted(int waitDurationInSeconds)
    {
      SimpleTimer timeoutTimer = new SimpleTimer(waitDurationInSeconds);

      while (ExistsOrNull(DownloadProgressDialog) && !timeoutTimer.Elapsed)
      {
//        Logger.LogAction("WaitUntilDownloadCompleted");
        Thread.Sleep(200);
      }
      
      if (ExistsOrNull(DownloadProgressDialog))
      {
        throw new WatiNException(string.Format("Still downloading after {0} seconds.", waitDurationInSeconds.ToString()));
      }

      Logger.LogAction("Download complete at " + DateTime.Now.ToLongTimeString());
    }

    private Window DownloadProgressDialog
    {
      get { return downloadProgressDialog; }
      set
      {
//        Logger.LogAction("Entering DownloadProgressDialog.");
        if (downloadProgressDialog == null)
        {
//          Logger.LogAction("downloadProgressDialog == null");
          
          Window dialog = value;

          if (dialog != null)
          {
//            Logger.LogAction("!dialog.Exists()");

            if (!dialog.Exists())
            {
//              Logger.LogAction("download progress dialog should exist.");
              dialog = null;
            }

//            Logger.LogAction("!IsDownloadProgressDialog(dialog)");
            if (!IsDownloadProgressDialog(dialog))
            {
//              Logger.LogAction("Should be a download progress dialog.");
              dialog = null;
            }
          }

//          Logger.LogAction("dialog != null");
          if (dialog != null)
          {
//            Logger.LogAction("Set downloadProgressDialog field.");
            downloadProgressDialog = dialog;
          }
          else
          {
//            Logger.LogAction("downloadProgressDialog not set.");
          }
        }
      }
    }

    private void HandleFileSaveDialog(Window window)
    {      
      IntPtr usernameControlHandle = NativeMethods.GetChildWindowHwnd(window.Hwnd, "Edit");

      NativeMethods.SetForegroundWindow(usernameControlHandle);
      NativeMethods.SetActiveWindow(usernameControlHandle);

      System.Windows.Forms.SendKeys.SendWait(saveAsFilename + "{ENTER}");
    }
  }
  
  public class SimpleJavaDialogHandler : BaseDialogHandler
  {
    JavaDialogHandler dialogHandler;
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

    public override bool HandleDialog(Window window)
    {
      if (dialogHandler.CanHandleDialog(window))
      {
        dialogHandler.window = window;

        message = dialogHandler.Message;

        ConfirmDialogHandler confirmDialogHandler = dialogHandler as ConfirmDialogHandler;

        // hasHandledDialog must be set before the Click and not
        // after because this code executes on a different Thread
        // and could lead to property HasHandledDialog returning false
        // while hasHandledDialog set had to be set.
        hasHandledDialog = true;

        if (confirmDialogHandler != null && clickCancelButton)
        {
          confirmDialogHandler.CancelButton.Click();
        }
        else
        {
          dialogHandler.OKButton.Click();
        }
      }
      
      return hasHandledDialog;
    }
  }
  
  public abstract class BaseDialogHandler : IDialogHandler
  {
    public override bool Equals(object obj)
    {
      if (obj == null) return false;
      
      return (GetType().Equals(obj.GetType()));
    }

    public override int GetHashCode()
    {
      return GetType().ToString().GetHashCode();
    }
    #region IDialogHandler Members

    public abstract bool HandleDialog(Window window);

    #endregion
  }
  
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

  public class UseDialogOnce : IDisposable
  {
    private DialogWatcher dialogWatcher;
    private IDialogHandler dialogHandler;

    public UseDialogOnce(DialogWatcher dialogWatcher, IDialogHandler dialogHandler)
    {
      if (dialogWatcher == null)
      {
        throw new ArgumentNullException("dialogWatcher");
      }

      if (dialogHandler == null)
      {
        throw new ArgumentNullException("dialogHandler");
      }

      this.dialogWatcher = dialogWatcher;
      this.dialogHandler = dialogHandler;

      dialogWatcher.Add(dialogHandler);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
      return;
    }

    protected virtual void Dispose(bool managedAndNative)
    {
      dialogWatcher.Remove(dialogHandler);

      dialogWatcher = null;
      dialogHandler = null;
    }
  }
}
