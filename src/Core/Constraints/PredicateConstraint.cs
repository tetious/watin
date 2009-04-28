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
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
    /// <summary>
    /// A predicate-based constraint.
    /// </summary>
	public class PredicateConstraint<T> : Constraint
        where T : class
	{
		private readonly Predicate<T> predicate;

        /// <summary>
        /// Creates a predicate constraint.
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null</exception>
        public PredicateConstraint(Predicate<T> predicate)
		{
            if (predicate == null)
                throw new ArgumentNullException("predicate");

			this.predicate = predicate;
		}

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            var typeToConvertTo = typeof(T);
            if (typeToConvertTo.IsSubclassOf(typeof(Control)))
            {
                if (attributeBag.GetType().IsSubclassOf(typeof(Element)))
                {
                    T control = Control.CreateControl(typeToConvertTo, (Element) attributeBag) as T;
                    return predicate(control);
                }
            }
            else
            {
                T value = attributeBag.GetAdapter<T>();
                if (value == null)
                    throw new WatiNException(string.Format("The PredicateConstraint class can only be used to compare against values adaptable to {0}.", typeToConvertTo));

                return predicate(value);
            }
            return false;
        }

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("Predicate Constraint");
        }		
	}
}
