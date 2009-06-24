using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class ControlTests : BaseWithBrowserTests
    {
        [Test]
        public void ExistsShouldReturnTrue()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                var control = browser.Control<TextFieldControl>("name");

                // WHEN
                var exists = control.Exists;

                // THEN
                Assert.That(exists, Is.True);
            });
        }

        [Test]
        public void ExistsShouldReturnFalse()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                var control = browser.Control<TextFieldControl>("noneExistingTextFieldId");

                // WHEN
                var exists = control.Exists;

                // THEN
                Assert.That(exists, Is.False);
            });
        }

        [Test]
        public void ExistsShouldReturnImmediatelyIfNoMatchingElementCanBeFound()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                Assert.That(Settings.WaitUntilExistsTimeOut, Is.GreaterThan(1), "pre-condition failed");
                var control = browser.Control<TextFieldControl>("noneExistingTextFieldId");

                // WHEN
                var start = DateTime.Now;
                var exists = control.Exists;
                var end = DateTime.Now;

                // THEN
                Assert.That(end.Subtract(start).TotalSeconds, Is.LessThanOrEqualTo(1d),
                    "Should not wait for element to show up");
                Assert.That(exists, Is.False, "control shouldn't exist");
            });
        }

        [Test]
        public void ShouldFindControlWhenUsingAPredicate()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                var control = browser.Control<TextFieldControl>(tfc => Equals(tfc.Id, "name"));

                // WHEN
                var exists = control.Exists;

                // THEN
                Assert.That(exists, Is.True);
            });
        }

        [Test]
        public void ShouldInitializeElementField()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                var control = browser.Control<FormControl>("Form");

                // WHEN
                var member = control.NameTextField;

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
                var control = browser.Control<FormControl>("Form");

                // WHEN
                var member = control.PopUpButton;

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
                var control = browser.Control<FormControl>("Form");

                // WHEN
                var member = control.NameTextFieldControl;

                // THEN
                Assert.That(member, Is.Not.Null);
                Assert.That(member.Description, Is.EqualTo("Text field control."));
                Assert.That(member.Id, Is.EqualTo("name"));
            });
        }

        [Test]
        public void ToStringWhenDescriptionIsNotSetShouldDescribeControl()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                var control = browser.Control<FormControl>("Form");

                // WHEN
                var description = control.Description;
                var toString = control.ToString();

                // THEN
                Assert.That(description, Is.Null);
                Assert.That(toString, Is.EqualTo("FormControl (Form)"));
            });
        }

        [Test]
        public void ToStringWhenDescriptionIsSetShouldReturnDescription()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
#if !NET20
                var control = browser.Control<FormControl>("Form").WithDescription("foo");
#else
                var control = browser.Control<FormControl>("Form");
                control.Description = "foo";
#endif

                // WHEN
                var description = control.Description;
                var toString = control.ToString();

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

        public class FormControl : Control<Form>
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
