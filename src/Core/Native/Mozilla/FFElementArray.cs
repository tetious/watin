using System;
using System.Collections.Generic;

namespace WatiN.Core.Native.Mozilla
{
    public class FFElementArray : INativeElementCollection
    {
        public delegate bool IsMatch(FFElement ffElement);

        public FireFoxClientPort ClientPort { get; private set; }
        public string GetCommand { get; private set; }

        public FFElementArray(FireFoxClientPort clientPort, string getCommand)
        {
            if (clientPort == null) throw new ArgumentNullException("clientPort");
            if (getCommand == null) throw new ArgumentNullException("getCommand");

            ClientPort = clientPort;
            GetCommand = getCommand;
        }

        public IEnumerable<INativeElement> GetElements()
        {
            return GetArrayElements(ffElement => true);
        }

        public IEnumerable<INativeElement> GetElementsByTag(string tagName)
        {
            return GetArrayElements(ffElement => Comparers.StringComparer.AreEqual(ffElement.TagName, tagName, true));
        }

        public IEnumerable<INativeElement> GetElementsById(string id)
        {
            return GetArrayElements(ffElement => Comparers.StringComparer.AreEqual(ffElement.GetAttributeValue("id"), id, true));
        }

        private IEnumerable<INativeElement> GetArrayElements(IsMatch constraint)
        {
            Initialize();

            var ffElements = FFUtils.ElementArrayEnumerator(GetCommand, ClientPort);

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
            ClientPort.InitializeDocument();
        }
    }
}