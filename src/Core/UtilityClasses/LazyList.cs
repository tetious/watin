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
using WatiN.Core.Properties;

namespace WatiN.Core.UtilityClasses
{
    /// <summary>
    /// A lazily populated list that fills itself up gradually from an enumeration on demand.
    /// </summary>
    /// <typeparam name="T">The type of element in the list</typeparam>
    internal class LazyList<T> : IList<T>
    {
        private readonly IEnumerable<T> source;
        private List<T> cache;
        private IEnumerator<T> enumerator;

        /// <summary>
        /// Creates a lazily populated list from an enumeration.
        /// </summary>
        /// <param name="source">The enumeration of elements from which to populate the list lazily</param>
        public LazyList(IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            this.source = source;
        }

        /// <summary>
        /// Gets the total number of elements in the list.
        /// </summary>
        public int Count
        {
            get
            {
                PopulateCacheUpToIndex(int.MaxValue);
                return GetCurrentCacheSize();
            }
        }

        /// <summary>
        /// Gets the element with the specified index in the list.
        /// </summary>
        /// <param name="index">The zero-based index</param>
        /// <returns>The element</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is
        /// out of range</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || !PopulateCacheUpToIndex(index))
                    throw new IndexOutOfRangeException();

                return cache[index];
            }
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            foreach (T element in this)
                array[arrayIndex++] = element;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            int index = 0;
            while (PopulateCacheUpToIndex(index))
                yield return cache[index++];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GetCurrentCacheSize()
        {
            return cache != null ? cache.Count : 0;
        }

        private bool PopulateCacheUpToIndex(int index)
        {
            if (enumerator == null)
                enumerator = source.GetEnumerator();

            for (; ; )
            {
                if (GetCurrentCacheSize() > index)
                    return true;

                if (!enumerator.MoveNext())
                    return false;

                if (cache == null)
                    cache = new List<T>();

                cache.Add(enumerator.Current);
            }
        }

        #region Hidden / Unsupported List Methods

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        int IList<T>.IndexOf(T item)
        {
            ThrowCollectionDoesNotSupportSearchingByEquality();
            return 0;
        }

        bool ICollection<T>.Contains(T item)
        {
            ThrowCollectionDoesNotSupportSearchingByEquality();
            return false;
        }

        void IList<T>.Insert(int index, T item)
        {
            ThrowCollectionIsReadOnly();
        }

        void IList<T>.RemoveAt(int index)
        {
            ThrowCollectionIsReadOnly();
        }

        T IList<T>.this[int index]
        {
            get { return this[index]; }
            set { ThrowCollectionIsReadOnly(); }
        }

        void ICollection<T>.Add(T item)
        {
            ThrowCollectionIsReadOnly();
        }

        void ICollection<T>.Clear()
        {
            ThrowCollectionIsReadOnly();
        }

        bool ICollection<T>.Remove(T item)
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
