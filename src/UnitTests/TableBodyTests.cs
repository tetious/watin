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
using WatiN.Core.UnitTests.TestUtils;

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
        public void TableElementTags()
        {
            var elementTags = ElementFactory.GetElementTags<TableBody>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
            Assert.AreEqual("tbody", elementTags[0].TagName);
        }

		[Test]
		public void OwnTableBodiesExcludesBodiesFromNestedTables()
		{
		    ExecuteTest(browser =>
		                    {
                                var tableBodies = browser.Table("Table1").OwnTableBodies;
		                        Assert.AreEqual(2, tableBodies.Count, "Unexpected number of tbodies");
		                        Assert.AreEqual("tbody1", tableBodies[0].Id, "Unexpected tbody[0].id");
		                        Assert.AreEqual("tbody3", tableBodies[1].Id, "Unexpected tbody[1].id");
		                    });
		}

		[Test]
		public void OwnTableBodiesExcludesRowsFromNestedTables()
		{
		    ExecuteTest(browser =>
		                    {
                                var tableBody = browser.Table("Table1").OwnTableBodies[0];

		                        Assert.AreEqual(1, tableBody.Tables.Count, "Expected nested table");
		                        Assert.AreEqual(2, tableBody.OwnTableRows.Count, "Expected 2 rows");
                                Assert.AreEqual("1", tableBody.OwnTableRows[0].Id, "Unexpected tablerows[0].id");
                                Assert.AreEqual("3", tableBody.OwnTableRows[1].Id, "Unexpected tablerows[1].id");
		                    });
		}

        [Test]
        public void FindTableRowUsingPredicateT()
        {
            ExecuteTest(browser =>
                            {
                                var tableRow = browser.Table("Table2").TableBody("tbody2").TableRow(r => r.Id == "2");

                                Assert.That(tableRow.Exists);

                            });
        }
	}
}
