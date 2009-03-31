using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;

namespace TestProject
{
    [TestClass]
    public class ProblemWithSharingTests
    {
        private static IE ie;

        [ClassInitialize]
        public static void testInit(TestContext testContext)
        {
            ie = new IE("http://news.bbc.co.uk");
        }

        [TestMethod]
        public void testOne()
        {
            Assert.IsTrue(ie.ContainsText("Low graphics"));
        }

        [TestMethod]
        public void testTwo()
        {
            Assert.IsTrue(ie.ContainsText("Low graphics"));
        }
    }

}
