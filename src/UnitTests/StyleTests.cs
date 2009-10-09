using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class StyleTests : BaseWithBrowserTests
    {
        [Test]
        public void Should_return_style_set_by_a_style_sheet()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var divWithExternalStyleApplied = browser.Div("logo");

                // WHEN
                var backgroundUrl = divWithExternalStyleApplied.Style.GetAttributeValue("BACKGROUND-IMAGE");

                // THEN
                Assert.That(backgroundUrl, Text.Contains("watin.jpg"));
            });
        }

        public override Uri TestPageUri
        {
            get { return StyleTestUri; }
        }
    }
}
