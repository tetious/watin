#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

#if !NET11

using System;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Comparers
{
    /// <summary>
    /// This class supports comparing classes op type <see cref="Element"/> using a <see cref="Predicate{E}"/>.
    /// </summary>
    /// <typeparam name="E">An instance or sub type of type <see cref="Element"/></typeparam>
    public class PredicateElementComparer<E> : BaseComparer, ICompareElement where E : Element
    {
        private readonly Predicate<E> _compareElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateElementComparer&lt;E&gt;"/> class.
        /// </summary>
        /// <param name="predicate">The predicate will be used by <see cref="Compare(Element)"/>.</param>
        public PredicateElementComparer(Predicate<E> predicate)
        {
            _compareElement = predicate;	
        }

        /// <summary>
        /// Compares the specified element using the predicate passed in as parameter in the constructor.
        /// </summary>
        /// <param name="element">The element to evaluate.</param>
        /// <returns>The result of the comparison done by the predicate</returns>
        public virtual bool Compare(Element element)
        {
            try
            {
                return _compareElement.Invoke((E)element);
            }
            catch (Exception e)
            {
                throw new WatiNException("Exception during execution of predicate for " + element.OuterHtml, e);
            }
        }
    }
}
#endif