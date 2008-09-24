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
	/// This class provides specialized functionality for a HTML td element.
	/// </summary>
#if NET11
    public class TableCell : ElementsContainer
#else
    public class TableCell : ElementsContainer<TableCell>
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
					elementTags.Add(new ElementTag("td"));
				}

				return elementTags;
			}
		}

		public TableCell(DomContainer domContainer, IHTMLTableCell htmlTableCell) : 
            base(domContainer, domContainer.NativeBrowser.CreateElement(htmlTableCell)) {}

		public TableCell(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="TableCell"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public TableCell(Element element) : base(element, ElementTags) {}

		/// <summary>
		/// Gets the parent <see cref="TableRow"/> of this <see cref="TableCell"/>.
		/// </summary>
		/// <value>The parent table row.</value>
		public TableRow ParentTableRow
		{
			get { return (TableRow) Ancestor(typeof (TableRow)); }
		}

		/// <summary>
		/// Gets the index of the <see cref="TableCell"/> in the <see cref="TableCellCollection"/> of the parent <see cref="TableRow"/>.
		/// </summary>
		/// <value>The index of the cell.</value>
		public int Index
		{
			get { return int.Parse(GetAttributeValue("cellindex")); }
		}

		internal new static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new TableCell(domContainer, (IHTMLTableCell) element);
		}
	}
}
