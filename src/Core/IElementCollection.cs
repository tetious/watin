using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;

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