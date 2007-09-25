#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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

namespace WatiN.Core
{
  using System;
  using System.Text.RegularExpressions;
  using WatiN.Core.Comparers;
  using WatiN.Core.Exceptions;
  using WatiN.Core.Interfaces;

  /// <summary>
  /// This is the base class for finding elements by a specified attribute. Use
  /// this class or one of it's subclasses to implement your own comparison logic.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new Attribute("id", "testlinkid")).Url</code>
  /// or use 
  /// <code>ie.Link(Find.By("id", "testlinkid")).Url</code>
  /// </example>
  public class AttributeConstraint
  {
    private string attributeName;
    private string valueToLookFor;
    private bool busyComparing = false;

    protected ICompare comparer;
    protected AttributeConstraint andAttributeConstraint;
    protected AttributeConstraint orAttributeConstraint;
    protected AttributeConstraint lastAddedAttributeConstraint;
    protected AttributeConstraint lastAddedOrAttributeConstraint;


    // This makes the Find.ByName() & Find.By() syntax possible
    // and is needed for the && operator
    public static AttributeConstraint operator &(AttributeConstraint first, AttributeConstraint second)
    {
      return first.And(second);
    }

    // This makes the Find.ByName() | Find.By() syntax possible
    // and is needed for the || operator
    public static AttributeConstraint operator |(AttributeConstraint first, AttributeConstraint second)
    {
      return first.Or(second);
    }

    // This makes the Find.ByName() && Find.By() syntax possible
    public static bool operator true(AttributeConstraint attributeConstraint)
    {
      return false;
    }

    // This makes the Find.ByName() || Find.By() syntax possible
    public static bool operator false(AttributeConstraint attributeConstraint)
    {
      return false;
    }

