using System;
using WatiN.Core.Logging;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.Native.InternetExplorer
{
    public class IESWindowHelper
    {
        private readonly IntPtr _mainIeHwnd;
        private int _ieVersion;

        public IESWindowHelper(IntPtr mainIeHwnd)
        {
            _mainIeHwnd = mainIeHwnd;
            _ieVersion = IE.GetMajorIEVersion();
        }

        public bool IsChildWindow(Window window)
        {
            var mainWindow = new Window(_mainIeHwnd);
            return _ieVersion < 8 ?
                                      DialogBelongsToIEWindowForIe7AndLower(window, mainWindow) :
                                                                                                    DialogBelongsToIEWindowForIe8AndHigher(window, mainWindow);
        }

        private static bool DialogBelongsToIEWindowForIe7AndLower(Window dialog, Window mainWindow)
        {
            return dialog.ToplevelWindow.Equals(mainWindow);
        }

        private static bool DialogBelongsToIEWindowForIe8AndHigher(Window dialog, Window mainWindow)
        {
            Logger.LogDebug("Main: " + mainWindow.Hwnd + ", " + mainWindow.Title + ", " + mainWindow.ProcessID);

            var hWnd = IEUtils.GetInteretExplorerServerHwnd(mainWindow.Hwnd);
            var window1 = new Window(hWnd);
            Logger.LogDebug("IES: " + window1.Hwnd + ", " + window1.Title + ", " + window1.ProcessID);

            return window1.ProcessID == dialog.ProcessID;
        }
    }
}