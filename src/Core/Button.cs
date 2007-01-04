
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
using mshtml;

namespace WatiN.Core
{
  /// <summary>
  /// This class provides specialized functionality for a HTML input element of type 
  /// button, submit, image and reset.
  /// </summary>
  public class Button : Element
  {
    private static ArrayList elementTags;
    
    public static ArrayList ElementTags
    {
      get
      {
        if (elementTags == null)
        {
          elementTags = new ArrayList();
          elementTags.Add(new ElementTag("input", "button submit image reset"));
          elementTags.Add(new ElementTag("button"));
        }

        return elementTags;
      }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Button"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The <see cref="DomContainer" /> the element is in.</param>
    /// <param name="element">The input button or button element.</param>
    public Button(DomContainer domContainer, IHTMLElement element) : base(domContainer, element)
    {}
    
    /// <summary>
    /// Initialises a new instance of the <see cref="Button"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The <see cref="DomContainer" /> the element is in.</param>
    /// <param name="finder">The input button or button element.</param>
    public Button(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder)
    {}

    /// <summary>
    /// Initialises a new instance of the <see cref="Button"/> class based on <paramref name="element"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    public Button(Element element) : base(element, ElementTags)
    {}

    /// <summary>
    /// The text displayed at the button.
    /// </summary>
    /// <value>The displayed text.</value>
    public string Value
    {
      get { return GetAttributeValue("value"); }
    }

    /// <summary>
    /// The text displayed at the button (alias for the Value property).
    /// </summary>
    /// <value>The displayed text.</value>
    public override string Text
    {
      get { return Value; }
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      return Value;
    }
  }
}
