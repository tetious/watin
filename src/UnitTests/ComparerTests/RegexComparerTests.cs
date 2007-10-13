namespace WatiN.Core.UnitTests
{
  using System;
  using System.Text.RegularExpressions;
  using NUnit.Framework;
  using WatiN.Core.Comparers;
  using WatiN.Core.Interfaces;

  [TestFixture]
  public class RegexComparerTests
  {
    [Test]
    public void ConstructorWithValue()
    {
      ICompare comparer = new RegexComparer(new Regex("^A test value$"));

      Assert.IsTrue(comparer.Compare("A test value"), "Exact match should pass.");

      Assert.IsFalse(comparer.Compare("a test Value"), "Match should be case sensitive");
      Assert.IsFalse(comparer.Compare("A test value 2"), "Exact match plus more should not pass.");
      Assert.IsFalse(comparer.Compare("test"), "Partial match should not match");
      Assert.IsFalse(comparer.Compare("completely different"), "Something completely different should not match");
      Assert.IsFalse(comparer.Compare(String.Empty), "String.Empty should not match");
      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void ConstructorWithNullShouldThrowArgumentNullException()
    {
      new RegexComparer(null);
    }

    [Test]
    public void ConstuctorWithStringEmpty()
    {
      ICompare comparer = new RegexComparer(new Regex(String.Empty));

      Assert.IsTrue(comparer.Compare(String.Empty), "String.Empty should match");
      Assert.IsTrue(comparer.Compare(" "), "Any string should not match");

      Assert.IsFalse(comparer.Compare(null), "null should not match");
    }

    [Test]
    public void ToStringTest()
    {
      RegexComparer comparer = new RegexComparer(new Regex("^A test value$"));

      Assert.AreEqual("WatiN.Core.Comparers.RegexComparer matching against: ^A test value$", comparer.ToString());
    }
  }
}