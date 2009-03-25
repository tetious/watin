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
using System.Collections;
using WatiN.Core.Exceptions;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.DialogHandlers
{
	public class AlertAndConfirmDialogHandler : BaseDialogHandler
	{
		private readonly Queue alertQueue;

		public AlertAndConfirmDialogHandler()
		{
			alertQueue = new Queue();
		}

		/// <summary>
		/// Gets the count of the messages in the queue of displayed alert and confirm windows.
		/// </summary>
		/// <value>The count of the alert and confirm messages in the queue.</value>
		public int Count
		{
			get { return alertQueue.Count; }
		}

		/// <summary>
		/// Pops the most recent message from a queue of displayed alert and confirm windows.
		/// Use this method to get the displayed message.
		/// </summary>
		/// <returns>The displayed message.</returns>
        /// <exception cref="MissingAlertException">if no alerts are present</exception>
        public string Pop()
		{
			if (alertQueue.Count == 0)
			{
				throw new MissingAlertException();
			}

			return (string) alertQueue.Dequeue();
		}

   		/// <summary>
		/// Peeks at the most recent message from a queue of displayed alert and confirm windows, but does not remove it.
		/// Use this method to look at the first displayed message but not to remove it.
		/// </summary>
		/// <returns>The first displayed message.</returns>
		/// <exception cref="MissingAlertException">if no alerts are present</exception>
		public string Peek()
		{
			if (alertQueue.Count == 0)
			{
				throw new MissingAlertException();
			}

			return (string)alertQueue.Peek();
		}

		/// <summary>
		/// Gets the alert and confirm messages in the queue of displayed alert and confirm windows.
		/// </summary>
		/// <value>The alert and confirm messages in the queue.</value>
		public string[] Alerts
		{
			get
			{
				var result = new string[alertQueue.Count];
				Array.Copy(alertQueue.ToArray(), result, alertQueue.Count);
				return result;
			}
		}

		/// <summary>
		/// Clears all the messages from the queue of displayed alert and confirm windows.
		/// </summary>
		public void Clear()
		{
			alertQueue.Clear();
		}

		public override bool HandleDialog(Window window)
		{
			var handle = GetMessageBoxHandle(window);

			if (handle != IntPtr.Zero)
			{
				alertQueue.Enqueue(NativeMethods.GetWindowText(handle));

				window.ForceClose();

				return true;
			}

			return false;
		}

	    private static IntPtr GetMessageBoxHandle(Window window)
	    {
	        return NativeMethods.GetDlgItem(window.Hwnd, 0xFFFF);
	    }

        /// <summary>
        /// See if the dialog has a static control with a controlID 
        /// of 0xFFFF. This is unique for alert and confirm dialogboxes.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
	    public override bool CanHandleDialog(Window window)
	    {
	        return GetMessageBoxHandle(window) != IntPtr.Zero;
	    }
	}
}