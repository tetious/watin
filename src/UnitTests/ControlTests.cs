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
            // GIVEN
            var control = Ie.Control<TextFieldControl>("name");

            // WHEN
            var exists = control.Exists;

            // THEN
            Assert.That(exists, Is.True);
        }

        [Test]
        public void ExistsShouldReturnFalse()
        {
            // GIVEN
            var control = Ie.Control<TextFieldControl>("noneExistingTextFieldId");

            // WHEN
            var exists = control.Exists;

            // THEN
            Assert.That(exists, Is.False);
        }

        [Test]
        public void ExistsShouldReturnImmediatelyIfNoMatchingElementCanBeFound()
        {
            // GIVEN
            Assert.That(Settings.WaitUntilExistsTimeOut, Is.GreaterThan(1), "pre-condition failed");
            var control = Ie.Control<TextFieldControl>("noneExistingTextFieldId");

            // WHEN
            var start = DateTime.Now;
            var exists = control.Exists;
            var end = DateTime.Now;

            // THEN
            Assert.That(end.Subtract(start).TotalSeconds, Is.LessThanOrEqualTo(1d), "Should not wait for element to show up");
            Assert.That(exists, Is.False, "control shouldn't exist");
        }

        [Test]
        public void ShouldFindControlWhenUsingAPredicate()
        {
            // GIVEN
            var control = Ie.Control<TextFieldControl>(tfc => Equals(tfc.Id, "name"));

            // WHEN
            var exists = control.Exists;

            // THEN
            Assert.That(exists, Is.True);
        }


        public override Uri TestPageUri
        {
            get { return MainURI; }
        }
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
}
