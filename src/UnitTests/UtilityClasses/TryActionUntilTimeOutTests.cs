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
    public class TryActionUntilTimeOutTests
    {
        [Test]
        public void ShouldNotAllowNullAsAction()
        {
            // Given
            var timeout = new TryActionUntilTimeOut(30);
            
            // When
            try
            {
                timeout.Try(null);
            }
            catch (Exception e)
            {
                Assert.That(e is ArgumentNullException, Is.True, "Action should be required");
                Assert.That(e.Message, Text.Contains("action"), "Expected for argument 'action'");
                return;
            }
            Assert.Fail("Expected an ArgumentNullException");
        }

        [Test]
        public void ShouldCallTheAction()
        {
            // GIVEN
            var actionCalled = false;
            var timeOut = new TryActionUntilTimeOut(2);
            
            // WHEN
            timeOut.Try(() => { actionCalled = true; return true; });


            // THEN
            Assert.That(actionCalled, Is.True, "action not called");
        }

        [Test]
        public void TryShouldReturnTrueIfNoTimeOut()
        {
            // GIVEN
            var timeOut = new TryActionUntilTimeOut(1);
            
            // WHEN
            var result = timeOut.Try(() => true );

            // THEN
            Assert.That(result, Is.True);
        }

        [Test]
        public void TryShouldReturnFalseIfDidTimeOut()
        {
            // GIVEN
            var timeOut = new TryActionUntilTimeOut(1);
            
            // WHEN
            var result = timeOut.Try(() => false );

            // THEN
            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldTimeOutifActionDidNotReturnSucces()
        {
            // GIVEN
            var timeOut = new TryActionUntilTimeOut(1);
            
            // WHEN
            timeOut.Try( () => false );

            // THEN
            Assert.That(timeOut.DidTimeOut, Is.True, "Expected timout");
        }

        [Test]
        public void ShouldCreateTimeOutException()
        {
            // GIVEN
            var timeoutsec = 1;
            var timeOut = new TryActionUntilTimeOut(timeoutsec)
                              {
                                  ExceptionMessage = () => string.Format("returning false for {0} seconds", timeoutsec)
                              };
            
            // WHEN
            try
            {
                timeOut.Try(() => false );
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
            var timeOut = new TryActionUntilTimeOut(1)
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
            var timeOut = new TryActionUntilTimeOut(1);

            // THEN
            Assert.That(timeOut.SleepTime, Is.EqualTo(123), "Unexpected default timeout");
        }
    }
}