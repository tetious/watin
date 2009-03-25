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
	public class CertificateWarningHandler : BaseDialogHandler
	{
		public enum ButtonsEnum
		{
			Yes = 1,
			No = 2
		}

		private const string certificateWarningDialogStyle = "94C808C4";

		private readonly ButtonsEnum buttonToPush;

		/// <summary>
		/// Initializes a new instance of the <see cref="CertificateWarningHandler"/> class.
		/// This handler will click the "Yes" button at the certificate warning dialog.
		/// </summary>
		public CertificateWarningHandler()
		{
			buttonToPush = ButtonsEnum.Yes;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CertificateWarningHandler"/> class.
		/// </summary>
		/// <param name="buttonToPush">The button to push.</param>
		public CertificateWarningHandler(ButtonsEnum buttonToPush)
		{
			this.buttonToPush = buttonToPush;
		}

		public override bool HandleDialog(Window window)
		{
			if (CanHandleDialog(window))
			{
				NativeMethods.SetForegroundWindow(window.Hwnd);
				NativeMethods.SetActiveWindow(window.Hwnd);

				ButtonToPush(window).Click();

				return true;
			}

			return false;
		}

        /// <summary>
        /// Determines whether the specified window is a certificate dialog by checking <see cref="Window.StyleInHex"/>.
        /// valid value is "94C808C4".
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>
        /// 	<c>true</c> if the specified window is a certificate dialog; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandleDialog(Window window)
	    {
	        return window.StyleInHex == certificateWarningDialogStyle;
	    }

	    private WinButton ButtonToPush(Window window)
		{
			return new WinButton((int) buttonToPush, window.Hwnd);
		}
	}
}
