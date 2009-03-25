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
	public class VbScriptMsgBoxDialogHandler : WaitUntilHandledDialogHandler
	{
	    public enum Button
		{
			OK = 1,
			Cancel = 2,
			Abort = 3,
			Retry = 4,
			Ignore = 5,
			Yes = 6,
			No = 7
		}

		private readonly Button _button = Button.Yes;

		public VbScriptMsgBoxDialogHandler(Button button)
		{
			_button = button;
		}

		public override bool HandleDialog(Window window)
		{
			if (CanHandleDialog(window))
			{
				ButtonToPush(window).Click();

				HasHandledDialog = true;
				return true;
			}

			return false;
		}

        /// <summary>
        /// Determines whether VbScriptMsgBoxDialogHandler can handle the specified window by checking <see cref="Window.StyleInHex"/>.
        /// Valid value is "94C803C5".
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>
        /// 	<c>true</c> if VbScriptMsgBoxDialogHandler can handle the dialog; otherwise, <c>false</c>.
        /// </returns>
		public override bool CanHandleDialog(Window window)
		{
			return window.StyleInHex == "94C803C5";
		}

		private WinButton ButtonToPush(Window window)
		{
			var button = IfOKButtonThenGetTheRightButtonId(window, _button);

			return GetButton(window, button);
		}

		/// <summary>
		/// When OK is the only button on the msgbox (buttons value = 1)
		/// then the button Id = 2. In all other situations the button Id
		/// for OK is 1.
		/// </summary>
		/// <param name="window"></param>
		/// <param name="button"></param>
		/// <returns></returns>
		private Button IfOKButtonThenGetTheRightButtonId(Window window, Button button) 
		{
			if (button == Button.OK)
			{
				if (!GetButton(window, Button.OK).Exists())
				{
					return Button.Cancel;
				}
			}

			return button;
		}

		private static WinButton GetButton(Window window, Button button) 
		{
			return new WinButton((int)button, window.Hwnd);
		}

	}
}