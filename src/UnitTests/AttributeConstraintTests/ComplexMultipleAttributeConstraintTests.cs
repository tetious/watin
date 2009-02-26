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

using NUnit.Framework;
using WatiN.Core.Constraints;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
    [TestFixture]
    public class ComplexMultipleAttributeConstraintTests
    {
        private MockAttributeBag mockAttributeBag;

        private Constraint findBy1;
        private Constraint findBy2;
        private Constraint findBy3;
        private Constraint findBy4;
        private Constraint findBy5;

        private Constraint findBy;

        [SetUp]
        public void Setup()
        {
            mockAttributeBag = new MockAttributeBag("1", "true");
            mockAttributeBag.Add("2","false");
            mockAttributeBag.Add("4", "true");
            mockAttributeBag.Add("5", "false");

            findBy = null;

            findBy1 = Find.By("1", "true");
            findBy2 = Find.By("2", "true");
            findBy3 = Find.By("3", "true");
            findBy4 = Find.By("4", "true");
            findBy5 = Find.By("5", "true");
        }

        [Test]
        public void WithoutBrackets()
        {
            findBy = findBy1.And(findBy2).And(findBy3).Or(findBy4).And(findBy5);
        }

        [Test]
        public void WithBrackets()
        {
            findBy = findBy1.And(findBy2.And(findBy3)).Or(findBy4.And(findBy5));
        }

        [Test]
        public void WithBracketsOperators1()
        {
            findBy = findBy1 & findBy2 & findBy3 | findBy4 & findBy5;
        }

        [Test]
        public void WithBracketsOperators2()
        {
            findBy = findBy1 && findBy2 && findBy3 || findBy4 && findBy5;
        }

        [TearDown]
        public void TearDown()
        {
            Assert.IsFalse(findBy.Matches(mockAttributeBag, new ConstraintContext()));
        }
    }
}
