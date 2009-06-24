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

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class PageTests : BaseWithBrowserTests
    {
        [Test]
        public void ShouldInitializeElementField()
        {
            ExecuteTestWithAnyBrowser(browser =>
                {
                    // GIVEN
                    var page = browser.Page<MainPage>();

                    // WHEN
                    var member = page.NameTextField;

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

        public class MainPage : Page
        {
            [FindBy(Name = "textinput1")]
            public TextField NameTextField;

            [FindBy(Id = "popupid")]
            [Description("Popup button.")]
            public Button PopUpButton { get; set; }

            [FindBy(Name = "textinput1")]
            [Description("Text field control.")]
            internal TextFieldControl NameTextFieldControl = null; // intentionally non-public
        }
    }
}
