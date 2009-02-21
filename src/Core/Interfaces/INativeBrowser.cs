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

using System.Collections.Generic;
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
	public interface INativeBrowser 
	{
        /// <summary>
        /// Creates an element finder.
        /// </summary>
        /// <param name="tags">The tags, or null if all tags are admissible</param>
        /// <param name="baseConstraint">The constraint, or null if no additional constraint required</param>
        /// <param name="elements">The element collection to search</param>
        /// <returns>The finder</returns>
        ElementFinder CreateElementFinder(IList<ElementTag> tags, BaseConstraint baseConstraint, IElementCollection elements);

		INativeElement CreateElement(object element);

	    INativeDocument CreateDocument(object document);
	}
}
