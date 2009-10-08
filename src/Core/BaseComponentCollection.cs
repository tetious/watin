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
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using WatiN.Core.Properties;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
    /// <summary>
    /// Represents a read-only list of components that can be enumerated, searched and filtered.
    /// </summary>
    /// <typeparam name="TComponent">The component type</typeparam>
    /// <typeparam name="TCollection">The derived collection type</typeparam>
    public abstract class BaseComponentCollection<TComponent, TCollection> : IComponentCollection<TComponent>
        where TComponent : Component
        where TCollection : BaseComponentCollection<TComponent, TCollection>
	{
        private IList<TComponent> cache;

		/// <summary>
		/// Creates a base collection.
		/// </summary>
        protected BaseComponentCollection()
		{
        }

	    /// <inheritdoc />
        public virtual int Count
        {
            get { return Cache.Count; }
        }

        /// <inheritdoc />
        public virtual void CopyTo(TComponent[] array, int arrayIndex)
        {
            Cache.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the component at the specified index in the collection.
        /// </summary>
        /// <param name="index">The zero-based index</param>
        /// <returns>The component</returns>
        public virtual TComponent this[int index]
        {
            get { return Cache[index]; }
        }

	    /// <summary>
		/// Gets the number of components in the collection.
		/// </summary>
		/// <value>The number of components in the collection</value>
        [Obsolete("Use Count property instead.")]
        public virtual int Length
		{
			get { return Count; }
		}

        /// <inheritdoc />
        public virtual bool Exists(Constraint findBy)
		{
            if (findBy == null)
                throw new ArgumentNullException("findBy");

            var context = new ConstraintContext();
            foreach (TComponent component in this)
                if (component.Matches(findBy, context))
                    return true;

            return false;
		}

        /// <inheritdoc />
        public virtual bool Exists(Predicate<TComponent> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return Exists(CreateConstraintFromPredicate(predicate));
        }

        /// <inheritdoc />
        public virtual TComponent First()
        {
            foreach (var component in this)
                return component;

            return null;
        }

        /// <inheritdoc />
        public virtual TComponent First(Constraint findBy)
        {
            if (findBy == null)
                throw new ArgumentNullException("findBy");

            var context = new ConstraintContext();
            foreach (TComponent component in this)
                if (component.Matches(findBy, context))
                    return component;

            return null;
        }

        /// <inheritdoc />
        public virtual TComponent First(Predicate<TComponent> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return First(CreateConstraintFromPredicate(predicate));
        }

        /// <summary>
        /// Returned a filtered view of the collection consisting only of the components that
        /// match the given constraint.
        /// </summary>
        /// <param name="findBy">The constraint to match</param>
        /// <returns>The filtered component collection</returns>
        public virtual TCollection Filter(Constraint findBy)
        {
            if (findBy == null)
                throw new ArgumentNullException("findBy");

            return CreateFilteredCollection(findBy);
        }

	    /// <summary>
        /// Returns a filtered view of the collection consisting only of the components that
        /// match the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>The filtered component collection</returns>
        public virtual TCollection Filter(Predicate<TComponent> predicate)
        {
            return Filter(CreateConstraintFromPredicate(predicate));
        }

        /// <inheritdoc />
        public virtual IEnumerator<TComponent> GetEnumerator()
		{
            return Cache.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

	    /// <summary>
        /// Creates a filtered instance of the collection.
        /// </summary>
        /// <param name="findBy">The constraint, not null</param>
        /// <returns>The component collection</returns>
        protected abstract TCollection CreateFilteredCollection(Constraint findBy);

        /// <summary>
        /// Gets the components of the collection.
        /// </summary>
        /// <returns>The collection components</returns>
        protected abstract IEnumerable<TComponent> GetComponents();

        /// <summary>
        /// Creates a new constraint from a given component-based predicate.
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>The constraint</returns>
        protected Constraint CreateConstraintFromPredicate(Predicate<TComponent> predicate)
        {
            return new ComponentConstraint(new PredicateComparer<TComponent, Component>(predicate));
        }

        /// <summary>
        /// Gets a lazily-populated list of all components within the collection.
        /// </summary>
        protected IList<TComponent> Cache
        {
            get
            {
                if (cache == null)
                    cache = new LazyList<TComponent>(GetComponents());
                return cache;
            }
        }

        IComponentCollection<TComponent> IComponentCollection<TComponent>.Filter(Constraint findBy)
        {
            return Filter(findBy);
        }

        IComponentCollection<TComponent> IComponentCollection<TComponent>.Filter(Predicate<TComponent> predicate)
        {
            return Filter(predicate);
        }

        #region Hidden / Unsupported List Methods

        bool ICollection<TComponent>.IsReadOnly
        {
            get { return true; }
        }

        int IList<TComponent>.IndexOf(TComponent item)
	    {
            ThrowCollectionDoesNotSupportSearchingByEquality();
            return 0;
	    }

        bool ICollection<TComponent>.Contains(TComponent item)
        {
            ThrowCollectionDoesNotSupportSearchingByEquality();
            return false;
        }

	    void IList<TComponent>.Insert(int index, TComponent item)
	    {
            ThrowCollectionIsReadOnly();
        }

	    void IList<TComponent>.RemoveAt(int index)
	    {
            ThrowCollectionIsReadOnly();
	    }

	    TComponent IList<TComponent>.this[int index]
	    {
	        get { return this[index]; }
	        set { ThrowCollectionIsReadOnly(); }
	    }

        void ICollection<TComponent>.Add(TComponent item)
        {
            ThrowCollectionIsReadOnly();
        }

        void ICollection<TComponent>.Clear()
        {
            ThrowCollectionIsReadOnly();
        }

        bool ICollection<TComponent>.Remove(TComponent item)
        {
            ThrowCollectionIsReadOnly();
            return false;
        }

        private static void ThrowCollectionDoesNotSupportSearchingByEquality()
        {
            throw new NotSupportedException(Resources.BaseComponentCollection_DoesNotSupportSearchingByEquality);
        }

	    private static void ThrowCollectionIsReadOnly()
        {
            throw new NotSupportedException(Resources.BaseComponentCollection_CollectionIsReadonly);
        }

        #endregion
    }
}