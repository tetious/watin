using System;
using Moq;
using mshtml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.InternetExplorer;

namespace WatiN.Core.UnitTests.IETests
{
    [TestFixture]
    public class IEDocumentTests : BaseWithBrowserTests
    {
        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ConstructorShouldThrowExceptionIfArgumentNotOfTypeIHTMLDocument()
        {
            new IEDocument(new object());
        }

        [Test]
        public void NativeDocumentShouldBeSetByConstructor()
        {
            // GIVEN
            var htmlDocument2Mock = new Mock<IHTMLDocument2>();
            var htmlDocument = htmlDocument2Mock.Object;
            var ieDocument = new IEDocument(htmlDocument);

            // WHEN
            var nativeDocument = ieDocument.Object;

            // THEN
            Assert.That(ReferenceEquals(nativeDocument, htmlDocument));
        }

        [Test]
        public void BodyShouldReturnAnIENativeElement()
        {
            // GIVEN
            var htmlElementMock = new Mock<IHTMLElement>();
            var htmlElement = htmlElementMock.Object;

            var htmlDocument2Mock = new Mock<IHTMLDocument2>();
            htmlDocument2Mock.Expect(document => document.body).Returns(htmlElement);
            var htmlDocument = htmlDocument2Mock.Object;
            
            var ieDocument = new IEDocument(htmlDocument);

            // WHEN
            var nativeElement = ieDocument.Body;

            // THEN
            Assert.That(nativeElement is IEElement, Is.True, "Unexpected type");
            Assert.That(ReferenceEquals(nativeElement.Object, htmlElement), "Unexpected instance");
        }

        [Test]
        public void UrlShouldReturnUrlOfCurrentDocument()
        {
            // GIVEN
            var ieDocument = new IEDocument(((SHDocVw.InternetExplorer)Ie.InternetExplorer).Document);

            // WHEN
            var url = ieDocument.Url;

            // THEN
            Assert.That(new Uri(url), Is.EqualTo(MainURI));
        }

        [Test]
        public void TitleShouldReturnTitleofThePage()
        {
            // GIVEN
            var ieDocument = new IEDocument(((SHDocVw.InternetExplorer)Ie.InternetExplorer).Document);

            // WHEN
            var title = ieDocument.Title;

            // THEN
            Assert.That(title, Is.EqualTo("Main"));
        }

        [Test] public void ActiveElementShouldReturnActiveElement()
        {
            // GIVEN
            Ie.CheckBox("Checkbox1").Focus();
            var ieDocument = new IEDocument(((SHDocVw.InternetExplorer)Ie.InternetExplorer).Document);

            // WHEN
            var activeElement = ieDocument.ActiveElement;

            // THEN
            Assert.That(activeElement.GetAttributeValue("id"), Is.EqualTo("Checkbox1"));

        }

        [Test]
        public void RunScriptShouldCallNativeDocumentProperty()
        {
            // GIVEN
            var htmlDocumentMock = new Mock<IHTMLDocument2>();
            var parentWindowMock = new Mock<IHTMLWindow2>();
            htmlDocumentMock.Expect(doc => doc.parentWindow).Returns(parentWindowMock.Object);
            var htmlDocument = htmlDocumentMock.Object;
            var ieDocument = new IEDocument(htmlDocument);

            // WHEN
            ieDocument.RunScript("Somescript", "javascript");

            // THEN
            parentWindowMock.Verify(window => window.execScript("Somescript", "javascript"));
        }
    }
}
