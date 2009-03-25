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
	public class ReturnDialogHandler : ConfirmDialogHandler
	{
        /// <summary>
        /// Determines whether this instance can handle the specified window by checking <see cref="Window.StyleInHex"/>.
        /// equals "94C803C5" and if the window has a button with Id 1.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can handle dialog] the specified window; otherwise, <c>false</c>.
        /// </returns>
		public override bool CanHandleDialog(Window window)
		{
			return (window.StyleInHex == "94C803C5" && ButtonWithId1Exists(window.Hwnd));
		}
	}
}