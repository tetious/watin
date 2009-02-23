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

using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Native;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
    [TestFixture]
    public class WinButtonTests
    {
        [Test]
        public void ExistShouldReturnFalseIfWindowIsNotAButton()
        {
            // GIVEN
            var hwndMock = new Mock<IHwnd>();

            hwndMock.Expect(hwnd => hwnd.IsWindow).Returns(true);
            hwndMock.Expect(hwnd => hwnd.ClassName).Returns("NoButton");

            // WHEN
            var button = new WinButton(hwndMock.Object);
            
            // THEN
            Assert.That(button.Exists(), Is.False);
            hwndMock.VerifyAll();
        }

        [Test]
        public void ExistShouldReturnTrueIfWindowIsAButton()
        {
            // GIVEN
            var hwndMock = new Mock<IHwnd>();

            hwndMock.Expect(hwnd => hwnd.IsWindow).Returns(true);
            hwndMock.Expect(hwnd => hwnd.ClassName).Returns("Button");

            // WHEN
            var button = new WinButton(hwndMock.Object);

            // THEN
            Assert.That(button.Exists(), Is.True);
            hwndMock.VerifyAll();
        }
    }
}
