using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;

namespace TestProject
{
    [TestClass]
    public class UnitTest 
    {
        private static IEStaticInstanceHelper ieStaticInstanceHelper;

        [ClassInitialize]
        public static void testInit(TestContext testContext)
        {
            ieStaticInstanceHelper = new IEStaticInstanceHelper();
            ieStaticInstanceHelper.IE = new IE("http://news.bbc.co.uk");
        }

        public IE IE
        {
            get { return ieStaticInstanceHelper.IE; }
            set { ieStaticInstanceHelper.IE = value; }
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            ieStaticInstanceHelper.IE.Close();
            ieStaticInstanceHelper = null;
        }

        [TestMethod]
        public void testOne()
        {
            Assert.IsTrue(IE.ContainsText("Low graphics"));
        }

        [TestMethod]
        public void testTwo()
        {
            Assert.IsTrue(IE.ContainsText("Low graphics"));
        }
    }
}
