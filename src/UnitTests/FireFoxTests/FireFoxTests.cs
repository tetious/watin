using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests.FireFoxTests
{
    [TestFixture]
    public class FireFoxTests : BaseWatiNTest
    {
        [Test]
        public void Test()
        {
            var browser = new FireFox(MainURI);
            var text = browser.Button("helloid").Text;
            Assert.That(text, Is.EqualTo("Show allert"));
        }
    }
}