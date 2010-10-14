#region WatiN Copyright (C) 2006-2010 Jeroen van Menen

//Copyright 2006-2010 Jeroen van Menen
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
using NUnit.Framework;
using WatiN.Core.DialogHandlers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
	[TestFixture]
	public class AlertDialogHandlerTests : BaseWithBrowserTests
	{
		[Test]
		public void AlertDialogHandler()
		{
			Assert.AreEqual(0, Ie.DialogWatcher.Count, "DialogWatcher count should be zero");

			var alertDialogHandler = new AlertDialogHandler();
			using (new UseDialogOnce(Ie.DialogWatcher, alertDialogHandler))
			{
				Ie.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

				alertDialogHandler.WaitUntilExists();

				var message = alertDialogHandler.Message;
				alertDialogHandler.OKButton.Click();

				Ie.WaitForComplete();

				Assert.AreEqual("This is an alert!", message, "Unexpected message");
				Assert.IsFalse(alertDialogHandler.Exists(), "Alert Dialog should be closed.");
			}
		}

		[Test]
		public void CloseSpecificBrowserAlert()
		{
			Assert.AreEqual(0, Ie.DialogWatcher.Count, "DialogWatcher count should be zero");

			var ie1 = new IE(TestPageUri);

			// set up a second browser with an open dialog
			var alertDialogHandler1 = new AlertDialogHandler();
			using (new UseDialogOnce(ie1.DialogWatcher, alertDialogHandler1))
			{
				ie1.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

				alertDialogHandler1.WaitUntilExists();

                // close the original message
				var alertDialogHandler2 = new AlertDialogHandler();
				using (new UseDialogOnce(Ie.DialogWatcher, alertDialogHandler2))
				{
					Ie.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

					alertDialogHandler2.WaitUntilExists();

					var message = alertDialogHandler2.Message;
					alertDialogHandler2.OKButton.Click();

					Ie.WaitForComplete();

					Assert.IsTrue(alertDialogHandler1.Exists(), "Original Alert Dialog should be open.");

					Assert.AreEqual("This is an alert!", message, "Unexpected message");
					Assert.IsFalse(alertDialogHandler2.Exists(), "Alert Dialog should be closed.");
				}

                // close the second message
				alertDialogHandler1.OKButton.Click();

				Assert.IsFalse(alertDialogHandler1.Exists(), "Alert Dialog should be closed.");
			}
		}

		[Test]
		public void AlertDialogHandlerWithoutAutoCloseDialogs()
		{
			Assert.AreEqual(0, Ie.DialogWatcher.Count, "DialogWatcher count should be zero");

			Ie.DialogWatcher.CloseUnhandledDialogs = false;

			Ie.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

			var alertDialogHandler = new AlertDialogHandler();

			using (new UseDialogOnce(Ie.DialogWatcher, alertDialogHandler))
			{
				alertDialogHandler.WaitUntilExists();

				var message = alertDialogHandler.Message;
				alertDialogHandler.OKButton.Click();

				Ie.WaitForComplete();

				Assert.AreEqual("This is an alert!", message, "Unexpected message");
				Assert.IsFalse(alertDialogHandler.Exists(), "Alert Dialog should be closed.");
			}
		}

		public override Uri TestPageUri
		{
			get { return TestEventsURI; }
		}
	}
}