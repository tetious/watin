#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

namespace WatiN.Core.UnitTests
{
	/// <summary>
	/// Summary description for TableTests.
	/// </summary>
	[TestFixture]
	public class TableTests : BaseWithIETests
	{
		private const string tableId = "table1";

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

		[Test]
		public void TableElementTags()
		{
			Assert.AreEqual(1, Table.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("table", ((ElementTag) Table.ElementTags[0]).TagName);
		}

		[Test]
		public void TableFromElement()
		{
			Element element = ie.Element(tableId);
			Table table = new Table(element);
			Assert.AreEqual(tableId, table.Id);
		}

		[Test]
		public void TableExists()
		{
			Assert.IsTrue(ie.Table(tableId).Exists);
			Assert.IsFalse(ie.Table("nonexistingtableid").Exists);
		}

		[Test]
		public void TableRowGetParentTable()
		{
			TableRow tableRow = ie.TableRow("row0");
			Assert.IsInstanceOfType(typeof (TableBody), tableRow.Parent, "Parent should be a TableBody Type");
			Assert.IsInstanceOfType(typeof (Table), tableRow.ParentTable, "Should be a Table Type");
			Assert.AreEqual("table1", tableRow.ParentTable.Id, "Unexpected id");
		}

		[Test]
		public void TableTest()
		{
			Assert.AreEqual(tableId, ie.Table(Find.ById(tableId)).Id);

			Table table = ie.Table(tableId);
			Assert.AreEqual(tableId, table.Id);
			Assert.AreEqual(tableId, table.ToString());
			Assert.AreEqual(3, table.TableRows.Length, "Unexpected number of rows");

			TableRow row = table.FindRow("a1", 0);
			Assert.IsNotNull(row, "Row with a1 expected");
			Assert.AreEqual("a1", row.TableCells[0].Text, "Unexpected text in cell");


			row = table.FindRow("b2", 1);
			Assert.IsNotNull(row, "Row with b2 expected");
			Assert.AreEqual("b2", row.TableCells[1].Text, "Unexpected text in cell");

			row = table.FindRow("c1", 0);
			Assert.IsNull(row, "No row with c1 expected");
		}

		[Test]
		public void TableFindRowShouldIgnoreTHtagsInTBody()
		{
			Table table = ie.Table(tableId);
			Assert.AreEqual("TH", table.TableRows[0].Elements[0].TagName.ToUpper(), "First tablerow should contain a TH element");

			TableRow row = table.FindRow(new Regex("a"), 0);
			Assert.IsNotNull(row, "row expected");
			Assert.AreEqual("a1", row.TableCells[0].Text);
		}

		[Test]
		public void TableFindRowWithTextIgnoreCase()
		{
			Table table = ie.Table(tableId);

			// test: ignore case of the text to find
			TableRow row = table.FindRow("A2", 1);
			Assert.IsNotNull(row, "Row with a1 expected");
			Assert.AreEqual("a2", row.TableCells[1].Text, "Unexpected text in cell");
		}

		[Test]
		public void TableFindRowWithTextNoPartialMatch()
		{
			Table table = ie.Table(tableId);

			// test: ignore case of the text to find
			TableRow row = table.FindRow("a", 1);
			Assert.IsNull(row, "No row expected");
		}

		[Test]
		public void TableFindRowWithRegex()
		{
			Table table = ie.Table(tableId);

			TableRow row = table.FindRow(new Regex("b"), 1);

			Assert.IsNotNull(row, "Row with b1 expected");
			Assert.AreEqual("b2", row.TableCells[1].Text, "Unexpected text in cell");
		}

		[Test]
		public void TableFindRowShouldIgnoreTableCellsInsideOfNestedTables()
		{
			ie.GoTo(TablesUri);

			Table table = ie.Table("Table1");

			Assert.IsNotNull(table.FindRow("12", 1));
		}

		[Test]
		public void Tables()
		{
			// Collection.length
			TableCollection tables = ie.Tables;

			Assert.AreEqual(2, tables.Length);

			// Collection items by index
			Assert.AreEqual("table1", tables[0].Id);
			Assert.AreEqual("table2", tables[1].Id);

			IEnumerable tableEnumerable = tables;
			IEnumerator tableEnumerator = tableEnumerable.GetEnumerator();

			// Collection iteration and comparing the result with Enumerator
			int count = 0;
			foreach (Table table in tables)
			{
				tableEnumerator.MoveNext();
				object enumTable = tableEnumerator.Current;

				Assert.IsInstanceOfType(table.GetType(), enumTable, "Types are not the same");
				Assert.AreEqual(table.OuterHtml, ((Table) enumTable).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(tableEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(2, count);
		}

#if !NET11
        [Test]
        public void FindTableBodyUsingPredicateT()
        {
            ie.GoTo(TablesUri);
            TableBody tableBody = ie.Table("Table1").TableBody(delegate(TableBody t) { return t.Id == "tbody3"; });

            Assert.That(tableBody.Exists);
        }

        [Test]
        public void FindRowWithNoResultUsingPredicateT()
        {
            Table table = ie.Table(tableId);
            Assert.That(table.Exists);

            TableRow tableRow = table.FindRow(delegate(TableCell c) { return c.Text == "b"; }, 1);
//            TableRow tableRow = table.FindRow(c => c.Text == "b", 1);

            Assert.That(tableRow, Is.Null);
        }

        [Test]
        public void FindRowUsingPredicateT()
        {
            Table table = ie.Table(tableId);
            Assert.That(table.Exists);

            TableRow tableRow = table.FindRow(delegate(TableCell c) { return c.Text == "b2"; }, 1);
//            TableRow tableRow = table.FindRow(c => c.Text == "b2", 1);

            Assert.That(tableRow, Is.Not.Null);
        }

#endif
	}
}