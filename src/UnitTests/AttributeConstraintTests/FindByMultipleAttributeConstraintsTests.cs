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
using Moq;
using NUnit.Framework;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
    [TestFixture]
    public class FindByMultipleAttributeConstraintsTests
    {
        [Test]
        public void AndTrue()
        {
            // GIVEN
            var mockAttributeBag = new Mock<IAttributeBag>();

            var findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

            mockAttributeBag.Expect(bag => bag.GetAttributeValue("name")).Returns("X");
            mockAttributeBag.Expect(bag => bag.GetAttributeValue("value")).Returns("Cancel");

            // WHEN
            var compare = findBy.Matches(mockAttributeBag.Object, new ConstraintContext());
            
            // THEN
            Assert.IsTrue(compare);
        }

        [Test]
        public void AndFalseFirstSoSecondPartShouldNotBeEvaluated()
        {
            // GIVEN
            var mockAttributeBag = new Mock<IAttributeBag>();

            var findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

            mockAttributeBag.Expect(bag => bag.GetAttributeValue("name")).Returns("Y");

            // WHEN
            var compare = findBy.Matches(mockAttributeBag.Object, new ConstraintContext());

            // THEN
            Assert.IsFalse(compare);
        }

        [Test]
        public void AndFalseSecond()
        {
            var findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

            var mockAttributeBag = new MockAttributeBag("name", "X");
            mockAttributeBag.Add("value", "OK");

            var compare = findBy.Matches(mockAttributeBag, new ConstraintContext());
            Assert.IsFalse(compare);
        }

        [Test]
        public void OrFirstTrue()
        {
            var findBy = Find.ByName("X").Or(Find.ByName("Y"));
            var mockAttributeBag = new MockAttributeBag("name", "X");
            var compare = findBy.Matches(mockAttributeBag, new ConstraintContext());
            Assert.IsTrue(compare);
        }

        [Test]
        public void OrSecondTrue()
        {
            var findBy = Find.ByName("X").Or(Find.ByName("Y"));
            var mockAttributeBag = new MockAttributeBag("name", "Y");
            ConstraintContext context = new ConstraintContext();
            Assert.IsTrue(findBy.Matches(mockAttributeBag, context));
        }

        [Test]
        public void OrFalse()
        {
            var findBy = Find.ByName("X").Or(Find.ByName("Y"));
            var mockAttributeBag = new MockAttributeBag("name", "Z");
            var compare = findBy.Matches(mockAttributeBag, new ConstraintContext());
            Assert.IsFalse(compare);
        }

        [Test]
        public void AndOr()
        {
            var findByNames = Find.ByName("X").Or(Find.ByName("Y"));
            var findBy = Find.ByValue("Cancel").And(findByNames);

            var mockAttributeBag = new MockAttributeBag("name", "X");
            mockAttributeBag.Add("value", "Cancel");
            var compare = findBy.Matches(mockAttributeBag, new ConstraintContext());
            Assert.IsTrue(compare);
        }

        [Test]
        public void AndOrThroughOperatorOverloads()
        {
            var findBy = Find.ByName("X") & Find.ByValue("Cancel") | (Find.ByName("Z") & Find.ByValue("Cancel"));

            var mockAttributeBag = new MockAttributeBag("name", "Z");
            mockAttributeBag.Add("value", "OK");
            var compare = findBy.Matches(mockAttributeBag, new ConstraintContext());
            Assert.IsFalse(compare);
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void OccurrenceShouldNotAcceptNegativeValue()
        {
            new IndexConstraint(-1);
        }

        [Test]
        public void Occurence0()
        {
            Constraint findBy = new IndexConstraint(0);

            var mockAttributeBag = new MockAttributeBag("name", "Z");

            ConstraintContext context = new ConstraintContext();
            Assert.IsTrue(findBy.Matches(mockAttributeBag, context));
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
        }

        [Test]
        public void Occurence2()
        {
            Constraint findBy = new IndexConstraint(2);

            var mockAttributeBag = new MockAttributeBag("name", "Z");

            ConstraintContext context = new ConstraintContext();
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
            Assert.IsTrue(findBy.Matches(mockAttributeBag, context));
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
        }

        [Test]
        public void OccurenceAndTrue()
        {
            var findBy = new IndexConstraint(1).And(Find.ByName("Z"));

            var mockAttributeBag = new MockAttributeBag("name", "Z");

            ConstraintContext context = new ConstraintContext();
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
            Assert.IsTrue(findBy.Matches(mockAttributeBag, context));
        }

        [Test]
        public void OccurenceOr()
        {
            var findBy = new IndexConstraint(2).Or(Find.ByName("Z"));
            ConstraintContext context = new ConstraintContext();

            var mockAttributeBag = new MockAttributeBag("name", "Z");
            Assert.IsTrue(findBy.Matches(mockAttributeBag, context));

            mockAttributeBag = new MockAttributeBag("name", "y");

            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
            Assert.IsTrue(findBy.Matches(mockAttributeBag, context));
        }

        [Test]
        public void OccurenceAndFalse()
        {
            var findBy = new IndexConstraint(1).And(Find.ByName("Y"));

            var mockAttributeBag = new MockAttributeBag("name", "Z");

            ConstraintContext context = new ConstraintContext();
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
        }

        [Test]
        public void TrueAndOccurence()
        {
            var findBy = Find.ByName("Z").And(new IndexConstraint(1));

            var mockAttributeBag = new MockAttributeBag("name", "Z");

            ConstraintContext context = new ConstraintContext();
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
            Assert.IsTrue(findBy.Matches(mockAttributeBag, context));
        }

        [Test]
        public void FalseAndOccurence()
        {
            var findBy = Find.ByName("Y").And(new IndexConstraint(1));

            var mockAttributeBag = new MockAttributeBag("name", "Z");

            ConstraintContext context = new ConstraintContext();
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
        }

        [Test]
        public void TrueAndIndexConstraintAndTrue()
        {
            var mockAttributeBag = new Mock<IAttributeBag>();

            var findBy = Find.ByName("Z").And(new IndexConstraint(1)).And(Find.ByValue("text"));
            ConstraintContext context = new ConstraintContext();

            mockAttributeBag.Expect(bag => bag.GetAttributeValue("name")).Returns("Z");

            Assert.IsFalse(findBy.Matches(mockAttributeBag.Object, context), "False because index will be 0 when evaluated.");

            mockAttributeBag.Expect(bag => bag.GetAttributeValue("name")).Returns("Y");

            Assert.IsFalse(findBy.Matches(mockAttributeBag.Object, context), "False because index will be skipped since first clause fails to match.");

            mockAttributeBag.Expect(bag => bag.GetAttributeValue("name")).Returns("Z");
            mockAttributeBag.Expect(bag => bag.GetAttributeValue("value")).Returns("text");

            Assert.IsTrue(findBy.Matches(mockAttributeBag.Object, context), "True because index will be 1 when evaluated.");

            mockAttributeBag.Expect(bag => bag.GetAttributeValue("name")).Returns("Z");

            Assert.IsFalse(findBy.Matches(mockAttributeBag.Object, context), "False because index will be 2 when evaluated.");

            mockAttributeBag.VerifyAll();
        }

        [Test]
        public void OccurenceAndOrWithOrTrue()
        {
            var findBy = new IndexConstraint(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

            var mockAttributeBag = new MockAttributeBag("name", "Z");
            ConstraintContext context = new ConstraintContext();
            Assert.IsTrue(findBy.Matches(mockAttributeBag, context));
        }

        [Test]
        public void OccurenceAndOrWithAndTrue()
        {
            var findBy = new IndexConstraint(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

            ConstraintContext context = new ConstraintContext();
            var mockAttributeBag = new MockAttributeBag("name", "Y");
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
            Assert.IsTrue(findBy.Matches(mockAttributeBag, context));
            Assert.IsFalse(findBy.Matches(mockAttributeBag, context));
        }

        [Test, ExpectedException(typeof (ReEntryException))]
        public void RecusiveCallExceptionExpected()
        {
            var mockAttributeBag = new Mock<IAttributeBag>();
            
            // Note: Creating a re-entrant constraint is now much more difficult than
            //       before because constraints are immutable.  Even the code findBy |= findBy
            //       will not create a re-entrant constraint, it will just be an Or constraint
            //       with two identical clauses.  Given this change in constraint construction
            //       we should consider removing the re-entrance checks in the future.  -- Jeff.
            Constraint findBy = Find.By("tag", "value");
            findBy |=  new PredicateConstraint<IAttributeBag>(bag => findBy.Matches(bag, new ConstraintContext()));

            ConstraintContext context = new ConstraintContext();
            mockAttributeBag.Expect(bag => bag.GetAttributeValue("tag")).Returns("val");
            mockAttributeBag.Expect(bag => bag.GetAdapter<IAttributeBag>()).Returns(mockAttributeBag.Object);

            findBy.Matches(mockAttributeBag.Object, context);

            mockAttributeBag.VerifyAll();
        }
    }
}