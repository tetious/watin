#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

namespace WatiN.Core.DialogHandlers
{
    public class PromptDialogHandler : BaseDialogHandler
    {
        readonly string _input;
        readonly bool _cancel;

        public PromptDialogHandler(bool cancel)
        {
            _cancel = cancel;
        }

        public PromptDialogHandler(string input)
        {
            _input = input;
            _cancel = false;
        }

        public override bool HandleDialog(Window window)
        {
            if (IsPromptDialog(window))
            {
                window.ToFront();
                window.SetActivate();

                Window inputBox = new
                Window(NativeMethods.GetChildWindowHwnd(window.Hwnd, "Edit"));

                if (inputBox.Hwnd != IntPtr.Zero)
                {
                    if (_cancel)
                    {
                        window.ForceClose();
                    }
                    else
                    {
                        NativeMethods.SetActiveWindow(inputBox.Hwnd);

                        System.Windows.Forms.SendKeys.SendWait(_input);
                        System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the window is a prompt dialog.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>
        /// 	<c>true</c> if window is a prompt dialog; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPromptDialog(Window window)
        {
            return (window.StyleInHex == "94C800C4");
        }
    }
}
