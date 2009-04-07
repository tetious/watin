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
using WatiN.Core.Native.Windows;

namespace WatiN.Core.DialogHandlers
{
    /// <summary>
    /// This class handles the Refresh Warning dialog and does press
    /// the retry or cancel button when the dialog shows up.
    /// </summary>
    public class RefreshWarningDialogHandler : BaseDialogHandler
    {
        private readonly bool _pressReTry;
        private IntPtr _hwnd;

        public RefreshWarningDialogHandler(bool pressReTry)
        {
            _pressReTry = pressReTry;
        }

        /// <inheritdoc />
        public override bool HandleDialog(Window window)
        {
            if (CanHandleDialog(window))
            {
                _hwnd = window.Hwnd;

                if (_pressReTry)
                {
                    ReTry.Click();
                }
                else
                {
                    Cancel.Click();
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether this instance can handle the specified window by checking <see cref="Window.StyleInHex"/>
        /// equals "94C801C5".
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can handle dialog] the specified window; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandleDialog(Window window)
        {
            return window.StyleInHex == "94C801C5";
        }

        private WinButton ReTry
        {
            get { return new WinButton(4, _hwnd); }
        }

        private WinButton Cancel
        {
            get { return new WinButton(2, _hwnd); }
        }
    }
}
