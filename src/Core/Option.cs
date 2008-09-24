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
	/// This class provides specialized functionality for a HTML option element.
	/// </summary>
#if NET11
	public class Option : Element
#else
    public class Option : Element<Option>
#endif
	{
		private static ArrayList elementTags;

		/// <summary>
		/// Gets the element tags supported by this element
		/// </summary>
		/// <value>The element tags.</value>
		public static ArrayList ElementTags
		{
			get
			{
				if (elementTags == null)
				{
					elementTags = new ArrayList();
					elementTags.Add(new ElementTag("option"));
				}

				return elementTags;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Option"/> class.
		/// </summary>
		/// <param name="domContainer">The domContainer.</param>
        /// <param name="element">The option element.</param>
        public Option(DomContainer domContainer, IHTMLOptionElement element) :
            base(domContainer, domContainer.NativeBrowser.CreateElement(element)) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Option"/> class.
		/// </summary>
		/// <param name="domContainer">The domContainer.</param>
		/// <param name="finder">The finder.</param>
		public Option(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="Option"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public Option(Element element) : base(element, ElementTags) {}

		/// <summary>
		/// Returns the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value
		{
			get { return GetAttributeValue("value"); }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Option"/> is selected.
		/// </summary>
		/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
		public bool Selected
		{
			get { return bool.Parse(GetAttributeValue("selected")); }
		}

		/// <summary>
		/// Returns the index of this <see cref="Option"/> in the <see cref="SelectList"/>.
		/// </summary>
		/// <value>The index.</value>
		public int Index
		{
			get { return int.Parse(GetAttributeValue("index")); }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Option"/> is selected by default.
		/// </summary>
		/// <value><c>true</c> if selected by default; otherwise, <c>false</c>.</value>
		public bool DefaultSelected
		{
			get { return bool.Parse(GetAttributeValue("defaultSelected")); }
		}

		/// <summary>
		/// De-selects this option in the selectlist (if selected),
		/// fires the "onchange" event on the selectlist and waits for it
		/// to complete.
		/// </summary>
		public void Clear()
		{
			setSelected(false, true);
		}

		/// <summary>
		/// De-selects this option in the selectlist (if selected),
		/// fires the "onchange" event on the selectlist and does not wait for it
		/// to complete.
		/// </summary>
		public void ClearNoWait()
		{
			setSelected(false, false);
		}

		/// <summary>
		/// Selects this option in the selectlist (if not selected),
		/// fires the "onchange" event on the selectlist and waits for it
		/// to complete.
		/// </summary>
		public void Select()
		{
			setSelected(true, true);
		}

		/// <summary>
		/// Selects this option in the selectlist (if not selected),
		/// fires the "onchange" event on the selectlist and does not wait for it
		/// to complete.
		/// </summary>
		public void SelectNoWait()
		{
			setSelected(true, false);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return Text;
		}

		/// <summary>
		/// Gets the parent <see cref="SelectList"/>.
		/// </summary>
		/// <value>The parent <see cref="SelectList"/>.</value>
		public SelectList ParentSelectList
		{
			get { return (SelectList) Ancestor(typeof (SelectList)); }
		}

		private void setSelected(bool value, bool WaitForComplete)
		{
			if (optionElement.selected != value)
			{
				optionElement.selected = value;
				if (WaitForComplete)
				{
					ParentSelectList.FireEvent("onchange");
				}
				else
				{
					ParentSelectList.FireEventNoWait("onchange");
				}
			}
		}

		private IHTMLOptionElement optionElement
		{
			get { return (IHTMLOptionElement) HTMLElement; }
		}

		internal new static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new Option(domContainer, (IHTMLOptionElement) element);
		}
	}
}
