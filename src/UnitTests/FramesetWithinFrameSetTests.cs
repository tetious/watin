using System;
using NUnit.Framework;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class FramesetWithinFrameSetTests : BaseWithBrowserTests
    {
        [Test]
        public void FramesLength()
        {
            ExecuteTest(browser =>
                            {
                                Assert.AreEqual(2, browser.Frames.Length);
                                Assert.AreEqual(2, browser.Frames[1].Frames.Length);
                            });
        }

        public override Uri TestPageUri
        {
            get { return FramesetWithinFramesetURI; }
        }
    }
}