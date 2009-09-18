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
using System.Collections.Generic;
using WatiN.Core.Native;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class TableCellTests : BaseWithBrowserTests
	{
		[Test]
		public void TableCellElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<TableCell>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("td", elementTags[0].TagName);
		}

		[Test]
		public void TableCellExists()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.IsTrue(browser.TableCell("td1").Exists);
                                Assert.IsTrue(browser.TableCell(new Regex("td1")).Exists);
                                Assert.IsFalse(browser.Table("nonexistingtd1").Exists);
		                    });
		}

		[Test]
		public void TableCellByIndexExists()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.IsTrue(browser.TableCell("td1", 0).Exists);
                                Assert.IsTrue(browser.TableCell(new Regex("td1"), 0).Exists);
                                Assert.IsFalse(browser.TableCell("td1", 100).Exists);
		                    });
		}

		[Test]
		public void TableCellByIndex()
		{
		    ExecuteTest(browser =>
		                    {
		                        // accessing several occurences with equal id
                                Assert.AreEqual("a1", browser.TableCell("td1", 0).Text);
                                Assert.AreEqual("b1", browser.TableCell("td1", 1).Text);
		                    });
		}

		[Test]
		public void TableCellGetParentTableRow()
		{
		    ExecuteTest(browser =>
		                    {
                                var tableCell = browser.TableCell(Find.ByText("b1"));
		                        Assert.IsInstanceOfType(typeof (TableRow), tableCell.ContainingTableRow, "Should be a TableRow Type");
		                        Assert.AreEqual("row1", tableCell.ParentTableRow.Id, "Unexpected id");
		                    });
		}

		[Test]
		public void TableCellCellIndex()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.AreEqual(0, browser.TableCell(Find.ByText("b1")).Index);
                                Assert.AreEqual(1, browser.TableCell(Find.ByText("b2")).Index);
		                    });
		}

		[Test]
		public void TableCells()
		{
		    ExecuteTest(browser =>
		                    {
		                        // Collection.Length
                                var cells = browser.Table("table1").TableRows[1].TableCells;

		                        Assert.AreEqual(2, cells.Count);

		                        // Collection items by index
		                        Assert.AreEqual("td1", cells[0].Id);
		                        Assert.AreEqual("td2", cells[1].Id);

		                        IEnumerable cellEnumerable = cells;
		                        var cellEnumerator = cellEnumerable.GetEnumerator();

		                        // Collection iteration and comparing the result with Enumerator
		                        var count = 0;
		                        foreach (var cell in cells)
		                        {
		                            cellEnumerator.MoveNext();
		                            var enumTable = cellEnumerator.Current;

		                            Assert.IsInstanceOfType(cell.GetType(), enumTable, "Types are not the same");
		                            Assert.AreEqual(cell.OuterHtml, ((TableCell) enumTable).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(cellEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(2, count);
		                    });
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
    }
}