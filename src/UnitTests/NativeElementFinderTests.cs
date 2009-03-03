using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.Native;

namespace WatiN.Core.UnitTests
{
    /// <summary>
    ///This is a NUnit TestFixture class
    ///</summary>
    [TestFixture]
    public class NativeElementFinderTests
    {
        [Test]
        public void ShouldCallGetElementsById()
        {
            // GIVEN
            var finder = CreateNativeElementFinder(Find.ById("myId"));

            // WHEN
            finder.CallFindAllImpl();

            // THEN
            finder.MockElementCollection.Verify(collection => collection.GetElementsById("myId"));
        }

        [Test]
        public void ShouldCallGetElementsByTagName()
        {
            // GIVEN
            var finder = CreateNativeElementFinder(Find.ByName("myName"));

            // WHEN
            finder.CallFindAllImpl();

            // THEN
            finder.MockElementCollection.Verify(collection => collection.GetElementsByTag("div"));
        }

        [Test]
        public void GetElementIdHintShouldReturnIdFor_FindByIdAndFindAny()
        {
            // GIVEN
            var finder = CreateNativeElementFinder(null);

            // WHEN
            var idHint = finder.DoGetElementIdHint(Find.ById("myId") & Find.Any);

            // THEN
            Assert.That(idHint, Is.EqualTo("myId"));
        }

        [Test]
        public void GetElementIdHintShouldReturnIdFor_FindAnyAndFindById()
        {
            // GIVEN
            var finder = CreateNativeElementFinder(null);

            // WHEN
            var idHint = finder.DoGetElementIdHint(Find.Any & Find.ById("myId"));

            // THEN
            Assert.That(idHint, Is.EqualTo("myId"));
        }

        [Test]
        public void GetElementIdHintShouldReturnNullFor_FindByIdAndFindAnythingElse()
        {
            // GIVEN
            var finder = CreateNativeElementFinder(null);

            // WHEN
            var idHint = finder.DoGetElementIdHint(Find.ById("myId") & Find.First());

            // THEN
            Assert.That(idHint, Is.Null);
        }

        [Test]
        public void GetElementIdHintShouldReturnNullFor_FindAnythingElseAndFindById()
        {
            // GIVEN
            var finder = CreateNativeElementFinder(null);

            // WHEN
            var idHint = finder.DoGetElementIdHint(Find.First() & Find.ById("myId"));

            // THEN
            Assert.That(idHint, Is.Null);
        }

        private static MockNativeElementFinder CreateNativeElementFinder(Constraint constraint)
        {
            var domContainer = new Mock<DomContainer>().Object;
            var tags = new List<ElementTag> { new ElementTag("div") };
            var mockElementCollection = new Mock<INativeElementCollection>();

            return new MockNativeElementFinder(mockElementCollection, domContainer, tags, constraint);
        }

        public class MockNativeElementFinder : NativeElementFinder
        {
            public IMock<INativeElementCollection> MockElementCollection { get; set; }

            public MockNativeElementFinder(IMock<INativeElementCollection> mockElementCollection, DomContainer domContainer, IList<ElementTag> elementTags, Constraint constraint)
                : base(() => mockElementCollection.Object, domContainer, elementTags, constraint)
            {
                MockElementCollection = mockElementCollection;
            }

            public List<Element> CallFindAllImpl()
            {
                return new List<Element>(base.FindAllImpl());
            }

            public string DoGetElementIdHint(Constraint constraint)
            {
                return GetElementIdHint(constraint);
            }
        }
    }
}