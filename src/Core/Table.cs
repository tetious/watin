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
using mshtml;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML table element.
	/// </summary>
	public class Table : ElementsContainer
	{
		private static ArrayList elementTags;

		public static ArrayList ElementTags
		{
			get
			{
				if (elementTags == null)
				{
					elementTags = new ArrayList();
					elementTags.Add(new ElementTag("table"));
				}

				return elementTags;
			}
		}

		public Table(DomContainer ie, IHTMLTable htmlTable) : base(ie, (IHTMLElement) htmlTable) {}

		public Table(DomContainer ie, ElementFinder finder) : base(ie, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="Table"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public Table(Element element) : base(element, ElementTags) {}

		/// <summary>
		/// Returns all rows in the first TBODY section of a table. If no
		/// explicit sections are defined in the table (like THEAD, TBODY 
		/// and/or TFOOT sections), it will return all the rows in the table.
		/// This method also returns rows from nested tables.
		/// </summary>
		/// <value>The table rows.</value>
		public override TableRowCollection TableRows
		{
			get { return ElementsSupport.TableRows(DomContainer, TableBodies[0]); }
		}

		/// <summary>
		/// Returns the table body sections belonging to this table (not including table body sections 
		/// from tables nested in this table).
		/// </summary>
		/// <value>The table bodies.</value>
		public override TableBodyCollection TableBodies
		{
			get { return new TableBodyCollection(DomContainer, UtilityClass.IHtmlElementCollectionToArrayList(HTMLTable.tBodies)); }
		}

		/// <summary>
		/// Returns the table body section belonging to this table (not including table body sections 
		/// from tables nested in this table).
		/// </summary>
		/// <param name="findBy">The find by.</param>
		/// <returns></returns>
		public override TableBody TableBody(BaseConstraint findBy)
		{
			return ElementsSupport.TableBody(DomContainer, findBy, new TBodies(this));
		}

		private IHTMLElement GetFirstTBody()
		{
			return (IHTMLElement) HTMLTable.tBodies.item(0, null);
		}

		private IHTMLTable HTMLTable
		{
			get { return (IHTMLTable) HTMLElement; }
		}

		/// <summary>
		/// Finds te first row that matches findText in inColumn defined as a TD html element.
		/// If no match is found, null is returned.
		/// </summary>
		/// <param name="findText">The text to find.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
		public TableRow FindRow(string findText, int inColumn)
		{
			Logger.LogAction("Searching for '" + findText + "' in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

			TableRowAttributeConstraint constraint = new TableRowAttributeConstraint(findText, inColumn);

			return findRow(constraint);
		}

		/// <summary>
		/// Finds te first row that matches findTextRegex in inColumn defined as a TD html element.
		/// If no match is found, null is returned.
		/// </summary>
		/// <param name="findTextRegex">The regular expression the cell text must match.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
		public TableRow FindRow(Regex findTextRegex, int inColumn)
		{
			Logger.LogAction("Matching regular expression'" + findTextRegex + "' with text in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

			TableRowAttributeConstraint constraint = new TableRowAttributeConstraint(findTextRegex, inColumn);

			return FindRow(constraint);
		}

		private TableRow findRow(TableRowAttributeConstraint attributeConstraint)
		{
			string innertext = GetFirstTBody().innerText;

			if (innertext != null && attributeConstraint.IsTextContainedIn(innertext))
			{
				return FindRow(attributeConstraint);
			}

			return null;
		}

		public override string ToString()
		{
			return Id;
		}

		public TableRow FindRow(TableRowAttributeConstraint findBy)
		{
			TableRow row = ElementsSupport.TableRow(DomContainer, findBy, new ElementsInFirstTBody(this));

			if (row.Exists)
			{
				return row;
			}

			return null;
		}

		public abstract class TableElementCollectionsBase : IElementCollection
		{
			protected Table table;

			public TableElementCollectionsBase(Table table)
			{
				this.table = table;
			}

			public abstract IHTMLElementCollection Elements { get; }
		}

		public class TBodies : TableElementCollectionsBase
		{
			public TBodies(Table table) : base(table) {}

			public override IHTMLElementCollection Elements
			{
				get { return table.HTMLTable.tBodies; }
			}
		}

		public class ElementsInFirstTBody : TableElementCollectionsBase
		{
			public ElementsInFirstTBody(Table table) : base(table) {}

			public override IHTMLElementCollection Elements
			{
				get { return (IHTMLElementCollection) table.GetFirstTBody().all; }
			}
		}
	}
}