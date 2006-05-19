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
using System.Diagnostics;
using System.Threading;

using mshtml;

using SHDocVw;

using WatiN.Core;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.Core
{
  public class IE : DomContainer, IDisposable
  {
    const int waitForWindowTime = 60;

    private InternetExplorer ie;

    private bool autoClose = true;
    private PopupWatcher popupWatcher;
    private Thread popupWatcherThread;

    /// <summary>
    /// Attach to an existing Internet Explorer by url or title. Internet Explorer will be closed when destroying this object.
    /// </summary>
    /// <param name="findBy">The Url to find IE</param>
    public static IE AttachToIE(UrlValue findBy)
    {
      return AttachToIE(findBy, waitForWindowTime);
    }

    /// <summary>
    /// Attach to an existing Internet Explorer by url or title. Internet Explorer will be closed when destroying this object.
    /// </summary>
    /// <param name="findBy">The (partial) Title of the IE window to find</param>
    public static IE AttachToIE(TitleValue findBy)
    {
      return AttachToIE(findBy, waitForWindowTime);
    }

    /// <summary>
    /// Attach to an existing Internet Explorer by url or title. Internet Explorer will be closed when destroying this object.
    /// </summary>
    /// <param name="findBy">The Url to find IE</param>
    /// <param name="timeout">The number of seconds to wait before timing out</param>
    public static IE AttachToIE(UrlValue findBy, int timeout)
    {
      return attachToIE(findBy, timeout);
    }

    /// <summary>
    /// Attach to an existing Internet Explorer by url or title. Internet Explorer will be closed when destroying this object.
    /// </summary>
    /// <param name="findBy">Valid attributes are Url and Title</param>
    /// <param name="timeout">The number of seconds to wait before timing out</param>
    public static IE AttachToIE(TitleValue findBy, int timeout)
    {
      return attachToIE(findBy, timeout);
    }

   private static IE attachToIE(AttributeValue findBy, int timeout)
    {
      Logger.LogAction("Finding IE instance with " + findBy.AttributeName + " '" + findBy.Value + "'");

      return FindIE(findBy, timeout);
    }

    /// <summary>
    /// This is a convenience method for new IECollection
    /// </summary>
    /// <returns></returns>
    public static IECollection InternetExplorers()
    {
      return new IECollection();
    }

    /// <summary>
    /// Open new Internet Explorer with a blank page. Internet Explorer will be closed when destroying this object.
    /// </summary>
    public IE()
    {
      CreateNewIE("about:blank", true);
    }

    /// <summary>
    /// Open new Internet Explorer and goto url. Internet Explorer will be closed when destroying this object.
    /// </summary>
    /// <param name="url">The URL te open</param>
    public IE(string url)
    {
      CreateNewIE(url, true);
    }

    /// <summary>
    /// Open new Internet Explorer with a blank page.
    /// </summary>
    /// <param name="autoClose">Close Internet Explorer when destroying this object.</param>
    public IE(bool autoClose)
    {
      CreateNewIE("about:blank", autoClose);
    }

    /// <summary>
    /// Open new Internet Explorer and goto url.
    /// </summary>
    /// <param name="url">The URL te open</param>
    /// <param name="autoClose">Close Internet Explorer when destroying this object.</param>
    public IE(string url, bool autoClose)
    {
      CreateNewIE(url, autoClose);
    }

    /// <summary>
    /// Use existing InternetExplorer object. The param is of type
    /// object because otherwise all projects using WatiN should also
    /// reference the Interop.SHDocVw assembly.
    /// </summary>
    /// <param name="shDocVwInternetExplorer">The Interop.SHDocVw.InternetExplorer object to use</param>
    internal IE(object shDocVwInternetExplorer)
    {
      InternetExplorer ie = null;
      try
      {
        ie = (InternetExplorer) shDocVwInternetExplorer;
      }
      catch(System.InvalidCastException)
      {
        throw new ArgumentException("SHDocVwInternetExplorer needs to be of type SHDocVw.InternetExplorer");
      }

      InitIEAndStartPopupWatcher(ie);
      WaitForComplete();
    }

    private void CreateNewIE(string url, bool autoClose)
    {
      Logger.LogAction("Creating new IE instance");

      SetAutoCloseAndMoveMouse(autoClose);

      InitIEAndStartPopupWatcher(new InternetExplorerClass());

      GoTo(url);
    }

    private void InitIEAndStartPopupWatcher(InternetExplorer ie)
    {
      this.ie = ie;      
      this.ie.Visible = true;

      int iePid = GetProcessID();
      StartPopupWatcher(iePid);
    }

    private int GetProcessID()
    {
      int iePid = 0;
      IntPtr hwnd = GetIEHandle();
      foreach (Process p in Process.GetProcesses())
      {
        if (p.MainWindowHandle == hwnd)
        {
          iePid = p.Id;
        }
      }
      return iePid;
    }

    private static IE FindIE(AttributeValue findBy, int timeout)
    {
      DateTime startTime = DateTime.Now;

      while (NotTimedOut(startTime, timeout))
      {
        Thread.Sleep(500);

        ShellWindows allBrowsers = new ShellWindows();

        int browserCount = allBrowsers.Count;
        int browserCounter = 0;

        while (browserCounter < browserCount)
        {
          InternetExplorer e = (InternetExplorer) allBrowsers.Item(browserCounter);

          string compareValue = string.Empty;

          if (findBy is UrlValue)
          {
            try
            {
              compareValue = e.LocationURL;
            }
            catch
            {}
          }

          else if(findBy is TitleValue)
          {
            try
            {
              compareValue = ((HTMLDocument) e.Document).title;
            }
            catch
            {}
          }

          if (findBy.Compare(compareValue))
          {
            return new IE(e);
          }

          browserCounter++;
        }
      }

      throw new IENotFoundException(findBy.AttributeName, findBy.Value, timeout);
    }

    private void SetAutoCloseAndMoveMouse(bool autoClose)
    {
      AutoCloseIE = autoClose;

      if (autoClose)
      {
        // We assume that when this flag is on, everything should run automatically.
        // Better move the mouse out of the way.
        System.Windows.Forms.Cursor.Position = new System.Drawing.Point(0, 0);
      }
    }

    public void GoTo(Uri url)
    {
      GoTo(url.ToString());
    }

    public void GoTo(string url)
    {
      Logger.LogAction("Navigating to '" + url + "'");
      
      object nil = null;
      ie.Navigate(url, ref nil, ref nil, ref nil, ref nil);
      WaitForComplete();
      
    }

    private void StartPopupWatcher(int iePid)
    {
      popupWatcher = new PopupWatcher(iePid);
      popupWatcherThread = new Thread(new ThreadStart(popupWatcher.run));

      // Start the thread.
      popupWatcherThread.Start();
    }

    /// <summary>
    /// Use this to gain access to the 'raw' internet explorer object.
    /// Cast the object to type SHDocVw.InternetExplorer found in the
    /// assembly Interop.SHDocVw supplied in the WatiN distribution.
    /// </summary>
    public object InternetExplorer
    {
      get
      {
        return ie;
      }
    }

    /// <summary>
    /// Returns the current url, as displayed in the address bar of the browser
    /// </summary>
    public string Url
    {
      get { return ie.LocationURL; }
    }

    public string PopAlert()
    {
      return popupWatcher.popAlert();
    }

    public void FlushAlerts()
    {
      popupWatcher.flushAlerts();
    }

    public override void FireEvent(DispHTMLBaseElement element, string eventName)
    {
      // The code in base.FireEvent should work, but it doesn't. 
      // The button value inside the event handling scripts in the html page 
      // is still 0 while explicitly set to 1 in the code
      // I've searched the internet, but this really should be it.
      // Since it doesn't work, I came up with this workaround using execScript.
      // Only this doesn't work if the elements ID attribute isn't set in the html

      // TODO: Passing the eventarguments in a new param of type array. This array
      //       holds 0 or more name/value pairs where the name is a property of the event object
      //       and the value is the value that's assigned to the property.

      if (element.id != null)
      {
        FireEventOnElementByJScript(element, eventName);
      }
      else
      {
        base.FireEvent(element, eventName);
      }
    }

    private void FireEventOnElementByJScript(DispHTMLBaseElement element, string eventName)
    {
      string scriptCode = "var newEvt = document.createEventObject();";
      scriptCode += "newEvt.button = 1;";
      scriptCode += "document.getElementById('" + element.id + "').fireEvent('" + eventName + "', newEvt);";

      IHTMLWindow2 window = ((HTMLDocument) element.document).parentWindow;

      try
      {
        window.execScript(scriptCode, "javascript");
      }
      catch 
      {
        base.FireEvent(element, eventName);
      }
    }

    public void Back()
    {
      ie.GoBack();
      WaitForComplete();
      Logger.LogAction("Navigated Back to '" + Url + "'");
    }

    public void Forward()
    {
      ie.GoForward();
      WaitForComplete();
      Logger.LogAction("Navigated Forward to '" + Url + "'");
    }

    public void Refresh()
    {
      Logger.LogAction("Refreshing browser from '" + Url + "'");

      object REFRESH_COMPLETELY = 3;
      ie.Refresh2(ref REFRESH_COMPLETELY);
      WaitForComplete();
    }

    public void BringToFront()
    {
      IntPtr ieHandle = GetIEHandle();

      if (NativeMethods.GetForegroundWindow() != ieHandle)
      {
        NativeMethods.SetForegroundWindow(ieHandle);
      }
    }

    public void ShowWindow(NativeMethods.WindowShowStyle showStyle)
    {
      NativeMethods.ShowWindow(GetIEHandle(), (int)showStyle);
    }

    public void Close()
    {
      Logger.LogAction("Closing browser '" + MainDocument.Title + "'");
      StopPopupWatcherAndQuitIE();
    }

    private IntPtr GetIEHandle()
    {
      return new IntPtr(ie.HWND);
    }

    private int StopPopupWatcherAndQuitIE()
    {
      popupWatcher.Stop();
      popupWatcherThread.Join();

      foreach(HtmlDialog htmlDialog in HtmlDialogs)
      {
        htmlDialog.Close();
      }

      base.Dispose();

      int iePid = GetProcessID();

      ie.Quit(); // ask IE to close
      ie = null;
      Thread.Sleep(1000); // wait for IE to close by itself

      return iePid;
    }

    public virtual void ForceClose()
    {
      Logger.LogAction("Force closing all IE instances");

      int iePid = StopPopupWatcherAndQuitIE();

      try
      {
        Process.GetProcessById(iePid).Kill(); // force IE to close if needed
        Debug.WriteLine("IE didn't close by itself, so we explicitly killed it");
      }
      catch (ArgumentException)
      {
        // ignore: IE is no longer running
      }
    }

    public override IHTMLDocument2 OnGetHtmlDocument()
    {
      return (IHTMLDocument2)ie.Document;
    }


    public override void WaitForComplete()
    {
      InitTimeout();

      WaitWhileIEBusy();
      WaitWhileIEStateNotComplete();
      
      WaitForCompleteTimeoutIsInitialized();
    }

    private void WaitWhileIEStateNotComplete()
    {
      while (ie.ReadyState !=  tagREADYSTATE.READYSTATE_COMPLETE)
      {
        ThrowExceptionWhenTimeout("Internet Explorer state not complete");

        Thread.Sleep(100);        
      }
    }

    private void WaitWhileIEBusy()
    {
      while (ie.Busy)
      {
        ThrowExceptionWhenTimeout("Internet Explorer busy");

        Thread.Sleep(100);
      }
    }

    #region IDisposable Members

    public new void Dispose()
    {
      if (AutoCloseIE)
      {
        Close();
      }

      if (popupWatcher.alertCount() != 0)
      {
//        throw new UnanticipatedAlertsException(popupWatcher.alerts);
      }
    }
    #endregion

    public bool AutoCloseIE
    {
      get
      {
        return autoClose;
      }
      set
      {
        autoClose = value;
      }
    }
    public HtmlDialogCollection HtmlDialogs
    {
      get
      {
        Process p = Process.GetProcessById(GetProcessID());
        HtmlDialogCollection htmlDialogCollection = new HtmlDialogCollection(p); 

        return htmlDialogCollection;
      }
    }

    /// <summary>
    /// Find a HtmlDialog
    /// </summary>
    /// <param name="findBy">The url of the html page shown in the dialog</param>
    public HtmlDialog HtmlDialog(UrlValue findBy)
    {
      return HtmlDialog(findBy, waitForWindowTime);
    }

    /// <summary>
    /// Find a HtmlDialog
    /// </summary>
    /// <param name="findBy">The Title of the html page</param>
    public HtmlDialog HtmlDialog(TitleValue findBy)
    {
      return HtmlDialog(findBy, waitForWindowTime);
    }

    /// <summary>
    /// Find a HtmlDialog
    /// </summary>
    /// <param name="findBy">The url of the html page shown in the dialog</param>
    /// <param name="timeout">Number of seconds before the search times out.</param>
    public HtmlDialog HtmlDialog(UrlValue findBy, int timeout)
    {
      return findHtmlDialog(findBy, timeout);
    }

    /// <summary>
    /// Find a HTMLDialog
    /// </summary>
    /// <param name="findBy">The Title of the html page</param>
    /// <param name="timeout">Number of seconds before the search times out.</param>
    public HtmlDialog HtmlDialog(TitleValue findBy, int timeout)
    {
      return findHtmlDialog(findBy, timeout);
    }

    private HtmlDialog findHtmlDialog(AttributeValue findBy, int timeout)
    {
      Logger.LogAction("Finding HTMLDialog with " + findBy.AttributeName + " '" + findBy.Value + "'");

      DateTime startTime = DateTime.Now;

      while (NotTimedOut(startTime, timeout))
      {
        Thread.Sleep(500);

        foreach(HtmlDialog htmlDialog in HtmlDialogs)
        {
          string compareValue = string.Empty;

          if (findBy is UrlValue)
          {
            compareValue = htmlDialog.MainDocument.Url;
          }

          else if (findBy is TitleValue)
          {
            compareValue = htmlDialog.MainDocument.Title;
          }

          if (findBy.Compare(compareValue))
          {
            return htmlDialog;
          }
        }
      }

      throw new HtmlDialogNotFoundException(findBy.AttributeName, findBy.Value, timeout);
    }

    private static bool NotTimedOut(DateTime startTime, int durationInSeconds)
    {
      return !IsTimedOut(startTime, durationInSeconds);
    }
  }
}

