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

namespace WatiN.Core.Native
{
    public class JSElementArray : INativeElementCollection
    {
        public delegate bool IsMatch(JSElement jsElement);

        public ClientPortBase ClientPort { get; private set; }
        public string GetCommand { get; private set; }

        public JSElementArray(ClientPortBase clientPort, string getCommand)
        {
            if (clientPort == null) throw new ArgumentNullException("clientPort");
            if (getCommand == null) throw new ArgumentNullException("getCommand");

            this.ClientPort = clientPort;
            this.GetCommand = getCommand;
        }

        public IEnumerable<INativeElement> GetElements()
        {
            return this.GetArrayElements(ffElement => true);
        }

        public IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            return this.GetArrayElements(ffElement => Comparers.StringComparer.AreEqual(ffElement.TagName, tagName, true));
        }

        public IEnumerable<INativeElement> GetElementsById(string id)
        {
            return this.GetArrayElements(ffElement => Comparers.StringComparer.AreEqual(ffElement.GetAttributeValue("id"), id, true));
        }

        private IEnumerable<INativeElement> GetArrayElements(IsMatch constraint)
        {
            this.Initialize();

            var ffElements = JSUtils.ElementArrayEnumerator(this.GetCommand, this.ClientPort);

            foreach (var ffElement in ffElements)
            {
                if (!constraint.Invoke(ffElement)) continue;

                ffElement.Pin();
                yield return ffElement;
            }
        }

        private void Initialize()
        {
            // In case of a redirect this call makes sure the doc variable is pointing to the "active" page.
            this.ClientPort.InitializeDocument();
        }
    }
}