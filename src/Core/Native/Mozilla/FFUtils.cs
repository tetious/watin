using System.Collections.Generic;

namespace WatiN.Core.Native.Mozilla
{
    internal static class FFUtils
    {
        public static string WrapCommandInTimer(string command)
        {
            return "window.setTimeout(function() {" + command + ";" + "}, 5);";
        }

        public static IEnumerable<FFElement> ElementArrayEnumerator(string getElementsCommand, FireFoxClientPort clientPort)
        {
            var ElementArrayName = FireFoxClientPort.CreateVariableName();

            var numberOfElements = GetNumberOfElements(getElementsCommand, clientPort, ElementArrayName);

            for (var index = 0; index < numberOfElements; index++)
            {
                var indexedElementVariableName = string.Format("{0}[{1}]", ElementArrayName, index);
                var ffElement = new FFElement(indexedElementVariableName, clientPort);

                yield return ffElement;
            }

            DeReferenceElementArrayName(ElementArrayName, clientPort);
        }

        private static void DeReferenceElementArrayName(string elementName, FireFoxClientPort clientPort)
        {
            var command = string.Format("{0} = null; ", elementName);

            clientPort.Write(command);
        }

        private static int GetNumberOfElements(string getElementsCommand, FireFoxClientPort clientPort, string elementArrayName)
        {
            var command = string.Format("{0}={1}; {0}.length;", elementArrayName, getElementsCommand);

            return clientPort.WriteAndReadAsInt(command);
        }

    }
}