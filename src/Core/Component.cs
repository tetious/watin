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
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
    /// <summary>
    /// Describes a WatiN component such as an element, document, browser or custom control
    /// which may be located using various constraints.
    /// </summary>
    public abstract class Component : IAttributeBag, IHasDescription
    {
        /// <summary>
        /// Gets or sets the description of the component, or null if none.
        /// </summary>
        public string Description { get; set; }

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
        public virtual bool Matches(Constraint constraint)
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
        public virtual bool Matches(Constraint constraint, ConstraintContext context)
        {
            if (constraint == null)
                throw new ArgumentNullException("constraint");
            if (context == null)
                throw new ArgumentNullException("context");

            return MatchesImpl(constraint, context);
        }

        /// <inheritdoc />
        public virtual string GetAttributeValue(string attributeName)
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
        public virtual string GetValue(string attributeName)
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
