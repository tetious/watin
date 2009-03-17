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
    /// An element-based constraint.
    /// </summary>
	public class ElementConstraint : Constraint 
	{
		private readonly Comparer<Element> comparer;

        /// <summary>
        /// Creates an element constraint.
        /// </summary>
        /// <param name="comparer">The comparer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is null</exception>
        public ElementConstraint(Comparer<Element> comparer)
		{
            if (comparer == null)
                throw new ArgumentNullException("comparer");

			this.comparer = comparer;
		}

        /// <summary>
        /// Gets the element comparer.
        /// </summary>
        public Comparer<Element> Comparer
        {
            get { return comparer; }
        }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            Element element = attributeBag.GetAdapter<Element>();
            if (element == null)
                throw new WatiNException("This constraint class can only be used to compare against an element");

            return comparer.Compare(element);
		}

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("Custom Constraint");
        }		
	}
}
