#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
    /// <summary>
    /// This delegate is mainly used by BaseElementCollectionT to 
    /// delegate the creation of a specialized element type. 
	/// </summary>
    public delegate Element CreateElementInstance(DomContainer domContainer, INativeElement element);

	/// <summary>
	/// This class is mainly used by Watin internally as the base class for all 
	/// of the element collections.
	/// </summary>
    public abstract class BaseElementCollection<T> : IEnumerable<T> where T:Element
	{
		protected DomContainer domContainer;

		private List<INativeElement> elements;
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
			this(domContainer, (IEnumerable<INativeElement>) null, createElementInstance)
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
		protected BaseElementCollection(DomContainer domContainer, IEnumerable<INativeElement> elements, CreateElementInstance createElementInstance)
		{
            if (domContainer == null) throw new ArgumentNullException("domContainer");

		    if (elements != null) this.elements = new List<INativeElement>(elements);
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

        protected T ElementsTyped(int index)
		{
            return (T)CreateElementInstance(Elements[index]); 
		}

        private Element CreateElementInstance(INativeElement element)
        {
            return createElementInstance(domContainer, element);
        }


        protected List<INativeElement> Elements
		{
			get
			{
				if (elements == null)
				{
					elements = finder != null ? new List<INativeElement>(finder.FindAll()) : new List<INativeElement>();
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
			var attributeBag = new ElementAttributeBag(domContainer);

			foreach (var element in Elements)
			{
				attributeBag.INativeElement = element;
				if (findBy.Compare(attributeBag))
				{
					return true;
				}
			}

			return false;
		}

        public bool Exists(Predicate<T> predicate)
        {
            return Exists(Find.ByElement(predicate));
        }

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

        private Element FindFirst(BaseConstraint findBy)
        {
            if (elements == null)
                return finder != null ? CreateElementInstance(finder.FindFirst(findBy)) : null;

            return ElementsTyped(0);
        }

		protected IEnumerable<INativeElement> DoFilter(BaseConstraint findBy)
		{
			if (elements == null)
			{
			    return finder != null ? finder.FindAll(findBy) : new List<INativeElement>();
			}

		    return FilterElements(findBy);
		}

	    private IEnumerable<INativeElement> FilterElements(BaseConstraint findBy)
	    {
	        var returnElements = new List<INativeElement>();
	        var attributeBag = new ElementAttributeBag(domContainer);

	        foreach (var element in Elements)
	        {
	            attributeBag.INativeElement = element;

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
	        foreach (var element in Elements)
	        {
	            yield return createElementInstance(domContainer, element);
	        }
		}

	    IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
            foreach (var element in Elements)
            {
                yield return (T)createElementInstance(domContainer, element);
            }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}