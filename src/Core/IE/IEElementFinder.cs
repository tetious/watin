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
using System.Collections.Generic;
using System.Threading;
using mshtml;
using WatiN.Core.Constraints;
using StringComparer = WatiN.Core.Comparers.StringComparer;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.InternetExplorer
{
	/// <summary>
	/// This class is mainly used internally by WatiN to find elements in
	/// an <see cref="IHTMLElementCollection"/> or <see cref="ArrayList"/> matching
	/// the given <see cref="BaseConstraint"/>.
	/// </summary>
	public class IEElementFinder : INativeElementFinder
	{
		private readonly ArrayList tagsToFind = new ArrayList();

		protected BaseConstraint _constraint;
		protected IElementCollection _elementCollection;
	    protected DomContainer _domContainer;

	    public IEElementFinder(ArrayList elementTags, IElementCollection elementCollection, DomContainer domContainer) : this(elementTags, null, elementCollection, domContainer) {}
	    public IEElementFinder(ArrayList elementTags, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer)
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

		public IEElementFinder(string tagName, string inputType, IElementCollection elementCollection, DomContainer domContainer) : this(tagName, inputType, null, elementCollection, domContainer) {}
		public IEElementFinder(string tagName, string inputType, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer)
		{
            CheckAndInitPrivateFields(elementCollection, domContainer, constraint);

			AddElementTag(tagName, inputType);
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
				var elements = findElementsByAttribute(elementTag, constraint, true);

				if (elements.Count > 0)
				{
					return elements[0];
				}
			}

			return null;
		}

		public string ElementTagsToString
		{
			get { return ElementTag.ElementTagsToString(tagsToFind); }
		}

		public string ConstraintToString
		{
			get { return _constraint.ConstraintToString(); }
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
				return findElementsByAttribute((ElementTag) tagsToFind[0], constraint, false);
			}

		    return FindAllWithMultipleTags(constraint);
		}

	    private IEnumerable<INativeElement> FindAllWithMultipleTags(BaseConstraint constraint)
	    {
	        var elements = new List<INativeElement>();

	        foreach (ElementTag elementTag in tagsToFind)
	        {
	            elements.AddRange(findElementsByAttribute(elementTag, constraint, false));
	        }

	        return elements;
	    }

		private List<INativeElement> findElementsByAttribute(ElementTag elementTag, BaseConstraint constraint, bool returnAfterFirstMatch)
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

	    public List<INativeElement> FindElements(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection elementCollection)
	    {
	        var elements = elementTag.GetElementCollection(elementCollection.Elements);
            var children = new List<INativeElement>();

	        if (elements != null)
	        {
	            // Loop through each element and evaluate
	            var length = elements.length;
	            for (var index = 0; index < length; index++ )
                {
                    var element = (IHTMLElement)elements.item(index, null);
                    if (FinishedAddingChildrenThatMetTheConstraints(constraint, elementTag, attributeBag, returnAfterFirstMatch, element, children))
                    {
                        return children;
                    }
                }
	        }

	        return children;
	    }

        private List<INativeElement> FindElementById(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection elementCollection)
	    {
            var children = new List<INativeElement>();

	        var element = elementTag.GetElementById(elementCollection.Elements, ((AttributeConstraint)constraint).Value);

	        if (element != null)
	        {
	            FinishedAddingChildrenThatMetTheConstraints(constraint, elementTag, attributeBag, returnAfterFirstMatch, element, children);
	        }
	        return children;
	    }

	    private bool FinishedAddingChildrenThatMetTheConstraints(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IHTMLElement element, ICollection<INativeElement> children)
	    {            
	        waitUntilElementReadyStateIsComplete(element);

            var nativeElement = _domContainer.NativeBrowser.CreateElement(element);
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

	    private static bool FindByExactMatchOnIdPossible(BaseConstraint constraint)
	    {
            var attributeConstraint = constraint as AttributeConstraint;
			
            return attributeConstraint != null && 
                   StringComparer.AreEqual(attributeConstraint.AttributeName, "id", true) && 
                   !(constraint.HasAnd || constraint.HasOr) && 
                   attributeConstraint.Comparer.GetType() == typeof(StringComparer);
		}

	    private static void waitUntilElementReadyStateIsComplete(IHTMLElement element)
		{
			//TODO: See if this method could be dropped, it seems to give
			//      more trouble (uninitialized state of elements)
			//      then benefits (I just introduced this method to be on 
			//      the save side)

			if (ElementTag.IsValidElement(element, Image.ElementTags))
			{
				return;
			}

			// Wait if the readystate of an element is BETWEEN
			// Uninitialized and Complete. If it's uninitialized,
			// it's quite probable that it will never reach Complete.
			// Like for elements that could not load an image or ico
			// or some other bits not part of the HTML page.     
			var timeoutTimer = new SimpleTimer(30);

			do
			{
				var readyState = ((IHTMLElement2) element).readyStateValue;

				if (readyState == 0 || readyState == 4)
				{
					return;
				}

				Thread.Sleep(Settings.SleepTime);
			} while (!timeoutTimer.Elapsed);

			throw new WatiNException("Element didn't reach readystate = complete within 30 seconds: " + element.outerText);
		}
	}
}
