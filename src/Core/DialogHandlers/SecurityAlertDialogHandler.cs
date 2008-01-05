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

namespace WatiN.Core.DialogHandlers
{
	/// <summary>
	/// This class handles the Security Warning dialog and does press
	/// the OK button when the dialog shows up.
	/// </summary>
	public class SecurityAlertDialogHandler : BaseDialogHandler
	{
		private const string securityAlertDialogStyle = "94C80AC4";

		/// <summary>
		/// Handles the dialog if the <paramref name="window" /> is a
		/// security alert dialog.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns></returns>
		public override bool HandleDialog(Window window)
		{
			if (IsSecurityAlertDialog(window))
			{
				NativeMethods.SetForegroundWindow(window.Hwnd);
				NativeMethods.SetActiveWindow(window.Hwnd);

				WinButton buttonOk = new WinButton(1, window.Hwnd);
				buttonOk.Click();

				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified window is a security alert dialog.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>
		/// 	<c>true</c> if the specified window is a security alert dialog; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsSecurityAlertDialog(Window window)
		{
			return window.StyleInHex == securityAlertDialogStyle;
		}
	}
}