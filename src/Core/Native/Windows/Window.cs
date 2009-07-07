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

namespace WatiN.Core.Native.Windows
{
    public class Window
    {
        private readonly IntPtr hwnd;

        public Window(IntPtr hwnd)
        {
            this.hwnd = hwnd;
        }

        public virtual IntPtr Hwnd
        {
            get { return hwnd; }
        }

        public virtual IntPtr ParentHwnd
        {
            get { return NativeMethods.GetParent(Hwnd); }
        }

        public bool HasParentWindow
        {
            get { return ParentHwnd != IntPtr.Zero; }
        }

        public virtual string Title
        {
            get { return NativeMethods.GetWindowText(Hwnd); }
        }

        public virtual string ClassName
        {
            get { return NativeMethods.GetClassName(Hwnd); }
        }

        public virtual Int64 Style
        {
            get { return NativeMethods.GetWindowStyle(Hwnd); }
        }

        public virtual string StyleInHex
        {
            get { return Style.ToString("X"); }
        }

        public virtual bool IsDialog()
        {
            return (ClassName == "#32770");
        }

        public virtual void ForceClose()
        {
            NativeMethods.SendMessage(Hwnd, NativeMethods.WM_CLOSE, 0, 0);
        }

        public virtual bool Exists()
        {
            return NativeMethods.IsWindow(Hwnd);
        }

        public virtual bool Visible
        {
            get { return NativeMethods.IsWindowVisible(Hwnd); }
        }

        public virtual void ToFront()
        {
            NativeMethods.SetForegroundWindow(Hwnd);
        }

        public virtual void SetActivate()
        {
            NativeMethods.SetActiveWindow(Hwnd);
        }

        public virtual Window ToplevelWindow
        {
            get
            {
                var toplevelWindow = this;
                do
                {
                    if (toplevelWindow.HasParentWindow)
                        toplevelWindow = new Window(toplevelWindow.ParentHwnd);
                    else
                        break;
                } while (true);
                return toplevelWindow;
            }
        }

        /// <summary>
        /// Gets the process ID in which the window is running.
        /// </summary>
        /// <value>The process ID.</value>
        public virtual int ProcessID
        {
            get
            {
                int iePid;

                NativeMethods.GetWindowThreadProcessId(Hwnd, out iePid);

                return iePid;
            }
        }

        public virtual string ProcessName
        {
            get { return Process.GetProcessById(ProcessID).ProcessName; }
        }

        public string Message
        {
            get
            {
                var messagehWnd = NativeMethods.GetDlgItem(Hwnd, 65535);
                return NativeMethods.GetWindowText(messagehWnd);
            }
        }
    }
}