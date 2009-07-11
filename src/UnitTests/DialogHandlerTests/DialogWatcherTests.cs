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

using NUnit.Framework;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class DialogWatcherTests : BaseWatiNTest
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
			Settings.Reset();
			Logger.LogWriter = saveLoggerInstance;
		}

//		[Test]
//		public void DialogWatcherShouldKeepRunningWhenClosingOneOfTwoInstancesInSameProcess()
//		{
//			using (IE ie1 = new IE(), ie2 = new IE())
//			{
//				Assert.AreEqual(ie1.ProcessID, ie2.ProcessID, "Processids should be the same");
//
//				var dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie1.ProcessID);
//
//				Assert.IsNotNull(dialogWatcher, "dialogWatcher should not be null");
//				Assert.AreEqual(ie1.ProcessID, dialogWatcher.MainWindowHwnd, "Processids of ie1 and dialogWatcher should be the same");
//				Assert.AreEqual(2, dialogWatcher.ReferenceCount, "Expected 2 as reference count");
//				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist");
//				Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should be running");
//
//				ie2.Close();
//
//				Assert.AreEqual(1, dialogWatcher.ReferenceCount, "Expected 1 as reference count");
//				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should still exist");
//				Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should still be running");
//
//				ie1.Close();
//
//				Assert.AreEqual(0, dialogWatcher.ReferenceCount, "Expected 0 as reference count");
//				Assert.IsFalse(dialogWatcher.IsRunning, "dialogWatcher should not be running");
//			}
//		}

