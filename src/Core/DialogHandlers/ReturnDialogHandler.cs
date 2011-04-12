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
using System.Threading;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.DialogHandlers
{
	public class ReturnDialogHandler : ConfirmDialogHandler, IReturnDialogHandler
	{
        /// <summary>
        /// Determines whether this instance can handle the specified window by checking <see cref="Window.StyleInHex"/>.
        /// equals "94C803C5" and if the window has a button with Id 1.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can handle dialog] the specified window; otherwise, <c>false</c>.
        /// </returns>
		public override bool CanHandleDialog(Window window)
		{
			return (window.StyleInHex == "94C803C5" && ButtonWithId1Exists(window.Hwnd));
		}

	    public static IReturnDialogHandler CreateInstance()
	    {
	        var ieVersion = IE.GetMajorIEVersion();

	        return ieVersion < 9 ? (IReturnDialogHandler) new ReturnDialogHandler() : new ReturnDialogHandlerIe9();
	    }
	}

    public interface IReturnDialogHandler : IDialogHandler
    {
        WinButton CancelButton { get; }
        WinButton OKButton { get; }
        void WaitUntilExists(int waitDurationInSeconds);
        bool Exists();
        void WaitUntilExists();
    }

    public class ReturnDialogHandlerIe9 : BaseDialogHandler, IReturnDialogHandler
    {
        private Window _window;

        public WinButton CancelButton
        {
            get
            {
                var hwnd = GetChildWindowHwnd(_window.Hwnd, "5000200E");
                return hwnd != IntPtr.Zero ? new WinButton(hwnd) : null;
            }
        }

        public WinButton OKButton
        {
            get
            {
                var hwnd = GetChildWindowHwnd(_window.Hwnd, "5000200F");
                return hwnd != IntPtr.Zero ? new WinButton(hwnd) : null;
            }
        }

        public override bool HandleDialog(Window window)
        {
            if (CanHandleDialog(window))
            {
                _window = window;

                while (window.Exists())
                {
                    Thread.Sleep(200);
                }
                return true;
            }
            return false;
        }

        public override bool CanHandleDialog(Window window)
        {
            return (window.StyleInHex == "96C00284");
        }

        public void WaitUntilExists(int waitDurationInSeconds)
        {
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(waitDurationInSeconds));
            tryActionUntilTimeOut.Try<bool>(Exists);

            if (!Exists())
            {
                throw new WatiNException(string.Format("Dialog not available within {0} seconds.", waitDurationInSeconds));
            }
        }

        public bool Exists()
        {
            return _window != null && _window.Exists();
        }

        private IntPtr GetChildWindowHwnd(IntPtr parentHwnd, string styleInHex)
        {
            var hWnd = IntPtr.Zero;
            NativeMethods.EnumChildWindows(parentHwnd, (childHwnd, lParam) =>
            {
                var window = new Window(childHwnd);
//                Console.WriteLine("childhwnd: " + childHwnd);
//                Console.WriteLine("childhwnd.styleinhex: " + window.StyleInHex);
                if (window.StyleInHex == styleInHex)
                {
                    hWnd = childHwnd;
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            return hWnd;
        }

        public void WaitUntilExists()
        {
            WaitUntilExists(30);
        }
    }

}