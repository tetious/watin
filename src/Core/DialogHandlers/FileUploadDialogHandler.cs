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
	public class FileUploadDialogHandler : BaseDialogHandler
	{
		private string fileName;

		public FileUploadDialogHandler(string fileName)
		{
			this.fileName = UtilityClass.EscapeSendKeysCharacters(fileName);
		}

		public override bool HandleDialog(Window window)
		{
			if (IsFileUploadDialog(window))
			{
				IntPtr usernameControlHandle = NativeMethods.GetChildWindowHwnd(window.Hwnd, "Edit");

				NativeMethods.SetForegroundWindow(usernameControlHandle);
				NativeMethods.SetActiveWindow(usernameControlHandle);


				System.Windows.Forms.SendKeys.SendWait(fileName + "{ENTER}");
				return true;
			}

			return false;
		}

		public bool IsFileUploadDialog(Window window)
		{
			// "96CC20C4" is valid for Windows XP, Win 2000 and Win 2003
			// "96CC02C4" is valid for Windows Vista
			return (window.StyleInHex == "96CC20C4") || (window.StyleInHex == "96CC02C4");
		}
	}
}