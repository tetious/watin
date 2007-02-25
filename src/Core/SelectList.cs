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

using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using mshtml;

using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.Core
{
  /// <summary>
  /// This class provides specialized functionality for a HTML select element.
  /// </summary>
  public class SelectList : Element
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
    /// <param name="ie">The <see cref="DomContainer"/> the element is in.</param>
    /// <param name="htmlSelectElement">The HTML select element.</param>
    public SelectList(DomContainer ie, IHTMLElement htmlSelectElement) : base(ie, htmlSelectElement)
    {}

    /// <summary>
    /// Returns an instance of a SelectList object.
    /// Mainly used internally.
    /// </summary>
    /// <param name="ie">The <see cref="DomContainer"/> the element is in.</param>
    /// <param name="finder">The element finder to use.</param>
    public SelectList(DomContainer ie, ElementFinder finder) : base(ie, finder)
    {}

    /// <summary>
    /// Initialises a new instance of the <see cref="SelectList"/> class based on <paramref name="element"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    public SelectList(Element element) : base(element, ElementTags)
    {}

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
      get { return selectElement.multiple; }
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

      SelectByTextOrValue(new Value(new StringEqualsAndCaseInsensitiveComparer(value)));
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
    public Option Option(Attribute findBy)
    {
      return ElementsSupport.Option(DomContainer, findBy, (IHTMLElementCollection) htmlElement.all);
    }

    /// <summary>
    /// Returns all the <see cref="Core.Option"/> elements in the <see cref="SelectList"/>.
    /// </summary>
    public OptionCollection Options
    {
      get
      {
        return ElementsSupport.Options(DomContainer, (IHTMLElementCollection)htmlElement.all);
      }
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

    private static Attribute GetIsSelectedAttribute()
    {
      return new Attribute("selected", true.ToString());
    }

    private void SelectByTextOrValue(Attribute findBy)
    {
      OptionCollection options = Options.Filter(findBy);
      
      foreach (Option option in options)
      {
        option.Select();
      }

      if (options.Length == 0)
      {
        throw new SelectListItemNotFoundException(findBy.Value) ;
      }
    }

    private static Text GetTextAttribute(string text)
    {
      return new Text(new StringEqualsAndCaseInsensitiveComparer(text));
    }

    private IHTMLSelectElement selectElement
    {
      get { return ((IHTMLSelectElement) HTMLElement); }
    }
  }
}