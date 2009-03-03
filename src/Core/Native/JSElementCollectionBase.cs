// --------------------------------------------------------------------------------------------------------------------- 
// <copyright file="JSElementCollectionBase.cs">
//   Copyright 2006-2009 Jeroen van Menen
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
// </copyright>
// <summary>
//   Defines the JSElementCollectionBase type common to all browsers that communicate with WatiN using javascript.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------
namespace WatiN.Core.Native
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base element collection common to all browsers that communicate with WatiN using javascript.
    /// </summary>
    internal abstract class JSElementCollectionBase : INativeElementCollection
    {
        protected ClientPortBase clientPort;

        protected string containerReference;

        public JSElementCollectionBase(ClientPortBase clientPort, string containerReference)
        {
            if (clientPort == null)
                throw new ArgumentNullException("clientPort");
            if (containerReference == null)
                throw new ArgumentNullException("containerReference");

            this.clientPort = clientPort;
            this.containerReference = containerReference;
        }

        /// <inheritdoc />
        public IEnumerable<INativeElement> GetElements()
        {
            return this.GetElementByTagImpl("*");
        }

        protected abstract IEnumerable<INativeElement> GetElementByTagImpl(string tagName);

        protected abstract IEnumerable<INativeElement> GetElementsByIdImpl(string id);

        protected void Initialize()
        {
            // In case of a redirect this call makes sure the doc variable is pointing to the "active" page.
            this.clientPort.InitializeDocument();
        }

        protected static string GetDocumentReference(string referencedElement)
        {
            if (referencedElement.Contains(".") &&
                !(referencedElement.EndsWith("contentDocument") || referencedElement.EndsWith("ownerDocument")))
            {
                return referencedElement + ".ownerDocument";
            }

            return referencedElement;
        }

        /// <inheritdoc />
        public virtual IEnumerable<INativeElement> GetElementsById(string id)
        {
            return this.GetElementsByIdImpl(id);
        }

        /// <inheritdoc />
        public virtual IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            return this.GetElementByTagImpl(tagName);
        }
    }
}