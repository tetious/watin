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
using WatiN.Core.Constraints;
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML tbody element. 
	/// </summary>
    [ElementTag("tbody")]
    public sealed class TableBody : ElementsContainer<TableBody>
	{
        public TableBody(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

		public TableBody(DomContainer domContainer, INativeElement element) : base(domContainer, element) {}

		/// <summary>
		/// Returns the table rows belonging to this table body (not including table rows 
		/// from tables nested in this table body).
		/// </summary>
		/// <value>The table rows.</value>
		public override TableRowCollection TableRows
		{
            get { return TableRowsDirectChildren; }
		}

        /// <summary>
        /// Gets the table rows that are direct children of this <see cref="TableBody"/>, leaving
        /// out table rows of any nested tables within this <see cref="TableBody"/>.
        /// </summary>
        /// <value>The table rows collection.</value>
        public TableRowCollection TableRowsDirectChildren
        {
            get
            {
                return new TableRowCollection(DomContainer, NativeElement.TableBodyRows(DomContainer));
            }
        }

		/// <summary>
		/// Returns the table row belonging to this table body (not including table rows 
		/// from tables nested in this table body).
		/// </summary>
		/// <param name="findBy">The find by.</param>
		/// <returns></returns>
		public override TableRow TableRow(BaseConstraint findBy)
		{
			return ElementsSupport.TableRow(DomContainer, NativeElement.TableBodyRows(DomContainer).Filter(findBy));
		}

        /// <summary>
		/// Returns the table row belonging to this table body (not including table rows 
		/// from tables nested in this table body).
		/// </summary>
        /// <param name="predicate">The expression to use.</param>
		/// <returns></returns>
		public override TableRow TableRow(Predicate<TableRow> predicate)
		{
			return TableRow(Find.ByElement(predicate));
		}
	}
}
