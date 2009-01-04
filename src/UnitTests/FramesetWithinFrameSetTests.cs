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
            Assert.AreEqual(2, Ie.Frames.Length);
            Assert.AreEqual(2, Ie.Frames[1].Frames.Length);
        }

        public override Uri TestPageUri
        {
            get { return FramesetWithinFramesetURI; }
        }
    }
}