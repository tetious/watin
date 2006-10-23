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
using mshtml;

namespace WatiN.Core
{
  /// <summary>
  /// This class provides specialized functionality for a HTML img element.
  /// </summary>
  public class Image : Element
  {
    public Image(DomContainer ie, IHTMLImgElement imgElement) : base(ie, (IHTMLElement) imgElement)
    {}
    
    public string Src
    {
      get
      {
        return ((IHTMLImgElement) DomElement).src;
      }
    }
    
    [Obsolete("The Image.Href syntax is no longer supported, use Image.Src instead.")]
    public string Href
    {
      get
      {
        return Src;
      }
    }
    
    public string Alt
    {
      get
      {
        return ((IHTMLImgElement) DomElement).alt;
      }
    }
    
    public string Name
    {
      get
      {
        return ((IHTMLImgElement) DomElement).name;
      }
    }

  }
}