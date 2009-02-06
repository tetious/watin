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
using NUnit.Framework;

namespace WatiN.Core.UnitTests
{
	/// <summary>
	/// Summary description for TableBodyTests.
	/// </summary>
	[TestFixture]
	public class TableBodyTests : BaseWithBrowserTests 
	{
		public override Uri TestPageUri
		{
			get { return TablesUri; }
		}

		[Test]
		public void TableTableBodiesExcludesBodiesFromNestedTables()
		{
			TableBodyCollection tableBodies = Ie.Table("Table1").TableBodies;
			Assert.AreEqual(2, tableBodies.Length, "Unexpected number of tbodies");
			Assert.AreEqual("tbody1", tableBodies[0].Id, "Unexpected tbody[0].id");
			Assert.AreEqual("tbody3", tableBodies[1].Id, "Unexpected tbody[1].id");
		}

		[Test]
		public void TableBodyExcludesRowsFromNestedTables()
		{
			TableBody tableBody = Ie.Table("Table1").TableBodies[0];

			Assert.AreEqual(1, tableBody.Tables.Length, "Expected nested table");
			Assert.AreEqual(2, tableBody.TableRows.Length, "Expected 2 rows");
			Assert.AreEqual("1", tableBody.TableRows[0].Id, "Unexpected tablerows[0].id");
			Assert.AreEqual("3", tableBody.TableRows[1].Id, "Unexpected tablerows[1].id");
		}

        [Test]
        public void FindTableRowUsingPredicateT()
        {
            TableRow tableRow = Ie.Table("Table2").TableBody("tbody2").TableRow(
                delegate(TableRow r) { return r.Id == "2"; });

            Assert.That(tableRow.Exists);
        }
	}
}
