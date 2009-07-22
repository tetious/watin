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
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;
using StringComparer = WatiN.Core.Comparers.StringComparer;
using WatiN.Core.Exceptions;

namespace WatiN.Core
{
	/// <summary>
	/// This is the main class to access a webpage within a modal or modeless
	/// HTML dialog.
	/// </summary>
	public class HtmlDialog : DomContainer
	{
		private readonly IntPtr hwnd = IntPtr.Zero;

		public override IntPtr hWnd
		{
			get { return hwnd; }
		}

		public HtmlDialog(IntPtr windowHandle)
		{
			hwnd = windowHandle;
			StartDialogWatcher();
		}

		protected override void Dispose(bool disposing)
		{
			Close();
		}

	    public override void WaitForComplete(int waitForCompleteTimeOut)
	    {
	        WaitForComplete(new IEWaitForComplete((IEDocument) NativeDocument, waitForCompleteTimeOut));
	    }

	    public virtual void Close()
		{
			var dialog = new Window(hwnd);
			if (dialog.Visible)
			{
				dialog.ForceClose();
			}
			base.Dispose(true);
		}

		public override INativeDocument OnGetNativeDocument()
		{
			return new IEDocument(IEUtils.IEDOMFromhWnd(hwnd));
		}

        /// <inheritdoc />
        protected override string GetAttributeValueImpl(string attributeName)
        {
			string value = null;

            if (StringComparer.AreEqual(attributeName, "href", true))
			{
                UtilityClass.TryActionIgnoreException(() => value = Url);
			}
            else if (StringComparer.AreEqual(attributeName, "title", true))
			{
                UtilityClass.TryActionIgnoreException(() => value = Title);
			}
			else
			{
                throw new InvalidAttributeException(attributeName, "HTMLDialog");
			}

			return value;
		}
    }
}