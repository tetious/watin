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
using WatiN.Core.Logging;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class ListTests: BaseWithBrowserTests
    {
        public override void FixtureSetup()
        {
            base.FixtureSetup();
            Logger.LogWriter = new ConsoleLogWriter { HandlesLogDebug = true };
        }

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

        [Test]
        public void Should_return_own_listitem()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var list = browser.List("Ul1");

                // WHEN
                var listItem = list.OwnListItem("ul1_second");

                // THEN
                Assert.That(listItem.Exists);
            });
        }
        [Test]
        public void Should_not_return_listitem_from_contained_list_as_own_list_item()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var list = browser.List("Ul1");

                // WHEN
                var listItem = list.OwnListItem("ol1_first");

                // THEN
                Assert.That(listItem.Exists, Is.False);
            });
        }

        [Test]
        public void Should_return_own_listitems_only_and_none_from_contained_list()
        {
            ExecuteTest(browser =>
            {
                // GIVEN
                var list = browser.List("Ul1");

                // WHEN
                var listItems = list.OwnListItems;

                // THEN
                Assert.That(listItems.Count, Is.EqualTo(2));
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
