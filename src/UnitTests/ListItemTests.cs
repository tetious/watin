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
