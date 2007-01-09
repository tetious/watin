#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006-2007 Jeroen van Menen
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
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core
{
  /// <summary>
  /// This class is used to define the default settings used by WatiN. 
  /// Use <c>IE.Settings</c> to access or change these settings.
  /// </summary>
  /// <example>
  /// The following example shows you how to change the default time out which is used
  /// by the AttachtToIE(findBy) method to attach to an already existing Internet Explorer window 
  /// or to an Internet Explorer window that will show up within 60 seconds after calling
  /// the AttachToIE(findBy) method.
  /// <code>
  /// public void AttachToIEExample()
  /// {
  ///   // Change de default time out from 30 to 60 seconds.
  ///   IE.Settings.AttachToIETimeOut = 60;
  /// 
  ///   // Now start Internet Explorer manually and type 
  ///   // http://watin.sourceforge.net in the navigation bar.
  /// 
  ///   // Now Attach to an existing Internet Explorer window
  ///   IE ie = IE.AttachToIE(Find.ByTitle("WatiN");
  /// 
  ///   System.Diagnostics.Debug.WriteLine(ie.Url);
  /// }
  /// </code>
  /// When you frequently want to change these settings you could also create
  /// two or more instances of the Settings class, set the desired defaults 
  /// and set the settings class to IE.Settings.
  /// <code>
  /// public void ChangeSettings()
  /// {
  ///   IE.Settings = LongTimeOut();
  ///   
  ///   // Do something here that requires more time then the defaults
  /// 
  ///   IE.Settings = ShortTimeOut();
  /// 
  ///   // Do something here if you want a short time out to get
  ///   // the exception quickly incase the item isn't found.  
  /// }
  /// 
  /// public Settings LongTimeOut()
  /// {
  ///   Settings settings = new Settings();
  /// 
  ///   settings.AttachToIETimeOut = 60;
  ///   settings.WaitUntilExistsTimeOut = 60;
  ///   settings.WaitForCompleteTimeOut = 60;
  /// 
  ///   return settings;
  /// }
  /// 
  /// public Settings ShortTimeOut()
  /// {
  ///   Settings settings = new Settings();
  /// 
  ///   settings.AttachToIETimeOut = 5;
  ///   settings.WaitUntilExistsTimeOut = 5;
  ///   settings.WaitForCompleteTimeOut = 5;
  /// 
  ///   return settings;
  /// }
  /// </code>
  /// </example>
  public class Settings
  {
    private struct settingsStruct
    {
      public int attachToIETimeOut;
      public int waitUntilExistsTimeOut;
      public int waitForCompleteTimeOut;
      public bool highLightElement;
      public string highLightColor;
      public bool autoCloseDialogs;
    }
    
    private settingsStruct settings;

    public Settings()
    {
      SetDefaults();
    }

    private Settings(settingsStruct settings)
    {
      this.settings = settings;
    }

    /// <summary>
    /// Resets this instance to the initial defaults.
    /// </summary>
    public void Reset()
    {
      SetDefaults();      
    }
    
    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns></returns>
    public Settings Clone()
    {
      return new Settings(settings);
    }
    
    private void SetDefaults()
    {
      settings = new settingsStruct();
      settings.attachToIETimeOut = 30;
      settings.waitUntilExistsTimeOut = 30;
      settings.waitForCompleteTimeOut = 30;
      settings.highLightElement = true;
      settings.highLightColor = "yellow";
      settings.autoCloseDialogs = true;
    }

    /// <summary>
    /// Get or set the default time out used when calling IE ie = IE.AttachToIE(findBy).
    /// The initial value is 30 seconds. Setting the time out to a negative value will
    /// throw a <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    public int AttachToIETimeOut
    {
      get { return settings.attachToIETimeOut; }
      set
      {
        IfValueLessThenZeroThrowArgumentOutOfRangeException(value);
        settings.attachToIETimeOut = value;
      }
    }

    /// <summary>
    /// Get or set the default time out used when calling Element.WaitUntilExists().
    /// The initial value is 30 seconds. Setting the time out to a negative value will
    /// throw a <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    public int WaitUntilExistsTimeOut
    {
      get { return settings.waitUntilExistsTimeOut; }
      set
      {
        IfValueLessThenZeroThrowArgumentOutOfRangeException(value);        
        settings.waitUntilExistsTimeOut = value;
      }
    }

    /// <summary>
    /// Get or set the default time out used when calling ie.WaitForComplete().
    /// The initial value is 30 seconds. Setting the time out to a negative value will
    /// throw a <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    public int WaitForCompleteTimeOut
    {
      get { return settings.waitForCompleteTimeOut; }
      set
      {
        IfValueLessThenZeroThrowArgumentOutOfRangeException(value);
        settings.waitForCompleteTimeOut = value;
      }
    }

    /// <summary>
    /// Turn highlighting of elements by WatiN on (<c>true</c>) or off (<c>false</c>).
    /// Highlighting of an element is done when WatiN fires an event on an
    /// element or executes a methode (like TypeText).
    /// </summary>
    public bool HighLightElement
    {
      get { return settings.highLightElement; }
      set { settings.highLightElement = value; }
    }

    /// <summary>
    /// Set or get the color to highlight elements. Will be used if
    /// HighLightElement is set to <c>true</c>.
    /// Visit http://msdn.microsoft.com/workshop/author/dhtml/reference/colors/colors_name.asp
    /// for a full list of supported RGB colors and their names.
    /// </summary>
    public string HighLightColor
    {
      get { return settings.highLightColor; }
      set { settings.highLightColor = value; }
    }

    /// <summary>
    /// Turn auto closing of dialogs on (<c>true</c>) or off (<c>false</c>).
    /// You need to set this value before creating or attaching to any 
    /// Internet Explorer to have effect.
    /// </summary>
    public bool AutoCloseDialogs
    {
      get { return settings.autoCloseDialogs; }
      set { settings.autoCloseDialogs = value; }
    }
    
    private static void IfValueLessThenZeroThrowArgumentOutOfRangeException(int value)
    {
      if (value < 0 )
      {
        throw new ArgumentOutOfRangeException("value", "time out should be 0 seconds or more.");
      }
    }
  }
  
  /// <summary>
  /// This is the main class to access a webpage in Internet Explorer to 
  /// get to all the elements and (i)frames on the page.
  /// 
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
  public class IE : DomContainer
  {
    private InternetExplorer ie;

    private bool autoClose = true;
    private bool isDisposed = false;
    private static Settings settings = new Settings();

    public static Settings Settings
    {
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }
        
        settings = value;
      }
      get { return settings; }
    }
    
    /// <summary>
    /// Attach to an existing Internet Explorer. 
    /// The first instance that matches the given <paramref name="findBy"/> will be returned.
    /// The attached Internet Explorer will be closed after destroying the IE instance.
    /// </summary>
    /// <param name="findBy">The <see cref="Attribute"/> of the IE window to find 
    /// (<see cref="Url"/> and <see cref="Title"/> are supported)</param>
    /// <returns>An <see cref="IE"/> instance.</returns>
    /// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
    /// IENotFoundException will be thrown if an Internet Explorer window with the given <paramref name="findBy"/> isn't found within 30 seconds.
    /// To change this default, set <see cref="IE.Settings.AttachToIETimeOut"/>
    /// </exception>
    /// <example>
    /// The following example searches for an Internet Exlorer instance with "Example"
    /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
    /// When found, ieExample will hold a pointer to this Internet Explorer instance and
    /// can be used to test the Example page.
    /// <code>
    /// IE ieExample = IE.AttachToIE(Find.ByTitle("Example"));
    /// </code>
    /// A partial match should also work
    /// <code>
    /// IE ieExample = IE.AttachToIE(Find.ByTitle("Exa"));
    /// </code>
    /// </example>

    public static IE AttachToIE(Attribute findBy)
    {
      return findIE(findBy, Settings.AttachToIETimeOut);
    }

    /// <summary>
    /// Attach to an existing Internet Explorer. 
    /// The first instance that matches the given <paramref name="findBy"/> will be returned.
    /// The attached Internet Explorer will be closed after destroying the IE instance.
    /// </summary>
    /// <param name="findBy">The <see cref="Attribute"/> of the IE window to find 
    /// (<see cref="Url"/> and <see cref="Title"/> are supported)</param>
    /// <param name="timeout">The number of seconds to wait before timing out</param>
    /// <returns>An <see cref="IE"/> instance.</returns>
    /// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
    /// IENotFoundException will be thrown if an Internet Explorer window with the given 
    /// <paramref name="findBy"/> isn't found within <paramref name="timeout"/> seconds.
    /// </exception>
    /// <example>
    /// The following example searches for an Internet Exlorer instance with "Example"
    /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
    /// It will try to find an Internet Exlorer instance for 60 seconds maxs.
    /// When found, ieExample will hold a pointer to this Internet Explorer instance and
    /// can be used to test the Example page.
    /// <code>
    /// IE ieExample = IE.AttachToIE(Find.ByTitle("Example"), 60);
    /// </code>
    /// A partial match should also work
    /// <code>
    /// IE ieExample = IE.AttachToIE(Find.ByTitle("Exa"), 60);
    /// </code>
    /// </example>
    public static IE AttachToIE(Attribute findBy, int timeout)
    {
      return findIE(findBy, timeout);
    }

    /// <summary>
    /// Does the specified Internet Explorer exist.
    /// </summary>
    /// <param name="findBy">The <see cref="Attribute"/> of the IE window to find (<see cref="Url"/> and <see cref="Title"/> are supported)</param>
    /// <returns><c>true</c> if an Internet Explorer instance matches the given <paramref name="findBy"/> <see cref="Attribute"/>. Otherwise it returns <c>false</c>. </returns>
    public static bool Exists(Attribute findBy)
    {
      return (null != findInternetExplorer(findBy));
    }
    
    /// <summary>
    /// Creates a collection of new IE instances associated with open Internet Explorer windows.
    /// </summary>
    /// <example>
    /// This code snippet illustrates the use of this method to found out the number of open
    /// Internet Explorer windows.
    /// <code>int IECount = IE.InternetExplorers.Length;</code>
    /// </example>
    public static IECollection InternetExplorers()
    {
      return new IECollection();
    }

    /// <summary>
    /// Opens a new Internet Explorer with a blank page. 
    /// <note>
    /// When the <see cref="WatiN.Core.IE" />
    /// instance is destroyed the created Internet Explorer window will also be closed.
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
      CreateNewIEAndGoToUri(new Uri("about:blank"), null);
    }

    /// <summary>
    /// Opens a new Internet Explorer and navigates to the given <paramref name="url"/>.
    /// <note>
    /// When the <see cref="WatiN.Core.IE" />
    /// instance is destroyed the created Internet Explorer window will also be closed.
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
    ///      public OpenWatiNWebsite()
    ///      {
    ///        IE ie = new IE("http://watin.sourceforge.net");
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE(string url)
    {
      CreateNewIEAndGoToUri(new Uri(url), null);
    }
    /// <summary>
    /// Opens a new Internet Explorer and navigates to the given <paramref name="uri"/>.
    /// <note>
    /// When the <see cref="WatiN.Core.IE" />
    /// instance is destroyed the created Internet Explorer window will also be closed.
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
    ///      public OpenWatiNWebsite()
    ///      {
    ///        IE ie = new IE(new Uri("http://watin.sourceforge.net"));
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE(Uri uri)
    {
      CreateNewIEAndGoToUri(uri, null);
    }
    
    /// <summary>
    /// Opens a new Internet Explorer and navigates to the given <paramref name="url"/>.
    /// </summary>
    /// <param name="url">The Url te open</param>
    /// <param name="logonDialogHandler">A <see cref="LogonDialogHandler"/> class instanciated with the logon credentials.</param>
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
    ///        LogonDialogHandler logon = new LogonDialogHandler("username", "password");
    ///        IE ie = new IE("http://watin.sourceforge.net", logon);
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE(string url, LogonDialogHandler logonDialogHandler)
    {
      CreateNewIEAndGoToUri(new Uri(url), logonDialogHandler);
    }
    
    /// <summary>
    /// Opens a new Internet Explorer and navigates to the given <paramref name="uri"/>.
    /// </summary>
    /// <param name="uri">The Uri te open</param>
    /// <param name="logonDialogHandler">A <see cref="LogonDialogHandler"/> class instanciated with the logon credentials.</param>
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
    ///        LogonDialogHandler logon = new LogonDialogHandler("username", "password");
    ///        IE ie = new IE(new Uri("http://watin.sourceforge.net"), logon);
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public IE(Uri uri, LogonDialogHandler logonDialogHandler)
    {
      CreateNewIEAndGoToUri(uri, logonDialogHandler);
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

      InternetExplorer internetExplorer = shDocVwInternetExplorer as InternetExplorer;
      
      if(shDocVwInternetExplorer == null)
      {
        throw new ArgumentException("SHDocVwInternetExplorer needs to be of type Interop.SHDocVw.InternetExplorer");
      }

      InitIEAndStartDialogWatcher(internetExplorer);
    }

    private void CreateNewIEAndGoToUri(Uri uri, LogonDialogHandler logonDialogHandler)
    {
      CheckThreadApartmentStateIsSTA();

      Logger.LogAction("Creating new IE instance");

      MoveMouseToTopLeft();

      InitIEAndStartDialogWatcher(new InternetExplorerClass(), uri);

      if (logonDialogHandler != null)
      {
        // remove other logon dialog handlers since only one handler
        // can effectively handle the logon dialog.
        DialogWatcher.RemoveAll(new LogonDialogHandler("a", "b"));

        // Add the (new) logonHandler
        DialogWatcher.Add(logonDialogHandler);
      }
      
      WaitForComplete();
    }

    private static void CheckThreadApartmentStateIsSTA()
    {
      
#if NET11      
      // Code for .Net 1.1
      bool isSTA = (Thread.CurrentThread.ApartmentState == ApartmentState.STA);
#elif NET20
      // Code for .Net 2.0
      bool isSTA = (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
#endif
      if (!isSTA)
      {
        throw new ThreadStateException("The CurrentThread needs to have it's ApartmentState set to ApartmentState.STA to be able to automate Internet Explorer.");
      }
    }

    private void InitIEAndStartDialogWatcher(InternetExplorer internetExplorer)
    {
      InitIEAndStartDialogWatcher(internetExplorer, null);
    }

    private void InitIEAndStartDialogWatcher(InternetExplorer internetExplorer, Uri uri)
    {
      ie = internetExplorer;
      
      // Due to UAC in Vista the navigate has to be done
      // before showing the new Internet Explorer instance
      if (uri !=null)
      {
        navigateTo(uri);
      }
      ie.Visible = true;
      StartDialogWatcher();
    }

    private static IE findIE(Attribute findBy, int timeout)
    {
      Logger.LogAction("Busy finding Internet Explorer with " + findBy.AttributeName + " '" + findBy.Value + "'");

      SimpleTimer timeoutTimer = new SimpleTimer(timeout);

      do
      {
        Thread.Sleep(500);

        InternetExplorer internetExplorer = findInternetExplorer(findBy);
        
        if (internetExplorer != null)
        {
          IE ie = new IE(internetExplorer);
          ie.WaitForComplete();

          return ie;
        }
      } while (!timeoutTimer.Elapsed);


      throw new IENotFoundException(findBy.AttributeName, findBy.Value, timeout);
    }

    private static InternetExplorer findInternetExplorer(Attribute findBy)
    {
      ShellWindows allBrowsers = new ShellWindows();

      int browserCount = allBrowsers.Count;
      int browserCounter = 0;

      IEAttributeBag attributeBag = new IEAttributeBag();
      
      while (browserCounter < browserCount)
      {
        attributeBag.InternetExplorer = (InternetExplorer) allBrowsers.Item(browserCounter);

        if (findBy.Compare(attributeBag))
        {
          return attributeBag.InternetExplorer;
        }

        browserCounter++;
      }
      
      return null;
    }

    private void MoveMouseToTopLeft()
    {
      // Better move the mouse out of the way.
      System.Windows.Forms.Cursor.Position = new System.Drawing.Point(0, 0);
    }

    
    /// <summary>
    /// Navigates Internet Explorer to the given <paramref name="url" />.
    /// </summary>
    /// <param name="url">The URL specified as a wel formed Uri.</param>
    /// <example>
    /// The following example creates an Uri and Internet Explorer instance and navigates to
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
      navigateTo(url);
      WaitForComplete();
    }

    private void navigateTo(Uri url)
    {
      Logger.LogAction("Navigating to '" + url + "'");
      
      object nil = null;
      ie.Navigate(url.ToString(), ref nil, ref nil, ref nil, ref nil);
    }

    /// <summary>
    /// Navigates Internet Explorer to the given <paramref name="url" />.
    /// </summary>
    /// <param name="url">The URL to GoTo.</param>
    /// <example>
    /// The following example creates a new Internet Explorer instance and navigates to
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
    /// Sends a Tab key to the IE window to simulate tabbing through
    /// the elements (and adres bar).
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
        
        intThreadIDIE = ProcessID;
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
      if (NativeMethods.GetForegroundWindow() != hWnd)
      {
        NativeMethods.SetForegroundWindow(hWnd);
      }
    }

    /// <summary>
    /// Make the referenced Internet Explorer full screen, minimized, maximized and more.
    /// </summary>
    /// <param name="showStyle">The style to apply.</param>
    public void ShowWindow(NativeMethods.WindowShowStyle showStyle)
    {
      NativeMethods.ShowWindow(hWnd, (int)showStyle);
    }

    /// <summary>
    /// Gets the window style.
    /// </summary>
    /// <returns>The style currently applied to the ie window.</returns>
    public NativeMethods.WindowShowStyle GetWindowStyle()
    {
      NativeMethods.WINDOWPLACEMENT placement = new NativeMethods.WINDOWPLACEMENT();
      placement.length = Marshal.SizeOf(placement);
      
      NativeMethods.GetWindowPlacement(hWnd, ref placement);
      
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
      if (!isDisposed)
      {
        if (IsInternetExplorerStillAvailable())
        {
          Logger.LogAction("Closing browser '" + Title + "'");
        }
        
        DisposeAndCloseIE(true);
      }
    }

    private void DisposeAndCloseIE(bool closeIE)
    {
      if (!isDisposed)
      {
        if (closeIE && IsInternetExplorerStillAvailable())
        {

          //TODO: Since HTMLDialog collection contains all HTMLDialogs
          //      within the processId of this IE instance, there might be
          //      other HTMLDialogs not created by this IE instance. Closing
          //      also those HTMLDialogs seems not right.
          //      So how will we handle this? For now we keep the "old"
          //      implementation.
          
          // Close all open HTMLDialogs
          HtmlDialogs.CloseAll();
        }
        
        base.Dispose(true);

        if (closeIE && IsInternetExplorerStillAvailable())
        {
          // Ask IE to close
          ie.Quit(); 
        }
        
        ie = null;
        
        if (closeIE)
        {
          // Wait for IE to close to prevent RPC errors when creating
          // a new WatiN.Core.IE instance.
          Thread.Sleep(1000); 
        }
        
        isDisposed = true;
      }
    }

    /// <summary>
    /// Closes <i>all</i> running instances of Internet Explorer by killing the
    /// process these instances run in. 
    /// </summary>
    public virtual void ForceClose()
    {
      if (isDisposed)
      {
        throw new ObjectDisposedException("Internet Explorer", "The Internet Explorer instance is already disposed. ForceClose can't be performed.");
      }
      
      Logger.LogAction("Force closing all IE instances");

      int iePid = ProcessID;
      
      DisposeAndCloseIE(true);

      try
      {
        // Force Internet Explorer instances to close
        Process.GetProcessById(iePid).Kill(); 
      }
      catch (ArgumentException)
      {
        // ignore: IE is no longer running
      }
    }


    private bool IsInternetExplorerStillAvailable()
    {
      try
      {
        // Call a property of the
        // ie instance to see of it isn't disposed by 
        // another IE instance.
        int hwndDummy = ie.HWND;
      }
      catch (COMException)
      {
        return false;
      }
      
      return true;
    }

    internal override IHTMLDocument2 OnGetHtmlDocument()
    {
      return (IHTMLDocument2)ie.Document;
    }

    /// <summary>
    /// Waits till the webpage, it's frames and all it's elements are loaded. This
    /// function is called by WatiN after each action (like clicking a link) so you
    /// should have to use this function on rare occasions.
    /// To change the default time out, set <see cref="IE.Settings.WaitForCompleteTimeOut"/>
    /// </summary>
    public override void WaitForComplete()
    {
      InitTimeout();

      waitWhileIEBusy(ie);
      waitWhileIEStateNotComplete(ie);
      
      WaitForCompleteOrTimeout();
    }

    #region IDisposable Members

    /// <summary>
    /// This method must be called by its inheritor to dispose references
    /// to internal resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      DisposeAndCloseIE(AutoClose);
    }
    #endregion

    /// <summary>
    /// Gets or sets a value indicating whether to auto close IE after destroying
    /// a reference to the corresponding IE instance.
    /// </summary>
    /// <value><c>true</c> when to auto close IE (this is the default); otherwise, <c>false</c>.</value>
    public bool AutoClose
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
        Process p = Process.GetProcessById(ProcessID);
        HtmlDialogCollection htmlDialogCollection = new HtmlDialogCollection(p); 

        return htmlDialogCollection;
      }
    }

    public override IntPtr hWnd
    {
      get
      {
        return new IntPtr(ie.HWND);
      }
    }

    /// <summary>
    /// Find a HtmlDialog by an attribute. Currently 
    /// Find.ByUrl and Find.ByTitle are supported.
    /// </summary>
    /// <param name="findBy">The url of the html page shown in the dialog</param>
    public HtmlDialog HtmlDialog(Attribute findBy)
    {
      return findHtmlDialog(findBy, Settings.AttachToIETimeOut);
    }

    /// <summary>
    /// Find a HtmlDialog by an attribute within the given <paramref name="timeout" /> period.
    /// Currently Find.ByUrl and Find.ByTitle are supported.
    /// </summary>
    /// <param name="findBy">The url of the html page shown in the dialog</param>
    /// <param name="timeout">Number of seconds before the search times out.</param>
    public HtmlDialog HtmlDialog(Attribute findBy, int timeout)
    {
      return findHtmlDialog(findBy, timeout);
    }

    private HtmlDialog findHtmlDialog(Attribute findBy, int timeout)
    {
      Logger.LogAction("Busy finding HTMLDialog with " + findBy.AttributeName + " '" + findBy.Value + "'");

      SimpleTimer timeoutTimer = new SimpleTimer(timeout);

      do
      {
        Thread.Sleep(500);

        foreach (HtmlDialog htmlDialog in HtmlDialogs)
        {          
          if (findBy.Compare(htmlDialog))
          {
            return htmlDialog;
          }
        }
      } while (!timeoutTimer.Elapsed);

      throw new HtmlDialogNotFoundException(findBy.AttributeName, findBy.Value, timeout);
    }
  }

  public class IEAttributeBag: IAttributeBag
  {
    private InternetExplorer internetExplorer = null;

    public InternetExplorer InternetExplorer
    {
      get
      {
        return internetExplorer;
      }
      set
      {
        internetExplorer = value;
      }
    }

    public string GetValue(string attributename)
    {
      string value = null;

      if (attributename.ToLower().Equals("href"))
      {
        try
        {
          value = InternetExplorer.LocationURL;
        }
        catch{}
      }
      else if (attributename.ToLower().Equals("title"))
      {
        try
        {
          value = ((HTMLDocument) InternetExplorer.Document).title;
        }
        catch{}
      }
      else
      {
        throw new InvalidAttributException(attributename, "IE");
      }
      
      return value;
    }
  }
}

