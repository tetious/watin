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

using System.Threading;

namespace WatiN.Core.DialogHandlers
{
	public class SimpleJavaDialogHandler : BaseDialogHandler
	{
		private JavaDialogHandler dialogHandler;
		private bool clickCancelButton = false;
		private bool hasHandledDialog = false;
		private string message;

		public SimpleJavaDialogHandler()
		{
			dialogHandler = new AlertDialogHandler();
		}

		public SimpleJavaDialogHandler(bool clickCancelButton)
		{
			this.clickCancelButton = clickCancelButton;
			dialogHandler = new ConfirmDialogHandler();
		}

		public string Message
		{
			get { return message; }
		}

		public bool HasHandledDialog
		{
			get { return hasHandledDialog; }
		}

		public override bool HandleDialog(Window window)
		{
			if (dialogHandler.CanHandleDialog(window))
			{
				dialogHandler.window = window;

				message = dialogHandler.Message;

				ConfirmDialogHandler confirmDialogHandler = dialogHandler as ConfirmDialogHandler;

				// hasHandledDialog must be set before the Click and not
				// after because this code executes on a different Thread
				// and could lead to property HasHandledDialog returning false
				// while hasHandledDialog set had to be set.
				hasHandledDialog = true;

				if (confirmDialogHandler != null && clickCancelButton)
				{
					confirmDialogHandler.CancelButton.Click();
				}
				else
				{
					dialogHandler.OKButton.Click();
				}
			}

			return hasHandledDialog;
		}

        public bool WaitUntilHandled()
        {
            return WaitUntilHandled(Settings.WaitForCompleteTimeOut);
        }

        public bool WaitUntilHandled(int timeoutAfterSeconds)
        {
            SimpleTimer timer = new SimpleTimer(timeoutAfterSeconds);
            while (!HasHandledDialog && !timer.Elapsed)
            {
                Thread.Sleep(200);
            }

            return HasHandledDialog;
        }

	}
}