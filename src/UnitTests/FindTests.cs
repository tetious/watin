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
using System.Text.RegularExpressions;
using NUnit.Framework;
using Rhino.Mocks;
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using Is = NUnit.Framework.SyntaxHelpers.Is;
using StringComparer = WatiN.Core.Comparers.StringComparer;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class FindTests
	{
		private const string _href = "href";
#if NET20
    private string _expectedPredicateCompareValue;

    [SetUp]
    public void SetUp()
    {
      _expectedPredicateCompareValue = null;
    }
#endif

		[Test]
		public void FindByFor()
		{
			const string htmlfor = "htmlfor";

			AttributeConstraint value = Find.ByFor("foridvalue");

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "For class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			Assert.That(value.AttributeName, Is.EqualTo(htmlfor), "Wrong attributename");
			Assert.AreEqual("foridvalue", value.Value, "Wrong value");

			Regex regex = new Regex("^id");
			value = Find.ByFor(regex);
			MockAttributeBag attributeBag = new MockAttributeBag(htmlfor, "idvalue");

			Assert.IsTrue(value.Compare(attributeBag), "Regex ^id should match");

			value = Find.ByFor(new StringContainsAndCaseInsensitiveComparer("VAl"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
      attributeBag = new MockAttributeBag(htmlfor, "forvalue");
			_expectedPredicateCompareValue = "forvalue";
			value = Find.ByFor(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

		[Test]
		public void FindByID()
		{
			AttributeConstraint value = Find.ById("idvalue");

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Id class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string id = "id";
			Assert.AreEqual(id, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("idvalue", value.Value, "Wrong value");

			MockAttributeBag attributeBag = new MockAttributeBag("id", "idvalue");
			value = Find.ById(new StringContainsAndCaseInsensitiveComparer("Val"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "idvalue";
			value = Find.ById(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

		[Test]
		public void IdWithRegexAndComparer()
		{
			MockRepository mocks = new MockRepository();
			ICompare comparer = (ICompare) mocks.CreateMock(typeof (ICompare));
			IAttributeBag attributeBag = (IAttributeBag) mocks.CreateMock(typeof (IAttributeBag));

			Expect.Call(attributeBag.GetValue("id")).Return("idvalue");

			Expect.Call(attributeBag.GetValue("id")).Return("MyMockComparer");
			Expect.Call(comparer.Compare("MyMockComparer")).Return(true);

			mocks.ReplayAll();

			BaseConstraint value = Find.ById(new Regex("lue$"));
			Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match.");

			// See if mocked comparer is used. VerifyAll will check this
			Find.ById(comparer).Compare(attributeBag);

			mocks.VerifyAll();
		}

		[Test]
		public void FindByAlt()
		{
			AttributeConstraint value = Find.ByAlt("alt text");
			
			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Alt class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string name = "alt";
			Assert.AreEqual(name, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("alt text", value.Value, "Wrong value");

			Regex regex = new Regex("ext$");
			value = Find.ByAlt(regex);
			MockAttributeBag attributeBag = new MockAttributeBag(name, "alt text");

			Assert.IsTrue(value.Compare(attributeBag), "Regex ext$ should match");

			value = Find.ByAlt(new StringContainsAndCaseInsensitiveComparer("ALT TexT"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "alt text";
			value = Find.ByAlt(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

#if NET20
		private bool TestPredicateCompareMethod(string value)
		{
			return value == _expectedPredicateCompareValue;
		}
#endif


		[Test]
		public void FindByName()
		{
			AttributeConstraint value = Find.ByName("namevalue");

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Name class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string name = "name";
			Assert.AreEqual(name, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("namevalue", value.Value, "Wrong value");

			Regex regex = new Regex("lue$");
			value = Find.ByName(regex);
			MockAttributeBag attributeBag = new MockAttributeBag(name, "namevalue");

			Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match");

			value = Find.ByName(new StringContainsAndCaseInsensitiveComparer("eVAl"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "namevalue";
			value = Find.ByName(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

		[Test]
		public void FindByText()
		{
			AttributeConstraint value = Find.ByText("textvalue");

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Text class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string innertext = "innertext";
			Assert.AreEqual(innertext, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("textvalue", value.Value, "Wrong value");

			Regex regex = new Regex("lue$");
			value = Find.ByText(regex);
			MockAttributeBag attributeBag = new MockAttributeBag(innertext, "textvalue");

			Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match");

			value = Find.ByText(new StringContainsAndCaseInsensitiveComparer("tVal"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "textvalue";
			value = Find.ByText(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

		[Test]
		public void FindByStyle()
		{
			const string attributeName = "background-color";
			AttributeConstraint value = Find.ByStyle(attributeName, "red");

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "StyleAttributeConstraint class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string fullAttributeName = "style.background-color";
			Assert.AreEqual(fullAttributeName, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("red", value.Value, "Wrong value");

			Regex regex = new Regex("een$");
			value = Find.ByStyle(attributeName, regex);
			MockAttributeBag attributeBag = new MockAttributeBag(fullAttributeName, "green");

			Assert.IsTrue(value.Compare(attributeBag), "Regex een$ should match");

			value = Find.ByStyle(attributeName, new StringContainsAndCaseInsensitiveComparer("rEe"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "green";
			value = Find.ByStyle(attributeName, TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

		[Test]
		public void FindByUrl()
		{
			string url = BaseWithIETests.WatiNURI.ToString();
			AttributeConstraint value = Find.ByUrl(url);

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Url class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(UriComparer)), "Unexpected comparer");
			AssertUrlValue(value);

			// make sure overload also works
			value = Find.ByUrl(url, true);

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Url class should inherit AttributeConstraint class");
			AssertUrlValue(value);

			MockAttributeBag attributeBag = new MockAttributeBag("href", url);
			value = Find.ByUrl(new StringContainsAndCaseInsensitiveComparer("/watin.Sour"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = url;
			value = Find.ByUrl(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

		[Test, ExpectedException(typeof (UriFormatException))]
		public void FindingEmptyUrlNotAllowed()
		{
			Find.ByUrl(String.Empty);
		}

		[Test]
		public void FindByUri()
		{
			AttributeConstraint value = Find.ByUrl(BaseWithIETests.WatiNURI);
			AssertUrlValue(value);

			// make sure the ignore querystring constructer also works.
			value = Find.ByUrl(BaseWithIETests.WatiNURI, true);
			AssertUrlValue(value);
		}

		private static void AssertUrlValue(AttributeConstraint value)
		{
			Assert.AreEqual(_href, value.AttributeName, "Wrong attributename");
			Assert.AreEqual(BaseWithIETests.WatiNURI.ToString(), value.Value, "Wrong value");

			MockAttributeBag attributeBag = new MockAttributeBag(_href, BaseWithIETests.WatiNURI.ToString());

			Assert.IsTrue(value.Compare(attributeBag), "Should match WatiN url");

			attributeBag = new MockAttributeBag(_href, "http://www.microsoft.com");
			Assert.IsFalse(value.Compare(attributeBag), "Shouldn't match Microsoft");

			attributeBag = new MockAttributeBag(_href, null);
			Assert.IsFalse(value.Compare(attributeBag), "Null should not match");

			attributeBag = new MockAttributeBag(_href, String.Empty);
			Assert.IsFalse(value.Compare(attributeBag), "Null should not match");
		}

		[Test]
		public void FindByUrlWithRegex()
		{
			Regex regex = new Regex("^http://watin");
			BaseConstraint value = Find.ByUrl(regex);
			MockAttributeBag attributeBag = new MockAttributeBag(_href, "http://watin.sourceforge.net");

			Assert.IsTrue(value.Compare(attributeBag), "Regex ^http://watin should match");
		}

		[Test, ExpectedException(typeof (UriFormatException))]
		public void FindByUrlInvalidParam()
		{
			Find.ByUrl("www.xyz.nl");
		}

		[Test, ExpectedException(typeof (UriFormatException))]
		public void FindByUrlInvalidCompare()
		{
			BaseConstraint value = Find.ByUrl(BaseWithIETests.WatiNURI.ToString());
			MockAttributeBag attributeBag = new MockAttributeBag(_href, "watin.sourceforge.net");

			value.Compare(attributeBag);
		}

		[Test]
		public void FindByTitle()
		{
			const string title = "title";

			AttributeConstraint value = Find.ByTitle("titlevalue");

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Title class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringContainsAndCaseInsensitiveComparer)), "Unexpected comparer");

			Assert.AreEqual(title, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("titlevalue", value.Value, "Wrong value");


			MockAttributeBag attributeBag = new MockAttributeBag(title, String.Empty);
			Assert.IsFalse(value.Compare(attributeBag), "Empty should not match");

			attributeBag = new MockAttributeBag(title, null);
			Assert.IsFalse(value.Compare(attributeBag), "Null should not match");

			attributeBag = new MockAttributeBag(title, "titlevalue");

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
			attributeBag = new MockAttributeBag(title, "title");

			Assert.IsFalse(value.Compare(attributeBag), "Compare should not match title");

			attributeBag = new MockAttributeBag(title, "title");
			value = Find.ByTitle(new StringContainsAndCaseInsensitiveComparer("iTl"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "title";
			value = Find.ByTitle(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

		[Test]
		public void FindByValue()
		{
			const string valueAttrib = "value";

			AttributeConstraint value = Find.ByValue("valuevalue");

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Value class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			Assert.AreEqual(valueAttrib, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("valuevalue", value.Value, "Wrong value");
			Assert.AreEqual("valuevalue", value.ToString(), "Wrong ToString result");

			Regex regex = new Regex("lue$");
			value = Find.ByValue(regex);
			MockAttributeBag attributeBag = new MockAttributeBag(valueAttrib, "valuevalue");

			Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match");

			value = Find.ByValue(new StringContainsAndCaseInsensitiveComparer("eVal"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "valuevalue";
			value = Find.ByValue(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

		[Test]
		public void FindBySrc()
		{
			const string src = "src";

			AttributeConstraint value = Find.BySrc("image.gif");

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Src class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			Assert.AreEqual(src, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("image.gif", value.Value, "Wrong value");

			MockAttributeBag attributeBag = new MockAttributeBag(src, "/images/image.gif");
			Assert.IsFalse(value.Compare(attributeBag), "Should not match /images/image.gif");

			attributeBag = new MockAttributeBag(src, "image.gif");
			Assert.IsTrue(value.Compare(attributeBag), "Should match image.gif");

			Regex regex = new Regex("image.gif$");
			value = Find.BySrc(regex);
			attributeBag = new MockAttributeBag(src, "/images/image.gif");

			Assert.IsTrue(value.Compare(attributeBag), "Regex image.gif$ should match");

			value = Find.BySrc(new StringContainsAndCaseInsensitiveComparer("es/Im"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "/images/image.gif";
			value = Find.BySrc(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}
		
		[Test]
		public void FindByElement()
		{
			ElementComparerMock comparer = new ElementComparerMock();
			ElementConstraint constraint = Find.ByElement(comparer);
			
			Assert.That(constraint.Comparer, Is.InstanceOfType(typeof(ElementComparerMock)));

#if NET20
			constraint = Find.ByElement(CallThisPredicate);

			Assert.That(constraint.Comparer, Is.InstanceOfType(typeof(PredicateComparer)));
#endif
		}

#if NET20
		private bool CallThisPredicate(Element element)
		{
			return true;
		}
#endif

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void NewAttributeWithNullAttribute()
		{
			new AttributeConstraint(null, "idvalue");
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void NewAttributeWithNullValue()
		{
			new AttributeConstraint("id", (string) null);
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void NewAttributeWithNulls()
		{
			new AttributeConstraint(null, (string) null);
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void NewAttributeWithNullRegex()
		{
			new AttributeConstraint("id", (Regex) null);
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void NewAttributeWithNullsRegex()
		{
			new AttributeConstraint(null, (Regex) null);
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void NewAttributeWithEmptyAttribute()
		{
			new AttributeConstraint(string.Empty, "idvalue");
		}

		[Test]
		public void NewAttributeWithEmptyValue()
		{
			AttributeConstraint attribute = new AttributeConstraint("id", string.Empty);
			Assert.IsEmpty(attribute.Value);
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void NewAttributeWithEmpties()
		{
			new AttributeConstraint(string.Empty, string.Empty);
		}

		[Test]
		public void FindBy()
		{
			const string id = "id";
			AttributeConstraint value = Find.By(id, "idvalue");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			Assert.AreEqual(id, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("idvalue", value.Value, "Wrong value");

			MockAttributeBag attributeBag = new MockAttributeBag(id, "idvalue");
			Assert.IsTrue(value.Compare(attributeBag), "Compare should match");

			attributeBag = new MockAttributeBag(id, "id");
			Assert.IsFalse(value.Compare(attributeBag), "Compare should not partial match id");

			attributeBag = new MockAttributeBag(id, "val");
			Assert.IsFalse(value.Compare(attributeBag), "Compare should not partial match val");

			attributeBag = new MockAttributeBag(id, "value");
			Assert.IsFalse(value.Compare(attributeBag), "Compare should not partial match value");

			Regex regex = new Regex("lue$");
			value = Find.By(id, regex);
			attributeBag = new MockAttributeBag(id, "idvalue");

			Assert.IsTrue(value.Compare(attributeBag), "Regex lue$ should match");

			value = Find.By(id, new StringContainsAndCaseInsensitiveComparer("dVal"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "idvalue";
			value = Find.By(id, TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}

		[Test]
		public void FindByClass()
		{
			AttributeConstraint value = Find.ByClass("highlighted");

			Assert.IsInstanceOfType(typeof (BaseConstraint), value, "Find.ByClass should return an AttributeConstraint");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string classname = "classname";
			Assert.AreEqual(classname, value.AttributeName, "Wrong attributename");
			Assert.AreEqual("highlighted", value.Value, "Wrong value");

			Regex regex = new Regex("ghted$");
			value = Find.ByClass(regex);
			MockAttributeBag attributeBag = new MockAttributeBag(classname, "highlighted");

			Assert.IsTrue(value.Compare(attributeBag), "Regex ghted$ should match");

			value = Find.ByClass(new StringContainsAndCaseInsensitiveComparer("hLIg"));
			Assert.That(value.Compare(attributeBag), Is.True, "Comparer not used");

#if NET20
			_expectedPredicateCompareValue = "highlighted";
			value = Find.ByClass(TestPredicateCompareMethod);
			Assert.That(value.Compare(attributeBag), Is.True, "PredicateComparer not used");
#endif
		}
		
		public class ElementComparerMock : ICompareElement
		{
			public bool IsCalled = false;
			
			public bool Compare(Element element)
			{
				IsCalled = true;
				return true;
			}
		}
		
	}
}
