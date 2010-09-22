using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SHDocVw;
using WatiN.Core.Native.InternetExplorer;

namespace WatiN.Core.UnitTests.Native.IETests
{
    [TestFixture]
    public class IEWaitForCompleteTests
    {
        [Test]
        public void Should_proceed_when_ready_state_is_interactive()
        {
            //GIVEN
            var complete = new MyIEWaitforComplete();
            
            //WHEN
            var result = complete.call_IsStateInteractiveOrComplete(tagREADYSTATE.READYSTATE_INTERACTIVE);

            //THEN
            Assert.That(result, Is.True);
        }
        [Test]
        public void Should_proceed_when_ready_state_is_complete()
        {
            //GIVEN
            var complete = new MyIEWaitforComplete();
            
            //WHEN
            var result = complete.call_IsStateInteractiveOrComplete(tagREADYSTATE.READYSTATE_COMPLETE);

            //THEN
            Assert.That(result, Is.True);
        }

        [Test]
        public void Should_call_IsStateInteractiveOrComplete_from_WaitWhileIEReadyStateNotComplete()
        {
            //GIVEN
            var webBrowser = new Mock<IWebBrowser2>();
            webBrowser.Expect(browser => browser.ReadyState).Returns(tagREADYSTATE.READYSTATE_COMPLETE);

            var complete = new MyIEWaitforComplete();

            //WHEN
            var result = complete.call_WaitWhileIEReadyStateNotComplete(webBrowser.Object);

            //THEN
            Assert.That(result, Is.True, "Expected true");
            Assert.That(complete.IsStateInteractiveOrCompleteCallCount, Is.EqualTo(1), "should call IsStateInteractiveOrComplete");
        }

        private class MyIEWaitforComplete : IEWaitForComplete
        {
            public int IsStateInteractiveOrCompleteCallCount { get; set; }

            public MyIEWaitforComplete()
                : base((IEBrowser)null)
            {
                
            }
            public bool call_WaitWhileIEReadyStateNotComplete(IWebBrowser2 webBrowser2)
            {
                return WaitWhileIEReadyStateNotComplete(webBrowser2);
            }

            public bool call_IsStateInteractiveOrComplete(tagREADYSTATE readystate)
            {
                return IsStateInteractiveOrComplete(readystate);
            }

            protected override bool IsStateInteractiveOrComplete(tagREADYSTATE readystate)
            {
                IsStateInteractiveOrCompleteCallCount += 1;
                return base.IsStateInteractiveOrComplete(readystate);
            }

        }


    }
}
