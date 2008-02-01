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
using NUnit.Framework;
using SHDocVw;
using WatiN.Core.DialogHandlers;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ReturnJavaDialogHandlerTest : BaseWatiNTest
	{
		[Test]
		public void WhenOnBeforeUnloadReturnJavaDialogIsShown_ClickingOnOkShouldCloseIE()
		{
			using (IE ie = new IE(OnBeforeUnloadJavaDialogURI))
			{
				ReturnDialogHandler returnDialogHandler = new ReturnDialogHandler();
				ie.AddDialogHandler(returnDialogHandler);

				IntPtr hWnd = ie.hWnd;
				// can't use ie.Close() here cause this will cleanup the registered
				// returnDialogHandler which leads to a timeout on the WaitUntilExists
				SHDocVw.InternetExplorer internetExplorer = (SHDocVw.InternetExplorer) ie.InternetExplorer;
				internetExplorer.Quit();

				returnDialogHandler.WaitUntilExists();
				returnDialogHandler.OKButton.Click();

				Thread.Sleep(2000);
				Assert.IsFalse(IE.Exists(new AttributeConstraint("hwnd", hWnd.ToString())));
			}
		}

		[Test]
		public void WhenOnBeforeUnloadReturnJavaDialogIsShown_ClickingOnCancelShouldKeepIEOpen()
		{
			using (IE ie = new IE(OnBeforeUnloadJavaDialogURI))
			{
				ReturnDialogHandler returnDialogHandler = new ReturnDialogHandler();
				ie.AddDialogHandler(returnDialogHandler);

				IntPtr hWnd = ie.hWnd;

				// can't use ie.Close() here cause this will cleanup the registered
				// returnDialogHandler which leads to a timeout on the WaitUntilExists
				SHDocVw.InternetExplorer internetExplorer = (SHDocVw.InternetExplorer) ie.InternetExplorer;
				internetExplorer.Quit();

				returnDialogHandler.WaitUntilExists();
				returnDialogHandler.CancelButton.Click();

				Thread.Sleep(2000);
				Assert.IsTrue(IE.Exists(new AttributeConstraint("hwnd", hWnd.ToString())));

				// finally close the ie instance
				internetExplorer.Quit();
				returnDialogHandler.WaitUntilExists();
				returnDialogHandler.OKButton.Click();
			}
		}
	}
}