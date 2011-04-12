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
using System.Collections;
using System.Collections.Generic;
using mshtml;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native.InternetExplorer
{
    internal class IEElementCollection : INativeElementCollection2
    {
        private readonly IEnumerable _htmlElementCollection;
        private readonly IEElement _element;

        public IEElementCollection(IEnumerable htmlElementCollection, IEElement element)
        {
            if (htmlElementCollection == null)
                throw new ArgumentNullException("htmlElementCollection");

            _htmlElementCollection = htmlElementCollection;
            _element = element;
        }

        /// <inheritdoc />
        public IEnumerable<INativeElement> GetElements()
        {
            return AsNative(_htmlElementCollection);
        }

        /// <inheritdoc />
        public IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            if (tagName == null)
                throw new ArgumentNullException("tagName");

            var elementCollection = _htmlElementCollection as IHTMLElementCollection;
            if (elementCollection == null) return AsNative(_htmlElementCollection);
            
            return AsNative((IHTMLElementCollection)elementCollection.tags(tagName));
        }

        /// <inheritdoc />
        public IEnumerable<INativeElement> GetElementsById(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            var htmlElementCollection3 = _htmlElementCollection as IHTMLElementCollection3;
            if (htmlElementCollection3 != null)
            {
                var htmlItem = htmlElementCollection3.namedItem(id);
                var htmlElement = htmlItem as IHTMLElement;

                if (htmlElement != null)
                {
                	yield return new IEElement(htmlElement);
                }
                else
                	if ((htmlItem as IHTMLElementCollection) != null)
                    {
                		foreach(IHTMLElement element in (IHTMLElementCollection)htmlItem)
                			yield return new IEElement(element);
                	}
            }
        }

        public IEnumerable<INativeElement> GetElementsWithQuerySelector(string selector, DomContainer domContainer)
        {
            domContainer.RunScript(new ScriptLoader().GetSizzleInstallScript());
            var container = "document";
            if (_element != null)
            {
                container = _element.GetJavaScriptElementReference();
                if (new ElementTag(_element.TagName).Equals(new ElementTag("frame")))
                    container = container + ".contentDocument";
            }

            var code = string.Format("document.___WATINRESULT = Sizzle('{0}', {1});", selector, container);
            domContainer.RunScript(code);

            return new JScriptElementArrayEnumerator((IEDocument) domContainer.NativeDocument, "___WATINRESULT");
        }

        private static IEnumerable<INativeElement> AsNative(IEnumerable htmlElementCollection)
        {
            foreach (IHTMLElement htmlElement in htmlElementCollection)
                yield return new IEElement(htmlElement);
        }
    }
}
