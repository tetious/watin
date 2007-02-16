#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006-2007 Jeroen van Menen
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
  /// Represent the CSS style rule.
  /// </summary>
  public class Style
  {
    private IHTMLStyle style;

	/// <summary>
	/// Initializes a new instance of the <see cref="Style"/> class.
	/// </summary>
	/// <param name="style">The underlying <see cref="IHTMLStyle"/>.</param>
    public Style(IHTMLStyle style)
    {
      this.style = style;
    }

    /// <summary>
    /// Retrieves the color of the text of the element.
    /// Visit http://msdn.microsoft.com/workshop/author/dhtml/reference/colors/colors_name.asp
    /// for a full list of supported RGB colors and their names.
    /// </summary>
    /// <value>The color of the text.</value>
    public string Color
    {
      get { return GetAttributeValue("color"); }
    }

    /// <summary>
    /// Retrieves the color behind the content of the element.
    /// Visit http://msdn.microsoft.com/workshop/author/dhtml/reference/colors/colors_name.asp
    /// for a full list of supported RGB colors and their names.
    /// </summary>
    /// <value>The color of the background.</value>
    public string BackgroundColor
    {
      get { return GetAttributeValue("backgroundcolor"); }
    }

    /// <summary>
    /// Retrieves the name of the font used for text in the element.
    /// </summary>
    /// <value>The font family.</value>
    public string FontFamily
    {
      get { return GetAttributeValue("fontfamily"); }
    }

    /// <summary>
    /// Retrieves a value that indicates the font size used for text in the element. 
    /// </summary>
    /// <value>The size of the font.</value>
    public string FontSize
    {
      get { return GetAttributeValue("fontsize"); }
    }

    /// <summary>
    /// Retrieves the font style of the element as italic, normal, or oblique.
    /// </summary>
    /// <value>The fount style.</value>
    public string FontStyle
    {
      get { return GetAttributeValue("fontstyle"); }
    }

    /// <summary>
    /// Retrieves the height of the element.
    /// </summary>
    /// <value>The height of the element.</value>
    public string Height
    {
      get { return GetAttributeValue("height"); }
    }

	/// <summary>
	/// Retrieves wheter the object is rendered.
	/// </summary>
	/// <remarks>For a complete list of the values visit 
	/// http://msdn.microsoft.com/workshop/author/dhtml/reference/properties/display.asp .</remarks>
	/// <value>The display mode.</value>
  	public string Display
  	{
	  get { return GetAttributeValue("display"); }
  	}

    /// <summary>
    /// Retrieves the CSS text.
    /// </summary>
    /// <value>The CSS text.</value>
    public string CssText
    {
      get { return GetAttributeValue("csstext"); }
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// The value of <see cref="CssText"/>.
    /// </returns>
    public override string ToString()
    {
      return CssText;
    }

    /// <summary>
    /// This methode can be used if the attribute isn't available as a property of
    /// of this <see cref="Style"/> class.
    /// </summary>
    /// <param name="attributeName">The attribute name. This could be different then named in
    /// the HTML. It should be the name of the property exposed by IE on it's style object.</param>
    /// <returns>The value of the attribute if available; otherwise <c>null</c> is returned.</returns>
    public string GetAttributeValue(string attributeName)
    {
      if (UtilityClass.IsNullOrEmpty(attributeName))
      {
        throw new ArgumentNullException("attributeName", "Null or Empty not allowed.");
      }

      object attribute = GetAttributeValue(attributeName, style);

      if (attribute == DBNull.Value || attribute == null)
      {
        return null;
      }

      return attribute.ToString();
    }

    internal static object GetAttributeValue(string attributeName, IHTMLStyle style)
    {
      if (attributeName.IndexOf(Char.Parse("-")) > 0)
      {
        attributeName = attributeName.Replace("-", "");
      }

      return style.getAttribute(attributeName, 0);
    }

  }
}
