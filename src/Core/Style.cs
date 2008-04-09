#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
		private readonly IHTMLStyle style;

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

			object attributeValue = GetAttributeValue(attributeName, style);

			if (attributeValue == DBNull.Value || attributeValue == null)
			{
				return null;
			}

			return attributeValue.ToString();
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