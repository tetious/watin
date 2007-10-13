namespace WatiN.Core.UnitTests
{
  using NUnit.Framework;
  using Rhino.Mocks;
  using WatiN.Core.Interfaces;

  [TestFixture]
  public class NotAttributeConstraintTests
  {
    private MockRepository mocks;
    private AttributeConstraint attribute;
    private IAttributeBag attributeBag;

    [SetUp]
    public void Setup()
    {
      mocks = new MockRepository();
      attribute = (AttributeConstraint) mocks.DynamicMock(typeof (AttributeConstraint), "fake", "");
      attributeBag = (IAttributeBag) mocks.DynamicMock(typeof (IAttributeBag));

      SetupResult.For(attribute.Compare(null)).IgnoreArguments().Return(false);
      mocks.ReplayAll();
    }

    [TearDown]
    public void TearDown()
    {
      mocks.VerifyAll();
    }

    [Test]
    public void NotTest()
    {
      NotAttributeConstraint notAttributeConstraint = new NotAttributeConstraint(attribute);
      Assert.IsTrue(notAttributeConstraint.Compare(attributeBag));
    }

    [Test]
    public void AttributeOperatorNotOverload()
    {
      AttributeConstraint attributenot = !attribute;

      Assert.IsInstanceOfType(typeof (NotAttributeConstraint), attributenot, "Expected NotAttributeConstraint instance");
      Assert.IsTrue(attributenot.Compare(attributeBag));
    }
  }
}