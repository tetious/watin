using System;
using System.Collections.Generic;
using System.Text;
using WatiN.Core.Constraints;

namespace WatiN.Core
{
    /// <summary>
    /// Describes a WatiN component such as an element, document, browser or custom control
    /// which may be located using various constraints.
    /// </summary>
    public abstract class Component : IAttributeBag
    {
        /// <summary>
        /// Returns true if the component matches the specified constraint.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method uses a new constraint context each time.  Note that doing so will
        /// prevent stateful constraints such as <see cref="IndexConstraint" /> from working correctly.
        /// </para>
        /// </remarks>
        /// <param name="constraint">The constraint to match</param>
        /// <returns>True if the component matches the constraint</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constraint"/> is null</exception>
        public bool Matches(Constraint constraint)
        {
            if (constraint == null)
                throw new ArgumentNullException("constraint");

            return MatchesImpl(constraint, new ConstraintContext());
        }

        /// <summary>
        /// Returns true if the component matches the specified constraint.
        /// </summary>
        /// <param name="constraint">The constraint to match</param>
        /// <param name="context">The constraint context</param>
        /// <returns>True if the component matches the constraint</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constraint"/> or
        /// <paramref name="context"/> is null</exception>
        public bool Matches(Constraint constraint, ConstraintContext context)
        {
            if (constraint == null)
                throw new ArgumentNullException("constraint");
            if (context == null)
                throw new ArgumentNullException("context");

            return MatchesImpl(constraint, context);
        }

        /// <inheritdoc />
        public string GetAttributeValue(string attributeName)
        {
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            return GetAttributeValueImpl(attributeName);
        }

        /// <inheritdoc />
        public virtual T GetAdapter<T>()
            where T : class
        {
            return this as T;
        }

        /// <summary>
        /// Gets the value of an attribute that can be used for constraint evaluation.
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>The attribute's associated value or null if none</returns>
        [Obsolete("Use GetAttributeValue instead.")]
        public string GetValue(string attributeName)
        {
            return GetAttributeValue(attributeName);
        }

        /// <summary>
        /// Returns true if the component matches the specified constraint.
        /// </summary>
        /// <param name="constraint">The constraint to match, not null</param>
        /// <param name="context">The constraint context, not null</param>
        /// <returns>True if the component matches the constraint</returns>
        private bool MatchesImpl(Constraint constraint, ConstraintContext context)
        {
            return constraint.Matches(this, context);
        }

        /// <summary>
        /// Gets the value of an attribute that can be used for constraint evaluation.
        /// </summary>
        /// <param name="attributeName">The name of the attribute, not null</param>
        /// <returns>The attribute's associated value or null if none</returns>
        protected abstract string GetAttributeValueImpl(string attributeName);
    }
}
