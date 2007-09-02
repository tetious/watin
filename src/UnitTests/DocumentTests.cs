namespace WatiN.Core.UnitTests
{
  using mshtml;
  using NUnit.Framework;
  using Rhino.Mocks;

  [TestFixture]
  public class DocumentTests
  {
    private MockRepository _mockRepository;
    private IHTMLDocument2 _mockHtmlDocument;
    private IHTMLWindow2 _mockHtmlWindow2;

    [SetUp]
    public void Setup()
    {
      _mockRepository = new MockRepository();
      _mockHtmlDocument = (IHTMLDocument2) _mockRepository.CreateMock(typeof (IHTMLDocument2));
      _mockHtmlWindow2 = (IHTMLWindow2) _mockRepository.CreateMock(typeof (IHTMLWindow2));
    }

    [Test]
    public void RunScriptShouldCallHtmlDocumentProperty()
    {
      Document mockDocument = (Document) _mockRepository.PartialMock(typeof (Document));

      Expect.Call(mockDocument.HtmlDocument).Repeat.Once().Return(_mockHtmlDocument);
      Expect.Call(_mockHtmlDocument.parentWindow).Repeat.Once().Return(_mockHtmlWindow2);
      Expect.Call(_mockHtmlWindow2.execScript("alert('hello')", "javascript")).Return(null);

      _mockRepository.ReplayAll();

      mockDocument.RunScript("alert('hello')");

      _mockRepository.VerifyAll();
   }

    [Test]
    public void TestEval()
    {
      using(IE ie = new IE())
      {
        string result = ie.Eval("2+5");
        Assert.That(result, NUnit.Framework.SyntaxHelpers.Is.EqualTo("7"));
      }
    }
  }
}