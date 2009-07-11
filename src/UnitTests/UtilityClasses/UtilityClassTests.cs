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
using System.Threading;
using NUnit.Framework;
using WatiN.Core.UnitTests.TestUtils;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.UnitTests.UtilityClasses
{
    [TestFixture]
    public class UtilityClassTests : BaseWithBrowserTests
    {
        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        [Test]
        public void IsNullOrEmpty()
        {
            Assert.IsTrue(UtilityClass.IsNullOrEmpty(null), "null should return true");
            Assert.IsTrue(UtilityClass.IsNullOrEmpty(String.Empty), "Empty should return true");
            Assert.IsTrue(UtilityClass.IsNullOrEmpty(""), "zero length string should return true");
            Assert.IsFalse(UtilityClass.IsNullOrEmpty("test"), "string 'test' should return false");
        }

        [Test]
        public void IsNotNullOrEmpty()
        {
            Assert.IsFalse(UtilityClass.IsNotNullOrEmpty(null), "null should return false");
            Assert.IsFalse(UtilityClass.IsNotNullOrEmpty(String.Empty), "Empty should return false");
            Assert.IsFalse(UtilityClass.IsNotNullOrEmpty(""), "zero length string should return false");
            Assert.IsTrue(UtilityClass.IsNotNullOrEmpty("test"), "string 'test' should return true");
        }


        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void SimpleTimerWithNegativeTimeoutNotAllowed()
        {
            new SimpleTimer(TimeSpan.FromSeconds(-1));
        }

        [Test]
        public void SimpleTimerWithZeroTimoutIsAllowed()
        {
            var timer = new SimpleTimer(TimeSpan.FromSeconds(0));
            Assert.IsTrue(timer.Elapsed);
        }

        [Test]
        public void SimpleTimerOneSecond()
        {
            var timer = new SimpleTimer(TimeSpan.FromSeconds(1));
            Thread.Sleep(1200);
            Assert.IsTrue(timer.Elapsed);
        }

        [Test]
        public void SimpleTimerThreeSeconds()
        {
            var timer = new SimpleTimer(TimeSpan.FromSeconds(3));
            Thread.Sleep(2500);
            Assert.IsFalse(timer.Elapsed);
            Thread.Sleep(1000);
            Assert.IsTrue(timer.Elapsed);
        }

        [Test]
        public void ShouldEscapeSendKeysCharacters()
        {
            var original = @"C:\TAdev\~%^+{}[]()Test\Doc.txt";
            var expected = @"C:\TAdev\{~}{%}{^}{+}{{}{}}{[}{]}{(}{)}Test\Doc.txt";

            Assert.AreEqual(expected, UtilityClass.EscapeSendKeysCharacters(original));
        }

        [Test]
        public void ShouldNotEscapeCharacters()
        {
            var original = "just a test";

            Assert.AreEqual(original, UtilityClass.EscapeSendKeysCharacters(original));
        }
    }
}