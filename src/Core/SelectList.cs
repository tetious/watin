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
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using mshtml;
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML select element.
	/// </summary>
#if NET11
    public class SelectList : Element
#else
    public class SelectList : Element<SelectList>
#endif
	{
		private static ArrayList elementTags;

		/// <summary>
		/// Gets the element tags supported by this element
		/// </summary>
		/// <value>Arraylist with ElementTag instances.</value>
		public static ArrayList ElementTags
		{
			get
			{
				if (elementTags == null)
				{
					elementTags = new ArrayList();
					elementTags.Add(new ElementTag("select"));
				}

				return elementTags;
			}
		}

		/// <summary>
		/// Returns an initialized instance of a SelectList object.
		/// Mainly used by the collectionclass SelectLists.
		/// </summary>
		/// <param name="domContainer">The <see cref="DomContainer"/> the element is in.</param>
        /// <param name="element">The HTML select element.</param>
        public SelectList(DomContainer domContainer, IHTMLElement element) :
            base(domContainer, domContainer.NativeBrowser.CreateElement(element)) { }

		/// <summary>
		/// Returns an instance of a SelectList object.
		/// Mainly used internally.
		/// </summary>
		/// <param name="domContainer">The <see cref="DomContainer"/> the element is in.</param>
		/// <param name="finder">The element finder to use.</param>
		public SelectList(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="SelectList"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public SelectList(Element element) : base(element, ElementTags) {}

		/// <summary>
		/// This method clears the selected items in the select box and waits for the 
		/// onchange event to complete after the list is cleared
		/// </summary>
		public void ClearList()
		{
			Logger.LogAction("Clearing selection(s) in " + GetType().Name + " '" + Id + "'");

			OptionCollection options = Options.Filter(GetIsSelectedAttribute());

			foreach (Option option in options)
			{
				option.ClearNoWait();
			}

			if (options.Length > 0)
			{
				WaitForComplete();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="SelectList"/> allows multiple select.
		/// </summary>
		/// <value><c>true</c> if multiple; otherwise, <c>false</c>.</value>
		public bool Multiple
		{
			get { return bool.Parse(GetAttributeValue("multiple")); }
		}

		/// <summary>
		/// This method selects an item by text.
		/// Raises NoValueFoundException if the specified value is not found.
		/// </summary>
		/// <param name="text">The text.</param>
		public void Select(string text)
		{
			Logger.LogAction("Selecting '" + text + "' in " + GetType().Name + " '" + Id + "'");

			SelectByTextOrValue(GetTextAttribute(text));
		}

		/// <summary>
		/// This method selects an item by text using the supplied regular expression.
		/// Raises NoValueFoundException if the specified value is not found.
		/// </summary>
		/// <param name="regex">The regex.</param>
		public void Select(Regex regex)
		{
			Logger.LogAction("Selecting text using regular expresson '" + regex.ToString() + "' in " + GetType().Name + " '" + Id + "'");

			SelectByTextOrValue(Find.ByText(regex));
		}

		/// <summary>
		/// Selects an item in a select box, by value.
		/// Raises NoValueFoundException if the specified value is not found.
		/// </summary>
		/// <param name="value">The value.</param>
		public void SelectByValue(string value)
		{
			Logger.LogAction("Selecting item with value '" + value + "' in " + GetType().Name + " '" + Id + "'");

			SelectByTextOrValue(Find.ByValue(new StringEqualsAndCaseInsensitiveComparer(value)));
		}

		/// <summary>
		/// Selects an item in a select box by value using the supplied regular expression.
		/// Raises NoValueFoundException if the specified value is not found.
		/// </summary>
		/// <param name="regex">The regex.</param>
		public void SelectByValue(Regex regex)
		{
			Logger.LogAction("Selecting text using regular expresson '" + regex.ToString() + "' in " + GetType().Name + " '" + Id + "'");

			SelectByTextOrValue(Find.ByValue(regex));
		}

		/// <summary>
		/// Returns all the items in the select list as an array.
		/// An empty array is returned if the select box has no contents.
		/// </summary>
		public StringCollection AllContents
		{
			get
			{
				StringCollection items = new StringCollection();

				foreach (Option option in Options)
				{
					items.Add(option.Text);
				}

				return items;
			}
		}

		/// <summary>
		/// Returns the <see cref="Options" /> which matches the specified <paramref name="text"/>.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns><see cref="Options" /></returns>
		public Option Option(string text)
		{
			return Option(GetTextAttribute(text));
		}

		/// <summary>
		/// Returns the <see cref="Options" /> which matches the specified <paramref name="text"/>.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns><see cref="Options" /></returns>
		public Option Option(Regex text)
		{
			return Option(Find.ByText(text));
		}

		/// <summary>
		/// Returns the <see cref="Options" /> which matches the specified <paramref name="findBy"/>.
		/// </summary>
		/// <param name="findBy">The find by to use.</param>
		/// <returns></returns>
		public Option Option(BaseConstraint findBy)
		{
			return ElementsSupport.Option(DomContainer, findBy, new ElementCollection(this));
		}

#if !NET11
        /// <summary>
		/// Returns the <see cref="Options" /> which matches the specified expression.
		/// </summary>
		/// <param name="predicate">The expression to use.</param>
		/// <returns></returns>
		public Option Option(Predicate<Option> predicate)
		{
			return Option(Find.ByElement((predicate)));
		}
#endif

		/// <summary>
		/// Returns all the <see cref="Core.Option"/> elements in the <see cref="SelectList"/>.
		/// </summary>
		public OptionCollection Options
		{
			get { return ElementsSupport.Options(DomContainer, new ElementCollection(this)); }
		}

		/// <summary>
		/// Returns the selected option(s) in an array list.
		/// </summary>
		public ArrayList SelectedOptions
		{
			get
			{
				ArrayList items = new ArrayList();

				OptionCollection options = Options.Filter(GetIsSelectedAttribute());
				foreach (Option option in options)
				{
					if (option.Selected)
					{
						items.Add(option);
					}
				}

				return items;
			}
		}

		/// <summary>
		/// Returns the selected item(s) in a <see cref="StringCollection"/>.
		/// </summary>
		public StringCollection SelectedItems
		{
			get
			{
				StringCollection items = new StringCollection();

				OptionCollection options = Options.Filter(GetIsSelectedAttribute());
				foreach (Option option in options)
				{
					items.Add(option.Text);
				}

				return items;
			}
		}

		/// <summary>
		/// Returns the first selected item in the selectlist. Other items may be selected.
		/// Use SelectedItems to get a StringCollection of all selected items.
		/// When there's no item selected, the return value will be null.
		/// </summary>
		public string SelectedItem
		{
			get
			{
				Option option = SelectedOption;
				if (option == null) return null;

				return option.Text;
			}
		}

		/// <summary>
		/// Returns the first selected option in the selectlist. Other options may be selected.
		/// Use SelectedOptions to get an ArrayList of all selected options.
		/// When there's no option selected, the return value will be null.
		/// </summary>
		public Option SelectedOption
		{
			get
			{
				Option option = Option(GetIsSelectedAttribute());

				if (option.Exists)
				{
					return option;
				}

				return null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has selected items.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has selected items; otherwise, <c>false</c>.
		/// </value>
		public bool HasSelectedItems
		{
			get { return SelectedOption != null; }
		}

		private static AttributeConstraint GetIsSelectedAttribute()
		{
			return new AttributeConstraint("selected", true.ToString());
		}

		private void SelectByTextOrValue(BaseConstraint findBy)
		{
			OptionCollection options = Options.Filter(findBy);

			foreach (Option option in options)
			{
				option.Select();
			}

			if (options.Length == 0)
			{
				throw new SelectListItemNotFoundException(findBy.ConstraintToString());
			}
		}

		private static AttributeConstraint GetTextAttribute(string text)
		{
			return Find.ByText(new StringEqualsAndCaseInsensitiveComparer(text));
		}


		public class ElementCollection : IElementCollection
		{
			private SelectList selectlist;

			public ElementCollection(SelectList selectList)
			{
				selectlist = selectList;
			}

			public IHTMLElementCollection Elements
			{
				get { return (IHTMLElementCollection) selectlist.htmlElement.all; }
			}
		}

		internal new static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new SelectList(domContainer, element);
		}
	}
}
