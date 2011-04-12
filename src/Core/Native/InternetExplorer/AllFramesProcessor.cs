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

using System.Collections.Generic;
using mshtml;
using SHDocVw;
using WatiN.Core.Exceptions;

namespace WatiN.Core.Native.InternetExplorer
{
    internal class AllFramesProcessor : IWebBrowser2Processor
    {
        public List<INativeDocument> Elements { get; private set; }

        private readonly HTMLDocument _htmlDocument;
        private readonly IHTMLElementCollection _iFrameElements;
        private int _index;

        public AllFramesProcessor(HTMLDocument htmlDocument)
        {
            Elements = new List<INativeDocument>();
            _htmlDocument = htmlDocument;

            _iFrameElements = (IHTMLElementCollection)htmlDocument.all.tags("iframe");
        }

        public HTMLDocument HTMLDocument()
        {
            return _htmlDocument;
        }

        public void Process(IWebBrowser2 webBrowser2)
        {
            var htmlDocument2 = (IHTMLDocument2)webBrowser2.Document;
            var containingFrameElement = GetContainingFrameElement();
            Elements.Add(new IEDocument(htmlDocument2, containingFrameElement));

            _index++;
        }

        public IEElement GetContainingFrameElement()
        {
            var uniqueId = RetrieveUniqueIdOfFrameElement();
            var frameElement = RetrieveSameFrameFromHtmlDocument(uniqueId);
            return new IEElement(frameElement);
        }

        private string RetrieveUniqueIdOfFrameElement()
        {
            var frame = _iFrameElements.length == 0 ? 
                        FrameByIndexProcessor.GetFrameFromHTMLDocument(_index, _htmlDocument) :
                        _iFrameElements.item(_index, null);

            return new Expando(frame).GetValue<string>("uniqueID");
        }

        // This is lookup in htmldocument is needed to bridge between 
        // two different "memory" representations of document and its frame elements.
        private IHTMLElement2 RetrieveSameFrameFromHtmlDocument(string frameElementUniqueId)
        {
            var frame = _htmlDocument.getElementById(frameElementUniqueId) as IHTMLElement2;
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