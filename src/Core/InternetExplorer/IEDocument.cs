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
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.InternetExplorer
{
    public class IEDocument : INativeDocument
    {
        private readonly IHTMLDocument2 _nativeDocument;

        public IEDocument(object document)
        {
            if (!(document is IHTMLDocument2)) throw new ArgumentException("document should be of type IHTMLDocument2");

            _nativeDocument = (IHTMLDocument2) document;
        }

        public object Object
        {
            get { return _nativeDocument; }
        }

        public object Objects
        {
            get { return _nativeDocument.all; }
        }

        public INativeElement Body
        {
            get
            {
                return _nativeDocument.body != null ? new IEElement(_nativeDocument.body) : null;
            }
        }

        public string Url
        {
            get { return _nativeDocument.url; }
        }

        public string Title
        {
            get { return _nativeDocument.title; }
        }

        public INativeElement ActiveElement
        {
            get { return new IEElement(_nativeDocument.activeElement); }
        }

        public void RunScript(string scriptCode, string language)
        {
            Logger.LogDebug(scriptCode);
            UtilityClass.RunScript(scriptCode, language, _nativeDocument.parentWindow);
        }

        public string JavaScriptVariableName
        {
            get { return "document"; }
        }

        public List<Frame> Frames(DomContainer domContainer)
        {
            var processor = new AllFramesProcessor(domContainer, (HTMLDocument)_nativeDocument);

            NativeMethods.EnumIWebBrowser2Interfaces(processor);

            return processor.elements;
        }

        public string GetPropertyValue(string propertyName)
        {
            var domDocumentExpando = (IExpando)_nativeDocument;

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