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
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.DialogHandlers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
	[TestFixture]
	public class VbScriptMsgBoxDialogHandlerTests : BaseWithBrowserTests
	{
		[Test]
		public void TestOkOnly()
		{
			const int buttons = 0;
			string result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.OK);
			Assert.That(result, Is.EqualTo("1"), "Unexpected return value from message box");
		}

		[Test]
		public void TestOkCancel()
		{
			const int buttons = 1;
			string result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.OK);
			Assert.That(result, Is.EqualTo("1"), "Unexpected return value from message box");
			
			result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.Cancel);
			Assert.That(result, Is.EqualTo("2"), "Unexpected return value from message box");
		}

		[Test]
		public void TestAbortRetryIgnore()
		{
			const int buttons = 2;
			string result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.Abort);
			Assert.That(result, Is.EqualTo("3"), "Unexpected return value from message box");
			
			result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.Retry);
			Assert.That(result, Is.EqualTo("4"), "Unexpected return value from message box");
			
			result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.Ignore);
			Assert.That(result, Is.EqualTo("5"), "Unexpected return value from message box");
		}

		[Test]
		public void TestYesNoCancel ()
		{
			const int buttons = 3;
			string result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.Yes);
			Assert.That(result, Is.EqualTo("6"), "Unexpected return value from message box");
			
			result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.No);
			Assert.That(result, Is.EqualTo("7"), "Unexpected return value from message box");
			
			result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.Cancel);
			Assert.That(result, Is.EqualTo("2"), "Unexpected return value from message box");
		}
		[Test]
		public void TestYesNo ()
		{
			const int buttons = 4;
			string result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.Yes);
			Assert.That(result, Is.EqualTo("6"), "Unexpected return value from message box");
			
			result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.No);
			Assert.That(result, Is.EqualTo("7"), "Unexpected return value from message box");
		}

		[Test]
		public void TestRetryCancel ()
		{
			const int buttons = 5;
			string result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.Retry);
			Assert.That(result, Is.EqualTo("4"), "Unexpected return value from message box");
			
			result = GetResultFromMsgBox(buttons, VbScriptMsgBoxDialogHandler.Button.Cancel);
			Assert.That(result, Is.EqualTo("2"), "Unexpected return value from message box");
		}

		private string GetResultFromMsgBox(int buttons, VbScriptMsgBoxDialogHandler.Button buttonToPush) 
		{
			Ie.TextField("msgBoxButtons").TypeText(buttons.ToString());
			var handler = new VbScriptMsgBoxDialogHandler(buttonToPush);
			using(new UseDialogOnce(Ie.DialogWatcher, handler ))
			{
				Ie.Button("vbScriptMsgBox").ClickNoWait();
				handler.WaitUntilHandled(10);

                Assert.That(handler.HasHandledDialog, "Should have handled dialog");
				return Ie.TextField("msgBoxReturnValue").Value;
			}
		}

	    public override Uri TestPageUri
		{
			get { return TestEventsURI; }
		}
	}
}