    public static AttributeConstraint operator !(AttributeConstraint attributeConstraint)
    {
      return new Not(attributeConstraint);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeConstraint"/> class.
    /// </summary>
    /// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
    /// <param name="value">The value to look for.</param>
    public AttributeConstraint(string attributeName, string value)
    {
      CheckArgumentNotNull("value", value);
      Init(attributeName, value, new StringComparer(value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeConstraint"/> class.
    /// </summary>
    /// <param name="attributeName">Name of the AttributeConstraint as recognised by Internet Explorer.</param>
    /// <param name="regex">The regular expression to use.</param>
    public AttributeConstraint(string attributeName, Regex regex)
    {
      CheckArgumentNotNull("regex", regex);
      Init(attributeName, regex.ToString(), new RegexComparer(regex));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeConstraint"/> class.
    /// </summary>
    /// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
    /// <param name="comparer">The comparer.</param>
    public AttributeConstraint(string attributeName, ICompare comparer)
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
    /// <seealso cref="LockCompare"/>
    /// <seealso cref="UnLockCompare"/>
    /// </summary>
    /// <param name="attributeBag">Value to compare with</param>
    /// <returns><c>true</c> if the searched for value equals the given value</returns>
    /// <example>
    /// The following example shows the use of the LockCompare and UnlockCompare methods.
    /// <code>
    ///   public override bool Compare(IAttributeBag attributeBag)
    ///    {
    ///      bool result = false;
    /// 
    ///      // Call LockCompare if you don't call base.compare.
    ///      base.LockCompare();
    ///
    ///      try
    ///      {
    ///         // Your compare code goes here.
    /// 
    ///         // Don't call base.compare here because that will throw
    ///         // a ReEntryException since you already locked the compare
    ///         // method for recursive calls.
    ///      }
    ///      finally
    ///      {
    ///      // Call UnLockCompare if you called LockCompare.
    ///        base.UnLockCompare();
    ///      }      
    ///
    ///      return result;
    ///    }
    /// </code>
    /// </example>
    public virtual bool Compare(IAttributeBag attributeBag)
    {
      LockCompare();
      bool returnValue;

      try
      {
        returnValue = doCompare(attributeBag);
      }
      finally
      {
        UnLockCompare();
      }

      return returnValue;
    }

    /// <summary>
    /// Does the compare without calling <see cref="LockCompare"/> and <see cref="UnLockCompare"/>.
    /// </summary>
    /// <param name="attributeBag">The attribute bag.</param>
    protected virtual bool doCompare(IAttributeBag attributeBag)
    {
      return EvaluateAndOrAttributes(attributeBag, comparer.Compare(attributeBag.GetValue(attributeName)));
    }

    /// <summary>
    /// Evaluates the and or attributes.
    /// </summary>
    /// <param name="attributeBag">The attribute bag.</param>
    /// <param name="initialReturnValue">if set to <c>false</c> it will skip the And evaluation.</param>
    /// <returns></returns>
    protected bool EvaluateAndOrAttributes(IAttributeBag attributeBag, bool initialReturnValue)
    {
      bool returnValue = initialReturnValue;

      if (returnValue && andAttributeConstraint != null)
      {
        returnValue = andAttributeConstraint.Compare(attributeBag);
      }

      if (returnValue == false && orAttributeConstraint != null)
      {
        returnValue = orAttributeConstraint.Compare(attributeBag);
      }

      return returnValue;
    }


    /// <summary>
    /// Call this method to unlock the compare method. Typically done at the
    /// end of your compare method. 
    /// <seealso cref="LockCompare"/>
    /// <seealso cref="Compare"/>
    /// </summary>
    protected virtual void UnLockCompare()
    {
      busyComparing = false;
    }

    /// <summary>
    /// Call this method if you override the Compare method and don't call base.compare.
    /// You should typically call this method as the first line of code in your
    /// compare method. Calling this will prevent unnoticed reentry problems, resulting
    /// in a StackOverflowException when your AttributeConstraint class is recursively used in a multiple
    /// findby scenario.
    /// <seealso cref="Compare"/>
    /// <seealso cref="UnLockCompare"/>
    /// </summary>
    /// <example>
    /// The following example shows the use of the LockCompare and UnlockCompare methods.
    /// <code>
    ///   public override bool Compare(IAttributeBag attributeBag)
    ///    {
    ///      bool result = false;
    /// 
    ///      base.LockCompare();
    ///
    ///      try
    ///      {
    ///         // Your compare code goes here.
    ///         // Don't call base.compare here because that will throw
    ///         // a ReEntryException since you already locked the compare
    ///         // method for recursive calls.
    ///      }
    ///      finally
    ///      {
    ///        base.UnLockCompare();
    ///      }      
    ///
    ///      return result;
    ///    }
    /// </code>
    /// </example>
    protected virtual void LockCompare()
    {
      if (busyComparing)
      {
        UnLockCompare();
        throw new ReEntryException(this);
      }

      busyComparing = true;
    }

    /// <summary>
    /// Adds the specified AttributeConstraint to the And AttributeConstraint chain of a multiple <see cref="AttributeConstraint"/>
    /// element search. When calling And or using the operators, WatiN will always use
    /// ConditionAnd (&amp;&amp;) during the evaluation.
    /// <seealso cref="Or"/>
    /// </summary>
    /// <param name="attributeConstraint">The <see cref="AttributeConstraint"/> instance.</param>
    /// <returns>This <see cref="AttributeConstraint"/></returns>
    /// <example>
    /// If you want to find a Button by it's name and value this example shows you how to use
    /// the And method to do this:
    /// <code> 
    /// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
    /// 
    /// Button myButton = ie.Button(Find.ByName("buttonname").And(Find.ByValue("Button value")));
    /// </code>
    /// 
    /// You can also use the &amp; or &amp;&amp; operators, resulting in a bit more readable code.
    /// <code> 
    /// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
    /// 
    /// Button myButton = ie.Button(Find.ByName("buttonname") &amp; Find.ByValue("Button value"));
    /// </code>
    /// </example>
    public AttributeConstraint And(AttributeConstraint attributeConstraint)
    {
      if (andAttributeConstraint == null)
      {
        andAttributeConstraint = attributeConstraint;
      }
      else
      {
        lastAddedAttributeConstraint.And(attributeConstraint);
      }

      lastAddedAttributeConstraint = attributeConstraint;

      return this;
    }

    /// <summary>
    /// Adds the specified AttributeConstraint to the Or AttributeConstraint chain of a multiple <see cref="AttributeConstraint"/>
    /// element search. When calling Or or using the | or || operators, WatiN will always use
    /// ConditionOr (||) during the evaluation.
    /// <seealso cref="And"/>
    /// </summary>
    /// <param name="attributeConstraint">The <see cref="AttributeConstraint"/> instance.</param>
    /// <returns>This <see cref="AttributeConstraint"/></returns>
    /// <example>
    /// If you want to find a Button by it's English or Dutch value this example shows you how to use
    /// the Or method to do this:
    /// <code> 
    /// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
    /// 
    /// Button myButton = ie.Button(Find.ByValue("Cancel").Or(Find.ByValue("Annuleren")));
    /// </code>
    /// 
    /// You can also use the | or || operators, resulting in a bit more readable code.
    /// <code> 
    /// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
    /// 
    /// Button myButton = ie.Button(Find.ByValue("Cancel") || Find.ByValue("Annuleren"));
    /// </code>
    /// </example>
    public AttributeConstraint Or(AttributeConstraint attributeConstraint)
    {
      if (orAttributeConstraint == null)
      {
        orAttributeConstraint = attributeConstraint;
      }
      else
      {
        lastAddedOrAttributeConstraint.Or(attributeConstraint);
      }

      lastAddedOrAttributeConstraint = attributeConstraint;
      lastAddedAttributeConstraint = attributeConstraint;

      return this;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// The Value of this <see cref="AttributeConstraint" />
    /// </returns>
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

  [Obsolete("Use AttributeConstraint instead")]
  public class Attribute : AttributeConstraint
  {
    public Attribute(string attributeName, ICompare comparer) : base(attributeName, comparer) {}

    public Attribute(string attributeName, Regex regex) : base(attributeName, regex) {}

    public Attribute(string attributeName, string value) : base(attributeName, value) {}
  }

  public class Not : AttributeConstraint
  {
    private AttributeConstraint _attributeConstraint;

    public Not(AttributeConstraint attributeConstraint) : base("not", string.Empty)
    {
      if (attributeConstraint == null)
      {
        throw new ArgumentNullException("_attributeConstraint");
      }

      _attributeConstraint = attributeConstraint;
    }

    public override bool Compare(IAttributeBag attributeBag)
    {
      bool result;
      LockCompare();

      try
      {
        result = !(_attributeConstraint.Compare(attributeBag));
      }
      finally
      {
        UnLockCompare();
      }

      return result;
    }
  }

  /// <summary>
  /// This class is only used in the ElementsSupport Class to 
  /// create a collection of all elements.
  /// </summary>
  public class AlwaysTrueAttribute : AttributeConstraint
  {
    public AlwaysTrueAttribute() : base("noAttribute", "")
    {
    }

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
  [Obsolete("Use Find.ById() or the class AttributeConstraint instead")]
  public class Id : AttributeConstraint
  {
    private const string attributeName = "id";

    /// <summary>
    /// Initializes a new instance of the <see cref="Id"/> class.
    /// </summary>
    /// <param name="id">The id to find.</param>
    public Id(string id) : base(attributeName, id)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Id"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Id(Regex regex) : base(attributeName, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Id"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Id(ICompare comparer) : base(attributeName, comparer)
    {
    }
  }

  /// <summary>
  /// Class to find an element by the n-th index.
  /// Index counting is zero based.
  /// </summary>  
  /// <example>
  /// This example will get the second link of the collection of links
  /// which have "linkname" as their name value. 
  /// <code>ie.Link(new Index(1) &amp;&amp; Find.ByName("linkname"))</code>
  /// You could also consider filtering the Links collection and getting
  /// the second item in the collection, like this:
  /// <code>ie.Links.Filter(Find.ByName("linkname"))[1]</code>
  /// </example>
  public class Index : AttributeConstraint
  {
    private int index;
    private int counter = -1;

    public Index(int index) : base("index", index.ToString())
    {
      if (index < 0)
      {
        throw new ArgumentOutOfRangeException("index", index, "Should be zero or more.");
      }

      this.index = index;
    }

    public override bool Compare(IAttributeBag attributeBag)
    {
      base.LockCompare();

      bool resultOr;

      try
      {
        bool resultAnd = false;
        resultOr = false;

        if (andAttributeConstraint != null)
        {
          resultAnd = andAttributeConstraint.Compare(attributeBag);
        }

        if (resultAnd || andAttributeConstraint == null)
        {
          counter++;
        }

        if (orAttributeConstraint != null && resultAnd == false)
        {
          resultOr = orAttributeConstraint.Compare(attributeBag);
        }
      }
      finally
      {
        base.UnLockCompare();
      }

      return (counter == index) || resultOr;
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
  [Obsolete("Use Find.ByName() or the class AttributeConstraint instead")]
  public class Name : AttributeConstraint
  {
    private const string attributeName = "name";

    /// <summary>
    /// Initializes a new instance of the <see cref="Name"/> class.
    /// </summary>
    /// <param name="name">The name to find.</param>
    public Name(string name) : base(attributeName, name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Name"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Name(Regex regex) : base(attributeName, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Name"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Name(ICompare comparer) : base(attributeName, comparer)
    {
    }
  }

  /// <summary>
  /// Class to find an element by it's text.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new Text("my link")).Url</code>
  /// or use
  /// <code>ie.Link(Find.ByText("my link")).Url</code>
  /// </example>
  [Obsolete("Use Find.ByText() or the class AttributeConstraint instead")]
  public class Text : AttributeConstraint
  {
    private const string attributeName = "innertext";

    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param name="text">The text to find.</param>
    public Text(string text) : base(attributeName, text)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Text(Regex regex) : base(attributeName, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Text(ICompare comparer) : base(attributeName, comparer)
    {
    }
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
  [Obsolete("Use Find.ByStyle() or the class AttributeConstraint instead")]
  public class StyleAttribute : AttributeConstraint
  {
    private const string attributeName = "style.";

    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="value">The value it should match.</param>
    public StyleAttribute(string styleAttributeName, string value) : base(attributeName + styleAttributeName, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Text"/> class.
    /// </summary>
    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="regex">The regex.</param>
    public StyleAttribute(string styleAttributeName, Regex regex) : base(attributeName + styleAttributeName, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StyleAttribute"/> class.
    /// </summary>
    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="comparer">The comparer.</param>
    public StyleAttribute(string styleAttributeName, ICompare comparer)
      : base(attributeName + styleAttributeName, comparer)
    {
    }
  }

  /// <summary>
  /// Class to find a label element placed for an element.
  /// </summary>
  /// <example>
  /// <code>ie.Label(new For("optionbuttonid")).Text</code>
  /// or use
  /// <code>ie.Label(Find.ByFor("optionbuttonid")).Text</code>
  /// </example>
  [Obsolete("Use Find.ByFor() or the class AttributeConstraint instead")]
  public class For : AttributeConstraint
  {
    private const string attributeName = "htmlfor";

    /// <summary>
    /// Initializes a new instance of the <see cref="For"/> class.
    /// </summary>
    /// <param name="forId">For id to find.</param>
    public For(string forId) : base(attributeName, forId)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="For"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public For(Regex regex) : base(attributeName, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="For"/> class.
    /// </summary>
    /// <param name="element">The element to which the Label element is attached. This element must an Id value.</param>
    public For(Element element) : base(attributeName, element.Id)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="For"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public For(ICompare comparer) : base(attributeName, comparer)
    {
    }
  }

  /// <summary>
  /// Class to find a Link, Frame, Internet Explorer window or HTML Dialog by a Url.
  /// </summary>
  /// <example>
  /// <code>ie.Link(new Url("http://watin.sourceforge.net")).Click</code>
  /// or use
  /// <code>ie.Link(Find.ByUrl("http://watin.sourceforge.net")).Url</code>
  /// </example>
  [Obsolete("Use Find.ByUrl() or the class AttributeConstraint instead")]
  public class Url : AttributeConstraint
  {
    private const string attributeName = "href";

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="url">The (well-formed) URL to find.</param>
    public Url(string url) : this(new Uri(url))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="url">The (well-formed) URL to find.</param>
    /// <param name="ignoreQuery">Set to true to ignore querystring when comparing.</param>
    public Url(string url, bool ignoreQuery) : this(new Uri(url), ignoreQuery)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Url(Regex regex) : base(attributeName, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="uri">The URL to find.</param>
    public Url(Uri uri) : this(uri, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="uri">The URL to find.</param>
    /// <param name="ignoreQuery">Set to true to ignore querystring when comparing.</param>
    public Url(Uri uri, bool ignoreQuery) : base(attributeName, uri.ToString())
    {
      comparer = new UriComparer(uri, ignoreQuery);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Url(ICompare comparer) : base(attributeName, comparer)
    {
    }

    /// <summary>
    /// Compares the specified Uri.
    /// </summary>
    /// <param name="uri">The Uri.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c></returns>
    public bool Compare(Uri uri)
    {
      return ((UriComparer) comparer).Compare(uri);
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
  [Obsolete("Use Find.ByTitle() or the class AttributeConstraint instead")]
  public class Title : AttributeConstraint
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
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Title"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Title(ICompare comparer) : base(attributeName, comparer)
    {
    }
  }

  /// <summary>
  /// Class to find an element by it's value.
  /// </summary>
  /// <example>
  /// <code>ie.Button(new Value("My Button"))</code>
  /// or use
  /// <code>ie.Button(Find.ByValue("My Button"))</code>
  /// </example>
  [Obsolete("Use Find.ByValue() or the class AttributeConstraint instead")]
  public class Value : AttributeConstraint
  {
    private const string attributeName = "value";

    /// <summary>
    /// Initializes a new instance of the <see cref="Value"/> class.
    /// </summary>
    /// <param name="value">The value to find.</param>
    public Value(string value) : base(attributeName, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Value"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Value(Regex regex) : base(attributeName, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Value"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Value(ICompare comparer) : base(attributeName, comparer)
    {
    }
  }

  /// <summary>
  /// Class to find an image by it's source (src) attribute.
  /// </summary>
  /// <example>
  /// <code>ie.Image(new Src("image.gif")).Url</code>
  /// or use
  /// <code>ie.Image(Find.BySrc("image.gif")).Url</code>
  /// </example>
  [Obsolete("Use Find.BySrc() or the class AttributeConstraint instead")]
  public class Src : AttributeConstraint
  {
    private const string attributeName = "src";

    /// <summary>
    /// Initializes a new instance of the <see cref="Src"/> class.
    /// </summary>
    /// <param name="name">The exact src to find.</param>
    public Src(string name) : base(attributeName, name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Src"/> class.
    /// </summary>
    /// <param regex="regex">The regular expression to match with.</param>
    public Src(Regex regex) : base(attributeName, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Src"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public Src(ICompare comparer) : base(attributeName, comparer)
    {
    }
  }

  /// <summary>
  /// This class provides factory methods for de most commonly used attributes
  /// to find an element on a web page.
  /// </summary>
  public class Find
  {
		internal const string altAttribute = "alt";
    internal const string idAttribute = "id";
    internal const string forAttribute = "htmlfor";
    internal const string nameAttribute = "name";
    internal const string srcAttribute = "src";
    internal const string styleBaseAttribute = "style.";
    internal const string textAttribute = "innertext";    
    internal const string titleAttribute = "title";    
    internal const string valueAttribute = "value";
    internal const string hrefAttribute = "href";

		/// <summary>
		/// Finds an element by its alt text.
		/// </summary>
		/// <param name="alt">The alt text to find.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// <code>ie.Image(Find.ByAlt("alt text")).Name</code>
		/// </example>
		public static AttributeConstraint ByAlt(string alt)
		{
		  return new AttributeConstraint(altAttribute, alt);
		}

		/// <summary>
		/// Finds an element by its alt text.
		/// </summary>
		/// <param name="regex">The regular expression for the alt text to find.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// <code>ie.Image(Find.ByAlt(new Regex("pattern goes here")))).Name</code>
		/// </example>
		public static AttributeConstraint ByAlt(Regex regex)
		{
			return new AttributeConstraint(altAttribute, regex);
		}

		/// <summary>
		/// Finds an element by its alt text.
		/// </summary>
		/// <param name="compare">The compare.</param>
		/// <returns>The AttributeConstraint</returns>
		/// <example>
		/// 	<code>Image img = ie.Image(Find.ByAlt(new StringContainsAndCaseInsensitiveComparer("alt text")));</code>
		/// </example>
		public static AttributeConstraint ByAlt(ICompare compare)
		{
			return new AttributeConstraint(altAttribute, compare);
		}

    /// <summary>
    /// Find a Label element by the id of the element it's linked with.
    /// </summary>
    /// <param name="forId">Id of the element the label is linked with.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Label(Find.ByFor("optionbuttonid")).Text</code>
    /// </example>
    public static AttributeConstraint ByFor(string forId)
    {
      return new AttributeConstraint(forAttribute, forId);
    }

    /// <param name="regex">Regular expression to find the matching Id of the element
    ///  the label is linked with.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Label(Find.ByFor(new Regex("pattern goes here")).Text</code>
    /// </example>
    public static AttributeConstraint ByFor(Regex regex)
    {
      return new AttributeConstraint(forAttribute, regex);
    }

    /// <param name="element">The element to which the Label element is attached. This element must an Id value.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>
    /// CheckBox checkbox = ie.CheckBox("checkboxid");
    /// ie.Label(Find.ByFor(checkbox).Text</code>
    /// </example>
    public static AttributeConstraint ByFor(Element element)
    {
      return new AttributeConstraint(forAttribute, element.Id);
    }

    /// <param name="comparer">The comparer.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>
    /// Label label = ie.Label(Find.ByFor(new StringContainsAndCaseInsensitiveComparer("optionbuttonid")));</code>
    /// </example>
    public static AttributeConstraint ByFor(ICompare comparer)
    {
      return new AttributeConstraint(forAttribute, comparer);
    }

    /// <summary>
    /// Find an element by its id.
    /// </summary>
    /// <param name="id">Element id to find.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ById("testlinkid")).Url</code>
    /// </example>
    public static AttributeConstraint ById(string id)
    {
      return new AttributeConstraint(idAttribute, id);
    }

    /// <param name="regex">Regular expression to find a matching Id.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ById(new Regex("pattern goes here"))).Url</code>
    /// </example>
    public static AttributeConstraint ById(Regex regex)
    {
      return new AttributeConstraint(idAttribute, regex);
    }

    /// <param name="compare">The compare.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>Link link = ie.Link(Find.ById(new StringContainsAndCaseInsensitiveComparer("linkId1")));</code>
    /// </example>
    public static AttributeConstraint ById(ICompare compare)
    {
      return new AttributeConstraint(idAttribute, compare);
    }

    /// <summary>
    /// Find an element by its index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    public static Index ByIndex(int index)
    {
      return new Index(index);
    }

    /// <summary>
    /// Find an element by its name.
    /// </summary>
    /// <param name="name">Name to find.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByName("testlinkname")).Url</code>
    /// </example>
    public static AttributeConstraint ByName(string name)
    {
      return new AttributeConstraint(nameAttribute, name);
    }

    /// <param regex="regex">Regular expression to find a matching Name.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByName(new Regex("pattern goes here")))).Url</code>
    /// </example>
    public static AttributeConstraint ByName(Regex regex)
    {
      return new AttributeConstraint(nameAttribute, regex);
    }

    /// <summary>
    /// Bies the name.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>ie.Link(Find.ByName(new StringContainsAndCaseInsensitiveComparer("linkname")))).Url</code>
    /// </example>
    public static AttributeConstraint ByName(ICompare comparer)
    {
      return new AttributeConstraint(nameAttribute, comparer);
    }

    /// <summary>
    /// Find an element by its (inner) text
    /// </summary>
    /// <param name="text">Element text</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByText("my link")).Url</code>
    /// </example>
    public static AttributeConstraint ByText(string text)
    {
      return new AttributeConstraint(textAttribute, new StringContainsAndCaseInsensitiveComparer(text));
    }

    /// <param name="regex">Regular expression to find a matching Text.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByText(new Regex("pattern goes here"))).Url</code>
    /// </example>
    public static AttributeConstraint ByText(Regex regex)
    {
      return new AttributeConstraint(textAttribute, regex);
    }

    /// <param name="comparer">The comparer.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>Link link = ie.Link(Find.ByText(new StringContainsAndCaseInsensitiveComparer("my li"))).Url</code>
    /// </example>
    public static AttributeConstraint ByText(ICompare comparer)
    {
      return new AttributeConstraint(textAttribute, comparer);
    }

    /// <summary>
    /// Find an element, frame, IE instance or HTMLDialog by its Url.
    /// </summary>
    /// <param name="url">The well-formed url to find.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByUrl("http://watin.sourceforge.net")).Url</code>
    /// </example>
    public static AttributeConstraint ByUrl(string url)
    {
      return ByUrl(new Uri(url));
    }

    /// <param name="url">The well-formed url to find.</param>
    /// <param name="ignoreQuery">Set to true to ignore querystring when matching.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByUrl("http://watin.sourceforge.net", true)).Url</code>
    /// </example>
    public static AttributeConstraint ByUrl(string url, bool ignoreQuery)
    {
      return ByUrl(new Uri(url), ignoreQuery);
    }

    /// <param name="uri">The uri to find.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByUrl(new Uri("watin.sourceforge.net"))).Url</code>
    /// </example>
    public static AttributeConstraint ByUrl(Uri uri)
    {
      return ByUrl(uri, false);
    }

    /// <param name="uri">The uri to find.</param>
    /// <param name="ignoreQuery">Set to true to ignore querystring when matching.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByUrl(new Uri("watin.sourceforge.net", true))).Url</code>
    /// </example>
    public static AttributeConstraint ByUrl(Uri uri, bool ignoreQuery)
    {
      return new AttributeConstraint(hrefAttribute, new UriComparer(uri, ignoreQuery));
    }

    /// <param name="regex">Regular expression to find a matching Url.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByUrl(new Regex("pattern goes here"))).Url</code>
    /// </example>
    public static AttributeConstraint ByUrl(Regex regex)
    {
      return new AttributeConstraint(hrefAttribute, regex);
    }

    /// <param name="comparer">The comparer.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>ie.Link(Find.ByUrl(new UriComparer(uri, ignoreQuery))).Url</code>
    /// </example>
    public static AttributeConstraint ByUrl(ICompare comparer)
    {
      return new AttributeConstraint(hrefAttribute, comparer);
    }

    /// <summary>
    /// Find an element, frame, IE instance or HTMLDialog by its Title.
    /// </summary>
    /// <param name="title">The title to match partially.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// <code>IE ie = IE.AttachToIE(Find.ByTitle("WatiN Home Page"))</code>
    /// </example>
    public static AttributeConstraint ByTitle(string title)
    {
      return new AttributeConstraint(titleAttribute, new StringContainsAndCaseInsensitiveComparer(title));
    }

    /// <param name="regex">Regular expression to find a matching Title.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// <code>IE ie = IE.AttachToIE(Find.ByTitle(new Regex("pattern goes here")))</code>
    /// </example>
    public static AttributeConstraint ByTitle(Regex regex)
    {
      return new AttributeConstraint(titleAttribute, regex);
    }

    /// <param name="comparer">The comparer.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>IE ie = IE.AttachToIE(Find.ByTitle(new StringContainsAndCaseInsensitiveComparer("part of the title")));</code>
    /// </example>
    public static AttributeConstraint ByTitle(ICompare comparer)
    {
      return new AttributeConstraint(titleAttribute, comparer);
    }

    /// <summary>
    /// Find an element by its value attribute.
    /// </summary>
    /// <param name="value">The value to find.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// <code>ie.Button(Find.ByValue("My Button"))</code>
    /// </example>
    public static AttributeConstraint ByValue(string value)
    {
      return new AttributeConstraint(valueAttribute, value);
    }

    /// <param name="regex">Regular expression to find a matching Value.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// <code>ie.Button(Find.ByValue(new Regex("pattern goes here")))</code>
    /// </example>
    public static AttributeConstraint ByValue(Regex regex)
    {
      return new AttributeConstraint(valueAttribute, regex);
    }

    /// <param name="comparer">The comparer.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>ie.Button(Find.ByValue(new StringContainsAndCaseInsensitiveComparer("pattern goes here")));</code>
    /// </example>
    public static AttributeConstraint ByValue(ICompare comparer)
    {
      return new AttributeConstraint(valueAttribute, comparer);
    }

    /// <summary>
    /// Find an <see cref="Image"/> by its source (src) attribute.
    /// </summary>
    /// <param name="src">Src to find.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Image(Find.BySrc("image.gif"))</code>
    /// </example>
    public static AttributeConstraint BySrc(string src)
    {
      return new AttributeConstraint(srcAttribute, src);
    }

    /// <param regex="regex">Regular expression to find a matching Src.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Image(Find.BySrc(new Regex("pattern goes here"))))</code>
    /// </example>
    public static AttributeConstraint BySrc(Regex regex)
    {
      return new AttributeConstraint(srcAttribute, regex);
    }

    /// <param name="comparer">The comparer.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>Image image = ie.Image(Find.BySrc(new StringContainsAndCaseInsensitiveComparer("watin/sourceforge")));</code>
    /// </example>
    public static AttributeConstraint BySrc(ICompare comparer)
    {
      return new AttributeConstraint(srcAttribute, comparer);
    }

    /// <summary>
    /// Find an element by an attribute.
    /// </summary>
    /// <param name="attributeName">The attribute name to compare the value with.</param>
    /// <param name="value">The exact matching value of the attribute.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.By("id", "testlinkid")).Url</code>
    /// </example>
    public static AttributeConstraint By(string attributeName, string value)
    {
      return new AttributeConstraint(attributeName, value);
    }

    /// <param name="attributeName">The attribute name to compare the value with.</param>
    /// <param name="regex">Regular expression to find a matching value of the given attribute.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.By("id", new Regex("pattern goes here"))).Url</code>
    /// </example>
    public static AttributeConstraint By(string attributeName, Regex regex)
    {
      return new AttributeConstraint(attributeName, regex);
    }

    /// <param name="attributeName">The attribute to compare the value with.</param>
    /// <param name="comparer">The comparer to be used.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>Link link = ie.Link(Find.By("innertext", new StringContainsAndCaseInsensitiveComparer("pattern goes here")));</code>
    /// </example>
    public static AttributeConstraint By(string attributeName, ICompare comparer)
    {
      return new AttributeConstraint(attributeName, comparer);
    }

    /// <summary>
    /// Find an element by an attribute.
    /// </summary>
    /// <param name="attributeName">The attribute to compare the value with.</param>
    /// <param name="value">The exact matching value of the attribute.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByCustom("id", "testlinkid")).Url</code>
    /// </example>
    [Obsolete("Use Find.By(attributeName, value) instead")]
    public static AttributeConstraint ByCustom(string attributeName, string value)
    {
      return By(attributeName, value);
    }

    /// <param name="attributeName">The attribute to compare the value with.</param>
    /// <param name="regex">Regular expression to find a matching value of the given attribute.</param>
    /// <returns><see cref="AttributeConstraint" /></returns>
    /// <example>
    /// <code>ie.Link(Find.ByCustom("id", new Regex("pattern goes here"))).Url</code>
    /// </example>
    [Obsolete("Use Find.By(attributeName, regex) instead")]
    public static AttributeConstraint ByCustom(string attributeName, Regex regex)
    {
      return By(attributeName, regex);
    }

    /// <param name="attributeName">The attribute to compare the value with.</param>
    /// <param name="comparer">The comparer to be used.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>Link link = ie.Link(Find.ByCustom("innertext", new StringContainsAndCaseInsensitiveComparer("pattern goes here")));</code>
    /// </example>
    [Obsolete("Use Find.By(attributeName, comparer) instead")]
    public static AttributeConstraint ByCustom(string attributeName, ICompare comparer)
    {
      return By(attributeName, comparer);
    }

    /// <summary>
    /// Find an element by a style attribute.
    /// </summary>
    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="value">The exact matching value of the attribute.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>ie.Span(Find.ByStyle("background-color", "red"))</code>
    /// </example>
    public static AttributeConstraint ByStyle(string styleAttributeName, string value)
    {
      return new AttributeConstraint(styleBaseAttribute + styleAttributeName, value);
    }

    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="value">Regular expression to find a matching value of the given style attribute.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>ie.Link(Find.ByStyle("font-family", new Regex("pattern goes here")))</code>
    /// </example>
    public static AttributeConstraint ByStyle(string styleAttributeName, Regex value)
    {
      return new AttributeConstraint(styleBaseAttribute + styleAttributeName, value);
    }

    /// <param name="styleAttributeName">Name of the style attribute.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns><see cref="AttributeConstraint"/></returns>
    /// <example>
    /// 	<code>Link link = ie.Link(Find.ByStyle("font-family", new StringContainsAndCaseInsensitiveComparer("aria")));</code>
    /// </example>
    public static AttributeConstraint ByStyle(string styleAttributeName, ICompare comparer)
    {
      return new AttributeConstraint(styleBaseAttribute + styleAttributeName, comparer);
    }
  }
}
