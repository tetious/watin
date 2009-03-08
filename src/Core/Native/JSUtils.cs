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

using System.Collections.Generic;

namespace WatiN.Core.Native
{
    internal static class JSUtils
    {
        public static string WrapCommandInTimer(string command)
        {
            return "window.setTimeout(function() {" + command + ";" + "}, 5);";
        }

        public static IEnumerable<JSElement> ElementArrayEnumerator(string getElementsCommand, ClientPortBase clientPort)
        {
            var ElementArrayName = clientPort.CreateVariableName();

            var numberOfElements = GetNumberOfElements(getElementsCommand, clientPort, ElementArrayName);

            try
            {
                for (var index = 0; index < numberOfElements; index++)
                {
                    var indexedElementVariableName = string.Concat(ElementArrayName, "[", index.ToString(), "]");
                    var ffElement = new JSElement(clientPort, indexedElementVariableName);

                    yield return ffElement;
                }
            }
            finally
            {
                DeleteElementArray(ElementArrayName, clientPort);
            }
        }

        private static void DeleteElementArray(string elementName, ClientPortBase clientPort)
        {
            var command = string.Format("delete {0};", elementName);

            clientPort.Write(command);
        }

        private static int GetNumberOfElements(string getElementsCommand, ClientPortBase clientPort, string elementArrayName)
        {
            var command = string.Format("{0}={1}; {0}.length;", elementArrayName, getElementsCommand);

            return clientPort.WriteAndReadAsInt(command);
        }

    }
}