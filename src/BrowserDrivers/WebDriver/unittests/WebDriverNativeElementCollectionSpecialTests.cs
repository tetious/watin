using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using OpenQA.Selenium;

namespace Watin.BrowserDrivers.WebDriver.Tests
{
    [TestFixture]
    public class WebDriverNativeElementCollectionSpecialTests
    {
        [Test]
        public void Should_return_all_elements()
        {
            // GIVEN
            var element1 = CreateDivNativeElement("id1");
            var element2 = CreateDivNativeElement("id2");

            IList<IWebElement> elements = new List<IWebElement> { element1, element2 };
            var collection = new WebDriverNativeElementCollectionSpecial(() => elements);

            // WHEN
            var result = collection.GetElements();

            // THEN
            Assert.That(result.Count(), Is.EqualTo(2));

        }

        [Test]
        public void Should_return_elements_with_specific_id()
        {
            // GIVEN
            var element1 = CreateDivNativeElement("id1");
            var element2 = CreateDivNativeElement("id2");
            var element3 = CreateDivNativeElement("id1");

            IList<IWebElement> elements = new List<IWebElement> { element1, element2, element3 };
            var collection = new WebDriverNativeElementCollectionSpecial(() => elements);

            // WHEN
            var result = collection.GetElementsById("id1");

            // THEN
            Assert.That(result.Count(), Is.EqualTo(2), "Unexpected number of elements");
            var id1Count = result.Count(element => element.GetAttributeValue("Id") == "id1");
            Assert.That(id1Count, Is.EqualTo(2), "Unexpected number of elements with id1");
        }

        [Test]
        public void Should_return_no_elements_when_theres_no_match_with_id()
        {
            // GIVEN
            var element1 = CreateDivNativeElement("id1");
            var element2 = CreateDivNativeElement("id2");
            var element3 = CreateDivNativeElement("id3");

            IList<IWebElement> elements = new List<IWebElement> { element1, element2, element3 };
            var collection = new WebDriverNativeElementCollectionSpecial(() => elements);

            // WHEN
            var result = collection.GetElementsById("noneExistingId");

            // THEN
            Assert.That(result.Count(), Is.EqualTo(0), "Unexpected number of elements");
        }

        [Test]
        public void Should_return_elements_with_specific_tagname()
        {
            // GIVEN
            var element1 = CreateNativeElement("id1", "div");
            var element2 = CreateNativeElement("id2", "a");
            var element3 = CreateNativeElement("id1", "div");

            IList<IWebElement> elements = new List<IWebElement> { element1, element2, element3 };
            var collection = new WebDriverNativeElementCollectionSpecial(() => elements);

            // WHEN
            var result = collection.GetElementsByTag("div");

            // THEN
            Assert.That(result.Count(), Is.EqualTo(2), "Unexpected number of elements");
            var id1Count = result.Count(element => element.TagName == "div");
            Assert.That(id1Count, Is.EqualTo(2), "Unexpected number of div elements");
        }

        [Test]
        public void Should_return_no_elements_when_theres_no_match_with_tagname()
        {
            // GIVEN
            var element1 = CreateNativeElement("id1", "div");
            var element2 = CreateNativeElement("id2", "a");
            var element3 = CreateNativeElement("id3", "li");

            IList<IWebElement> elements = new List<IWebElement> { element1, element2, element3 };
            var collection = new WebDriverNativeElementCollectionSpecial(() => elements);

            // WHEN
            var result = collection.GetElementsByTag("input");

            // THEN
            Assert.That(result.Count(), Is.EqualTo(0), "Unexpected number of elements");
        }

        private static IWebElement CreateDivNativeElement(string id)
        {
            return CreateNativeElement(id, "div");
        }

        private static IWebElement CreateNativeElement(string id, string tagname)
        {
            var element = Substitute.For<IWebElement>();
            element.GetAttribute("id").Returns(id);
            element.TagName.Returns(tagname);

            return element;
        }
    }
}
