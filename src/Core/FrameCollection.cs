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
  /// A typed collection of <see cref="Frame" /> instances within a <see cref="Document"/>.
  /// </summary>
  public class FrameCollection : IEnumerable
	{
		ArrayList elements;
		
		public FrameCollection(DomContainer ie, IHTMLDocument2 htmlDocument) 
		{
			elements = new ArrayList();
      IHTMLElementCollection frameElements = (IHTMLElementCollection)htmlDocument.all.tags(SubElementsSupport.FrameTagName);

      for (int index = 0; index < htmlDocument.frames.length; index++)
      {
        // Get the frame
        DispHTMLWindow2 thisFrame = Frame.GetFrameFromHTMLDocument(index, htmlDocument);
        
        // Get the frame element from the parent document
        IHTMLElement frameElement = (IHTMLElement)frameElements.item(index, null);

        // Create new Frame instance
        Frame frame = new Frame(ie, thisFrame.document, thisFrame.name, frameElement.id);

        elements.Add(frame);
			}
		}

		public int Length { get { return elements.Count; } }

		public Frame this[int index] { get { return (Frame)elements[index]; } }

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

			public Frame Current 
			{
				get 
				{
					return (Frame)children[index];
				}
			}

			object IEnumerator.Current { get { return Current; } }
		}
	}
}
