using System;
using System.Collections.Generic;

namespace WatiN.Core.Native
{
    using Mozilla;

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

            var ffElements = FFUtils.ElementArrayEnumerator(this.GetCommand, this.ClientPort);

            foreach (var ffElement in ffElements)
            {
                if (!constraint.Invoke(ffElement)) continue;

                ffElement.ReAssignElementReference();
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