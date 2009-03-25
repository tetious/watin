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
using System.Threading;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.DialogHandlers
{
	/// <summary>
	/// This class handles the logon dialog by passing in the username and password
	/// and clicking the OK button.
	/// </summary>
	/// <example>
	/// The following code shows the use of this dialog handler
	/// <code>
	/// IE ie = new IE();
	///
	/// ie.DialogWatcher.Add(new LogonDialogHandler(@"domain\username", "password"));
	///
	/// ie.GoTo("https://www.somesecuresite.com");
	/// </code>
	/// </example>
	public class LogonDialogHandler : BaseDialogHandler
	{
		private readonly string userName;
		private readonly string password;

		/// <summary>
		/// Initializes a new instance of the <see cref="LogonDialogHandler"/> class.
		/// </summary>
		/// <param name="userName">Name of the user. Is required.</param>
		/// <param name="password">The password. If no password is required, it can be left blank (<c>null</c> or <c>String.Empty</c>). </param>
		public LogonDialogHandler(string userName, string password)
		{
			checkArgument("Username must be specified", userName, "username");

		    this.userName = UtilityClass.EscapeSendKeysCharacters(userName);
			this.password = password == null ? String.Empty : UtilityClass.EscapeSendKeysCharacters(password);
		}

        /// <summary>
        /// Handles the logon dialog by filling in the username and password.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
		public override bool HandleDialog(Window window)
		{
			if (CanHandleDialog(window))
			{
				// Find Handle of the "Frame" and then the combo username entry box inside the frame
				var systemCredentialsHwnd = GetSystemCredentialsHwnd(window);
				
				NativeMethods.SetActiveWindow(systemCredentialsHwnd);
				Thread.Sleep(50);

				NativeMethods.SetForegroundWindow(systemCredentialsHwnd);
				Thread.Sleep(50);

                var windowEnumarator = new WindowsEnumerator();
                
                // Find input fields
                var edits = windowEnumarator.GetChildWindows(systemCredentialsHwnd, "Edit");
                
                // Enter userName
                var hwnd = new Hwnd(edits[0].Hwnd);
                hwnd.SetFocus();
            	hwnd.SendString(userName);
                
                // Enter password
                hwnd = new Hwnd(edits[1].Hwnd);
                hwnd.SetFocus();
            	hwnd.SendString(password);
                
            	// Click OK button
            	new WinButton(1, window.Hwnd).Click();
				
            	return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified window is a logon dialog.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>
		/// 	<c>true</c> if the specified window is a logon dialog; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanHandleDialog(Window window)
		{
			// If a logon dialog window is found hWnd will be set.
			return GetSystemCredentialsHwnd(window) != IntPtr.Zero;
		}

		private IntPtr GetSystemCredentialsHwnd(Window window)
		{
			return NativeMethods.GetChildWindowHwnd(window.Hwnd, "SysCredential");
		}
		
		private static void checkArgument(string message, string parameter, string parameterName)
		{
			if (UtilityClass.IsNullOrEmpty(parameter))
			{
				throw new ArgumentNullException(message, parameterName);
			}
		}
	}
}