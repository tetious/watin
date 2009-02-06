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

            mockAttributeBag.Expect(bag => bag.GetValue("name")).Returns("X");
            mockAttributeBag.Expect(bag => bag.GetValue("value")).Returns("Cancel");

            // WHEN
            var compare = findBy.Compare(mockAttributeBag.Object);
            
            // THEN
            Assert.IsTrue(compare);
        }

        [Test]
        public void AndFalseFirstSoSecondPartShouldNotBeEvaluated()
        {
            // GIVEN
            var mockAttributeBag = new Mock<IAttributeBag>();

            var findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

            mockAttributeBag.Expect(bag => bag.GetValue("name")).Returns("Y");

            // WHEN
            var compare = findBy.Compare(mockAttributeBag.Object);

            // THEN
            Assert.IsFalse(compare);
        }

        [Test]
        public void AndFalseSecond()
        {
            var findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

            var attributeBag = new MockAttributeBag("name", "X");
            attributeBag.Add("value", "OK");
            
            Assert.IsFalse(findBy.Compare(attributeBag));
        }

        [Test]
        public void OrFirstTrue()
        {
            var findBy = Find.ByName("X").Or(Find.ByName("Y"));
            var attributeBag = new MockAttributeBag("name", "X");
            Assert.IsTrue(findBy.Compare(attributeBag));
        }

        [Test]
        public void OrSecondTrue()
        {
            var findBy = Find.ByName("X").Or(Find.ByName("Y"));
            var attributeBag = new MockAttributeBag("name", "Y");
            Assert.IsTrue(findBy.Compare(attributeBag));
        }

        [Test]
        public void OrFalse()
        {
            var findBy = Find.ByName("X").Or(Find.ByName("Y"));
            var attributeBag = new MockAttributeBag("name", "Z");
            Assert.IsFalse(findBy.Compare(attributeBag));
        }

        [Test]
        public void AndOr()
        {
            var findByNames = Find.ByName("X").Or(Find.ByName("Y"));
            var findBy = Find.ByValue("Cancel").And(findByNames);

            var attributeBag = new MockAttributeBag("name", "X");
            attributeBag.Add("value", "Cancel");
            Assert.IsTrue(findBy.Compare(attributeBag));
        }

        [Test]
        public void AndOrThroughOperatorOverloads()
        {
            var findBy = Find.ByName("X") & Find.ByValue("Cancel") | (Find.ByName("Z") & Find.ByValue("Cancel"));

            var attributeBag = new MockAttributeBag("name", "Z");
            attributeBag.Add("value", "OK");
            Assert.IsFalse(findBy.Compare(attributeBag));
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void OccurrenceShouldNotAcceptNegativeValue()
        {
            new IndexConstraint(-1);
        }

        [Test]
        public void Occurence0()
        {
            BaseConstraint findBy = new IndexConstraint(0);

            var attributeBag = new MockAttributeBag("name", "Z");
            
            Assert.IsTrue(findBy.Compare(attributeBag));
            Assert.IsFalse(findBy.Compare(attributeBag));
        }

        [Test]
        public void Occurence2()
        {
            BaseConstraint findBy = new IndexConstraint(2);

            var attributeBag = new MockAttributeBag("name", "Z");
            
            Assert.IsFalse(findBy.Compare(attributeBag));
            Assert.IsFalse(findBy.Compare(attributeBag));
            Assert.IsTrue(findBy.Compare(attributeBag));
            Assert.IsFalse(findBy.Compare(attributeBag));
        }

        [Test]
        public void OccurenceAndTrue()
        {
            var findBy = new IndexConstraint(1).And(Find.ByName("Z"));

            var attributeBag = new MockAttributeBag("name", "Z");
            
            Assert.IsFalse(findBy.Compare(attributeBag));
            Assert.IsTrue(findBy.Compare(attributeBag));
        }

        [Test]
        public void OccurenceOr()
        {
            var findBy = new IndexConstraint(2).Or(Find.ByName("Z"));

            var attributeBag = new MockAttributeBag("name", "Z");
            Assert.IsTrue(findBy.Compare(attributeBag));

            attributeBag = new MockAttributeBag("name", "y");
            
            Assert.IsFalse(findBy.Compare(attributeBag));
            Assert.IsTrue(findBy.Compare(attributeBag));
        }

        [Test]
        public void OccurenceAndFalse()
        {
            var findBy = new IndexConstraint(1).And(Find.ByName("Y"));

            var attributeBag = new MockAttributeBag("name", "Z");
            
            Assert.IsFalse(findBy.Compare(attributeBag));
            Assert.IsFalse(findBy.Compare(attributeBag));
        }

        [Test]
        public void TrueAndOccurence()
        {
            var findBy = Find.ByName("Z").And(new IndexConstraint(1));

            var attributeBag = new MockAttributeBag("name", "Z");
            
            Assert.IsFalse(findBy.Compare(attributeBag));
            Assert.IsTrue(findBy.Compare(attributeBag));
        }

        [Test]
        public void FalseAndOccurence()
        {
            var findBy = Find.ByName("Y").And(new IndexConstraint(1));

            var attributeBag = new MockAttributeBag("name", "Z");
            
            Assert.IsFalse(findBy.Compare(attributeBag));
            Assert.IsFalse(findBy.Compare(attributeBag));
        }

        [Test]
        public void TrueAndIndexConstraintAndTrue()
        {
            var mockAttributeBag = new Mock<IAttributeBag>();

            var findBy = Find.ByName("Z").And(new IndexConstraint(1)).And(Find.ByValue("text"));

            mockAttributeBag.Expect(bag => bag.GetValue("name")).Returns("Z");
            mockAttributeBag.Expect(bag => bag.GetValue("value")).Returns("text");

            Assert.IsFalse(findBy.Compare(mockAttributeBag.Object));

            mockAttributeBag.Expect(bag => bag.GetValue("name")).Returns("Z");
            mockAttributeBag.Expect(bag => bag.GetValue("value")).Returns("some other text");

            Assert.IsFalse(findBy.Compare(mockAttributeBag.Object));

            mockAttributeBag.Expect(bag => bag.GetValue("name")).Returns("Y");

            Assert.IsFalse(findBy.Compare(mockAttributeBag.Object));

            mockAttributeBag.Expect(bag => bag.GetValue("name")).Returns("Z");
            mockAttributeBag.Expect(bag => bag.GetValue("value")).Returns("text");

            Assert.IsTrue(findBy.Compare(mockAttributeBag.Object));

            mockAttributeBag.VerifyAll();
        }

        [Test]
        public void OccurenceAndOrWithOrTrue()
        {
            var findBy = new IndexConstraint(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

            var attributeBag = new MockAttributeBag("name", "Z");
            Assert.IsTrue(findBy.Compare(attributeBag));
        }

        [Test]
        public void OccurenceAndOrWithAndTrue()
        {
            var findBy = new IndexConstraint(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

            var attributeBag = new MockAttributeBag("name", "Y");
            Assert.IsFalse(findBy.Compare(attributeBag));
            Assert.IsFalse(findBy.Compare(attributeBag));
            Assert.IsTrue(findBy.Compare(attributeBag));
            Assert.IsFalse(findBy.Compare(attributeBag));
        }

        [Test, ExpectedException(typeof (ReEntryException))]
        public void RecusiveCallExceptionExpected()
        {
            var mockAttributeBag = new Mock<IAttributeBag>();
            
            BaseConstraint findBy = Find.By("tag", "value");
            findBy.Or(findBy);

            mockAttributeBag.Expect(bag => bag.GetValue("tag")).Returns("val");

            findBy.Compare(mockAttributeBag.Object);

            mockAttributeBag.VerifyAll();
        }
    }
}