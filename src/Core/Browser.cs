using System.Diagnostics;
using System.Runtime.InteropServices;
using WatiN.Core.Logging;

namespace WatiN.Core
{
    public abstract class Browser : DomContainer
    {
        /// <summary>
        /// Brings the referenced Internet Explorer to the front (makes it the top window)
        /// </summary>
        public void BringToFront()
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
        public NativeMethods.WindowShowStyle GetWindowStyle()
        {
            var placement = new NativeMethods.WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);

            NativeMethods.GetWindowPlacement(hWnd, ref placement);

            return (NativeMethods.WindowShowStyle)placement.showCmd;
        }

        /// <summary>
        /// Make the referenced Internet Explorer full screen, minimized, maximized and more.
        /// </summary>
        /// <param name="showStyle">The style to apply.</param>
        public void ShowWindow(NativeMethods.WindowShowStyle showStyle)
        {
            NativeMethods.ShowWindow(hWnd, (int) showStyle);
        }

        /// <summary>
        /// Sends a Tab key to the IE window to simulate tabbing through
        /// the elements (and adres bar).
        /// </summary>
        public void PressTab()
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
    }
}