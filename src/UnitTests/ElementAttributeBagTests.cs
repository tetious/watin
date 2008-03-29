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

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementAttributeBagTests
	{
		private MockRepository mocks;
		private IHTMLStyle mockHTMLStyle;
		private IHTMLElement mockHTMLElement;
	    private DomContainer domContainer;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			mockHTMLStyle = (IHTMLStyle) mocks.CreateMock(typeof (IHTMLStyle));
            mockHTMLElement = (IHTMLElement)mocks.CreateMock(typeof(IHTMLElement)); 
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
			Expect.Call(mockHTMLElement.style).Return(mockHTMLStyle);

			mocks.ReplayAll();

            ElementAttributeBag attributeBag = new ElementAttributeBag(domContainer, mockHTMLElement);

			Assert.AreEqual(cssText, attributeBag.GetValue("style"));
		}

		[Test]
		public void StyleDotStyleAttributeNameShouldReturnStyleAttribute()
		{
			const string styleAttributeValue = "white";
			const string styleAttributeName = "color";

			Expect.Call(mockHTMLStyle.getAttribute(styleAttributeName, 0)).Return(styleAttributeValue);
			Expect.Call(mockHTMLElement.style).Return(mockHTMLStyle);

			mocks.ReplayAll();

            ElementAttributeBag attributeBag = new ElementAttributeBag(domContainer, mockHTMLElement);

			Assert.AreEqual(styleAttributeValue, attributeBag.GetValue("style.color"));
		}
	}
}