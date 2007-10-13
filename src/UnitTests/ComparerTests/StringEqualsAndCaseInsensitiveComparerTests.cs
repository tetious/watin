namespace WatiN.Core.UnitTests
{
  using System;
  using NUnit.Framework;
  using WatiN.Core.Comparers;
  using WatiN.Core.Interfaces;

  [TestFixture]
  public class StringEqualsAndCaseInsensitiveComparerTests
  {
    [Test]
    public void ConstructorWithValue()
    {
      ICompare comparer = new StringEqualsAndCaseInsensitiveComparer("A test value");

      Assert.IsTrue(comparer.Compare("A test value"), "Exact match should pass.");
      Assert.IsTrue(comparer.Compare("a test Value"), "Match should be case insensitive");

      Assert.IsFalse(comparer.Compare("A test value 2"), "Exact match plus more should not pass.");
      Assert.IsFalse(comparer.Compare("test"), "Partial match should not match");
      Assert.IsFalse(comparer.Compare("completely different"), "Something completely different should not match");
      Assert.IsFalse(comparer.Compare(String.Empty), "String.Empty should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void ConstructorWithNullShouldThrowArgumentNullException()
    {
      new StringEqualsAndCaseInsensitiveComparer(null);
    }

    [Test]
    public void ConstuctorWithStringEmpty()
    {
      ICompare comparer = new StringEqualsAndCaseInsensitiveComparer(String.Empty);

      Assert.IsTrue(comparer.Compare(String.Empty), "String.Empty should match");

      Assert.IsFalse(comparer.Compare(" "), "None Empty string should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }

    [Test]
    public void ToStringTest()
    {
      StringEqualsAndCaseInsensitiveComparer comparer = new StringEqualsAndCaseInsensitiveComparer("A test value");

      Assert.AreEqual("A test value", comparer.ToString());
    }
  }
}