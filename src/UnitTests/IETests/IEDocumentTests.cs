using System;
using mshtml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests.IETests
{
    [TestFixture]
    public class IEDocumentTests : BaseWithIETests
    {
        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        [Test]
        public void ConstructorShouldThrowExceptionIfArgumentNotOfTypeIHTMLDocument()
        {
            // GIVEN
            
            // WHEN

            // THEN
//            Assert.That();
        }
    }
}
