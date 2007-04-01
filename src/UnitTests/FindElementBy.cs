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

using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using mshtml;
using NUnit.Framework;

using WatiN.Core;
using WatiN.Core.Interfaces;
using Attribute=WatiN.Core.Attribute;

namespace WatiN.UnitTests
{
  using System.Collections;
  using System.Diagnostics;
  using Rhino.Mocks;
  using WatiN.Core.Exceptions;

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
    }

    [Test]
    public void IdWithRegexAndComparer()
    {
      MockRepository mocks = new MockRepository();
      ICompare comparer = (ICompare) mocks.CreateMock(typeof(ICompare));
      IAttributeBag attributeBag = (IAttributeBag) mocks.CreateMock(typeof(IAttributeBag));

      Expect.Call(attributeBag.GetValue("id")).Return("idvalue");
      
      Expect.Call(attributeBag.GetValue("id")).Return("MyMockComparer");
      Expect.Call(comparer.Compare("MyMockComparer")).Return(true);
      
      mocks.ReplayAll();

      Id value = Find.ById(new Regex("lue$"));
      Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match.");

      // See if mocked comparer is used. VerifyAll will check this
      new Id(comparer).Compare(attributeBag);

      mocks.VerifyAll();
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
      Core.Text value = Find.ByText("textvalue");
      
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
    public void FindByStyle()
    {
      const string attributeName = "background-color";
      StyleAttribute value = Find.ByStyle(attributeName,"red");
      
      Assert.IsInstanceOfType(typeof(Attribute), value, "StyleAttribute class should inherit Attribute class" );

      const string fullAttributeName = "style.background-color";
      Assert.AreEqual(fullAttributeName, value.AttributeName, "Wrong attributename");
      Assert.AreEqual("red", value.Value, "Wrong value");
      
      Regex regex = new Regex("een$");
      value = Find.ByStyle(attributeName, regex);
      TestAttributeBag attributeBag = new TestAttributeBag(fullAttributeName, "green");

      Assert.IsTrue(value.Compare(attributeBag), "Regex een$ should match");
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
  public class BoolComparerTests
  {
    [Test]
    public void CompareToTrue()
    {
      ICompare comparer = new BoolComparer(true);

      Assert.IsTrue(comparer.Compare(true.ToString()), "true.ToString()");
      Assert.IsTrue(comparer.Compare("true"), "true");
      Assert.IsTrue(comparer.Compare("True"), "True");
      Assert.IsFalse(comparer.Compare("false"), "false");
      Assert.IsFalse(comparer.Compare("some other string"), "some other string");
    }

    [Test]
    public void CompareToNull()
    {
      Assert.IsFalse(new BoolComparer(false).Compare(null), "null");
    }

    [Test]
    public void CompareToStringEmpty()
    {
      Assert.IsFalse(new BoolComparer(false).Compare(String.Empty), String.Empty);
    }
    
    [Test]
    public void CompareToFalse()
    {
      ICompare comparer = new BoolComparer(false);

      Assert.IsTrue(comparer.Compare(false.ToString()), "false.ToString()");
      Assert.IsTrue(comparer.Compare("false"), "false");
      Assert.IsTrue(comparer.Compare("False"), "False");
      Assert.IsFalse(comparer.Compare("true"), "true");
      Assert.IsFalse(comparer.Compare("some other string"), "some other string");
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
      
      Assert.AreEqual("A test value", comparer.ToString());
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

    [Test]
    public void IsValidElementWithNullElementShouldReturnFalse()
    {
      Assert.IsFalse(ElementTag.IsValidElement(null, new ArrayList()));
    }

    [Test]
    public void IsValidElementWithObjectNotImplementingIHTMLElementShouldReturnFalse()
    {
      Assert.IsFalse(ElementTag.IsValidElement(new object(), new ArrayList()));
    }
  }
  
  [TestFixture]
  public class FindByMultipleAttributes
  {
    MockRepository mocks;
    IAttributeBag mockAttributeBag;

    [SetUp]
    public void Setup()
    {
      mocks = new MockRepository();
      mockAttributeBag = (IAttributeBag)mocks.CreateMock(typeof(IAttributeBag));
    }

    [Test]
    public void AndTrue()
    {
      Attribute findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

      Expect.Call(mockAttributeBag.GetValue("name")).Return("X");
      Expect.Call(mockAttributeBag.GetValue("value")).Return("Cancel");
      
      mocks.ReplayAll();

      Assert.IsTrue(findBy.Compare(mockAttributeBag));

      mocks.VerifyAll();
    }

    [Test]
    public void AndFalseFirstSoSecondPartShouldNotBeEvaluated()
    {
      Attribute findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

      Expect.Call(mockAttributeBag.GetValue("name")).Return("Y");

      mocks.ReplayAll();

      Assert.IsFalse(findBy.Compare(mockAttributeBag));

      mocks.VerifyAll();
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
    
    [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OccurrenceShouldNotAcceptNegativeValue()
    {
      new Index(-1);
    }

    [Test]
    public void Occurence0()
    {
      Attribute findBy = new Index(0);

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsTrue(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void Occurence2()
    {
      Attribute findBy = new Index(2);

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsTrue(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void OccurenceAndTrue()
    {
      Attribute findBy = new Index(1).And(Find.ByName("Z"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsTrue(findBy.Compare(attributeBag));
    }

    [Test]
    public void OccurenceOr()
    {
      Attribute findBy = new Index(2).Or(Find.ByName("Z"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsTrue(findBy.Compare(attributeBag));
      
      attributeBag = new TestAttributeBag("name", "y");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsTrue(findBy.Compare(attributeBag));
    }

    [Test]
    public void OccurenceAndFalse()
    {
      Attribute findBy = new Index(1).And(Find.ByName("Y"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
    }

    [Test]
    public void TrueAndOccurence()
    {
      Attribute findBy = Find.ByName("Z").And(new Index(1));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsTrue(findBy.Compare(attributeBag));
    }

    [Test]
    public void FalseAndOccurence()
    {
      Attribute findBy = Find.ByName("Y").And(new Index(1));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
    }

    [Test]
    public void TrueAndOccurenceAndTrue()
    {
      Attribute findBy = Find.ByName("Z").And(new Index(1)).And(Find.ByValue("text"));

      Expect.Call(mockAttributeBag.GetValue("name")).Return("Z");
      Expect.Call(mockAttributeBag.GetValue("value")).Return("text");

      Expect.Call(mockAttributeBag.GetValue("name")).Return("Z");
      Expect.Call(mockAttributeBag.GetValue("value")).Return("some other text");
      
      Expect.Call(mockAttributeBag.GetValue("name")).Return("Y");
      
      Expect.Call(mockAttributeBag.GetValue("name")).Return("Z");
      Expect.Call(mockAttributeBag.GetValue("value")).Return("text");
      
      mocks.ReplayAll();

      Assert.IsFalse(findBy.Compare(mockAttributeBag));
      Assert.IsFalse(findBy.Compare(mockAttributeBag));
      Assert.IsFalse(findBy.Compare(mockAttributeBag));
      Assert.IsTrue(findBy.Compare(mockAttributeBag));

      mocks.VerifyAll();
    }

    [Test]
    public void OccurenceAndOrWithOrTrue()
    {
      Attribute findBy = new Index(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Z");
      Assert.IsTrue(findBy.Compare(attributeBag));
    }
    
    [Test]
    public void OccurenceAndOrWithAndTrue()
    {
      Attribute findBy = new Index(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

      TestAttributeBag attributeBag = new TestAttributeBag("name", "Y");
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
      Assert.IsTrue(findBy.Compare(attributeBag));
      Assert.IsFalse(findBy.Compare(attributeBag));
    }

    [Test, ExpectedException(typeof(ReEntryException))]
    public void RecusiveCallExceptionExpected()
    {
      Attribute findBy = Find.ByCustom("tag", "value");
      findBy.Or(findBy);

      Expect.Call(mockAttributeBag.GetValue("tag")).Return("val").Repeat.AtLeastOnce();

      mocks.ReplayAll();
      findBy.Compare(mockAttributeBag);
      mocks.VerifyAll();
    }
  }

  [TestFixture]
  public class ComplexMultipleAttributes
  {
    MockRepository mocks;
    IAttributeBag mockAttributeBag;

    Attribute findBy1;
    Attribute findBy2;
    Attribute findBy3;
    Attribute findBy4;
    Attribute findBy5;
    
    Attribute findBy;

    [SetUp]
    public void Setup()
    {
      mocks = new MockRepository();
      mockAttributeBag = (IAttributeBag)mocks.CreateMock(typeof(IAttributeBag));
      findBy = null;

      findBy1 = Find.ByCustom("1", "true");
      findBy2 = Find.ByCustom("2", "true");
      findBy3 = Find.ByCustom("3", "true");
      findBy4 = Find.ByCustom("4", "true");
      findBy5 = Find.ByCustom("5", "true");
    }

    [Test]
    public void WithoutBrackets()
    {
      findBy = findBy1.And(findBy2).And(findBy3).Or(findBy4).And(findBy5);
    }

    [Test]
    public void WithBrackets()
    {
      findBy = findBy1.And(findBy2.And(findBy3)).Or(findBy4.And(findBy5));
    }

    [Test]
    public void WithBracketsOperators1()
    {
      findBy = findBy1 & findBy2 & findBy3 | findBy4 & findBy5;
    }

    [Test]
    public void WithBracketsOperators2()
    {
      findBy = findBy1 && findBy2 && findBy3 || findBy4 && findBy5;
    }

    [TearDown]
    public void TearDown()
    {
      Expect.Call(mockAttributeBag.GetValue("1")).Return("true");
      Expect.Call(mockAttributeBag.GetValue("2")).Return("false");
      Expect.Call(mockAttributeBag.GetValue("4")).Return("true");
      Expect.Call(mockAttributeBag.GetValue("5")).Return("false");

      mocks.ReplayAll();

      Assert.IsFalse(findBy.Compare(mockAttributeBag));

      mocks.VerifyAll();
    }

//    [Test]
//    public void testAndOr()
//    {
//      Assert.IsTrue(EchoBoolean(1) && EchoBoolean(5) && EchoBoolean(3) || EchoBoolean(2) && EchoBoolean(6));
//    }
//
//    public bool EchoBoolean(int value)
//    {
//      System.Diagnostics.Debug.WriteLine(value.ToString());
//      if (value==1) return true;
//      if (value==2) return true;
//      if (value==3) return true;
//      if (value==4) return true;
//      return false;
//    }
  }

  [TestFixture]
  public class EvenMoreComplexMultipleAttributes
  {
    MockRepository mocks;
    IAttributeBag mockAttributeBag;

    Attribute findBy1;
    Attribute findBy2;
    Attribute findBy3;
    Attribute findBy4;
    Attribute findBy5;
    Attribute findBy6;
    Attribute findBy7;
    Attribute findBy8;
    Attribute findBy;

    [SetUp]
    public void Setup()
    {
      mocks = new MockRepository();
      mockAttributeBag = (IAttributeBag)mocks.CreateMock(typeof(IAttributeBag));
      findBy = null;

      findBy1 = Find.ByCustom("1", "true");
      findBy2 = Find.ByCustom("2", "true");
      findBy3 = Find.ByCustom("3", "true");
      findBy4 = Find.ByCustom("4", "true");
      findBy5 = Find.ByCustom("5", "true");
      findBy6 = Find.ByCustom("6", "true");
      findBy7 = Find.ByCustom("7", "true");
      findBy8 = Find.ByCustom("8", "true");
    }

    [Test]
    public void WithOperators()
    {
      findBy = findBy1 & findBy2 & findBy3 | findBy4 & findBy5 & findBy6 | findBy7 & findBy8;
    }

    [Test]
    public void WithoutBrackets()
    {
      findBy = findBy1.And(findBy2).And(findBy3).Or(findBy4).And(findBy5).And(findBy6).Or(findBy7).And(findBy8);
    }

    [Test]
    public void WithBrackets()
    {
      findBy = findBy1.And(findBy2.And(findBy3)).Or(findBy4.And(findBy5.And(findBy6))).Or(findBy7.And(findBy8));
    }

    [TearDown]
    public void TearDown()
    {
      Expect.Call(mockAttributeBag.GetValue("1")).Return("true");
      Expect.Call(mockAttributeBag.GetValue("2")).Return("false");
      Expect.Call(mockAttributeBag.GetValue("4")).Return("true");
      Expect.Call(mockAttributeBag.GetValue("5")).Return("false");
      Expect.Call(mockAttributeBag.GetValue("7")).Return("true");
      Expect.Call(mockAttributeBag.GetValue("8")).Return("true");

      mocks.ReplayAll();

      Assert.IsTrue(findBy.Compare(mockAttributeBag));

      mocks.VerifyAll();
    }
  }
  
  [TestFixture]
  public class ElementAttributeBagTests
  {
    MockRepository mocks;
    IHTMLStyle mockHTMLStyle;
    IHTMLElement mockHTMLElement;

    [SetUp]
    public void SetUp()
    {
      mocks = new MockRepository();
      mockHTMLStyle = (IHTMLStyle) mocks.CreateMock(typeof (IHTMLStyle));
      mockHTMLElement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));
    }

    [TearDown]
    public void TearDown()
    {
      mocks.VerifyAll();
    }

    [Test]
    public void StyleAttributeShouldReturnAsString()
    {
      const string cssText = "COLOR: white; FONT-STYLE: italic";

      Expect.Call(mockHTMLStyle.cssText).Return(cssText);
      Expect.Call(mockHTMLElement.style).Return(mockHTMLStyle);

      mocks.ReplayAll();

      ElementAttributeBag attributeBag = new ElementAttributeBag(mockHTMLElement);
      
      Assert.AreEqual(cssText, attributeBag.GetValue("style"));
    }
    
    [Test]
    public void StyleDotStyleAttributeNameShouldReturnStyleAttribute()
    {
      const string styleAttributeValue = "white";
      const string styleAttributeName = "color";

      Expect.Call(mockHTMLStyle.getAttribute(styleAttributeName,0)).Return(styleAttributeValue);
      Expect.Call(mockHTMLElement.style).Return(mockHTMLStyle);

      mocks.ReplayAll();

      ElementAttributeBag attributeBag = new ElementAttributeBag(mockHTMLElement);
      
      Assert.AreEqual(styleAttributeValue, attributeBag.GetValue("style.color"));
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

  [TestFixture]
  public class ElementFinderTests
  {
    [Test]
    public void FindFirstShoudlReturnNullIfIHTMLCollectionIsNull()
    {
      ElementFinder finder = new ElementFinder("input", "text", null);

      Assert.IsNull(finder.FindFirst());
    }

    [Test]
    public void FindAllShouldReturnEmptyArrayListIfIHTMLCollectionIsNull()
    {
      ElementFinder finder = new ElementFinder("input", "text", null);

      Assert.AreEqual(0, finder.FindAll().Count);
    }
  }

//  public class ContraintSyntaxTest
//  {
//    public void Test1()
//    {
//      IE ie = new IE();
//
//      ie.Link(Find.ByCustom("disabled", Is.True));
//      ie.Link(Find.ByCustom("disabled", Is.False));
//
//      ie.Link(Find.ByCustom("disabled", Is.GreaterThan() && Is.LessThan()));
//
//      ie.Button(Find.ByText(Text.Contains("something")));
//      ie.Button(Find.ByText(Text.StartsWith("something")));
//      ie.Button(Find.ByText(Text.EndsWith("something")));
//      ie.Button(Find.ByText(Text.Like("something")));
//
//      ie.Button(Find.ByName(Text.StartsWith("something")));
//
//    }
//  }
}
