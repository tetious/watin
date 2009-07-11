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
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;
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
        public void ScreenShotOfIeShouldNotBeBlack()
        {
            // GIVEN
            Ie.Visible = true;
            try
            {
                var captureWebPage = new CaptureWebPageMock(Ie.DomContainer);

                // WHEN
                captureWebPage.CaptureWebPageToFile(@"C:\capture.jpg", true, true, 100, 100);

                // THEN
                var bitmap = new Bitmap(captureWebPage.Image);
                var color = bitmap.GetPixel(150,50);

                Assert.That(IsNotEqualToColorBlack(color));
                
            }
            finally
            {
                Ie.Visible = Settings.MakeNewIeInstanceVisible;
            }
        }

        [Test]
        public void ScreenShotOfHtmlDialogShouldNotBeBlack()
        {
            // GIVEN
            CaptureWebPageMock captureWebPage;
            Ie.Button("modalid").ClickNoWait();

            using (var htmlDialog = Ie.HtmlDialog(Find.ByTitle("PopUpTest")))
            {
                captureWebPage = new CaptureWebPageMock(htmlDialog.DomContainer);

                // WHEN
                captureWebPage.CaptureWebPageToFile(@"C:\capture.jpg", true, true, 100, 100);
            }

            // THEN
            var bitmap = new Bitmap(captureWebPage.Image);
            var color = bitmap.GetPixel(150,50);

            Assert.That(IsNotEqualToColorBlack(color));

        }

        private static bool IsNotEqualToColorBlack(Color color)
        {
            return color.R != 0 && color.G != 0 && color.B != 0;
        }

        [Test]
        public void SaveDefaultJpegImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.default", CaptureWebPage.ImageCodecs.Jpeg);
        }

        [Test]
        public void ShouldSaveJpgImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.jpg", CaptureWebPage.ImageCodecs.Jpeg);
        }

        [Test]
        public void ShouldSaveJpegImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.jpeg", CaptureWebPage.ImageCodecs.Jpeg);
        }

        [Test]
        public void ShouldSaveTifImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.tif", CaptureWebPage.ImageCodecs.Tiff);
        }

        [Test]
        public void ShouldSaveGifImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.gif", CaptureWebPage.ImageCodecs.Gif);
        }

        [Test]
        public void ShouldSavePngImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.png", CaptureWebPage.ImageCodecs.Png);
        }

        [Test]
        public void ShouldSaveBmpImageToFile()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.bmp", CaptureWebPage.ImageCodecs.Bmp);
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
            CaptureWebPageToFileAndAssert(@"C:\capture.jpeg", CaptureWebPage.ImageCodecs.Jpeg);
        }

        [Test]
        public void ShouldUseTiffEncoderForTif()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.tif", CaptureWebPage.ImageCodecs.Tiff);
        }

        [Test]
        public void ShouldUseGifEncoderForGif()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.gif", CaptureWebPage.ImageCodecs.Gif);
        }

        [Test]
        public void ShouldUsePngEncoderForPng()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.png", CaptureWebPage.ImageCodecs.Png);
        }

        [Test]
        public void ShouldUseBmpEncoderForBmp()
        {
            CaptureWebPageToFileAndAssert(@"C:\capture.bmp", CaptureWebPage.ImageCodecs.Bmp);
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

        private void CaptureWebPageToFileAndAssert(string imageFileName, CaptureWebPage.ImageCodecs expectedImageType)
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
            public bool CallBaseCaptureWebPageToFile { private get; set; }
            public ImageCodecs ImageType { get; private set; }
            public ImageCodecInfo ImageCodecInfo { get; private set; }
            public System.Drawing.Image Image { get; private set; }

            public CaptureWebPageMock(DomContainer domContainer) : base(domContainer)
            {
                CallBaseCaptureWebPageToFile = true;
            }

            public override void CaptureWebPageToFile(Stream stream, ImageCodecs imagetype, bool writeUrl, bool showGuides, int scalePercentage, int quality)
            {
                ImageType = imagetype;
                if (CallBaseCaptureWebPageToFile) base.CaptureWebPageToFile(stream, imagetype, writeUrl, showGuides,scalePercentage,quality);
            }

            protected override void SaveImage(System.Drawing.Image finalImage, Stream stream, ImageCodecInfo ici, EncoderParameters eps)
            {
                ImageCodecInfo = ici;
                Image = finalImage;
            }
        }
    }
}
