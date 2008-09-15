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

using System;
using System.Collections;
#if !NET11
    using System.Collections.Generic;
#endif
using System.Text.RegularExpressions;
using mshtml;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
#if NET11
	/// <summary>
	/// This delegate is mainly used by <see cref="BaseElementCollection"/> to 
    /// delegate the creation of a specialized element type. 
	/// </summary>
#else
    /// <summary>
    /// This delegate is mainly used by BaseElementCollectionT to 
    /// delegate the creation of a specialized element type. 
	/// </summary>
#endif
    public delegate Element CreateElementInstance(DomContainer domContainer, IHTMLElement element);

	/// <summary>
	/// This class is mainly used by Watin internally as the base class for all 
	/// of the element collections.
	/// </summary>
#if NET11
    public abstract class BaseElementCollection : IEnumerable
#else
    public abstract class BaseElementCollection<T> : IEnumerable<T> where T:Element
#endif
	{
		protected DomContainer domContainer;

		private ArrayList elements;
		private readonly CreateElementInstance createElementInstance;
		protected INativeElementFinder finder;

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonCollection"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="finder">The finder.</param>
		/// <param name="createElementInstance">The create element instance.</param>
		protected BaseElementCollection(DomContainer domContainer, INativeElementFinder finder, CreateElementInstance createElementInstance) :
			this(domContainer, (ArrayList) null, createElementInstance)
		{
			this.finder = finder;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonCollection"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="elements">The elements.</param>
		/// <param name="createElementInstance">The create element instance.</param>
		protected BaseElementCollection(DomContainer domContainer, ArrayList elements, CreateElementInstance createElementInstance)
		{
            if (domContainer == null) throw new ArgumentNullException("domContainer");

			this.elements = elements;
			this.domContainer = domContainer;
			this.createElementInstance = createElementInstance;
		}

		/// <summary>
		/// Gets the length.
		/// </summary>
		/// <value>The length.</value>
		public int Length
		{
			get { return Elements.Count; }
		}

#if NET11
        protected Element ElementsTyped(int index)
		{
			return CreateElementInstance((IHTMLElement) Elements[index]); 
		}
#else
        protected T ElementsTyped(int index)
		{
            return (T)CreateElementInstance((IHTMLElement)Elements[index]); 
		}
#endif

        private Element CreateElementInstance(IHTMLElement element)
        {
            return createElementInstance(domContainer, element);
        }

		
		protected ArrayList Elements
		{
			get
			{
				if (elements == null)
				{
					elements = finder != null ? finder.FindAll() : new ArrayList();
				}

				return elements;
			}
		}

		public bool Exists(string elementId)
		{
			return Exists(Find.ByDefault(elementId));
		}

		public bool Exists(Regex elementId)
		{
            return Exists(Find.ByDefault(elementId));
		}

		public bool Exists(BaseConstraint findBy)
		{
			ElementAttributeBag attributeBag = new ElementAttributeBag(domContainer);

			foreach (IHTMLElement element in Elements)
			{
				attributeBag.IHTMLElement = element;
				if (findBy.Compare(attributeBag))
				{
					return true;
				}
			}

			return false;
		}

#if !NET11
        public bool Exists(Predicate<T> predicate)
        {
            return Exists(Find.ByElement(predicate));
        }
#endif

#if NET11
        public Element First()
        {
            return FindFirst(Find.First());
        }

        public Element First(BaseConstraint findBy)
        {
            return FindFirst(findBy);
        }
#else
        public T First()
        {
            return (T)FindFirst(Find.First());
        }

        public T First(BaseConstraint findBy)
        {
            return (T)FindFirst(findBy);
        }

        public T First(Predicate<T> predicate)
        {
            return (T)FindFirst(Find.ByElement(predicate));
        }
#endif

        private Element FindFirst(BaseConstraint findBy)
        {
            if (elements == null)
                return finder != null ? CreateElementInstance((IHTMLElement)finder.FindFirst(findBy).NativeElement) : null;

            return ElementsTyped(0);
        }

		protected ArrayList DoFilter(BaseConstraint findBy)
		{
			if (elements == null)
			{
			    if (finder != null)
				{
					return finder.FindAll(findBy);
				}
			    
                return new ArrayList();
			}
		    
            return FilterElements(findBy);
		}

	    private ArrayList FilterElements(BaseConstraint findBy)
	    {
	        ArrayList returnElements = new ArrayList();
	        ElementAttributeBag attributeBag = new ElementAttributeBag(domContainer);

	        foreach (IHTMLElement element in Elements)
	        {
	            attributeBag.IHTMLElement = element;

	            if (findBy.Compare(attributeBag))
	            {
	                returnElements.Add(element);
	            }
	        }

	        return returnElements;
	    }

	    /// <exclude />
		public IEnumerator GetEnumerator()
		{
			return new Enumerator(domContainer, Elements, createElementInstance);
		}

#if !NET11
	    IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new EnumeratorT(domContainer, Elements, createElementInstance);
		}
#endif

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <exclude />
		public class Enumerator : IEnumerator
		{
			private readonly ArrayList children;
			private readonly DomContainer domContainer;
			private readonly CreateElementInstance createElementInstance;
			private int index;

			/// <exclude />
			public Enumerator(DomContainer domContainer, ArrayList children, CreateElementInstance createElementInstance)
			{
				this.children = children;
				this.domContainer = domContainer;
				this.createElementInstance = createElementInstance;

				Reset();
			}

			/// <exclude />
			public void Reset()
			{
				index = -1;
			}

			/// <exclude />
			public bool MoveNext()
			{
				++index;
				return index < children.Count;
			}

			/// <exclude />
			public virtual object Current
			{
				get { return createElementInstance(domContainer, (IHTMLElement) children[index]); }
			}

			/// <exclude />
			object IEnumerator.Current
			{
				get { return Current; }
			}
		}

#if !NET11
        /// <exclude />
        public class EnumeratorT : IEnumerator<T>
        {
            private ArrayList children;
            private DomContainer domContainer;
            private CreateElementInstance createElementInstance;
            private int index;

            /// <exclude />
            public EnumeratorT(DomContainer domContainer, ArrayList children, CreateElementInstance createElementInstance)
            {
                this.children = children;
                this.domContainer = domContainer;
                this.createElementInstance = createElementInstance;

                Reset();
            }

            public void Dispose()
            {
                children = null;
                domContainer = null;
                createElementInstance = null;
            }

            /// <exclude />
            public void Reset()
            {
                index = -1;
            }

            /// <exclude />
            public bool MoveNext()
            {
                ++index;
                return index < children.Count;
            }

            /// <exclude />
            public virtual T Current
            {
                get { return (T)createElementInstance(domContainer, (IHTMLElement)children[index]); }
            }

            /// <exclude />
            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
#endif
	}
}