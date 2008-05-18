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
using Rhino.Mocks;
using WatiN.Core.DialogHandlers;
using Iz = NUnit.Framework.SyntaxHelpers.Is;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
    [TestFixture]
    public class WinButtonTests
    {
        [Test]
        public void ExistShouldReturnFalseIfWindowIsNotAButton()
        {
            MockRepository mocks = new MockRepository();

            IHwnd hwnd = (IHwnd) mocks.CreateMock(typeof (IHwnd));
            
            Expect.Call(hwnd.IsWindow).Return(true);
            Expect.Call(hwnd.ClassName).Return("NoButton");

            mocks.ReplayAll();

            WinButton button = new WinButton(hwnd);
            
            Assert.That(button.Exists(), Iz.False);

            mocks.VerifyAll();
        }

        [Test]
        public void ExistShouldReturnTrueIfWindowIsAButton()
        {
            MockRepository mocks = new MockRepository();

            IHwnd hwnd = (IHwnd) mocks.CreateMock(typeof (IHwnd));
            
            Expect.Call(hwnd.IsWindow).Return(true);
            Expect.Call(hwnd.ClassName).Return("Button");

            mocks.ReplayAll();

            WinButton button = new WinButton(hwnd);
            
            Assert.That(button.Exists(), Iz.True);

            mocks.VerifyAll();
        }
    }
}
