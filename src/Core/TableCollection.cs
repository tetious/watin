#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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
using WatiN.Core.Constraints;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of <see cref="Table" /> instances within a <see cref="Document"/> or <see cref="Element"/>.
	/// </summary>
	public class TableCollection : BaseElementCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TableCollection"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="finder">The finder.</param>
		public TableCollection(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder, new CreateElementInstance(New)) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="TableCollection"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="elements">The elements.</param>
		public TableCollection(DomContainer domContainer, ArrayList elements) : base(domContainer, elements, new CreateElementInstance(New)) {}

		/// <summary>
		/// Gets the <see cref="Table"/> at the specified index.
		/// </summary>
		/// <value></value>
		public Table this[int index]
		{
			get { return new Table(domContainer, (IHTMLTable) Elements[index]); }
		}

		public TableCollection Filter(BaseConstraint findBy)
		{
			return new TableCollection(domContainer, DoFilter(findBy));
		}

		private static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new Table(domContainer, (IHTMLTable) element);
		}
	}
}