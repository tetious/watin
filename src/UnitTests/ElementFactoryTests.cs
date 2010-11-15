#region WatiN Copyright (C) 2006-2010 Jeroen van Menen

//Copyright 2006-2010 Jeroen van Menen
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
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class ElementFactoryTests
    {
        private const bool dont_inherit = false;

        [Test]
        public void Should_retrieve_element_tags_from_attributes()
        {

            // GIVEN
            var elementType = typeof(TestElement);
            Assert.That(elementType.GetCustomAttributes(dont_inherit).Length, Is.EqualTo(2), "Pre-condition: Expected 2 attributes");
            
            // WHEN
            var elementTags = ElementFactory.GetElementTags(elementType);

            // THEN
            Assert.That(elementTags.Count, Is.EqualTo(2), "Unexpected number of element tagnames");
        }

        [Test]
        public void Should_retrieve_element_tags_from_base_class_if_no_elementtag_attributes_are_specified()
        {
            // GIVEN
            var testElementWithNoElementTags = typeof(TestElementWithNoElementTags);
            Assert.That(testElementWithNoElementTags.GetCustomAttributes(dont_inherit).Length, Is.EqualTo(0), "Pre-condition: Expected no attributes");

            // WHEN
            var elementTags = ElementFactory.GetElementTags(testElementWithNoElementTags);

            // THEN
            Assert.That(elementTags.Count, Is.EqualTo(2), "Unexpected number of element tagnames");
        }

        [Test]
        public void Should_not_retrieve_element_tags_from_base_class_if_at_least_one_elementtag_attribute_is_specified()
        {
            // GIVEN
            var testElementWithOneElementTag = typeof(TestElementWithOneElementTag);
            Assert.That(testElementWithOneElementTag.GetCustomAttributes(dont_inherit).Length, Is.EqualTo(1), "Pre-condition: Expected one attribute");

            // WHEN
            var elementTags = ElementFactory.GetElementTags(testElementWithOneElementTag);

            // THEN
            Assert.That(elementTags.Count, Is.EqualTo(1), "Unexpected number of element tagnames");
        }

        [Test]
        public void Should_not_inherit_element_tag_attributes_when_type_is_an_native_watin_element()
        {
            // GIVEN
            var nativeElement = typeof(Element<Div>);

            Assert.That(nativeElement.GetCustomAttributes(dont_inherit).Length, Is.EqualTo(0), "Pre-condition: Expected no attributes");
            Assert.That(nativeElement.IsSubclassOf(typeof(Element)), "Pre-condition: Should inherit from Element which has the Any attribute returned from GetElementTags");
            Assert.That(nativeElement.Namespace, Is.EqualTo(ElementFactory.ElementNameSpace), "Pre-condition: Should be in the same namespace otherwise it will inherit attributes");

            // WHEN
            var elementTags = ElementFactory.GetElementTags(nativeElement);

            // THEN
            Assert.That(elementTags.Count, Is.EqualTo(0), "Unexpected number of element tagnames");

        }

        [Test]
        public void Should_return_any_element_tag_for_element()
        {
            // GIVEN
            var nativeWatinElementWithNoAttributes = typeof(Element);
            Assert.That(nativeWatinElementWithNoAttributes.GetCustomAttributes(typeof(ElementTagAttribute), dont_inherit).Length, Is.EqualTo(0), "Pre-condition: Expected no attributes");

            // WHEN
            var elementTags = ElementFactory.GetElementTags(nativeWatinElementWithNoAttributes);

            // THEN
            Assert.That(elementTags.Count, Is.EqualTo(1), "Unexpected number of element tagnames");
            Assert.That(elementTags[0].IsAny, Is.True, "Unexpected number of element tagnames");
        }

    }

    public class TestElementWithNoElementTags : TestElement
    {
    }

    [ElementTag("a")]
    public class TestElementWithOneElementTag : TestElement
    {
    }

    [ElementTag("div", Index = 0)]
    [ElementTag("span", Index = 1)]
    public class TestElement
    {
    }
}
