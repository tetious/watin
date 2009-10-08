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

using System;
using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Comparers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class TableRowTests : BaseWithBrowserTests
	{
		[Test]
		public void TableRowElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<TableRow>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("tr", elementTags[0].TagName);
		}

		[Test]
		public void TableRowExists()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.IsTrue(browser.TableRow("row0").Exists);
                                Assert.IsTrue(browser.TableRow(new Regex("row0")).Exists);
                                Assert.IsFalse(browser.TableRow("nonexistingtr").Exists);
		                    });
		}

		[Test]
		public void TableRows()
		{
		    ExecuteTest(browser =>
		                    {
		                        // Collection.Length
                                var rows = browser.Table("table1").TableRows;

		                        Assert.AreEqual(3, rows.Count);

		                        // Collection items by index
		                        Assert.AreEqual("row0", rows[1].Id);
		                        Assert.AreEqual("row1", rows[2].Id);

		                        IEnumerable rowEnumerable = rows;
		                        var rowEnumerator = rowEnumerable.GetEnumerator();

		                        // Collection iteration and comparing the result with Enumerator
		                        var count = 0;
		                        foreach (var row in rows)
		                        {
		                            rowEnumerator.MoveNext();
		                            var enumTable = rowEnumerator.Current;

		                            Assert.IsInstanceOfType(row.GetType(), enumTable, "Types are not the same");
		                            Assert.AreEqual(row.OuterHtml, ((TableRow) enumTable).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(rowEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(3, count);
		                    });
		}

		[Test]
		public void TableRowRowIndex()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.AreEqual(1, browser.TableRow(Find.ByText(new StringContainsAndCaseInsensitiveComparer("a1"))).Index);
                                Assert.AreEqual(2, browser.TableRow(Find.ByText(new StringContainsAndCaseInsensitiveComparer("b2"))).Index);
		                    });
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

	    [Test]
	    public void TableRowDirectChildren()
	    {
	        ExecuteTest(browser =>
	                        {
                                browser.GoTo(TablesUri);

                                var tableRow = browser.TableRow("1");

	                            Assert.That(tableRow.TableCellsDirectChildren.Count, Is.EqualTo(2));
	                            Assert.That(tableRow.TableCellsDirectChildren[0].Id, Is.EqualTo("td1"));
	                            Assert.That(tableRow.TableCellsDirectChildren[1].Id, Is.EqualTo("td12"));
	                        });
	    }

	    [Test]
	    public void Should_filter_thead_rows()
	    {
            ExecuteTest(browser =>
            {
                // GIVEN
                browser.GoTo(TablesUri);

                var tableRows = browser.Table("thead_tbody_tfoot").OwnTableRows;

                // WHEN
                var noHeaderRows = tableRows.Filter(TableRow.IsHeaderRow());

                // THEN
                Assert.That(noHeaderRows.Count, Is.EqualTo(1));
                Assert.That(noHeaderRows[0].Id, Is.EqualTo("head_row"));
            });
        }

	    [Test]
	    public void Should_filter_tfoot_rows()
	    {
            ExecuteTest(browser =>
            {
                // GIVEN
                browser.GoTo(TablesUri);

                var tableRows = browser.Table("thead_tbody_tfoot").OwnTableRows;

                // WHEN
                var noHeaderRows = tableRows.Filter(TableRow.IsFooterRow());

                // THEN
                Assert.That(noHeaderRows.Count, Is.EqualTo(1));
                Assert.That(noHeaderRows[0].Id, Is.EqualTo("foot_row"));
            });
        }
	}
}