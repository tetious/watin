namespace WatiN.Core.UnitTests
{
  using mshtml;
  using NUnit.Framework;
  using Rhino.Mocks;
  using WatiN.Core.Exceptions;

  [TestFixture]
  public class DocumentTests : WatiNTest
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
    public void RunJavaScript()
    {
      using (IE ie = new IE(IndexURI))
      {
        ie.RunScript("window.document.write('java script has run');");
        Assert.That(ie.Text, NUnit.Framework.SyntaxHelpers.Is.EqualTo("java script has run"));

        try
        {
          ie.RunScript("a+1");
          Assert.Fail("Expected RunScriptException");
        }
        catch (RunScriptException)
        {
          // OK;
        }
      }
    }

    [Test]
    public void Text()
    {
      IHTMLElement _mockHTMLElement = (IHTMLElement) _mockRepository.CreateMock(typeof (IHTMLElement));

      Document mockDocument = (Document) _mockRepository.PartialMock(typeof (Document));

      Expect.Call(mockDocument.HtmlDocument).Repeat.Once().Return(_mockHtmlDocument);
      Expect.Call(_mockHtmlDocument.body).Return(_mockHTMLElement);
      Expect.Call(_mockHTMLElement.innerText).Return("Document innertext returned");

      _mockRepository.ReplayAll();

      Assert.That(mockDocument.Text, NUnit.Framework.SyntaxHelpers.Is.EqualTo("Document innertext returned"));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void TestEval()
    {
      using (IE ie = new IE())
      {
        string result = ie.Eval("2+5");
        Assert.That(result, NUnit.Framework.SyntaxHelpers.Is.EqualTo("7"));

        result = ie.Eval("'te' + 'st'");
        Assert.That(result, NUnit.Framework.SyntaxHelpers.Is.EqualTo("test"));
        

        try
        {
          ie.Eval("a+1");
          Assert.Fail("Expected JavaScriptException");
        }
        catch (JavaScriptException)
        {
          //;
        }

        // Make sure a valid eval after a failed eval executes OK.
        result = ie.Eval("window.document.write('java script has run');4+4;");
        Assert.AreEqual("8", result);
        Assert.That(ie.Text, NUnit.Framework.SyntaxHelpers.Is.EqualTo("java script has run"));
      }
    }

    [Test]
    public void RunScriptAndEval()
    {
      using (IE ie = new IE())
      {
        ie.RunScript("var myVar = 5;");
        string result = ie.Eval("myVar;");
        Assert.AreEqual("5", result);
      }
    }

  }
}