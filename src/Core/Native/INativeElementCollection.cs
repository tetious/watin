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
