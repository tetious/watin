using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;

namespace WatiN.Examples.Tests
{
    [TestClass]
    public class MyIEAttachToHelperExample
    {
        static MyIEAttachToHelperExample()
        {
            Browser.RegisterAttachToHelper(typeof(MyIE), new AttachToMyIEHelper());
        }

        [TestMethod]
        public void Attach_should_return_MyIE_instance()
        {
            // GIVEN
            // launch browser to attach to later
            new MyIE("www.google.com") {AutoClose = false};

            // WHEN
            var myIe = Browser.AttachTo<MyIE>(Find.ByTitle("Google"));

            // THEN
            Assert.IsNotNull(myIe);
            Assert.IsTrue(myIe.ToString().StartsWith("Google"));
            Assert.IsTrue(myIe.ToString().EndsWith(myIe.Url));

            myIe.Close();
        }
    }
}
