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
using WatiN.Core.Interfaces;
using WatiN.Core.InternetExplorer;
using Iz = NUnit.Framework.SyntaxHelpers.Is;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementAttributeBagTests
	{
		private MockRepository mocks;
	    private INativeElement mockNativeElement;
	    private DomContainer domContainer;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
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
            // GIVEN
			const string cssText = "COLOR: white; FONT-STYLE: italic";

            Expect.Call(mockNativeElement.GetStyleAttributeValue("csstext")).Return(cssText);

			mocks.ReplayAll();

            var attributeBag = new ElementAttributeBag(domContainer, mockNativeElement);

            // WHEN
		    var value = attributeBag.GetValue("style");

            // THEN
		    Assert.That(cssText, Iz.EqualTo(value));
		}

		[Test]
		public void StyleDotStyleAttributeNameShouldReturnStyleAttribute()
		{
            // GIVEN
			const string styleAttributeValue = "white";

		    Expect.Call(mockNativeElement.GetStyleAttributeValue("color")).Return(styleAttributeValue);

			mocks.ReplayAll();

            var attributeBag = new ElementAttributeBag(domContainer, mockNativeElement);

            // WHEN
		    var value = attributeBag.GetValue("style.color");

            // THEN
		    Assert.AreEqual(styleAttributeValue, value);
		}

        [Test]
        public void CachedElementPropertiesShouldBeClearedIfNewHtmlElementIsSet()
        {
            // GIVEN
            Expect.Call(mockNativeElement.GetAttributeValue("id")).Return("one").Repeat.Any();
            Expect.Call(mockNativeElement.TagName).Return("li").Repeat.Any();
            
            var mockNativeElement2 = (INativeElement)mocks.CreateMock(typeof(INativeElement));
            Expect.Call(mockNativeElement2.GetAttributeValue("id")).Return("two").Repeat.Any();
            Expect.Call(mockNativeElement2.TagName).Return("li").Repeat.Any();
            
            Expect.Call(domContainer.NativeBrowser).Return(new IEBrowser(domContainer)).Repeat.Any();

            mocks.ReplayAll();

            var attributeBag = new ElementAttributeBag(domContainer, mockNativeElement);

            Assert.That(attributeBag.Element.Id, Iz.EqualTo("one"), "Unexpected Element");
            Assert.That(attributeBag.ElementTyped.Id, Iz.EqualTo("one"), "Unexpected ElementTyped");

            // WHEN
            attributeBag.INativeElement = mockNativeElement2;

            // THEN
            Assert.That(attributeBag.Element.Id, Iz.EqualTo("two"), "Unexpected Element");
            Assert.That(attributeBag.ElementTyped.Id, Iz.EqualTo("two"), "Unexpected ElementTyped");
        }
	}
}