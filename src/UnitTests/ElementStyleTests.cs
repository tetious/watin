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

using NUnit.Framework.SyntaxHelpers;
using System;
using Moq;
using NUnit.Framework;
using WatiN.Core.Native;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class ElementStyleTests : BaseWithBrowserTests
    {
        private const string EXPECTED_STYLE = "background-color: blue; font-style: italic; font-family: Arial; height: 50px; color: white; font-size: 12px;";

        private TextField _element;

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        [Test]
        public void GetAttributeValueStyleAsString()
        {
            ExecuteTest(browser =>
                            {
                                _element = browser.TextField("Textarea1");
                                AssertStyleText(_element.GetAttributeValue("style").ToLowerInvariant());
                            });
        }

        [Test]
        public void ElementStyleToStringReturnsCssText()
        {
            ExecuteTest(browser =>
                            {
                                _element = browser.TextField("Textarea1");
                                Console.WriteLine(_element.Style.ToString().ToLowerInvariant());
                                AssertStyleText(_element.Style.ToString().ToLowerInvariant());
                            });
        }

        [Test]
        public void ElementStyleCssText()
        {
            ExecuteTest(browser =>
                            {
                                _element = browser.TextField("Textarea1");
                                AssertStyleText(_element.Style.CssText.ToLowerInvariant());
                            });
        }

        private void AssertStyleText(string cssText)
        {
            Assert.That(cssText.Length, Is.EqualTo(EXPECTED_STYLE.Length), "Unexpected length");

            var items = EXPECTED_STYLE.Split(Char.Parse(";"));

            foreach (var item in items)
            {
                Assert.That(cssText, Text.Contains(item.ToLowerInvariant().Trim()));
            }
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void GetAttributeValueOfNullThrowsArgumenNullException()
        {
            // GIVEN
            var nativeElementMock = new Mock<INativeElement>();
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
            var style = new Style(nativeElementMock.Object);

            // WHEN
            style.GetAttributeValue(null);

            // THEN exception
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void GetAttributeValueOfEmptyStringThrowsArgumenNullException()
        {
            // GIVEN
            var nativeElementMock = new Mock<INativeElement>();
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
            var style = new Style(nativeElementMock.Object);

            // WHEN
            style.GetAttributeValue(string.Empty);

            // THEN exception
        }

        [Test]
        public void GetAttributeValueBackgroundColor()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                _element = browser.TextField("Textarea1");
                                
                                // WHEN
                                var backgroundColor = _element.Style.GetAttributeValue("backgroundColor");
                                
                                // THEN
                                Assert.That(new HtmlColor(backgroundColor), Is.EqualTo(HtmlColor.Blue));
                            });
        }

        [Test]
        public void GetAttributeValueBackgroundColorByOriginalHTMLattributename()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                _element = browser.TextField("Textarea1");
                                
                                // WHEN
                                var backgroundColor = _element.Style.GetAttributeValue("background-color");
                                
                                // THEN
                                Assert.That(new HtmlColor(backgroundColor), Is.EqualTo(HtmlColor.Blue));
                            });
        }

        [Test]
        public void GetAttributeValueOfUndefinedButValidAttribute()
        {
            ExecuteTest(browser =>
                            {
                                _element = browser.TextField("Textarea1");
                                Assert.That(_element.Style.GetAttributeValue("backgroundImage"), Is.EqualTo("none"));
                            });
        }

        [Test]
        public void GetAttributeValueOfUndefinedAndInvalidAttribute()
        {
            ExecuteTest(browser =>
                            {
                                _element = browser.TextField("Textarea1");
                                Assert.IsNull(_element.Style.GetAttributeValue("nonexistingattrib"));
                            });
        }

        [Test]
        public void BackgroundColor()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                _element = browser.TextField("Textarea1");
                                
                                // WHEN
                                var backgroundColor = _element.Style.BackgroundColor;
                                
                                // THEN
                                Assert.That(backgroundColor, Is.EqualTo(HtmlColor.Blue));
                            });
        }

        [Test]
        public void Color()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                _element = browser.TextField("Textarea1");

                                // WHEN
                                var color = _element.Style.Color;
                                
                                // THEN
                                Assert.That(color, Is.EqualTo(HtmlColor.White));
                            });
        }

        [Test]
        public void FontFamily()
        {
            ExecuteTest(browser =>
                            {
                                _element = browser.TextField("Textarea1");
                                Assert.AreEqual("Arial", _element.Style.FontFamily);
                            });
        }

        [Test]
        public void FontSize()
        {
            ExecuteTest(browser =>
                            {
                                _element = browser.TextField("Textarea1");
                                Assert.AreEqual("12px", _element.Style.FontSize);
                            });
        }

        [Test]
        public void FontStyle()
        {
            ExecuteTest(browser =>
                            {
                                _element = browser.TextField("Textarea1");
                                Assert.AreEqual("italic", _element.Style.FontStyle);
                            });
        }

        [Test]
        public void Height()
        {
            ExecuteTest(browser =>
                            {
                                _element = browser.TextField("Textarea1");

                                if ((browser as FireFox) != null)
                                {
                                    Assert.That(_element.Style.Height.Equals("46px"));
                                }
                                else
                                {
                                    Assert.That(_element.Style.Height.Equals("50px"));
                                }

                            });
        }

        [Test]
        public void Should_return_style_set_by_a_style_sheet()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                browser.GoTo(StyleTestUri);
                var divWithExternalStyleApplied = browser.Div("logo");

                // WHEN
                var backgroundUrl = divWithExternalStyleApplied.Style.GetAttributeValue("BACKGROUND-IMAGE");

                // THEN
                Assert.That(backgroundUrl, Text.Contains("watin.jpg"));
            });
        }
    }
}