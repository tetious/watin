using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using mshtml;

namespace WatiN.Core.Native.InternetExplorer
{
    public class JScriptElementArrayEnumerator : IEnumerable<INativeElement>
    {
        private readonly IEDocument _ieDocument;
        private readonly string _fieldName;

        public JScriptElementArrayEnumerator(IEDocument ieDocument, string fieldName)
        {
            _ieDocument = ieDocument;
            _fieldName = fieldName;
        }

        public IEnumerator<INativeElement> GetEnumerator()
        {
            var result = GetPropertyValue(_fieldName, _ieDocument.HtmlDocument);

            if (result == null) yield break;

            var length = (int) GetPropertyValue("length", result);
            for (var i = 0; i < length; i++)
            {
                var element1 = GetPropertyValue(i.ToString(), result) as IHTMLElement;
                if (element1 != null) yield return new IEElement(element1);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object GetPropertyValue(string propertyName, object hasExpandoInterface)
        {
            var expando = (IExpando)hasExpandoInterface;

            var property = expando.GetProperty(propertyName, BindingFlags.Default);
            if (property != null)
            {
                try
                {
                    return property.GetValue(hasExpandoInterface, null);
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