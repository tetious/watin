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
using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ImageTests : BaseWithBrowserTests
	{
		private readonly Uri watinwebsiteImage = new Uri(HtmlTestBaseURI, "images\\watinwebsite.jpg");
		private readonly Uri watinwebsiteLogoImage = new Uri(HtmlTestBaseURI, "images\\watin.jpg");

		public override Uri TestPageUri
		{
			get { return ImagesURI; }
		}

		[Test]
		public void ImageElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<Image>();
            Assert.AreEqual(2, elementTags.Count, "2 elementtags expected");
		    Assert.That(elementTags[0], Is.EqualTo(new ElementTag("img")));
            Assert.That(elementTags[1], Is.EqualTo(new ElementTag("input", "image")));
		}

		[Test]
		public void ImageExists()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.IsTrue(browser.Image("Image2").Exists);
                                Assert.IsTrue(browser.Image(new Regex("Image2")).Exists);
                                Assert.IsFalse(browser.Image("nonexistingImage").Exists);
		                    });
		}

		[Test]
		public void ImageTag()
		{
		    ExecuteTest(browser =>
		                    {
		                        var image = browser.Image("Image2");

		                        Assert.AreEqual("img", image.TagName.ToLower(), "Should be image element");
		                        Assert.AreEqual("Image2", image.Id, "Unexpected id");
		                        Assert.AreEqual("ImageName2", image.Name, "Unexpected name");
		                        Assert.AreEqual(watinwebsiteImage, new Uri(image.Src), "Unexpected Src");
		                        Assert.AreEqual(watinwebsiteImage, image.Uri, "Unexpected Src");
		                        Assert.AreEqual("WatiN website", image.Alt, "Unexpected Alt");
		                    });
		}

		[Test]
		public void ImageInputTag()
		{
		    ExecuteTest(browser =>
		                    {
		                        var image = browser.Image("Image4");

		                        Assert.AreEqual("input", image.TagName.ToLower(), "Should be input element");
		                        Assert.AreEqual("Image4", image.Id, "Unexpected id");
		                        Assert.AreEqual("ImageName4", image.Name, "Unexpected name");
		                        Assert.AreEqual(watinwebsiteLogoImage, new Uri(image.Src), "Unexpected Src");
		                        Assert.AreEqual("WatiN logo in input element of type image", image.Alt, "Unexpected Alt");
		                    });
		}

		[Test]
		public void ImageReadyStateUninitializedButShouldReturn()
		{
		    ExecuteTest(browser => Assert.IsFalse(browser.Image("Image3").Complete));
		}

		[Test]
		public void Images()
		{
		    ExecuteTest(browser =>
		                    {
		                        const int expectedImagesCount = 5;
                                Assert.AreEqual(expectedImagesCount, browser.Images.Count, "Unexpected number of Images");

		                        // Collection.Length
		                        var formImages = browser.Images;

		                        // Collection items by index
                                Assert.AreEqual("Image1", browser.Images[0].Id);
                                Assert.AreEqual("Image2", browser.Images[1].Id);
                                Assert.AreEqual("Image3", browser.Images[2].Id);
                                Assert.AreEqual("linkimage", browser.Images[3].Id);
                                Assert.AreEqual("Image4", browser.Images[4].Id);

		                        IEnumerable ImageEnumerable = formImages;
		                        var ImageEnumerator = ImageEnumerable.GetEnumerator();

		                        // Collection iteration and comparing the result with Enumerator
		                        var count = 0;
		                        foreach (Image inputImage in formImages)
		                        {
		                            ImageEnumerator.MoveNext();
		                            var enumImage = ImageEnumerator.Current;

		                            Assert.IsInstanceOfType(inputImage.GetType(), enumImage, "Types are not the same");
		                            Assert.AreEqual(inputImage.OuterHtml, ((Image) enumImage).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(ImageEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(expectedImagesCount, count);
		                    });
		}
    }
}