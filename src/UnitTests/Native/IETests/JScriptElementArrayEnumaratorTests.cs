using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.ResearchTests
{
    [TestFixture]
    public class JScriptElementArrayEnumaratorTests : BaseWithBrowserTests
    {
        [Test]
        public void Should_itterate_elements()
        {
            Ie.RunScript("document.result = toArray();");

            var elements = new JScriptElementArrayEnumerator((IEDocument)Ie.NativeDocument, "result");

            Assert.That(elements.Count(), Is.EqualTo(2));
            Assert.That(elements.First().GetAttributeValue("Id"), Is.EqualTo("popupid"));
            Assert.That(elements.Last().GetAttributeValue("Id"), Is.EqualTo("Select1"));
        }

        [Test]
        public void Should_not_crash_if_field_doesnt_exist()
        {
            // GIVEN
            const string iDontExist = "I_dont_exist";

            // WHEN
            var elements = new JScriptElementArrayEnumerator((IEDocument) Ie.NativeDocument, iDontExist);

            // THEN
            Assert.That(elements.Count(), Is.EqualTo(0));
        }

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }
    }
}
