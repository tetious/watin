using System;
using System.Collections.Generic;

namespace WatiN.Core.Native.Mozilla
{
    internal class FFElementCollection : JSElementCollectionBase
    {
        public FFElementCollection(ClientPortBase clientPort, string containerReference)
            : base(clientPort, containerReference)
        {
        }
        
        protected override IEnumerable<INativeElement> GetElementsByIdImpl(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Initialize();

            var documentReference = GetDocumentReference(containerReference);

            var elementReference = clientPort.CreateVariableName();
            var command = string.Format("{0} = {1}.getElementById(\"{2}\"); {0} != null", elementReference, documentReference, id);

            if (clientPort.WriteAndReadAsBool(command))
                yield return new JSElement(this.clientPort, elementReference);
        }
    }
}
