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
using mshtml;
using NUnit.Framework;
using Iz = NUnit.Framework.SyntaxHelpers.Is;
using Rhino.Mocks;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class DocumentTests : BaseWithIETests
	{
		private MockRepository _mockRepository;
		private IHTMLDocument2 _mockHtmlDocument;
		private IHTMLWindow2 _mockHtmlWindow2;
	    private int _originalWaitUntilExistsTimeOut;

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

        private void InitMocks() 
		{
			_mockRepository = new MockRepository();
			_mockHtmlDocument = (IHTMLDocument2) _mockRepository.CreateMock(typeof (IHTMLDocument2));
			_mockHtmlWindow2 = (IHTMLWindow2) _mockRepository.CreateMock(typeof (IHTMLWindow2));
			_originalWaitUntilExistsTimeOut = Settings.Instance.WaitUntilExistsTimeOut;
		}

		[TearDown]
	    public void TearDown()
	    {
	        Settings.Instance.WaitUntilExistsTimeOut = _originalWaitUntilExistsTimeOut;
	    }


		[Test]
        public void DocumentIsIElementsContainer()
		{
			Assert.IsInstanceOfType(typeof (IElementsContainer), ie);
		}

		[Test]
		public void DocumentUrlandUri()
		{
			Uri uri = new Uri(ie.Url);
			Assert.AreEqual(MainURI, uri);
			Assert.AreEqual(ie.Uri, uri);
		}

		[Test]
		public void RunScriptShouldCallHtmlDocumentProperty()
		{
			InitMocks();

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
			ie.GoTo(IndexURI);

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

		[Test]
		public void Text()
		{
			InitMocks();

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

		[Test]
		public void RunScriptAndEval()
		{
			ie.RunScript("var myVar = 5;");
			string result = ie.Eval("myVar;");
			Assert.AreEqual("5", result);
		}

        [Test, ExpectedException(typeof(WatiN.Core.Exceptions.TimeoutException))]
        public void WaitUntilContainsTextShouldThrowTimeOutException()
        {
            Settings.Instance.WaitUntilExistsTimeOut = 1;

			HTMLInjector.Start(ie, "some text 1", 2);
            ie.WaitUntilContainsText("some text 1");
            ie.GoTo(TestPageUri);
        }

        [Test]
        public void WaitUntilContainsTextShouldReturn()
        {
            Settings.Instance.WaitUntilExistsTimeOut = 2;

			HTMLInjector.Start(ie, "some text 2", 1);
            ie.WaitUntilContainsText("some text 2");
            ie.GoTo(TestPageUri);
        }

        [Test, ExpectedException(typeof(WatiN.Core.Exceptions.TimeoutException))]
        public void WaitUntilContainsTextRegexShouldThrowTimeOutException()
        {
            Settings.Instance.WaitUntilExistsTimeOut = 1;

			HTMLInjector.Start(ie, "some text 3", 2);
            ie.WaitUntilContainsText(new Regex("me text 3"));
            ie.GoTo(TestPageUri);
        }

        [Test]
        public void WaitUntilContainsTextRegexShouldReturn()
        {
            Settings.Instance.WaitUntilExistsTimeOut = 2;

			HTMLInjector.Start(ie, "some text 4", 1);
            ie.WaitUntilContainsText(new Regex("me text 4"));
            ie.GoTo(TestPageUri);
        }

#if !NET11
        [Test, Ignore("TODO")]
        public void TestAreaPredicateOverload()
        {
            //            Area Area = ie.Area(t => t.Name == "q");
            Area Area = ie.Area(delegate(Area t) { return t.Id == "readonlytext"; });

            Assert.That(Area.Id, Iz.EqualTo("readonlytext"));
        }

        [Test]
        public void TestButtonPredicateOverload()
        {
            //            Button Button = ie.Button(t => t.Name == "q");
            Button Button = ie.Button(delegate(Button t) { return t.Id == "popupid"; });

            Assert.That(Button.Id, Iz.EqualTo("popupid"));
        }

        [Test]
        public void TestCheckBoxPredicateOverload()
        {
            //            CheckBox CheckBox = ie.CheckBox(t => t.Name == "q");
            CheckBox CheckBox = ie.CheckBox(delegate(CheckBox t) { return t.Id == "Checkbox2"; });

            Assert.That(CheckBox.Id, Iz.EqualTo("Checkbox2"));
        }

        [Test]
        public void TestElementPredicateOverload()
        {
            //            Element Element = ie.Element(t => t.Name == "q");
            Element Element = ie.Element(delegate(Element t) { return t.Id == "Radio1"; });

            Assert.That(Element.Id, Iz.EqualTo("Radio1"));
        }

        [Test]
        public void TestFileUploadPredicateOverload()
        {
            //            FileUpload FileUpload = ie.FileUpload(t => t.Name == "q");
            FileUpload FileUpload = ie.FileUpload(delegate(FileUpload t) { return t.Id == "upload"; });

            Assert.That(FileUpload.Id, Iz.EqualTo("upload"));
        }

        [Test]
        public void TestFormPredicateOverload()
        {
            //            Form Form = ie.Form(t => t.Name == "q");
            Form Form = ie.Form(delegate(Form t) { return t.Id == "Form"; });

            Assert.That(Form.Id, Iz.EqualTo("Form"));
        }

        [Test]
        public void TestLabelPredicateOverload()
        {
            //            Label Label = ie.Label(t => t.Name == "q");
            Label Label = ie.Label(delegate(Label t) { return t.For == "Checkbox21"; });

            Assert.That(Label.For, Iz.EqualTo("Checkbox21"));
        }

        [Test]
        public void TestLinkPredicateOverload()
        {
            //            Link Link = ie.Link(t => t.Name == "q");
            Link Link = ie.Link(delegate(Link t) { return t.Id == "testlinkid"; });

            Assert.That(Link.Id, Iz.EqualTo("testlinkid"));
        }

        [Test]
        public void TestParaPredicateOverload()
        {
            //            Para Para = ie.Para(t => t.Name == "q");
            Para Para = ie.Para(delegate(Para t) { return t.Id == "links"; });

            Assert.That(Para.Id, Iz.EqualTo("links"));
        }

        [Test]
        public void TestRadioButtonPredicateOverload()
        {
            //            RadioButton RadioButton = ie.RadioButton(t => t.Name == "q");
            RadioButton RadioButton = ie.RadioButton(delegate(RadioButton t) { return t.Id == "Radio1"; });

            Assert.That(RadioButton.Id, Iz.EqualTo("Radio1"));
        }

        [Test]
        public void TestSelectListPredicateOverload()
        {
            //            SelectList SelectList = ie.SelectList(t => t.Name == "q");
            SelectList SelectList = ie.SelectList(delegate(SelectList t) { return t.Id == "Select1"; });

            Assert.That(SelectList.Id, Iz.EqualTo("Select1"));
        }

        [Test]
        public void TestTablePredicateOverload()
        {
            //            Table Table = ie.Table(t => t.Name == "q");
            Table Table = ie.Table(delegate(Table t) { return t.Id == "table1"; });

            Assert.That(Table.Id, Iz.EqualTo("table1"));
        }

        [Test]
        public void TestTableCellPredicateOverload()
        {
            //            TableCell TableCell = ie.TableCell(t => t.Name == "q");
            TableCell TableCell = ie.TableCell(delegate(TableCell t) { return t.Id == "td2"; });

            Assert.That(TableCell.Id, Iz.EqualTo("td2"));
        }

        [Test]
        public void TestTableRowPredicateOverload()
        {
            //            TableRow TableRow = ie.TableRow(t => t.Name == "q");
            TableRow TableRow = ie.TableRow(delegate(TableRow t) { return t.Id == "row0"; });

            Assert.That(TableRow.Id, Iz.EqualTo("row0"));
        }

        [Test, Ignore("TODO")]
        public void TestTableBodyPredicateOverload()
        {
            //            TableBody TableBody = ie.TableBody(t => t.Name == "q");
            TableBody TableBody = ie.TableBody(delegate(TableBody t) { return t.Id == "readonlytext"; });

            Assert.That(TableBody.Id, Iz.EqualTo("readonlytext"));
        }

        [Test]
        public void TestTextFieldPredicateOverload()
        {
            //            TextField textField = ie.TextField(t => t.Name == "q");
            TextField textField = ie.TextField(delegate(TextField t) { return t.Id == "readonlytext"; });

            Assert.That(textField.Id, Iz.EqualTo("readonlytext"));
        }

        [Test]
        public void TestSpanPredicateOverload()
        {
            //            Span Span = ie.Span(t => t.Name == "q");
            Span Span = ie.Span(delegate(Span t) { return t.Id == "Span1"; });

            Assert.That(Span.Id, Iz.EqualTo("Span1"));
        }

        [Test]
        public void TestDivPredicateOverload()
        {
            //            Div Div = ie.Div(t => t.Name == "q");
            Div Div = ie.Div(delegate(Div t) { return t.Id == "NextAndPreviousTests"; });

            Assert.That(Div.Id, Iz.EqualTo("NextAndPreviousTests"));
        }

        [Test, Ignore("TODO")]
        public void TestImagePredicateOverload()
        {
            //            Image Image = ie.Image(t => t.Name == "q");
            Image Image = ie.Image(delegate(Image t) { return t.Id == "readonlytext"; });

            Assert.That(Image.Id, Iz.EqualTo("readonlytext"));
        }

#endif 

	}

    internal class HTMLInjector
    {
        private string _html;
        private readonly int _numberOfSecondsToWaitBeforeInjection;
        private Document _document;

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
            HTMLInjector htmlInjector = new HTMLInjector(document, html, numberOfSecondsToWaitBeforeInjection);

            ThreadStart start = new ThreadStart(htmlInjector.Inject);
            Thread thread = new Thread(start);
            thread.Start();
        }

    }
}