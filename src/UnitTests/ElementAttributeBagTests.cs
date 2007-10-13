namespace WatiN.Core.UnitTests
{
  using mshtml;
  using NUnit.Framework;
  using Rhino.Mocks;

  [TestFixture]
  public class ElementAttributeBagTests
  {
    private MockRepository mocks;
    private IHTMLStyle mockHTMLStyle;
    private IHTMLElement mockHTMLElement;

    [SetUp]
    public void SetUp()
    {
      mocks = new MockRepository();
      mockHTMLStyle = (IHTMLStyle) mocks.CreateMock(typeof (IHTMLStyle));
      mockHTMLElement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));
    }

    [TearDown]
    public void TearDown()
    {
      mocks.VerifyAll();
    }

    [Test]
    public void StyleAttributeShouldReturnAsString()
    {
      const string cssText = "COLOR: white; FONT-STYLE: italic";

      Expect.Call(mockHTMLStyle.cssText).Return(cssText);
      Expect.Call(mockHTMLElement.style).Return(mockHTMLStyle);

      mocks.ReplayAll();

      ElementAttributeBag attributeBag = new ElementAttributeBag(mockHTMLElement);

      Assert.AreEqual(cssText, attributeBag.GetValue("style"));
    }

    [Test]
    public void StyleDotStyleAttributeNameShouldReturnStyleAttribute()
    {
      const string styleAttributeValue = "white";
      const string styleAttributeName = "color";

      Expect.Call(mockHTMLStyle.getAttribute(styleAttributeName, 0)).Return(styleAttributeValue);
      Expect.Call(mockHTMLElement.style).Return(mockHTMLStyle);

      mocks.ReplayAll();

      ElementAttributeBag attributeBag = new ElementAttributeBag(mockHTMLElement);

      Assert.AreEqual(styleAttributeValue, attributeBag.GetValue("style.color"));
    }
  }
}