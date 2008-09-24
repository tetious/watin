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

using System.Collections;
using mshtml;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML input element of type 
	/// button, submit, image and reset.
	/// </summary>
#if NET11
	public class Button : Element
#else
    public class Button : Element<Button>
#endif
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
		public Button(DomContainer domContainer, IHTMLElement element) :
            base(domContainer, domContainer.NativeBrowser.CreateElement(element)) { }

		/// <summary>
		/// Initialises a new instance of the <see cref="Button"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The <see cref="DomContainer" /> the element is in.</param>
		/// <param name="finder">The input button or button element.</param>
		public Button(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="Button"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public Button(Element element) : base(element, ElementTags) {}

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

		internal new static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new Button(domContainer, element);
		}
	}
}
