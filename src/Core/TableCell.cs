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
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML td element.
	/// </summary>
    [ElementTag("td")]
    // TODO: Adding th support in this way would break many test out in the field
    //       maybe add a TableHeaderCell element instead.
    //[ElementTag("th")] S
    public class TableCell : ElementContainer<TableCell>
	{
		public TableCell(DomContainer domContainer, INativeElement htmlTableCell) : base(domContainer, htmlTableCell) {}

        public TableCell(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

        /// <summary>
        /// Gets the table that contains this cell.
        /// </summary>
        public virtual Table ContainingTable
        {
            get { return Ancestor<Table>(); }
        }

        /// <summary>
        /// Gets the table body that contains this cell.
        /// </summary>
        public virtual TableBody ContainingTableBody
        {
            get { return Ancestor<TableBody>(); }
        }

        /// <summary>
        /// Gets the table row that contains this cell.
        /// </summary>
        public virtual TableRow ContainingTableRow
        {
            get { return Ancestor<TableRow>(); }
        }

        /// <summary>
        /// Gets the table row that contains this cell.
        /// </summary>
        [Obsolete("Use ContainingTableRow instead.")]
        public virtual TableRow ParentTableRow
        {
            get { return ContainingTableRow; }
        }

		/// <summary>
		/// Gets the index of the <see cref="TableCell"/> in the <see cref="TableCellCollection"/> of the parent <see cref="TableRow"/>.
		/// </summary>
		/// <value>The index of the cell.</value>
        public virtual int Index
		{
			get { return int.Parse(GetAttributeValue("cellIndex")); }
		}
	}
}
