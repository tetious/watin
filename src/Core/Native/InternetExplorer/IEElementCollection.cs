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
using System.Collections;
using System.Collections.Generic;
using mshtml;

namespace WatiN.Core.Native.InternetExplorer
{
    internal class IEElementCollection : INativeElementCollection
    {
        private readonly IEnumerable htmlElementCollection;

        public IEElementCollection(IEnumerable htmlElementCollection)
        {
            if (htmlElementCollection == null)
                throw new ArgumentNullException("htmlElementCollection");

            this.htmlElementCollection = htmlElementCollection;
        }

        /// <inheritdoc />
        public IEnumerable<INativeElement> GetElements()
        {
            return AsNative(htmlElementCollection);
        }

        /// <inheritdoc />
        public IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            if (tagName == null)
                throw new ArgumentNullException("tagName");

            var elementCollection = htmlElementCollection as IHTMLElementCollection;
            if (elementCollection == null) return AsNative(htmlElementCollection);
            
            return AsNative((IHTMLElementCollection)elementCollection.tags(tagName));
        }

        /// <inheritdoc />
        public IEnumerable<INativeElement> GetElementsById(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            var htmlElementCollection3 = htmlElementCollection as IHTMLElementCollection3;
            if (htmlElementCollection3 != null)
            {
                var htmlItem = htmlElementCollection3.namedItem(id);
                var htmlElement = htmlItem as IHTMLElement;

                if (htmlElement == null && (htmlItem as IHTMLElementCollection) != null)
                    htmlElement = (IHTMLElement)((IHTMLElementCollection)htmlItem).item(null, 0);

                if (htmlElement != null)
                    yield return new IEElement(htmlElement);
            }
        }

        private static IEnumerable<INativeElement> AsNative(IEnumerable htmlElementCollection)
        {
            foreach (IHTMLElement htmlElement in htmlElementCollection)
                yield return new IEElement(htmlElement);
        }
    }
}
