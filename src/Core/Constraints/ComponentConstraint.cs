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
using WatiN.Core.Comparers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
    /// <summary>
    /// A component-based constraint using a comparer.
    /// </summary>
	public class ComponentConstraint : Constraint 
	{
		private readonly Comparer<Component> comparer;

        /// <summary>
        /// Creates an component constraint.
        /// </summary>
        /// <param name="comparer">The comparer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is null</exception>
        public ComponentConstraint(Comparer<Component> comparer)
		{
            if (comparer == null)
                throw new ArgumentNullException("comparer");

			this.comparer = comparer;
		}

        /// <summary>
        /// Gets the component comparer.
        /// </summary>
        public Comparer<Component> Comparer
        {
            get { return comparer; }
        }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            Component component = attributeBag.GetAdapter<Component>();
            if (component == null)
                throw new WatiNException("This constraint class can only be used to compare against a component");

            return comparer.Compare(component);
		}

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("Custom Constraint");
        }		
	}
}
