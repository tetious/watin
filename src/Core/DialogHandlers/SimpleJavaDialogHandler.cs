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
	public class SimpleJavaDialogHandler : WaitUntilHandledDialogHandler
	{
		private readonly JavaDialogHandler dialogHandler;
		private readonly bool clickCancelButton;

	    public SimpleJavaDialogHandler()
		{
			dialogHandler = new AlertDialogHandler();
		}

		public SimpleJavaDialogHandler(bool clickCancelButton)
		{
			this.clickCancelButton = clickCancelButton;
			dialogHandler = new ConfirmDialogHandler();
		}

	    public string Message { get; private set; }

	    public override bool HandleDialog(Window window)
		{
			if (CanHandleDialog(window))
			{
				dialogHandler._window = window;

				Message = dialogHandler.Message;

				var confirmDialogHandler = dialogHandler as ConfirmDialogHandler;

				// hasHandledDialog must be set before the Click and not
				// after because this code executes on a different Thread
				// and could lead to property HasHandledDialog returning false
				// while hasHandledDialog set had to be set.
				HasHandledDialog = true;

				if (confirmDialogHandler != null && clickCancelButton)
				{
					confirmDialogHandler.CancelButton.Click();
				}
				else
				{
					dialogHandler.OKButton.Click();
				}
			}

			return HasHandledDialog;
		}

	    public override bool CanHandleDialog(Window window)
	    {
	        return dialogHandler.CanHandleDialog(window);
	    }
	}
}