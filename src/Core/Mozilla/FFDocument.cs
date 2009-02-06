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
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Mozilla
{
    public class FFDocument : INativeDocument
    {
        public FireFoxClientPort ClientPort { get; set; }

        public FFDocument(FireFoxClientPort clientPort)
        {
            ClientPort = clientPort;
        }

        public object Object
        {
            get { return FireFoxClientPort.DocumentVariableName; }
        }

        public object Objects
        {
            get { return FireFoxClientPort.DocumentVariableName; }
        }

        public INativeElement Body
        {
            get
            {
                var bodyReference = string.Format("{0}.body", FireFoxClientPort.DocumentVariableName);
                return new FFElement(bodyReference, ClientPort);
            }
        }

        public string Url
        {
            get
            {
                var url = ClientPort.WriteAndRead("{0}.location.href", FireFoxClientPort.WindowVariableName);
                url = string.IsNullOrEmpty(url) ? "about:blank" : url;
                return url;
            }
        }

        public string Title
        {
            get { return ClientPort.WriteAndRead("{0}.title", FireFoxClientPort.DocumentVariableName); }
        }

        public INativeElement ActiveElement
        {
            get
            {
                // TODO: Make "activeElement" a FireFox constant
                var propertyName = "activeElement";

                var elementvar = FireFoxClientPort.CreateVariableName();
                var command = string.Format("{0}={1}.{2};{0}==null", elementvar, FireFoxClientPort.DocumentVariableName, propertyName);
                var result = ClientPort.WriteAndReadAsBool(command);

                return !result ? new FFElement(elementvar, ClientPort) : null;
            }
        }

        public void RunScript(string scriptCode, string language)
        {
            try
            {
                ClientPort.Write(scriptCode);
            }
            catch (FireFoxException e)
            {
                throw new RunScriptException(e);
            }
        }

        public string JavaScriptVariableName
        {
            get { return FireFoxClientPort.DocumentVariableName; }
        }

        public List<Frame> Frames(DomContainer domContainer)
        {
            return new List<Frame>();
        }

        public string GetPropertyValue(string propertyName)
        {
            var command = string.Format("{0}.{1};", FireFoxClientPort.DocumentVariableName, propertyName);
            
            if (propertyName == Document.ERROR_PROPERTY_NAME)
            {
                return ClientPort.WriteAndReadIgnoreError(command);
            }

            return ClientPort.WriteAndRead(command);
        }
    }
}
