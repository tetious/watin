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
using WatiN.Core.Logging;
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML table element.
	/// </summary>
    /// <remarks>
    /// <para>
    /// To find rows that contain particular cell values use the <see cref="Find.ByTextInColumn(string, int)"/>
    /// constraint as in the following example:
    /// </para>
    /// <code>
    /// // Find a table row with "some text" in the 3rd (!) column.
    /// Table table = document.Table("my_table");
    /// table.OwnTableRow(Find.ByTextInColumn("some text", 2));
    /// </code>
    /// <para>
    /// To find rows based on other properties of their contents use the <see cref="Find.ByExistenceOfRelatedElement{T}" />
    /// constraint as in the following example:
    /// </para>
    /// <code>
    /// // Find a table row with "some text" in any of its columns.
    /// Table table = document.Table("my_table");
    /// table.OwnTableRow(Find.ByExistenceOfRelatedElement(row => row.OwnTableCell("some text")));
    /// </code>
    /// </remarks>
    [ElementTag("table")]
	public class Table : ElementContainer<Table>
	{
		public Table(DomContainer domContainer, INativeElement htmlTable) : 
            base(domContainer, htmlTable) {}

        public Table(DomContainer domContainer, ElementFinder finder)
            : base(domContainer, finder) { }

        /// <summary>
        /// Finds a table row within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id</param>
        /// <returns>The table row</returns>
        public TableRow OwnTableRow(string elementId)
        {
            return OwnTableRow(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table row within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id regular expression</param>
        /// <returns>The table row</returns>
        public TableRow OwnTableRow(Regex elementId)
        {
            return OwnTableRow(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table row within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="findBy">The constraint</param>
        /// <returns>The table row</returns>
        public TableRow OwnTableRow(Constraint findBy)
        {
            return new TableRow(DomContainer, CreateElementFinder<TableRow>(NativeElement.TableRows, findBy));
        }

        /// <summary>
        /// Finds a table row within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>The table row</returns>
        public TableRow OwnTableRow(Predicate<TableRow> predicate)
        {
            return OwnTableRow(Find.ByElement(predicate));
        }

        /// <summary>
        /// Gets a collection of all table rows within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <returns>The table row collection</returns>
        public TableRowCollection OwnTableRows
        {
            get { return new TableRowCollection(DomContainer, CreateElementFinder<TableRow>(NativeElement.TableRows, null)); }
        }

        /// <summary>
        /// Finds a table body within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id</param>
        /// <returns>The table body</returns>
        public TableBody OwnTableBody(string elementId)
        {
            return OwnTableBody(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table body within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id regular expression</param>
        /// <returns>The table body</returns>
        public TableBody OwnTableBody(Regex elementId)
        {
            return OwnTableBody(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table body within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="findBy">The constraint</param>
        /// <returns>The table body</returns>
        public TableBody OwnTableBody(Constraint findBy)
        {
            return new TableBody(DomContainer, CreateElementFinder<TableBody>(NativeElement.TableBodies, findBy));
        }

        /// <summary>
        /// Finds a table body within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>The table body</returns>
        public TableBody OwnTableBody(Predicate<TableBody> predicate)
        {
            return OwnTableBody(Find.ByElement(predicate));
        }

        /// <summary>
        /// Gets a collection of all table bodies within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <returns>The table body collection</returns>
        public TableBodyCollection OwnTableBodies
        {
            get { return new TableBodyCollection(DomContainer, CreateElementFinder<TableBody>(NativeElement.TableBodies, null)); }
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
            return TableRow(Find.ByTextInColumn(findText, inColumn));
        }

        /// <summary>
        /// Finds te first row that has an exact match with <paramref name="findText"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, null is returned. This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
        /// </summary>
        /// <param name="findText">The text to find.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRowInOwnTableRows(string findText, int inColumn)
        {
            Logger.LogAction("Searching for '" + findText + "' in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");
            return OwnTableRow(Find.ByTextInColumn(findText, inColumn));
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
            return TableRow(Find.ByTextInColumn(findTextRegex, inColumn));
        }

        /// <summary>
        /// Finds te first row that matches <paramref name="findTextRegex"/> in <paramref name="inColumn"/>
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
        /// </summary>
        /// <param name="findTextRegex">The regular expression the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRowInOwnTableRows(Regex findTextRegex, int inColumn)
        {
            Logger.LogAction("Matching regular expression'{0}' with text in column {1} of {2} '{3}'", findTextRegex, inColumn, GetType().Name, Id);
            return OwnTableRow(Find.ByTextInColumn(findTextRegex, inColumn));
        }

        /// <summary>
        /// Finds te first row that matches <paramref name="comparer"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in the
        /// first <see cref="Core.TableBody"/> including rows in nested tables.
        /// </summary>
        /// <param name="comparer">The comparer that the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRow(Comparer<string> comparer, int inColumn)
        {
            Logger.LogAction("Matching comparer'{0}' with text in column {1} of {2} '{3}'", comparer, inColumn, GetType().Name, Id);
            return TableRow(Find.ByTextInColumn(comparer, inColumn));
        }

        /// <summary>
        /// Finds te first row that matches <paramref name="comparer"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
        /// </summary>
        /// <param name="comparer">The comparer that the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRowInOwnTableRows(Comparer<string> comparer, int inColumn)
        {
            Logger.LogAction("Matching comparer'{0}' with text in column {1} of {2} '{3}'", comparer, inColumn, GetType().Name, Id);
            return OwnTableRow(Find.ByTextInColumn(comparer, inColumn));
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
            return TableRow(Find.ByTextInColumn(predicate, inColumn));
        }

        /// <summary>
        /// Finds te first row that matches <paramref name="predicate"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned. This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
        /// </summary>
        /// <param name="predicate">The predicate that the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRowInOwnTableRows(Predicate<string> predicate, int inColumn)
        {
            return OwnTableRow(Find.ByTextInColumn(predicate, inColumn));
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
            return TableRow(Find.ByExistenceOfRelatedElement<TableCell>(row => TableCell(Find.ByIndex(inColumn) & Find.ByElement(predicate))));
        }

        /// <summary>
        /// Finds te first row that matches <paramref name="predicate"/> in <paramref name="inColumn"/> 
        /// defined as a TD html element. If no match is found, <c>null</c> is returned.  This method will look for rows in all
        /// <see cref="Core.TableBody"/> elements but will ignore rows in nested tables.
        /// </summary>
        /// <param name="predicate">The predicate that the cell text must match.</param>
        /// <param name="inColumn">Index of the column to find the text in.</param>
        /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
        public TableRow FindRowInOwnTableRows(Predicate<TableCell> predicate, int inColumn)
        {
            return OwnTableRow(Find.ByExistenceOfRelatedElement<TableCell>(row => TableCell(Find.ByIndex(inColumn) & Find.ByElement(predicate))));
        }
    }
}
