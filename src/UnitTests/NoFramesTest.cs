using System;
using NUnit.Framework;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class NoFramesTest : BaseWithBrowserTests
    {
        [Test]
        public void HasNoFrames()
        {
            ExecuteTest(browser => Assert.AreEqual(0, browser.Frames.Count));
        }

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }
    }
}
