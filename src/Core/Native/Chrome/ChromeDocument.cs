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

namespace WatiN.Core.Native.Chrome
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// The chrome document class, giving access to common document elements.
    /// </summary>
    public class ChromeDocument : INativeDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeDocument"/> class.
        /// </summary>
        /// <param name="clientPort">The client port.</param>
        public ChromeDocument(ClientPortBase clientPort)
        {
            this.ClientPort = clientPort;
            this.DocumentReference = "document";
        }

        /// <summary>
        /// Gets the FireFox client port.
        /// </summary>
        public ClientPortBase ClientPort { get; private set; }

        /// <summary>
        /// Gets the collection of all elements in the document.
        /// </summary>
        public INativeElementCollection AllElements
        {
            get
            {
                return new ChromeElementCollection(this.ClientPort, this.DocumentReference); 
            }
        }

        /// <summary>
        /// Gets the containing frame element, or null if none.
        /// </summary>
        public INativeElement ContainingFrameElement
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the body element for the current docuemnt.
        /// </summary>
        /// <value>The body element.</value>
        public INativeElement Body
        {
            get
            {
                var bodyReference = string.Format("{0}.body", this.DocumentReference);
                return new JSElement(this.ClientPort, bodyReference);
            }
        }

        /// <summary>
        /// Gets the URL for the current document.
        /// </summary>
        /// <value>The URL for the current document.</value>
        public string Url
        {
            get
            {
                var url = this.ClientPort.WriteAndRead("{0}.location.href", this.DocumentReference);
                url = string.IsNullOrEmpty(url) ? "about:blank" : url;
                return url;
            }
        }

        /// <summary>
        /// Gets the title of the current docuemnt.
        /// </summary>
        /// <value>The title of the current document.</value>
        public string Title
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the active element.
        /// </summary>
        /// <value>The active element.</value>
        public INativeElement ActiveElement
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the name of the java script variable.
        /// </summary>
        /// <value>The name of the java script variable.</value>
        public string JavaScriptVariableName
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the list of frames.
        /// </summary>
        /// <value>The list of frames of the current document.</value>
        public IList<INativeDocument> Frames
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the document reference.
        /// </summary>
        /// <value>The document reference.</value>
        private string DocumentReference
        {
            get; set;
        }

        /// <summary>
        /// Runs the script.
        /// </summary>
        /// <param name="scriptCode">
        /// The script code to run.
        /// </param>
        /// <param name="language">
        /// The language the script was written in.
        /// </param>
        public void RunScript(string scriptCode, string language)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the value for the corresponding <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        /// <returns>
        /// The value for the corresponding <paramref name="propertyName"/>.
        /// </returns>
        public string GetPropertyValue(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the bounds of all matching text substrings within the document.
        /// </summary>
        /// <param name="text">
        /// The text to find
        /// </param>
        /// <returns>
        /// The text bounds in screen coordinates
        /// </returns>
        public IEnumerable<Rectangle> GetTextBounds(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}