using System;
using mshtml;
using WatiN.Core.Interfaces;

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
    }
}