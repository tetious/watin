using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Logging;

namespace WatiN.Core.UnitTests.FireFoxTests
{
    [TestFixture]
    public class FireFoxTests : BaseWatiNTest
    {
        [Test]
        public void TestFF()
        {
            Logger.LogWriter = new ConsoleLogWriter();
            var browser = new FireFox(MainURI);
            var text = browser.Button("helloid").Text;
            Assert.That(text, Is.EqualTo("Show allert"));
            browser.Dispose();
        }

//        [Test]
//        public void TestFF2()
//        {
//            Logger.LogWriter = new ConsoleLogWriter();
//            var browser = new FireFox(MainURI);
//            browser.Button("helloid").Click();
//            browser.Dispose();
//        }

        [Test]
        public void TestIE()
        {
            var browser = new IE(MainURI);
            var text = browser.Button("helloid").Text;
            Assert.That(text, Is.EqualTo("Show allert"));
        }
    }
}