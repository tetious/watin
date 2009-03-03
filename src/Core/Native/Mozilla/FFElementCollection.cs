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

            Initialize();

            var command = string.Format("{0}.getElementsByTagName(\"{1}\");", containerReference, tagName);
            // TODO (prevent chatter): Force setting of tagName on FFElement to prevent calls to FireFox to (again) establish the tagName of the Element
            var ffElements = FFUtils.ElementArrayEnumerator(command, (FireFoxClientPort) clientPort);

            foreach (var ffElement in ffElements)
            {
                // TODO (prevent chatter): Delay reassigning until after this ffElement is known to be a match
                ffElement.ReAssignElementReference();
                yield return ffElement;
            }
        }

        protected override IEnumerable<INativeElement> GetElementsByIdImpl(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Initialize();

            var documentReference = GetDocumentReference(containerReference);

            var elementReference = FireFoxClientPort.CreateVariableName();
            var command = string.Format("{0} = {1}.getElementById(\"{2}\"); {0} != null", elementReference, documentReference, id);

            if (clientPort.WriteAndReadAsBool(command))
                yield return new FFElement((FireFoxClientPort)clientPort, elementReference);
        }
    }
}
