namespace WatiN.Core.UnitTests
{
  using System;
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;

  [TestFixture]
  public class ImageTests : WatiNTest
  {
    private IE ie = new IE(WatiNTest.ImagesURI);

    private Uri watinwebsiteImage = new Uri(WatiNTest.HtmlTestBaseURI, "images\\watinwebsite.jpg");
    private Uri watinwebsiteLogoImage = new Uri(WatiNTest.HtmlTestBaseURI, "images\\watin.jpg");

    [TestFixtureTearDown]
    public void Teardown()
    {
      ie.Close();
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
      Element element = ie.Element(id);
      Image image = new Image(element);
      Assert.AreEqual(id, image.Id);
    }

    [Test]
    public void ImageExists()
    {
      Assert.IsTrue(ie.Image("Image2").Exists);
      Assert.IsTrue(ie.Image(new Regex("Image2")).Exists);
      Assert.IsFalse(ie.Image("nonexistingImage").Exists);
    }

    [Test]
    public void ImageTag()
    {
      Image image = ie.Image("Image2");

      Assert.AreEqual("img", image.TagName.ToLower(), "Should be image element");
      Assert.AreEqual("Image2", image.Id, "Unexpected id");
      Assert.AreEqual("ImageName2", image.Name, "Unexpected name");
      Assert.AreEqual(watinwebsiteImage, new Uri(image.Src), "Unexpected Src");
      Assert.AreEqual("WatiN website", image.Alt, "Unexpected Alt");
    }

    [Test]
    public void ImageInputTag()
    {
      Image image = ie.Image("Image4");

      Assert.AreEqual("input", image.TagName.ToLower(), "Should be input element");
      Assert.AreEqual("Image4", image.Id, "Unexpected id");
      Assert.AreEqual("ImageName4", image.Name, "Unexpected name");
      Assert.AreEqual(watinwebsiteLogoImage, new Uri(image.Src), "Unexpected Src");
      Assert.AreEqual("WatiN logo in input element of type image", image.Alt, "Unexpected Alt");
    }

    [Test]
    public void ImageReadyStateUninitializedButShouldReturn()
    {
      Assert.IsFalse(ie.Image("Image3").Complete);
    }

    [Test]
    public void Images()
    {
      const int expectedImagesCount = 4;
      Assert.AreEqual(expectedImagesCount, ie.Images.Length, "Unexpected number of Images");

      // Collection.Length
      ImageCollection formImages = ie.Images;

      // Collection items by index
      Assert.AreEqual("Image1", ie.Images[0].Id);
      Assert.AreEqual("Image2", ie.Images[1].Id);
      Assert.AreEqual("Image3", ie.Images[2].Id);
      Assert.AreEqual("Image4", ie.Images[3].Id);

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
      Button button = ie.Button(Find.BySrc(new Regex("images/watin.jpg")));

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
      Element element = ie.Element("Area1");
      Area area = new Area(element);
      Assert.AreEqual("Area1", area.Id);
    }

    [Test]
    public void AreaExists()
    {
      Assert.IsTrue(ie.Area("Area1").Exists);
      Assert.IsTrue(ie.Area(new Regex("Area1")).Exists);
      Assert.IsFalse(ie.Area("noneexistingArea1id").Exists);
    }

    [Test]
    public void AreaTest()
    {
      Area area1 = ie.Area("Area1");

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
      AreaCollection areas = ie.Areas;
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