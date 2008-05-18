#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using Rhino.Mocks;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
    [TestFixture]
    public class NotConstraintTests
    {
        private MockRepository mocks;
        private BaseConstraint _base;
        private IAttributeBag attributeBag;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            _base = (BaseConstraint) mocks.DynamicMock(typeof (BaseConstraint));
            attributeBag = (IAttributeBag) mocks.DynamicMock(typeof (IAttributeBag));

            SetupResult.For(_base.Compare(null)).IgnoreArguments().Return(false);
            mocks.ReplayAll();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void NotTest()
        {
            NotConstraint notConstraint = new NotConstraint(_base);
            Assert.IsTrue(notConstraint.Compare(attributeBag));
        }

        [Test]
        public void AttributeOperatorNotOverload()
        {
            BaseConstraint attributenot = !_base;

            Assert.IsInstanceOfType(typeof (NotConstraint), attributenot, "Expected NotAttributeConstraint instance");
            Assert.IsTrue(attributenot.Compare(attributeBag));
        }
    }
}