//		[Test]
//		public void DialogWatcherShouldTerminateWhenNoWatiNCoreIEInstancesExistButProcessDoesExist()
//		{
//			int ieProcessId;
//
//			// Create running Internet Explorer instance but no longer referenced by 
//			// an instance of WatiN.Core.IE.
//			using (var ie = new IE(MainURI))
//			{
//				ie.AutoClose = false;
//				ieProcessId = ie.ProcessID;
//			}
//
//			// Create IE instances and see if DialogWatcher behaves as expected
//			using (var ie1 = new IE())
//			{
//				Assert.AreEqual(ieProcessId, ie1.ProcessID, "ProcessIds ie and ie1 should be the same");
//
//				using (var ie2 = new IE())
//				{
//					Assert.AreEqual(ie1.ProcessID, ie2.ProcessID, "Processids ie1 and ie2 should be the same");
//				}
//
//				var dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie1.ProcessID);
//
//				Assert.IsNotNull(dialogWatcher, "dialogWatcher should not be null");
//				Assert.AreEqual(ie1.ProcessID, dialogWatcher.MainWindowHwnd, "Processids of ie1 and dialogWatcher should be the same");
//				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist");
//				Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should be running");
//
//				ie1.Close();
//
//				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist after ie1.close");
//				Assert.IsFalse(dialogWatcher.IsRunning, "dialogWatcher should not be running");
//			}
//
//			// Find created but not referenced Internet Explorer instance and close it.
//			IE.AttachToIE(Find.ByUrl(MainURI)).Close();
//		}

		[Test]
		public void DialogWatcherOfIEAndHTMLDialogShouldNotBeNull()
		{
			using (var ie = new IE(MainURI))
			{
				Assert.IsNotNull(ie.DialogWatcher, "ie.DialogWatcher should not be null");

				ie.Button("modalid").ClickNoWait();

				using (var htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest")))
				{
					Assert.IsNotNull(htmlDialog.DialogWatcher, "htmlDialog.DialogWatcher should not be null");
				}
			}
		}

//		[Test]
//		public void DialogWatcherShouldKeepRunningWhenClosingHTMLDialog()
//		{
//			using (var ie = new IE(MainURI))
//			{
//				ie.Button("modalid").ClickNoWait();
//
//				DialogWatcher dialogWatcher;
//				using (var htmlDialog = ie.HtmlDialog(Find.ByTitle("PopUpTest")))
//				{
//					Assert.AreEqual(ie.ProcessID, htmlDialog.ProcessID, "Processids should be the same");
//
//					dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie.ProcessID);
//					Assert.IsNotNull(dialogWatcher, "dialogWatcher should not be null");
//					Assert.AreEqual(ie.ProcessID, dialogWatcher.MainWindowHwnd, "Processids of ie and dialogWatcher should be the same");
//					Assert.AreEqual(2, dialogWatcher.ReferenceCount, "Expected 2 as reference count");
//					Assert.IsTrue(dialogWatcher.ProcessExists, "Process should exist");
//					Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should be running");
//				}
//
//				Assert.AreEqual(1, dialogWatcher.ReferenceCount, "Expected 1 as reference count");
//				Assert.IsTrue(dialogWatcher.ProcessExists, "Process should still exist");
//				Assert.IsTrue(dialogWatcher.IsRunning, "dialogWatcher should still be running");
//
//				ie.WaitForComplete();
//				ie.Close();
//
//				Assert.AreEqual(0, dialogWatcher.ReferenceCount, "Expected 0 as reference count");
//				Assert.IsFalse(dialogWatcher.IsRunning, "dialogWatcher should not be running");
//			}
//		}

//		[Test]
//		public void ThrowReferenceCountException()
//		{
//			using (var ie = new IE())
//			{
//				var dialogWatcher = DialogWatcher.GetDialogWatcherFromCache(ie.ProcessID);
//				Assert.AreEqual(1, dialogWatcher.ReferenceCount);
//
//				dialogWatcher.DecreaseReferenceCount();
//
//				Assert.AreEqual(0, dialogWatcher.ReferenceCount);
//
//				try
//				{
//					dialogWatcher.DecreaseReferenceCount();
//					Assert.Fail("ReferenceCountException expected");
//				}
//				catch (ReferenceCountException) {}
//				catch
//				{
//					Assert.Fail("ReferenceCountException expected");
//				}
//				finally
//				{
//					dialogWatcher.IncreaseReferenceCount();
//				}
//			}
//		}

//		[Test]
//		public void ExceptionsInDialogHandlersShouldBeLoggedAndNeglected()
//		{
//			//Make the mocks
//			var mockLogWriter = new Mock<ILogWriter>();
//			var mockBuggyDialogHandler = new Mock<IDialogHandler>();
//			var mockNextDialogHandler = new Mock<IDialogHandler>();
//			var mockDialog = new Mock<Window>(IntPtr.Zero);
//
//			// Handle window does check if window IsDialog and Visible
//			mockDialog.Expect(dialog => dialog.IsDialog()).Returns(true);
//			mockDialog.Expect(dialog => dialog.Visible).Returns(true);
//
//			// If this HandleDialog is called throw an exception
//			mockBuggyDialogHandler.Expect(buggyHandler => buggyHandler.HandleDialog(mockDialog.Object)).Throws(new Exception());
//
//            // Expect Logger will be called with the exception text and stack trace
//            var expectedMessage = "Exception was thrown while DialogWatcher called HandleDialog:";
//            mockLogWriter.Expect(writer => writer.LogAction(It.IsRegex(expectedMessage)));
//
//            var exceptionMessage = "^System.Exception:.*";
//            mockLogWriter.Expect(writer => writer.LogAction(It.IsRegex(exceptionMessage)));
//			
//            // Expect the next dialogHandler will be called even do an exception
//			// has been thrown by the previous handler
//			mockNextDialogHandler.Expect(nextHandler => nextHandler.HandleDialog(mockDialog.Object)).Returns(true);
//
//			// Set Logger
//			Logger.LogWriter = mockLogWriter.Object;
//
//			// Add dialogHandlers
//			var dialogWatcher = new DialogWatcher(0);
//			dialogWatcher.Add(mockBuggyDialogHandler.Object);
//			dialogWatcher.Add(mockNextDialogHandler.Object);
//
//			Assert.IsNull(dialogWatcher.LastException, "LastException should be null");
//
//			// Call HandleDialog
//			dialogWatcher.HandleWindow(mockDialog.Object);
//
//			Assert.IsNotNull(dialogWatcher.LastException, "LastException should not be null");
//            mockLogWriter.VerifyAll();
//		}

		[Test]
		public void StartingDialogWatcherShouldAdhereToSetting()
		{
			Settings.Reset();

			Assert.That(Settings.AutoStartDialogWatcher, "Unexpected value for AutoStartDialogWatcher");

			Settings.AutoStartDialogWatcher = false;
			using (var ie = new IE())
			{
				Assert.That(ie.DialogWatcher, NUnit.Framework.SyntaxHelpers.Is.Null);
			}
		}
	}
}