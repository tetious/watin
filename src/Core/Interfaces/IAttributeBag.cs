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

namespace WatiN.Core.Interfaces
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