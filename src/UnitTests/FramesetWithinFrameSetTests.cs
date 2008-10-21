using System;
using NUnit.Framework;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class FramesetWithinFrameSetTests : BaseWithIETests
    {
        [Test]
        public void FramesLength()
        {
            Assert.AreEqual(2, ie.Frames.Length);
            Assert.AreEqual(2, ie.Frames[1].Frames.Length);
        }

        public override Uri TestPageUri
        {
            get { return FramesetWithinFramesetURI; }
        }
    }
}