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

#if !NET20
namespace WatiN.Core
{
    /// <summary>
    /// Provides extension methods for <see cref="Component" />.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Assigns a description to a component and returns it.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="component">The component.</param>
        /// <param name="description">The description, or null if none.</param>
        /// <returns>The component.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="component"/> is null.</exception>
        public static T WithDescription<T>(this T component, string description)
            where T : IHasDescription
        {
            if (component == null)
                throw new ArgumentNullException("component");

            component.Description = description;
            return component;
        }
    }
}
#endif