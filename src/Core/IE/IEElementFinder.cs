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
using System.Threading;
using mshtml;
using WatiN.Core.Constraints;
using WatiN.Core.InternetExplorer;
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
		private ArrayList tagsToFind = new ArrayList();

		protected readonly BaseConstraint _findBy;
		protected readonly IElementCollection _elementCollection;
	    protected readonly DomContainer _domContainer;

	    public IEElementFinder(ArrayList elementTags, BaseConstraint findBy, IElementCollection elementCollection, DomContainer domContainer)
		{
			if (elementCollection == null) throw new ArgumentNullException("elementCollection");
            if (domContainer == null) throw new ArgumentNullException("domContainer");

			_findBy = getFindBy(findBy);
			_elementCollection = elementCollection;
		    _domContainer = domContainer;

		    if (elementTags != null)
			{
				tagsToFind = elementTags;
			}
			else
			{
				AddElementTag(null, null);
			}
		}

        public IEElementFinder(ArrayList elementTags, IElementCollection elementCollection, DomContainer domContainer) : this(elementTags, null, elementCollection, domContainer) { }

		public IEElementFinder(string tagName, string inputType, BaseConstraint findBy, IElementCollection elementCollection, DomContainer domContainer)
		{
			if (elementCollection == null) throw new ArgumentNullException("elementCollection");
			if (domContainer == null) throw new ArgumentNullException("domContainer");

			_findBy = getFindBy(findBy);
			_elementCollection = elementCollection;

			AddElementTag(tagName, inputType);
		}

		public IEElementFinder(string tagName, string inputType, IElementCollection elementCollection, DomContainer domContainer) : this(tagName, inputType, null, elementCollection, domContainer) {}

		public virtual INativeElement FindFirst()
		{
		    return FindFirst(_findBy);
		}

        public virtual INativeElement FindFirst(BaseConstraint findBy)
		{
			foreach (ElementTag elementTag in tagsToFind)
			{
				ArrayList elements = findElementsByAttribute(elementTag, findBy, true);

				if (elements.Count > 0)
				{
					return new IEElement(elements[0]);
				}
			}

			return null;
		}

		public string ElementTagsToString
		{
			get { return ElementTag.ElementTagsToString(tagsToFind); }
		}

		public string ConstriantToString
		{
			get { return _findBy.ConstraintToString(); }
		}

		public void AddElementTag(string tagName, string inputType)
		{
			tagsToFind.Add(new ElementTag(tagName, inputType));
		}

		public ArrayList FindAll()
		{
			return FindAll(_findBy);
		}

		public ArrayList FindAll(BaseConstraint findBy)
		{
		    if (tagsToFind.Count == 1)
			{
				return findElementsByAttribute((ElementTag) tagsToFind[0], findBy, false);
			}

		    return FindAllWithMultipleTags(findBy);
		}

	    private ArrayList FindAllWithMultipleTags(BaseConstraint findBy)
	    {
	        ArrayList elements = new ArrayList();

	        foreach (ElementTag elementTag in tagsToFind)
	        {
	            elements.AddRange(findElementsByAttribute(elementTag, findBy, false));
	        }

	        return elements;
	    }

	    private static BaseConstraint getFindBy(BaseConstraint findBy)
		{
			if (findBy == null)
			{
				return new AlwaysTrueConstraint();
			}
			return findBy;
		}

		private ArrayList findElementsByAttribute(ElementTag elementTag, BaseConstraint findBy, bool returnAfterFirstMatch)
		{
			// Get elements with the tagname from the page
		    findBy.Reset();
            ElementAttributeBag attributeBag = new ElementAttributeBag(_domContainer);

            if (FindByExactMatchOnId(findBy))
            {
                return FindElementById(findBy, elementTag, attributeBag, returnAfterFirstMatch);
            }
            return FindElements(findBy, elementTag, attributeBag, returnAfterFirstMatch, elementTag.GetElementCollection(_elementCollection.Elements));
		}

	    public static ArrayList FindElements(BaseConstraint findBy, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IHTMLElementCollection elements)
	    {
            ArrayList children = new ArrayList();

	        if (elements != null)
	        {

	            // Loop through each element and evaluate
	            foreach (IHTMLElement element in elements)
	            {
                    if (!FinishedAddingChildrenThatMetTheConstraints(findBy, elementTag, attributeBag, returnAfterFirstMatch, element, ref children))
	                {
	                    return children;
	                }
	            }
	        }

	        return children;
	    }

	    private ArrayList FindElementById(BaseConstraint findBy, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch)
	    {
            ArrayList children = new ArrayList();

	        IHTMLElement element = elementTag.GetElementById(_elementCollection.Elements, ((AttributeConstraint)findBy).Value);

	        if (element != null)
	        {
	            FinishedAddingChildrenThatMetTheConstraints(findBy, elementTag, attributeBag, returnAfterFirstMatch, element, ref children);
	        }
	        return children;
	    }

	    private static bool FinishedAddingChildrenThatMetTheConstraints(BaseConstraint findBy, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IHTMLElement element, ref ArrayList children)
	    {            
	        waitUntilElementReadyStateIsComplete(element);

	        attributeBag.IHTMLElement = element;

	        bool validElementType = true;
            if (elementTag.IsInputElement)
            {
                validElementType = elementTag.CompareInputTypes(new IEElement(element));
            }

	        if (validElementType && findBy.Compare(attributeBag))
	        {
	            children.Add(element);
	            if (returnAfterFirstMatch)
	            {
	                return false;
	            }
	        }
	        return true;
	    }

	    private static bool FindByExactMatchOnId(BaseConstraint findBy)
	    {
            Constraints.AttributeConstraint constraint = findBy as AttributeConstraint;
#if !NET11
			return constraint != null && constraint.AttributeName.ToLowerInvariant() == "id" && !(findBy.HasAnd || findBy.HasOr) && constraint.Comparer.GetType() == typeof(StringComparer);
#else
			return constraint != null && constraint.AttributeName.ToLower(System.Globalization.CultureInfo.InvariantCulture) == "id" && !(findBy.HasAnd || findBy.HasOr) && constraint.Comparer.GetType() == typeof(StringComparer);
#endif
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
			SimpleTimer timeoutTimer = new SimpleTimer(30);

			do
			{
				int readyState = ((IHTMLElement2) element).readyStateValue;

				if (readyState == 0 || readyState == 4)
				{
					return;
				}

				Thread.Sleep(100);
			} while (!timeoutTimer.Elapsed);

			throw new WatiNException("Element didn't reach readystate = complete within 30 seconds: " + element.outerText);
		}
	}
}