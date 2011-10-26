#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.DialogHandlers
{
	public abstract class BaseDialogHandler : IDialogHandler
	{
		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return GetType().ToString().GetHashCode();
		}

		#region IDialogHandler Members

	    /// <inheritdoc />
		public abstract bool HandleDialog(Window window);

		/// <inheritdoc />
		public virtual bool CanHandleDialog(Window window, IntPtr mainWindowHwnd)
		{
            var ieVersion = IE.GetMajorIEVersion();
            var dialogBelongsToIeWindow = ieVersion < 8 ? 
                                        DialogBelongsToIEWindowForIe7AndLower(window, mainWindowHwnd) : 
                                        DialogBelongsToIEWindowForIe8AndHigher(window, mainWindowHwnd);

            return dialogBelongsToIeWindow && CanHandleDialog(window);
        }

	    private static bool DialogBelongsToIEWindowForIe7AndLower(Window window, IntPtr mainWindowHwnd)
	    {
	        var mainWindow = new Window(mainWindowHwnd);
	        return window.ToplevelWindow.Equals(mainWindow);
        }

        private static bool DialogBelongsToIEWindowForIe8AndHigher(Window window, IntPtr mainWindowHwnd)
        {
            var mainWindow = new Window(mainWindowHwnd);
            Logger.LogDebug("Main: " + mainWindow.Hwnd + ", " + mainWindow.Title + ", " + mainWindow.ProcessID);
            
            var hWnd = IEUtils.GetInteretExplorerServerHwnd(mainWindowHwnd);
            var window1 = new Window(hWnd);            
            Logger.LogDebug("IES: " + window1.Hwnd + ", " + window1.Title + ", " + window1.ProcessID);

            return window1.ProcessID == window.ProcessID;
        }

		public abstract bool CanHandleDialog(Window window);

		#endregion
	}
}
