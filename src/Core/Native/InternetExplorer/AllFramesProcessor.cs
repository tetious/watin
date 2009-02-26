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

using System.Collections.Generic;
using mshtml;
using SHDocVw;
using WatiN.Core.Exceptions;

namespace WatiN.Core.Native.InternetExplorer
{
    internal class AllFramesProcessor : IWebBrowser2Processor
    {
        public List<INativeDocument> Elements { get; private set; }

        private readonly HTMLDocument htmlDocument;
        private readonly IHTMLElementCollection frameElements;
        private int index;

        public AllFramesProcessor(HTMLDocument htmlDocument)
        {
            Elements = new List<INativeDocument>();

            frameElements = (IHTMLElementCollection) htmlDocument.all.tags("frame");

            // If the current document doesn't contain FRAME elements, it then
            // might contain IFRAME elements.
            if (frameElements.length == 0)
            {
                frameElements = (IHTMLElementCollection)htmlDocument.all.tags("iframe");
            }

            this.htmlDocument = htmlDocument;
        }

        public HTMLDocument HTMLDocument()
        {
            return htmlDocument;
        }

        public void Process(IWebBrowser2 webBrowser2)
        {
            // Get the frame element from the parent document
            var uniqueId = RetrieveUniqueIdOfFrameElement();

            var frameElement = RetrieveSameFrameFromHtmlDocument(uniqueId);
            var nativeFrameElement = new IEElement(frameElement);
            Elements.Add(new IEDocument((IHTMLDocument2) webBrowser2.Document, nativeFrameElement));

            index++;
        }

        private string RetrieveUniqueIdOfFrameElement()
        {
            var element = (IHTMLElement) frameElements.item(index, null);
            return ((DispHTMLBaseElement)element).uniqueID;
        }

        // This is lookup in htmldocument is needed to bridge between 
        // two different "memory" representations of document and its frame elements.
        private IHTMLElement2 RetrieveSameFrameFromHtmlDocument(string frameElementUniqueId)
        {
            var frame = htmlDocument.getElementById(frameElementUniqueId) as IHTMLElement2;
            if (frame == null)
            {
                throw new WatiNException("Couldn't find Frame or IFrame.");
            }

            return frame;
        }

        public bool Continue()
        {
            return true;
        }
    }
}