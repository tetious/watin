namespace WatiN.Core.UnitTests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  public class ElementStyleTests : WatiNTest
  {
    private const string style = "FONT-SIZE: 12px; COLOR: white; FONT-STYLE: italic; FONT-FAMILY: Arial; HEIGHT: 50px; BACKGROUND-COLOR: blue";

    private IE ie = new IE();
    private TextField element;

    [SetUp]
    public void TestSetup()
    {
      if (!ie.Uri.Equals(WatiNTest.MainURI))
      {
        ie.GoTo(MainURI);
        element = ie.TextField("Textarea1");
      }
    }

    [TestFixtureTearDown]
    public void FixtureTeardown()
    {
      ie.Close();
    }

    [Test]
    public void GetAttributeValueStyleAsString()
    {
      Assert.AreEqual(style, element.GetAttributeValue("style"));
    }

    [Test]
    public void ElementStyleToStringReturnsCssText()
    {
      Assert.AreEqual(style, element.Style.ToString());
    }

    [Test]
    public void ElementStyleCssText()
    {
      Assert.AreEqual(style, element.Style.CssText);
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void GetAttributeValueOfNullThrowsArgumenNullException()
    {
      element.Style.GetAttributeValue(null);
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void GetAttributeValueOfEmptyStringThrowsArgumenNullException()
    {
      element.Style.GetAttributeValue(String.Empty);
    }

    [Test]
    public void GetAttributeValueBackgroundColor()
    {
      Assert.AreEqual("blue", element.Style.GetAttributeValue("BackgroundColor"));
    }

    [Test]
    public void GetAttributeValueBackgroundColorByOriginalHTMLattribname()
    {
      Assert.AreEqual("blue", element.Style.GetAttributeValue("background-color"));
    }

    [Test]
    public void GetAttributeValueOfUndefiniedButValidAttribute()
    {
      Assert.IsNull(element.Style.GetAttributeValue("cursor"));
    }

    [Test]
    public void GetAttributeValueOfUndefiniedAndInvalidAttribute()
    {
      Assert.IsNull(element.Style.GetAttributeValue("nonexistingattrib"));
    }

    [Test]
    public void BackgroundColor()
    {
      Assert.AreEqual("blue", element.Style.BackgroundColor);
    }

    [Test]
    public void Color()
    {
      Assert.AreEqual("white", element.Style.Color);
    }

    [Test]
    public void FontFamily()
    {
      Assert.AreEqual("Arial", element.Style.FontFamily);
    }

    [Test]
    public void FontSize()
    {
      Assert.AreEqual("12px", element.Style.FontSize);
    }

    [Test]
    public void FontStyle()
    {
      Assert.AreEqual("italic", element.Style.FontStyle);
    }

    [Test]
    public void Height()
    {
      Assert.AreEqual("50px", element.Style.Height);
    }
  }
}