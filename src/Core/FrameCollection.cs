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

using System;
using System.Collections;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;

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

          IEnumUnknown eu = null;
          IOleContainer oc = htmlDocument as IOleContainer;
          int hr = oc.EnumObjects(tagOLECONTF.OLECONTF_EMBEDDINGS, out eu);
          Marshal.ThrowExceptionForHR(hr);

          try
          {
            IHTMLElementCollection frameElements = (IHTMLElementCollection)htmlDocument.all.tags(SubElementsSupport.FrameTagName);
            object pUnk = null;
            int fetched = 0;
            const int MAX_FETCH_COUNT = 1;

            // get the first embedded object
            // pUnk alloc
            hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
            Marshal.ThrowExceptionForHR(hr);

            int index = 0;
            while (hr == 0)
            {
              //QI pUnk for the IWebBrowser2 interface
              IWebBrowser2 brow = pUnk as IWebBrowser2;

              if (brow != null)
              {
                string name = "";

                // try to get the name from the frame
                try
                {
                  // Get the frame name from the HTMLDocument - this will throw an exception
                  // with cross-domain scripting
                  name = Frame.GetFrameFromHTMLDocument(index, htmlDocument).name;
                }
                catch
                {
                    name = brow.Name;
                }

                // Get the frame element from the parent document
                IHTMLElement frameElement = (IHTMLElement)frameElements.item(index, null);
                string frameid = "";
                if (frameElement != null)
                  frameid = frameElement.id;

                Frame frame = new Frame(ie, brow.Document as IHTMLDocument2, name, frameid);
                elements.Add(frame);
                index++;
              } // if(brow != null)

              // pUnk free
              Marshal.ReleaseComObject(brow);

              //get the next ebmedded object
              // pUnk alloc
              hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
              Marshal.ThrowExceptionForHR(hr);
            } // while (hr == 0)
          }
          finally
          {
            // eu free
            Marshal.ReleaseComObject(eu);
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
