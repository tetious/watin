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
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using WatiN.Core.UnitTests.TestUtils;
using StringComparer = WatiN.Core.Comparers.StringComparer;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class FindTests : BaseWithBrowserTests
	{
		private const string _href = "href";
	    private string _expectedPredicateCompareValue;

	    [SetUp]
		public void SetUp()
		{
            Settings.Reset();
			_expectedPredicateCompareValue = null;
		}

	    public override Uri TestPageUri
	    {
            get { return ProximityURI; }
	    }


        [Test]
		public void FindByFor()
		{
			const string htmlfor = "htmlFor";

			var value = Find.ByFor("foridvalue");

			Assert.IsInstanceOfType(typeof (Constraint), value, "For class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			Assert.That(value.AttributeName, Is.EqualTo(htmlfor), "Wrong attributename");
			Assert.That(value.Comparer.ToString(), Text.Contains("'foridvalue'"), "Wrong value");

			var regex = new Regex("^id");
			value = Find.ByFor(regex);
			var mockAttributeBag = new MockAttributeBag(htmlfor, "idvalue");

            var context = new ConstraintContext();
            Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex ^id should match");

			value = Find.ByFor(new StringContainsAndCaseInsensitiveComparer("VAl"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            mockAttributeBag = new MockAttributeBag(htmlfor, "forvalue");
			_expectedPredicateCompareValue = "forvalue";
			value = Find.ByFor(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
        }

		[Test]
		public void FindByID()
		{
			var value = Find.ById("idvalue");

			Assert.IsInstanceOfType(typeof (Constraint), value, "Id class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string id = "id";
			Assert.AreEqual(id, value.AttributeName, "Wrong attributename");
			Assert.That(value.Comparer.ToString(), Text.Contains("'idvalue'"), "Wrong value");

			var mockAttributeBag = new MockAttributeBag("id", "idvalue");
            var context = new ConstraintContext();
            value = Find.ById(new StringContainsAndCaseInsensitiveComparer("Val"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "idvalue";
			value = Find.ById(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}

		[Test]
		public void IdWithRegexAndComparer()
		{
            var context = new ConstraintContext();
            var mockAttributeBag = new MockAttributeBag("id", "idvalue");

			Constraint value = Find.ById(new Regex("lue$"));
			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex lue$ should match.");

			// See if mocked comparer is used. VerifyAll will check this
            var comparerMock = new Mock<Comparer<string>>();
            comparerMock.Expect(comparer => comparer.Compare("MyMockComparer")).Returns(true);

            mockAttributeBag = new MockAttributeBag("id", "MyMockComparer");
            Find.ById(comparerMock.Object).Matches(mockAttributeBag, context);

			comparerMock.VerifyAll();
		}

		[Test]
		public void FindByAlt()
		{
			var value = Find.ByAlt("alt text");
			
			Assert.IsInstanceOfType(typeof (Constraint), value, "Alt class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string name = "alt";
			Assert.AreEqual(name, value.AttributeName, "Wrong attributename");
            Assert.That(value.Comparer.ToString(), Text.Contains("'alt text'"), "Wrong value");

			var regex = new Regex("ext$");
			value = Find.ByAlt(regex);
			var mockAttributeBag = new MockAttributeBag(name, "alt text");
            var context = new ConstraintContext();

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex ext$ should match");

			value = Find.ByAlt(new StringContainsAndCaseInsensitiveComparer("ALT TexT"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "alt text";
			value = Find.ByAlt(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
        }

        private bool TestPredicateCompareMethod(string value)
		{
			return value == _expectedPredicateCompareValue;
		}

		[Test]
		public void FindByName()
		{
			var value = Find.ByName("namevalue");

			Assert.IsInstanceOfType(typeof (Constraint), value, "Name class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string name = "name";
			Assert.AreEqual(name, value.AttributeName, "Wrong attributename");
            Assert.That(value.Comparer.ToString(), Text.Contains("'namevalue'"), "Wrong value");

			var regex = new Regex("lue$");
			value = Find.ByName(regex);
			var mockAttributeBag = new MockAttributeBag(name, "namevalue");
            var context = new ConstraintContext();

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex lue$ should match");

			value = Find.ByName(new StringContainsAndCaseInsensitiveComparer("eVAl"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "namevalue";
			value = Find.ByName(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}

		[Test]
		public void FindByText()
		{
			var value = Find.ByText("textvalue");

			Assert.IsInstanceOfType(typeof (Constraint), value, "Text class should inherit Attribute class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(RegexComparer)), "Unexpected comparer");

			const string innertext = "innertext";
			Assert.AreEqual(innertext, value.AttributeName, "Wrong attributename");
		    Assert.That(value.Comparer.ToString(), Text.Contains("'^ *textvalue *$'"), "Wrong value");

			var regex = new Regex("lue$");
			value = Find.ByText(regex);
			var mockAttributeBag = new MockAttributeBag(innertext, "textvalue");
            var context = new ConstraintContext();

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex lue$ should match");

			value = Find.ByText(new StringContainsAndCaseInsensitiveComparer("tVal"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "textvalue";
			value = Find.ByText(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}


        [Test]
        public void FindByTextOnTextShouldIgnoreSpacesBeforeOrAfterText()
        {
            const string innertext = "innertext";

            var value = Find.ByText("textvalue");
            var mockAttributeBag = new MockAttributeBag(innertext, "textvalue");
            var context = new ConstraintContext();
            Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Exact match expected");

            mockAttributeBag = new MockAttributeBag(innertext, " textvalue");
            Assert.That(value.Matches(mockAttributeBag, context), Is.True, "1 Space before should match");
            
            mockAttributeBag = new MockAttributeBag(innertext, "  textvalue");
            Assert.That(value.Matches(mockAttributeBag, context), Is.True, "2 Spaces before should match");
            
            mockAttributeBag = new MockAttributeBag(innertext, "textvalue ");
            Assert.That(value.Matches(mockAttributeBag, context), Is.True, "1 Spaces after should match");
            
            mockAttributeBag = new MockAttributeBag(innertext, "textvalue  ");
            Assert.That(value.Matches(mockAttributeBag, context), Is.True, "2 Spaces after should match");
            
            mockAttributeBag = new MockAttributeBag(innertext, " textvalue ");
            Assert.That(value.Matches(mockAttributeBag, context), Is.True, "1 Space before and 1 after should match");

            mockAttributeBag = new MockAttributeBag(innertext, "a textvalue");
            Assert.That(value.Matches(mockAttributeBag, context), Is.False, "should only match with only spaces before searched text");
            
            mockAttributeBag = new MockAttributeBag(innertext, "textvalue z");
            Assert.That(value.Matches(mockAttributeBag, context), Is.False, "should only match with only spaces after searched text");
            
            mockAttributeBag = new MockAttributeBag(innertext, "a textvalue z");
            Assert.That(value.Matches(mockAttributeBag, context), Is.False, "should not match");
        }

	    [Test]
		public void FindByStyle()
		{
			const string attributeName = "background-color";
			var value = Find.ByStyle(attributeName, "red");

			Assert.IsInstanceOfType(typeof (Constraint), value, "StyleAttributeConstraint class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string fullAttributeName = "style.background-color";
			Assert.AreEqual(fullAttributeName, value.AttributeName, "Wrong attributename");
	        Assert.That(value.Comparer.ToString(), Text.Contains("'red'"), "Wrong value");

			var regex = new Regex("een$");
			value = Find.ByStyle(attributeName, regex);
			var mockAttributeBag = new MockAttributeBag(fullAttributeName, "green");
            var context = new ConstraintContext();

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex een$ should match");

			value = Find.ByStyle(attributeName, new StringContainsAndCaseInsensitiveComparer("rEe"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "green";
			value = Find.ByStyle(attributeName, TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}

		[Test]
		public void FindByUrl()
		{
            var url = WatiNURI.AbsoluteUri;
			var value = Find.ByUrl(url);

			Assert.IsInstanceOfType(typeof (Constraint), value, "Url class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(UriComparer)), "Unexpected comparer");
			AssertUrlValue(value);

			// make sure overload also works
			value = Find.ByUrl(url, true);

			Assert.IsInstanceOfType(typeof (Constraint), value, "Url class should inherit AttributeConstraint class");
			AssertUrlValue(value);

			var mockAttributeBag = new MockAttributeBag("href", url);
            var context = new ConstraintContext();

			value = Find.ByUrl(new StringContainsAndCaseInsensitiveComparer("/watin.Sour"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = url;
			value = Find.ByUrl(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}

		[Test, ExpectedException(typeof (UriFormatException))]
		public void FindingEmptyUrlNotAllowed()
		{
			Find.ByUrl(String.Empty);
		}

		[Test]
		public void FindByUri()
		{
			var value = Find.ByUrl(WatiNURI);
			AssertUrlValue(value);

			// make sure the ignore querystring constructer also works.
			value = Find.ByUrl(WatiNURI, true);
			AssertUrlValue(value);
		}

		private static void AssertUrlValue(AttributeConstraint value)
		{
			Assert.AreEqual(_href, value.AttributeName, "Wrong attributename");
			Assert.That(value.Comparer.ToString(),Text.Contains("'" + WatiNURI.AbsoluteUri + "'"), "Wrong value");

            var mockAttributeBag = new MockAttributeBag(_href, WatiNURI.AbsoluteUri);
            var context = new ConstraintContext();

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Should match WatiN url");

			mockAttributeBag = new MockAttributeBag(_href, "http://www.microsoft.com");
			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Shouldn't match Microsoft");

			mockAttributeBag = new MockAttributeBag(_href, null);
			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Null should not match");

			mockAttributeBag = new MockAttributeBag(_href, String.Empty);
			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Null should not match");
		}

		[Test]
		public void FindByUrlWithRegex()
		{
			var regex = new Regex("^http://watin");
			var value = Find.ByUrl(regex);
			var mockAttributeBag = new MockAttributeBag(_href, "http://watin.sourceforge.net");
            var context = new ConstraintContext();

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex ^http://watin should match");
		}

		[Test, ExpectedException(typeof (UriFormatException))]
		public void FindByUrlInvalidParam()
		{
			Find.ByUrl("www.xyz.nl");
		}

		[Test]
		public void FindByTitle()
		{
			const string title = "title";

			var value = Find.ByTitle("titlevalue");

			Assert.IsInstanceOfType(typeof (Constraint), value, "Title class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringContainsAndCaseInsensitiveComparer)), "Unexpected comparer");

			Assert.AreEqual(title, value.AttributeName, "Wrong attributename");
			Assert.That(value.Comparer.ToString(),Text.Contains("'titlevalue'"), "Wrong value");


			var mockAttributeBag = new MockAttributeBag(title, String.Empty);
            var context = new ConstraintContext();

			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Empty should not match");

			mockAttributeBag = new MockAttributeBag(title, null);
			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Null should not match");

			mockAttributeBag = new MockAttributeBag(title, "titlevalue");

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Compare should match");

			value = Find.ByTitle("titl");
			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Compare should partial match titl");

			value = Find.ByTitle("tItL");
			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Compare should partial match tItL");

			value = Find.ByTitle("lev");
			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Compare should partial match lev");

			value = Find.ByTitle("alue");
			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Compare should partial match alue");

			var regex = new Regex("^titl");
			value = Find.ByTitle(regex);
			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex ^titl should match");

			value = Find.ByTitle("titlevalue");
			mockAttributeBag = new MockAttributeBag(title, "title");

			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Compare should not match title");

			mockAttributeBag = new MockAttributeBag(title, "title");
			value = Find.ByTitle(new StringContainsAndCaseInsensitiveComparer("iTl"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "title";
			value = Find.ByTitle(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}

		[Test]
		public void FindByValue()
		{
			const string valueAttrib = "value";

			var value = Find.ByValue("valuevalue");

			Assert.IsInstanceOfType(typeof (Constraint), value, "Value class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			Assert.AreEqual(valueAttrib, value.AttributeName, "Wrong attributename");
			Assert.That(value.Comparer.ToString(),Text.Contains("'valuevalue'"), "Wrong value");

			var regex = new Regex("lue$");
			value = Find.ByValue(regex);

			var mockAttributeBag = new MockAttributeBag(valueAttrib, "valuevalue");
            var context = new ConstraintContext();

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex lue$ should match");

			value = Find.ByValue(new StringContainsAndCaseInsensitiveComparer("eVal"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "valuevalue";
			value = Find.ByValue(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}

		[Test]
		public void FindBySrc()
		{
			const string src = "src";

			var value = Find.BySrc("image.gif");

			Assert.IsInstanceOfType(typeof (Constraint), value, "Src class should inherit AttributeConstraint class");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			Assert.AreEqual(src, value.AttributeName, "Wrong attributename");
            Assert.That(value.Comparer.ToString(),Text.Contains("'image.gif'"), "Wrong value");

			var mockAttributeBag = new MockAttributeBag(src, "/images/image.gif");
            var context = new ConstraintContext();

			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Should not match /images/image.gif");

			mockAttributeBag = new MockAttributeBag(src, "image.gif");
			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Should match image.gif");

			var regex = new Regex("image.gif$");
			value = Find.BySrc(regex);
			mockAttributeBag = new MockAttributeBag(src, "/images/image.gif");

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex image.gif$ should match");

			value = Find.BySrc(new StringContainsAndCaseInsensitiveComparer("es/Im"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "/images/image.gif";
			value = Find.BySrc(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}
		
		[Test]
		public void FindByElement()
		{
			var comparer = new ElementComparerMock();
			var constraint = Find.ByElement(comparer);
			
			Assert.That(constraint.Comparer, Is.InstanceOfType(typeof(ElementComparerMock)));

            constraint = Find.ByElement(CallThisPredicate);

			Assert.That(constraint.Comparer, Is.InstanceOfType(typeof(PredicateComparer<Element, Element>)));
        }

        private static bool CallThisPredicate(Element element)
		{
			return true;
		}

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
			var attribute = new AttributeConstraint("id", string.Empty);
            Assert.That(attribute.Comparer.ToString(), Text.Contains("''"));
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
			var value = Find.By(id, "idvalue");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			Assert.AreEqual(id, value.AttributeName, "Wrong attributename");
            Assert.That(value.Comparer.ToString(), Text.Contains("'idvalue'") , "Wrong value");

			var mockAttributeBag = new MockAttributeBag(id, "idvalue");
            var context = new ConstraintContext();

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Compare should match");

			mockAttributeBag = new MockAttributeBag(id, "id");
			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Compare should not partial match id");

			mockAttributeBag = new MockAttributeBag(id, "val");
			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Compare should not partial match val");

			mockAttributeBag = new MockAttributeBag(id, "value");
			Assert.IsFalse(value.Matches(mockAttributeBag, context), "Compare should not partial match value");

			var regex = new Regex("lue$");
			value = Find.By(id, regex);
			mockAttributeBag = new MockAttributeBag(id, "idvalue");

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex lue$ should match");

			value = Find.By(id, new StringContainsAndCaseInsensitiveComparer("dVal"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "idvalue";
			value = Find.By(id, TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}

		[Test]
		public void FindByClass()
		{
			var value = Find.ByClass("highlighted");

			Assert.IsInstanceOfType(typeof (Constraint), value, "Find.ByClass should return an AttributeConstraint");
			Assert.That(value.Comparer,  Is.TypeOf(typeof(StringComparer)), "Unexpected comparer");

			const string classname = "className";
			Assert.AreEqual(classname, value.AttributeName, "Wrong attributename");
            Assert.That(value.Comparer.ToString(), Text.Contains("'highlighted'"), "Wrong value");

			var regex = new Regex("ghted$");
			value = Find.ByClass(regex);

			var mockAttributeBag = new MockAttributeBag(classname, "highlighted");
            var context = new ConstraintContext();

			Assert.IsTrue(value.Matches(mockAttributeBag, context), "Regex ghted$ should match");

			value = Find.ByClass(new StringContainsAndCaseInsensitiveComparer("hLIg"));
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "Comparer not used");

            _expectedPredicateCompareValue = "highlighted";
			value = Find.ByClass(TestPredicateCompareMethod);
			Assert.That(value.Matches(mockAttributeBag, context), Is.True, "PredicateComparer not used");
		}

        [Test]
        public void FindFirst()
        {
            Assert.That(Find.First(), Is.TypeOf(typeof(IndexConstraint)));
            Assert.That(Find.First().ToString(), Is.EqualTo("Index = 0"));
        }
		
		[Test]
		public void ShouldFindFormElementsByNearbyText() 
		{
		    ExecuteTest(browser =>
		                    {
                                var inputUsername = browser.TextField(Find.Near("User Name"));
		                        Assert.AreEqual("inputUserName", inputUsername.Id, "Left/right proximity for text did not find 'User Name' field.");

                                var inputPassword = browser.TextField(Find.Near("Password"));
		                        Assert.AreEqual("inputPassword", inputPassword.Id, "Left/right proximity for text did not find 'Password' field.");
		
		                        // Test with a constraint
                                var inputUsername2 = browser.TextField(new ProximityTextConstraint("User Name"));
		                        Assert.AreEqual(inputUsername.Id, inputUsername2.Id, "Find.Near and ProximityTextConstraint did not find same element.");
		                    });
		}
		
		[Test]
		public void ShouldFindFormElementsByLabelText() 
		{
		    ExecuteTest(browser =>
		                    {
                                // GIVEN
                                browser.GoTo(MainURI);
			
		                        // The control to test against
                                var checkBox21a = browser.CheckBox("Checkbox21");
                                Assert.That(checkBox21a.Exists, Is.True, "Checkbox21 missing.");
			
		                        // Test with Find.ByLabelText
                                var checkBox21b = browser.CheckBox(Find.ByLabelText("label for Checkbox21"));
		                        Assert.AreEqual(checkBox21a.Id, checkBox21b.Id, "Checkbox attached to Label for Checkbox21 did not match CheckBox21.");
			
		                        // Test with a constraint
                                var checkBox21c = browser.CheckBox(new LabelTextConstraint("label for Checkbox21"));
		                        Assert.AreEqual(checkBox21b.Id, checkBox21c.Id, "Using a LabelTextContraint did not return the same as Find.ByLabelText for Checkbox21.");

		                    });
		}
		
        [Test]
        public void FindByDefaultStringShouldReturnDefaultFromTheSetDefaultFindFactory()
        {
            // GIVEN
            Settings.FindByDefaultFactory = new MyTestDefaultFindFactory();

            // WHEN
            var byDefault = (AttributeConstraint) Find.ByDefault("testValue");

            // THEN
            Assert.That(byDefault.AttributeName, Is.EqualTo(MyTestDefaultFindFactory.TEST_ATTRIBUTE));
        }

        [Test]
        public void FindByDefaultRegexShouldReturnDefaultFromTheSetDefaultFindFactory()
        {
            // GIVEN
            Settings.FindByDefaultFactory = new MyTestDefaultFindFactory();

            // WHEN
            var byDefault = (AttributeConstraint) Find.ByDefault(new Regex("testValue"));

            // THEN
            Assert.That(byDefault.AttributeName, Is.EqualTo(MyTestDefaultFindFactory.TEST_ATTRIBUTE));
        }

	    public class MyTestDefaultFindFactory : IFindByDefaultFactory
	    {
	        public const string TEST_ATTRIBUTE = "testAttribute";
            
            public Constraint ByDefault(string value)
	        {
	            return Find.By(TEST_ATTRIBUTE, value);
	        }

	        public Constraint ByDefault(Regex value)
	        {
                return Find.By(TEST_ATTRIBUTE, value);
            }
	    }

	    public class ElementComparerMock : Comparer<Element>
		{
			public bool IsCalled;
			
			public override bool Compare(Element element)
			{
				IsCalled = true;
				return true;
			}
		}
	}
}
