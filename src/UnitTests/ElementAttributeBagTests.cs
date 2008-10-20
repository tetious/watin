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

using mshtml;
using NUnit.Framework;
using Rhino.Mocks;
using WatiN.Core.Interfaces;
using WatiN.Core.InternetExplorer;
using Iz = NUnit.Framework.SyntaxHelpers.Is;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementAttributeBagTests
	{
		private MockRepository mocks;
		private IHTMLStyle mockHTMLStyle;
		private INativeElement mockNativeElement;
	    private DomContainer domContainer;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			mockHTMLStyle = (IHTMLStyle) mocks.CreateMock(typeof (IHTMLStyle));
            mockNativeElement = (INativeElement)mocks.CreateMock(typeof(INativeElement)); 
            domContainer = (DomContainer)mocks.DynamicMock(typeof(DomContainer));
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void StyleAttributeShouldReturnAsString()
		{
			const string cssText = "COLOR: white; FONT-STYLE: italic";

			Expect.Call(mockHTMLStyle.cssText).Return(cssText);
			Expect.Call(mockNativeElement.GetAttributeValue("style")).Return(mockHTMLStyle);

			mocks.ReplayAll();

            var attributeBag = new ElementAttributeBag(domContainer, mockNativeElement);

			Assert.AreEqual(cssText, attributeBag.GetValue("style"));
		}

		[Test]
		public void StyleDotStyleAttributeNameShouldReturnStyleAttribute()
		{
			const string styleAttributeValue = "white";
			const string styleAttributeName = "color";

			Expect.Call(mockHTMLStyle.getAttribute(styleAttributeName, 0)).Return(styleAttributeValue);
			Expect.Call(mockNativeElement.GetAttributeValue("style")).Return(mockHTMLStyle);

			mocks.ReplayAll();

            var attributeBag = new ElementAttributeBag(domContainer, mockNativeElement);

			Assert.AreEqual(styleAttributeValue, attributeBag.GetValue("style.color"));
		}

        [Test]
        public void CachedElementPropertiesShouldBeClearedIfNewHtmlElementIsSet()
        {
            Expect.Call(mockNativeElement.GetAttributeValue("id")).Return("one").Repeat.Any();
            Expect.Call(mockNativeElement.GetAttributeValue("tagName")).Return("li").Repeat.Any();
            
            var mockHTMLElement2 = (INativeElement)mocks.CreateMock(typeof(INativeElement));
            Expect.Call(mockHTMLElement2.GetAttributeValue("id")).Return("two").Repeat.Any();
            Expect.Call(mockHTMLElement2.GetAttributeValue("tagName")).Return("li").Repeat.Any();
            Expect.Call(domContainer.NativeBrowser).Return(new IEBrowser(domContainer)).Repeat.Any();

            mocks.ReplayAll();

            var attributeBag = new ElementAttributeBag(domContainer, mockNativeElement);

            Assert.That(attributeBag.Element.Id, Iz.EqualTo("one"), "Unexpected Element");
            Assert.That(attributeBag.ElementTyped.Id, Iz.EqualTo("one"), "Unexpected ElementTyped");

            attributeBag.INativeElement = mockHTMLElement2;

            Assert.That(attributeBag.Element.Id, Iz.EqualTo("two"), "Unexpected Element");
            Assert.That(attributeBag.ElementTyped.Id, Iz.EqualTo("two"), "Unexpected ElementTyped");

        }
	}
}