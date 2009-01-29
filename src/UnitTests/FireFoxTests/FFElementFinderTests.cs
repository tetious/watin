using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using WatiN.Core.Mozilla;

namespace WatiN.Core.UnitTests.FireFoxTests
{
    [TestFixture]
    public class FFElementFinderTests
    {
        [Test]
        public void FindElementByIdShouldReturnNullIfParentHasNoElements()
        {
            // GIVEN
            var elementTags = new List<ElementTag>{new ElementTag("button")};
            var domContainer = new Mock<DomContainer>().Object;
            var fireFoxClientPort = new FireFoxClientPort();
            var findById = Find.ById("myId");

            var mockElementCollection = new Mock<IElementCollection>();
            mockElementCollection.ExpectGet(ec => ec.Elements).Returns(null);
            
            var elementCollection = mockElementCollection.Object;

            var finder = new WrapperForFFElementFinder(elementTags, findById, elementCollection, domContainer, fireFoxClientPort);

            // WHEN
            var elements = finder.FindElemById(findById, elementTags[0], new ElementAttributeBag(domContainer), true, elementCollection);

            // THEN
            Assert.That(elements, Is.Empty, "No elements expected");
            mockElementCollection.VerifyAll();
        }
    }

    public class WrapperForFFElementFinder : FFElementFinder
    {
        public WrapperForFFElementFinder(List<ElementTag> elementTags, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer, FireFoxClientPort clientPort) : base(elementTags, constraint, elementCollection, domContainer, clientPort)
        {
        }

        public List<INativeElement> FindElemById(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection elementCollection)
        {
            return base.FindElementById(constraint, elementTag, attributeBag, returnAfterFirstMatch, elementCollection);
        }
    }
}
