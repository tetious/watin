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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using mshtml;
using WatiN.Core.Logging;
using WatiN.Core.UtilityClasses;

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
            UtilityClass.RunScript(scriptCode, language, htmlDocument.parentWindow);
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

            var errorProperty = domDocumentExpando.GetProperty(propertyName, BindingFlags.Default);
            if (errorProperty != null)
            {
                try
                {
                    return (string)errorProperty.GetValue(domDocumentExpando, null);
                }
                catch (COMException)
                {
                    return null;
                }
            }

            return null;
        }
    }
}