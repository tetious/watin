using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.UnitTests.ResearchTests;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
    [TestFixture]
    public class FindBySelectorTests :BaseWithBrowserTests
    {
        [Test, Ignore("Work in progress")]
        public void Does_this_work()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                browser.RunScript(new ScriptLoader().GetInstallScript());

                                // WHEN
                                var elements = browser.Elements.Filter(new QuerySelector(".input"));

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
                                browser.RunScript(new ScriptLoader().GetInstallScript());

                                // WHEN
                                var element = browser.Element(new QuerySelector("#popupid"));

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
                                browser.RunScript(new ScriptLoader().GetInstallScript());

                                // WHEN
                                var div = browser.Div(new QuerySelector("#popupid"));

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
                browser.RunScript(new ScriptLoader().GetInstallScript());
                var containerElement = browser.Form("Form");
                // WHEN
                var element = containerElement.Element(new QuerySelector("#popupid"));

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
                browser.RunScript(new ScriptLoader().GetInstallScript());
                var containerElement = browser.Form("FormInputElement");
                // WHEN
                var element = containerElement.Element(new QuerySelector("#popupid"));

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
