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

    public DialogWatcher(int ieProcessId)
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

    /// <summary>
    /// Called by the constructor to start watching popups
    /// on a separate thread.
    /// </summary>
    public void Start()
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
        process = Process.GetProcessById(ieProcessId);
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
        
        // If no dialogHandler handled the dialog, the
        // defaultHandler will close the dialog.
        if (defaultHandler != null)
        {
          DefaultHandler.HandleDialog(window);
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
  
  public class LogonDialogHandler : IDialogHandler
  {
    private string userName;
    private string password;
    
    public LogonDialogHandler(string userName, string password)
    {
      if (UtilityClass.IsNullOrEmpty(userName))
      {
        throw new ArgumentException("Username must be specified", "username");
      }

      if (UtilityClass.IsNullOrEmpty(password))
      {
        throw new ArgumentException("Password must be specified", "password");
      }
      
      this.userName = userName;
      this.password = password;
    }
    
    public bool HandleDialog(Window window)
    {
      if (IsLogonDialog(window))
//      if (IsLogonDialog(window.Title))
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

    public virtual bool IsLogonDialog(string message)
    {
      return message.StartsWith("Connect to") || message.StartsWith("Enter Network Password");
    }
    
    public virtual bool IsLogonDialog(Window window)
    {
      IntPtr hWnd = window.Hwnd;
      
      // Get 1st child IE server window
      NativeMethods.EnumChildProc childProc = new NativeMethods.EnumChildProc(EnumChildForSysCredentials);
      NativeMethods.EnumChildWindows(window.Hwnd, childProc, ref hWnd);
      
      return IsSysCredentialsWindow(hWnd);

//      return message.StartsWith("Connect to") || message.StartsWith("Enter Network Password");
    }
    
    private bool EnumChildForSysCredentials(IntPtr hWnd, ref IntPtr lParam)
    {
      if (IsSysCredentialsWindow(hWnd))
      {
        lParam = hWnd;
        return false;
      }
      else
      {
        return true;
      }
    }

    private static bool IsSysCredentialsWindow(IntPtr hWnd)
    {
      return UtilityClass.CompareClassNames(hWnd, "SysCredential");
    }

  }
}