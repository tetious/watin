#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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

namespace WatiN.Core.UnitTests
{
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;

  [TestFixture]
  public class TableCellTests : BaseElementsTests
  {
    [Test]
    public void TableCellElementTags()
    {
      Assert.AreEqual(1, TableCell.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("td", ((ElementTag) TableCell.ElementTags[0]).TagName);
    }

    [Test]
    public void TableCellFromElement()
    {
      Element element = ie.Element("td1");
      TableCell tableCell = new TableCell(element);
      Assert.AreEqual("td1", tableCell.Id);
    }

    [Test]
    public void TableCellExists()
    {
      Assert.IsTrue(ie.TableCell("td1").Exists);
      Assert.IsTrue(ie.TableCell(new Regex("td1")).Exists);
      Assert.IsFalse(ie.Table("nonexistingtd1").Exists);
    }

    [Test]
    public void TableCellByIndexExists()
    {
      Assert.IsTrue(ie.TableCell("td1", 0).Exists);
      Assert.IsTrue(ie.TableCell(new Regex("td1"), 0).Exists);
      Assert.IsFalse(ie.TableCell("td1", 100).Exists);
    }

    [Test]
    public void TableCellByIndex()
    {
      // accessing several occurences with equal id
      Assert.AreEqual("a1", ie.TableCell("td1", 0).Text);
      Assert.AreEqual("b1", ie.TableCell("td1", 1).Text);
    }

    [Test]
    public void TableCellGetParentTableRow()
    {
      TableCell tableCell = ie.TableCell(Find.ByText("b1"));
      Assert.IsInstanceOfType(typeof (TableRow), tableCell.ParentTableRow, "Should be a TableRow Type");
      Assert.AreEqual("row1", tableCell.ParentTableRow.Id, "Unexpected id");
    }

    [Test]
    public void TableCellCellIndex()
    {
      Assert.AreEqual(0, ie.TableCell(Find.ByText("b1")).Index);
      Assert.AreEqual(1, ie.TableCell(Find.ByText("b2")).Index);
    }

    [Test]
    public void TableCells()
    {
      // Collection.Length
      TableCellCollection cells = ie.Table("table1").TableRows[1].TableCells;

      Assert.AreEqual(2, cells.Length);

      // Collection items by index
      Assert.AreEqual("td1", cells[0].Id);
      Assert.AreEqual("td2", cells[1].Id);

      IEnumerable cellEnumerable = cells;
      IEnumerator cellEnumerator = cellEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (TableCell cell in cells)
      {
        cellEnumerator.MoveNext();
        object enumTable = cellEnumerator.Current;

        Assert.IsInstanceOfType(cell.GetType(), enumTable, "Types are not the same");
        Assert.AreEqual(cell.OuterHtml, ((TableCell) enumTable).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }

      Assert.IsFalse(cellEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(2, count);
    }
  }
}