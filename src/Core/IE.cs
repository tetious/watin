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
using System.Runtime.InteropServices;
using System.Threading;

using mshtml;

using SHDocVw;

using WatiN.Core;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.Core
{
  /// <summary>
  /// This is the main class to access a webpage and all it's elements
  /// in Internet Explorer
  /// </summary>
  /// <example>
  /// The following example creates a new Internet Explorer instances and navigates to
  /// the WatiN Project website on SourceForge.
  /// <code>
  /// using WatiN.Core;
  /// 
  /// namespace NewIEExample
  /// {
  ///    public class WatiNWebsite
  ///    {
  ///      public WatiNWebsite()
  ///      {
  ///        IE ie = new IE("http://watin.sourceforge.net");
  ///        ie.Link(Find.ByText("RSS Feeds")).Click;
  ///        ie.Close;
  ///      }
  ///    }
  ///  }
  /// </code>
  /// </example>
  public class IE : DomContainer, IDisposable
  {
    const int waitForWindowTime = 60;

    private InternetExplorer ie;

    private bool autoClose = true;
    private DialogWatcher dialogWatcher;

    /// <summary>
    /// Attach to an existing Internet Explorer by it's Url. The attached Internet Explorer will be closed after destroying the IE instance.
    /// </summary>
    /// <param name="findBy">The Url to find.</param>
    /// <returns>An instance of IE. If multiple Internet Explorer windows have the same 
    /// Url, the first match will be returned. </returns>
    /// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
    /// IENotFoundException will be throw if an Internet Explorer window with the given Url isn't found within 30 seconds.
    /// </exception>
    /// <example>
    /// Assuming you have http://www.example.com open in an Internet Explorer window,
    /// this example will search for this Internet Explorer window and Attach to it so you
    /// can start manipulating the page.
    /// <code>
    /// IE ieGoogle = IE.AttachToIE(Find.ByUrl("http://www.example.com"));
    /// </code>
    /// </example>
    public static IE AttachToIE(Url findBy)
    {
      return findIE(findBy, waitForWindowTime);
    }

    /// <summary>
    /// Attach to an existing Internet Explorer by it's Url. The attached Internet Explorer will be closed after destroying the IE instance.
    /// </summary>
    /// <param name="findBy">The Url to find.</param>
    /// <param name="timeout">The number of seconds to wait before attach times out</param>
    /// <returns>An instance of IE. If multiple Internet Explorer windows have the same 
    /// Url, the first match will be returned. </returns>
    /// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
    /// IENotFoundException will be throw if an Internet Explorer window with the given Url isn't found within the specified <paramref name = "timeout" />.
    /// </exception>
    /// <example>
    /// Assuming you have http://www.example.com open in an Internet Explorer window,
    /// this example will search for this Internet Explorer window and Attach to it so you
    /// can start manipulating the page.
    /// <code>
    /// IE ieGoogle = IE.AttachToIE(Find.ByUrl("http://www.example.com"));
    /// </code>
    /// </example>
    public static IE AttachToIE(Url findBy, int timeout)
    {
      return findIE(findBy, timeout);
    }
    
    /// <summary>
    /// Attach to an existing Internet Explorer by it's title. If multiple Internet Explorer windows have the same or partially 
    /// the same Title, the first match will be returned. The attached Internet Explorer will be closed after destroying the IE instance.
    /// </summary>
    /// <param name="findBy">The (partial) Title of the IE window to find</param>
    /// <returns>An instance of IE. If multiple Internet Explorer windows have the same 
    /// Url, the first match will be returned. </returns>
    /// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
    /// IENotFoundException will be throw if an Internet Explorer window with the given Title isn't found within 30 seconds.
    /// </exception>
    /// <example>
    /// Assuming you have an Internet Explorer window open with a titel "Example" (this 
    /// will show as "Example - Microsoft Internet Explorer" in the titlebar).
    /// This example will search for this Internet Explorer window and Attach to it so you
    /// can start manipulating the page 
    /// <code>
    /// IE ieGoogle = IE.AttachToIE(Find.ByUrlTitle("Example"));
    /// </code>
    /// A parial match should also work
    /// <code>
    /// IE ieGoogle = IE.AttachToIE(Find.ByUrlTitle("Exa"));
    /// </code>
    /// </example>

    public static IE AttachToIE(Title findBy)
    {
      return findIE(findBy, waitForWindowTime);
    }

    /// <summary>
    /// Attach to an existing Internet Explorer by it's title. If multiple Internet Explorer windows have the same or partially 
    /// the same Title, the first match will be returned. The attached Internet Explorer will be closed after destroying the IE instance.
    /// </summary>
    /// <param name="findBy">The (partial) Title of the IE window to find</param>
    /// <param name="timeout">The number of seconds to wait before timing out</param>
    /// <returns>An instance of IE. If multiple Internet Explorer windows have the same 
    /// Url, the first match will be returned. </returns>
    /// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
    /// IENotFoundException will be throw if an Internet Explorer window with the given Title isn't found within the specified <paramref name = "timeout" />.
    /// </exception>
    /// <example>
    /// Assuming you have an Internet Explorer window open with a titel "Example" (this 
    /// will show as "Example - Microsoft Internet Explorer" in the titlebar).
    /// This example will search for this Internet Explorer window and Attach to it so you
    /// can start manipulating the page 
    /// <code>
    /// IE ieGoogle = IE.AttachToIE(Find.ByUrlTitle("Example"));
    /// </code>
    /// A parial match should also work
    /// <code>
    /// IE ieGoogle = IE.AttachToIE(Find.ByUrlTitle("Exa"));
    /// </code>
    /// </example>
    public static IE AttachToIE(Title findBy, int timeout)
    {
      return findIE(findBy, timeout);
    }

    /// <summary>
    /// Creates a collection of new InternetExplorer objects and associates them with open Internet Explorers.
    /// </summary>
    /// <example>
    /// This code snippet illustrates the use of this method:
    /// <code>IECollection InternetExplorers = IE.InternetExplorers</code>
    /// The following code gives the same result:
    /// <code>IECollection InternetExplorers = new IECollection</code>
    /// </example>
    public static IECollection InternetExplorers()
    {
      return new IECollection();
    }

    /// <summary>
    /// Opens a new Internet Explorer with the Url pointing at a blank page. 
    /// <note>
    /// When the <see cref="WatiN.Core.IE" />
    /// instance is destroyed the openend Internet Explore will also be closed.
    /// </note>
    /// </summary>
    /// <remarks>
    /// You could also use one of the overloaded constructors.
    /// </remarks>
    /// <example>
    /// The following example creates a new Internet Explorer instances and navigates to
    /// the WatiN Project website on SourceForge.
    /// <code>
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE();
    ///        ie.GoTo("http://watin.sourceforge.net");
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE()
    {
      createNewIEAndGotToUrl("about:blank", true);
    }

    /// <summary>
    /// Opens a new Internet Explorer and navigates to the given <paramref name="url"/>.
    /// <note>
    /// When the <see cref="WatiN.Core.IE" />
    /// instance is destroyed the openend Internet Explore will also be closed.
    /// </note>
    /// </summary>
    /// <param name="url">The URL te open</param>
    /// <remarks>
    /// You could also use one of the overloaded constructors.
    /// </remarks>
    /// <example>
    /// The following example creates a new Internet Explorer instances and navigates to
    /// the WatiN Project website on SourceForge.
    /// <code>
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE("http://watin.sourceforge.net");
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE(string url)
    {
      createNewIEAndGotToUrl(url, true);
    }
    /// <summary>
    /// Opens a new Internet Explorer and navigates to the given <paramref name="uri"/>.
    /// <note>
    /// When the <see cref="WatiN.Core.IE" />
    /// instance is destroyed the openend Internet Explore will also be closed.
    /// </note>
    /// </summary>
    /// <param name="uri">The Uri te open</param>
    /// <remarks>
    /// You could also use one of the overloaded constructors.
    /// </remarks>
    /// <example>
    /// The following example creates a new Internet Explorer instances and navigates to
    /// the WatiN Project website on SourceForge.
    /// <code>
    /// using System;
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE(new Uri("http://watin.sourceforge.net"));
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE(Uri uri)
    {
      createNewIEAndGotToUrl(uri, true);
    }

    /// <summary>
    /// Opens a new Internet Explorer with the Url pointing at a blank page. The <paramref name="autoClose" />
    /// parameter provides the option to <i>not</i> close the created Internet Explorer when the
    /// corresponding <see cref="WatiN.Core.IE" /> instance is destroyed. 
    /// </summary>
    /// <param name="autoClose">Close Internet Explorer when destroying this object.</param>
    /// <remarks>
    /// You could also use one of the overloaded constructors.
    /// </remarks>
    /// <example>
    /// The following example creates a new Internet Explorer instances and navigates to
    /// the WatiN Project website on SourceForge leaving the created Internet Explorer open.
    /// <code>
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE(false);
    ///        ie.GoTo("http://watin.sourceforge.net");
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE(bool autoClose)
    {
      createNewIEAndGotToUrl("about:blank", autoClose);
    }

    /// <summary>
    /// Opens a new Internet Explorer and navigates to the given <paramref name="url"/>. The <paramref name="autoClose" />
    /// parameter provides the option to <i>not</i> close the created Internet Explorer when the
    /// corresponding <see cref="WatiN.Core.IE" /> instance is destroyed. 
    /// </summary>
    /// <param name="url">The Url te open</param>
    /// <param name="autoClose">Close Internet Explorer when destroying this object.</param>
    /// <remarks>
    /// You could also use one of the overloaded constructors.
    /// </remarks>
    /// <example>
    /// The following example creates a new Internet Explorer instances and navigates to
    /// the WatiN Project website on SourceForge leaving the created Internet Explorer open.
    /// <code>
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE("http://watin.sourceforge.net", false);
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE(string url, bool autoClose)
    {
      createNewIEAndGotToUrl(url, autoClose);
    }
    /// <summary>
    /// Opens a new Internet Explorer and navigates to the given <paramref name="uri"/>. The <paramref name="autoClose" />
    /// parameter provides the option to <i>not</i> close the created Internet Explorer when the
    /// corresponding <see cref="WatiN.Core.IE" /> instance is destroyed. 
    /// </summary>
    /// <param name="uri">The Uri te open</param>
    /// <param name="autoClose">Close Internet Explorer when destroying this object.</param>
    /// <remarks>
    /// You could also use one of the overloaded constructors.
    /// </remarks>
    /// <example>
    /// The following example creates a new Internet Explorer instances and navigates to
    /// the WatiN Project website on SourceForge leaving the created Internet Explorer open.
    /// <code>
    /// using System;
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE(new Uri("http://watin.sourceforge.net"), false);
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE(Uri uri, bool autoClose)
    {
      createNewIEAndGotToUrl(uri, autoClose);
    }

    /// <summary>
    /// Use existing InternetExplorer object. The param is of type
    /// object because otherwise all projects using WatiN should also
    /// reference the Interop.SHDocVw assembly (yes also with the interal
    /// access modifier)
    /// </summary>
    /// <param name="shDocVwInternetExplorer">The Interop.SHDocVw.InternetExplorer object to use</param>
    internal IE(object shDocVwInternetExplorer)
    {
      CheckThreadApartmentStateIsSTA();

      InternetExplorer internetExplorer;
      try
      {
        internetExplorer = (InternetExplorer) shDocVwInternetExplorer;
      }
      catch(InvalidCastException)
      {
        throw new ArgumentException("SHDocVwInternetExplorer needs to be of type SHDocVw.InternetExplorer");
      }

      InitIEAndStartPopupWatcher(internetExplorer);
    }

    private void createNewIEAndGotToUrl(string url, bool autoClose)
    {
      createNewIE(autoClose);

      GoTo(url);
    }
    
    private void createNewIEAndGotToUrl(Uri uri, bool autoClose)
    {
      createNewIE(autoClose);

      GoTo(uri);
    }

    private void createNewIE(bool autoClose)
    {
      CheckThreadApartmentStateIsSTA();

      Logger.LogAction("Creating new IE instance");

      SetAutoCloseAndMoveMouse(autoClose);

      InitIEAndStartPopupWatcher(new InternetExplorerClass());
    }

    private static void CheckThreadApartmentStateIsSTA()
    {
      
#if NET11      
      if (Thread.CurrentThread.ApartmentState != ApartmentState.STA)
      {
        throw new ThreadStateException("The CurrentThread needs to have it's ApartmentState set to ApartmentState.STA to be able to automate Internet Explorer.");
      }
#elif NET20
      // Code for .Net 2.0
      if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
      {
        throw new ThreadStateException("The CurrentThread needs to have it's ApartmentState set to ApartmentState.STA to be able to automate Internet Explorer.");
      }
#endif
    } 
        
    private void InitIEAndStartPopupWatcher(InternetExplorer internetExplorer)
    {
      ie = internetExplorer;      
      ie.Visible = true;

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

    private static IE findIE(Attribute findBy, int timeout)
    {
      if (timeout < 0)
      {
        throw new ArgumentOutOfRangeException("timeout", timeout, "Should be equal are greater then zero.");
      }
      
      Logger.LogAction("Busy finding Internet Explorer with " + findBy.AttributeName + " '" + findBy.Value + "'");

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

          if (findBy is Url)
          {
            try
            {
              compareValue = e.LocationURL;
            }
            catch
            {}
          }

          else if(findBy is Title)
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
            IE ie = new IE(e);
            ie.WaitForComplete();
            
            return ie;
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

    
    /// <summary>
    /// Navigates Internet Explorer to the given <paramref name="url" />.
    /// </summary>
    /// <param name="url">The URL specified as a wel formed Uri.</param>
    /// <example>
    /// The following example creates a new Uri, Internet Explorer instances and navigates to
    /// the WatiN Project website on SourceForge.
    /// <code>
    /// using WatiN.Core;
    /// using System;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        Uri URL = new Uri("http://watin.sourceforge.net");
    ///        IE ie = new IE();
    ///        ie.GoTo(URL);
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public void GoTo(Uri url)
    {
      Logger.LogAction("Navigating to '" + url + "'");
      
      object nil = null;
      ie.Navigate(url.ToString(), ref nil, ref nil, ref nil, ref nil);
      WaitForComplete();
    }

    /// <summary>
    /// Navigates Internet Explorer to the given <paramref name="url" />.
    /// </summary>
    /// <param name="url">The URL to GoTo.</param>
    /// <example>
    /// The following example creates a new Internet Explorer instances and navigates to
    /// the WatiN Project website on SourceForge.
    /// <code>
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE();
    ///        ie.GoTo("http://watin.sourceforge.net");
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public void GoTo(string url)
    {
      GoTo(new Uri(url));
    }

    private void StartPopupWatcher(int iePid)
    {
      dialogWatcher = new DialogWatcher(iePid);
    }

    /// <summary>
    /// Use this method to gain access to the full Internet Explorer object.
    /// Do this by referencing the Interop.SHDocVw assembly (supplied in the WatiN distribution)
    /// and cast the return value of this method to type SHDocVw.InternetExplorer.
    /// </summary>
    public object InternetExplorer
    {
      get
      {
        return ie;
      }
    }

    /// <summary>
    /// Pops the most recent alert message from a que of shown alerts.C:\TAdev\WatiNSF\trunk\src\UnitTests\FindElementBy.cs
    /// Use this method to get access to the displayed message in an alert window.
    /// </summary>
    /// <returns>The message displayed in the alert window</returns>
    /// <example>For a working example see the unitttest Alert in WatiN.UnitTests.IEAndMainDocument</example>
    public string PopAlert()
    {
      return dialogWatcher.PopAlert();
    }

    /// <summary>
    /// Flushes the alert messages in the que of displayed alert windows.
    /// </summary>
    public void FlushAlerts()
    {
      dialogWatcher.FlushAlerts();
    }

    /// <exclude />
    public override void FireEvent(DispHTMLBaseElement element, string eventName)
    {
      // The code in base.FireEvent should work, but it doesn't. 
      // The EventObject.button property in the java event handling scripts (in the html page) 
      // still returns 0 while explicitly set to 1 in the code.
      // I've searched the internet, but this really should be it.
      // Since it doesn't work, I came up with this workaround using execScript.

      // TODO: Passing the eventarguments in a new param of type array. This array
      //       holds 0 or more name/value pairs where the name is a property of the event object
      //       and the value is the value that's assigned to the property.

      bool removeIdAttribute = false;
      
      // If the element has no Id, assign a temporary and unique Id so we can find 
      // the element within the java code (I know, it's a bit dirty hack)
      if (element.id == null)
      {
        element.id = Guid.NewGuid().ToString();
        removeIdAttribute = true;
      }

      // Execute the JScriopt to fire the event inside the Browser.
      FireEventOnElementByJScript(element, eventName);
      
      // Remove Id attribute if temporary Id was assigned.
      if (removeIdAttribute)
      {
        element.removeAttribute("id", 0);
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

    /// <summary>
    /// Navigates the browser back to the previously displayed Url (like the back
    /// button in Internet Explorer). 
    /// </summary>
    public void Back()
    {
      ie.GoBack();
      WaitForComplete();
      Logger.LogAction("Navigated Back to '" + Url + "'");
    }

    /// <summary>
    /// Navigates the browser forward to the next displayed Url (like the forward
    /// button in Internet Explorer). 
    /// </summary>
    public void Forward()
    {
      ie.GoForward();
      WaitForComplete();
      Logger.LogAction("Navigated Forward to '" + Url + "'");
    }

    /// <summary>
    /// Reloads the currently displayed webpage (like the Refresh button in 
    /// Internet Explorer).
    /// </summary>
    public void Refresh()
    {
      Logger.LogAction("Refreshing browser from '" + Url + "'");

      object REFRESH_COMPLETELY = 3;
      ie.Refresh2(ref REFRESH_COMPLETELY);
      WaitForComplete();
    }

    /// <summary>
    /// Sends a Tab keyto the IE window to simulate tabbing through
    /// the elements (and adress bar).
    /// </summary>
    public void PressTab()

    {
      if (!Debugger.IsAttached )
      {
        int intThreadIDIE;
        int intCurrentThreadID;

        NativeMethods.WindowShowStyle currentStyle = GetWindowStyle();

        ShowWindow(NativeMethods.WindowShowStyle.Restore);
        BringToFront();
        
        intThreadIDIE = GetProcessID();
        intCurrentThreadID = NativeMethods.GetCurrentThreadId();
        
        NativeMethods.AttachThreadInput(intCurrentThreadID,intThreadIDIE, true);

        NativeMethods.keybd_event( NativeMethods.KEYEVENTF_TAB, 0x45, NativeMethods.KEYEVENTF_EXTENDEDKEY, 0 );
        NativeMethods.keybd_event( NativeMethods.KEYEVENTF_TAB, 0x45, NativeMethods.KEYEVENTF_EXTENDEDKEY | NativeMethods.KEYEVENTF_KEYUP, 0 );

        NativeMethods.AttachThreadInput(intCurrentThreadID,intThreadIDIE, false);
        
        ShowWindow(currentStyle);
      }
    }

    /// <summary>
    /// Brings the referenced Internet Explorer to the front (makes it the top window)
    /// </summary>
    public void BringToFront()
    {
      IntPtr ieHandle = GetIEHandle();

      if (NativeMethods.GetForegroundWindow() != ieHandle)
      {
        NativeMethods.SetForegroundWindow(ieHandle);
      }
    }

    /// <summary>
    /// Make the referenced Internet Explorer full screen, minimized, maximized and more.
    /// </summary>
    /// <param name="showStyle">The style to apply.</param>
    public void ShowWindow(NativeMethods.WindowShowStyle showStyle)
    {
      NativeMethods.ShowWindow(GetIEHandle(), (int)showStyle);
    }

    /// <summary>
    /// Gets the window style.
    /// </summary>
    /// <returns>The style currently applied to the ie window.</returns>
    public NativeMethods.WindowShowStyle GetWindowStyle()
    {
      NativeMethods.WINDOWPLACEMENT placement = new NativeMethods.WINDOWPLACEMENT();
      placement.length = Marshal.SizeOf(placement);
      
      NativeMethods.GetWindowPlacement(GetIEHandle(), ref placement);
      
      return (NativeMethods.WindowShowStyle)placement.showCmd;
    }
    
    /// <summary>
    /// Closes the referenced Internet Explorer. Almost
    /// all other functionality in this class and the element classes will give
    /// exceptions when used after closing the browser.
    /// </summary>
    /// <example>
    /// The following example creates a new Internet Explorer instances and navigates to
    /// the WatiN Project website on SourceForge. 
    /// <code>
    /// using WatiN.Core;
    /// using System.Diagnostics;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE("http://watin.sourceforge.net");
    ///        Debug.WriteLine(ie.Html);
    ///        ie.Close;
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public void Close()
    {
      Logger.LogAction("Closing browser '" + Title + "'");
      StopPopupWatcherAndQuitIE();
    }

    private IntPtr GetIEHandle()
    {
      return new IntPtr(ie.HWND);
    }

    private int StopPopupWatcherAndQuitIE()
    {
      dialogWatcher.Stop();

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

    /// <summary>
    /// Closes <i>all</i> running instances of Internet Explorer by killing the
    /// process these instances run in. 
    /// </summary>
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

    /// <exclude />
    public override IHTMLDocument2 OnGetHtmlDocument()
    {
      return (IHTMLDocument2)ie.Document;
    }

    /// <summary>
    /// Waits till the webpage, it's frames and all it's elements are loaded. This
    /// function is called by WatiN after each action (like clicking a link) so you
    /// should have to use this function on rare occasions.
    /// </summary>
    public override void WaitForComplete()
    {
      InitTimeout();

      waitWhileIEBusy();
      waitWhileIEStateNotComplete();
      
      WaitForCompleteOrTimeout();
    }

    private void waitWhileIEStateNotComplete()
    {
      while (ie.ReadyState !=  tagREADYSTATE.READYSTATE_COMPLETE)
      {
        ThrowExceptionWhenTimeout("Internet Explorer state not complete");

        Thread.Sleep(100);        
      }
    }

    private void waitWhileIEBusy()
    {
      while (ie.Busy)
      {
        ThrowExceptionWhenTimeout("Internet Explorer busy");

        Thread.Sleep(100);
      }
    }

    #region IDisposable Members

    /// <summary>
    /// This method must be called by its inheritor to dispose references
    /// to internal resources.
    /// </summary>
    public new void Dispose()
    {
      if (AutoCloseIE)
      {
        Close();
      }
    }
    #endregion

    /// <summary>
    /// Gets or sets a value indicating whether to auto close IE after destroying
    /// a reference to the corresponding IE instance.
    /// </summary>
    /// <value><c>true</c> when to auto close IE (this is the default); otherwise, <c>false</c>.</value>
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
    
    /// <summary>
    /// Returns a collection of open HTML dialogs (modal as well as modeless).
    /// </summary>
    /// <value>The HTML dialogs.</value>
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
    /// Find a HtmlDialog by it's Url.
    /// </summary>
    /// <param name="findBy">The url of the html page shown in the dialog</param>
    public HtmlDialog HtmlDialog(Url findBy)
    {
      return findHtmlDialog(findBy, waitForWindowTime);
    }

    /// <summary>
    /// Find a HtmlDialog by it's title.
    /// </summary>
    /// <param name="findBy">The Title of the html page</param>
    public HtmlDialog HtmlDialog(Title findBy)
    {
      return findHtmlDialog(findBy, waitForWindowTime);
    }

    /// <summary>
    /// Find a HtmlDialog by it's Url within the given <paramref name="timeout" /> period.
    /// </summary>
    /// <param name="findBy">The url of the html page shown in the dialog</param>
    /// <param name="timeout">Number of seconds before the search times out.</param>
    public HtmlDialog HtmlDialog(Url findBy, int timeout)
    {
      return findHtmlDialog(findBy, timeout);
    }

    /// <summary>
    /// Find a HtmlDialog by it's Title within the given <paramref name="timeout" /> period.
    /// </summary>
    /// <param name="findBy">The Title of the html page</param>
    /// <param name="timeout">Number of seconds before the search times out.</param>
    public HtmlDialog HtmlDialog(Title findBy, int timeout)
    {
      return findHtmlDialog(findBy, timeout);
    }

    private HtmlDialog findHtmlDialog(Attribute findBy, int timeout)
    {
      if (timeout < 0)
      {
        throw new ArgumentOutOfRangeException("timeout", timeout, "Should be equal are greater then zero.");
      }

      Logger.LogAction("Busy finding HTMLDialog with " + findBy.AttributeName + " '" + findBy.Value + "'");

      DateTime startTime = DateTime.Now;

      while (NotTimedOut(startTime, timeout))
      {
        Thread.Sleep(500);

        try
        {
          foreach(HtmlDialog htmlDialog in HtmlDialogs)
          {
            string compareValue = string.Empty;

            if (findBy is Url)
            {
              compareValue = htmlDialog.Url;
            }

            else if (findBy is Title)
            {
              compareValue = htmlDialog.Title;
            }

            if (findBy.Compare(compareValue))
            {
              htmlDialog.WaitForComplete();
              return htmlDialog;
            }
          }
        }
        catch 
        {
          // SF 1530859 fix
          // Accessing the DOM of the HTMLDialog page hasn't fully loaded yet 
          // raises an error. Using WaitForComplete at some point in the search
          // for a HTMLDialog would be a better option but for now I've implemented 
          // this catch all.
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

