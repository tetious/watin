using System;
using System.Collections.Generic;

namespace WatiN.Core.Native.Mozilla
{
    internal class FFElementCollection : JSElementCollectionBase
    {
        public FFElementCollection(FireFoxClientPort clientPort, string containerReference) : base(clientPort, containerReference)
        {
        }

        protected override IEnumerable<INativeElement> GetElementByTagImpl(string tagName)
        {
            if (tagName == null)
                throw new ArgumentNullException("tagName");

            this.Initialize();

            var elementArrayReference = FireFoxClientPort.CreateVariableName();
            var command = string.Format(
                    "{0} = {1}.getElementsByTagName(\"{2}\");"
                    + "{0}.length;", elementArrayReference, this.containerReference, tagName);

            int numberOfElements = this.clientPort.WriteAndReadAsInt(command);

            try
            {
                for (var index = 0; index < numberOfElements; index++)
                {
                    var elementReference = string.Concat(elementArrayReference, "[", index.ToString(), "]");
                    var ffElement = new FFElement((FireFoxClientPort)this.clientPort, elementReference);
                    ffElement.ReAssignElementReference();
                    yield return ffElement;
                }
            }
            finally
            {
                command = string.Format("delete {0};", elementArrayReference);
                this.clientPort.Write(command);
            }
        }

        protected override IEnumerable<INativeElement> GetElementsByIdImpl(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            this.Initialize();

            var documentReference = GetDocumentReference(this.containerReference);

            var elementReference = FireFoxClientPort.CreateVariableName();
            var command = string.Format("{0} = {1}.getElementById(\"{2}\"); {0} != null", elementReference, documentReference, id);

            if (this.clientPort.WriteAndReadAsBool(command))
                yield return new FFElement((FireFoxClientPort)this.clientPort, elementReference);
        }
    }
}
