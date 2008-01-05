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
using NUnit.Framework;
using Rhino.Mocks;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class DialogWatcherTests : WatiNTest
	{
		private ILogWriter saveLoggerInstance;

		[SetUp]
		public void SetUp()
		{
			saveLoggerInstance = Logger.LogWriter;
		}

		[TearDown]
		public void TearDown()
		{
			IE.Settings.Reset();
			Logger.LogWriter = saveLoggerInstance;
		}

		[Test]
		public void DialogWatcherShouldKeepRunningWhenClosingOneOfTwoInstancesInSameProcess()
		{
			using (IE ie1 = new IE(), ie2 = new IE())
			{
				Assert.AreEqual(ie1.ProcessID, ie2.ProcessID, "Processids should be the same");

				DialogWatcher dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie1.ProcessID);

				Assert.IsNotNull(dialogWatcher, "dialogWatcher should not be null");
				Assert.AreEqual(ie1.ProcessID, dialogWatcher.ProcessId, "Processids of ie1 and dialogWatcher should be the same");
				Assert.AreEqual(2, dialogWatcher.ReferenceCount, "Expected 2 as reference count");
				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist");
				Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should be running");

				ie2.Close();

				Assert.AreEqual(1, dialogWatcher.ReferenceCount, "Expected 1 as reference count");
				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should still exist");
				Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should still be running");

				ie1.Close();

				Assert.AreEqual(0, dialogWatcher.ReferenceCount, "Expected 0 as reference count");
				Assert.IsFalse(dialogWatcher.IsRunning, "dialogWatcher should not be running");
			}
		}

		[Test]
		public void DialogWatcherShouldTerminateWhenNoWatiNCoreIEInstancesExistButProcessDoesExist()
		{
			int ieProcessId;

			// Create running Internet Explorer instance but no longer referenced by 
			// an instance of WatiN.Core.IE.
			using (IE ie = new IE(MainURI))
			{
				ie.AutoClose = false;
				ieProcessId = ie.ProcessID;
			}

			// Create IE instances and see if DialogWatcher behaves as expected
			using (IE ie1 = new IE())
			{
				Assert.AreEqual(ieProcessId, ie1.ProcessID, "ProcessIds ie and ie1 should be the same");

				using (IE ie2 = new IE())
				{
					Assert.AreEqual(ie1.ProcessID, ie2.ProcessID, "Processids ie1 and ie2 should be the same");
				}

				DialogWatcher dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie1.ProcessID);

				Assert.IsNotNull(dialogWatcher, "dialogWatcher should not be null");
				Assert.AreEqual(ie1.ProcessID, dialogWatcher.ProcessId, "Processids of ie1 and dialogWatcher should be the same");
				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist");
				Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should be running");

				ie1.Close();

				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist after ie1.close");
				Assert.IsFalse(dialogWatcher.IsRunning, "dialogWatcher should not be running");
			}

			// Find created but not referenced Internet Explorer instance and close it.
			IE.AttachToIE(Find.ByUrl(MainURI)).Close();
		}

		[Test]
		public void DialogWatcherOfIEAndHTMLDialogShouldNotBeNull()
		{
			using (IE ie = new IE(MainURI))
			{
				Assert.IsNotNull(ie.DialogWatcher, "ie.DialogWatcher should not be null");

				ie.Button("modalid").ClickNoWait();

				using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest")))
				{
					Assert.IsNotNull(htmlDialog.DialogWatcher, "htmlDialog.DialogWatcher should not be null");
				}
			}
		}

		[Test]
		public void DialogWatcherShouldKeepRunningWhenClosingHTMLDialog()
		{
			using (IE ie = new IE(MainURI))
			{
				ie.Button("modalid").ClickNoWait();

				DialogWatcher dialogWatcher;
				using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest")))
				{
					Assert.AreEqual(ie.ProcessID, htmlDialog.ProcessID, "Processids should be the same");

					dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie.ProcessID);
					Assert.IsNotNull(dialogWatcher, "dialogWatcher should not be null");
					Assert.AreEqual(ie.ProcessID, dialogWatcher.ProcessId, "Processids of ie and dialogWatcher should be the same");
					Assert.AreEqual(2, dialogWatcher.ReferenceCount, "Expected 2 as reference count");
					Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist");
					Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should be running");
				}

				Assert.AreEqual(1, dialogWatcher.ReferenceCount, "Expected 1 as reference count");
				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should still exist");
				Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should still be running");

				ie.WaitForComplete();
				ie.Close();

				Assert.AreEqual(0, dialogWatcher.ReferenceCount, "Expected 0 as reference count");
				Assert.IsFalse(dialogWatcher.IsRunning, "dialogWatcher should not be running");
			}
		}

		[Test]
		public void ThrowReferenceCountException()
		{
			using (IE ie = new IE())
			{
				DialogWatcher dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie.ProcessID);
				Assert.AreEqual(1, dialogWatcher.ReferenceCount);

				dialogWatcher.DecreaseReferenceCount();

				Assert.AreEqual(0, dialogWatcher.ReferenceCount);

				try
				{
					dialogWatcher.DecreaseReferenceCount();
					Assert.Fail("ReferenceCountException expected");
				}
				catch (ReferenceCountException) {}
				catch
				{
					Assert.Fail("ReferenceCountException expected");
				}
				finally
				{
					dialogWatcher.IncreaseReferenceCount();
				}
			}
		}

		[Test]
		public void ExceptionsInDialogHandlersShouldBeLoggedAndNeglected()
		{
			MockRepository mocks = new MockRepository();

			//Make the mocks
			ILogWriter mockLogWriter = (ILogWriter) mocks.CreateMock(typeof (ILogWriter));
			IDialogHandler buggyDialogHandler = (IDialogHandler) mocks.CreateMock(typeof (IDialogHandler));
			IDialogHandler nextDialogHandler = (IDialogHandler) mocks.CreateMock(typeof (IDialogHandler));
			Window dialog = (Window) mocks.DynamicMock(typeof (Window), IntPtr.Zero);

			// Handle window does check if window IsDialog and Visible
			Expect.Call(dialog.IsDialog()).Return(true);
			Expect.Call(dialog.Visible).Return(true);

			// If this HandleDialog is called throw an exception
			Expect.Call(buggyDialogHandler.HandleDialog(dialog)).Throw(new Exception());
			// Expect Logger will be called with the exception text and stack trace
			mockLogWriter.LogAction("");
			LastCall.Constraints(Rhino.Mocks.Text.Like("Exception was thrown while DialogWatcher called HandleDialog:"));
			mockLogWriter.LogAction("");
			LastCall.Constraints(Rhino.Mocks.Text.StartsWith("System.Exception:"));
			// Expect the next dialogHandler will be called even do an exception
			// has been thrown by the previous handler
			Expect.Call(nextDialogHandler.HandleDialog(dialog)).Return(true);

			mocks.ReplayAll();

			// Set Logger
			Logger.LogWriter = mockLogWriter;

			// Add dialogHandlers
			DialogWatcher dialogWatcher = new DialogWatcher(0);
			dialogWatcher.Add(buggyDialogHandler);
			dialogWatcher.Add(nextDialogHandler);

			Assert.IsNull(dialogWatcher.LastException, "LastException should be null");

			// Call HandleDialog
			dialogWatcher.HandleWindow(dialog);

			Assert.IsNotNull(dialogWatcher.LastException, "LastException should not be null");

			mocks.VerifyAll();
		}

		[Test]
		public void StartingDialogWatcherShouldAdhereToSetting()
		{
			IE.Settings.Reset();

			Assert.That(IE.Settings.AutoStartDialogWatcher, "Unexpected value for AutoStartDialogWatcher");

			IE.Settings.AutoStartDialogWatcher = false;
			using (IE ie = new IE())
			{
				Assert.That(ie.DialogWatcher, NUnit.Framework.SyntaxHelpers.Is.Null);
			}
		}
	}
}