namespace WatiN.Core.UnitTests
{
  using System.Text.RegularExpressions;
  using mshtml;
  using NUnit.Framework;
  using Rhino.Mocks;
  using SHDocVw;

  [TestFixture]
  public class DomContainerTests
  {
    private MockRepository _mockRepository;
    private InternetExplorer _mockInternetExplorer;
    private IE _ie;
    private IHTMLDocument2 _mockHTMLDocument2;

    private IWait _mockWait;
    private IHTMLElement _mockIHTMLElement;

    [SetUp]
    public void Setup()
    {
      _mockRepository = new MockRepository();

      _mockInternetExplorer = (InternetExplorer) _mockRepository.DynamicMock(typeof (InternetExplorer));
      _mockHTMLDocument2 = (IHTMLDocument2) _mockRepository.DynamicMock(typeof (IHTMLDocument2));
      _mockIHTMLElement = (IHTMLElement) _mockRepository.DynamicMock(typeof (IHTMLElement));
      
      SetupResult.For(_mockInternetExplorer.Document).Return(_mockHTMLDocument2);
      SetupResult.For(_mockHTMLDocument2.body).Return(_mockIHTMLElement);
      SetupResult.For(_mockIHTMLElement.innerText).Return("Test 'Contains text in DIV' text");

      _mockRepository.Replay(_mockIHTMLElement);
      _mockRepository.Replay(_mockHTMLDocument2);
      _mockRepository.Replay(_mockInternetExplorer);
      
      _ie = new IE(_mockInternetExplorer);
      _mockWait = (IWait)_mockRepository.CreateMock(typeof(IWait));
    }

    [Test]
    public void WaitForCompletUsesGivenWaitClass()
    {
      _mockWait.DoWait();

      _mockRepository.Replay(_mockWait);

      _ie.WaitForComplete(_mockWait);

      _mockRepository.Verify(_mockWait);
    }

    [Test]
    public void ContainsText()
    {
      Assert.IsTrue(_ie.ContainsText("Contains text in DIV"), "Text not found");
      Assert.IsFalse(_ie.ContainsText("abcde"), "Text incorrectly found");
              
      Assert.IsTrue(_ie.ContainsText(new Regex("Contains text in DIV")), "Regex: Text not found");
      Assert.IsFalse(_ie.ContainsText(new Regex("abcde")), "Regex: Text incorrectly found");
    }

    [TearDown]
    public virtual void TearDown()
    {
      _ie.Dispose();
    }

  }
}
