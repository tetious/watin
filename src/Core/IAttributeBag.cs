using System;
using System.Collections.Generic;
using System.Text;

namespace WatiN.Core
{
    /// <summary>
    /// Provides values for attributes used during constraint matching.
    /// </summary>
    public interface IAttributeBag
    {
        /// <summary>
        /// Gets the value of an attribute that can be used for constraint evaluation.
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>The attribute's associated value or null if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attributeName"/> is null</exception>
        string GetAttributeValue(string attributeName);

        /// <summary>
        /// Gets an adapter for the object to a particular type, or null if the object
        /// cannot be adapted to that type.
        /// </summary>
        /// <typeparam name="T">The adapter type</typeparam>
        /// <returns>The adapter, or null if the object cannot be adapted as requested</returns>
        T GetAdapter<T>()
            where T : class;
    }
}
