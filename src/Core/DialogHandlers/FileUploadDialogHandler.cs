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
	public class FileUploadDialogHandler : BaseDialogHandler
	{
		private readonly string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadDialogHandler"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file which should be uploaded.</param>
		public FileUploadDialogHandler(string fileName)
		{
		    this.fileName = fileName;
		}

        /// <summary>
        /// Handles the File upload dialog.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
		public override bool HandleDialog(Window window)
		{
            if (CanHandleDialog(window))
			{
				var fileNameHandle = NativeMethods.GetChildWindowHwnd(window.Hwnd, "Edit");
			    var fileNameHwnd = new Hwnd(fileNameHandle);

			    fileNameHwnd.SetFocus();
                fileNameHwnd.SendString(fileName);

                var openButton = new WinButton(1, window.Hwnd);
                openButton.Click();
                
				return true;
			}

			return false;
		}

        /// <summary>
        /// Determines whether this instance can handle the specified window.
        /// It check <see cref="Window.StyleInHex"/> for the value "96CC20C4"
        /// or "96CC02C4".
        /// </summary>
        /// <param name="window">The window to validate.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can handle the dialog; otherwise, <c>false</c>.
        /// </returns>
		public override bool CanHandleDialog(Window window)
		{
			return (window.StyleInHex == "96CC20C4") || (window.StyleInHex == "96CC02C4");
		}
	}
}