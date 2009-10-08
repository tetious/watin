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
using WatiN.Core.Native;

namespace WatiN.Core
{
    /// <summary>
    /// Finds elements within native element containers.
    /// </summary>
    public class NativeElementFinder : ElementFinder
    {
        public delegate INativeElementCollection NativeElementCollectionFactory();

        private readonly DomContainer domContainer;
        private readonly NativeElementCollectionFactory factory;

        /// <summary>
        /// Creates an element finder.
        /// </summary>
        /// <param name="domContainer">The DOM container</param>
        /// <param name="factory">The factory to get the element(s) to search in</param>
        /// <param name="elementTags">The element tags considered by the finder, or null if all tags considered</param>
        /// <param name="constraint">The constraint used by the finder to filter elements, or null if no additional constraint</param>
        public NativeElementFinder(NativeElementCollectionFactory factory, DomContainer domContainer, IList<ElementTag> elementTags, Constraint constraint)
            : base(elementTags, constraint)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (domContainer == null)
                throw new ArgumentNullException("domContainer");

            this.factory = factory;
            this.domContainer = domContainer;
        }

        /// <inheritdoc />
        protected override ElementFinder FilterImpl(Constraint findBy)
        {
            return new NativeElementFinder(factory, domContainer, ElementTags, Constraint & findBy);
        }

        /// <inheritdoc />
        protected override IEnumerable<Element> FindAllImpl()
        {
            var id = GetElementIdHint(Constraint);
            return id != null ? FindElementsById(id) : FindElementByTags();
        }

        private IEnumerable<Element> FindElementByTags()
        {
            foreach (var elementTag in ElementTagNames)
            {
                foreach (var element in FindElementsByTag(elementTag))
                    yield return element;
            }
        }

        private IEnumerable<Element> FindElementsByTag(string tagName)
        {
            return WrapMatchingElements(tagName == null
                ? GetNativeElementCollection().GetElements()
                : GetNativeElementCollection().GetElementsByTag(tagName));
        }

        private IEnumerable<Element> FindElementsById(string id)
        {
            return WrapMatchingElements(GetNativeElementCollection().GetElementsById(id));
        }

        private IEnumerable<Element> WrapMatchingElements(IEnumerable<INativeElement> nativeElements)
        {
            var context = new ConstraintContext();
            foreach (var nativeElement in nativeElements)
            {
                var element = WrapElementIfMatch(nativeElement, context);
                if (element == null) continue;
                
                yield return element;
            }
        }

        private Element WrapElementIfMatch(INativeElement nativeElement, ConstraintContext context)
        {
            nativeElement.WaitUntilReady();

            if (IsMatchByTag(nativeElement))
            {
                var element = WrapElement(nativeElement);
                if (element == null) return null;

                if (IsMatchByConstraint(element, context))
                    return element;
            }

            return null;
        }

        private Element WrapElement(INativeElement nativeElement)
        {
            return ElementFactory.CreateElement(domContainer, nativeElement);
        }

        private bool IsMatchByTag(INativeElement nativeElement)
        {
            return ElementTag.IsMatch(ElementTags, nativeElement);
        }

        private bool IsMatchByConstraint(Component element, ConstraintContext context)
        {
            return element.Matches(Constraint, context);
        }

        private INativeElementCollection GetNativeElementCollection()
        {
            var nativeElementCollection = factory.Invoke();
            return nativeElementCollection ?? EmptyElementCollection.Empty;
        }

        /// <summary>
        /// If the constraint can only match on element with a particular id, returns the id,
        /// otherwise returns null.
        /// </summary>
        /// <returns>The id or null if the constraint could match elements with no particular id</returns>
        protected virtual string GetElementIdHint(Constraint constraint)
        {
            return IdHinter.GetIdHint(constraint);
        }
    }
}