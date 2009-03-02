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
	/// This class provides specialized functionality for a HTML tr element.
	/// </summary>
    [ElementTag("tr")]
    public class TableRow : ElementContainer<TableRow>
	{
		public TableRow(DomContainer domContainer, INativeElement htmlTableRow) : base(domContainer, htmlTableRow) {}

        public TableRow(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

        /// <summary>
        /// Gets the table that contains this row.
        /// </summary>
		public Table ContainingTable
		{
			get { return Ancestor<Table>(); }
		}

        /// <summary>
        /// Gets the table body that contains this row.
        /// </summary>
        public TableBody ContainingTableBody
        {
            get { return Ancestor<TableBody>(); }
        }

        /// <summary>
        /// Gets the table that contains this row.
        /// </summary>
        [Obsolete("Use ContainingTable instead.")]
        public Table ParentTable
        {
            get { return ContainingTable; }
        }

		/// <summary>
		/// Gets the index of the <see cref="TableRow"/> in the <see cref="TableRowCollection"/> of the parent <see cref="Table"/>.
		/// </summary>
		/// <value>The index of the row.</value>
		public int Index
		{
			get { return int.Parse(GetAttributeValue("rowIndex")); }
		}

        /// <summary>
        /// Finds a table cell within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id</param>
        /// <returns>The table cell</returns>
        public TableCell OwnTableCell(string elementId)
        {
            return OwnTableCell(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table cell within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id regular expression</param>
        /// <returns>The table cell</returns>
        public TableCell OwnTableCell(Regex elementId)
        {
            return OwnTableCell(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table cell within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="findBy">The constraint</param>
        /// <returns>The table cell</returns>
        public TableCell OwnTableCell(Constraint findBy)
        {
            return new TableCell(DomContainer, CreateElementFinder<TableCell>(nativeElement => nativeElement.TableRows, findBy));
        }

        /// <summary>
        /// Finds a table cell within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>The table cell</returns>
        public TableCell OwnTableCell(Predicate<TableCell> predicate)
        {
            return OwnTableCell(Find.ByElement(predicate));
        }

        /// <summary>
        /// Gets a collection of all table cells within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <returns>The table cell collection</returns>
        public TableCellCollection OwnTableCells
        {
            get { return new TableCellCollection(DomContainer, CreateElementFinder<TableCell>(nativeElement => nativeElement.TableCells, null)); }
        }

        /// <summary>
        /// Gets the table cells that are direct children of this <see cref="TableRow"/>, leaving
        /// out table cells of any nested tables within this <see cref="TableRow"/>.
        /// </summary>
        /// <value>The table cells collection.</value>
        [Obsolete("Use OwnTableCells instead.")]
        public TableCellCollection TableCellsDirectChildren
        {
            get { return OwnTableCells; }
        }
    }
}
