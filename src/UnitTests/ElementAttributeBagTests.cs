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
using WatiN.Core.Interfaces;
using WatiN.Core.InternetExplorer;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementAttributeBagTests
	{
	    private Mock<DomContainer> mockDomContainer;

		[SetUp]
		public void SetUp()
		{
            mockDomContainer = new Mock<DomContainer>();
		}

		[Test]
		public void StyleAttributeShouldReturnAsString()
		{
            // GIVEN
			const string cssText = "COLOR: white; FONT-STYLE: italic";

            var mockNativeElement = new Mock<INativeElement>();
            mockNativeElement.Expect(element => element.GetStyleAttributeValue("cssText")).Returns(cssText);

            var attributeBag = new ElementAttributeBag(mockDomContainer.Object, mockNativeElement.Object);

            // WHEN
		    var value = attributeBag.GetValue("style");

            // THEN
		    Assert.That(value, Is.EqualTo(cssText));
		}

		[Test]
		public void StyleDotStyleAttributeNameShouldReturnStyleAttribute()
		{
            // GIVEN
			const string styleAttributeValue = "white";

            var mockNativeElement = new Mock<INativeElement>();
            mockNativeElement.Expect(element => element.GetStyleAttributeValue("color")).Returns(styleAttributeValue);
            
            var attributeBag = new ElementAttributeBag(mockDomContainer.Object, mockNativeElement.Object);

            // WHEN
		    var value = attributeBag.GetValue("style.color");

            // THEN
		    Assert.AreEqual(styleAttributeValue, value);
		}

        [Test]
        public void CachedElementInstancesShouldBeClearedWhenINativeElementIsSet()
        {
            // GIVEN
            var mockNativeElement = new Mock<INativeElement>();
            mockNativeElement.Expect(element => element.GetAttributeValue("id")).Returns("one");
            mockNativeElement.Expect(element => element.TagName).Returns("li");
            
            var mockNativeElement2 = new Mock<INativeElement>();
            mockNativeElement2.Expect(element => element.GetAttributeValue("id")).Returns("two");
            mockNativeElement2.Expect(element => element.TagName).Returns("li");

            var ieBrowser = new IEBrowser(mockDomContainer.Object);
            mockDomContainer.Expect(domContainer => domContainer.NativeBrowser).Returns(ieBrowser);

            var attributeBag = new ElementAttributeBag(mockDomContainer.Object, mockNativeElement.Object);

            Assert.That(attributeBag.Element.Id, Is.EqualTo("one"), "Unexpected Element");
            Assert.That(attributeBag.ElementTyped.Id, Is.EqualTo("one"), "Unexpected ElementTyped");

            // WHEN
            attributeBag.INativeElement = mockNativeElement2.Object;

            // THEN
            Assert.That(attributeBag.Element.Id, Is.EqualTo("two"), "Unexpected Element");
            Assert.That(attributeBag.ElementTyped.Id, Is.EqualTo("two"), "Unexpected ElementTyped");
        }
	}
}