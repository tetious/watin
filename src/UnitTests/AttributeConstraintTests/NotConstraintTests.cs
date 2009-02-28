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

using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
    [TestFixture]
    public class NotConstraintTests
    {
        [Test]
        public void NotTest()
        {
            // Given
            var mockAttributeBag = new Mock<IAttributeBag>().Object;
            ConstraintContext context = new ConstraintContext();

            var notConstraint = new NotConstraint(Find.None);

            // WHEN
            var result = notConstraint.Matches(mockAttributeBag, context);
            
            // THEN
            Assert.That(result, Is.True);
        }

        [Test]
        public void AttributeOperatorNotOverload()
        {
            // Given
            var mockAttributeBag = new Mock<IAttributeBag>().Object;
            ConstraintContext context = new ConstraintContext();

            // WHEN
            var attributenot = !Find.Any;

            // THEN
            Assert.IsInstanceOfType(typeof (NotConstraint), attributenot, "Expected NotAttributeConstraint instance");
            Assert.IsFalse(attributenot.Matches(mockAttributeBag, context));
        }
    }
}