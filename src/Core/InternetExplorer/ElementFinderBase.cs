using System;
using System.Collections;
using System.Collections.Generic;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using StringComparer=WatiN.Core.Comparers.StringComparer;

namespace WatiN.Core.InternetExplorer
{
    public abstract class ElementFinderBase
    {
        private readonly ArrayList tagsToFind = new ArrayList();
        protected BaseConstraint _constraint;
        protected IElementCollection _elementCollection;
        protected DomContainer _domContainer;

        protected ElementFinderBase(ArrayList elementTags, IElementCollection elementCollection, DomContainer domContainer) : this(elementTags, null, elementCollection, domContainer) {}

        protected ElementFinderBase(ArrayList elementTags, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer)
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
            foreach (ElementTag elementTag in tagsToFind)
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
                return FindElementsByAttribute((ElementTag) tagsToFind[0], constraint, false);
            }

            return FindAllWithMultipleTags(constraint);
        }

        private IEnumerable<INativeElement> FindAllWithMultipleTags(BaseConstraint constraint)
        {
            var elements = new List<INativeElement>();

            foreach (ElementTag elementTag in tagsToFind)
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

            if (FindByExactMatchOnIdPossible(constraint))
            {
                return FindElementById(constraint, elementTag, attributeBag, returnAfterFirstMatch, _elementCollection);
            }
            return FindElements(constraint, elementTag, attributeBag, returnAfterFirstMatch, _elementCollection);
        }

        protected abstract List<INativeElement> FindElements(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection elementCollection);
        protected abstract List<INativeElement> FindElementById(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection elementCollection);

        private static bool FindByExactMatchOnIdPossible(BaseConstraint constraint)
        {
            var attributeConstraint = constraint as AttributeConstraint;
			
            return attributeConstraint != null && 
                   StringComparer.AreEqual(attributeConstraint.AttributeName, "id", true) && 
                   !(constraint.HasAnd || constraint.HasOr) && 
                   attributeConstraint.Comparer.GetType() == typeof(StringComparer);
        }

        protected bool FinishedAddingChildrenThatMetTheConstraints(INativeElement nativeElement, ElementAttributeBag attributeBag, ElementTag elementTag, BaseConstraint constraint, ICollection<INativeElement> children, bool returnAfterFirstMatch)
        {
            WaitUntilElementReadyStateIsComplete(nativeElement);

            attributeBag.INativeElement = nativeElement;

            var validElementType = true;
            if (elementTag.IsInputElement)
            {
                validElementType = elementTag.CompareInputTypes(nativeElement);
            }

            if (validElementType && constraint.Compare(attributeBag))
            {
                children.Add(nativeElement);
                if (returnAfterFirstMatch)
                {
                    return true;
                }
            }
            return false;
        }

        protected abstract void WaitUntilElementReadyStateIsComplete(INativeElement element);
    }
}