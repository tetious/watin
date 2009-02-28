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
using WatiN.Core.Exceptions;

namespace WatiN.Core.UnitTests
{
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Comparers;

    [TestFixture]
    public class PredicateComparerTests : BaseWithBrowserTests
    {
        private bool _called;
        private string _value;
        private bool _returnValue;

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        [SetUp]
        public void SetUp()
        {
            _called = false;
            _value = null;
        }

        [Test]
        public void StringPredicateShouldBeCalledAndReturnTrue()
        {
            _returnValue = true;
            PredicateComparer<string, string> comparer = new PredicateComparer<string, string>(CallThisMethod);

            Assert.That(comparer.Compare("test value"), Is.True);
            Assert.That(_called, Is.True);
            Assert.That(_value, Is.EqualTo("test value"));
        }

        [Test]
        public void StringPredicateShouldBeCalledAndReturnFalse()
        {
            _returnValue = false;
            PredicateComparer<string, string> comparer = new PredicateComparer<string, string>(CallThisMethod);

            Assert.That(comparer.Compare("some input"), Is.False);
            Assert.That(_called, Is.True);
            Assert.That(_value, Is.EqualTo("some input"));
        }

        public bool CallThisMethod(string value)
        {
            _called = true;
            _value = value;
            return _returnValue;
        }

        public bool CallElementCompareMethod(Element element)
        {
            _called = true;
            return _returnValue;
        }

        [Test, ExpectedException(typeof(WatiNException), ExpectedMessage = "Exception occurred during execution of predicate for 'Button with no Id'")]
        public void IfExceptionDuringExecutionOfPredicateItShouldBeClearThatThisAProblemInThePredicate()
        {
            var button = Ie.Button(b =>
            {
                if (b.Value == "Button with no Id")
                    throw new Exception("boo");
                else
                    return false;
            });
            
            //trigger search of the button
            var exists = button.Exists;
        }

        [Test]
        public void ToStringShouldDescribeTheCondition()
        {
            var comparer = new PredicateComparer<string, string>(x => false);
            Assert.AreEqual("satisfies predicate", comparer.ToString());
        }
    }
}
