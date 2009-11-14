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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class PageTests : BaseWithBrowserTests
    {
        [Test, Ignore("SF Bug 2866821: TODO for WatiN 2.1")]
        // https://sourceforge.net/tracker/?func=detail&aid=2866821&group_id=167632&atid=843727
        public void Should_fail_element_lookup_by_attribute_cause_we_are_on_the_wrong_page()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                browser.GoTo(AboutBlank);
                var page = browser.Page<MainPage>();

                // WHEN
                try
                {
                    var member = page.NameTextFieldUsingFindbyAttribute.Text;
                    Assert.Fail("Expected " + typeof(PageVerificationException).ToString());
                }
                catch (PageVerificationException)
                {
                    // OK
                }
                catch(Exception e)
                {
                    Assert.Fail("Unexpected exception: " + e.ToString());
                }

                // THEN expected exception
            });
        }

        [Test, ExpectedException(ExceptionType = typeof(PageVerificationException))]
        public void Should_fail_element_lookup_by_code_cause_we_are_on_the_wrong_page()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                browser.GoTo(AboutBlank);
                var page = browser.Page<MainPage>();

                // WHEN
                var member = page.NameTextFieldUsingCode;

                // THEN expected exception
            });
        }

        [Test]
        public void ShouldInitializeElementField()
        {
            ExecuteTestWithAnyBrowser(browser =>
                {
                    // GIVEN
                    var page = browser.Page<MainPage>();

                    // WHEN
                    var member = page.NameTextFieldUsingFindbyAttribute;

                    // THEN
                    Assert.That(member, Is.Not.Null);
                    Assert.That(member.Description, Is.Null);
                    Assert.That(member.Name, Is.EqualTo("textinput1"));
                });
        }

        [Test]
        public void ShouldInitializeElementPropertyWithDescription()
        {
            ExecuteTestWithAnyBrowser(browser =>
                {
                    // GIVEN
                    var page = browser.Page<MainPage>();

                    // WHEN
                    var member = page.PopUpButton;

                    // THEN
                    Assert.That(member, Is.Not.Null);
                    Assert.That(member.Description, Is.EqualTo("Popup button."));
                    Assert.That(member.Id, Is.EqualTo("popupid"));
                });
        }

        [Test]
        public void ShouldInitializeControlFieldWithDescription()
        {
            ExecuteTestWithAnyBrowser(browser =>
                {
                    // GIVEN
                    var page = browser.Page<MainPage>();

                    // WHEN
                    var member = page.NameTextFieldControl;

                    // THEN
                    Assert.That(member, Is.Not.Null);
                    Assert.That(member.Description, Is.EqualTo("Text field control."));
                    Assert.That(member.Id, Is.EqualTo("name"));
                });
        }

        [Test]
        public void ToStringWhenDescriptionIsNotSetShouldDescribePage()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                var page = browser.Page<MainPage>();

                // WHEN
                var description = page.Description;
                var toString = page.ToString();

                // THEN
                Assert.That(description, Is.Null);
                Assert.That(Regex.IsMatch(toString, @"MainPage \(file://.*\)"));
            });
        }

        [Test]
        public void ToStringWhenDescriptionIsSetShouldReturnDescription()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
#if !NET20
                var page = browser.Page<MainPage>().WithDescription("foo");
#else
                var page = browser.Page<MainPage>();
                page.Description = "foo";
#endif

                // WHEN
                var description = page.Description;
                var toString = page.ToString();

                // THEN
                Assert.That(description, Is.EqualTo("foo"));
                Assert.That(toString, Is.EqualTo("foo"));
            });
        }

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        public class TextFieldControl : Control<TextField>
        {
            public string Text
            {
                get { return Element.Value; }
            }

            public string Id
            {
                get { return Element.Id; }
            }
        }

        [Page(UrlRegex = "main.html$")]
        public class MainPage : Page
        {
            [FindBy(Name = "textinput1")]
            public TextField NameTextFieldUsingFindbyAttribute;
            
            public TextField NameTextFieldUsingCode
            {
                get { return Document.TextField(Find.ByName("textinput1"));}
            }

            [FindBy(Id = "popupid")]
            [Description("Popup button.")]
            public Button PopUpButton { get; set; }

            [FindBy(Name = "textinput1")]
            [Description("Text field control.")]
            internal TextFieldControl NameTextFieldControl = null; // intentionally non-public
        }
    }

    [TestFixture]
    public class GoogleTests
    {
        [Test, Ignore("SF Bug 2897406: TODO for WatiN 2.1")]
        // https://sourceforge.net/tracker/?func=detail&aid=2897406&group_id=167632&atid=843727
        public void Search_for_watin_on_google_using_page_class()
        {
            using (var browser = new IE("http://www.google.com"))
            {
                var searchPage = browser.Page<GoogleSearchPage>();
                searchPage.SearchCriteria.TypeText("WatiN");
                searchPage.SearchButton.Click();

                Assert.IsTrue(browser.ContainsText("WatiN"));
                browser.Back();

                //This line throws UnauthorizedAccessException.
                searchPage.SearchCriteria.TypeText("Search Again");
                searchPage.SearchButton.Click();
                Assert.IsTrue(browser.ContainsText("Glenn"));

            }
        }

        [Page(UrlRegex = "www.google.*")]
        public class GoogleSearchPage : Page
        {
            [FindBy(Name = "q")]
            public TextField SearchCriteria;

            [FindBy(Name = "btnG")]
            public Button SearchButton;

            public void SearchFor(string searchCriteria)
            {
                SearchCriteria.TypeText("WatiN");
                SearchButton.Click();
            }
        }
    }

}
