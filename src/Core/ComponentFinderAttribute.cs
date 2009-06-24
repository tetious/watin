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

namespace WatiN.Core
{
    /// <summary>
    /// Abstract base class for attributes that are used to find components based on a
    /// declarative description.
    /// </summary>
    /// <seealso cref="FindByAttribute"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ComponentFinderAttribute : Attribute
    {
        /// <summary>
        /// Finds a component of the specified type within a container.
        /// </summary>
        /// <param name="componentType">The component type, not null.</param>
        /// <param name="container">The container, not null.</param>
        /// <returns>The component that was found, or null if not found.</returns>
        public abstract Component FindComponent(Type componentType, IElementContainer container);
    }
}
