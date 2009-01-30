#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

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
using Moq;
using NUnit.Framework;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementStyleTests : BaseWithBrowserTests
	{
		private const string expectedStyle = "font-size: 12px; color: white; font-style: italic; font-family: arial; height: 50px; background-color: blue;";
		private TextField element;

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

		[Test]
		public void GetAttributeValueStyleAsString()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual(expectedStyle, element.GetAttributeValue("style").ToLowerInvariant());
		                    });
		}

		[Test]
		public void ElementStyleToStringReturnsCssText()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual(expectedStyle, element.Style.ToString().ToLowerInvariant());
		                    });
		}

		[Test]
		public void ElementStyleCssText()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual(expectedStyle, element.Style.CssText.ToLowerInvariant());
		                    });
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void GetAttributeValueOfNullThrowsArgumenNullException()
		{
		    // GIVEN
		    var style = new Style(new Mock<INativeElement>().Object);

            // WHEN
		    style.GetAttributeValue(null);

            // THEN exception
		}

	    [Test, ExpectedException(typeof (ArgumentNullException))]
		public void GetAttributeValueOfEmptyStringThrowsArgumenNullException()
		{
            // GIVEN
            var style = new Style(new Mock<INativeElement>().Object);

            // WHEN
            style.GetAttributeValue(string.Empty);

            // THEN exception
        }

		[Test]
		public void GetAttributeValueBackgroundColor()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual("blue", element.Style.GetAttributeValue("backgroundColor"));
		                    });
		}

		[Test]
		public void GetAttributeValueBackgroundColorByOriginalHTMLattributename()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual("blue", element.Style.GetAttributeValue("background-color"));
		                    });
		}

		[Test]
		public void GetAttributeValueOfUndefinedButValidAttribute()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.IsNull(element.Style.GetAttributeValue("cursor"));
		                    });
		}

		[Test]
		public void GetAttributeValueOfUndefinedAndInvalidAttribute()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.IsNull(element.Style.GetAttributeValue("nonexistingattrib"));
		                    });
		}

		[Test]
		public void BackgroundColor()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual("blue", element.Style.BackgroundColor);
		                    });
		}

		[Test]
		public void Color()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual("white", element.Style.Color);
		                    });
		}

		[Test]
		public void FontFamily()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual("Arial", element.Style.FontFamily);
		                    });
		}

		[Test]
		public void FontSize()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual("12px", element.Style.FontSize);
		                    });
		}

		[Test]
		public void FontStyle()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual("italic", element.Style.FontStyle);
		                    });
		}

		[Test]
		public void Height()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.TextField("Textarea1");
		                        Assert.AreEqual("50px", element.Style.Height);
		                    });
		}
	}
}