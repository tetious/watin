namespace WatiN.Core.UnitTests
{
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;

  [TestFixture]
  public class TableRowTests : BaseElementsTests
  {
    [Test]
    public void TableRowElementTags()
    {
      Assert.AreEqual(1, TableRow.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("tr", ((ElementTag) TableRow.ElementTags[0]).TagName);
    }

    [Test]
    public void TableRowFromElement()
    {
      Element element = ie.Element("row0");
      TableRow tableRow = new TableRow(element);
      Assert.AreEqual("row0", tableRow.Id);
    }

    [Test]
    public void TableRowExists()
    {
      Assert.IsTrue(ie.TableRow("row0").Exists);
      Assert.IsTrue(ie.TableRow(new Regex("row0")).Exists);
      Assert.IsFalse(ie.TableRow("nonexistingtr").Exists);
    }

    [Test]
    public void TableRows()
    {
      // Collection.Length
      TableRowCollection rows = ie.Table("table1").TableRows;

      Assert.AreEqual(3, rows.Length);

      // Collection items by index
      Assert.AreEqual("row0", rows[1].Id);
      Assert.AreEqual("row1", rows[2].Id);

      IEnumerable rowEnumerable = rows;
      IEnumerator rowEnumerator = rowEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (TableRow row in rows)
      {
        rowEnumerator.MoveNext();
        object enumTable = rowEnumerator.Current;

        Assert.IsInstanceOfType(row.GetType(), enumTable, "Types are not the same");
        Assert.AreEqual(row.OuterHtml, ((TableRow) enumTable).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }

      Assert.IsFalse(rowEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(3, count);
    }

    [Test]
    public void TableRowRowIndex()
    {
      Assert.AreEqual(1, ie.TableRow(Find.ByText("a1")).Index);
      Assert.AreEqual(2, ie.TableRow(Find.ByText("b2")).Index);
    }
  }
}