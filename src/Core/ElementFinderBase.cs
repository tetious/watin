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

namespace WatiN.Core
{
    public abstract class ElementFinderBase : INativeElementFinder
    {
        private readonly List<ElementTag> tagsToFind = new List<ElementTag>();
        protected BaseConstraint _constraint;
        protected IElementCollection _elementCollection;
        protected DomContainer _domContainer;

        protected ElementFinderBase(List<ElementTag> elementTags, IElementCollection elementCollection, DomContainer domContainer) : this(elementTags, null, elementCollection, domContainer) { }

        protected ElementFinderBase(List<ElementTag> elementTags, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer)
        {
            CheckAndInitPrivateFields(elementCollection, domContainer, constraint);

            if (elementTags != null)
            {
                tagsToFind = elementTags;
            }
            else
            {
                AddElementTag(null, null);
            }
        }

        protected ElementFinderBase(string tagName, string inputType, IElementCollection elementCollection, DomContainer domContainer) : this(tagName, inputType, null, elementCollection, domContainer) {}

        protected ElementFinderBase(string tagName, string inputType, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer)
        {
            CheckAndInitPrivateFields(elementCollection, domContainer, constraint);

            AddElementTag(tagName, inputType);
        }

        public string ElementTagsToString
        {
            get { return ElementTag.ElementTagsToString(tagsToFind); }
        }

        public string ConstraintToString
        {
            get { return _constraint.ConstraintToString(); }
        }

        private void CheckAndInitPrivateFields(IElementCollection elementCollection, DomContainer domContainer, BaseConstraint constraint)
        {
            if (elementCollection == null) throw new ArgumentNullException("elementCollection");
            if (domContainer == null) throw new ArgumentNullException("domContainer");

            _constraint = GetConstraint(constraint);
            _elementCollection = elementCollection;
            _domContainer = domContainer;
        }

        private static BaseConstraint GetConstraint(BaseConstraint constraint)
        {
            return constraint ?? new AlwaysTrueConstraint();
        }

        public virtual INativeElement FindFirst()
        {
            return FindFirst(_constraint);
        }

        public virtual INativeElement FindFirst(BaseConstraint constraint)
        {
            foreach (var elementTag in tagsToFind)
            {
                var elements = FindElementsByAttribute(elementTag, constraint, true);

                if (elements.Count > 0)
                {
                    return elements[0];
                }
            }

            return null;
        }

        public void AddElementTag(string tagName, string inputType)
        {
            tagsToFind.Add(new ElementTag(tagName, inputType));
        }

        public IEnumerable<INativeElement> FindAll()
        {
            return FindAll(_constraint);
        }

        public IEnumerable<INativeElement> FindAll(BaseConstraint constraint)
        {
            if (tagsToFind.Count == 1)
            {
                return FindElementsByAttribute(tagsToFind[0], constraint, false);
            }

            return FindAllWithMultipleTags(constraint);
        }

        private IEnumerable<INativeElement> FindAllWithMultipleTags(BaseConstraint constraint)
        {
            var elements = new List<INativeElement>();

            foreach (var elementTag in tagsToFind)
            {
                elements.AddRange(FindElementsByAttribute(elementTag, constraint, false));
            }

            return elements;
        }

        private List<INativeElement> FindElementsByAttribute(ElementTag elementTag, BaseConstraint constraint, bool returnAfterFirstMatch)
        {
            // Get elements with the tagname from the page
            constraint.Reset();

            var attributeBag = new ElementAttributeBag(_domContainer);
            if (IsGetElementByIdPossible(constraint))
            {
                return GetElementById(constraint, elementTag, attributeBag);
            }

            return FindElements(constraint, elementTag, attributeBag, returnAfterFirstMatch, _elementCollection);
        }

        protected abstract List<INativeElement> FindElements(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection elementCollection);
        protected abstract INativeElement FindElementById(string Id, IElementCollection elementCollection);

        private static bool IsGetElementByIdPossible(BaseConstraint constraint)
        {
            var attributeConstraint = constraint as AttributeConstraint;

            return attributeConstraint != null &&
                   StringComparer.AreEqual(attributeConstraint.AttributeName, "id", true) &&
                   !(constraint.HasAnd || constraint.HasOr) &&
                   attributeConstraint.Comparer.GetType() == typeof(StringComparer);
        }

        private List<INativeElement> GetElementById(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag elementAttributeBag)
        {
            var Id = ((AttributeConstraint) constraint).Value;
            var element = FindElementById(Id, _elementCollection);
            
            var elements = new List<INativeElement>();

            if (element != null && elementTag.Compare(element) && IsMatchingElement(element, elementAttributeBag, elementTag, constraint))
            {
                return new List<INativeElement> {element};
            }

            return elements;
        }

        protected bool FinishedAddingChildrenThatMetTheConstraints(INativeElement nativeElement, ElementAttributeBag attributeBag, ElementTag elementTag, BaseConstraint constraint, ICollection<INativeElement> matchingElements, bool returnAfterFirstMatch)
        {
            if (!IsMatchingElement(nativeElement, attributeBag, elementTag, constraint)) return false;

            AddToMatchingElements(nativeElement, matchingElements);
            return returnAfterFirstMatch;
        }

        private bool IsMatchingElement(INativeElement nativeElement, ElementAttributeBag attributeBag, ElementTag elementTag, BaseConstraint constraint)
        {
            WaitUntilElementReadyStateIsComplete(nativeElement);

            attributeBag.INativeElement = nativeElement;

            var validElementType = true;
            if (elementTag.IsInputElement)
            {
                validElementType = elementTag.CompareInputTypes(nativeElement);
            }

            return (validElementType && constraint.Compare(attributeBag));
        }

        protected virtual void AddToMatchingElements(INativeElement nativeElement, ICollection<INativeElement> matchingElements)
        {
            matchingElements.Add(nativeElement);
        }

        protected abstract void WaitUntilElementReadyStateIsComplete(INativeElement element);
    }
}