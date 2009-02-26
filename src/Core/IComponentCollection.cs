using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;

namespace WatiN.Core
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