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
using WatiN.Core.Logging;

namespace WatiN.Core.Native.Windows
{
    public class WinButton
    {
        private readonly IHwnd _hWnd;

        public WinButton(IntPtr Hwnd) : this(new Hwnd(Hwnd))
        {}

        public WinButton(int buttonid, IntPtr parentHwnd) : this(buttonid, new Hwnd(parentHwnd))
        {}

        public WinButton(IHwnd Hwnd)
        {
            _hWnd = Hwnd;
        }

        public WinButton(int buttonid, IHwnd parentHwnd)
        {
            _hWnd = new Hwnd(parentHwnd.GetDlgItem(buttonid));
        }

        public void Click()
        {
            if (!Exists()) return;

            Logger.LogAction("Clicking on '{0}'", Title);

            _hWnd.SendMessage(NativeMethods.WM_ACTIVATE, NativeMethods.MA_ACTIVATE, 0);
            _hWnd.SendMessage(NativeMethods.BM_CLICK, 0, 0);
        }

        public bool Exists()
        {
            return _hWnd.IsWindow && _hWnd.ClassName.Equals("Button");
        }

        public string Title
        {
            get { return _hWnd.WindowText; }
        }

        public bool Enabled
        {
            get { return _hWnd.IsWindowEnabled; }
        }

        public bool Visible
        {
            get { return _hWnd.IsWindowVisible; }
        }
    }
}