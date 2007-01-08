#region WatiN Copyright (C) 2006 - 2007 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 - 2007 Jeroen van Menen
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
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using mshtml;
using NUnit.Framework;
using NUnit.Mocks;

using WatiN.Core;
using WatiN.Core.Interfaces;
using Attribute=WatiN.Core.Attribute;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class FindElementBy : WatiNTest
  {
    const string href = "href";

    [Test]
    public void FindByFor()
    {
      const string htmlfor = "htmlfor";

      For value = Find.ByFor("foridvalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "For class should inherit Attribute class" );

      Assert.AreEqual(htmlfor, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("foridvalue", value.Value, "Wrong value");
      
      Regex regex = new Regex("^id");
      value = Find.ByFor(regex);
      TestAttributeBag attributeBag = new TestAttributeBag(htmlfor, "idvalue");

      Assert.IsTrue(value.Compare(attributeBag), "Regex ^id should match");
    }

    [Test]
    public void FindByID()
    {
      Id value = Find.ById("idvalue");

      Assert.IsInstanceOfType(typeof(Attribute), value, "Id class should inherit Attribute class" );

      const string id = "id";
      Assert.AreEqual(id, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("idvalue", value.Value, "Wrong value");
      
      Regex regex = new Regex("lue$");
      value = Find.ById(regex);
      TestAttributeBag attributeBag = new TestAttributeBag(id, "idvalue");

      Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match");
    }
    
    [Test]
    public void FindByName()
    {
      Name value = Find.ByName("namevalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Name class should inherit Attribute class" );

      const string name = "name";
      Assert.AreEqual(name, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("namevalue", value.Value, "Wrong value");
      
      Regex regex = new Regex("lue$");
      value = Find.ByName(regex);
      TestAttributeBag attributeBag = new TestAttributeBag(name, "namevalue");

      Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match");
    }

    [Test]
    public void FindByText()
    {
      Text value = Find.ByText("textvalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Text class should inherit Attribute class" );

      const string innertext = "innertext";
      Assert.AreEqual(innertext, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("textvalue", value.Value, "Wrong value");
      
      Regex regex = new Regex("lue$");
      value = Find.ByText(regex);
      TestAttributeBag attributeBag = new TestAttributeBag(innertext, "textvalue");

      Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match");
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
      Assert.AreEqual(href, value.AttributeName, "Wrong attributename");
      Assert.AreEqual(WatiNURI.ToString(), value.Value, "Wrong value");
      
      TestAttributeBag attributeBag = new TestAttributeBag(href, WatiNURI.ToString());

      Assert.IsTrue(value.Compare(attributeBag), "Should match WatiN url");
      Assert.IsTrue(value.Compare(WatiNURI), "Should match WatiN uri");

      attributeBag = new TestAttributeBag(href, "http://www.microsoft.com");
      Assert.IsFalse(value.Compare(attributeBag), "Shouldn't match Microsoft");
      Assert.IsFalse(value.Compare(MainURI), "Shouldn't match mainUri");
      
      attributeBag = new TestAttributeBag(href, null);
      Assert.IsFalse(value.Compare(attributeBag), "Null should not match");

      attributeBag = new TestAttributeBag(href, String.Empty);
      Assert.IsFalse(value.Compare(attributeBag), "Null should not match");
    }

    [Test]
    public void FindByUrlWithRegex()
    {
      Regex regex = new Regex("^http://watin");
      Url value = Find.ByUrl(regex);
      TestAttributeBag attributeBag = new TestAttributeBag(href, "http://watin.sourceforge.net");

      Assert.IsTrue(value.Compare(attributeBag), "Regex ^http://watin should match");
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
      TestAttributeBag attributeBag = new TestAttributeBag(href, "watin.sourceforge.net");

      value.Compare(attributeBag);
    }

    [Test]
    public void FindByTitle()
    {
      const string title = "title";

      Title value = Find.ByTitle("titlevalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Title class should inherit Attribute class" );

      Assert.AreEqual(title, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("titlevalue", value.Value, "Wrong value");
      
      
      TestAttributeBag attributeBag = new TestAttributeBag(title, String.Empty);
      Assert.IsFalse(value.Compare(attributeBag), "Empty should not match");

      attributeBag = new TestAttributeBag(title, null);
      Assert.IsFalse(value.Compare(attributeBag), "Null should not match");
      
      attributeBag = new TestAttributeBag(title, "titlevalue");
 
      Assert.IsTrue(value.Compare(attributeBag), "Compare should match");

      value = Find.ByTitle("titl");
      Assert.IsTrue(value.Compare(attributeBag), "Compare should partial match titl");
      
      value = Find.ByTitle("tItL");
      Assert.IsTrue(value.Compare(attributeBag), "Compare should partial match tItL");

      value = Find.ByTitle("lev");
      Assert.IsTrue(value.Compare(attributeBag), "Compare should partial match lev");
      
      value = Find.ByTitle("alue");
      Assert.IsTrue(value.Compare(attributeBag), "Compare should partial match alue");

      Regex regex = new Regex("^titl");
      value = Find.ByTitle(regex);
      Assert.IsTrue(value.Compare(attributeBag), "Regex ^titl should match");
      
      value = Find.ByTitle("titlevalue");
      attributeBag = new TestAttributeBag(title, "title");

      Assert.IsFalse(value.Compare(attributeBag), "Compare should not match title");
      
    }

    [Test]
    public void FindByValue()
    {
      const string valueAttrib = "value";

      Value value = Find.ByValue("valuevalue");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Value class should inherit Attribute class" );

      Assert.AreEqual(valueAttrib, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("valuevalue", value.Value, "Wrong value");
      Assert.AreEqual("valuevalue", value.ToString(), "Wrong ToString result");
      
      Regex regex = new Regex("lue$");
      value = Find.ByValue(regex);
      TestAttributeBag attributeBag = new TestAttributeBag(valueAttrib, "valuevalue");
      
      Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match");
    }

    [Test]
    public void FindBySrc()
    {
      const string src = "src";

      Src value = Find.BySrc("image.gif");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "Src class should inherit Attribute class" );

      Assert.AreEqual(src, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("image.gif", value.Value, "Wrong value");
      
      TestAttributeBag attributeBag = new TestAttributeBag(src, "/images/image.gif");
      Assert.IsFalse(value.Compare(attributeBag), "Should not match /images/image.gif");

      attributeBag = new TestAttributeBag(src, "image.gif");
      Assert.IsTrue(value.Compare(attributeBag), "Should match image.gif");
      
      Regex regex = new Regex("image.gif$");
      value = Find.BySrc(regex);
      attributeBag = new TestAttributeBag(src, "/images/image.gif");

      Assert.IsTrue(value.Compare(attributeBag), "Regex image.gif$ should match");
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
    
    [Test]
    public void FindByCustom()
    {
      const string id = "id";
      Attribute value = Find.ByCustom(id,"idvalue");
      Assert.AreEqual(id, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("idvalue", value.Value, "Wrong value");
      
      TestAttributeBag attributeBag = new TestAttributeBag(id, "idvalue");
      Assert.IsTrue(value.Compare(attributeBag), "Compare should match");

      attributeBag = new TestAttributeBag(id, "id");
      Assert.IsFalse(value.Compare(attributeBag), "Compare should not partial match id");

      attributeBag = new TestAttributeBag(id, "val");
      Assert.IsFalse(value.Compare(attributeBag), "Compare should not partial match val");

      attributeBag = new TestAttributeBag(id, "value");
      Assert.IsFalse(value.Compare(attributeBag), "Compare should not partial match value");
      
      Regex regex = new Regex("lue$");
      value = Find.ByCustom(id, regex);
      attributeBag = new TestAttributeBag(id, "idvalue");

      Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match");
    }

    [Test]
    public void ElementUrlPartialValue()
    {
      ElementUrlPartialValue value = new ElementUrlPartialValue("watin.sourceforge");
      
      Assert.IsInstanceOfType(typeof(Url), value, "ElementUrlPartialValue class should inherit Url class" );

      Assert.AreEqual(href, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("watin.sourceforge", value.Value, "Wrong value");
      
      TestAttributeBag attributeBag = new TestAttributeBag(href, "watin.sourceforge");
      Assert.IsTrue(value.Compare(attributeBag), "Compare should match");
      
      value = new ElementUrlPartialValue("watin.sourceforge");

      attributeBag = new TestAttributeBag(href, WatiNURI.ToString());
      Assert.IsTrue(value.Compare(attributeBag), "Compare should partial match title");      

      attributeBag = new TestAttributeBag(href, "www.microsoft.com");
      Assert.IsFalse(value.Compare(attributeBag), "Compare should not match title");

      using (new IE(MainURI))
      {
        string partialUrl = MainURI.ToString();
        partialUrl = partialUrl.Remove(0, partialUrl.Length - 8);

        using(IE ie = IE.AttachToIE(new ElementUrlPartialValue(partialUrl)))
        {
          Assert.AreEqual(MainURI, ie.Uri);
        }
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
  
  [TestFixture]
  public class ElementTagTests
  {
    [Test]
    public void CompareNullShouldReturnFalse()
    {
      ElementTag elementTag = new ElementTag("tagname", "");
      Assert.IsFalse(elementTag.Compare(null));
    }
    
    [Test]
    public void CompareObjectNotImplementingIHTMLElementShouldReturnFalse()
    {
      ElementTag elementTag = new ElementTag("tagname", "");
      Assert.IsFalse(elementTag.Compare(new object()));
    }
  }
  
  [TestFixture]
  public class FindByMultipleAttributes
  {
    [Test]
    public void AndTrue()
    {
      Attribute findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "X");
      attributeBag.Add("value", "Cancel");
      Assert.IsTrue(findBy.Compare(attributeBag));
    }

    [Test]
    public void AndFalseFirst()
    {
      Attribute findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Y");
      attributeBag.Add("value", "Cancel");
      Assert.IsFalse(findBy.Compare(attributeBag));
    }

    [Test]
    public void AndFalseSecond()
    {
      Attribute findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "X");
      attributeBag.Add("value", "OK");
      Assert.IsFalse(findBy.Compare(attributeBag));
    }

    [Test]
    public void OrFirstTrue()
    {
      Attribute findBy = Find.ByName("X").Or(Find.ByName("Y"));
      TestAttributeBag attributeBag = new TestAttributeBag("name", "X");
      Assert.IsTrue(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void OrSecondTrue()
    {
      Attribute findBy = Find.ByName("X").Or(Find.ByName("Y"));
      TestAttributeBag attributeBag = new TestAttributeBag("name", "Y");
      Assert.IsTrue(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void OrFalse()
    {
      Attribute findBy = Find.ByName("X").Or(Find.ByName("Y"));
      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsFalse(findBy.Compare(attributeBag));
    }
       
    [Test]
    public void AndOr()
    {
      Attribute findByNames = Find.ByName("X").Or(Find.ByName("Y"));
      Attribute findBy = Find.ByValue("Cancel").And(findByNames);

      TestAttributeBag attributeBag = new TestAttributeBag("name", "X");
      attributeBag.Add("value", "Cancel");
      Assert.IsTrue(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void AndOrThroughOperatorOverloads()
    {
      Attribute findBy = Find.ByName("X") & Find.ByValue("Cancel") | (Find.ByName("Z") & Find.ByValue("Cancel"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      attributeBag.Add("value", "OK");
      Assert.IsFalse(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void Occurence0()
    {
      Attribute findBy = new Occurrence(0);

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsTrue(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void Occurence2()
    {
      Attribute findBy = new Occurrence(2);

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsTrue(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void OccurenceAndTrue()
    {
      Attribute findBy = new Occurrence(1).And(Find.ByName("Z"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsTrue(findBy.Compare(attributeBag));
    }

    [Test]
    public void OccurenceOr()
    {
      Attribute findBy = new Occurrence(2).Or(Find.ByName("Z"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsTrue(findBy.Compare(attributeBag));
      
      attributeBag = new TestAttributeBag("name", "y");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsTrue(findBy.Compare(attributeBag));
    }

    [Test]
    public void OccurenceAndFalse()
    {
      Attribute findBy = new Occurrence(1).And(Find.ByName("Y"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
    }

    [Test]
    public void OccurenceAndOrWithOrTrue()
    {
      Attribute findBy = new Occurrence(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsTrue(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void OccurenceAndOrWithAndTrue()
    {
      Attribute findBy = new Occurrence(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Y");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsTrue(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
    }
  }
  
  [TestFixture]
  public class ElementAttributeBagTests
  {
    [Test]
    public void StyleAttributeShouldReturnAsString()
    {
      const string cssText = "COLOR: white; FONT-STYLE: italic";

      DynamicMock mockIStyle = new DynamicMock(typeof(IHTMLStyle));
      mockIStyle.ExpectAndReturn("get_cssText", cssText);

      DynamicMock mockIHtmlElement = new DynamicMock(typeof(IHTMLElement));
      mockIHtmlElement.ExpectAndReturn("get_style", mockIStyle.MockInstance);

      ElementAttributeBag attributeBag = new ElementAttributeBag((IHTMLElement)mockIHtmlElement.MockInstance);
      
      Assert.AreEqual(cssText, attributeBag.GetValue("style"));

      mockIHtmlElement.Verify();
      mockIStyle.Verify();
    }
  }
  
  public class TestAttributeBag : IAttributeBag
  {
    public NameValueCollection attributeValues = new NameValueCollection();

    public TestAttributeBag(string attributeName, string value)
    {
      Add(attributeName, value);
    }

    public void Add(string attributeName, string value)
    {
      attributeValues.Add(attributeName.ToLower(), value);
    }
    
    public string GetValue(string attributename)
    {
      return attributeValues.Get(attributename.ToLower());
    }
  }
}
