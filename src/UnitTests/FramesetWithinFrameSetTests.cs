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
                                Assert.AreEqual(2, browser.Frames.Count);
                                Assert.AreEqual(2, browser.Frames[1].Frames.Count);
                            });
        }

        public override Uri TestPageUri
        {
            get { return FramesetWithinFramesetURI; }
        }
    }
}