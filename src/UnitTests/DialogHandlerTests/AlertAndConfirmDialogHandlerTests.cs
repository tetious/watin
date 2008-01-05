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

using NUnit.Framework;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
	[TestFixture]
	public class AlertAndConfirmDialogHandlerTests : WatiNTest
	{
		[Test]
		public void AlertAndConfirmDialogHandler()
		{
			DialogWatcher dialogWatcher;

			using (IE ie = new IE(MainURI))
			{
				Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero before test");

				// Create handler for Alert dialogs and register it.
				AlertAndConfirmDialogHandler dialogHandler = new AlertAndConfirmDialogHandler();
				using (new UseDialogOnce(ie.DialogWatcher, dialogHandler))
				{
					Assert.AreEqual(0, dialogHandler.Count);

					ie.Button("helloid").Click();

					Assert.AreEqual(1, dialogHandler.Count);
					Assert.AreEqual("hello", dialogHandler.Alerts[0]);

					// getting alert text
					Assert.AreEqual("hello", dialogHandler.Pop());

					Assert.AreEqual(0, dialogHandler.Count);

					// Test Clear
					ie.Button("helloid").Click();

					Assert.AreEqual(1, dialogHandler.Count);

					dialogHandler.Clear();

					Assert.AreEqual(0, dialogHandler.Count);

					dialogWatcher = ie.DialogWatcher;
				}
			}

			Assert.AreEqual(0, dialogWatcher.Count, "DialogWatcher count should be zero after test");
		}

		[Test, ExpectedException(typeof (MissingAlertException))]
		public void MissingAlertExceptionTest()
		{
			using (IE ie = new IE(MainURI))
			{
				Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero before test");

				AlertAndConfirmDialogHandler dialogHandler = new AlertAndConfirmDialogHandler();
				using (new UseDialogOnce(ie.DialogWatcher, dialogHandler))
				{
					dialogHandler.Pop();
				}
			}
		}
	}
}