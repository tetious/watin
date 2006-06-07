#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright

using System.Collections;
using mshtml;

namespace WatiN.Core
{
  /// <summary>
  /// A typed collection of <see cref="Button" /> instances within a <see cref="DomContainer"/> or <see cref="Element"/>.
  /// </summary>
	public class ButtonCollection : IEnumerable
	{
		ArrayList elements;
		
    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonCollection"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The DOM container.</param>
    /// <param name="elements">The elements.</param>
		public ButtonCollection(DomContainer domContainer, IHTMLElementCollection elements) 
		{
			this.elements = new ArrayList();
      IHTMLElementCollection inputElements = (IHTMLElementCollection)elements.tags("input");
			
      foreach (IHTMLInputElement inputElement in inputElements)
			{
        if ("button submit image reset".IndexOf(inputElement.type) >= 0)
        {
            Button v = new Button(domContainer, (HTMLInputElement)inputElement);
            this.elements.Add(v);
        }
			}
		}

    /// <summary>
    /// Gets the length.
    /// </summary>
    /// <value>The length.</value>
		public int Length { get { return elements.Count; } }

    /// <summary>
    /// Gets the <see cref="Button"/> at the specified index.
    /// </summary>
    /// <value></value>
		public Button this[int index] { get { return (Button)elements[index]; } }
    
    /// <exclude />
		public Enumerator GetEnumerator() 
		{
			return new Enumerator(elements);
		}

		IEnumerator IEnumerable.GetEnumerator() 
		{
			return GetEnumerator();
		}

    /// <exclude />
    public class Enumerator: IEnumerator 
		{
			ArrayList children;
			int index;
      /// <summary>
      /// Initializes a new instance of the <see cref="Enumerator"/> class.
      /// </summary>
      /// <param name="children">The children.</param>
			public Enumerator(ArrayList children) 
			{
				this.children = children;
				Reset();
			}

      /// <summary>
      /// Sets the enumerator to its initial position, which is before
      /// the first element in the collection.
      /// </summary>
      /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			public void Reset() 
			{
				index = -1;
			}

      /// <summary>
      /// Advances the enumerator to the next element of the collection.
      /// </summary>
      /// <returns>
      /// 	<see langword="true"/> if the enumerator was successfully advanced to the next element;
      /// <see langword="false"/> if the enumerator has passed the end of the collection.
      /// </returns>
      /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			public bool MoveNext() 
			{
				++index;
				return index < children.Count;
			}

      /// <summary>
      /// Gets the current element in the collection.
      /// </summary>
      /// <value></value>
      /// <exception cref="T:System.InvalidOperationException">
      /// The enumerator is positioned before the first element of the collection or after the last element.
      /// </exception>
			public Button Current 
			{
				get 
				{
					return (Button)children[index];
				}
			}

      /// <summary>
      /// Gets the current element in the collection.
      /// </summary>
      /// <value></value>
      /// <exception cref="T:System.InvalidOperationException">
      /// The enumerator is positioned before the first element of the collection or after the last element.
      /// </exception>
			object IEnumerator.Current { get { return Current; } }
		}
	}
}