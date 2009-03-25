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
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using mshtml;
using WatiN.Core.Logging;

namespace WatiN.Core.Native.InternetExplorer
{
    public class IEDocument : INativeDocument
    {
        private readonly IHTMLDocument2 htmlDocument;
        private readonly IEElement containingFrameElement;

        public IEDocument(IHTMLDocument2 htmlDocument)
            : this(htmlDocument, null)
        {
        }

        public IEDocument(IHTMLDocument2 htmlDocument, IEElement containingFrameElement)
        {
            if (htmlDocument == null)
                throw new ArgumentNullException("htmlDocument");

            this.htmlDocument = htmlDocument;
            this.containingFrameElement = containingFrameElement;
        }

        /// <summary>
        /// Gets the underlying <see cref="IHTMLDocument2" /> object.
        /// </summary>
        public IHTMLDocument2 HtmlDocument
        {
            get { return htmlDocument; }
        }

        /// <inheritdoc />
        public INativeElementCollection AllElements
        {
            get { return new IEElementCollection(htmlDocument.all); }
        }

        /// <inheritdoc />
        public INativeElement ContainingFrameElement
        {
            get { return containingFrameElement; }
        }

        public INativeElement Body
        {
            get
            {
                return htmlDocument.body != null ? new IEElement(htmlDocument.body) : null;
            }
        }

        public string Url
        {
            get { return htmlDocument.url; }
        }

        public string Title
        {
            get { return htmlDocument.title; }
        }

        public INativeElement ActiveElement
        {
            get { return new IEElement(htmlDocument.activeElement); }
        }

        public void RunScript(string scriptCode, string language)
        {
            Logger.LogDebug(scriptCode);
            IEUtils.RunScript(scriptCode, language, htmlDocument.parentWindow);
        }

        public string JavaScriptVariableName
        {
            get { return "document"; }
        }

        public IList<INativeDocument> Frames
        {
            get
            {
                var processor = new AllFramesProcessor((HTMLDocument)htmlDocument);
                IEUtils.EnumIWebBrowser2Interfaces(processor);
                return processor.Elements;
            }
        }

        public string GetPropertyValue(string propertyName)
        {
            var domDocumentExpando = (IExpando)htmlDocument;

            var property = domDocumentExpando.GetProperty(propertyName, BindingFlags.Default);
            if (property != null)
            {
                try
                {
                    return (string)property.GetValue(domDocumentExpando, null);
                }
                catch (COMException)
                {
                    return null;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<Rectangle> GetTextBounds(string text)
        {
            // Use the findText feature to search for text in the body
            // Add all matching ranges to the collection

            // See http://msdn2.microsoft.com/en-us/library/aa741525.aspx for details on the flags
            // Note that this is not multi-lingual

            var body = htmlDocument.body as IHTMLBodyElement;
            if (body == null)
                yield break;

            IHTMLTxtRange textRange = body.createTextRange();
            if (textRange == null)
                yield break;

            while (textRange.findText(text, 0, 0))
            {
                Rectangle rectangle = GetTextBoundsByInsertingElement(textRange, htmlDocument);
                yield return rectangle;

                // Move the pointer to just past the current range and search the balance of the doc
                textRange.moveStart("Character", textRange.htmlText.Length);

                // Not sure why, but MS find dialog uses this to get the range to the end
                textRange.moveEnd("Textedit", 1);
            }

        }

        private static Rectangle GetTextBoundsByInsertingElement(IHTMLTxtRange textRange, IHTMLDocument2 document)
        {
            // A bit of a hack: create an HTML element around the selected
            // text and get the location of that element from document.all[].
            // Note that this is actually pretty common hack for search/highlight functions:
            // http://www.pcmag.com/article2/0,2704,1166598,00.asp
            // http://www.codeproject.com/miscctrl/chtmlview_search.asp
            // http://www.itwriting.com/phorum/read.php?3,1561,1562,quote=1

            // Save the text
            string oldHtmlText = textRange.htmlText;

            // Sometimes the text range contains the containing HTML element such as a SPAN tag.
            // eg. "<SPAN id=spanValidateCode>Code and Confirm Code must match!</SPAN>"
            //
            // This is ok.  We just grab the bounds of the whole element, which happens to be the
            // one returned by parentElement().
            if (oldHtmlText != textRange.text)
            {
                return IEElement.GetHtmlElementBounds(textRange.parentElement());
            }

            // Create a unique ID
            string id = @"__WatiNTextRange_" + Guid.NewGuid();

            // Add a span tag the the HTML
            string code = String.Format("<span id=\"{0}\">{1}</span>", id, oldHtmlText);
            textRange.pasteHTML(code);

            // Get the element's position
            var element = (IHTMLElement)document.all.item(id, null);

            // Build the bounds
            Rectangle bounds = IEElement.GetHtmlElementBounds(element);

            // Restore the HTML

            // This only seems to work if the text is not immediately preceded by a span element.
            // In that case it fails because it seems to grab the parent span element when identifying
            // the 'outerHTML' and then duplicates that for each pass.
            element.outerHTML = oldHtmlText;

            // Doesn't work: Does not replace the text when pasted into place despite suggestions in implementations 
            // listed above
            //textRange.pasteHTML( oldHtml );

            return bounds;
        }
    }
}