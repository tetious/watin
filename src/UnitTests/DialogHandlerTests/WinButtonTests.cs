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
