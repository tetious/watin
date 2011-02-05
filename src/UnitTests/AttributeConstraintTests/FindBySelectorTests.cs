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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.UnitTests.TestUtils;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
    [TestFixture]
    public class FindBySelectorTests :BaseWithBrowserTests
    {
        [Test]
        public void Does_this_work()
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

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }
    }
}
