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
  /// A typed collection of <see cref="Button" /> instances within a <see cref="Document"/> or <see cref="Element"/>.
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
      
      /// <exclude />
      public Enumerator(ArrayList children) 
			{
				this.children = children;
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
      public Button Current 
			{
				get 
				{
					return (Button)children[index];
				}
			}

      /// <exclude />
      object IEnumerator.Current { get { return Current; } }
		}
	}
}