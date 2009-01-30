#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

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
using System.Collections.Generic;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of <see cref="TableBody"/> instances within a Document or Element. 
	/// </summary>
    public class TableBodyCollection : BaseElementCollection<TableBody>
    {
        public TableBodyCollection(DomContainer domContainer, IEnumerable<INativeElement> elements) : base(domContainer, elements, TableBody.New) { }

		public TableBodyCollection(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder, TableBody.New) {}

		public TableBody this[int index]
		{
			get { return ElementsTyped(index); }
		}

        public TableBodyCollection Filter(BaseConstraint findBy)
        {
            return new TableBodyCollection(domContainer, DoFilter(findBy));
        }

        public TableBodyCollection Filter(Predicate<TableBody> predicate)
        {
            return new TableBodyCollection(domContainer, DoFilter(Find.ByElement(predicate)));
        }
	}
}