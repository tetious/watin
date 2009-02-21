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
using WatiN.Core.Native;
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
	internal sealed class IEElementFinder : NativeElementFinder
	{
        public IEElementFinder(IList<ElementTag> elementTags, BaseConstraint constraint, IElementCollection elementCollection, DomContainer domContainer)
            : base(elementTags, constraint, elementCollection, domContainer)
        {
        }

        /// <inheritdoc />
        protected override ElementFinder FilterImpl(BaseConstraint findBy)
        {
            return new IEElementFinder(ElementTags, Constraint & findBy, ElementCollection, DomContainer);
        }

        /// <inheritdoc />
        protected override IEnumerable<Element> FindElementsByTag(ElementTag elementTag)
        {
            var elements = GetElementCollection((IHTMLElementCollection)ElementCollection.Elements, elementTag.TagName);

	        if (elements != null)
	        {
	            // Loop through each element and evaluate
	            var length = elements.length;
	            for (var index = 0; index < length; index++ )
                {
                    var htmlElement = (IHTMLElement)elements.item(index, null);
                    var element = WrapElementIfMatch(new IEElement(htmlElement));
                    if (element != null)
                        yield return element;
                }
	        }
	    }

        /// <inheritdoc />
        protected override IEnumerable<Element> FindElementsById(string id)
	    {
	        var htmlElements = ElementCollection.Elements as IHTMLElementCollection3;
            if (htmlElements != null)
            {
                var htmlItem = htmlElements.namedItem(id);
                var htmlElement = htmlItem as IHTMLElement;

                if (htmlElement == null && (htmlItem as IHTMLElementCollection) != null)
                    htmlElement = (IHTMLElement) ((IHTMLElementCollection) htmlItem).item(null, 0);

                if (htmlElement != null)
                {
                    var element = WrapElementIfMatch(new IEElement(htmlElement));
                    if (element != null)
                        yield return element;
                }
            }
	    }

        private static IHTMLElementCollection GetElementCollection(IHTMLElementCollection elements, string tagName)
        {
            if (elements == null) return null;

            if (tagName == null) return elements;

            return (IHTMLElementCollection)elements.tags(tagName);
        }
	}
}
