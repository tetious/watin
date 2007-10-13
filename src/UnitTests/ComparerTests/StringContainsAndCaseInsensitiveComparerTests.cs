namespace WatiN.Core.UnitTests
{
  using System;
  using NUnit.Framework;
  using WatiN.Core.Comparers;
  using WatiN.Core.Interfaces;

  [TestFixture]
  public class StringContainsAndCaseInsensitiveComparerTests
  {
    [Test]
    public void ConstructorWithValue()
    {
      ICompare comparer = new StringContainsAndCaseInsensitiveComparer("A test value");

      Assert.IsTrue(comparer.Compare("A test value"), "Exact match should pass.");
      Assert.IsTrue(comparer.Compare("a test Value"), "Case should be ignored");
      Assert.IsTrue(comparer.Compare("A test value 2"), "Exact match plus more should pass.");

      Assert.IsFalse(comparer.Compare("test"), "A part of the Value should not match");
      Assert.IsFalse(comparer.Compare("completely different"), "Something completely different should not match");
      Assert.IsFalse(comparer.Compare(String.Empty), "String.Empty should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void ConstructorWithNullShouldThrowArgumentNullException()
    {
      new StringContainsAndCaseInsensitiveComparer(null);
    }

    [Test]
    public void ConstuctorWithStringEmpty()
    {
      ICompare comparer = new StringContainsAndCaseInsensitiveComparer(String.Empty);

      Assert.IsTrue(comparer.Compare(String.Empty), "String.Empty should match");

      Assert.IsFalse(comparer.Compare(" "), "None Empty string should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }

    [Test]
    public void ToStringTest()
    {
      StringContainsAndCaseInsensitiveComparer comparer = new StringContainsAndCaseInsensitiveComparer("A test value");

      Assert.AreEqual("a test value", comparer.ToString());
    }
  }
}