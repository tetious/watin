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
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;

namespace WatiN.Core
{
	/// <summary>
	/// This class is mainly used by Watin internally as the base class for all 
	/// of the element collections.
	/// </summary>
    /// <typeparam name="TElement">The element type</typeparam>
    /// <typeparam name="TCollection">The derived collection type</typeparam>
    public abstract class BaseElementCollection<TElement, TCollection>
        : BaseComponentCollection<TElement, TCollection>, IElementCollection<TElement>
        where TElement : Element
        where TCollection : BaseElementCollection<TElement, TCollection>
	{
		private readonly DomContainer domContainer;
		private readonly ElementFinder elementFinder;

		/// <summary>
		/// Creates a base collection.
		/// </summary>
		/// <param name="domContainer">The DOM container</param>
		/// <param name="elementFinder">The element finder</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="domContainer"/>
        /// or <paramref name="elementFinder"/> is null</exception>
        protected BaseElementCollection(DomContainer domContainer, ElementFinder elementFinder)
		{
            if (domContainer == null)
                throw new ArgumentNullException("domContainer");
            if (elementFinder == null)
                throw new ArgumentNullException("elementFinder");

		    this.domContainer = domContainer;
            this.elementFinder = elementFinder;
        }

        /// <summary>
        /// Wraps all elements in the collection as controls of a particular type.
        /// </summary>
        /// <typeparam name="TControl">The <see cref="Control{TElement}" /> subclass</typeparam>
        /// <returns>The collection of controls</returns>
        public virtual ControlCollection<TControl> As<TControl>()
            where TControl : Control, new()
        {
            return ControlCollection<TControl>.CreateControlCollection(this);
        }

        /// <inheritdoc />
        public virtual bool Exists(string elementId)
		{
			return Exists(Find.ByDefault(elementId));
		}

        /// <inheritdoc />
        public virtual bool Exists(Regex elementId)
		{
            return Exists(Find.ByDefault(elementId));
		}

        /// <summary>
        /// Gets the DOM container to which the collection belongs.
        /// </summary>
        protected DomContainer DomContainer
        {
            get { return domContainer; }
        }

        /// <summary>
        /// Gets the underlying element finder.
        /// </summary>
        protected ElementFinder ElementFinder
        {
            get { return elementFinder; }
        }

	    /// <summary>
        /// Creates a filtered instance of the collection with the given finder.
        /// </summary>
        /// <param name="elementFinder">The element finder, not null</param>
        /// <returns>The element collection</returns>
        protected abstract TCollection CreateFilteredCollection(ElementFinder elementFinder);

        /// <inheritdoc />
        protected sealed override TCollection CreateFilteredCollection(Constraint findBy)
        {
            return CreateFilteredCollection(elementFinder.Filter(findBy));
        }

        /// <inheritdoc />
        protected sealed override IEnumerable<TElement> GetComponents()
        {
            foreach (TElement element in elementFinder.FindAll())
                yield return element;
        }

        IElementCollection<TElement> IElementCollection<TElement>.Filter(Constraint findBy)
        {
            return Filter(findBy);
        }

        IElementCollection<TElement> IElementCollection<TElement>.Filter(Predicate<TElement> predicate)
        {
            return Filter(predicate);
        }
    }
}