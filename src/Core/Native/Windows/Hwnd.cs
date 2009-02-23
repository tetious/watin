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
using System.Runtime.InteropServices;
using System.Threading;

namespace WatiN.Core.Native.Windows
{
    public class Hwnd : IHwnd
    {
        private readonly IntPtr _hwnd = IntPtr.Zero;

        public Hwnd(IntPtr Hwnd)
        {
            _hwnd = Hwnd;
        }

        public IntPtr hwnd
        {
            get { return _hwnd; }
        }

        public string WindowText
        {
            get { return NativeMethods.GetWindowText(_hwnd); }
        }

        public bool IsWindowEnabled
        {
            get { return NativeMethods.IsWindowEnabled(_hwnd); }
        }

        public bool IsWindow
        {
            get { return NativeMethods.IsWindow(_hwnd); }
        }

        public bool IsWindowVisible
        {
            get { return NativeMethods.IsWindowVisible(_hwnd); }
        }

        #region IHwnd Members

        public string ClassName
        {
            get { return NativeMethods.GetClassName(_hwnd); }
        }

        #endregion

        public void SendMessage(int msg, int wParam, int lParam)
        {
            NativeMethods.SendMessage(_hwnd, msg, wParam, lParam);
        }

        public IntPtr GetDlgItem(int controlid)
        {
            return NativeMethods.GetDlgItem(_hwnd, controlid);
        }

        public void SendString(string s)
        {
            foreach (var c in s)
            {
                Thread.Sleep(50);
                SendChar(c, _hwnd);
            }
        }

        public void SetFocus()
        {
            NativeMethods.SetFocus(_hwnd);
        }

        private static void SendChar(char c, IntPtr handle)
        {
            NativeMethods.SendMessage(new HandleRef(null, handle), NativeMethods.WM_CHAR, new IntPtr(Convert.ToInt64(c)), IntPtr.Zero);
        }
    }
}