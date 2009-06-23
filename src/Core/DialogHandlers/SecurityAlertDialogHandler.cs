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
	/// <summary>
	/// This class handles the Security Warning dialog and does press
	/// the OK button when the dialog shows up.
	/// </summary>
	public class SecurityAlertDialogHandler : BaseDialogHandler
	{
		private const string securityAlertDialogStyleWithOkButton = "94C80AC4";
        private const string securityAlertDialogStyleWithYesButton = "94C808C4";

		/// <summary>
		/// Handles the dialog if the <paramref name="window" /> is a
		/// security alert dialog.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns></returns>
		public override bool HandleDialog(Window window)
		{
            if (CanHandleDialog(window))
			{
				NativeMethods.SetForegroundWindow(window.Hwnd);
				NativeMethods.SetActiveWindow(window.Hwnd);

				var buttonOk = new WinButton(1, window.Hwnd);
				if (buttonOk.Exists())
                    buttonOk.Click();
                else
				{
				    var buttonYes = new WinButton(6, window.Hwnd);
                    buttonYes.Click();
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified window is a security alert dialog by checking <see cref="Window.StyleInHex"/>.
        /// Valid value is "94C80AC4" or "94C808C4".
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>
		/// 	<c>true</c> if the specified window is a security alert dialog; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanHandleDialog(Window window)
		{
			return window.StyleInHex == securityAlertDialogStyleWithOkButton ||
                   window.StyleInHex == securityAlertDialogStyleWithYesButton;
		}
	}
}