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

using System;
using System.Collections;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of <see cref="Area" /> instances within a <see cref="Document" /> or <see cref="Element" />.
	/// </summary>
#if NET11
	public class AreaCollection : BaseElementCollection
#else
    public class AreaCollection : BaseElementCollection<Area>
#endif
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="AreaCollection" /> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="elements">The elements.</param>
		public AreaCollection(DomContainer domContainer, ArrayList elements) : base(domContainer, elements, new CreateElementInstance(Area.New)) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="AreaCollection" /> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="finder">The finder.</param>
		public AreaCollection(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder, new CreateElementInstance(Area.New)) {}

		/// <summary>
		/// Returns a new <see cref="AreaCollection" /> filtered by the <see cref="BaseConstraint" />.
		/// </summary>
		/// <param name="findBy">The attribute to filter by.</param>
		/// <returns>The filtered collection.</returns>
		public AreaCollection Filter(BaseConstraint findBy)
		{
			return new AreaCollection(domContainer, DoFilter(findBy));
		}

#if !NET11
		/// <summary>
		/// Returns a new <see cref="AreaCollection" /> filtered by the given <see cref="predicate" />.
		/// </summary>
		/// <param name="predicate">A predicate which filters the elements.</param>
		/// <returns>The filtered collection.</returns>
        public AreaCollection Filter(Predicate<Area> predicate)
        {
            return new AreaCollection(domContainer, DoFilter(Find.ByElement(predicate)));
        }
#endif

		/// <summary>
		/// Gets the <see cref="Area" /> at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The area.</returns>
		public Area this[int index]
		{
			get { return (Area)ElementsTyped(index); }
		}
	}
}