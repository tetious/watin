namespace WatiN.Core.UnitTests
{
  using System.Collections;
  using NUnit.Framework;
  using NUnit.Framework.SyntaxHelpers;

  [TestFixture]
  public class ElementTagTests
  {
    [Test]
    public void CompareNullShouldReturnFalse()
    {
      ElementTag elementTag = new ElementTag("tagname", "");
      Assert.IsFalse(elementTag.Compare(null));
    }

    [Test]
    public void CompareObjectNotImplementingIHTMLElementShouldReturnFalse()
    {
      ElementTag elementTag = new ElementTag("tagname", "");
      Assert.IsFalse(elementTag.Compare(new object()));
    }

    [Test]
    public void IsValidElementWithNullElementShouldReturnFalse()
    {
      Assert.IsFalse(ElementTag.IsValidElement(null, new ArrayList()));
    }

    [Test]
    public void IsValidElementWithObjectNotImplementingIHTMLElementShouldReturnFalse()
    {
      Assert.IsFalse(ElementTag.IsValidElement(new object(), new ArrayList()));
    }

    [Test]
    public void ToStringShouldBeEmptyIfTagNameIsNull()
    {
      ElementTag elementTag = new ElementTag((string) null);
      Assert.That(elementTag.ToString(), Is.EqualTo(""));
    }
  }
}