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
using System.Text.RegularExpressions;
using WatiN.Core.Interfaces;
using WatiN.Core;

namespace WatiN.Core.Interfaces
{
  /// <summary>
  /// This interface is used by <see cref="Attribute"/> to compare a searched attribute
  /// with a given attribute.
  /// </summary>
  public interface ICompare
  {
    bool Compare(string value);
  }
  
  public interface IAttributeBag
  {
    string GetValue(string attributename);
  }
}

namespace WatiN.Core
{
  /// <summary>
  /// Class that supports an exact comparison of two string values.
  /// </summary>
  public class StringComparer : ICompare
  {
    protected string valueToCompareWith;
    
    public StringComparer(string value)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");        
      }
      valueToCompareWith = value;
    }
    
    public virtual bool Compare(string value)
    {
      if (value != null && valueToCompareWith.Equals(value))
      {
        return true;
      }      
      return false;
    }
 
    public override string ToString()
    {
      return GetType().ToString() + " compares with: " + valueToCompareWith;
    }
  }
  
  /// <summary>
  /// Class that supports a simple matching of two strings.
  /// </summary>
  public class StringContainsAndCaseInsensitiveComparer : StringComparer
  {
    public StringContainsAndCaseInsensitiveComparer(string value) : base(value)
    {
      valueToCompareWith = value.ToLower();
    }
    
    public override bool Compare(string value)
    {
      if (value == null)
      {
        return false;
      }
      
      if (valueToCompareWith == String.Empty & value != String.Empty)
      {
        return false;
      }
      
      return (value.ToLower().IndexOf(valueToCompareWith) >= 0);
    }
  }
  
  /// <summary>
  /// Class that supports a simple matching of two strings.
  /// </summary>
  public class StringEqualsAndCaseInsensitiveComparer : StringComparer
  {    
    public StringEqualsAndCaseInsensitiveComparer(string value) : base(value)
    {}
    
    public override bool Compare(string value)
    {
      if (value == null) return false;
      
      return (String.Compare(value, valueToCompareWith, true) == 0);
    }

    public override string ToString()
    {
      return valueToCompareWith;
    }
  }
  
  /// <summary>
  /// Class that supports matching a regular expression with a string value.
  /// </summary>
  public class RegexComparer :ICompare
  {
    private Regex regexToUse;
    
    public RegexComparer(Regex regex)
    {
      if (regex == null)
      {
        throw new ArgumentNullException("regex");        
      }
      regexToUse = regex;
    }
    
    public bool Compare(string value)
    {
      if (value == null) return false;
      
      return regexToUse.IsMatch(value);
    }
    
    public override string ToString()
    {
      return GetType().ToString() + " matching against: " + regexToUse.ToString();
    }
  }
  
  /// <summary>
  /// Class that supports compares a <see cref="Uri"/> instance with a string value.
  /// </summary>
  public class UriComparer :ICompare
  {
    private Uri uriToCompareWith;
    
    public UriComparer(Uri uri)
    {
      if (uri == null)
      {
        throw new ArgumentNullException("uri");        
      }
      uriToCompareWith = uri;
    }
    
    public bool Compare(string value)
    {
      if (UtilityClass.IsNullOrEmpty(value)) return false;
      
      return Compare(new Uri(value));
    }
    
    /// <summary>
    /// Compares the specified Uri.
    /// </summary>
    /// <param name="url">The Uri.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c></returns>
    public bool Compare(Uri url)
    {
      return uriToCompareWith.Equals(url);
    }
    
    public override string ToString()
    {
      return GetType().ToString() + " compares with: " + uriToCompareWith.ToString();
    }
  }

  public class BoolComparer : ICompare
  {
    private StringComparer comparer;

    public BoolComparer(bool value)
    {
      comparer = new StringComparer(value.ToString());
    }

    public bool Compare(string value)
    {
      return comparer.Compare(value);
    }
  }

  
  /// <summary>
  /// This is the base class for finding elements by a specified attribute. Use
  /// this class or one of it's subclasses to implement your own comparison logic.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new Attribute("id", "testlinkid")).Url</code>
  /// or use 
  /// <code>ie.Link(Find.ByCustom("id", "testlinkid")).Url</code>
  /// </example>
  public class Attribute
  {
    private string attributeName;
    private string valueToLookFor;
    protected ICompare comparer;
    protected Attribute andAttribute;
    protected Attribute orAttribute;
    protected Attribute lastAddedAttribute;
    protected Attribute lastAddedOrAttribute;

    // This makes the Find.ByName() & Find.ByCustom() syntax possible
    // and is needed for the && operator
    public static Attribute operator & (Attribute first, Attribute second) 
    {
      return first.And(second);
    }
    
    // This makes the Find.ByName() | Find.ByCustom() syntax possible
    // and is needed for the || operator
    public static Attribute operator | (Attribute first, Attribute second) 
    {
      return first.Or(second);
    }

    // This makes the Find.ByName() && Find.ByCustom() syntax possible
    public static bool operator true (Attribute attribute) 
    {
      return false;
    }

    // This makes the Find.ByName() || Find.ByCustom() syntax possible
    public static bool operator false (Attribute attribute) 
    {
      return false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Attribute"/> class.
    /// </summary>
    /// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
    /// <param name="value">The value to look for.</param>
    public Attribute(string attributeName, string value)
    {
      CheckArgumentNotNull("value", value);
      Init(attributeName, value, new StringComparer(value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Attribute"/> class.
    /// </summary>
    /// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
    /// <param name="regex">The regular expression to use.</param>
    public Attribute(string attributeName, Regex regex)
    {
      CheckArgumentNotNull("regex", regex);
      Init(attributeName, regex.ToString(), new RegexComparer(regex));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Attribute"/> class.
    /// </summary>
    /// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
    /// <param name="comparer">The comparer.</param>
    public Attribute(string attributeName, ICompare comparer)
    {
      CheckArgumentNotNull("comparer", comparer);
      Init(attributeName, comparer.ToString(), comparer);
    }

    private void Init(string attributeName, string value, ICompare comparerInstance)
    {
      CheckArgumentNotNullOrEmpty("attributeName", attributeName);

      this.attributeName = attributeName;
      valueToLookFor = value;
      comparer = comparerInstance;
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
    /// Gets the value to look for or the regex pattern that will be used.
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
    /// <param name="attributeBag">Value to compare with</param>
    /// <returns><c>true</c> if the searched for value equals the given value</returns>
    public virtual bool Compare(IAttributeBag attributeBag)
    {     
      bool returnValue = comparer.Compare(attributeBag.GetValue(attributeName));
      
      if (returnValue && andAttribute != null)
      {
        returnValue = andAttribute.Compare(attributeBag);
      }
      
      if (returnValue == false && orAttribute != null)
      {
        returnValue = orAttribute.Compare(attributeBag);
      }

      return returnValue;
    }
    
    public Attribute And(Attribute attribute)
    {
      if (andAttribute == null)
      {
        andAttribute = attribute;
      }
      else
      {
        lastAddedAttribute.And(attribute);
      }

      lastAddedAttribute = attribute;

      return this;
    }
    
    public Attribute Or(Attribute attribute)
    {
      if (orAttribute == null)
      {
        orAttribute = attribute;
      }
      else
      {
        lastAddedOrAttribute.Or(attribute);
      }

      lastAddedOrAttribute = attribute;
      lastAddedAttribute = attribute;

      return this;
    }

    public override string ToString()
    {
      return Value;
    }
          
    private static void CheckArgumentNotNullOrEmpty(string argumentName, string argumentValue)
    {
      if (UtilityClass.IsNullOrEmpty(argumentValue))
      {
        throw new ArgumentNullException(argumentName, "Null and Empty are not allowed.");
      }
    }
    private static void CheckArgumentNotNull(string argumentName, object argumentValue)
    {
      if (argumentValue == null)
      {
        throw new ArgumentNullException(argumentName, "Null is not allowed.");
      }
    }
  }

  /// <summary>
  /// This class is only used in the ElementsSupport Class to 
  /// create a collection of all elements.
  /// </summary>
  internal class AlwaysTrueAttribute : Attribute
  {
    public AlwaysTrueAttribute() : base("noAttribute", "")
    {}
    
    public override bool Compare(IAttributeBag attributeBag)
    {
      return true;
    }
  }
  
  /// <summary>
  /// Class to find an element by it's id.
  /// </summary>  
  /// <example>
  /// <code>ie.Link(new Id("testlinkid")).Url</code>
  /// or use
  /// <code>ie.Link(Find.ByLink("testlinkid")).Url</code>
  /// </example>
  public class Id : Attribute
  {
    private const string attributeName = "id";

    /// <summary>
    /// Initializes a new instance of the <see cref="Id"/> class.
    /// </summary>
    /// <param name="id">The id to find.</param>
    public Id(string id) : base(attributeName, id)
    {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Id"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Id(Regex regex) : base(attributeName, regex)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="Id"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Id(ICompare comparer) : base(attributeName, comparer)
    {}
  }
 
  /// <summary>
  /// Class to find an element by the n-th occurrence.
  /// Occurrence counting is zero based. If you combine <see cref="Occurrence"/> with
  /// other <see cref="Attribute"/> classes, always start with <see cref="Occurrence"/>
  /// as shown in the following example.  
  /// </summary>  
  /// <example>
  /// This example will get the second occurrence on the page of a
  /// link element with "linkname" as it's name value. 
  /// <code>ie.Link(new Occurrence(1).And(Find.ByName("linkname"))</code>
  /// You could also consider filtering the Links collection and getting
  /// the second item in the collection, like this:
  /// <code>ie.Links.Filter(Find.ByName("linkname"))[1]</code>
  /// </example>
  public class Occurrence : Attribute
  {
    private int occurrence;
    private int counter = -1;
    
    public Occurrence(int occurrence) : base("occurence", occurrence.ToString())
    {
      if (occurrence < 0 )
      {
        throw new ArgumentOutOfRangeException("occurrence", occurrence, "Should be zero or more.");
      }

      this.occurrence = occurrence;
    }
    
    public override bool Compare(IAttributeBag attributeBag)
    {
      bool resultAnd = false;
      bool resultOr = false;
      
      if (andAttribute != null)
      {
        resultAnd = andAttribute.Compare(attributeBag);
      }

      if (resultAnd || andAttribute == null)
      {
        counter++;
      }
      
      if (orAttribute != null && resultAnd == false)
      {
        resultOr = orAttribute.Compare(attributeBag);
      }
      
      return (counter == occurrence) || resultOr;
    }
  }
  
  /// <summary>
  /// Class to find an element by it's name.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new Name("testlinkname")).Url</code>
  /// or use
  /// <code>ie.Link(Find.ByName("testlinkname")).Url</code>
  /// </example>
  public class Name : Attribute
  {
    private const string attributeName = "name";

    /// <summary>
    /// Initializes a new instance of the <see cref="Name"/> class.
    /// </summary>
    /// <param name="name">The name to find.</param>
    public Name(string name) : base(attributeName, name)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="Name"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Name(Regex regex) : base(attributeName, regex)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="Name"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Name(ICompare comparer) : base(attributeName, comparer)
    {}
  }

  /// <summary>
  /// Class to find an element by it's text.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new Text("my link")).Url</code>
  /// or use
  /// <code>ie.Link(Find.ByText("my link")).Url</code>
  /// </example>
  public class Text : Attribute
  {
    private const string attributeName = "innertext";

    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param name="text">The text to find.</param>
    public Text(string text) : base(attributeName, text)
    {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Text(Regex regex) : base(attributeName, regex)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Text(ICompare comparer) : base(attributeName, comparer)
    {}
  }
  
  /// <summary>
  /// Class to find an element by it's style property color.
  /// </summary>
  /// The following code examples show how to use this class.
  /// <example>
  /// <code>ie.Span(new StyleAttribute("color", "red"))</code>
  /// or use
  /// <code>ie.Span(Find.ByStyle("color", "red"))</code>
  /// </example>
  public class StyleAttribute : Attribute
  {
    private const string attributeName = "style.";

    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="value">The value it should match.</param>
    public StyleAttribute(string styleAttributeName, string value) : base(attributeName + styleAttributeName, value)
    {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="regex">The regex.</param>
    public StyleAttribute(string styleAttributeName, Regex regex) : base(attributeName + styleAttributeName, regex)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="StyleAttribute"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public StyleAttribute(string styleAttributeName, ICompare comparer) : base(attributeName + styleAttributeName, comparer)
    {}
  }

  /// <summary>
  /// Class to find a label element placed for an element.
  /// </summary>
  /// <example>
  /// <code>ie.Label(new For("optionbuttonid")).Text</code>
  /// or use
  /// <code>ie.Label(Find.ByFor("optionbuttonid")).Text</code>
  /// </example>
  public class For : Attribute
  {
    private const string attributeName = "htmlfor";

    /// <summary>
    /// Initializes a new instance of the <see cref="For"/> class.
    /// </summary>
    /// <param name="forId">For id to find.</param>
    public For(string forId) : base(attributeName, forId)
    {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="For"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public For(Regex regex) : base(attributeName, regex)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="For"/> class.
    /// </summary>
    /// <param name="element">The element to which the Label element is attached. This element must an Id value.</param>
    public For(Element element) : base(attributeName, element.Id)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="For"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public For(ICompare comparer) : base(attributeName, comparer)
    {}
  }

  /// <summary>
  /// Class to find a Link, Frame, Internet Explorer window or HTML Dialog by a Url.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new Url("http://watin.sourceforge.net")).Click</code>
  /// or use
  /// <code>ie.Link(Find.ByUrl("http://watin.sourceforge.net")).Url</code>
  /// </example>
  public class Url : Attribute
  {
    private const string attributeName = "href";

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="url">The (well-formed) URL to find.</param>
    public Url(string url) : this(new Uri(url))
    {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Url(Regex regex) : base(attributeName, regex)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="uri">The URL to find.</param>
    public Url(Uri uri) : base(attributeName, uri.ToString())
    {
      comparer = new UriComparer(uri);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Url(ICompare comparer) : base(attributeName, comparer)
    {}

    /// <summary>
    /// Compares the specified Uri.
    /// </summary>
    /// <param name="uri">The Uri.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c></returns>
    public bool Compare(Uri uri)
    {
      return ((UriComparer)comparer).Compare(uri);
    }
  }

  /// <summary>
  /// Class to find an element, Internet Explorer window or HTML Dialog by it's title.
  /// If multiple Internet Explorer windows have the same or partially 
  /// the same Title, the first match will be returned.
  /// </summary>
  /// <example>
  /// <code>IE ie = IE.AttachToIE(new Title("WatiN Home Page"))</code>
  /// or use
  /// <code>IE ie = IE.AttachToIE(Find.ByTitle("WatiN Home Page"))</code>
  /// </example>
  public class Title : Attribute
  {
    private const string attributeName = "title";

    /// <summary>
    /// Initializes a new instance of the <see cref="Title"/> class.
    /// </summary>
    /// <param name="title">The (partial) title to find.</param>
    public Title(string title) : base(attributeName, title)
    {
      comparer = new StringContainsAndCaseInsensitiveComparer(title);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Title"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Title(Regex regex) : base(attributeName, regex)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="Title"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Title(ICompare comparer) : base(attributeName, comparer)
    {}
  }

  /// <summary>
  /// Class to find an element by it's value.
  /// </summary>
  /// <example>
  /// <code>ie.Button(new Value("My Button"))</code>
  /// or use
  /// <code>ie.Button(Find.ByValue("My Button"))</code>
  /// </example>
  public class Value : Attribute
  {
    private const string attributeName = "value";

    /// <summary>
    /// Initializes a new instance of the <see cref="Value"/> class.
    /// </summary>
    /// <param name="value">The value to find.</param>
    public Value(string value) : base(attributeName, value)
    {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Value"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Value(Regex regex) : base(attributeName, regex)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="Value"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Value(ICompare comparer) : base(attributeName, comparer)
    {}
  }

  /// <summary>
  /// Class to find an image by it's source (src) attribute.
  /// </summary>
  /// <example>
  /// <code>ie.Image(new Src("image.gif")).Url</code>
  /// or use
  /// <code>ie.Image(Find.BySrc("image.gif")).Url</code>
  /// </example>
  public class Src : Attribute
  {
    private const string attributeName = "src";

    /// <summary>
    /// Initializes a new instance of the <see cref="Src"/> class.
    /// </summary>
    /// <param name="name">The exact src to find.</param>
    public Src(string name) : base(attributeName, name)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="Src"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Src(Regex regex) : base(attributeName, regex)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="Src"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Src(ICompare comparer) : base(attributeName, comparer)
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
    /// <returns><see cref="For" /></returns>
    /// <example>
    /// <code>ie.Label(Find.ByFor("optionbuttonid")).Text</code>
    /// </example>
    public static For ByFor(string forId)
    {
      return new For(forId);
    }
    
    /// <param name="regex">Regular expression to find the matching Id of the element
    ///  the label is linked with.</param>
    /// <returns><see cref="For" /></returns>
    /// <example>
    /// <code>ie.Label(Find.ByFor(new Regex("pattern goes here")).Text</code>
    /// </example>
    public static For ByFor(Regex regex)
    {
      return new For(regex);
    }
    
    /// <param name="element">The element to which the Label element is attached. This element must an Id value.</param>
    /// <returns><see cref="For" /></returns>
    /// <example>
    /// <code>
    /// CheckBox checkbox = ie.CheckBox("checkboxid");
    /// ie.Label(Find.ByFor(checkbox).Text</code>
    /// </example>
    public static For ByFor(Element element)
    {
      return new For(element);
    }

    /// <summary>
    /// Find an element by its id.
    /// </summary>
    /// <param name="id">Element id to find.</param>
    /// <returns><see cref="Id" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByLink("testlinkid")).Url</code>
    /// </example>
    public static Id ById(string id)
    {
      return new Id(id);
    }

    /// <param name="regex">Regular expression to find a matching Id.</param>
    /// <returns><see cref="Id" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByLink(new Regex("pattern goes here"))).Url</code>
    /// </example>
    public static Id ById(Regex regex)
    {
      return new Id(regex);
    }

    /// <summary>
    /// Find an element by its name.
    /// </summary>
    /// <param name="name">Name to find.</param>
    /// <returns><see cref="Name" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByName("testlinkname")).Url</code>
    /// </example>
    public static Name ByName(string name)
    {
      return new Name(name);
    }

    /// <param regex="regex">Regular expression to find a matching Name.</param>
    /// <returns><see cref="Name" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByName(new Regex("pattern goes here")))).Url</code>
    /// </example>
    public static Name ByName(Regex regex)
    {
      return new Name(regex);
    }
        
    /// <summary>
    /// Find an element by its (inner) text
    /// </summary>
    /// <param name="text">Element text</param>
    /// <returns><see cref="Text" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByText("my link")).Url</code>
    /// </example>
    public static Text ByText(string text)
    {
      return new Text(text);
    }
    
    /// <param name="regex">Regular expression to find a matching Text.</param>
    /// <returns><see cref="Text" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByText(new Regex("pattern goes here"))).Url</code>
    /// </example>
    public static Text ByText(Regex regex)
    {
      return new Text(regex);
    }

    /// <summary>
    /// Find an element, frame, IE instance or HTMLDialog by its Url.
    /// </summary>
    /// <param name="url">The well-formed url to find.</param>
    /// <returns><see cref="Url" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByUrl("http://watin.sourceforge.net")).Url</code>
    /// </example>
    public static Url ByUrl(string url)
    {
      return new Url(url);
    }
    
    /// <param name="uri">The uri to find.</param>
    /// <returns><see cref="Url" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByUrl(new Uri("watin.sourceforge.net"))).Url</code>
    /// </example>
    public static Url ByUrl(Uri uri)
    {
      return new Url(uri);
    }
    
    /// <param name="regex">Regular expression to find a matching Url.</param>
    /// <returns><see cref="Url" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByUrl(new Regex("pattern goes here"))).Url</code>
    /// </example>
    public static Url ByUrl(Regex regex)
    {
      return new Url(regex);
    }

    /// <summary>
    /// Find an element, frame, IE instance or HTMLDialog by its Title.
    /// </summary>
    /// <param name="title">The title to match partially.</param>
    /// <returns><see cref="Title"/></returns>
    /// <example>
    /// <code>IE ie = IE.AttachToIE(Find.ByTitle("WatiN Home Page"))</code>
    /// </example>
    public static Title ByTitle(string title)
    {
      return new Title(title);
    }

    /// <param name="regex">Regular expression to find a matching Title.</param>
    /// <returns><see cref="Title"/></returns>
    /// <example>
    /// <code>IE ie = IE.AttachToIE(Find.ByTitle(new Regex("pattern goes here")))</code>
    /// </example>
    public static Title ByTitle(Regex regex)
    {
      return new Title(regex);
    }

    /// <summary>
    /// Find an element by its value attribute.
    /// </summary>
    /// <param name="value">The value to find.</param>
    /// <returns><see cref="Value"/></returns>
    /// <example>
    /// <code>ie.Button(Find.ByValue("My Button"))</code>
    /// </example>
    public static Value ByValue(string value)
    {
      return new Value(value);
    }

    /// <param name="regex">Regular expression to find a matching Value.</param>
    /// <returns><see cref="Value"/></returns>
    /// <example>
    /// <code>ie.Button(Find.ByValue(new Regex("pattern goes here")))</code>
    /// </example>
    public static Value ByValue(Regex regex)
    {
      return new Value(regex);
    }

    /// <summary>
    /// Find an <see cref="Image"/> by its source (src) attribute.
    /// </summary>
    /// <param name="src">Src to find.</param>
    /// <returns><see cref="Src" /></returns>
    /// <example>
    /// <code>ie.Image(Find.BySrc("image.gif"))</code>
    /// </example>
    public static Src BySrc(string src)
    {
      return new Src(src);
    }

    /// <param regex="regex">Regular expression to find a matching Src.</param>
    /// <returns><see cref="Src" /></returns>
    /// <example>
    /// <code>ie.Image(Find.BySrc(new Regex("pattern goes here"))))</code>
    /// </example>
    public static Src BySrc(Regex regex)
    {
      return new Src(regex);
    }

    /// <summary>
    /// Find an element by an attribute.
    /// </summary>
    /// <param name="attributeName">The attribute to compare the value with.</param>
    /// <param name="value">The exact matching value of the attribute.</param>
    /// <returns><see cref="Attribute" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByCustom("id", "testlinkid")).Url</code>
    /// </example>
    public static Attribute ByCustom(string attributeName, string value)
    {
      return new Attribute(attributeName, value);
    }
    
    /// <param name="attributeName">The attribute to compare the value with.</param>
    /// <param name="regex">Regular expression to find a matching value of the given attribute.</param>
    /// <returns><see cref="Attribute" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByCustom("id", new Regex("pattern goes here"))).Url</code>
    /// </example>
    public static Attribute ByCustom(string attributeName, Regex regex)
    {
      return new Attribute(attributeName, regex);
    }

    /// <summary>
    /// Find an element by a style attribute.
    /// </summary>
    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="value">The exact matching value of the attribute.</param>
    /// <returns><see cref="StyleAttribute"/></returns>
    /// <example>
    /// 	<code>ie.Span(Find.ByStyle("background-color", "red"))</code>
    /// </example>
    public static StyleAttribute ByStyle(string styleAttributeName, string value)
    {
      return new StyleAttribute(styleAttributeName, value);
    }
    
    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="value">Regular expression to find a matching value of the given style attribute.</param>
    /// <returns><see cref="StyleAttribute"/></returns>
    /// <example>
    /// 	<code>ie.Link(Find.ByStyle("font-family", new Regex("pattern goes here")))</code>
    /// </example>
    public static StyleAttribute ByStyle(string styleAttributeName, Regex value)
    {
      return new StyleAttribute(styleAttributeName, value);
    }
  } 
}
