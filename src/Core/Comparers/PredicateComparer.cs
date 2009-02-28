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
using WatiN.Core.Exceptions;

namespace WatiN.Core.Comparers
{
    /// <summary>
    /// A covariant comparer implementation based on a predicate delegate.
    /// </summary>
    public class PredicateComparer<TPredicateValue, TComparerValue> : Comparer<TComparerValue>
        where TPredicateValue : TComparerValue
    {
        private readonly Predicate<TPredicateValue> predicate;

        /// <summary>
        /// Creates a predicate-based comparer.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null</exception>
        public PredicateComparer(Predicate<TPredicateValue> predicate)
		{
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            this.predicate = predicate;	
        }

        /// <summary>
        /// Compares the specified element using the predicate passed in as parameter in the constructor.
        /// </summary>
        /// <param name="element">The element to evaluate.</param>
        /// <returns>The result of the comparison done by the predicate</returns>
        public override bool Compare(TComparerValue element)
        {
            try
            {
                return element is TPredicateValue && predicate((TPredicateValue)element);
            }
            catch (Exception ex)
            {
                throw new WatiNException(string.Format("Exception occurred during execution of predicate for '{0}'", element), ex);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "satisfies predicate";
        }
    }
}