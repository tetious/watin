namespace WatiN.Core.UnitTests
{
  using System;
  using NUnit.Framework;
  using WatiN.Core.Comparers;
  using WatiN.Core.Interfaces;

  [TestFixture]
  public class BoolComparerTests
  {
    [Test]
    public void CompareToTrue()
    {
      ICompare comparer = new BoolComparer(true);

      Assert.IsTrue(comparer.Compare(true.ToString()), "true.ToString()");
      Assert.IsTrue(comparer.Compare("true"), "true");
      Assert.IsTrue(comparer.Compare("True"), "True");
      Assert.IsFalse(comparer.Compare("false"), "false");
      Assert.IsFalse(comparer.Compare("some other string"), "some other string");
    }

    [Test]
    public void CompareToNull()
    {
      Assert.IsFalse(new BoolComparer(false).Compare(null), "null");
    }

    [Test]
    public void CompareToStringEmpty()
    {
      Assert.IsFalse(new BoolComparer(false).Compare(String.Empty), String.Empty);
    }

    [Test]
    public void CompareToFalse()
    {
      ICompare comparer = new BoolComparer(false);

      Assert.IsTrue(comparer.Compare(false.ToString()), "false.ToString()");
      Assert.IsTrue(comparer.Compare("false"), "false");
      Assert.IsTrue(comparer.Compare("False"), "False");
      Assert.IsFalse(comparer.Compare("true"), "true");
      Assert.IsFalse(comparer.Compare("some other string"), "some other string");
    }
  }
}