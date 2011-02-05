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

using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SHDocVw;
using WatiN.Core.Interfaces;
using WatiN.Core.Native.InternetExplorer;

namespace WatiN.Core.UnitTests.Native.IETests
{
    [TestFixture]
    public class IEWaitForCompleteTests
    {
        [Test]
        public void Should_use_settings_sleeptime_for_initialsleep()
        {
            //GIVEN
            var settings = new Mock<ISettings>();
            settings.Expect(s => s.SleepTime).Returns(10);
            Settings.Instance = settings.Object;

            var complete = new MyIEWaitforComplete();

            //WHEN
            complete.call_InitialSleep();

            //THEN
            settings.VerifyAll();
        }

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

            public void call_InitialSleep()
            {
                InitialSleep();
            }

            protected override bool IsStateInteractiveOrComplete(tagREADYSTATE readystate)
            {
                IsStateInteractiveOrCompleteCallCount += 1;
                return base.IsStateInteractiveOrComplete(readystate);
            }

        }


    }
}
