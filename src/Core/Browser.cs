#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.Native;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native.Mozilla;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
    public abstract class Browser : DomContainer 
    {

        private static readonly Dictionary<Type, IAttachTo> AttachToHelpers = new Dictionary<Type, IAttachTo>();

        static Browser()
        {
            RegisterAttachToHelper(typeof (IE), new AttachToIeHelper());
            RegisterAttachToHelper(typeof (FireFox), new AttachToFireFoxHelper());
        }

        public abstract INativeBrowser NativeBrowser { get; }

        /// <summary>
        /// Brings the referenced Internet Explorer to the front (makes it the top window)
        /// </summary>
        public virtual void BringToFront()
        {
            if (NativeMethods.GetForegroundWindow() == hWnd) return;
            
            var result = NativeMethods.SetForegroundWindow(hWnd);

            if (!result)
            {
                Logger.LogAction("Failed to set Firefox as the foreground window.");
            }
        }

        /// <summary>
        /// Gets the window style.
        /// </summary>
        /// <returns>The style currently applied to the ie window.</returns>
        public virtual NativeMethods.WindowShowStyle GetWindowStyle()
        {
            var placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);

            NativeMethods.GetWindowPlacement(hWnd, ref placement);

            return (NativeMethods.WindowShowStyle)placement.showCmd;
        }

        /// <summary>
        /// Make the referenced Internet Explorer full screen, minimized, maximized and more.
        /// </summary>
        /// <param name="showStyle">The style to apply.</param>
        public virtual void ShowWindow(NativeMethods.WindowShowStyle showStyle)
        {
            NativeMethods.ShowWindow(hWnd, (int) showStyle);
        }

        /// <summary>
        /// Sends a Tab key to the IE window to simulate tabbing through
        /// the elements (and adres bar).
        /// </summary>
        public virtual void PressTab()
        {
            if (Debugger.IsAttached) return;

            var currentStyle = GetWindowStyle();

            ShowWindow(NativeMethods.WindowShowStyle.Restore);
            BringToFront();

            var intThreadIDIE = ProcessID;
            var intCurrentThreadID = NativeMethods.GetCurrentThreadId();

            NativeMethods.AttachThreadInput(intCurrentThreadID, intThreadIDIE, true);

            NativeMethods.keybd_event(NativeMethods.KEYEVENTF_TAB, 0x45, NativeMethods.KEYEVENTF_EXTENDEDKEY, 0);
            NativeMethods.keybd_event(NativeMethods.KEYEVENTF_TAB, 0x45, NativeMethods.KEYEVENTF_EXTENDEDKEY | NativeMethods.KEYEVENTF_KEYUP, 0);

            NativeMethods.AttachThreadInput(intCurrentThreadID, intThreadIDIE, false);

            ShowWindow(currentStyle);
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
        public virtual void GoTo(Uri url)
        {
            Logger.LogAction("Navigating to '{0}'", url.AbsoluteUri);

            NativeBrowser.NavigateTo(url);
            WaitForComplete();
        }

        /// <summary>
        /// Navigates the browser to the given <paramref name="url" /> 
        /// without waiting for the page load to be finished.
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
        ///        ie.GoToNoWait("http://watin.sourceforge.net");
        ///      }
        ///    }
        ///  }
        /// </code>
        /// </example>
        public virtual void GoToNoWait(string url)
        {
            GoToNoWait(UtilityClass.CreateUri(url));
        }

        /// <summary>
        /// Navigates the browser to the given <paramref name="url" /> 
        /// without waiting for the page load to be finished.
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
        ///        Uri URL = new Uri("http://watin.sourceforge.net");
        ///        IE ie = new IE();
        ///        ie.GoToNoWait(URL);
        ///      }
        ///    }
        ///  }
        /// </code>
        /// </example>
        public virtual void GoToNoWait(Uri url)
        {
            NativeBrowser.NavigateToNoWait(url);
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
        public virtual void GoTo(string url)
        {
            GoTo(UtilityClass.CreateUri(url));
        }


        /// <summary>
        /// Navigates the browser back to the previously displayed Url (like the back
        /// button in Internet Explorer).
        /// </summary>
        /// <returns><c>true</c> if navigating back to a previous url was possible, otherwise <c>false</c></returns>
        public virtual bool Back()
        {
            var succeeded = NativeBrowser.GoBack();
            
            if (succeeded)
            {
                WaitForComplete();
                Logger.LogAction("Navigated Back to '{0}'",Url);
            }
            else
            {
                Logger.LogAction("No history available, didn't navigate Back.");
            }

            return succeeded;
        }

        /// <summary>
        /// Navigates the browser forward to the next displayed Url (like the forward
        /// button in Internet Explorer). 
        /// </summary>
        /// <returns><c>true</c> if navigating forward to a previous url was possible, otherwise <c>false</c></returns>
        public virtual bool Forward()
        {
            var succeeded = NativeBrowser.GoForward();

            if (succeeded)
            {
                WaitForComplete();
                Logger.LogAction("Navigated Forward to '{0}",Url);
            }
            else
            {
                Logger.LogAction("No forward history available, didn't navigate Forward.");
            }

            return succeeded;
        }

        /// <summary>
        /// Closes and then reopens the browser with a blank page.
        /// </summary>
        /// <example>
        /// The following example creates a new Internet Explorer instances and navigates to
        /// the WatiN Project website on SourceForge and then reopens the browser.
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
        ///        ie.Reopen();
        ///      }
        ///    }
        ///  }
        /// </code>
        /// </example>
        public virtual void Reopen()
        {
            Logger.LogAction("Reopening browser (closing current and creating new instance)");
            NativeBrowser.Reopen();
            WaitForComplete();
        }

        /// <summary>
        /// Reloads the currently displayed webpage (like the Refresh/reload button in 
        /// a browser).
        /// </summary>
        public virtual void Refresh()
        {
            Logger.LogAction("Refreshing browser from '{0}", Url);
            NativeBrowser.Refresh();
            WaitForComplete();
        }

        /// <summary>
        /// Closes the browser.
        /// </summary>
        public abstract void Close();

        public override IntPtr hWnd
        {
            get { return NativeBrowser.hWnd; }
        }

        /// <inheritdoc />
        public override INativeDocument OnGetNativeDocument()
        {
            return NativeBrowser.NativeDocument;
        }

        /// <inheritdoc />
        protected override string GetAttributeValueImpl(string attributeName)
        {
            var name = attributeName.ToLowerInvariant();
            string value = null;

            if (name.Equals("href"))
            {
                UtilityClass.TryActionIgnoreException(() => value = Url);
            }
            else if (name.Equals("title"))
            {
                UtilityClass.TryActionIgnoreException(() => value = Title);
            }
            else if (name.Equals("hwnd"))
            {
                UtilityClass.TryActionIgnoreException(() => value = hWnd.ToString());
            }
            else
            {
                throw new InvalidAttributeException(attributeName, "IE");
            }

            return value;
        }

        /// <summary>
        /// Attach to an existing browser instance. 
        /// The first instance that matches the given <paramref name="constraint"/> will be returned.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <returns>An <see cref="Browser"/> instance of the specified type T.</returns>
        /// <exception cref="WatiN.Core.Exceptions.BrowserNotFoundException" >
        /// BrowserNotFoundException will be thrown if a browser window with the given <paramref name="constraint"/> isn't found within 30 seconds.
        /// To change this default, set <see cref="P:WatiN.Core.Settings.AttachToBrowserTimeOut"/>
        /// </exception>
        /// <example>
        /// The following example searches for an Internet Exlorer instance with "Example"
        /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
        /// When found, ieExample will hold a pointer to this Internet Explorer instance and
        /// can be used to test the Example page.
        /// <code>
        /// IE ieExample = IE.AttachTo&lt;IE&gt;(Find.ByTitle("Example"));
        /// </code>
        /// A partial match should also work
        /// <code>
        /// IE ieExample = IE.AttachTo&lt;IE&gt;(Find.ByTitle("Exa"));
        /// </code>
        /// </example>
        public static T AttachTo<T>(Constraint constraint) where T : Browser
        {
            return (T)AttachTo(typeof(T), constraint);
        }

        /// <summary>
        /// Attach to an existing browser instance. 
        /// The first instance that matches the given <paramref name="constraint"/> will be returned.
        /// </summary>
        /// <param name="browserType">The WatiN browser type to attach to.</param>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <returns>An <see cref="Browser"/> instance of the specified type T.</returns>
        /// <exception cref="WatiN.Core.Exceptions.BrowserNotFoundException" >
        /// BrowserNotFoundException will be thrown if a browser window with the given <paramref name="constraint"/> isn't found within 30 seconds.
        /// To change this default, set <see cref="P:WatiN.Core.Settings.AttachToBrowserTimeOut"/>
        /// </exception>
        /// <example>
        /// The following example searches for an Internet Exlorer instance with "Example"
        /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
        /// When found, browser will hold a pointer to this Internet Explorer instance and
        /// can be used to test the Example page.
        /// <code>
        /// var browser = Browser.AttachTo(typeof(IE), Find.ByTitle("Example"));
        /// </code>
        /// A partial match should also work
        /// <code>
        /// var browser = Browser.AttachTo(typeof(IE), Find.ByTitle("Exa"));
        /// </code>
        /// </example>
        public static Browser AttachTo(Type browserType, Constraint constraint)
        {
            return AttachTo(browserType, constraint, Settings.AttachToBrowserTimeOut);
        }

        /// <summary>
        /// Attach to an existing browser instance. 
        /// The first instance that matches the given <paramref name="constraint"/> will be returned.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <param name="timeout">The number of seconds to wait before timing out</param>
        /// <returns>A <see cref="Browser"/> instance of the specified type.</returns>
        /// <exception cref="BrowserNotFoundException" >
        /// BrowserNotFoundException will be thrown if a browser window with the given 
        /// <paramref name="constraint"/> isn't found within <paramref name="timeout"/> seconds.
        /// </exception>
        /// <example>
        /// The following example searches for an Internet Exlorer instance with "Example"
        /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
        /// It will try to find an Internet Exlorer instance for 60 seconds maxs.
        /// When found, ieExample will hold a pointer to this Internet Explorer instance and
        /// can be used to test the Example page.
        /// <code>
        /// IE ieExample = IE.AttachTo&lt;IE&gt;(Find.ByTitle("Example"), 60);
        /// </code>
        /// A partial match should also work
        /// <code>
        /// IE ieExample = IE.AttachTo&lt;IE&gt;(Find.ByTitle("Exa"), 60);
        /// </code>
        /// </example>
        public static T AttachTo<T>(Constraint constraint, int timeout) where T : Browser
        {
            return (T)AttachTo(typeof(T), constraint, timeout);
        }

        /// <summary>
        /// Attach to an existing browser instance. 
        /// The first instance that matches the given <paramref name="constraint"/> will be returned.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <param name="timeout">The number of seconds to wait before timing out</param>
        /// <param name="browserType">The WatiN browser type to attach to.</param>
        /// <returns>A <see cref="Browser"/> instance of the specified type.</returns>
        /// <exception cref="BrowserNotFoundException" >
        /// BrowserNotFoundException will be thrown if a browser window with the given 
        /// <paramref name="constraint"/> isn't found within <paramref name="timeout"/> seconds.
        /// </exception>
        /// <example>
        /// The following example searches for an Internet Exlorer instance with "Example"
        /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
        /// It will try to find an Internet Exlorer instance for 60 seconds maxs.
        /// When found, browser will hold a pointer to this Internet Explorer instance and
        /// can be used to test the Example page.
        /// <code>
        /// var browser = Browser.AttachTo(typeof(IE), Find.ByTitle("Example"), 60);
        /// </code>
        /// A partial match should also work
        /// <code>
        /// var browser = Browser.AttachTo(typeof(IE), Find.ByTitle("Exa"), 60);
        /// </code>
        /// </example>
        public static Browser AttachTo(Type browserType, Constraint constraint, int timeout)
        {
            var helper = GetAttachToHelper(browserType);
            return helper.Find(constraint, timeout, true);
        }

        /// <summary>
        /// Attach to an existing browser instance. 
        /// The first instance that matches the given <paramref name="constraint"/> will be returned.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <returns>An <see cref="Browser"/> instance of the given type.</returns>
        /// <exception cref="BrowserNotFoundException" >
        /// BrowserNotFoundException will be thrown if a browser window with the given <paramref name="constraint"/> isn't found within 30 seconds.
        /// To change this default, set <see cref="P:WatiN.Core.Settings.AttachToBrowserTimeOut"/>
        /// </exception>
        /// <example>
        /// The following example searches for an Internet Exlorer instance with "Example"
        /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
        /// When found, ieExample will hold a pointer to this Internet Explorer instance and
        /// can be used to test the Example page.
        /// <code>
        /// IE ieExample = IE.AttachTo&lt;IE&gt;(Find.ByTitle("Example"));
        /// </code>
        /// A partial match should also work
        /// <code>
        /// IE ieExample = IE.AttachTo&lt;IE&gt;(Find.ByTitle("Exa"));
        /// </code>
        /// </example>
        public static T AttachToNoWait<T>(Constraint constraint) where T : Browser
        {
            return (T)AttachToNoWait(typeof(T), constraint);
        }

        /// <summary>
        /// Attach to an existing browser instance. 
        /// The first instance that matches the given <paramref name="constraint"/> will be returned.
        /// </summary>
        /// <param name="browserType">The WatiN browser type to attach to.</param>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <returns>An <see cref="Browser"/> instance of the given type.</returns>
        /// <exception cref="BrowserNotFoundException" >
        /// BrowserNotFoundException will be thrown if a browser window with the given <paramref name="constraint"/> isn't found within 30 seconds.
        /// To change this default, set <see cref="P:WatiN.Core.Settings.AttachToBrowserTimeOut"/>
        /// </exception>
        /// <example>
        /// The following example searches for an Internet Exlorer instance with "Example"
        /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
        /// When found, browser will hold a pointer to this Internet Explorer instance and
        /// can be used to test the Example page.
        /// <code>
        /// var browser = Browser.AttachTo(typeof(IE), Find.ByTitle("Example"));
        /// </code>
        /// A partial match should also work
        /// <code>
        /// var browser = Browser.AttachTo(typeof(IE), Find.ByTitle("Exa"));
        /// </code>
        /// </example>
        public static Browser AttachToNoWait(Type browserType, Constraint constraint)
        {
            return AttachToNoWait(browserType, constraint, Settings.AttachToBrowserTimeOut);
        }

        /// <summary>
        /// Attach to an existing browser but don't wait until the whole page is loaded. 
        /// The first instance that matches the given <paramref name="constraint"/> will be returned.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <param name="timeout">The number of seconds to wait before timing out</param>
        /// <returns>An <see cref="Browser"/> instance of the specified type.</returns>
        /// <exception cref="BrowserNotFoundException" >
        /// BrowserNotFoundException will be thrown if a browser window with the given 
        /// <paramref name="constraint"/> isn't found within <paramref name="timeout"/> seconds.
        /// </exception>
        /// <example>
        /// The following example searches for an Internet Exlorer instance with "Example"
        /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
        /// It will try to find an Internet Exlorer instance for 60 seconds maxs.
        /// When found, ieExample will hold a pointer to this Internet Explorer instance and
        /// can be used to test the Example page.
        /// <code>
        /// IE ieExample = IE.AttachToNoWait&lt;IE&gt;(Find.ByTitle("Example"), 60);
        /// </code>
        /// A partial match should also work
        /// <code>
        /// IE ieExample = IE.AttachToNoWait&lt;IE&gt;(Find.ByTitle("Exa"), 60);
        /// </code>
        /// </example>
        public static T AttachToNoWait<T>(Constraint constraint, int timeout) where T : Browser
        {
            return (T)AttachToNoWait(typeof(T), constraint, timeout);
        }

        /// <summary>
        /// Attach to an existing browser but don't wait until the whole page is loaded. 
        /// The first instance that matches the given <paramref name="constraint"/> will be returned.
        /// </summary>
        /// <param name="browserType">The WatiN browser type to attach to.</param>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <param name="timeout">The number of seconds to wait before timing out</param>
        /// <returns>An <see cref="Browser"/> instance of the specified type.</returns>
        /// <exception cref="BrowserNotFoundException" >
        /// BrowserNotFoundException will be thrown if a browser window with the given 
        /// <paramref name="constraint"/> isn't found within <paramref name="timeout"/> seconds.
        /// </exception>
        /// <example>
        /// The following example searches for an Internet Exlorer instance with "Example"
        /// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
        /// It will try to find an Internet Exlorer instance for 60 seconds maxs.
        /// When found, ieExample will hold a pointer to this Internet Explorer instance and
        /// can be used to test the Example page.
        /// <code>
        /// var browser = Browser.AttachToNoWait(typeof(IE), Find.ByTitle("Example"), 60);
        /// </code>
        /// A partial match should also work
        /// <code>
        /// var browser = Browser.AttachToNoWait(typeof(IE), Find.ByTitle("Exa"), 60);
        /// </code>
        /// </example>
        public static Browser AttachToNoWait(Type browserType, Constraint constraint, int timeout)
        {
            var helper = GetAttachToHelper(browserType);
            return helper.Find(constraint, timeout, false);
        }

        /// <summary>
        /// Does the specified browser type exist.
        /// </summary>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <returns><c>true</c> if a browser instance matches the given <paramref name="findBy"/> <see cref="Constraint"/>. Otherwise it returns <c>false</c>. </returns>
        public static bool Exists<T>(Constraint constraint) where T : Browser
        {
            return Exists(typeof(T), constraint);
        }

        /// <summary>
        /// Does the specified browser type exist.
        /// </summary>
        /// <param name="browserType">The WatiN browser type to attach to.</param>
        /// <param name="constraint">The <see cref="Constraint"/> of the browser window to find. 
        /// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
        /// <returns><c>true</c> if a browser instance matches the given <paramref name="findBy"/> <see cref="Constraint"/>. Otherwise it returns <c>false</c>. </returns>
        public static bool Exists(Type browserType, Constraint constraint)
        {
            var helper = GetAttachToHelper(browserType);
            return helper.Exists(constraint);
        }

        private static IAttachTo GetAttachToHelper(Type browserType)
        {
            if (!AttachToHelpers.ContainsKey(browserType)) throw new WatiNException("No AttachToHelper registered for type " + browserType);
            return AttachToHelpers[browserType];
        }

        /// <summary>
        /// Register a specific helper to attacht to .
        /// </summary>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="attachToHelper">The attach to helper.</param>
        public static void RegisterAttachToHelper(Type browserType, IAttachTo attachToHelper)
        {
            if (!browserType.IsSubclassOf(typeof(Browser))) throw new ArgumentException("Not a subclass of Browser", "browserType"); 
            
            AttachToHelpers.Add(browserType, attachToHelper);
        }
    }
}