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
	/// This class handles the print dialog clicking the Print, 
	/// Cancel or Apply button.
	/// </summary>
	/// <example>
	/// The following code shows the use of this dialog handler
	/// <code>
	/// IE ie = new IE();
	///
	/// ie.DialogWatcher.Add(new PrintDialogHandler(PrintDialogHandler.ButtonsEnum.Cancel));
	///
	/// ie.GoTo("http://www.someprintdialog.com");
	/// </code>
	/// </example>
	public class PrintDialogHandler : BaseDialogHandler
	{
		public enum ButtonsEnum
		{
			Print = 1,
			Cancel = 2,
		}

		// need to be checked whether this is valid for other operating systems 
        // for WinXP and Win2003
		private const string printDialogStyle = "96C820C4";
        // for Vista and Win2008
        private const string printDialogStyleVista = "96C800C4";

        private ButtonsEnum buttonToPush;

		public PrintDialogHandler(ButtonsEnum buttonToPush)
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

		private WinButton ButtonToPush(Window window)
		{
			return new WinButton((int) buttonToPush, window.Hwnd);
		}

		/// <summary>
		/// Determines whether the specified window is a print dialog by checking the <see cref="Window.StyleInHex"/>.
        /// Valid values are "96C820C4" or "96C800C4".
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>
		/// <c>true</c> if the specified window is a print dialog; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanHandleDialog(Window window)
		{
			return (window.StyleInHex == printDialogStyle || window.StyleInHex == printDialogStyleVista);
		}
	}
}