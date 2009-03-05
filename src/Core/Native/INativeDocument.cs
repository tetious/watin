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

namespace WatiN.Core.Native
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Behaviour common to all types of browser documents.
    /// </summary>
    public interface INativeDocument
    {
        /// <summary>
        /// Gets the collection of all elements in the document.
        /// </summary>
        INativeElementCollection AllElements { get; }

        /// <summary>
        /// Gets the containing frame element, or null if none.
        /// </summary>
        INativeElement ContainingFrameElement { get; }

        /// <summary>
        /// Gets the body element for the current docuemnt.
        /// </summary>
        /// <value>The body element.</value>
        INativeElement Body { get; }

        /// <summary>
        /// Gets the URL for the current document.
        /// </summary>
        /// <value>The URL for the current document.</value>
        string Url { get; }

        /// <summary>
        /// Gets the title of the current docuemnt.
        /// </summary>
        /// <value>The title of the current document.</value>
        string Title { get; }

        /// <summary>
        /// Gets the active element.
        /// </summary>
        /// <value>The active element.</value>
        INativeElement ActiveElement { get; }

        /// <summary>
        /// Gets the name of the java script variable.
        /// </summary>
        /// <value>The name of the java script variable.</value>
        string JavaScriptVariableName { get; }

        /// <summary>
        /// Gets the list of frames.
        /// </summary>
        /// <value>The list of frames of the current document.</value>
        IList<INativeDocument> Frames { get; }

        /// <summary>
        /// Runs the script.
        /// </summary>
        /// <param name="scriptCode">
        /// The script code to run.
        /// </param>
        /// <param name="language">
        /// The language the script was written in.
        /// </param>
        void RunScript(string scriptCode, string language);

        /// <summary>
        /// Gets the value for the corresponding <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        /// <returns>
        /// The value for the corresponding <paramref name="propertyName"/>.
        /// </returns>
        string GetPropertyValue(string propertyName);

        /// <summary>
        /// Gets the bounds of all matching text substrings within the document.
        /// </summary>
        /// <param name="text">
        /// The text to find
        /// </param>
        /// <returns>
        /// The text bounds in screen coordinates
        /// </returns>
        IEnumerable<Rectangle> GetTextBounds(string text);
    }
}