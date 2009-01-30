#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
			Assert.AreEqual(2, Image.ElementTags.Count, "2 elementtags expected");
			Assert.AreEqual("img", Image.ElementTags[0].TagName);
			Assert.AreEqual("input", Image.ElementTags[1].TagName);
			Assert.AreEqual("image", Image.ElementTags[1].InputTypes);
		}

		[Test]
		public void ImageFromElementInput()
		{
			AssertImageFromElement("Image4");
		}

		[Test]
		public void ImageFromElementImage()
		{
			AssertImageFromElement("Image2");
		}

		private void AssertImageFromElement(string id)
		{
			var element = Ie.Element(id);
			var image = new Image(element);
			Assert.AreEqual(id, image.Id);
		}

		[Test]
		public void ImageExists()
		{
			Assert.IsTrue(Ie.Image("Image2").Exists);
			Assert.IsTrue(Ie.Image(new Regex("Image2")).Exists);
			Assert.IsFalse(Ie.Image("nonexistingImage").Exists);
		}

		[Test]
		public void ImageTag()
		{
			var image = Ie.Image("Image2");

			Assert.AreEqual("img", image.TagName.ToLower(), "Should be image element");
			Assert.AreEqual("Image2", image.Id, "Unexpected id");
			Assert.AreEqual("ImageName2", image.Name, "Unexpected name");
			Assert.AreEqual(watinwebsiteImage, new Uri(image.Src), "Unexpected Src");
			Assert.AreEqual(watinwebsiteImage, image.Uri, "Unexpected Src");
			Assert.AreEqual("WatiN website", image.Alt, "Unexpected Alt");
		}

		[Test]
		public void ImageInputTag()
		{
			var image = Ie.Image("Image4");

			Assert.AreEqual("input", image.TagName.ToLower(), "Should be input element");
			Assert.AreEqual("Image4", image.Id, "Unexpected id");
			Assert.AreEqual("ImageName4", image.Name, "Unexpected name");
			Assert.AreEqual(watinwebsiteLogoImage, new Uri(image.Src), "Unexpected Src");
			Assert.AreEqual("WatiN logo in input element of type image", image.Alt, "Unexpected Alt");
		}

		[Test]
		public void ImageReadyStateUninitializedButShouldReturn()
		{
			Assert.IsFalse(Ie.Image("Image3").Complete);
		}

		[Test]
		public void Images()
		{
			const int expectedImagesCount = 4;
			Assert.AreEqual(expectedImagesCount, Ie.Images.Length, "Unexpected number of Images");

			// Collection.Length
			var formImages = Ie.Images;

			// Collection items by index
			Assert.AreEqual("Image1", Ie.Images[0].Id);
			Assert.AreEqual("Image2", Ie.Images[1].Id);
			Assert.AreEqual("Image3", Ie.Images[2].Id);
			Assert.AreEqual("Image4", Ie.Images[3].Id);

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
		}

		[Test]
		public void ButtonFromInputImage()
		{
			var button = Ie.Button(Find.BySrc(new Regex("images/watin.jpg")));

			Assert.IsTrue(button.Exists, "Button should exist");
			Assert.AreEqual("Image4", button.Id, "Unexpected id");
		}

    }
}