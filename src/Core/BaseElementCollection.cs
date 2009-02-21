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
using System.Collections;
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
        : IElementCollection<TElement>
        where TElement : Element
        where TCollection : BaseElementCollection<TElement, TCollection>
	{
		private readonly DomContainer domContainer;
		private readonly ElementFinder elementFinder;
        private List<TElement> cachedElements;

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonCollection"/> class.
		/// Mainly used by WatiN internally.
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

	    /// <inheritdoc />
        public int Count
        {
            get { return CachedElements.Count; }
        }

        /// <inheritdoc />
        public void CopyTo(TElement[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            foreach (TElement element in CachedElements)
                array[arrayIndex++] = element;
        }

        /// <summary>
        /// Gets the element at the specified index in the collection.
        /// </summary>
        /// <param name="index">The zero-based index</param>
        /// <returns>The element</returns>
        public TElement this[int index]
        {
            get { return CachedElements[index]; }
        }

	    bool ICollection<TElement>.IsReadOnly
	    {
	        get { return true; }
	    }

	    /// <summary>
		/// Gets the number of elements in the collection.
		/// </summary>
		/// <value>The number of elements in the collection</value>
        [Obsolete("Use Count property instead.")]
		public int Length
		{
			get { return Count; }
		}

        /// <inheritdoc />
		public bool Exists(string elementId)
		{
			return Exists(Find.ByDefault(elementId));
		}

        /// <inheritdoc />
        public bool Exists(Regex elementId)
		{
            return Exists(Find.ByDefault(elementId));
		}

        /// <inheritdoc />
        public bool Exists(BaseConstraint findBy)
		{
            return elementFinder.Filter(findBy).Exists();
		}

        /// <inheritdoc />
        public bool Exists(Predicate<TElement> predicate)
        {
            return Exists(Find.ByElement(predicate));
        }

        /// <inheritdoc />
        public TElement First()
        {
            if (cachedElements != null)
                return cachedElements.Count != 0 ? cachedElements[0] : null;

            return (TElement) elementFinder.FindFirst();
        }

        /// <inheritdoc />
        public TElement First(BaseConstraint findBy)
        {
            return (TElement)elementFinder.Filter(findBy).FindFirst();
        }

        /// <inheritdoc />
        public TElement First(Predicate<TElement> predicate)
        {
            return First(Find.ByElement(predicate));
        }

        /// <summary>
        /// Returned a filtered view of the collection consisting only of the elements that
        /// match the given constraint.
        /// </summary>
        /// <param name="findBy">The constraint to match</param>
        /// <returns>The filtered element collection</returns>
        public TCollection Filter(BaseConstraint findBy)
        {
            return CreateFilteredCollection(elementFinder.Filter(findBy));
        }

	    /// <summary>
        /// Returned a filtered view of the collection consisting only of the elements that
        /// match the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>The filtered element collection</returns>
        public TCollection Filter(Predicate<TElement> predicate)
        {
            return Filter(Find.ByElement(predicate));
        }

        /// <inheritdoc />
	    public IEnumerator<TElement> GetEnumerator()
		{
            foreach (var element in elementFinder.FindAll())
                yield return (TElement) element;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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
        /// <param name="elementFinder">The element finder</param>
        /// <returns>The element collection</returns>
        protected abstract TCollection CreateFilteredCollection(ElementFinder elementFinder);

        IElementCollection<TElement> IElementCollection<TElement>.Filter(BaseConstraint findBy)
        {
            return Filter(findBy);
        }

        IElementCollection<TElement> IElementCollection<TElement>.Filter(Predicate<TElement> predicate)
        {
            return Filter(predicate);
        }

        private IList<TElement> CachedElements
        {
            get
            {
                if (cachedElements == null)
                {
                    cachedElements = new List<TElement>();
                    foreach (TElement element in elementFinder.FindAll())
                        cachedElements.Add(element);
                }

                return cachedElements;
            }
        }

        #region Unsupported List Methods

        int IList<TElement>.IndexOf(TElement item)
	    {
            ThrowCollectionDoesNotSupportSearchingByElement();
            return 0;
	    }

        bool ICollection<TElement>.Contains(TElement item)
        {
            ThrowCollectionDoesNotSupportSearchingByElement();
            return false;
        }

	    void IList<TElement>.Insert(int index, TElement item)
	    {
            ThrowCollectionIsReadOnly();
        }

	    void IList<TElement>.RemoveAt(int index)
	    {
            ThrowCollectionIsReadOnly();
	    }

	    TElement IList<TElement>.this[int index]
	    {
	        get { return this[index]; }
	        set { ThrowCollectionIsReadOnly(); }
	    }

        void ICollection<TElement>.Add(TElement item)
        {
            ThrowCollectionIsReadOnly();
        }

        void ICollection<TElement>.Clear()
        {
            ThrowCollectionIsReadOnly();
        }

        bool ICollection<TElement>.Remove(TElement item)
        {
            ThrowCollectionIsReadOnly();
            return false;
        }

        private static void ThrowCollectionDoesNotSupportSearchingByElement()
        {
            throw new NotSupportedException("Collection does not support searching by element.");
        }

	    private static void ThrowCollectionIsReadOnly()
        {
            throw new NotSupportedException("Collection is read-only");
        }

        #endregion
    }
}