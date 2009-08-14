#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML select element.
	/// </summary>
    [ElementTag("select")]
    public class SelectList : Element<SelectList>
	{
		/// <summary>
		/// Returns an initialized instance of a SelectList object.
		/// Mainly used by the collectionclass SelectLists.
		/// </summary>
		/// <param name="domContainer">The <see cref="DomContainer"/> the element is in.</param>
        /// <param name="element">The HTML select element.</param>
        public SelectList(DomContainer domContainer, INativeElement element) : base(domContainer, element) { }

		/// <summary>
		/// Returns an instance of a SelectList object.
		/// Mainly used internally.
		/// </summary>
		/// <param name="domContainer">The <see cref="DomContainer"/> the element is in.</param>
		/// <param name="finder">The element finder to use.</param>
        public SelectList(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

		/// <summary>
		/// This method clears the selected items in the select box and waits for the 
		/// onchange event to complete after the list is cleared
		/// </summary>
        public virtual void ClearList()
		{
            Logger.LogAction("Clearing selection(s) in {0} '{1}', {2}", GetType().Name, IdOrName, Description);

			var options = Options.Filter(IsSelectedConstraint());

			foreach (Option option in options)
			{
				option.ClearNoWait();
			}

			if (options.Count > 0)
			{
				WaitForComplete();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="SelectList"/> allows multiple select.
		/// </summary>
		/// <value><c>true</c> if multiple; otherwise, <c>false</c>.</value>
        public virtual bool Multiple
		{
			get { return bool.Parse(GetAttributeValue("multiple")); }
		}

		/// <summary>
		/// This method selects an item by text.
		/// Raises NoValueFoundException if the specified value is not found.
		/// </summary>
		/// <param name="text">The text.</param>
        public virtual void Select(string text)
		{
            Logger.LogAction("Selecting '{0}' in {1} '{2}', {3}", text, GetType().Name, IdOrName, Description);
			SelectByTextOrValue(TextCaseInsensitiveConstraint(text));
		}

		/// <summary>
		/// This method selects an item by text using the supplied regular expression.
		/// Raises NoValueFoundException if the specified value is not found.
		/// </summary>
		/// <param name="regex">The regex.</param>
        public virtual void Select(Regex regex)
		{
            Logger.LogAction("Selecting text using regular expresson '{0}' in {1} '{2}', {3}", regex, GetType().Name, IdOrName, Description);

			SelectByTextOrValue(Find.ByText(regex));
		}

		/// <summary>
		/// Selects an item in a select box, by value.
		/// Raises NoValueFoundException if the specified value is not found.
		/// </summary>
		/// <param name="value">The value.</param>
        public virtual void SelectByValue(string value)
		{
            Logger.LogAction("Selecting item with value '{0}' in {1} '{2}', {3}", value, GetType().Name, IdOrName, Description);
			SelectByTextOrValue(Find.ByValue(new StringEqualsAndCaseInsensitiveComparer(value)));
		}

		/// <summary>
		/// Selects an item in a select box by value using the supplied regular expression.
		/// Raises NoValueFoundException if the specified value is not found.
		/// </summary>
		/// <param name="regex">The regex.</param>
        public virtual void SelectByValue(Regex regex)
		{
            Logger.LogAction("Selecting text using regular expresson '{0}' in {1} '{2}', {3}", regex, GetType().Name, IdOrName, Description);
			SelectByTextOrValue(Find.ByValue(regex));
		}

		/// <summary>
		/// Returns all the items in the select list as an array.
		/// An empty array is returned if the select box has no contents.
		/// </summary>
        public virtual StringCollection AllContents
		{
			get
			{
				var items = new StringCollection();

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
        public virtual Option Option(string text)
		{
			return Option(TextCaseInsensitiveConstraint(text));
		}

		/// <summary>
		/// Returns the <see cref="Options" /> which matches the specified <paramref name="text"/>.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns><see cref="Options" /></returns>
        public virtual Option Option(Regex text)
		{
			return Option(Find.ByText(text));
		}

		/// <summary>
		/// Returns the <see cref="Options" /> which matches the specified <paramref name="findBy"/>.
		/// </summary>
		/// <param name="findBy">The find by to use.</param>
		/// <returns></returns>
        public virtual Option Option(Constraint findBy)
		{
            return new Option(DomContainer, CreateElementFinder<Option>(nativeElement => nativeElement.Options, findBy));
		}

        /// <summary>
		/// Returns the <see cref="Options" /> which matches the specified expression.
		/// </summary>
		/// <param name="predicate">The expression to use.</param>
		/// <returns></returns>
        public virtual Option Option(Predicate<Option> predicate)
		{
			return Option(Find.ByElement((predicate)));
		}

		/// <summary>
		/// Returns all the <see cref="Core.Option"/> elements in the <see cref="SelectList"/>.
		/// </summary>
        public virtual OptionCollection Options
		{
            get { return new OptionCollection(DomContainer, CreateElementFinder<Option>(nativeElement => nativeElement.Options, null)); }
		}

		/// <summary>
		/// Returns the selected option(s) in an array list.
		/// </summary>
        public virtual ArrayList SelectedOptions
		{
			get
			{
				var items = new ArrayList();

				var options = Options.Filter(IsSelectedConstraint());
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
        public virtual StringCollection SelectedItems
		{
			get
			{
				var items = new StringCollection();

				var options = Options.Filter(IsSelectedConstraint());
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
        public virtual string SelectedItem
		{
			get
			{
				var option = SelectedOption;
				return option == null ? null : option.Text;
			}
		}

		/// <summary>
		/// Returns the first selected option in the selectlist. Other options may be selected.
		/// Use SelectedOptions to get an ArrayList of all selected options.
		/// When there's no option selected, the return value will be null.
		/// </summary>
        public virtual Option SelectedOption
		{
			get
			{
				var option = Option(IsSelectedConstraint());

				return option.Exists ? option : null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has selected items.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has selected items; otherwise, <c>false</c>.
		/// </value>
        public virtual bool HasSelectedItems
		{
			get { return SelectedOption != null; }
		}

		private void SelectByTextOrValue(Constraint findBy)
		{
            if (Multiple)
            {
                SelectMultiple(findBy);
            }
            else
            {
                SelectSingle(findBy);
            }
		}

	    private void SelectSingle(Constraint findBy)
	    {
	        try
	        {
	            Option(findBy).Select();
	        }
	        catch (ElementNotFoundException)
	        {
	            throw new SelectListItemNotFoundException(findBy.ToString(), this);
	        }
	    }

	    private void SelectMultiple(Constraint findBy)
	    {
	        var options = Options.Filter(findBy);

	        if (options.Count == 0)
	        {
	            throw new SelectListItemNotFoundException(findBy.ToString(), this);
	        }

	        // First select all options
	        foreach (var option in options)
	        {
	            if (option.Selected) continue;
                option.SetAttributeValue("selected", "true");
	        }
                
	        // Then fire the onchange event
	        FireEvent("onchange");
	    }

	    private static AttributeConstraint IsSelectedConstraint()
        {
            return new AttributeConstraint("selected", new StringEqualsAndCaseInsensitiveComparer(true.ToString()));
        }
        
        private static AttributeConstraint TextCaseInsensitiveConstraint(string text)
		{
			return Find.ByText(new StringEqualsAndCaseInsensitiveComparer(text));
		}
	}
}
