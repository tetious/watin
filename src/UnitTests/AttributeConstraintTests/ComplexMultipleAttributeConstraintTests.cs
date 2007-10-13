namespace WatiN.Core.UnitTests
{
  using NUnit.Framework;
  using Rhino.Mocks;
  using WatiN.Core.Interfaces;

  [TestFixture]
  public class ComplexMultipleAttributeConstraintTests
  {
    private MockRepository mocks;
    private IAttributeBag mockAttributeBag;

    private AttributeConstraint findBy1;
    private AttributeConstraint findBy2;
    private AttributeConstraint findBy3;
    private AttributeConstraint findBy4;
    private AttributeConstraint findBy5;

    private AttributeConstraint findBy;

    [SetUp]
    public void Setup()
    {
      mocks = new MockRepository();
      mockAttributeBag = (IAttributeBag) mocks.CreateMock(typeof (IAttributeBag));
      findBy = null;

      findBy1 = Find.By("1", "true");
      findBy2 = Find.By("2", "true");
      findBy3 = Find.By("3", "true");
      findBy4 = Find.By("4", "true");
      findBy5 = Find.By("5", "true");
    }

    [Test]
    public void WithoutBrackets()
    {
      findBy = findBy1.And(findBy2).And(findBy3).Or(findBy4).And(findBy5);
    }

    [Test]
    public void WithBrackets()
    {
      findBy = findBy1.And(findBy2.And(findBy3)).Or(findBy4.And(findBy5));
    }

    [Test]
    public void WithBracketsOperators1()
    {
      findBy = findBy1 & findBy2 & findBy3 | findBy4 & findBy5;
    }

    [Test]
    public void WithBracketsOperators2()
    {
      findBy = findBy1 && findBy2 && findBy3 || findBy4 && findBy5;
    }

    [TearDown]
    public void TearDown()
    {
      Expect.Call(mockAttributeBag.GetValue("1")).Return("true");
      Expect.Call(mockAttributeBag.GetValue("2")).Return("false");
      Expect.Call(mockAttributeBag.GetValue("4")).Return("true");
      Expect.Call(mockAttributeBag.GetValue("5")).Return("false");

      mocks.ReplayAll();

      Assert.IsFalse(findBy.Compare(mockAttributeBag));

      mocks.VerifyAll();
    }

//    [Test]
//    public void testAndOr()
//    {
//      Assert.IsTrue(EchoBoolean(1) && EchoBoolean(5) && EchoBoolean(3) || EchoBoolean(2) && EchoBoolean(6));
//    }
//
//    public bool EchoBoolean(int value)
//    {
//      System.Diagnostics.Debug.WriteLine(value.ToString());
//      if (value==1) return true;
//      if (value==2) return true;
//      if (value==3) return true;
//      if (value==4) return true;
//      return false;
//    }
  }
}