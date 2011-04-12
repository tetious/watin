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

using System.Collections;
using System.Collections.Generic;
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
            var result = new Expando(_ieDocument.HtmlDocument).GetValue(_fieldName);

            if (result == null) yield break;

            var resultAsExpando = new Expando(result);
            var length = resultAsExpando.GetValue<int>("length");
            for (var i = 0; i < length; i++)
            {
                var element1 = resultAsExpando.GetValue(i.ToString()) as IHTMLElement;
                if (element1 != null) yield return new IEElement(element1);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}