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
