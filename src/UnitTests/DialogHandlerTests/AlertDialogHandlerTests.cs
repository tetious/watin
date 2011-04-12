#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.DialogHandlers;
using WatiN.Core.UnitTests.TestUtils;
using WatiN.Core.UtilityClasses;

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

	    [Test, Ignore("This feature can't be supported on IE8 because there is no way to detect the parent ie/window for a dialog.")]
		public void CloseSpecificBrowserAlert()
		{
			Assert.AreEqual(0, Ie.DialogWatcher.Count, "DialogWatcher count should be zero");

		    var firstIE = Ie;
            firstIE.RunScript("document.title = 'firstIe'; ");

            using (var secondIe = new IE(TestPageUri,true))
            {
                secondIe.RunScript("document.title = 'secondIe'; ");
                // set up a second browser with an open dialog
                var secondAlertDialogHandler = new AlertDialogHandler();
                using (new UseDialogOnce(secondIe.DialogWatcher, secondAlertDialogHandler))
                {
                    secondIe.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

                    secondAlertDialogHandler.WaitUntilExists(5);

                    // close the original message
                    var firstAlertDialogHandler = new AlertDialogHandler();
                    using (new UseDialogOnce(firstIE.DialogWatcher, firstAlertDialogHandler))
                    {
                        firstIE.Button(Find.ByValue("Show alert dialog")).ClickNoWait();

                        firstAlertDialogHandler.WaitUntilExists(5);

                        var message = firstAlertDialogHandler.Message;
                        firstAlertDialogHandler.OKButton.Click();

                        firstIE.WaitForComplete(5);

                        Assert.IsTrue(secondAlertDialogHandler.Exists(), "Original Alert Dialog should be open.");

                        Assert.AreEqual("This is an alert!", message, "Unexpected message");
                        Assert.IsFalse(firstAlertDialogHandler.Exists(), "Alert Dialog should be closed.");
                    }

                    // close the second message
                    secondAlertDialogHandler.OKButton.Click();

                    secondIe.WaitForComplete(5);

                    Assert.IsFalse(secondAlertDialogHandler.Exists(), "Alert Dialog should be closed.");
                }
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

        [Test]
        public void Should_handle_alert_dialog_browser_agnostic()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                browser.DialogWatcher.CloseUnhandledDialogs = false;

                var dialogHandler = new JSAlertDialogHandler(browser);
                dialogHandler.InjectStub();

                // WHEN
                browser.Button(Find.ByValue("Show alert dialog")).Click();
                var message = dialogHandler.Message;

                dialogHandler.RevertStub();

                // THEN
                Assert.That(message, Is.EqualTo("This is an alert!"));
            });
        }
        
        [Test]
        public void Should_handle_alert_dialog_browser_agnostic2()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                browser.DialogWatcher.CloseUnhandledDialogs = false;

                var dialogHandler = new JSAlertDialogHandler2(browser, () => browser.Button(Find.ByValue("Show alert dialog")).Click());

                // WHEN
                dialogHandler.DoAction();

                // THEN
                Assert.That(dialogHandler.Message, Is.EqualTo("This is an alert!"));
            });
        }


		public override Uri TestPageUri
		{
			get { return TestEventsURI; }
		}
	}



    public class JSAlertDialogHandler2:JSAlertDialogHandler
    {
        private readonly Action _actionToInvokeShowAlertDialog;
        private string _message;

        public JSAlertDialogHandler2(Document document, Action actionToInvokeShowAlertDialog) : base(document)
        {
            _actionToInvokeShowAlertDialog = actionToInvokeShowAlertDialog;
        }

        public void DoAction()
        {
            InjectStub();
            _actionToInvokeShowAlertDialog.Invoke();
            _message = base.Message;
            RevertStub();
        }        

        public override string Message
        {
            get { return _message; }
        }
    }
    public class JSAlertDialogHandler
    {
        private static VariableNameHelper VariableNameHelper= new VariableNameHelper("watinalertdialog");

        private readonly Document _document;
        private string _orgAlertFunction;

        public JSAlertDialogHandler(Document document)
        {
            _document = document;
            _orgAlertFunction = VariableNameHelper.CreateVariableName();
        }

        public virtual string Message
        {
            get { return _document.Eval("window._watinalertmessage"); }
        }

        public void InjectStub()
        {
            var code = _orgAlertFunction + " = window.alert; window.alert = function(message){ window._watinalertmessage = message; return true; }";
            _document.RunScript(code);
        }

        public void RevertStub()
        {
            var code = "window.alert = " + _orgAlertFunction +"; delete " + _orgAlertFunction + ";";
            _document.RunScript(code);
        }
    }
}