namespace WatiN.Core.UnitTests
{
  using NUnit.Framework;
  using Rhino.Mocks;
  using WatiN.Core.Interfaces;

  [TestFixture]
  public class ElementFinderTests
  {
    private MockRepository mocks;
    private IElementCollection stubElementCollection;

    [SetUp]
    public void SetUp()
    {
      mocks = new MockRepository();
      stubElementCollection = (IElementCollection) mocks.DynamicMock(typeof (IElementCollection));

      SetupResult.For(stubElementCollection.Elements).Return(null);

      mocks.ReplayAll();
    }

    [TearDown]
    public void TearDown()
    {
      mocks.VerifyAll();
    }

    [Test]
    public void FindFirstShoudlReturnNullIfIElementCollectionIsNull()
    {
      ElementFinder finder = new ElementFinder("input", "text", stubElementCollection);

      Assert.IsNull(finder.FindFirst());
    }

    [Test]
    public void FindAllShouldReturnEmptyArrayListIfIElementCollectionIsNull()
    {
      ElementFinder finder = new ElementFinder("input", "text", stubElementCollection);

      Assert.AreEqual(0, finder.FindAll().Count);
    }
  }
}