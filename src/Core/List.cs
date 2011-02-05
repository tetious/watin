#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Native;

namespace WatiN.Core
{
    /// <summary>
    /// This class provides specialized functionality for a HTML ul and ol elements.
    /// </summary>
    
    [ElementTag("ul",Index=0)]
    [ElementTag("ol",Index=1)]
    public class List : ElementContainer<List>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="List"/> class.
        /// Mainly used by WatiN internally.
        /// </summary>
        /// <param name="domContainer">The DOM container.</param>
        /// <param name="nativeElement">The HTML ul or ol element.</param>
        public List(DomContainer domContainer, INativeElement nativeElement)
            : base(domContainer, nativeElement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List"/> class.
        /// Mainly used by WatiN internally.
        /// </summary>
        /// <param name="domContainer">The DOM container.</param>
        /// <param name="finder">The HTML ul or ol element.</param>
        public List(DomContainer domContainer, ElementFinder finder)
            : base(domContainer, finder)
        {
        }

        public virtual bool IsOrdered
        {
            get { return TagName.ToLowerInvariant().Equals("ol"); }
        }

        /// <summary>
        /// Finds a list item within the list itself (excluding content from any lists that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id</param>
        /// <returns>The list item</returns>
        public virtual ListItem OwnListItem(string elementId)
        {
            return OwnListItem(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a list item within the list itself (excluding content from any lists that
        /// might be nested within it).
        /// </summary>
        /// <param name="elementId">The element id regular expression</param>
        /// <returns>The list item</returns>
        public virtual ListItem OwnListItem(Regex elementId)
        {
            return OwnListItem(Find.ById(elementId));
        }

        /// <summary>
        /// Finds a list item within the list itself (excluding content from any lists that
        /// might be nested within it).
        /// </summary>
        /// <param name="findBy">The constraint</param>
        /// <returns>The list item</returns>
        public virtual ListItem OwnListItem(Constraint findBy)
        {
            return ChildOfType<ListItem>(findBy);
        }

        /// <summary>
        /// Finds a list item within the list itself (excluding content from any lists that
        /// might be nested within it).
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>The list item</returns>
        public virtual ListItem OwnListItem(Predicate<ListItem> predicate)
        {
            return OwnListItem(Find.ByElement(predicate));
        }

        /// <summary>
        /// Gets a collection of all table rows within the table itself (excluding content from any tables that
        /// might be nested within it).
        /// </summary>
        /// <returns>The table row collection</returns>
        public virtual ListItemCollection OwnListItems
        {
            get { return new ListItemCollection(DomContainer, CreateElementFinder<ListItem>(nativeElement => nativeElement.Children, null)); }
        }
    }
}
