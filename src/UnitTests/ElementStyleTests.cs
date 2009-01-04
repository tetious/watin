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

using System;
using NUnit.Framework;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementStyleTests : BaseWithBrowserTests
	{
		private const string style = "FONT-SIZE: 12px; COLOR: white; FONT-STYLE: italic; FONT-FAMILY: Arial; HEIGHT: 50px; BACKGROUND-COLOR: blue";
		private TextField element;

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

		[SetUp]
		public override void TestSetUp()
		{
			base.TestSetUp ();
			element = Ie.TextField("Textarea1");
		}

		[Test]
		public void GetAttributeValueStyleAsString()
		{
			Assert.AreEqual(style, element.GetAttributeValue("style"));
		}

		[Test]
		public void ElementStyleToStringReturnsCssText()
		{
			Assert.AreEqual(style, element.Style.ToString());
		}

		[Test]
		public void ElementStyleCssText()
		{
			Assert.AreEqual(style, element.Style.CssText);
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void GetAttributeValueOfNullThrowsArgumenNullException()
		{
			element.Style.GetAttributeValue(null);
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void GetAttributeValueOfEmptyStringThrowsArgumenNullException()
		{
			element.Style.GetAttributeValue(String.Empty);
		}

		[Test]
		public void GetAttributeValueBackgroundColor()
		{
			Assert.AreEqual("blue", element.Style.GetAttributeValue("BackgroundColor"));
		}

		[Test]
		public void GetAttributeValueBackgroundColorByOriginalHTMLattribname()
		{
			Assert.AreEqual("blue", element.Style.GetAttributeValue("background-color"));
		}

		[Test]
		public void GetAttributeValueOfUndefiniedButValidAttribute()
		{
			Assert.IsNull(element.Style.GetAttributeValue("cursor"));
		}

		[Test]
		public void GetAttributeValueOfUndefiniedAndInvalidAttribute()
		{
			Assert.IsNull(element.Style.GetAttributeValue("nonexistingattrib"));
		}

		[Test]
		public void BackgroundColor()
		{
			Assert.AreEqual("blue", element.Style.BackgroundColor);
		}

		[Test]
		public void Color()
		{
			Assert.AreEqual("white", element.Style.Color);
		}

		[Test]
		public void FontFamily()
		{
			Assert.AreEqual("Arial", element.Style.FontFamily);
		}

		[Test]
		public void FontSize()
		{
			Assert.AreEqual("12px", element.Style.FontSize);
		}

		[Test]
		public void FontStyle()
		{
			Assert.AreEqual("italic", element.Style.FontStyle);
		}

		[Test]
		public void Height()
		{
			Assert.AreEqual("50px", element.Style.Height);
		}
	}
}