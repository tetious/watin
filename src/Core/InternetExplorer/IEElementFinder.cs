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

using System.Collections;
using System.Collections.Generic;
using mshtml;
using WatiN.Core.Constraints;
using WatiN.Core.UtilityClasses;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.InternetExplorer
{
    /// <summary>
	/// This class is mainly used internally by WatiN to find elements in
	/// an <see cref="IHTMLElementCollection"/> or <see cref="ArrayList"/> matching
	/// the given <see cref="BaseConstraint"/>.
	/// </summary>
	public class IEElementFinder : ElementFinderBase
	{
        public IEElementFinder(List<ElementTag> elementTags, IElementCollection elementCollection, DomContainer domContainer) : base(elementTags, elementCollection, domContainer) { }

        public IEElementFinder(List<ElementTag> elementTags, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer) : base(elementTags, constraint, elementCollection, domContainer) { }

		public IEElementFinder(string tagName, string inputType, IElementCollection elementCollection, DomContainer domContainer) : base(tagName, inputType, elementCollection, domContainer) {}
		
        public IEElementFinder(string tagName, string inputType, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer) : base(tagName, inputType, constraint, elementCollection, domContainer) {}

        protected override List<INativeElement> FindElements(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IElementCollection elementCollection)
	    {
            var elements = GetElementCollection((IHTMLElementCollection)elementCollection.Elements, elementTag.TagName);
            var matchingElements = new List<INativeElement>();

	        if (elements != null)
	        {
	            // Loop through each element and evaluate
	            var length = elements.length;
	            for (var index = 0; index < length; index++ )
                {
                    var element = (IHTMLElement)elements.item(index, null);
                    if (element != null && FinishedAddingChildrenThatMetTheConstraints(constraint, elementTag, attributeBag, returnAfterFirstMatch, element, matchingElements))
                    {
                        return matchingElements;
                    }
                }
	        }

	        return matchingElements;
	    }

	    protected override INativeElement FindElementById(string Id, IElementCollection elementCollection)
	    {
	        var elements = (IHTMLElementCollection)elementCollection.Elements;
	        
            if (elements == null) return null;

	        var elementCollection3 = elements as IHTMLElementCollection3;

	        if (elementCollection3 == null) return null;

	        var item = elementCollection3.namedItem(Id);
            
	        var element = item as IHTMLElement;

	        if (element == null && (item as IHTMLElementCollection) != null)
	        {
	            element = (IHTMLElement) ((IHTMLElementCollection) item).item(null, 0);
	        }

	        return element == null ? null : new IEElement(element);
	    }

        protected bool FinishedAddingChildrenThatMetTheConstraints(BaseConstraint constraint, ElementTag elementTag, ElementAttributeBag attributeBag, bool returnAfterFirstMatch, IHTMLElement element, ICollection<INativeElement> matchingElements)
        {
            var nativeElement = _domContainer.NativeBrowser.CreateElement(element);

            return FinishedAddingChildrenThatMetTheConstraints(nativeElement, attributeBag, elementTag, constraint, matchingElements, returnAfterFirstMatch);
        }

        protected override void WaitUntilElementReadyStateIsComplete(INativeElement element)
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
	        var tryActionUntilTimeOut = new TryActionUntilTimeOut(30);
            var ihtmlElement2 = ((IHTMLElement2)element.Object);
            var success = tryActionUntilTimeOut.Try(() =>
            {
                var readyState = ihtmlElement2.readyStateValue;
                return (readyState == 0 || readyState == 4);
            });

	        if (success) return;

            var ihtmlElement = ((IHTMLElement) element.Object);
            throw new WatiNException("Element didn't reach readystate = complete within 30 seconds: " + ihtmlElement.outerText);
		}

        private static IHTMLElementCollection GetElementCollection(IHTMLElementCollection elements, string tagName)
        {
            if (elements == null) return null;

            if (tagName == null) return elements;

            return (IHTMLElementCollection)elements.tags(tagName);
        }
	}
}
