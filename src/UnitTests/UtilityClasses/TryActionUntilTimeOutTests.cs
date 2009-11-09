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
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.UnitTests.UtilityClasses
{
    /// <summary>
    ///This is a NUnit TestFixture class
    ///</summary>
    [TestFixture]
    public class TryFuncUntilTimeOutTests
    {
        [Test]
        public void ShouldNotAllowNullAsAction()
        {
            // Given
            var timeout = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(30));
            
            // When
            try
            {
                timeout.Try<object>(null);
            }
            catch (Exception e)
            {
                Assert.That(e is ArgumentNullException, Is.True, "Action should be required");
                Assert.That(e.Message, Text.Contains("func"), "Expected for argument 'func'");
                return;
            }
            Assert.Fail("Expected an ArgumentNullException");
        }

        [Test]
        public void ShouldCallTheAction()
        {
            // GIVEN
            var actionCalled = false;
            var timeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(2));
            
            // WHEN
            timeOut.Try(() => { actionCalled = true; return true; });


            // THEN
            Assert.That(actionCalled, Is.True, "action not called");
        }

        [Test]
        public void TryShouldReturnTrueIfNoTimeOut()
        {
            // GIVEN
            var timeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(1));
            
            // WHEN
            var result = timeOut.Try(() => true );

            // THEN
            Assert.That(result, Is.True);
        }

        [Test]
        public void TryShouldReturnFalseIfDidTimeOut()
        {
            // GIVEN
            var timeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(1));
            
            // WHEN
            var result = timeOut.Try(() => false );

            // THEN
            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldTimeOutifActionDidNotReturnSucces()
        {
            // GIVEN
            var timeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(1));
            
            // WHEN
            timeOut.Try(() => false );

            // THEN
            Assert.That(timeOut.DidTimeOut, Is.True, "Expected timout");
        }

        [Test]
        public void ShouldCreateTimeOutException()
        {
            // GIVEN
            var timeoutsec = 1;
            var timeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(timeoutsec))
                              {
                                  ExceptionMessage = () => string.Format("returning false for {0} seconds", timeoutsec)
                              };
            
            // WHEN
            try
            {
                timeOut.Try(() => false);
            }
            catch(Exception e)
            {
                // THEN
                Assert.That(e is Exceptions.TimeoutException, "Unexpected exception type: " + e.GetType());
                Assert.That(e.Message, Is.EqualTo("Timeout while returning false for 1 seconds"), "Unexpected exception message");
                Assert.That(e.InnerException, Is. Null, "Expected no InnerException");
                return;
            }

            Assert.Fail("Expected TimeOutException");
        }

        [Test]
        public void ShouldSetInnerExcpetionWithLastException()
        {
            // GIVEN
            var timeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(1))
                              {
                                  ExceptionMessage = () => "throwing exceptions"
                              };
            
            // WHEN
            try
            {
                timeOut.Try(() =>
                    {
                        var zero = 0;
                        return 1 / zero == 0;
                    });
            }
            catch(Exception e)
            {
                // THEN
                Assert.That(e.InnerException, Is.Not.Null, "Expected an innerexception");
                Assert.That(e.InnerException.GetType(), Is.EqualTo(typeof(DivideByZeroException)), "Expected DivideByZeroException");
                return;
            }

            Assert.Fail("Expected TimeOutException");
        }


        [Test]
        public void SleepTimeShouldDefaultToSettingsSleepTime()
        {
            // GIVEN
            Settings.SleepTime = 123;

            // WHEN
            var timeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(1));

            // THEN
            Assert.That(timeOut.SleepTime.TotalMilliseconds, Is.EqualTo(123), "Unexpected default timeout");
        }
    }
}