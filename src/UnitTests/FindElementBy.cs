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
using System.Text.RegularExpressions;
using NUnit.Framework;
using WatiN.Core;
using WatiN.Core.Interfaces;
using Attribute=WatiN.Core.Attribute;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class FindElementBy : WatiNTest
  {
    [Test]
    public void FindByFor()
    {
      For value = Find.ByFor("foridvalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "For class should inherit Attribute class" );

      Assert.AreEqual("htmlfor", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("foridvalue", value.Value, "Wrong value");
      
      Regex regex = new Regex("^id");
      value = Find.ByFor(regex);
      Assert.IsTrue(value.Compare("idvalue"), "Regex ^id should match");
    }

    [Test]
    public void FindByID()
    {
      Id value = Find.ById("idvalue");

      Assert.IsInstanceOfType(typeof(Attribute), value, "Id class should inherit Attribute class" );

      Assert.AreEqual("id", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("idvalue", value.Value, "Wrong value");
      
      Regex regex = new Regex("lue$");
      value = Find.ById(regex);
      Assert.IsTrue(value.Compare("idvalue"), "Regex lue$ should match");
    }
    
    [Test]
    public void FindByName()
    {
      Name value = Find.ByName("namevalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Name class should inherit Attribute class" );

      Assert.AreEqual("name", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("namevalue", value.Value, "Wrong value");
      
      Regex regex = new Regex("lue$");
      value = Find.ByName(regex);
      Assert.IsTrue(value.Compare("namevalue"), "Regex lue$ should match");
    }

    [Test]
    public void FindByText()
    {
      Text value = Find.ByText("textvalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Text class should inherit Attribute class" );

      Assert.AreEqual("innertext", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("textvalue", value.Value, "Wrong value");
      
      Regex regex = new Regex("lue$");
      value = Find.ByText(regex);
      Assert.IsTrue(value.Compare("textvalue"), "Regex lue$ should match");
    }

    [Test]
    public void FindByUrl()
    {
      string url = WatiNURI.ToString();
      Url value = Find.ByUrl(url);
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Url class should inherit Attribute class" );

      AssertUrlValue(value);
    }

    [Test, ExpectedException(typeof(UriFormatException))]
    public void FindingEmptyUrlNotAllowed()
    {
      new Url(String.Empty);
    }
    
    [Test]
    public void FindByUri()
    {
      Url value = new Url(WatiNURI);
      AssertUrlValue(value);
    }
    
    private static void AssertUrlValue(Url value)
    {
      Assert.AreEqual("href", value.AttributeName, "Wrong attributename");
      Assert.AreEqual(WatiNURI.ToString(), value.Value, "Wrong value");
      Assert.IsTrue(value.Compare(WatiNURI.ToString()), "Should match WatiN url");
      Assert.IsTrue(value.Compare(WatiNURI), "Should match WatiN uri");
      Assert.IsFalse(value.Compare("http://www.microsoft.com"), "Shouldn't match Microsoft");
      Assert.IsFalse(value.Compare(MainURI), "Shouldn't match mainUri");
      Assert.IsFalse(value.Compare(null), "Null should not match");
      Assert.IsFalse(value.Compare(String.Empty), "Null should not match");
    }

    [Test]
    public void FindByUrlWithRegex()
    {
      Regex regex = new Regex("^http://watin");
      Url value = Find.ByUrl(regex);
      Assert.IsTrue(value.Compare("http://watin.sourceforge.net"), "Regex ^http://watin should match");
    }

    [Test, ExpectedException(typeof(UriFormatException))]
    public void FindByUrlInvalidParam()
    {
      Find.ByUrl("www.xyz.nl");
    }

    [Test, ExpectedException(typeof(UriFormatException))]
    public void FindByUrlInvalidCompare()
    {
      Url value = Find.ByUrl(WatiNURI.ToString());
      value.Compare("watin.sourceforge.net");
    }

    [Test]
    public void FindByTitle()
    {
      Title value = Find.ByTitle("titlevalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Title class should inherit Attribute class" );

      Assert.AreEqual("title", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("titlevalue", value.Value, "Wrong value");
      
      Assert.IsTrue(value.Compare("titlevalue"), "Compare should match");
      
      Assert.IsFalse(value.Compare(String.Empty), "Empty should not match");
      Assert.IsFalse(value.Compare(null), "Null should not match");
      
      value = Find.ByTitle("titl");
      Assert.IsTrue(value.Compare("titlevalue"), "Compare should partial match titl");
      
      value = Find.ByTitle("tItL");
      Assert.IsTrue(value.Compare("titlevalue"), "Compare should partial match tItL");

      value = Find.ByTitle("lev");
      Assert.IsTrue(value.Compare("titlevalue"), "Compare should partial match lev");
      
      value = Find.ByTitle("alue");
      Assert.IsTrue(value.Compare("titlevalue"), "Compare should partial match alue");

      value = Find.ByTitle("titlevalue");
      Assert.IsFalse(value.Compare("title"), "Compare should not match title");
      
      Regex regex = new Regex("^titl");
      value = Find.ByTitle(regex);
      Assert.IsTrue(value.Compare("titlevalue"), "Regex ^titl should match");
    }

    [Test]
    public void FindByValue()
    {
      Value value = Find.ByValue("valuevalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Value class should inherit Attribute class" );

      Assert.AreEqual("value", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("valuevalue", value.Value, "Wrong value");
      Assert.AreEqual("valuevalue", value.ToString(), "Wrong ToString result");
      
      Regex regex = new Regex("lue$");
      value = Find.ByValue(regex);
      Assert.IsTrue(value.Compare("valuevalue"), "Regex lue$ should match");
    }

    [Test]
    public void FindBySrc()
    {
      Src value = Find.BySrc("image.gif");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Src class should inherit Attribute class" );

      Assert.AreEqual("src", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("image.gif", value.Value, "Wrong value");
      
      Assert.IsFalse(value.Compare("/images/image.gif"), "Should not match /images/image.gif");
      Assert.IsTrue(value.Compare("image.gif"), "Should match image.gif");
      
      Regex regex = new Regex("image.gif$");
      value = Find.BySrc(regex);
      Assert.IsTrue(value.Compare("/images/image.gif"), "Regex image.gif$ should match");
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewAttributeWithNullAttribute()
    {
      new Attribute(null,"idvalue");
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewAttributeWithNullValue()
    {
      new Attribute("id",(string)null);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewAttributeWithNulls()
    {
      new Attribute(null,(string)null);
    }
    
    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewAttributeWithNullRegex()
    {
      new Attribute("id",(Regex)null);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewAttributeWithNullsRegex()
    {
      new Attribute(null,(Regex)null);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewAttributeWithEmptyAttribute()
    {
      new Attribute(string.Empty,"idvalue");
    }

    [Test]
    public void NewAttributeWithEmptyValue()
    {
      Attribute attribute = new Attribute("id",string.Empty);
      Assert.IsEmpty(attribute.Value);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewAttributeWithEmpties()
    {
      new Attribute(string.Empty,string.Empty);
    }
    
    [Test, ExpectedException(typeof(ArgumentException))]
    public void AttributeCompareObject()
    {
      Attribute attribute = new Attribute("test", "value");
      attribute.Compare(new object());
    }
    
    [Test]
    public void FindByCustom()
    {
      Attribute value = Find.ByCustom("id","idvalue");
      Assert.AreEqual("id", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("idvalue", value.Value, "Wrong value");
      
      Assert.IsTrue(value.Compare("idvalue"), "Compare should match");
      Assert.IsFalse(value.Compare("id"), "Compare should not partial match id");
      Assert.IsFalse(value.Compare("val"), "Compare should not partial match val");
      Assert.IsFalse(value.Compare("value"), "Compare should not partial match value");
      
      Regex regex = new Regex("lue$");
      value = Find.ByCustom("id", regex);
      Assert.IsTrue(value.Compare("idvalue"), "Regex lue$ should match");
    }

    [Test]
    public void ElementUrlPartialValue()
    {
      ElementUrlPartialValue value = new ElementUrlPartialValue("watin.sourceforge");
      
      Assert.IsInstanceOfType(typeof(Url), value, "ElementUrlPartialValue class should inherit Url class" );

      Assert.AreEqual("href", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("watin.sourceforge", value.Value, "Wrong value");
      
      Assert.IsTrue(value.Compare("watin.sourceforge"), "Compare should match");
      
      value = new ElementUrlPartialValue("watin.sourceforge");
      Assert.IsTrue(value.Compare(WatiNURI.ToString()), "Compare should partial match title");      
      Assert.IsFalse(value.Compare("www.microsoft.com"), "Compare should not match title");

      using (new IE(MainURI))
      {
        string partialUrl = MainURI.ToString();
        partialUrl = partialUrl.Remove(0, partialUrl.Length - 8);

        IE ie = IE.AttachToIE(new ElementUrlPartialValue(partialUrl));
        
        Assert.AreEqual(MainURI, ie.Uri);
      }
    }   
  }

  public class ElementUrlPartialValue : Url
  {
    private string url = null;

    public ElementUrlPartialValue(string url) : base("http://www.fakeurl.com")
    {
      this.url = url;  
      comparer = new StringContainsAndCaseInsensitiveComparer(url);
    }

    public override string Value
    {
       get { return url; }
    }
  }
  
  [TestFixture]
  public class StringComparerTests
  {
    [Test]
    public void ConstructorWithValue()
    {
      ICompare comparer = new WatiN.Core.StringComparer("A test value");
      
      Assert.IsTrue(comparer.Compare("A test value"), "Exact match should pass.");
      
      Assert.IsFalse(comparer.Compare("a test Value"), "Match should be case sensitive");
      Assert.IsFalse(comparer.Compare("A test value 2"), "Exact match plus more should not pass.");
      Assert.IsFalse(comparer.Compare("test"), "Partial match should not match");
      Assert.IsFalse(comparer.Compare("completely different"), "Something completely different should not match");
      Assert.IsFalse(comparer.Compare(String.Empty), "String.Empty should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }
    
    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorWithNullShouldThrowArgumentNullException()
    {
      new WatiN.Core.StringComparer(null);
    }
    
    [Test]
    public void ConstuctorWithStringEmpty()
    {
      ICompare comparer = new WatiN.Core.StringComparer(String.Empty);
      
      Assert.IsTrue(comparer.Compare(String.Empty), "String.Empty should match");
      
      Assert.IsFalse(comparer.Compare(" "), "None Empty string should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }
    
    [Test]
    public void ToStringTest()
    {
      WatiN.Core.StringComparer comparer = new WatiN.Core.StringComparer("A test value");
      
      Assert.AreEqual("WatiN.Core.StringComparer compares with: A test value", comparer.ToString());
    }
  }
  
  [TestFixture]
  public class StringContainsAndCaseInsensitiveComparerTests
  {
    [Test]
    public void ConstructorWithValue()
    {
      ICompare comparer = new StringContainsAndCaseInsensitiveComparer("A test value");
      
      Assert.IsTrue(comparer.Compare("A test value"), "Exact match should pass.");
      Assert.IsTrue(comparer.Compare("a test Value"), "Case should be ignored");
      Assert.IsTrue(comparer.Compare("A test value 2"), "Exact match plus more should pass.");
      
      Assert.IsFalse(comparer.Compare("test"), "A part of the Value should not match");
      Assert.IsFalse(comparer.Compare("completely different"), "Something completely different should not match");
      Assert.IsFalse(comparer.Compare(String.Empty), "String.Empty should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }
    
    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorWithNullShouldThrowArgumentNullException()
    {
      new StringContainsAndCaseInsensitiveComparer(null);
    }
    
    [Test]
    public void ConstuctorWithStringEmpty()
    {
      ICompare comparer = new StringContainsAndCaseInsensitiveComparer(String.Empty);
      
      Assert.IsTrue(comparer.Compare(String.Empty), "String.Empty should match");
      
      Assert.IsFalse(comparer.Compare(" "), "None Empty string should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }
    
    [Test]
    public void ToStringTest()
    {
      StringContainsAndCaseInsensitiveComparer comparer = new StringContainsAndCaseInsensitiveComparer("A test value");
      
      Assert.AreEqual("WatiN.Core.StringContainsAndCaseInsensitiveComparer compares with: a test value", comparer.ToString());
    }
  }
  
  [TestFixture]
  public class StringEqualsAndCaseInsensitiveComparerTests
  {
    [Test]
    public void ConstructorWithValue()
    {
      ICompare comparer = new StringEqualsAndCaseInsensitiveComparer("A test value");
      
      Assert.IsTrue(comparer.Compare("A test value"), "Exact match should pass.");      
      Assert.IsTrue(comparer.Compare("a test Value"), "Match should be case insensitive");
      
      Assert.IsFalse(comparer.Compare("A test value 2"), "Exact match plus more should not pass.");
      Assert.IsFalse(comparer.Compare("test"), "Partial match should not match");
      Assert.IsFalse(comparer.Compare("completely different"), "Something completely different should not match");
      Assert.IsFalse(comparer.Compare(String.Empty), "String.Empty should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }
    
    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorWithNullShouldThrowArgumentNullException()
    {
      new StringEqualsAndCaseInsensitiveComparer(null);
    }
    
    [Test]
    public void ConstuctorWithStringEmpty()
    {
      ICompare comparer = new StringEqualsAndCaseInsensitiveComparer(String.Empty);
      
      Assert.IsTrue(comparer.Compare(String.Empty), "String.Empty should match");
      
      Assert.IsFalse(comparer.Compare(" "), "None Empty string should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }
    
    [Test]
    public void ToStringTest()
    {
      StringEqualsAndCaseInsensitiveComparer comparer = new StringEqualsAndCaseInsensitiveComparer("A test value");
      
      Assert.AreEqual("WatiN.Core.StringEqualsAndCaseInsensitiveComparer compares with: A test value", comparer.ToString());
    }
  }
  
  [TestFixture]
  public class RegexComparerTests
  {
    [Test]
    public void ConstructorWithValue()
    {
      ICompare comparer = new RegexComparer(new Regex("^A test value$"));
      
      Assert.IsTrue(comparer.Compare("A test value"), "Exact match should pass.");      
      
      Assert.IsFalse(comparer.Compare("a test Value"), "Match should be case sensitive");
      Assert.IsFalse(comparer.Compare("A test value 2"), "Exact match plus more should not pass.");
      Assert.IsFalse(comparer.Compare("test"), "Partial match should not match");
      Assert.IsFalse(comparer.Compare("completely different"), "Something completely different should not match");
      Assert.IsFalse(comparer.Compare(String.Empty), "String.Empty should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }
    
    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorWithNullShouldThrowArgumentNullException()
    {
      new RegexComparer(null);
    }
    
    [Test]
    public void ConstuctorWithStringEmpty()
    {
      ICompare comparer = new RegexComparer(new Regex(String.Empty));
      
      Assert.IsTrue(comparer.Compare(String.Empty), "String.Empty should match");
      Assert.IsTrue(comparer.Compare(" "), "Any string should not match");
      
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }
    
    [Test]
    public void ToStringTest()
    {
      RegexComparer comparer = new RegexComparer(new Regex("^A test value$"));
      
      Assert.AreEqual("WatiN.Core.RegexComparer matching against: ^A test value$", comparer.ToString());
    }
  }
  
  [TestFixture]
  public class UriComparerTests
  {
    [Test]
    public void ConstructorWithValueAndStringCompare()
    {
      ICompare comparer = new UriComparer(new Uri("http://watin.sourceforge.net"));
      
      // String Compare
      Assert.IsTrue(comparer.Compare("http://watin.sourceforge.net"), "Exact match should pass.");      
      Assert.IsTrue(comparer.Compare("HTTP://watin.Sourceforge.net"), "Match should not be case sensitive");

      Assert.IsFalse(comparer.Compare("http://watin.sourceforge.net/index.html"), "Exact match plus more should not pass.");
      Assert.IsFalse(comparer.Compare("http://watin"), "Partial match should not match");
      Assert.IsFalse(comparer.Compare("file://html/main.html"), "Something completely different should not match");
      Assert.IsFalse(comparer.Compare(String.Empty), "String.Empty should not match");
      Assert.IsFalse(comparer.Compare(null), "String: null should not match");
    }
    
    [Test]
    public void ConstructorWithValueAndUriCompare()
    {
      UriComparer comparer = new UriComparer(new Uri("http://watin.sourceforge.net"));
      
      // Uri Compare
      Assert.IsTrue(comparer.Compare(new Uri("http://watin.sourceforge.net")), "Uri: Exact match should pass.");      
      Assert.IsTrue(comparer.Compare(new Uri("HTTP://watin.Sourceforge.net")), "Uri: Match should not be case sensitive");

      Assert.IsFalse(comparer.Compare(new Uri("http://watin.sourceforge.net/index.html")), "Uri: Exact match plus more should not pass.");
      Assert.IsFalse(comparer.Compare(new Uri("http://watin")), "Uri: Partial match should not match");
      Assert.IsFalse(comparer.Compare(new Uri("file://html/main.html")), "Uri: Something completely different should not match");
      Assert.IsFalse(comparer.Compare((Uri)null), "Uri: null should not match");
    }
    
    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorWithNullShouldThrowArgumentNullException()
    {
      new UriComparer(null);
    }
    
    [Test, ExpectedException(typeof(UriFormatException))]
    public void StringCompareOnlyExceptsValidUrl()
    {
      ICompare comparer = new UriComparer(new Uri("http://watin.sourceforge.net"));

      comparer.Compare("watin");
    }
    
    [Test]
    public void ToStringTest()
    {
      UriComparer comparer = new UriComparer(new Uri("http://watin.sourceforge.net"));
      
      Assert.AreEqual("WatiN.Core.UriComparer compares with: http://watin.sourceforge.net/", comparer.ToString());
    }
  }
}
