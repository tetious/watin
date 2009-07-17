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
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using WatiN.Core.Native;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
    [TestFixture]
    public class ElementConstraintTest : BaseWithBrowserTests
    {
        [Test]
        public void ElementConstraintShouldCallComparerAndReturnTrue()
        {
            VerifyComparerIsUsed("testtagname", true);
        }

        [Test]
        public void ElementConstraintShouldCallComparerAndReturnFalse()
        {
            VerifyComparerIsUsed("tagname", false);
        }

        private static void VerifyComparerIsUsed(string tagname, bool expectedResult)
        {
            var nativeElementMock = new Mock<INativeElement>();
            var domContainerMock = new Mock<DomContainer>();
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
            var element = new Element(domContainerMock.Object, nativeElementMock.Object);

            nativeElementMock.Expect(native => native.TagName).Returns("testtagname");
			
            var elementComparerMock = new ElementComparerMock(tagname);
            var elementConstraint = new ElementConstraint(elementComparerMock);

            ConstraintContext context = new ConstraintContext();
            Assert.That(elementConstraint.Matches(element, context) == expectedResult);
        }

        [Test]
        public void ShouldEvaluateAlsoTheAndConstraint()
        {
            var link1 = Ie.Link(Find.ByElement(
                                    l => l.Id != null && l.Id.StartsWith("testlink")) && Find.ByUrl("http://watin.sourceforge.net"));
            var link2 = Ie.Link(Find.ByElement(
                                    l => l.Id != null && l.Id.StartsWith("testlink")) && Find.ByUrl("http://www.microsoft.com/"));

            Assert.That(link1.Url, Is.EqualTo("http://watin.sourceforge.net/"));
            Assert.That(link2.Url, Is.EqualTo("http://www.microsoft.com/"));
        }

        public class ElementComparerMock : Comparer<Element>
        {
            private readonly string _expectedTagName;
			
            public ElementComparerMock(string expectedTagName)
            {
                _expectedTagName = expectedTagName;
            }

            public override bool Compare(Element element)
            {
                return _expectedTagName == element.TagName;
            }
        }

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }
    }
}