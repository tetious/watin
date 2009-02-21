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
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using StringComparer=WatiN.Core.Comparers.StringComparer;

namespace WatiN.Core.Native
{
    /// <summary>
    /// Abstract base class for element finders defined over native elements.
    /// </summary>
    public abstract class NativeElementFinder : ElementFinder
    {
        private readonly IElementCollection elementCollection;
        private readonly DomContainer domContainer;

        /// <summary>
        /// Creates an element finder.
        /// </summary>
        /// <param name="elementTags">The element tags considered by the finder, or null if all tags considered</param>
        /// <param name="constraint">The constraint used by the finder to filter elements, or null if no additional constraint</param>
        /// <param name="elementCollection">The element collection</param>
        /// <param name="domContainer">The DOM container</param>
        protected NativeElementFinder(IList<ElementTag> elementTags, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer)
            : base(elementTags, constraint)
        {
            if (elementCollection == null)
                throw new ArgumentNullException("elementCollection");
            if (domContainer == null)
                throw new ArgumentNullException("domContainer");

            this.elementCollection = elementCollection;
            this.domContainer = domContainer;
        }

        /// <summary>
        /// Gets the DOM container.
        /// </summary>
        protected DomContainer DomContainer
        {
            get { return domContainer; }
        }

        /// <summary>
        /// Gets the element collection.
        /// </summary>
        protected IElementCollection ElementCollection
        {
            get { return elementCollection; }
        }

        /// <inheritdoc />
        protected override IEnumerable<Element> FindAllImpl()
        {
            string id = GetElementIdToMatchIfPossible(Constraint);
            if (id != null)
                return FindElementsById(id);

            return FindElementByTags();
        }

        private IEnumerable<Element> FindElementByTags()
        {
            foreach (var elementTag in ElementTags)
            {
                foreach (Element element in FindElementsByTag(elementTag))
                    yield return element;
            }
        }

        /// <summary>
        /// Finds a matching element by its tag.
        /// </summary>
        /// <param name="elementTag">The tag to filter on</param>
        /// <returns>The matching elements</returns>
        protected abstract IEnumerable<Element> FindElementsByTag(ElementTag elementTag);

        /// <summary>
        /// Finds a matching element by its id.
        /// </summary>
        /// <param name="id">The id to search with</param>
        /// <returns>The matching elements</returns>
        protected abstract IEnumerable<Element> FindElementsById(string id);

        /// <summary>
        /// Wraps a native element if it matches the criteria, returns null if not a match.
        /// </summary>
        /// <param name="nativeElement">The native element</param>
        /// <returns>The wrapped element, or null if not a match</returns>
        protected Element WrapElementIfMatch(INativeElement nativeElement)
        {
            nativeElement.WaitUntilReady();

            if (IsMatchByTag(nativeElement))
            {
                Element element = WrapElement(nativeElement);
                if (IsMatchByConstraint(element))
                    return element;
            }

            return null;
        }

        /// <summary>
        /// Wraps a native element if it matches the criteria.
        /// </summary>
        /// <param name="nativeElement">The native element</param>
        /// <returns>The wrapped element</returns>
        protected Element WrapElement(INativeElement nativeElement)
        {
            return ElementFactory.CreateElement(DomContainer, nativeElement);
        }

        private bool IsMatchByTag(INativeElement nativeElement)
        {
            return ElementTag.IsMatch(ElementTags, nativeElement);
        }

        private bool IsMatchByConstraint(Element element)
        {
            return Constraint.Compare(element);
        }

        private static string GetElementIdToMatchIfPossible(BaseConstraint constraint)
        {
            if (!OnlyAndConstraints(constraint))
                return null;

            AttributeConstraint idConstraint = RetrieveIdConstraint(constraint as AttributeConstraint);
            if (idConstraint == null)
                return null;

            return idConstraint.Value;
        }

        private static bool OnlyAndConstraints(BaseConstraint constraint)
        {
            if (constraint == null)
                return false;
            if (constraint.HasOr)
                return false;

            return !constraint.HasAnd || OnlyAndConstraints(constraint.GetAnd);
        }

        private static AttributeConstraint RetrieveIdConstraint(AttributeConstraint attributeConstraint)
        {
            if (attributeConstraint == null)
                return null;

            var validIdConstraint = StringComparer.AreEqual(attributeConstraint.AttributeName, "id", true) &&
                attributeConstraint.Comparer.GetType() == typeof (StringComparer);

            return validIdConstraint ? attributeConstraint : RetrieveIdConstraint(attributeConstraint.GetAnd as AttributeConstraint);
        }
    }
}