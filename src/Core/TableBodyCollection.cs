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
	/// A typed collection of <see cref="TableBody"/> instances within a Document or Element. 
	/// </summary>
#if NET11
	public class TableBodyCollection : BaseElementCollection
#else
    public class TableBodyCollection : BaseElementCollection<TableBody>
#endif
    {
		public TableBodyCollection(DomContainer domContainer, ArrayList elements) : base(domContainer, elements, new CreateElementInstance(TableBody.New)) {}

		public TableBodyCollection(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder, new CreateElementInstance(TableBody.New)) {}

		public TableBody this[int index]
		{
			get { return (TableBody) ElementsTyped(index); }
		}

        public TableBodyCollection Filter(BaseConstraint findBy)
        {
            return new TableBodyCollection(domContainer, DoFilter(findBy));
        }

#if !NET11
        public TableBodyCollection Filter(Predicate<TableBody> predicate)
        {
            return new TableBodyCollection(domContainer, DoFilter(Find.ByElement(predicate)));
        }
#endif

	}
}