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
using System.Drawing;
using WatiN.Core.Exceptions;

namespace WatiN.Core.Native.Mozilla
{
    public class FFDocument : INativeDocument
    {
        private readonly JSElement containingFrameElement;

        public FFDocument(ClientPortBase clientPort)
            : this(clientPort, clientPort.DocumentVariableName)
        {
        }

        public FFDocument(ClientPortBase clientPort, string documentReference)
            : this(clientPort, documentReference, null)
        {
        }

        public FFDocument(ClientPortBase clientPort, string documentReference, JSElement containingFrameElement)
        {
            DocumentReference = documentReference;
            ClientPort = clientPort;

            this.containingFrameElement = containingFrameElement;
        }

        /// <summary>
        /// Gets the FireFox client port.
        /// </summary>
        public ClientPortBase ClientPort { get; private set; }

        /// <summary>
        /// Gets the name of a variable that stores a reference to the document within FireFox.
        /// </summary>
        public string DocumentReference { get; private set; }

        /// <inheritdoc />
        public INativeElementCollection AllElements
        {
            get { return new FFElementCollection(ClientPort, DocumentReference); }
        }

        /// <inheritdoc />
        public INativeElement ContainingFrameElement
        {
            get { return containingFrameElement; }
        }

        /// <inheritdoc />
        public INativeElement Body
        {
            get
            {
                var bodyReference = string.Format("{0}.body", DocumentReference);
                return new JSElement(ClientPort, bodyReference);
            }
        }

        /// <inheritdoc />
        public string Url
        {
            get
            {
                var url = ClientPort.WriteAndRead("{0}.location.href", DocumentReference);
                url = string.IsNullOrEmpty(url) ? "about:blank" : url;
                return url;
            }
        }

        /// <inheritdoc />
        public string Title
        {
            get { return ClientPort.WriteAndRead("{0}.title", DocumentReference); }
        }

        public INativeElement ActiveElement
        {
            get
            {
                var elementvar = ClientPort.CreateVariableName();
                var command = string.Format("{0}={1}.{2};{0}==null", elementvar, DocumentReference, "activeElement");
                var result = ClientPort.WriteAndReadAsBool(command);

                return !result ? new JSElement(ClientPort, elementvar) : null;
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
            get { return DocumentReference; }
        }

        public IList<INativeDocument> Frames
        {
            get
            { 
                var frames = new List<INativeDocument>();
                PopulateFrames(frames, "frame");
                PopulateFrames(frames, "iframe");
                return frames;
            }
        }

        private void PopulateFrames(IList<INativeDocument> frames, string tagName)
        {
            foreach (JSElement frameElement in AllElements.GetElementsByTag(tagName))
            {
                var frameDocumentReference = frameElement.ElementReference + ".contentDocument";
                frames.Add(new FFDocument(ClientPort, frameDocumentReference, frameElement));
            }
        }

        public string GetPropertyValue(string propertyName)
        {
            var command = string.Format("{0}.{1};", DocumentReference, propertyName);
            
            if (propertyName == Document.ERROR_PROPERTY_NAME)
            {
                return ClientPort.WriteAndReadIgnoreError(command);
            }

            return ClientPort.WriteAndRead(command);
        }

        /// <inheritdoc />
        public IEnumerable<Rectangle> GetTextBounds(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}
