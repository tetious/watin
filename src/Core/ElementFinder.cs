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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WatiN.Core.Constraints;

namespace WatiN.Core
{
    /// <summary>
    /// Expresses an algorithm for finding elements.
    /// </summary>
    public abstract class ElementFinder
    {
        private readonly IList<ElementTag> elementTags;
        private readonly Constraint findBy;

        /// <summary>
        /// Creates an element finder.
        /// </summary>
        /// <param name="elementTags">The element tags considered by the finder, or null if all tags considered</param>
        /// <param name="findBy">The constraint used by the finder to filter elements, or null if no additional constraint</param>
        protected ElementFinder(IList<ElementTag> elementTags, Constraint findBy)
        {
            this.elementTags = new ReadOnlyCollection<ElementTag>(elementTags ?? new[] { ElementTag.Any });
            this.findBy = findBy ?? Find.Any;
        }

        /// <summary>
        /// Returns true if there exists at least one element that matches the finder's constraint.
        /// </summary>
        /// <returns>True if there is at least one matching element</returns>
        public bool Exists()
        {
            return FindFirst() != null;
        }

        /// <summary>
        /// Finds the first element that matches the finder's constraint.
        /// </summary>
        /// <returns>The first matching element, or null if none</returns>
        public Element FindFirst()
        {
            foreach (var element in FindAll())
                return element;
            return null;
        }

        /// <summary>
        /// Finds all elements that match the finder's constraint.
        /// </summary>
        /// <returns>An enumeration of all matching elements</returns>
        public IEnumerable<Element> FindAll()
        {
            return FindAllImpl();
        }

        /// <summary>
        /// Creates a new finder filtered by an additional constraint.
        /// </summary>
        /// <param name="constraint">The additional constraint</param>
        /// <returns>The filtered element finder</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constraint"/> is null</exception>
        public ElementFinder Filter(Constraint constraint)
        {
            if (constraint == null)
                throw new ArgumentNullException("constraint");

            return FilterImpl(constraint);
        }

        /// <summary>
        /// Gets the read-only list of tags considered by the finder.
        /// </summary>
        public IList<ElementTag> ElementTags
        {
            get { return elementTags; }
        }

        /// <summary>
        /// Gets a list of unique tag names from <see cref="ElementTags"/>.
        /// </summary>
        /// <value>The element tag names, which may contain null to signify that any tag is allowed</value>
        public IEnumerable<string> ElementTagNames
        {
            get { return ElementTag.ElementTagNames(ElementTags); }
        }

        /// <summary>
        /// Gets the constraint used by the finder to filter elements.
        /// </summary>
        public Constraint Constraint
        {
            get { return findBy; }
        }

        /// <summary>
        /// Returns a string representation of the element tags.
        /// </summary>
        public string ElementTagsToString()
        {
            return ElementTag.ElementTagsToString(ElementTags);
        }

        /// <summary>
        /// Returns a string representation of the constraint.
        /// </summary>
        public string ConstraintToString()
        {
            return findBy.ToString();
        }

        /// <summary>
        /// Creates a new finder filtered by an additional constraint.
        /// </summary>
        /// <param name="findBy">The additional constraint, not null</param>
        /// <returns>The filtered element finder</returns>
        protected abstract ElementFinder FilterImpl(Constraint findBy);

        /// <summary>
        /// Finds all elements that match the finder's constraint.
        /// </summary>
        /// <returns>An enumeration of all matching elements</returns>
        protected abstract IEnumerable<Element> FindAllImpl();
    }
}