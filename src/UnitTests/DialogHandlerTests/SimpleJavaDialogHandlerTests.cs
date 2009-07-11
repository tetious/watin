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
	public class SimpleJavaDialogHandlerTests : BaseWithBrowserTests
	{
		[Test]
		public void AlertDialogSimpleJavaDialogHandler()
		{
			Assert.AreEqual(0, Ie.DialogWatcher.Count, "DialogWatcher count should be zero");

			SimpleJavaDialogHandler dialogHandler = new SimpleJavaDialogHandler();

			Assert.IsFalse(dialogHandler.HasHandledDialog, "Alert Dialog should not be handled.");
			Assert.IsNull(dialogHandler.Message, "Message should be null");

			using (new UseDialogOnce(Ie.DialogWatcher, dialogHandler))
			{
				Ie.Button(Find.ByValue("Show alert dialog")).Click();

				Assert.IsTrue(dialogHandler.HasHandledDialog, "Alert Dialog should be handled.");
				Assert.AreEqual("This is an alert!", dialogHandler.Message, "Unexpected message");
			}
		}

		[Test]
		public void AlertDialogSimpleJavaDialogHandler2()
		{
			SimpleJavaDialogHandler dialogHandler = new SimpleJavaDialogHandler();

			using (new UseDialogOnce(Ie.DialogWatcher, dialogHandler))
			{
				Ie.Button(Find.ByValue("Show alert dialog")).Click();

				Assert.AreEqual("This is an alert!", dialogHandler.Message, "Unexpected message");
			}
		}

		[Test]
		public void ConfirmDialogSimpleJavaDialogHandlerCancel()
		{
			Assert.AreEqual(0, Ie.DialogWatcher.Count, "DialogWatcher count should be zero");

			SimpleJavaDialogHandler dialogHandler = new SimpleJavaDialogHandler(true);
			using (new UseDialogOnce(Ie.DialogWatcher, dialogHandler))
			{
				Ie.Button(Find.ByValue("Show confirm dialog")).Click();

				Assert.IsTrue(dialogHandler.HasHandledDialog, "Confirm Dialog should be handled.");
				Assert.AreEqual("Do you want to do xyz?", dialogHandler.Message);
				Assert.AreEqual("Cancel", Ie.TextField("ReportConfirmResult").Text, "Cancel button expected.");
			}
		}

		[Test]
		public void IEUseOnceDialogHandler()
		{
			Assert.AreEqual(0, Ie.DialogWatcher.Count, "DialogWatcher count should be zero");

			SimpleJavaDialogHandler dialogHandler = new SimpleJavaDialogHandler();

			using (new UseDialogOnce(Ie.DialogWatcher, dialogHandler))
			{
				Ie.Button(Find.ByValue("Show alert dialog")).Click();

				Assert.IsTrue(dialogHandler.HasHandledDialog, "Alert Dialog should be handled.");
				Assert.AreEqual("This is an alert!", dialogHandler.Message, "Unexpected message");
			}
		}

		public override Uri TestPageUri
		{
			get { return TestEventsURI; }
		}
	}
}