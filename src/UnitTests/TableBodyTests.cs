using System;
using NUnit.Framework;

namespace WatiN.Core.UnitTests
{
	/// <summary>
	/// Summary description for TableBodyTests.
	/// </summary>
	[TestFixture]
	public class TableBodyTests : BaseWithIETests 
	{
		public override Uri TestPageUri
		{
			get { return TablesUri; }
		}

		[Test]
		public void TableTableBodiesExcludesBodiesFromNestedTables()
		{
			TableBodyCollection tableBodies = ie.Table("Table1").TableBodies;
			Assert.AreEqual(2, tableBodies.Length, "Unexpected number of tbodies");
			Assert.AreEqual("tbody1", tableBodies[0].Id, "Unexpected tbody[0].id");
			Assert.AreEqual("tbody3", tableBodies[1].Id, "Unexpected tbody[1].id");
		}

		[Test]
		public void TableBodyExcludesRowsFromNestedTables()
		{
			TableBody tableBody = ie.Table("Table1").TableBodies[0];

			Assert.AreEqual(1, tableBody.Tables.Length, "Expected nested table");
			Assert.AreEqual(2, tableBody.TableRows.Length, "Expected 2 rows");
			Assert.AreEqual("1", tableBody.TableRows[0].Id, "Unexpected tablerows[0].id");
			Assert.AreEqual("3", tableBody.TableRows[1].Id, "Unexpected tablerows[1].id");
		}

#if !NET11
        [Test]
        public void FindTableRowUsingPredicateT()
        {
            TableRow tableRow = ie.Table("Table2").TableBody("tbody2").TableRow(
                delegate(TableRow r) { return r.Id == "2"; });

            Assert.That(tableRow.Exists);
        }
#endif

	}
}
