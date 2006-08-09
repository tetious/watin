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
      Assert.AreEqual("text", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("textvalue", value.Value, "Wrong value");
      
      Regex regex = new Regex("lue$");
      value = Find.ByText(regex);
      Assert.IsTrue(value.Compare("textvalue"), "Regex lue$ should match");
    }

    [Test]
    public void FindByUrl()
    {
      Url value = Find.ByUrl("http://www.google.com");
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
      Url value = new Url(GoogleURI);
      AssertUrlValue(value);
    }
    
    private static void AssertUrlValue(Url value)
    {
      Assert.AreEqual("href", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("http://www.google.com/", value.Value, "Wrong value");
      Assert.IsTrue(value.Compare("http://www.google.com/"), "Should match Google url");
      Assert.IsTrue(value.Compare(GoogleURI), "Should match Google uri");
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
      Find.ByUrl("www.google.nl");
    }

    [Test, ExpectedException(typeof(UriFormatException))]
    public void FindByUrlInvalidCompare()
    {
      Url value = Find.ByUrl("http://www.google.com");
      value.Compare("google.com");
    }

    [Test]
    public void FindByTitle()
    {
      Title value = Find.ByTitle("titlevalue");
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
      Assert.AreEqual("src", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("image.gif", value.Value, "Wrong value");
      
      Assert.IsFalse(value.Compare("/images/image.gif"), "Should not match /images/image.gif");
      Assert.IsTrue(value.Compare("image.gif"), "Should match image.gif");
      
      Regex regex = new Regex("image.gif$");
      value = Find.BySrc(regex);
      Assert.IsTrue(value.Compare("/images/image.gif"), "Regex image.gif$ should match");
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewElementAttributeWithNullAttribute()
    {
      Find.ByCustom(null,"idvalue");
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewElementAttributeWithNullValue()
    {
      new Attribute("id",(string)null);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewElementAttributeWithNulls()
    {
      new Attribute(null,(string)null);
    }
    
    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewElementAttributeWithNullRegex()
    {
      new Attribute("id",(Regex)null);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewElementAttributeWithNullsRegex()
    {
      new Attribute(null,(Regex)null);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewElementAttributeWithEmptyAttribute()
    {
      Find.ByCustom(string.Empty,"idvalue");
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewElementAttributeWithEmptyValue()
    {
      Find.ByCustom("id",string.Empty);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void NewElementAttributeWithEmpties()
    {
      Find.ByCustom(string.Empty,string.Empty);
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
      ElementUrlPartialValue value = new ElementUrlPartialValue("google.com");
      Assert.AreEqual("href", value.AttributeName, "Wrong attributename");
      Assert.AreEqual("google.com", value.Value, "Wrong value");
      
      Assert.IsTrue(value.Compare("google.com"), "Compare should match");
      
      value = new ElementUrlPartialValue("google.com");
      Assert.IsTrue(value.Compare("www.google.com"), "Compare should partial match tit");
      
      value = new ElementUrlPartialValue("google.com");
      Assert.IsFalse(value.Compare("www.microsoft.com"), "Compare should not match title");

      using (IE ie = new IE(MainURI.ToString()))
      {
        ie.Link("testlinkid").Click();
        IE ieGoogle = IE.AttachToIE(new ElementUrlPartialValue("google.com"));
        Assert.AreEqual(GoogleURI.ToString(), ieGoogle.Url);
        ieGoogle.Close();
      }
    }
  }

  public class ElementUrlPartialValue : Url
  {
    private string url = null;

    public ElementUrlPartialValue(string url) : base("http://www.fakeurl.com")
    {
      this.url = url;  
    }

    public override string Value
    {
       get { return url; }
    }

    public override bool Compare(string value)
    {
      bool containedInValue = value.ToLower().IndexOf(Value.ToLower()) >= 0;

      if (UtilityClass.IsNotNullOrEmpty(value) && containedInValue)
      {
        return true;
      }
      return false;
    }
  }
}