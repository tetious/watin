using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class ListTests: BaseWithBrowserTests
    {
        [Test]
        public void Should_find_unordered_list()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                // WHEN
                var ul = browser.List("unorderedlist");

                // THEN
                Assert.That(ul.Exists);
                Assert.That(ul.IsOrdered, Is.False);
            });
        }

        [Test]
        public void Should_find_ordered_list()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                // WHEN
                var ol = browser.List("orderedlist");

                // THEN
                Assert.That(ol.Exists);
                Assert.That(ol.IsOrdered, Is.True);

            });
        }

        [Test]
        public void Should_find_nested_list()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var div = browser.Div("wrapper");

                // WHEN
                var list = div.List("Ul1");

                // THEN
                Assert.That(list.Exists);

            });
        }

        [Test]
        public void Should_find_nested_list_in_list()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var first = browser.List("Ul1");

                // WHEN
                var nestedlist = first.List("Ol1");

                // THEN
                Assert.That(nestedlist.Exists);

            });
        }

        [Test]
        public void Should_find_all_lists_on_the_page()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                // WHEN
                var lists = browser.Lists;

                // THEN
                Assert.That(lists.Count, Is.EqualTo(4));
            });
        }

        #region Overrides of BaseWithBrowserTests

        public override Uri TestPageUri
        {
            get { return ListTestsUri; }
        }

        #endregion
    }
}
