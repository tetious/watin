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

using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace WatiN.Core.UnitTests
{
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