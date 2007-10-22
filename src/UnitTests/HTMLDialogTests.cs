#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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
using WatiN.Core.Exceptions;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class HTMLDialogTests : WatiNTest
	{
		[Test]
		public void HTMLDialogModalByTitle()
		{
			using (IE ie = new IE(MainURI))
			{
				ie.Button("modalid").ClickNoWait();

				using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest")))
				{
					Assert.IsInstanceOfType(typeof (DomContainer), htmlDialog);

					Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
					Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");

					htmlDialog.TextField("name").TypeText("Textfield in HTMLDialog");
					htmlDialog.Button("hello").Click();
				}
			}
		}

		[Test]
		public void HTMLDialogModalByUrl()
		{
			using (IE ie = new IE(MainURI))
			{
				ie.Button("modalid").ClickNoWait();

				using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByUrl(PopUpURI)))
				{
					Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
					Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");
				}
			}
		}

		[Test]
		public void HTMLDialogsExists()
		{
			using (IE ie = new IE(MainURI))
			{
				AttributeConstraint findBy = Find.ByUrl(PopUpURI);
				Assert.IsFalse(ie.HtmlDialogs.Exists(findBy));

				ie.Button("modalid").ClickNoWait();

				Thread.Sleep(1000);

				Assert.IsTrue(ie.HtmlDialogs.Exists(findBy));
			}
		}

		[Test]
		public void HTMLDialogNotFoundException()
		{
			using (IE ie = new IE(MainURI))
			{
				DateTime startTime = DateTime.Now;
				const int timeoutTime = 5;
				string expectedMessage = "Could not find a HTMLDialog by title with value 'popuptest'. (Search expired after '5' seconds)";

				try
				{
					// Time out after timeoutTime seconds
					startTime = DateTime.Now;
					ie.HtmlDialog(Find.ByTitle("PopUpTest"), timeoutTime);
					Assert.Fail("PopUpTest should not be found");
				}
				catch (Exception e)
				{
					Assert.IsInstanceOfType(typeof (HtmlDialogNotFoundException), e);
					// add 1 second to give it some slack.
					Assert.Greater(timeoutTime + 1, DateTime.Now.Subtract(startTime).TotalSeconds);
					Assert.AreEqual(expectedMessage, e.Message, "Unexpected exception message");
				}
			}
		}
	}
}