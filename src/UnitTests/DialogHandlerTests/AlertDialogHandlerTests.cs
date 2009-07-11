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