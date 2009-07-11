#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using Moq;
using mshtml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.IETests
{
    [TestFixture]
    public class IEDocumentTests : BaseWithBrowserTests
    {
        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        [Test]
        public void NativeDocumentShouldBeSetByConstructor()
        {
            // GIVEN
            var htmlDocument2Mock = new Mock<IHTMLDocument2>();
            var htmlDocument = htmlDocument2Mock.Object;
            var ieDocument = new IEDocument(htmlDocument);

            // WHEN
            var nativeDocument = ieDocument.HtmlDocument;

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
            IEElement ieElement = (IEElement)nativeElement;
            Assert.That(ReferenceEquals(ieElement.AsHtmlElement, htmlElement), "Unexpected instance");
        }

        [Test]
        public void UrlShouldReturnUrlOfCurrentDocument()
        {
            // GIVEN
            var ieDocument = new IEDocument((IHTMLDocument2)((SHDocVw.IWebBrowser2)Ie.InternetExplorer).Document);

            // WHEN
            var url = ieDocument.Url;

            // THEN
            Assert.That(new Uri(url), Is.EqualTo(MainURI));
        }

        [Test]
        public void TitleShouldReturnTitleofThePage()
        {
            // GIVEN
            var ieDocument = new IEDocument((IHTMLDocument2)((SHDocVw.IWebBrowser2)Ie.InternetExplorer).Document);

            // WHEN
            var title = ieDocument.Title;

            // THEN
            Assert.That(title, Is.EqualTo("Main"));
        }

        [Test] public void ActiveElementShouldReturnActiveElement()
        {
            // GIVEN
            Ie.CheckBox("Checkbox1").Focus();
            var ieDocument = new IEDocument((IHTMLDocument2)((SHDocVw.IWebBrowser2)Ie.InternetExplorer).Document);

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
