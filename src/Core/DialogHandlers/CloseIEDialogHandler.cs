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

using WatiN.Core.Native.Windows;

namespace WatiN.Core.DialogHandlers
{
    public class CloseIEDialogHandler : BaseDialogHandler
    {
        private const int okButtonId = 6;
        private const int cancelButtonId = 7;
        
        private readonly bool _clickOnOK;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseIEDialogHandler"/> class.
        /// </summary>
        /// <param name="clickOnOK">if set to <c>true</c> the OK button will be clicked on. Otherwise the Cancel button will be clicked.</param>
        public CloseIEDialogHandler(bool clickOnOK)
        {
            _clickOnOK = clickOnOK;
        }

        public override bool HandleDialog(Window window)
        {
            if (CanHandleDialog(window))
            {
                var buttonId = _clickOnOK ? okButtonId : cancelButtonId;

                var button = new WinButton(buttonId, window.Hwnd);
                button.Click();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if <see cref="Window.StyleInHex"/> of the <paramref name="window"/> is equal to "94C801C5".
        /// </summary>
        /// <param name="window">The window to validate</param>
        /// <returns></returns>
        public override bool CanHandleDialog(Window window)
        {
            return (window.StyleInHex == "94C801C5");
        }
    }
}
