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
  /// A typed collection of <see cref="Div" /> instances within a <see cref="Document"/> or <see cref="Element"/>.
  /// </summary>
  public class DivCollection : BaseElementCollection
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DivCollection"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The DOM container.</param>
    /// <param name="finder">The finder.</param>
    public DivCollection(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder, new CreateElementInstance(New))
    {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DivCollection"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The DOM container.</param>
    /// <param name="elements">The elements.</param>
    public DivCollection(DomContainer domContainer, ArrayList elements) : base(domContainer, elements, new CreateElementInstance(New))
    {}

    /// <summary>
    /// Gets the <see cref="Div"/> at the specified index.
    /// </summary>
    /// <value></value>
    public Div this[int index] 
    {
      get
      {
        return new Div(domContainer,(IHTMLDivElement)Elements[index]);
      } 
    }
    
    public DivCollection Filter(Attribute findBy)
    {      
      return new DivCollection(domContainer, DoFilter(findBy));
    }
    
    private static Element New(DomContainer domContainer, IHTMLElement element)
    {
      return new Div(domContainer, (IHTMLDivElement)element);
    }
  }
}
