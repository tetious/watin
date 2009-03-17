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
using System.Diagnostics;
using System.Runtime.InteropServices;
using WatiN.Core.Logging;
using WatiN.Core.Native;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
    public abstract class Browser : DomContainer
    {

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
            Logger.LogAction("Navigating to '" + url.AbsoluteUri + "'");

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
                Logger.LogAction("Navigated Back to '" + Url + "'");
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
                Logger.LogAction("Navigated Forward to '" + Url + "'");
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
            Logger.LogAction("Refreshing browser from '" + Url + "'");
            NativeBrowser.Refresh();
            WaitForComplete();
        }
    }
}