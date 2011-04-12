#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
    [TestFixture]
    public class FindBySelectorTests :BaseWithBrowserTests
    {
        [Test]
        public void Should_filter_input_elements_with_given_selector_filter()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                var querySelectorConstraint = new QuerySelectorConstraint("input");

                                // WHEN
                                var elements = browser.Elements.Filter(querySelectorConstraint);

                                // THEN
                                Assert.That(elements.Count, Is.EqualTo(19));
                            });
        }

        [Test]
        public void Should_find_element()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                var querySelectorConstraint = new QuerySelectorConstraint("#popupid");

                                // WHEN
                                var element = browser.Element(querySelectorConstraint);

                                // THEN
                                Assert.That(element.Exists, Is.True);
                                Assert.That(element.Id, Is.EqualTo("popupid"));
                            });
        }

        [Test]
        public void Should_not_find_div_when_given_selector_returns_an_input()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                var querySelectorConstraint = new QuerySelectorConstraint("input");

                                // WHEN
                                var div = browser.Div(querySelectorConstraint);

                                // THEN
                                Assert.That(div.Exists, Is.False);
                            });
        }

        [Test]
        public void Should_only_element_inside_container_element()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var querySelectorConstraint = new QuerySelectorConstraint("#popupid");
                var containerElement = browser.Form("Form");

                // WHEN
                var element = containerElement.Element(querySelectorConstraint);

                // THEN
                Assert.That(element.Exists, Is.True);
                Assert.That(element.Id, Is.EqualTo("popupid"));
            });
        }

        [Test]
        public void Should_not_find_element_not_inside_container_element()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var querySelectorConstraint = new QuerySelectorConstraint("#popupid");
                var containerElement = browser.Form("FormInputElement");

                // WHEN
                var element = containerElement.Element(querySelectorConstraint);

                // THEN
                Assert.That(element.Exists, Is.False);
            });
        }

        [Test]
        public void Should_be_able_to_use_single_quotes_in_selector_string()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var querySelectorConstraint = new QuerySelectorConstraint("input[name='textinput1']");

                // WHEN
                var element = browser.Element(querySelectorConstraint);

                // THEN
                Assert.That(element.Exists, Is.True);
            });
        }

        [Test]
        public void Should_be_able_to_use_escaped_single_quotes_in_selector_string()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var querySelectorConstraint = new QuerySelectorConstraint("input[name=\'textinput1\']");

                // WHEN
                var element = browser.Element(querySelectorConstraint);

                // THEN
                Assert.That(element.Exists, Is.True);
            });
        }

        [Test]
        public void Should_be_able_to_use_double_quotes_in_selector_string()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var querySelectorConstraint = new QuerySelectorConstraint("input[name=\"textinput1\"]");

                // WHEN
                var element = browser.Element(querySelectorConstraint);

                // THEN
                Assert.That(element.Exists, Is.True);
            });
        }

        [Test]
        public void Should_return_selector_string_when_calling_WriteDescriptionTo()
        {
            // GIVEN
            var constraint = new QuerySelectorConstraint(".Return >this");
            var sb = new StringBuilder();

            // WHEN
            constraint.WriteDescriptionTo(new StringWriter(sb));

            // THEN
            Assert.That(sb.ToString(), Is.EqualTo("Selector = '.Return >this'"));
        }

        [Test, Ignore("Bug: Queryselector can't be used to find Frames in IE")]
        public void Should_find_frame()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                browser.GoTo(FramesetURI);

                // WHEN
                var frame = browser.Frame(Find.BySelector(""));

                // THEN
                Assert.That(frame == null);                
            });
        }

        [Test, Ignore("Bug: Queryselector can't be used to find Frames in IE")]
        public void Should_find_element_in_frame()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                browser.GoTo(FramesetURI);
                var frame = browser.Frame("mainid");

                // WHEN
                var link = frame.Link(Find.BySelector("#Microsoft"));

                // THEN
                Assert.That(link.Exists);
                Assert.That(link.Id, Is.EqualTo("Microsoft"));
            });
        }

        [Test, Ignore("Bug: Queryselector not working with Frames in IE")]
        public void Should_find_element_in_frame_within_frame()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                browser.GoTo(FramesetWithinFramesetURI);
                var frame1 = browser.Frame("mainid");
                Console.WriteLine(frame1.FrameElement.GetJavascriptElementReference());
                var frame2 = frame1.Frame("mainid");
                var javascriptElementReference = frame2.FrameElement.GetJavascriptElementReference();
                Console.WriteLine(javascriptElementReference);
                    

                // WHEN
                var link = frame2.Link(Find.BySelector("#Microsoft"));

                // THEN
                Assert.That(link.Exists);                
            });
        }


        public override Uri TestPageUri
        {
            get { return MainURI; }
        }
    }
}
