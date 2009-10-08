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
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	/// <summary>
	/// Summary description for TableTests.
	/// </summary>
	[TestFixture]
	public class TableTests : BaseWithBrowserTests
	{
		private const string tableId = "table1";

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

		[Test]
		public void TableElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<Table>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("table", elementTags[0].TagName);
		}

		[Test]
		public void TableExists()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.IsTrue(browser.Table(tableId).Exists);
                                Assert.IsFalse(browser.Table("nonexistingtableid").Exists);
		                    });
		}

		[Test]
		public void TableRowGetParentTable()
		{
		    ExecuteTest(browser =>
		                    {
                                var tableRow = browser.TableRow("row0");
		                        Assert.IsInstanceOfType(typeof (TableBody), tableRow.Parent, "Parent should be a TableBody Type");
		                        Assert.IsInstanceOfType(typeof (Table), tableRow.ContainingTable, "Should be a Table Type");
		                        Assert.AreEqual("table1", tableRow.ParentTable.Id, "Unexpected id");
		                    });
		}

		[Test]
		public void TableTest()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.AreEqual(tableId, browser.Table(Find.ById(tableId)).Id);

                                var table = browser.Table(tableId);
		                        Assert.AreEqual(tableId, table.Id);
		                        Assert.AreEqual(tableId, table.ToString());
		                        Assert.AreEqual(3, table.TableRows.Count, "Unexpected number of rows");

		                        var row = table.FindRow("a1", 0);
		                        Assert.IsNotNull(row, "Row with a1 expected");
		                        Assert.AreEqual("a1", row.TableCells[0].Text, "Unexpected text in cell");

		                        row = table.FindRow("b2", 1);
		                        Assert.IsNotNull(row, "Row with b2 expected");
		                        Assert.AreEqual("b2", row.TableCells[1].Text, "Unexpected text in cell");

		                        row = table.FindRow("c1", 0);
		                        Assert.IsNull(row, "No row with c1 expected");
		                    });
		}

		[Test]
		public void TableFindRowShouldIgnoreTHtagsInTBody()
		{
		    ExecuteTest(browser =>
		                    {
                                var table = browser.Table(tableId);
		                        Assert.AreEqual("TH", table.TableRows[0].Elements[0].TagName.ToUpper(), "First tablerow should contain a TH element");

		                        var row = table.FindRow(new Regex("a"), 0);
		                        Assert.IsNotNull(row, "row expected");
		                        Assert.AreEqual("a1", row.TableCells[0].Text);
		                    });
		}

		[Test]
		public void TableFindRowWithTextIgnoreCase()
		{
		    ExecuteTest(browser =>
		                    {
                                var table = browser.Table(tableId);

		                        // test: ignore case of the text to find
		                        var row = table.FindRow("A2", 1);
		                        Assert.IsNotNull(row, "Row with a2 expected");
                                Assert.That(row.TableCells[1].Text, Is.EqualTo("a2"), "Unexpected text in cell");
		                    });
		}

		[Test]
		public void TableFindRowWithTextNoPartialMatch()
		{
		    ExecuteTest(browser =>
		                    {
                                var table = browser.Table(tableId);

		                        // test: ignore case of the text to find
		                        var row = table.FindRow("a", 1);
		                        Assert.IsNull(row, "No row expected");

		                    });
		}

		[Test]
		public void TableFindRowWithRegex()
		{
		    ExecuteTest(browser =>
		                    {
                                var table = browser.Table(tableId);

		                        var row = table.FindRow(new Regex("b"), 1);

		                        Assert.IsNotNull(row, "Row with b1 expected");
		                        Assert.AreEqual("b2", row.TableCells[1].Text, "Unexpected text in cell");

		                    });
		}

		[Test]
        public void TableRowAttributeConstraintCompareShouldNotCrashIfColumnIndexIsGreaterThenNumberOfColumnsInARow()
		{
		    ExecuteTest(browser =>
		                    {
                                browser.GoTo(TablesUri);

                                var table = browser.Table("Table1");

		                        Assert.That(table.FindRow("12", 999), Is.Null);

		                    });
		}

		[Test]
		public void TableFindRowShouldFindTableCellsInsideOfNestedTables()
		{
		    ExecuteTest(browser =>
		                    {
                                browser.GoTo(TablesUri);

                                var table = browser.Table("Table1");

		                        var row = table.FindRow("2", 0);

		                        Assert.That(row, Is.Not.Null);

		                    });
		}

		[Test]
        public void TableFindRowInOwnTableRowsShouldIgnoreTableCellsInsideOfNestedTables()
		{
		    ExecuteTest(browser =>
		                    {
                                browser.GoTo(TablesUri);

                                var table = browser.Table("Table1");

		                        Assert.That(table.FindRowInOwnTableRows("2", 1), Is.Null);

		                    });
		}

		[Test]
        public void TableFindRowInOwnTableRowsWithPredicateShouldIgnoreTableCellsInsideOfNestedTables()
		{
		    ExecuteTest(browser =>
		                    {
                                browser.GoTo(TablesUri);

                                // GIVEN
                                var table1 = browser.Table("Table1");
                                
                                // WHEN
                                var row = table1.FindRowInOwnTableRows(tableCell => Equals(tableCell.Text, "2"), 1);

                                // THEN
		                        Assert.That(row, Is.Null);

		                    });
		}

		[Test]
		public void Tables()
		{
		    ExecuteTest(browser =>
		                    {
		                        // Collection.length
                                var tables = browser.Tables;

		                        Assert.AreEqual(2, tables.Count);

		                        // Collection items by index
		                        Assert.AreEqual("table1", tables[0].Id);
		                        Assert.AreEqual("table2", tables[1].Id);

		                        IEnumerable tableEnumerable = tables;
		                        var tableEnumerator = tableEnumerable.GetEnumerator();

		                        // Collection iteration and comparing the result with Enumerator
		                        var count = 0;
		                        foreach (var table in tables)
		                        {
		                            tableEnumerator.MoveNext();
		                            var enumTable = tableEnumerator.Current;

		                            Assert.IsInstanceOfType(table.GetType(), enumTable, "Types are not the same");
		                            Assert.AreEqual(table.OuterHtml, ((Table) enumTable).OuterHtml, "foreach and IEnumerator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(tableEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(2, count);

		                    });
		}

        [Test]
        public void OwnTableRowsShouldNotReturnRowsOfNestedTables()
        {
            ExecuteTest(browser =>
                            {
                                browser.GoTo(TablesUri);

                                var table = browser.Table("Table1");
                                Assert.That(table.OwnTableRows.Count, Is.EqualTo(3), "Unexpected number of TableRows");
                                Assert.That(table.OwnTableRows[0].Id, Is.EqualTo("1"), "Unexpected Id");
                                Assert.That(table.OwnTableRows[1].Id, Is.EqualTo("3"), "Unexpected Id");
                                Assert.That(table.OwnTableRows[2].Id, Is.EqualTo("4"), "Unexpected Id");
                            });
        }

        [Test]
        public void FindTableBodyUsingPredicateT()
        {
            ExecuteTest(browser =>
                            {
                                browser.GoTo(TablesUri);
                                var tableBody = browser.Table("Table1").TableBody(t => t.Id == "tbody3");

                                Assert.That(tableBody.Exists);
                            });
        }

        [Test]
        public void FindRowWithNoResultUsingPredicateT()
        {
            ExecuteTest(browser =>
                            {
                                var table = browser.Table(tableId);
                                Assert.That(table.Exists);

                                var tableRow = table.FindRow(c => c.Text == "b", 1);

                                Assert.That(tableRow, Is.Null);
                            });
        }

        [Test]
        public void FindRowUsingPredicateT()
        {
            ExecuteTest(browser =>
                            {
                                var table = browser.Table(tableId);

                                var tableRow = table.FindRow(c => c.Text == "b2", 1);

                                Assert.That(tableRow, Is.Not.Null);
                            });
        }

        [Test]
        public void Header_and_footer_rows_should_be_included()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                browser.GoTo(TablesUri);

                                var table = browser.Table("thead_tbody_tfoot");

                                // WHEN
                                var tableRows = table.OwnTableRows;

                                // THEN
                                Assert.That(tableRows.Count, Is.EqualTo(3));
                            });
        }

	    [Test]
	    public void Should_return_all_own_table_rows_in_bodies()
	    {
            ExecuteTest(browser =>
            {
                // GIVEN
                browser.GoTo(TablesUri);

                var table = browser.Table("Table1");

                // WHEN
                var tableRows = table.OwnTableRows;

                // THEN
                Assert.That(tableRows.Count, Is.EqualTo(3));
                Assert.That(tableRows[0].Id, Is.EqualTo("1"), "Unexpected row 0");
                Assert.That(tableRows[1].Id, Is.EqualTo("3"), "Unexpected row 1");
                Assert.That(tableRows[2].Id, Is.EqualTo("4"), "Unexpected row 2");

            });
	    }

	}
}