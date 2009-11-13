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
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

#if !NET20
namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class ComponentExtensionsTests : BaseWithBrowserTests
    {
        [Test]
        public void WithDescriptionWhenDescriptionIsNotNullShouldSetDescriptionProperty()
        {
            var component = new Mock<IHasDescription>();

            var result = component.Object.WithDescription("Description.");

            Assert.That(result, Is.SameAs(component.Object));
            component.VerifySet(x => x.Description, "Description.");
        }

        [Test]
        public void WithDescriptionWhenDescriptionIsNullShouldSetDescriptionProperty()
        {
            var component = new Mock<IHasDescription>();

            var result = component.Object.WithDescription(null);

            Assert.That(result, Is.SameAs(component.Object));
            component.VerifySet(x => x.Description, null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void WithDescriptionWhenComponentIsNullShouldThrow()
        {
            ComponentExtensions.WithDescription<IHasDescription>(null, "Description.");
        }

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }
    }
}
#endif