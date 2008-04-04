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
using mshtml;
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
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

		public Table(DomContainer domContainer, IHTMLTable htmlTable) : base(domContainer, (IHTMLElement) htmlTable) {}

		public Table(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

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

#if !NET11
        /// <summary>
		/// Returns the table body section belonging to this table (not including table body sections 
		/// from tables nested in this table).
		/// </summary>
		/// <param name="predicate">The expression to use.</param>
		/// <returns></returns>
		public override TableBody TableBody(Predicate<TableBody> predicate)
		{
			return TableBody(Find.ByElement(predicate));
		}
#endif

		private IHTMLElement GetFirstTBody()
		{
			return (IHTMLElement) HTMLTable.tBodies.item(0, null);
		}

		private IHTMLTable HTMLTable
		{
			get { return (IHTMLTable) HTMLElement; }
		}

		/// <summary>
        /// Finds te first row that has an exact match with <paramref name="findText"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, null is returned.
		/// </summary>
		/// <param name="findText">The text to find.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
		public TableRow FindRow(string findText, int inColumn)
		{
			Logger.LogAction("Searching for '" + findText + "' in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

			TableRowAttributeConstraint constraint = new TableRowAttributeConstraint(findText, inColumn);

            if (TextIsInBody(constraint))
			{
                return FindRow(constraint);
			}
		    return null;
		}

        private bool TextIsInBody(TableRowAttributeConstraint attributeConstraint)
        {
            string innertext = GetFirstTBody().innerText;

            return (innertext != null && attributeConstraint.IsTextContainedIn(innertext));
        }
        
        /// <summary>
        /// Finds te first row that matches <paramref name="findTextRegex"/> in <paramref name="inColumn"/>
        /// defined as a TD html element. If no match is found, <c>null</c> is returned.
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

		/// <summary>
        /// Finds te first row that matches <paramref name="comparer"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned.
		/// </summary>
		/// <param name="comparer">The comparer that the cell text must match.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
		public TableRow FindRow(ICompare comparer, int inColumn)
		{
			Logger.LogAction("Matching comparer'" + comparer + "' with text in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

			TableRowAttributeConstraint constraint = new TableRowAttributeConstraint(comparer, inColumn);

			return FindRow(constraint);
        }

#if !NET11
        /// <summary>
        /// Finds te first row that matches <paramref name="predicate"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned.
        /// </summary>
        /// <param name="predicate">The predicate that the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRow(Predicate<string> predicate, int inColumn)
        {
            return FindRow(new PredicateComparer(predicate), inColumn);
        }
#endif

		public override string ToString()
		{
			return Id;
		}

        /// <summary>
		/// Finds the first row that meets the <see cref="TableRowAttributeConstraint"/>.
		/// If no match is found, <c>null</c> is returned.
		/// </summary>
		/// <param name="findBy">The constraint used to identify the table cell.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
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

		internal new static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new Table(domContainer, (IHTMLTable) element);
		}
	}
}