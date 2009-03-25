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
    public class PromptDialogHandler : BaseDialogHandler
    {
        readonly string _input;
        readonly bool _cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptDialogHandler"/> class.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> <see cref="HandleDialog"/> will click on the Cancel button of the prompt dialog.</param>
        public PromptDialogHandler(bool cancel)
        {
            _cancel = cancel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptDialogHandler"/> class.
        /// </summary>
        /// <param name="input">The text will be entered in the input field of the prompt dialog after which the OK button will be clicked.</param>
        public PromptDialogHandler(string input)
        {
            _input = input;
            _cancel = false;
        }

        /// <inheritdoc />
        public override bool HandleDialog(Window window)
        {
            if (CanHandleDialog(window))
            {
                window.ToFront();
                window.SetActivate();

                var inputBoxHwnd = new Hwnd(NativeMethods.GetChildWindowHwnd(window.Hwnd, "Edit"));

                if (inputBoxHwnd.hwnd == IntPtr.Zero) return false;

                if (_cancel)
                {
                    window.ForceClose();
                }
                else
                {
                    inputBoxHwnd.SetFocus();
                    inputBoxHwnd.SendString(_input);

                    var okButton = new WinButton(1, window.Hwnd);
                    okButton.Click();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the window is a prompt dialog by checking the <see cref="Window.StyleInHex"/>.
        /// Valid value is "94C800C4".
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>
        /// 	<c>true</c> if window is a prompt dialog; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandleDialog(Window window)
        {
            return (window.StyleInHex == "94C800C4");
        }
    }
}
