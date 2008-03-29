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
using System.Globalization;
using System.Runtime.InteropServices;
using mshtml;
using WatiN.Core.InternetExplorer;
using StringComparer = WatiN.Core.Comparers.StringComparer;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// This is the main class to access a webpage within a modal or modeles
	/// HTML dialog.
	/// </summary>
	public class HtmlDialog : DomContainer, IAttributeBag
	{
		private IntPtr hwnd = IntPtr.Zero;

		public override IntPtr hWnd
		{
			get { return hwnd; }
		}

		public override INativeBrowser NativeBrowser
		{
			get { return new IEBrowser(this); }
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

		public void Close()
		{
			Window dialog = new Window(hwnd);
			if (dialog.Visible)
			{
				dialog.ForceClose();
			}
			base.Dispose(true);
		}

		public override IHTMLDocument2 OnGetHtmlDocument()
		{
			return IEDOMFromhWnd(hwnd);
		}

		private IHTMLDocument2 IEDOMFromhWnd(IntPtr hWnd)
		{
			Guid IID_IHTMLDocument2 = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");

			Int32 lRes = 0;
			Int32 lMsg;
			Int32 hr;

			if (IsIETridentDlgFrame(hWnd))
			{
				if (!IsIEServerWindow(hWnd))
				{
					// Get 1st child IE server window
					hWnd = NativeMethods.GetChildWindowHwnd(hWnd, "Internet Explorer_Server");
				}

				if (IsIEServerWindow(hWnd))
				{
					// Register the message
					lMsg = NativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");
					// Get the object
					NativeMethods.SendMessageTimeout(hWnd, lMsg, 0, 0, NativeMethods.SMTO_ABORTIFHUNG, 1000, ref lRes);
					if (lRes != 0)
					{
						// Get the object from lRes
						IHTMLDocument2 ieDOMFromhWnd = null;
						hr = NativeMethods.ObjectFromLresult(lRes, ref IID_IHTMLDocument2, 0, ref ieDOMFromhWnd);
						if (hr != 0)
						{
							throw new COMException("ObjectFromLresult has thrown an exception", hr);
						}
						return ieDOMFromhWnd;
					}
				}
			}
			return null;
		}

		internal static bool IsIETridentDlgFrame(IntPtr hWnd)
		{
			return UtilityClass.CompareClassNames(hWnd, "Internet Explorer_TridentDlgFrame");
		}

		private static bool IsIEServerWindow(IntPtr hWnd)
		{
			return UtilityClass.CompareClassNames(hWnd, "Internet Explorer_Server");
		}

		public string GetValue(string attributename)
		{
			string value = null;

			if (StringComparer.AreEqual(attributename, "href", true))
			{
				try
				{
					value = Url;
				}
				catch {}
			}
			else if (StringComparer.AreEqual(attributename, "title", true))
			{
				try
				{
					value = Title;
				}
				catch {}
			}
			else
			{
				throw new InvalidAttributException(attributename, "HTMLDialog");
			}

			return value;
		}
	}
}