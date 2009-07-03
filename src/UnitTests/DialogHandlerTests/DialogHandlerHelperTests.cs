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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Interfaces;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
    [TestFixture]
    public class DialogHandlerHelperTests
    {
        [Test]
        public void ShouldNotBeAbleToHandleNullWindow()
        {
            // GIVEN
            
            var handlerHelper = new DialogHandlerHelper();

            // WHEN
            var canBeHandled = handlerHelper.CanHandleDialog(null);

            // THEN
            Assert.That(canBeHandled, Is.False, "MESSAGE");
        }

        [Test]
        public void ShouldMentionDialogHandlerAsCandidate()
        {
            // GIVEN
            var window = new Window(new IntPtr(123));
            var mainWindowHwnd = new IntPtr(456);
            
            var dialogHandlerMock = new Mock<IDialogHandler>();
            dialogHandlerMock.Expect(dialog => dialog.CanHandleDialog(window, mainWindowHwnd)).Returns(true);
            
            var dialogHandlerHelper = new DialogHandlerHelper();
            Assert.That(dialogHandlerHelper.CandidateDialogHandlers.Count, Is.EqualTo(0), "Pr-condition: expected no candidates");
            
            // WHEN
            dialogHandlerHelper.HandlePossibleCandidate(dialogHandlerMock.Object, window, mainWindowHwnd);

            // THEN
            Assert.That(dialogHandlerHelper.CandidateDialogHandlers.Count, Is.EqualTo(1));
            dialogHandlerMock.VerifyAll();
        }

        [Test]
        public void GetDialogHandlersShouldIncludeAllDialogHandlers()
        {
            // GIVEN
            var dialogHandlers = DialogHandlerHelper.GetDialogHandlers();
            var dialogHandlersList = new List<IDialogHandler>(dialogHandlers);

            // WHEN
            var count = dialogHandlersList.Count;

            // THEN
            Assert.That(count, Is.EqualTo(15), "Unexpected number of concreet dialog handlers");
        }

        [Test]
        public void ShouldMentionPromptDialogAsCandidate()
        {
            // GIVEN
            var mainWindowHwnd = new IntPtr(123);

            var topLevelWindow = new Window(mainWindowHwnd);

            var windowMock = new Mock<Window>(new IntPtr(456));
            windowMock.Expect(window => window.ToplevelWindow).Returns(topLevelWindow);
            windowMock.Expect(window => window.StyleInHex).Returns("94C800C4");

            var helper = new DialogHandlerHelper();

            // WHEN
            helper.CanHandleDialog(windowMock.Object, mainWindowHwnd);

            // THEN
            Assert.That(helper.CandidateDialogHandlers.Count, Is.EqualTo(1), "Unexpected number of candidate dialog handlers");
            Assert.That(helper.CandidateDialogHandlers[0], Is.EqualTo(typeof(PromptDialogHandler).FullName), "Unexpected candidate dialog handler");
        }

    }
}
