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
using WatiN.Core.Exceptions;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.DialogHandlers
{
	public abstract class JavaDialogHandler : BaseDialogHandler
	{
		internal Window _window;

		public string Title
		{
			get
			{
				ThrowExceptionIfDialogDoesNotExist();

				return _window.Title;
			}
		}

		public string Message
		{
			get
			{
				ThrowExceptionIfDialogDoesNotExist();

			    return _window.Message;
			}
		}

		public WinButton OKButton
		{
			get
			{
				ThrowExceptionIfDialogDoesNotExist();

				return new WinButton(getOKButtonID(), _window.Hwnd);
			}
		}

		public override bool HandleDialog(Window window)
		{
			if (CanHandleDialog(window))
			{
				_window = window;

				while (window.Exists())
				{
					Thread.Sleep(200);
				}
				return true;
			}
			return false;
		}

		public void WaitUntilExists()
		{
			WaitUntilExists(30);
		}

		public void WaitUntilExists(int waitDurationInSeconds)
		{
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(waitDurationInSeconds));
            tryActionUntilTimeOut.Try(() => Exists());
            
			if (!Exists())
			{
				throw new WatiNException(string.Format("Dialog not available within {0} seconds.", waitDurationInSeconds));
			}
		}

	    public abstract override bool CanHandleDialog(Window window);

		public bool Exists()
		{
		    return _window != null && _window.Exists();
		}

	    protected abstract int getOKButtonID();

		protected static bool ButtonWithId1Exists(IntPtr windowHwnd)
		{
			var button = new WinButton(1, windowHwnd);
			return button.Exists();
		}

		protected void ThrowExceptionIfDialogDoesNotExist()
		{
			if (!Exists())
			{
				throw new WatiNException("Operation not available. Dialog doesn't exist.");
			}
		}
	}
}