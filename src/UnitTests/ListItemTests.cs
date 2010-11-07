using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class ListItemTests : BaseWithBrowserTests
    {
        [Test]
        public void Should_find_listitem_by_id()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                // WHEN
                var listitem = browser.ListItem("secondLi");

                // THEN
                Assert.That(listitem.Exists);
            });
        }

        [Test]
        public void Should_find_all_listitems()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                // WHEN
                var listItems = browser.ListItems;

                // THEN
                Assert.That(listItems.Count, Is.EqualTo(8));
            });
        }

        [Test]
        public void Should_find_listitem_by_id_inside_list()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var list = browser.List("Ul1");

                // WHEN
                var listitem = browser.ListItem(Find.ByText("one"));

                // THEN
                Assert.That(listitem.Exists);
            });
        }

        [Test]
        public void Should_find_all_listitems_inside_list_with_nested_list()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var list = browser.List("Ul1");

                // WHEN
                var listItems = list.ListItems;

                // THEN
                Assert.That(listItems.Count, Is.EqualTo(4));
            });
        }

        public override Uri TestPageUri
        {
            get { return ListTestsUri; }
        }
    }
}
