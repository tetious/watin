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
        public virtual Table ContainingTable
		{
			get { return Ancestor<Table>(); }
		}

        /// <summary>
        /// Gets the table body that contains this row.
        /// </summary>
        public virtual TableBody ContainingTableBody
        {
            get { return Ancestor<TableBody>(); }
        }

        /// <summary>
        /// Gets the table that contains this row.
        /// </summary>
        [Obsolete("Use ContainingTable instead.")]
        public virtual Table ParentTable
        {
            get { return ContainingTable; }
        }

		/// <summary>
		/// Gets the index of the <see cref="TableRow"/> in the <see cref="TableRowCollection"/> of the parent <see cref="Table"/>.
		/// </summary>
		/// <value>The index of the row.</value>
        public virtual int Index
		{
			get { return int.Parse(GetAttributeValue("rowIndex")); }
		}

        /// <summary>
        /// Finds a table cell within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id</param>
        /// <returns>The table cell</returns>
        public virtual TableCell OwnTableCell(string elementId)
        {
            return OwnTableCell(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table cell within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id regular expression</param>
        /// <returns>The table cell</returns>
        public virtual TableCell OwnTableCell(Regex elementId)
        {
            return OwnTableCell(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a table cell within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="findBy">The constraint</param>
        /// <returns>The table cell</returns>
        public virtual TableCell OwnTableCell(Constraint findBy)
        {
            return new TableCell(DomContainer, CreateElementFinder<TableCell>(nativeElement => nativeElement.TableCells, findBy));
        }

        /// <summary>
        /// Finds a table cell within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>The table cell</returns>
        public virtual TableCell OwnTableCell(Predicate<TableCell> predicate)
        {
            return OwnTableCell(Find.ByElement(predicate));
        }

        /// <summary>
        /// Gets a collection of all table cells within the table row itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <returns>The table cell collection</returns>
        public virtual TableCellCollection OwnTableCells
        {
            get { return new TableCellCollection(DomContainer, CreateElementFinder<TableCell>(nativeElement => nativeElement.TableCells, null)); }
        }

        /// <summary>
        /// Gets the table cells that are direct children of this <see cref="TableRow"/>, leaving
        /// out table cells of any nested tables within this <see cref="TableRow"/>.
        /// </summary>
        /// <value>The table cells collection.</value>
        [Obsolete("Use OwnTableCells instead.")]
        public virtual TableCellCollection TableCellsDirectChildren
        {
            get { return OwnTableCells; }
        }

        /// <summary>
        /// Returns a <see cref="Constraint"/> which can be applied on a <see cref="TableRowCollection"/>
        /// to filter out <see cref="TableRow"/> elements contained in a thead section.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="IsFooterRow"/>
        /// <example>
        /// Following example shows how to get only the rows inside a thead section:
        /// <code>
        /// var browser = new IE("www.watin.net/examples/tables.htm");
        /// var tableRows = browser.Table("table_id").OwnTableRows;
        /// 
        /// var headerRows = tableRows.Filter(TableRow.IsHeaderRow());
        /// </code>
        /// If you don't want header rows in your tablerows collection apply to <see cref="NotConstraint"/>:
        /// <code>
        /// var browser = new IE("www.watin.net/examples/tables.htm");
        /// var tableRows = browser.Table("table_id").OwnTableRows;
        /// 
        /// var noHeaderRows = tableRows.Filter(!TableRow.IsHeaderRow());
        /// </code>
        /// </example>
	    public static Constraint IsHeaderRow()
	    {
	        return Find.ByElement(element => element.Parent.TagName.ToLowerInvariant() == "thead");
	    }

        /// <summary>
        /// Returns a <see cref="Constraint"/> which can be applied on a <see cref="TableRowCollection"/>
        /// to filter out <see cref="TableRow"/> elements contained in a tfoot section.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="IsHeaderRow"/>
        /// <example>
        /// Following example shows how to get only the rows inside a tfoot section:
        /// <code>
        /// var browser = new IE("www.watin.net/examples/tables.htm");
        /// var tableRows = browser.Table("table_id").OwnTableRows;
        /// 
        /// var footerRows = tableRows.Filter(TableRow.IsFooterRow());
        /// </code>
        /// If you don't want header rows in your tablerows collection apply to <see cref="NotConstraint"/>:
        /// <code>
        /// var browser = new IE("www.watin.net/examples/tables.htm");
        /// var tableRows = browser.Table("table_id").OwnTableRows;
        /// 
        /// var noFooterRows = tableRows.Filter(!TableRow.IsFooterRow());
        /// </code>
        /// </example>
        public static Constraint IsFooterRow()
	    {
            return Find.ByElement(element => element.Parent.TagName.ToLowerInvariant() == "tfoot");
        }
	}
}
