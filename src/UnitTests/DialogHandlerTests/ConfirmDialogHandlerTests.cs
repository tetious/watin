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

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
  using NUnit.Framework;
  using WatiN.Core.DialogHandlers;

  [TestFixture]
  public class ConfirmDialogHandlerTests : WatiNTest
  {
    [Test]
    public void ConfirmDialogHandlerOK()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

        ConfirmDialogHandler confirmDialogHandler = new ConfirmDialogHandler();

        using (new UseDialogOnce(ie.DialogWatcher, confirmDialogHandler))
        {
          ie.Button(Find.ByValue("Show confirm dialog")).ClickNoWait();

          confirmDialogHandler.WaitUntilExists();

          string message = confirmDialogHandler.Message;
          confirmDialogHandler.OKButton.Click();

          ie.WaitForComplete();

          Assert.AreEqual("Do you want to do xyz?", message, "Unexpected message");
          Assert.AreEqual("OK", ie.TextField("ReportConfirmResult").Text, "OK button expected.");
        }
      }
    }

    [Test]
    public void ConfirmDialogHandlerCancel()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        Assert.AreEqual(0, ie.DialogWatcher.Count, "DialogWatcher count should be zero");

        ConfirmDialogHandler confirmDialogHandler = new ConfirmDialogHandler();

        using (new UseDialogOnce(ie.DialogWatcher, confirmDialogHandler))
        {
          ie.Button(Find.ByValue("Show confirm dialog")).ClickNoWait();

          confirmDialogHandler.WaitUntilExists();

          string message = confirmDialogHandler.Message;
          confirmDialogHandler.CancelButton.Click();

          ie.WaitForComplete();

          Assert.AreEqual("Do you want to do xyz?", message, "Unexpected message");
          Assert.AreEqual("Cancel", ie.TextField("ReportConfirmResult").Text, "Cancel button expected.");
        }
      }
    }
  }
}