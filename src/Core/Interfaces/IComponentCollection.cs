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
using WatiN.Core.Constraints;

namespace WatiN.Core.Interfaces
{
    /// <summary>
    /// Represents a read-only list of components that can be enumerated, searched and filtered.
    /// </summary>
    /// <typeparam name="T">The component type</typeparam>
    public interface IComponentCollection<T> : IList<T>
        where T : Component
    {
        /// <summary>
        /// Returns true if there exists an element within the collection
        /// that matches the given constraint.
        /// </summary>
        /// <param name="findBy">The constraint to match</param>
        /// <returns>True if a matching element exists</returns>
        bool Exists(Constraint findBy);

        /// <summary>
        /// Returns true if there exists an element within the collection
        /// that matches the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>True if a matching element exists</returns>
        bool Exists(Predicate<T> predicate);

        /// <summary>
        /// Gets the first element in the collection.
        /// </summary>
        /// <returns>The first element</returns>
        T First();

        /// <summary>
        /// Gets the first element in the collection that matches the given constraint.
        /// </summary>
        /// <param name="findBy">The constraint to match</param>
        /// <returns>True if a matching element exists</returns>
        T First(Constraint findBy);

        /// <summary>
        /// Gets the first element in the collection that matches the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>True if a matching element exists</returns>
        T First(Predicate<T> predicate);

        /// <summary>
        /// Returns a filtered view of the collection consisting only of the elements that
        /// match the given constraint.
        /// </summary>
        /// <param name="findBy">The constraint to match</param>
        /// <returns>The filtered element collection</returns>
        IComponentCollection<T> Filter(Constraint findBy);

        /// <summary>
        /// Returns a filtered view of the collection consisting only of the elements that
        /// match the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>The filtered element collection</returns>
        IComponentCollection<T> Filter(Predicate<T> predicate);
    }
}