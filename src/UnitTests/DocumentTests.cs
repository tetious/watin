#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using System.Text.RegularExpressions;
using System.Threading;
using Moq;
using mshtml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class DocumentTests : BaseWithIETests
	{
		private Mock<IHTMLDocument2> _mockHtmlDocument;
		private Mock<IHTMLWindow2> _mockHtmlWindow2;
	    private int _originalWaitUntilExistsTimeOut;

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

        private void InitMocks() 
		{
			_mockHtmlDocument = new Mock<IHTMLDocument2>();
			_mockHtmlWindow2 = new Mock<IHTMLWindow2>();
			_originalWaitUntilExistsTimeOut = Settings.WaitUntilExistsTimeOut;
		}

		[TearDown]
	    public void TearDown()
	    {
	        Settings.WaitUntilExistsTimeOut = _originalWaitUntilExistsTimeOut;
	    }


		[Test]
        public void DocumentIsIElementsContainer()
		{
			Assert.IsInstanceOfType(typeof (IElementsContainer), ie);
		}

		[Test]
		public void DocumentUrlandUri()
		{
			var uri = new Uri(ie.Url);
			Assert.AreEqual(MainURI, uri);
			Assert.AreEqual(ie.Uri, uri);
		}

		[Test]
		public void RunScriptShouldCallHtmlDocumentProperty()
		{
			InitMocks();

			var mockDocument = new Mock<Document>();

			mockDocument.Expect(document => document.HtmlDocument).Returns(_mockHtmlDocument.Object).AtMostOnce();
			_mockHtmlDocument.Expect(htmldoc => htmldoc.parentWindow).Returns(_mockHtmlWindow2.Object).AtMostOnce();
			_mockHtmlWindow2.Expect(htmlwindow => htmlwindow.execScript("alert('hello')", "javascript")).Returns(null);

		    var document1 = mockDocument.Object;

		    document1.RunScript("alert('hello')");

			_mockHtmlDocument.VerifyAll();
			_mockHtmlWindow2.VerifyAll();
		}

		[Test]
		public void RunJavaScript()
		{
			ie.GoTo(IndexURI);

			ie.RunScript("window.document.write('java script has run');");
			Assert.That(ie.Text, Is.EqualTo("java script has run"));

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

		[Test]
		public void Text()
		{
			InitMocks();

			var _mockHTMLElement = new Mock<IHTMLElement>();
			var mockDocument = new Mock<Document>();

			mockDocument.Expect(doc => doc.HtmlDocument).Returns(_mockHtmlDocument.Object).AtMostOnce();
			_mockHtmlDocument.Expect(htmldoc => htmldoc.body).Returns(_mockHTMLElement.Object);
			_mockHTMLElement.Expect(element => element.innerText).Returns("Document innertext returned");

		    var document = mockDocument.Object;

		    Assert.That(document.Text, Is.EqualTo("Document innertext returned"));
		}

		[Test]
		public void TestEval()
		{
			var result = ie.Eval("2+5");
			Assert.That(result, Is.EqualTo("7"));

			result = ie.Eval("'te' + 'st'");
			Assert.That(result, Is.EqualTo("test"));


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
			Assert.That(ie.Text, Is.EqualTo("java script has run"));
		}

		[Test]
		public void RunScriptAndEval()
		{
			ie.RunScript("var myVar = 5;");
			var result = ie.Eval("myVar;");
			Assert.AreEqual("5", result);
		}

        [Test, ExpectedException(typeof(Exceptions.TimeoutException))]
        public void WaitUntilContainsTextShouldThrowTimeOutException()
        {
            Settings.WaitUntilExistsTimeOut = 1;

			HTMLInjector.Start(ie, "some text 1", 2);
            ie.WaitUntilContainsText("some text 1");
            ie.GoTo(TestPageUri);
        }

        [Test]
        public void WaitUntilContainsTextShouldReturn()
        {
            Settings.WaitUntilExistsTimeOut = 2;

			HTMLInjector.Start(ie, "some text 2", 1);
            ie.WaitUntilContainsText("some text 2");
            ie.GoTo(TestPageUri);
        }

        [Test, ExpectedException(typeof(Exceptions.TimeoutException))]
        public void WaitUntilContainsTextRegexShouldThrowTimeOutException()
        {
            Settings.WaitUntilExistsTimeOut = 1;

			HTMLInjector.Start(ie, "some text 3", 2);
            ie.WaitUntilContainsText(new Regex("me text 3"));
            ie.GoTo(TestPageUri);
        }

        [Test]
        public void WaitUntilContainsTextRegexShouldReturn()
        {
            Settings.WaitUntilExistsTimeOut = 2;

			HTMLInjector.Start(ie, "some text 4", 1);
            ie.WaitUntilContainsText(new Regex("me text 4"));
            ie.GoTo(TestPageUri);
        }

        [Test, Ignore("TODO")]
        public void TestAreaPredicateOverload()
        {
            //            Area Area = ie.Area(t => t.Name == "q");
            var Area = ie.Area(t => t.Id == "readonlytext");

            Assert.That(Area.Id, Is.EqualTo("readonlytext"));
        }

        [Test]
        public void TestButtonPredicateOverload()
        {
            var Button = ie.Button(t => t.Id == "popupid");

            Assert.That(Button.Id, Is.EqualTo("popupid"));
        }

        [Test]
        public void TestCheckBoxPredicateOverload()
        {
            var CheckBox = ie.CheckBox(t => t.Id == "Checkbox2");

            Assert.That(CheckBox.Id, Is.EqualTo("Checkbox2"));
        }

        [Test]
        public void TestElementPredicateOverload()
        {
            var Element = ie.Element(t => t.Id == "Radio1");

            Assert.That(Element.Id, Is.EqualTo("Radio1"));
        }

        [Test]
        public void TestFileUploadPredicateOverload()
        {
            var FileUpload = ie.FileUpload(t => t.Id == "upload");

            Assert.That(FileUpload.Id, Is.EqualTo("upload"));
        }

        [Test]
        public void TestFormPredicateOverload()
        {
            var Form = ie.Form(t => t.Id == "Form");

            Assert.That(Form.Id, Is.EqualTo("Form"));
        }

        [Test]
        public void TestLabelPredicateOverload()
        {
            var Label = ie.Label(t => t.For == "Checkbox21");

            Assert.That(Label.For, Is.EqualTo("Checkbox21"));
        }

        [Test]
        public void TestLinkPredicateOverload()
        {
            var Link = ie.Link(t => t.Id == "testlinkid");

            Assert.That(Link.Id, Is.EqualTo("testlinkid"));
        }

        [Test]
        public void TestParaPredicateOverload()
        {
            var Para = ie.Para(t => t.Id == "links");

            Assert.That(Para.Id, Is.EqualTo("links"));
        }

        [Test]
        public void TestRadioButtonPredicateOverload()
        {
            var RadioButton = ie.RadioButton(t => t.Id == "Radio1");

            Assert.That(RadioButton.Id, Is.EqualTo("Radio1"));
        }

        [Test]
        public void TestSelectListPredicateOverload()
        {
            var SelectList = ie.SelectList(t => t.Id == "Select1");

            Assert.That(SelectList.Id, Is.EqualTo("Select1"));
        }

        [Test]
        public void TestTablePredicateOverload()
        {
            var Table = ie.Table(t => t.Id == "table1");

            Assert.That(Table.Id, Is.EqualTo("table1"));
        }

        [Test]
        public void TestTableCellPredicateOverload()
        {
            var TableCell = ie.TableCell(t => t.Id == "td2");

            Assert.That(TableCell.Id, Is.EqualTo("td2"));
        }

        [Test]
        public void TestTableRowPredicateOverload()
        {
            var TableRow = ie.TableRow(t => t.Id == "row0");

            Assert.That(TableRow.Id, Is.EqualTo("row0"));
        }

        [Test, Ignore("TODO")]
        public void TestTableBodyPredicateOverload()
        {
            var TableBody = ie.TableBody(t => t.Id == "readonlytext");

            Assert.That(TableBody.Id, Is.EqualTo("readonlytext"));
        }

        [Test]
        public void TestTextFieldPredicateOverload()
        {
            var textField = ie.TextField(t => t.Id == "readonlytext");

            Assert.That(textField.Id, Is.EqualTo("readonlytext"));
        }

        [Test]
        public void TestSpanPredicateOverload()
        {
            var Span = ie.Span(t => t.Id == "Span1");

            Assert.That(Span.Id, Is.EqualTo("Span1"));
        }

        [Test]
        public void TestDivPredicateOverload()
        {
            var Div = ie.Div(t => t.Id == "NextAndPreviousTests");

            Assert.That(Div.Id, Is.EqualTo("NextAndPreviousTests"));
        }

        [Test, Ignore("TODO")]
        public void TestImagePredicateOverload()
        {
            var Image = ie.Image(t => t.Id == "readonlytext");

            Assert.That(Image.Id, Is.EqualTo("readonlytext"));
        }
	}

    internal class HTMLInjector
    {
        private readonly string _html;
        private readonly int _numberOfSecondsToWaitBeforeInjection;
        private readonly Document _document;

        public HTMLInjector(Document document, string html, int numberOfSecondsToWaitBeforeInjection)
        {
            _document = document;
            _html = html;
            _numberOfSecondsToWaitBeforeInjection = numberOfSecondsToWaitBeforeInjection;
        }

        public void Inject()
        {
            Thread.Sleep(_numberOfSecondsToWaitBeforeInjection * 1000);

            try
            {
                _document.HtmlDocument.writeln(_html);
            }
            catch { }
        }

        /// <summary>
        /// Starts a new thread and injects the html into the document after numberOfSecondsToWaitBeforeInjection.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="html"></param>
        /// <param name="numberOfSecondsToWaitBeforeInjection"></param>
        public static void Start(Document document, string html, int numberOfSecondsToWaitBeforeInjection)
        {
            var htmlInjector = new HTMLInjector(document, html, numberOfSecondsToWaitBeforeInjection);

            ThreadStart start = htmlInjector.Inject;
            var thread = new Thread(start);
            thread.Start();
        }

    }
}