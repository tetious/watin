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
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
    /// <summary>
    /// Represents a read-only list of elements that can be enumerated, searched and filtered.
    /// </summary>
    /// <typeparam name="TElement">The element type</typeparam>
    public interface IElementCollection<TElement> : IComponentCollection<TElement>
        where TElement : Element
    {
        /// <summary>
        /// Wraps all elements in the collection as controls of a particular type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Control{TElement}" /> subclass</typeparam>
        /// <returns>The collection of controls</returns>
        ControlCollection<T> As<T>()
            where T : Control, new();

        /// <summary>
        /// Returns true if there exists an element within the collection
        /// that matches the given element id.
        /// </summary>
        /// <param name="elementId">The element id to match</param>
        /// <returns>True if a matching element exists</returns>
        bool Exists(string elementId);

        /// <summary>
        /// Returns true if there exists an element within the collection
        /// that matches the given element id regular expression.
        /// </summary>
        /// <param name="elementId">The element id regular expression to match</param>
        /// <returns>True if a matching element exists</returns>
        bool Exists(Regex elementId);

        /// <summary>
        /// Returns a filtered view of the collection consisting only of the elements that
        /// match the given constraint.
        /// </summary>
        /// <param name="findBy">The constraint to match</param>
        /// <returns>The filtered element collection</returns>
        new IElementCollection<TElement> Filter(Constraint findBy);

        /// <summary>
        /// Returns a filtered view of the collection consisting only of the elements that
        /// match the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>The filtered element collection</returns>
        new IElementCollection<TElement> Filter(Predicate<TElement> predicate);
    }
}