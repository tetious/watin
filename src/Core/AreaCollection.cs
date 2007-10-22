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

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of <see cref="Area" /> instances within a <see cref="Document" /> or <see cref="Element" />.
	/// </summary>
	public class AreaCollection : BaseElementCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AreaCollection" /> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="elements">The elements.</param>
		public AreaCollection(DomContainer domContainer, ArrayList elements) : base(domContainer, elements, new CreateElementInstance(New)) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="AreaCollection" /> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="finder">The finder.</param>
		public AreaCollection(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder, new CreateElementInstance(New)) {}

		/// <summary>
		/// Returns a new <see cref="AreaCollection" /> filtered by the <see cref="AttributeConstraint" />.
		/// </summary>
		/// <param name="findBy">The attribute to filter by.</param>
		/// <returns>The filtered collection.</returns>
		public AreaCollection Filter(AttributeConstraint findBy)
		{
			return new AreaCollection(domContainer, DoFilter(findBy));
		}

		/// <summary>
		/// Gets the <see cref="Area" /> at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The area.</returns>
		public Area this[int index]
		{
			get { return new Area(domContainer, (IHTMLAreaElement) Elements[index]); }
		}

		private static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new Area(domContainer, (IHTMLAreaElement) element);
		}
	}
}