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
		private Uri watinwebsiteImage = new Uri(HtmlTestBaseURI, "images\\watinwebsite.jpg");
		private Uri watinwebsiteLogoImage = new Uri(HtmlTestBaseURI, "images\\watin.jpg");

		public override Uri TestPageUri
		{
			get { return ImagesURI; }
		}

		[Test]
		public void ImageElementTags()
		{
			Assert.AreEqual(2, Image.ElementTags.Count, "2 elementtags expected");
			Assert.AreEqual("img", ((ElementTag) Image.ElementTags[0]).TagName);
			Assert.AreEqual("input", ((ElementTag) Image.ElementTags[1]).TagName);
			Assert.AreEqual("image", ((ElementTag) Image.ElementTags[1]).InputTypes);
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
			Element element = Ie.Element(id);
			Image image = new Image(element);
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
			Image image = Ie.Image("Image2");

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
			Image image = Ie.Image("Image4");

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
			ImageCollection formImages = Ie.Images;

			// Collection items by index
			Assert.AreEqual("Image1", Ie.Images[0].Id);
			Assert.AreEqual("Image2", Ie.Images[1].Id);
			Assert.AreEqual("Image3", Ie.Images[2].Id);
			Assert.AreEqual("Image4", Ie.Images[3].Id);

			IEnumerable ImageEnumerable = formImages;
			IEnumerator ImageEnumerator = ImageEnumerable.GetEnumerator();

			// Collection iteration and comparing the result with Enumerator
			int count = 0;
			foreach (Image inputImage in formImages)
			{
				ImageEnumerator.MoveNext();
				object enumImage = ImageEnumerator.Current;

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
			Button button = Ie.Button(Find.BySrc(new Regex("images/watin.jpg")));

			Assert.IsTrue(button.Exists, "Button should exist");
			Assert.AreEqual("Image4", button.Id, "Unexpected id");
		}

		[Test]
		public void AreaElementTags()
		{
			Assert.AreEqual(1, Area.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("area", ((ElementTag) Area.ElementTags[0]).TagName);
			Assert.AreEqual(null, ((ElementTag) Area.ElementTags[0]).InputTypes);
		}

		[Test]
		public void AreaFromElement()
		{
			Element element = Ie.Element("Area1");
			Area area = new Area(element);
			Assert.AreEqual("Area1", area.Id);
		}

		[Test]
		public void AreaExists()
		{
			Assert.IsTrue(Ie.Area("Area1").Exists);
			Assert.IsTrue(Ie.Area(new Regex("Area1")).Exists);
			Assert.IsFalse(Ie.Area("noneexistingArea1id").Exists);
		}

		[Test]
		public void AreaTest()
		{
			Area area1 = Ie.Area("Area1");

			Assert.AreEqual("Area1", area1.Id, "Found wrong area");

			Assert.AreEqual("WatiN", area1.Alt, "Alt text was incorrect");
			Assert.IsTrue(area1.Url.EndsWith("main.html"), "Url was incorrect");
			Assert.AreEqual("0,0,110,45", area1.Coords, "Coords was incorrect");
			Assert.AreEqual("rect", area1.Shape.ToLower(), "Shape was incorrect");
		}

		[Test]
		public void Areas()
		{
			// Collection items by index
			AreaCollection areas = Ie.Areas;
			Assert.AreEqual(2, areas.Length, "Unexpected number of areas");
			Assert.AreEqual("Area1", areas[0].Id);
			Assert.AreEqual("Area2", areas[1].Id);

			// Collection iteration and comparing the result with Enumerator
			IEnumerable areaEnumerable = areas;
			IEnumerator areaEnumerator = areaEnumerable.GetEnumerator();

			int count = 0;
			foreach (Area area in areaEnumerable)
			{
				areaEnumerator.MoveNext();
				object enumArea = areaEnumerator.Current;

				Assert.IsInstanceOfType(area.GetType(), enumArea, "Types are not the same");
				Assert.AreEqual(area.OuterHtml, ((Area) enumArea).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(areaEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(2, count);
		}
	}
}