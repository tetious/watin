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
using WatiN.Core.Constraints;

namespace WatiN.Core
{
    /// <summary>
    /// A collection of user-defined control objects.
    /// </summary>
    /// <typeparam name="TControl">The control subclass</typeparam>
    /// <seealso cref="Control{TElement}"/>
    public abstract class ControlCollection<TControl> : BaseComponentCollection<TControl, ControlCollection<TControl>>
        where TControl : Control, new()
    {
        /// <summary>
        /// Creates a control collection from an element collection.
        /// </summary>
        /// <typeparam name="TElement">The element type</typeparam>
        /// <param name="elements">The element collection to wrap</param>
        /// <returns>The control collection</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elements"/> is null</exception>
        public static ControlCollection<TControl> CreateControlCollection<TElement>(IElementCollection<TElement> elements)
            where TElement : Element
        {
            return new ElementCollectionWrapper<TElement>(elements);
        }

        private sealed class ElementCollectionWrapper<TElement> : ControlCollection<TControl>
            where TElement : Element
        {
            private readonly IElementCollection<TElement> elements;

            public ElementCollectionWrapper(IElementCollection<TElement> elements)
            {
                if (elements == null)
                    throw new ArgumentNullException("elements");

                this.elements = elements;
            }

            protected override ControlCollection<TControl> CreateFilteredCollection(Constraint findBy)
            {
                return new ElementCollectionWrapper<TElement>(elements.Filter(findBy));
            }

            protected override IEnumerable<TControl> GetElements()
            {
                foreach (TElement element in elements)
                    yield return Control.CreateControl<TControl>(element);
            }
        }
    }
}
