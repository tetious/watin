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
	public class ConfirmDialogHandlerTests : BaseWithBrowserTests
	{
		[Test]
		public void ConfirmDialogHandlerOK()
		{
			Assert.AreEqual(0, Ie.DialogWatcher.Count, "DialogWatcher count should be zero");

			var confirmDialogHandler = new ConfirmDialogHandler();

			using (new UseDialogOnce(Ie.DialogWatcher, confirmDialogHandler))
			{
				Ie.Button(Find.ByValue("Show confirm dialog")).ClickNoWait();

				confirmDialogHandler.WaitUntilExists();

				var message = confirmDialogHandler.Message;
				confirmDialogHandler.OKButton.Click();

				Ie.WaitForComplete();

				Assert.AreEqual("Do you want to do xyz?", message, "Unexpected message");
				Assert.AreEqual("OK", Ie.TextField("ReportConfirmResult").Text, "OK button expected.");
			}
		}

		[Test]
		public void ConfirmDialogHandlerCancel()
		{
			Assert.AreEqual(0, Ie.DialogWatcher.Count, "DialogWatcher count should be zero");

			var confirmDialogHandler = new ConfirmDialogHandler();

			using (new UseDialogOnce(Ie.DialogWatcher, confirmDialogHandler))
			{
				Ie.Button(Find.ByValue("Show confirm dialog")).ClickNoWait();

				confirmDialogHandler.WaitUntilExists();

				string message = confirmDialogHandler.Message;
				confirmDialogHandler.CancelButton.Click();

				Ie.WaitForComplete();

				Assert.AreEqual("Do you want to do xyz?", message, "Unexpected message");
				Assert.AreEqual("Cancel", Ie.TextField("ReportConfirmResult").Text, "Cancel button expected.");
			}
		}

        [Test]
        public void Should_also_handle_dialog_when_more_then_one_browser_is_open()
        {
            var not_used_ie = new IE();
            var approveConfirmDialog = new ConfirmDialogHandler();

            using (new UseDialogOnce(Ie.DialogWatcher, approveConfirmDialog))
            {
                Ie.Button(Find.ByValue("Show confirm dialog")).ClickNoWait();
                approveConfirmDialog.WaitUntilExists(5);
                approveConfirmDialog.OKButton.Click();
            }
            Ie.WaitForComplete();
        }

        [Test]
        public void TestMultipleIE()
        {
            using (var ie = new IE(TestEventsURI))
            {
                var handler = new ConfirmDialogHandler();
                try
                {
                    ie.AddDialogHandler(handler);
                    ie.Button(Find.ByValue("Show confirm dialog")).ClickNoWait();
                    handler.WaitUntilExists(5);
                    handler.OKButton.Click();
                }
                finally
                {
                    ie.RemoveDialogHandler(handler);
                }

                using (var ie2 = new IE(TestEventsURI))
                {
                    var handler2 = new ConfirmDialogHandler();
                    try
                    {
                        ie2.AddDialogHandler(handler2);
                        ie2.Button(Find.ByValue("Show confirm dialog")).ClickNoWait();
                        handler2.WaitUntilExists(5);
                        handler2.OKButton.Click();
                    }
                    finally
                    {
                        ie2.RemoveDialogHandler(handler2);
                    }
                }
            }
        }

		public override Uri TestPageUri
		{
			get { return TestEventsURI; }
		}
	}
}