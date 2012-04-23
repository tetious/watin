#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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