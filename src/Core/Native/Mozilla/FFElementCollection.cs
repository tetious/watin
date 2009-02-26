using System;
using System.Collections.Generic;

namespace WatiN.Core.Native.Mozilla
{
    internal class FFElementCollection : INativeElementCollection
    {
        private readonly FireFoxClientPort clientPort;
        private readonly string containerReference;

        public FFElementCollection(FireFoxClientPort clientPort, string containerReference)
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
            return GetElementsByTag("*");
        }

        /// <inheritdoc />
        public IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            if (tagName == null)
                throw new ArgumentNullException("tagName");

            Initialize();

            var elementArrayReference = FireFoxClientPort.CreateVariableName();
            var command = string.Format(
                "{0} = {1}.getElementsByTagName(\"{2}\");"
                + "{0}.length;", elementArrayReference, containerReference, tagName);

            int numberOfElements = clientPort.WriteAndReadAsInt(command);

            try
            {
                for (var index = 0; index < numberOfElements; index++)
                {
                    var elementReference = string.Concat(elementArrayReference, "[", index.ToString(), "]");
                    var ffElement = new FFElement(clientPort, elementReference);
                    ffElement.ReAssignElementReference();
                    yield return ffElement;
                }
            }
            finally
            {
                command = string.Format("delete {0};", elementArrayReference);
                clientPort.Write(command);
            }
        }

        /// <inheritdoc />
        public IEnumerable<INativeElement> GetElementsById(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Initialize();

            var documentReference = GetDocumentReference(containerReference);

            var elementReference = FireFoxClientPort.CreateVariableName();
            var command = string.Format("{0} = {1}.getElementById(\"{2}\"); {0} != null", elementReference, documentReference, id);

            if (clientPort.WriteAndReadAsBool(command))
                yield return new FFElement(clientPort, elementReference);
        }

        private void Initialize()
        {
            // In case of a redirect this call makes sure the doc variable is pointing to the "active" page.
            clientPort.InitializeDocument();
        }

        private static string GetDocumentReference(string referencedElement)
        {
            if (referencedElement.Contains(".") &&
                !(referencedElement.EndsWith("contentDocument") || referencedElement.EndsWith("ownerDocument")))
            {
                return referencedElement + ".ownerDocument";
            }

            return referencedElement;
        }
    }
}
