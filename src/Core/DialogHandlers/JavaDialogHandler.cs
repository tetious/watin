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
using System.Threading;
using WatiN.Core.Exceptions;

namespace WatiN.Core.DialogHandlers
{
	public abstract class JavaDialogHandler : BaseDialogHandler
	{
		internal Window window;

		public string Title
		{
			get
			{
				ThrowExceptionIfDialogDoesNotExist();

				return window.Title;
			}
		}

		public string Message
		{
			get
			{
				ThrowExceptionIfDialogDoesNotExist();

				IntPtr messagehWnd = NativeMethods.GetDlgItem(window.Hwnd, 65535);
				return NativeMethods.GetWindowText(messagehWnd);
			}
		}

		public WinButton OKButton
		{
			get
			{
				ThrowExceptionIfDialogDoesNotExist();

				return new WinButton(getOKButtonID(), window.Hwnd);
			}
		}

		public override bool HandleDialog(Window window)
		{
			if (CanHandleDialog(window))
			{
				this.window = window;

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
			SimpleTimer timeoutTimer = new SimpleTimer(waitDurationInSeconds);

			while (!Exists() && !timeoutTimer.Elapsed)
			{
				Thread.Sleep(200);
			}

			if (!Exists())
			{
				throw new WatiNException(string.Format("Dialog not available within {0} seconds.", waitDurationInSeconds.ToString()));
			}
		}

		public abstract bool CanHandleDialog(Window window);

		public bool Exists()
		{
			if (window == null) return false;

			return window.Exists();
		}

		protected abstract int getOKButtonID();

		protected bool ButtonWithId1Exists(IntPtr windowHwnd)
		{
			WinButton button = new WinButton(1, windowHwnd);
			return button.Exists();
		}

		protected WinButton createCancelButton(IntPtr windowHwnd)
		{
			return new WinButton(2, windowHwnd);
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