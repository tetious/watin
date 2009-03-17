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
using System.IO;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
    /// <summary>
    /// A combined constraint that is satisfied only when two other constraints are both satisifed.
    /// </summary>
	public sealed class AndConstraint : Constraint
	{
        private readonly Constraint first;
        private readonly Constraint second;

        /// <summary>
        /// Creates a new AND constraint.
        /// </summary>
        /// <param name="first">The first constraint</param>
        /// <param name="second">The second constraint</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="first"/> or <paramref name="second"/> is null</exception>
        public AndConstraint(Constraint first, Constraint second) 
		{
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            this.first = first;
            this.second = second;
        }

        /// <summary>
        /// Gets the first constraint that will be evaluated.
        /// </summary>
        /// <value>The first <see cref="Constraint"/>.</value>
        public Constraint First
        {
            get { return first; }
        }

        /// <summary>
        /// Gets the second constraint that will be evaluated.
        /// </summary>
        /// <value>The second <see cref="Constraint"/>.</value>
        public Constraint Second
        {
            get { return second; }
        }

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("'");
            first.WriteDescriptionTo(writer);
            writer.Write("' And '");
            second.WriteDescriptionTo(writer);
            writer.Write("'");
        }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            return first.Matches(attributeBag, context)
                && second.Matches(attributeBag, context);
        }
	}
}