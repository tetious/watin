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

using System.Collections;
using mshtml;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML tr element.
	/// </summary>
#if NET11
    public class TableRow : ElementsContainer
#else
    public class TableRow : ElementsContainer<TableRow>
#endif
	{
		private static ArrayList elementTags;

		public static ArrayList ElementTags
		{
			get
			{
				if (elementTags == null)
				{
					elementTags = new ArrayList();
					elementTags.Add(new ElementTag("tr"));
				}

				return elementTags;
			}
		}

		public TableRow(DomContainer domContainer, IHTMLTableRow htmlTableRow) : 
            base(domContainer, domContainer.NativeBrowser.CreateElement(htmlTableRow)) {}

		public TableRow(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="TableRow"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public TableRow(Element element) : base(element, ElementTags) {}

		public Table ParentTable
		{
			get { return (Table) Ancestor(typeof (Table)); }
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
        /// Gets the table cells that are direct children of this <see cref="TableRow"/>, leaving
        /// out table cells of any nested tables within this <see cref="TableRow"/>.
        /// </summary>
        /// <value>The table cells collection.</value>
        public TableCellCollection TableCellsDirectChildren
        {
            get 
            { 
                IHTMLTableRow row = (IHTMLTableRow)HTMLElement;

                ArrayList list = UtilityClass.IHtmlElementCollectionToArrayList(row.cells);
                return new TableCellCollection(DomContainer, list);
            }
        }

        internal new static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new TableRow(domContainer, (IHTMLTableRow) element);
		}
	}
}
