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

namespace WatiN.Core
{
  /// <summary>
  /// This is the base class for finding elements by a specified attribute. Use
  /// this class or one of it's subclasses to implement your own comparison logic.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new AttributeValue("id", "testlinkid")).Url</code>
  /// or use 
  /// <code>ie.Link(Find.ByCustom("id", "testlinkid")).Url</code>
  /// </example>

  public class AttributeValue
  {
    private string attributeName;
    private string valueToLookFor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeValue"/> class.
    /// </summary>
    /// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
    /// <param name="value">The value to look for.</param>
    public AttributeValue(string attributeName, string value)
    {
      ArgumentNotNullOrEmpty(attributeName, "attributeName");
      ArgumentNotNullOrEmpty(value, "value");

      this.attributeName = attributeName;
      valueToLookFor = value;
    }

    /// <summary>
    /// Gets the name of the attribute.
    /// </summary>
    /// <value>The name of the attribute.</value>
    public virtual string AttributeName
    {
      get { return attributeName; }
    }

    /// <summary>
    /// Gets the value to look for.
    /// </summary>
    /// <value>The value.</value>
    public virtual string Value
    {
      get { return valueToLookFor; }
    }

    /// <summary>
    /// This methode implements an exact match comparison. If you want
    /// different behaviour, inherit this class or one of its subclasses and 
    /// override Compare with a specific implementation.
    /// </summary>
    /// <param name="value">Value to compare with (not null or string.empty)</param>
    /// <returns><c>true</c> if the searched for value equals the actual title</returns>
    public virtual bool Compare(string value)
    {
      if (!IsNullOrEmpty(value) && Value.Equals(value))
      {
        return true;
      }

      return false;
    }

    private static void ArgumentNotNullOrEmpty(string requiredObject, string name)
    {
      if (IsNullOrEmpty(requiredObject))
      {
        throw new ArgumentNullException(name, "Null or empty are not allowed");
      }
    }

    /// <summary>
    /// Determines whether the specified <paramref name="value" /> is null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// 	<c>true</c> if the specified value is null or empty; otherwise, <c>false</c>.
    /// </returns>
    protected static bool IsNullOrEmpty(string value)
    {
      return (value == null || value.Length == 0);
    }
  }

