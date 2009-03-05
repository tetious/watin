#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using System.Drawing.Imaging;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class CaptureWebPageToFileTests : BaseWithBrowserTests
    {
        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        [Test]
        public void SaveDefaultJpegImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.default", "default");
        }

        [Test]
        public void ShouldSaveJpgImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.jpg", "jpg");
        }

        [Test]
        public void ShouldSaveJpegImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.jpeg", "jpeg");
        }

        [Test]
        public void ShouldSaveTifImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.tif", "tif");
        }

        [Test]
        public void ShouldSaveGifImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.gif", "gif");
        }

        [Test]
        public void ShouldSavePngImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.png", "png");
        }

        [Test]
        public void ShouldSaveBmpImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.bmp", "bmp");
        }

        [Test]
        public void ShouldUseDefaultEncoderForUnknownImageType()
        {
            AssertImageTypeVersusEncoderMimeType(@"C:\capture.default", "image/jpeg");
        }

        [Test]
        public void ShouldUseJpegEncoderForJpg()
        {
            AssertImageTypeVersusEncoderMimeType(@"C:\capture.jpg", "image/jpeg");
        }

        [Test]
        public void ShouldUseJpegEncoderForJpeg()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.jpeg", "jpeg");
        }

        [Test]
        public void ShouldUseTiffEncoderForTif()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.tif", "tif");
        }

        [Test]
        public void ShouldUseGifEncoderForGif()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.gif", "gif");
        }

        [Test]
        public void ShouldUsePngEncoderForPng()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.png", "png");
        }

        [Test]
        public void ShouldUseBmpEncoderForBmp()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.bmp", "bmp");
        }

        private void AssertImageTypeVersusEncoderMimeType(string imageFileName, string expectedMimeType)
        {
            // GIVEN
            var captureWebPage = new CaptureWebPageMock(Ie.DomContainer);

            // WHEN
            captureWebPage.CaptureWebPageToFile(imageFileName, true, true, 100, 100);

            // THEN
            Assert.That(captureWebPage.ImageCodecInfo.MimeType, Is.EqualTo(expectedMimeType), "Unexpected mime type");
        }

        private void CaptureWebPageToFileAndAssert(string imageFileName, string expectedImageType)
        {
            // GIVEN
            var captureWebPage = new CaptureWebPageMock(Ie.DomContainer) { CallBaseCaptureWebPageToFile = false };

            // WHEN
            captureWebPage.CaptureWebPageToFile(imageFileName, true, true, 100, 100);

            // THEN
            Assert.That(captureWebPage.ImageType, Is.EqualTo(expectedImageType), "Unexpected imagetype");
        }

        private class CaptureWebPageMock : CaptureWebPage
        {
            public bool CallBaseCaptureWebPageToFile { get; set; }
            public string ImageType { get; private set; }
            public ImageCodecInfo ImageCodecInfo { get; private set; }

            public CaptureWebPageMock(DomContainer domContainer) : base(domContainer)
            {
                CallBaseCaptureWebPageToFile = true;
            }

            public override void CaptureWebPageToFile(Stream stream, string imagetype, bool writeUrl, bool showGuides, int scalePercentage, int quality)
            {
                ImageType = imagetype;
                if (CallBaseCaptureWebPageToFile) base.CaptureWebPageToFile(stream, imagetype,writeUrl, showGuides,scalePercentage,quality);
            }

            protected override void SaveImage(System.Drawing.Image finalImage, Stream stream, ImageCodecInfo ici, EncoderParameters eps)
            {
                ImageCodecInfo = ici;
            }
        }
    }
}
