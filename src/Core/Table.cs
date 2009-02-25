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
using System.Text.RegularExpressions;
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML table element.
	/// </summary>
    [ElementTag("table")]
	public sealed class Table : ElementsContainer<Table>
	{
		public Table(DomContainer domContainer, INativeElement htmlTable) : 
            base(domContainer, htmlTable) {}

        public Table(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

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
        /// Gets the table rows that are direct children of this <see cref="Table"/>, leaving
        /// out table rows of any nested tables within this <see cref="Table"/>. If the <see cref="Table"/>
        /// has multiple <see cref="Core.TableBody"/> elements, the <see cref="TableRowCollection"/> contains 
        /// all the rows of these <see cref="Core.TableBody"/> elements.
        /// </summary>
        /// <value>The table rows collection.</value>
        public TableRowCollection TableRowsDirectChildren
        {
            get { return new TableRowCollection(DomContainer, NativeElement.TableRows(DomContainer)); }
        }

	    /// <summary>
		/// Returns the table body sections belonging to this table (not including table body sections 
		/// from tables nested in this table).
		/// </summary>
		/// <value>The table bodies.</value>
		public override TableBodyCollection TableBodies
		{
            get { return new TableBodyCollection(DomContainer, NativeElement.TableBodies(DomContainer)); }
		}

		/// <summary>
		/// Returns the table body section belonging to this table (not including table body sections 
		/// from tables nested in this table).
		/// </summary>
		/// <param name="findBy">The find by.</param>
		/// <returns></returns>
		public override TableBody TableBody(BaseConstraint findBy)
		{
			return ElementsSupport.TableBody(DomContainer, NativeElement.TableBodies(DomContainer).Filter(findBy));
		}

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

		/// <summary>
        /// Finds te first row that has an exact match with <paramref name="findText"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, null is returned. This method will look for rows in the
        /// first <see cref="Core.TableBody"/> including rows in nested tables.
		/// </summary>
		/// <param name="findText">The text to find.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
		public TableRow FindRow(string findText, int inColumn)
		{
			Logger.LogAction("Searching for '" + findText + "' in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

			var constraint = new TableRowAttributeConstraint(findText, inColumn);

            return TextIsInBody(constraint) ? FindRow(constraint) : null;
		}

		/// <summary>
        /// Finds te first row that has an exact match with <paramref name="findText"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, null is returned. This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
		/// </summary>
		/// <param name="findText">The text to find.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRowInDirectChildren(string findText, int inColumn)
		{
			Logger.LogAction("Searching for '" + findText + "' in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

			var constraint = new TableRowAttributeConstraint(findText, inColumn);

            return TextIsInBody(constraint) ? FindRowInDirectChildren(constraint) : null;
		}

        private bool TextIsInBody(TableRowAttributeConstraint attributeConstraint)
        {
            var innertext = TableBody(Find.First()).Text;

            return (innertext != null && attributeConstraint.IsTextContainedIn(innertext));
        }
        
        /// <summary>
        /// Finds te first row that matches <paramref name="findTextRegex"/> in <paramref name="inColumn"/>
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in the
        /// first <see cref="Core.TableBody"/> including rows in nested tables.
		/// </summary>
		/// <param name="findTextRegex">The regular expression the cell text must match.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
		public TableRow FindRow(Regex findTextRegex, int inColumn)
		{
            Logger.LogAction("Matching regular expression'{0}' with text in column {1} of {2} '{3}'", findTextRegex, inColumn, GetType().Name, Id);

			var constraint = new TableRowAttributeConstraint(findTextRegex, inColumn);

			return FindRow(constraint);
		}

        /// <summary>
        /// Finds te first row that matches <paramref name="findTextRegex"/> in <paramref name="inColumn"/>
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
		/// </summary>
		/// <param name="findTextRegex">The regular expression the cell text must match.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
		public TableRow FindRowInDirectChildren(Regex findTextRegex, int inColumn)
		{
            Logger.LogAction("Matching regular expression'{0}' with text in column {1} of {2} '{3}'", findTextRegex, inColumn, GetType().Name, Id);

			var constraint = new TableRowAttributeConstraint(findTextRegex, inColumn);

			return FindRowInDirectChildren(constraint);
		}

		/// <summary>
        /// Finds te first row that matches <paramref name="comparer"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in the
        /// first <see cref="Core.TableBody"/> including rows in nested tables.
		/// </summary>
		/// <param name="comparer">The comparer that the cell text must match.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
		public TableRow FindRow(ICompare comparer, int inColumn)
		{
			Logger.LogAction("Matching comparer'{0}' with text in column {1} of {2} '{3}'", comparer, inColumn, GetType().Name, Id);

			var constraint = new TableRowAttributeConstraint(comparer, inColumn);

			return FindRow(constraint);
        }

		/// <summary>
        /// Finds te first row that matches <paramref name="comparer"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
		/// </summary>
		/// <param name="comparer">The comparer that the cell text must match.</param>
		/// <param name="inColumn">Index of the column to find the text in.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRowInDirectChildren(ICompare comparer, int inColumn)
		{
            Logger.LogAction("Matching comparer'{0}' with text in column {1} of {2} '{3}'", comparer, inColumn, GetType().Name, Id);

			var constraint = new TableRowAttributeConstraint(comparer, inColumn);

            return FindRowInDirectChildren(constraint);
        }

        /// <summary>
        /// Finds te first row that matches <paramref name="predicate"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in the
        /// first <see cref="Core.TableBody"/> including rows in nested tables.
        /// </summary>
        /// <param name="predicate">The predicate that the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRow(Predicate<string> predicate, int inColumn)
        {
            return FindRow(new PredicateStringComparer(predicate), inColumn);
        }

        /// <summary>
        /// Finds te first row that matches <paramref name="predicate"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
        /// </summary>
        /// <param name="predicate">The predicate that the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRowInDirectChildren(Predicate<string> predicate, int inColumn)
        {
            return FindRowInDirectChildren(new PredicateStringComparer(predicate), inColumn);
        }

        /// <summary>
        /// Finds te first row that matches <paramref name="predicate"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in the
        /// first <see cref="Core.TableBody"/> including rows in nested tables.
        /// </summary>
        /// <param name="predicate">The predicate that the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRow(Predicate<TableCell> predicate, int inColumn)
        {
            return FindRow(new PredicateElementComparer<TableCell>(predicate), inColumn);
        }

        /// <summary>
        /// Finds te first row that matches <paramref name="predicate"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned.  This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
        /// </summary>
        /// <param name="predicate">The predicate that the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRowInDirectChildren(Predicate<TableCell> predicate, int inColumn)
        {
            return FindRowInDirectChildren(new PredicateElementComparer<TableCell>(predicate), inColumn);
        }

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
			var row = ElementsSupport.TableRow(DomContainer, findBy, this);
			return row.Exists ? row : null;
		}

        /// <summary>
		/// Finds the first row that meets the <see cref="TableRowAttributeConstraint"/>.
		/// If no match is found, <c>null</c> is returned.
		/// </summary>
		/// <param name="findBy">The constraint used to identify the table cell.</param>
		/// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
		public TableRow FindRowInDirectChildren(TableRowAttributeConstraint findBy)
		{
            var rowsFinder = NativeElement.TableRows(DomContainer);
            return (TableRow) rowsFinder.Filter(findBy).FindFirst();
		}
	}
}
