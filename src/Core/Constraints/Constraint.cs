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
using System.IO;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
    /// <summary>
    /// Describes a constraint that determines whether an object satisfies a given property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Constraints may maintain state across multiple match attempts so as to implement
    /// rules such as finding the Nth match.  The constraint itself should remain immutable.
    /// </para>
    /// <para>
    /// Constraints can be combined using &amp;&amp;, ||, and ! operators.
    /// </para>
    /// <para>
    /// Constraints can also be printed to a string using the <see cref="ToString()" /> method.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// If you want to find a Button by it's English or Dutch value this example shows you how to use
    /// the Or method to do this:
    /// </para>
    /// <code> 
    /// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
    /// 
    /// Button myButton = ie.Button(Find.ByValue("Cancel").Or(Find.ByValue("Annuleren")));
    /// </code>
    /// <para>
    /// You can also use the | or || operators, resulting in a bit more readable code.
    /// </para>
    /// <code> 
    /// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
    /// 
    /// Button myButton = ie.Button(Find.ByValue("Cancel") || Find.ByValue("Annuleren"));
    /// </code>
    /// </example>
    public abstract class Constraint
    {
        [ThreadStatic]
        private static Dictionary<Constraint, bool> enteredConstraints;

        /// <summary>
        /// Returns true if the constraint matches an object described by an attribute bag.
        /// </summary>
        /// <param name="attributeBag">The attribute bag</param>
        /// <param name="context">The constraint matching context</param>
        /// <returns>True if the constraint matches</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attributeBag"/> or <paramref name="context"/> is null</exception>
        public bool Matches(IAttributeBag attributeBag, ConstraintContext context)
        {
            if (attributeBag == null)
                throw new ArgumentNullException("attributeBag");
            if (context == null)
                throw new ArgumentNullException("context");

            try
            {
                EnterMatch();
                return MatchesImpl(attributeBag, context);
            }
            finally
            {
                ExitMatch();
            }
        }

        /// <summary>
        /// Combines two contraints to produce a new constraint that is satisfied only when
        /// both constraints are satisfied.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The operation is short-circuiting: if the first constraint is not satisfied for
        /// a given value then the second constraint is not evaluated.
        /// </para>
        /// <para>
        /// This makes the Find.ByName() &amp; Find.By() syntax possible and is needed for the &amp;&amp; operator.
        /// </para>
        /// </remarks>
        /// <param name="first">The first constraint</param>
        /// <param name="second">The second constraint</param>
        /// <returns>The combined constraint</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="first"/> or <paramref name="second"/> is null</exception>
        public static Constraint operator &(Constraint first, Constraint second)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            return first.And(second);
        }

        /// <summary>
        /// Combines two contraints to produce a new constraint that is satisfied only when
        /// either (or both) constraint is satisfied.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The operation is short-circuiting: if the first constraint is satisfied for
        /// a given value then the second constraint is not evaluated.
        /// </para>
        /// <para>
        /// This makes the Find.ByName() | Find.By() syntax possible and is needed for the || operator.
        /// </para>
        /// </remarks>
        /// <param name="first">The first constraint</param>
        /// <param name="second">The second constraint</param>
        /// <returns>The combined constraint</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="first"/> or <paramref name="second"/> is null</exception>
        public static Constraint operator |(Constraint first, Constraint second)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            return first.Or(second);
        }

        /// <summary>
        /// Returns a new constraint that evaluates to the opposite value of the specified constraint.
        /// </summary>
        /// <remarks>
        /// This makes the ! Find.ByName() syntax possible.
        /// </remarks>
        /// <param name="constraint">The constraint</param>
        /// <returns>The inverse constraint</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constraint"/> is null</exception>
        public static Constraint operator !(Constraint constraint)
        {
            if (constraint == null)
                throw new ArgumentNullException("constraint");

            return constraint.Not();
        }

        /// <summary>
        /// Syntax sugar to make the Find.ByName() &amp;&amp; Find.By() syntax possible.
        /// </summary>
        /// <param name="constraint">The constraint</param>
        /// <returns>Always false</returns>
        public static bool operator true(Constraint constraint)
        {
            return false;
        }

        /// <summary>
        /// Syntax sugar to make the Find.ByName() || Find.By() syntax possible.
        /// </summary>
        /// <param name="constraint">The constraint</param>
        /// <returns>Always false</returns>
        public static bool operator false(Constraint constraint)
        {
            return false;
        }

        /// <summary>
        /// Combines this constraint with another one to produce a new constraint that is satisfied
        /// only when both constraints are satisfied.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The operation is short-circuiting: if the first constraint is not satisfied for
        /// a given value then the second constraint is not evaluated.
        /// </para>
        /// </remarks>
        /// <param name="constraint">The other constraint</param>
        /// <returns>The combined constraint</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constraint"/> is null</exception>
        /// <seealso cref="Or"/>
        public Constraint And(Constraint constraint)
        {
            if (constraint == null)
                throw new ArgumentNullException("constraint");

            return new AndConstraint(this, constraint);
        }

        /// <summary>
        /// Combines this constraint with another one to produce a new constraint that is satisfied only when
        /// either (or both) constraint is satisfied.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The operation is short-circuiting: if the first constraint is satisfied for
        /// a given value then the second constraint is not evaluated.
        /// </para>
        /// </remarks>
        /// <param name="constraint">The other constraint</param>
        /// <returns>The combined constraint</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constraint"/> is null</exception>
        /// <seealso cref="And"/>
        public Constraint Or(Constraint constraint)
        {
            if (constraint == null)
                throw new ArgumentNullException("constraint");

            return new OrConstraint(this, constraint);
        }

        /// <summary>
        /// Returns a new constraint that evaluates to the opposite value of this constraint.
        /// </summary>
        /// <returns>The inverse constraint</returns>
        public Constraint Not()
        {
            return new NotConstraint(this);
        }

        /// <summary>
        /// Returns a human-readable description of the constraint.
        /// </summary>
        /// <returns>The description</returns>
        public sealed override string ToString()
        {
            StringWriter writer = new StringWriter();
            WriteDescriptionTo(writer);
            return writer.ToString();
        }

        /// <summary>
        /// Writes a human-readable description of the constraint to a text writer.
        /// </summary>
        /// <param name="writer">The text writer for the description, not null</param>
        public abstract void WriteDescriptionTo(TextWriter writer);

        /// <summary>
        /// Returns true if the constraint matches an object described by an attribute bag.
        /// </summary>
        /// <param name="attributeBag">The attribute bag, not null</param>
        /// <param name="context">The constraint matching context, not null</param>
        /// <returns>True if the constraint matches</returns>
        protected abstract bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context);

        /// <summary>
        /// Tracks when a constraint's Match method has been entered by the current thread.
        /// </summary>
        /// <exception cref="ReEntryException">Thrown if reentrance has been detected</exception>
        private void EnterMatch()
        {
            if (enteredConstraints == null)
            {
                enteredConstraints = new Dictionary<Constraint, bool>();
            }
            else
            {
                if (enteredConstraints.ContainsKey(this))
                    throw new ReEntryException(this);
            }

            enteredConstraints.Add(this, false);
        }

        /// <summary>
        /// Tracks when a constraint's Match method has been exited by the current thread.
        /// </summary>
        private void ExitMatch()
        {
            if (enteredConstraints != null)
                enteredConstraints.Remove(this);
        }
    }
}
