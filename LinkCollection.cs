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

namespace WatiN
{
  public class LinkCollection : IEnumerable
  {
    ArrayList elements;
		
    public LinkCollection(DomContainer ie, IHTMLElementCollection elements) 
    {
      this.elements = new ArrayList();
      IHTMLElementCollection links = (IHTMLElementCollection)elements.tags("a");

      foreach (HTMLAnchorElement link in links)
      {
        Link v = new Link(ie, link);
        this.elements.Add(v);
      }
    }

    public int length { get { return elements.Count; } }

    public Link this[int index] { get { return (Link)elements[index]; } }

    public Enumerator GetEnumerator() 
    {
      return new Enumerator(elements);
    }

    IEnumerator IEnumerable.GetEnumerator() 
    {
      return GetEnumerator();
    }

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

      public Link Current 
      {
        get 
        {
          return (Link)children[index];
        }
      }

      object IEnumerator.Current { get { return Current; } }
    }
  }
}