  /// <summary>
  /// Class to find an element by it's id.
  /// </summary>  
  /// <example>
  /// <code>ie.Link(new IdValue("testlinkid")).Url</code>
  /// or use
  /// <code>ie.Link(Find.ByLink("testlinkid")).Url</code>
  /// </example>
  public class IdValue : AttributeValue
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="IdValue"/> class.
    /// </summary>
    /// <param name="id">The id to find.</param>
    public IdValue(string id) : base("id", id)
    {}
  }

  /// <summary>
  /// Class to find an element by it's name.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new NameValue("testlinkname")).Url</code>
  /// or use
  /// <code>ie.Link(Find.ByName("testlinkname")).Url</code>
  /// </example>
  public class NameValue : AttributeValue
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NameValue"/> class.
    /// </summary>
    /// <param name="name">The name to find.</param>
    public NameValue(string name) : base("name", name)
    {}
  }

  /// <summary>
  /// Class to find an element by it's text.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new TextValue("my link")).Url</code>
  /// or use
  /// <code>ie.Link(Find.ByText("my link")).Url</code>
  /// </example>
  public class TextValue : AttributeValue
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TextValue"/> class.
    /// </summary>
    /// <param name="text">The text to find.</param>
    public TextValue(string text) : base("text", text)
    {}
  }

  /// <summary>
  /// Class to find a label element placed for an element.
  /// </summary>
  /// <example>
  /// <code>ie.Label(new ForValue("optionbuttonid")).Text</code>
  /// or use
  /// <code>ie.Label(Find.ByFor("optionbuttonid")).Text</code>
  /// </example>
  public class ForValue : AttributeValue
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ForValue"/> class.
    /// </summary>
    /// <param name="forId">For id to find.</param>
    public ForValue(string forId) : base("htmlfor", forId)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="ForValue"/> class.
    /// </summary>
    /// <param name="element">The element to which the Label element is attached.</param>
    public ForValue(Element element) : base("htmlfor", element.Id)
    {}
  }

  /// <summary>
  /// Class to find a Link, Frame, Internet Explorer window or HTML Dialog by a Url.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new UrlValue("http://watin.sourceforge.net")).Click</code>
  /// or use
  /// <code>ie.Link(Find.ByUrl("http://watin.sourceforge.net")).Url</code>
  /// </example>
  public class UrlValue : AttributeValue
  {
    Uri findUrl = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlValue"/> class.
    /// </summary>
    /// <param name="url">The (well-formed) URL to find.</param>
    public UrlValue(string url) : base("href", url)
    {
      findUrl = new Uri(url);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlValue"/> class.
    /// </summary>
    /// <param name="url">The URL to find.</param>
    public UrlValue(Uri url) : base("href", url.ToString())
    {
      findUrl = url;
    }

    /// <summary>
    /// This methode implements an exact match comparison based on comparing both Urls
    /// with System.Uri.Equals. If you want different behaviour, inherit this class or 
    /// one of its subclasses and override Compare with a specific implementation.
    /// </summary>
    /// <param name="value">A well-formed Url to compare with</param>
    /// <returns>True if the searched for Url is equal with the actual Url</returns>
    public override bool Compare(string value)
    {
      return Compare(new Uri(value));       
    }
    /// <summary>
    /// Compares the specified URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c></returns>
    public bool Compare(Uri url)
    {
      if (url.Equals(findUrl))
      {
        return true;
      }
      return false;
    }
  }

  /// <summary>
  /// Class to find an element, Internet Explorer window or HTML Dialog by it's title.
  /// </summary>
  /// <example>
  /// <code>IE ie = IE.AttachToIE(new TitleValue("WatiN Home Page"))</code>
  /// or use
  /// <code>IE ie = IE.AttachToIE(Find.ByTitle("WatiN Home Page"))</code>
  /// </example>
  public class TitleValue : AttributeValue
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TitleValue"/> class.
    /// </summary>
    /// <param name="title">The (partial) title to find.</param>
    public TitleValue(string title) : base("title", title)
    {}

    /// <summary>
    /// This override implements a 'contains' instead of een exact match. If you want
    /// an exact match, inherit this class and override Compare with a specific
    /// implementation.
    /// </summary>
    /// <param name="value">Value to compare with (not null or string.empty)</param>
    /// <returns>True if the searched for title is equal with or is contained by the actual title</returns>
    public override bool Compare(string value)
    {
      bool containedInValue = value.ToLower().IndexOf(Value.ToLower()) >= 0;

      if (!IsNullOrEmpty(value) && containedInValue)
      {
        return true;
      }

      return false;
    }
  }

  /// <summary>
  /// Class to find an element by it's value.
  /// </summary>
  /// <example>
  /// <code>ie.Button(new ValueValue("My Button"))</code>
  /// or use
  /// <code>ie.Button(Find.ByValue("My Button"))</code>
  /// </example>
  public class ValueValue : AttributeValue
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueValue"/> class.
    /// </summary>
    /// <param name="value">The value to find.</param>
    public ValueValue(string value) : base("value", value)
    {}
  }

  /// <summary>
  /// This class provides factory methods for de most commonly used attributes
  /// to find an element on a web page.
  /// </summary>
  public sealed class Find
  {
    /// <summary>
    /// Prevent creating an instance of this class (contains only static members)
    /// </summary>
    private Find(){}
    
    /// <summary>
    /// Find a Label element by the id of the element it's linked with.
    /// </summary>
    /// <param name="forId">Id of the element the label is linked with.</param>
    /// <returns><see cref="ForValue" /></returns>
    /// <example>
    /// <code>ie.Label(Find.ByFor("optionbuttonid")).Text</code>
    /// </example>
    public static ForValue ByFor(string forId)
    {
      return new ForValue(forId);
    }

    /// <summary>
    /// Find an element by its id.
    /// </summary>
    /// <param name="id">Element id to find.</param>
    /// <returns><see cref="IdValue" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByLink("testlinkid")).Url</code>
    /// </example>
    public static IdValue ById(string id)
    {
      return new IdValue(id);
    }

    /// <summary>
    /// Find an element by its name.
    /// </summary>
    /// <param name="name">Name to find.</param>
    /// <returns><see cref="NameValue" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByName("testlinkname")).Url</code>
    /// </example>
    public static NameValue ByName(string name)
    {
      return new NameValue(name);
    }

    /// <summary>
    /// Find an element by its (inner) text
    /// </summary>
    /// <param name="text">Element text</param>
    /// <returns><see cref="TextValue" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByText("my link")).Url</code>
    /// </example>
    public static TextValue ByText(string text)
    {
      return new TextValue(text);
    }

    /// <summary>
    /// Find an element, frame, IE instance or HTMLDialog by its Url.
    /// </summary>
    /// <param name="url">The well-formed url to find.</param>
    /// <returns><see cref="UrlValue" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByUrl("http://watin.sourceforge.net")).Url</code>
    /// </example>
    public static UrlValue ByUrl(string url)
    {
      return new UrlValue(url);
    }

    /// <summary>
    /// Find an element, frame, IE instance or HTMLDialog by its Title.
    /// </summary>
    /// <param name="title">The title to match partially.</param>
    /// <returns><see cref="TitleValue"/></returns>
    /// <example>
    /// <code>IE ie = IE.AttachToIE(Find.ByTitle("WatiN Home Page"))</code>
    /// </example>
    public static TitleValue ByTitle(string title)
    {
      return new TitleValue(title);
    }

    /// <summary>
    /// Find an element by its value attribute.
    /// </summary>
    /// <param name="value">The value to find.</param>
    /// <returns><see cref="ValueValue"/></returns>
    /// <example>
    /// <code>ie.Button(Find.ByValue("My Button"))</code>
    /// </example>
    public static ValueValue ByValue(string value)
    {
      return new ValueValue(value);
    }

    /// <summary>
    /// Find an element by an attribute
    /// </summary>
    /// <param name="attributeName">The attribute to compare the value with</param>
    /// <param name="value">The exact matching value of the attribute</param>
    /// <returns><see cref="AttributeValue" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByCustom("id", "testlinkid")).Url</code>
    /// </example>
    public static AttributeValue ByCustom(string attributeName, string value)
    {
      return new AttributeValue(attributeName, value);
    }
  }
}
