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
  /// A typed collection of <see cref="TextField" /> instances within a <see cref="Document"/> or <see cref="Element"/>.
  /// </summary>
	public class TextFieldCollection : IEnumerable
	{
		ArrayList children;
		
    public TextFieldCollection(DomContainer ie, IHTMLElementCollection elements) 
    {
      children = new ArrayList();
      IHTMLElementCollection inputElements = (IHTMLElementCollection)elements.tags("input");

      foreach (IHTMLInputElement inputElement in inputElements)
      {
        if ("text password textarea hidden".IndexOf(inputElement.type) >= 0)
        {
          TextField v = new TextField(ie, (HTMLInputElement)inputElement);
          children.Add(v);
        }
      }

      IHTMLElementCollection textElements = (IHTMLElementCollection)elements.tags("textarea");

      foreach (IHTMLElement textElement in textElements)
      {
        TextField v = new TextField(ie, (HTMLInputElement)textElement);
        children.Add(v);
      }
    }

		public int Length { get { return children.Count; } }

		public TextField this[int index] { get { return (TextField)children[index]; } }

    /// <exclude />
    public Enumerator GetEnumerator() 
		{
			return new Enumerator(children);
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
			public Enumerator(ArrayList children) 
			{
				this.children = children;
				Reset();
			}

			public void Reset() 
			{
				index = -1;
			}

			public bool MoveNext() 
			{
				++index;
				return index < children.Count;
			}

			public TextField Current 
			{
				get 
				{
					return (TextField)children[index];
				}
			}

			object IEnumerator.Current { get { return Current; } }
		}
	}
}