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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML tbody element. 
	/// </summary>
    /// <remarks>
    /// <para>
    /// To find rows that contain particular cell values use the <see cref="Find.ByTextInColumn(string, int)"/>
    /// constraint as in the following example:
    /// </para>
    /// <code>
    /// // Find a table row with "some text" in the 3rd (!) column.
    /// TableBody tableBody = document.Table("my_table").OwnTableBodies[0];
    /// table.OwnTableRow(Find.ByTextInColumn("some text", 2));
    /// </code>
    /// <para>
    /// To find rows based on other properties of their contents use the <see cref="Find.ByExistenceOfRelatedElement{T}" />
    /// constraint as in the following example:
    /// </para>
    /// <code>
    /// // Find a table row with "some text" in any of its columns.
    /// TableBody tableBody = document.Table("my_table").OwnTableBodies[0];
    /// table.OwnTableRow(Find.ByExistenceOfRelatedElement(row => row.OwnTableCell("some text")));
    /// </code>
    /// </remarks>
    [ElementTag("tbody")]
    public class TableBody : ElementContainer<TableBody>
	{
        public TableBody(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

		public TableBody(DomContainer domContainer, INativeElement element) : base(domContainer, element) {}

        /// <summary>
        /// Gets the table that contains this body.
        /// </summary>
        public virtual Table ContainingTable
        {
            get { return Ancestor<Table>(); }
        }

        /// <summary>
        /// Finds a table row within the table body itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id</param>
        /// <returns>The table row</returns>
        public virtual TableRow OwnTableRow(string elementId)
        {
            return OwnTableRow(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table row within the table body itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id regular expression</param>
        /// <returns>The table row</returns>
        public virtual TableRow OwnTableRow(Regex elementId)
        {
            return OwnTableRow(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table row within the table body itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="findBy">The constraint</param>
        /// <returns>The table row</returns>
        public virtual TableRow OwnTableRow(Constraint findBy)
        {
            return new TableRow(DomContainer, CreateElementFinder<TableRow>(nativeElement => nativeElement.TableRows, findBy));
        }

        /// <summary>
        /// Finds a table row within the table body itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>The table row</returns>
        public virtual TableRow OwnTableRow(Predicate<TableRow> predicate)
        {
            return OwnTableRow(Find.ByElement(predicate));
        }

        /// <summary>
        /// Gets a collection of all table rows within the table body itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <returns>The table row collection</returns>
        public virtual TableRowCollection OwnTableRows
        {
            get { return new TableRowCollection(DomContainer, CreateElementFinder<TableRow>(nativeElement => nativeElement.TableRows, null)); }
        }

        /// <summary>
        /// Gets the table rows that are direct children of this <see cref="TableBody"/>, leaving
        /// out table rows of any nested tables within this <see cref="TableBody"/>.
        /// </summary>
        /// <value>The table rows collection.</value>
        [Obsolete("Use OwnTableRows instead.")]
        public virtual TableRowCollection TableRowsDirectChildren
        {
            get { return OwnTableRows; }
        }
    }
}
