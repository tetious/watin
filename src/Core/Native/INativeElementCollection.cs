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

using System;
using System.Collections.Generic;

namespace WatiN.Core.Native
{
    /// <summary>
    /// Provides access to the native elements contained within an element or document.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The collection recursively enumerates all elements within a subtree of the document.
    /// </para>
    /// </remarks>
    public interface INativeElementCollection
    {
        /// <summary>
        /// Gets the native elements within the collection.
        /// </summary>
        /// <returns>The enumeration of native elements</returns>
        IEnumerable<INativeElement> GetElements();

        /// <summary>
        /// Gets the native elements within the collection by tag name.
        /// </summary>
        /// <param name="tagName">The tag name to search for</param>
        /// <returns>The enumeration of native elements</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tagName"/> is null</exception>
        IEnumerable<INativeElement> GetElementsByTag(string tagName);

        /// <summary>
        /// Gets the native elements within the collection by id.
        /// </summary>
        /// <param name="id">The id to find</param>
        /// <returns>The enumeration of native elements</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is null</exception>
        IEnumerable<INativeElement> GetElementsById(string id);
    }
}
