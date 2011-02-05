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

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class IdHinterTests
    {
        [Test]
        public void ShouldBeAbleToFindElementByIdWhenComparerIsStringComparer()
        {
            // GIVEN
            var byID = Find.ById("myId");
            Assert.That(byID.Comparer.GetType(), Is.EqualTo(typeof(Comparers.StringComparer)), "pre-condition failed");

            // WHEN
            var hinter = IdHinter.GetIdHint(byID);

            // THEN
            Assert.That(hinter, Is.EqualTo("myId"));
        }

        [Test]
        public void ShouldBeNotAbleToFindElementByIdWhenComparerIsSubClassOfStringComparer()
        {
            // GIVEN
            var endsWithComparer = new EndsWithComparer("myId");
            var byID = Find.ById(endsWithComparer);
            
            // WHEN
            var hinter = IdHinter.GetIdHint(byID);

            // THEN
            Assert.That(hinter, Is.Null);
        }

        private class EndsWithComparer : Comparers.StringComparer
        {
            public EndsWithComparer(string comparisonValue) : base(comparisonValue) { }

            public override bool Compare(string value)
            {
                return value.EndsWith(ComparisonValue);
            }
        }
    }

    [TestFixture]
    public class QuerySelectorHinterTests
    {
        [Test]
        public void Should_return_selector_string_for_query_selector_constraint()
        {
            // GIVEN
            var hinter = new QuerySelectorHinter(new QuerySelectorConstraint("#ID"));
            
            // WHEN
            var selector = hinter.GetSelector();

            // THEN
            Assert.That(selector, Is.EqualTo("#ID"));
        }

        [Test]
        public void Should_return_null_when_constraint_is_not_a_query_selector_constraint()
        {
            // GIVEN
            var hinter = new QuerySelectorHinter(Find.ById("ID"));
            
            // WHEN
            var selector = hinter.GetSelector();

            // THEN
            Assert.That(selector, Is.Null);
        }

        [Test]
        public void Should_return_selector_when_constraint_is_an_and_constraint_with_a_query_selector_constraint()
        {
            // GIVEN
            var constraint = Find.Any.And(new QuerySelectorConstraint("#ID"));
            var hinter = new QuerySelectorHinter(constraint);
            
            // WHEN
            var selector = hinter.GetSelector();

            // THEN
            Assert.That(selector, Is.EqualTo("#ID"));
        }
    }
}
