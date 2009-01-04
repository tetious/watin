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
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.UnitTests.IETests;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class DocumentTests : BaseWithBrowserTests
	{
	    private int _originalWaitUntilExistsTimeOut;

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

        [SetUp]
        public override void TestSetUp() 
		{
            base.TestSetUp();
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
			Assert.IsInstanceOfType(typeof (IElementsContainer), Ie);
		}

		[Test]
		public void DocumentUrlandUri()
		{
			var uri = new Uri(Ie.Url);
			Assert.AreEqual(MainURI, uri);
			Assert.AreEqual(Ie.Uri, uri);
		}

		[Test]
		public void RunScriptShouldCallRunScriptOnNativeDocument()
		{
            // GIVEN
		    var domContainer = new Mock<DomContainer>().Object;
		    var nativeDocumentMock = new Mock<INativeDocument>();
		    var nativeDocument = nativeDocumentMock.Object;
		    var document = new TestDocument(domContainer, nativeDocument);

            var scriptCode = "alert('hello')";

		    // WHEN
		    document.RunScript(scriptCode);

            // THEN
            nativeDocumentMock.Verify(doc => doc.RunScript(scriptCode, "javascript"));
		}

		[Test]
		public void RunJavaScript()
		{
			Ie.GoTo(IndexURI);

			Ie.RunScript("window.document.write('java script has run');");
			Assert.That(Ie.Text, Is.EqualTo("java script has run"));

			try
			{
				Ie.RunScript("a+1");
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
            // GIVEN
		    var nativeDocumentMock = new Mock<INativeDocument>();
		    var nativeElementMock = new Mock<INativeElement>();
		    nativeElementMock.Expect(element => element.GetAttributeValue("innertext")).Returns("Something");
		    nativeDocumentMock.Expect(native => native.Body).Returns(nativeElementMock.Object);

		    var domContainer = new Mock<DomContainer>().Object;
		    var document = new TestDocument(domContainer, nativeDocumentMock.Object);

		    // WHEN
		    var text = document.Text;

            // THEN
		    Assert.That(text, Is.EqualTo("Something"));
		}

		[Test]
		public void TestEval()
		{
			var result = Ie.Eval("2+5");
			Assert.That(result, Is.EqualTo("7"));

			result = Ie.Eval("'te' + 'st'");
			Assert.That(result, Is.EqualTo("test"));


			try
			{
				Ie.Eval("a+1");
				Assert.Fail("Expected JavaScriptException");
			}
			catch (JavaScriptException)
			{
				//;
			}

			// Make sure a valid eval after a failed eval executes OK.
			result = Ie.Eval("window.document.write('java script has run');4+4;");
			Assert.AreEqual("8", result);
			Assert.That(Ie.Text, Is.EqualTo("java script has run"));

		    Ie.GoTo(TestPageUri);
		}

		[Test]
		public void RunScriptAndEval()
		{
			Ie.RunScript("var myVar = 5;");
			var result = Ie.Eval("myVar;");
			Assert.AreEqual("5", result);
		}

        [Test]
        public void ContainsText()
        {
            Assert.IsTrue(Ie.ContainsText("Contains text in DIV"), "Text not found");
            Assert.IsFalse(Ie.ContainsText("abcde"), "Text incorrectly found");

            Assert.IsTrue(Ie.ContainsText(new Regex("Contains text in DIV")), "Regex: Text not found");
            Assert.IsFalse(Ie.ContainsText(new Regex("abcde")), "Regex: Text incorrectly found");
        }

        [Test]
        public void FindText()
        {
            Assert.AreEqual("Contains text in DIV", Ie.FindText(new Regex("Contains .* in DIV")), "Text not found");
            Assert.IsNull(Ie.FindText(new Regex("abcde")), "Text incorrectly found");
        }

        [Test, ExpectedException(typeof(Exceptions.TimeoutException))]
        public void WaitUntilContainsTextShouldThrowTimeOutException()
        {
            Settings.WaitUntilExistsTimeOut = 1;

			IEHtmlInjector.Start(Ie, "some text 1", 2);
            Ie.WaitUntilContainsText("some text 1");
            Ie.GoTo(TestPageUri);
        }

        [Test]
        public void WaitUntilContainsTextShouldReturn()
        {
            Settings.WaitUntilExistsTimeOut = 2;

			IEHtmlInjector.Start(Ie, "some text 2", 1);
            Ie.WaitUntilContainsText("some text 2");
            Ie.GoTo(TestPageUri);
        }

        [Test, ExpectedException(typeof(Exceptions.TimeoutException))]
        public void WaitUntilContainsTextRegexShouldThrowTimeOutException()
        {
            Settings.WaitUntilExistsTimeOut = 1;

			IEHtmlInjector.Start(Ie, "some text 3", 2);
            Ie.WaitUntilContainsText(new Regex("me text 3"));
            Ie.GoTo(TestPageUri);
        }

        [Test]
        public void WaitUntilContainsTextRegexShouldReturn()
        {
            Settings.WaitUntilExistsTimeOut = 2;

			IEHtmlInjector.Start(Ie, "some text 4", 1);
            Ie.WaitUntilContainsText(new Regex("me text 4"));
            Ie.GoTo(TestPageUri);
        }

        [Test, Ignore("TODO")]
        public void TestAreaPredicateOverload()
        {
            //            Area Area = ie.Area(t => t.Name == "q");
            var Area = Ie.Area(t => t.Id == "readonlytext");

            Assert.That(Area.Id, Is.EqualTo("readonlytext"));
        }

        [Test]
        public void TestButtonPredicateOverload()
        {
            var Button = Ie.Button(t => t.Id == "popupid");

            Assert.That(Button.Id, Is.EqualTo("popupid"));
        }

        [Test]
        public void TestCheckBoxPredicateOverload()
        {
            var CheckBox = Ie.CheckBox(t => t.Id == "Checkbox2");

            Assert.That(CheckBox.Id, Is.EqualTo("Checkbox2"));
        }

        [Test]
        public void TestElementPredicateOverload()
        {
            var Element = Ie.Element(t => t.Id == "Radio1");

            Assert.That(Element.Id, Is.EqualTo("Radio1"));
        }

        [Test]
        public void TestFileUploadPredicateOverload()
        {
            var FileUpload = Ie.FileUpload(t => t.Id == "upload");

            Assert.That(FileUpload.Id, Is.EqualTo("upload"));
        }

        [Test]
        public void TestFormPredicateOverload()
        {
            var Form = Ie.Form(t => t.Id == "Form");

            Assert.That(Form.Id, Is.EqualTo("Form"));
        }

        [Test]
        public void TestLabelPredicateOverload()
        {
            var Label = Ie.Label(t => t.For == "Checkbox21");

            Assert.That(Label.For, Is.EqualTo("Checkbox21"));
        }

        [Test]
        public void TestLinkPredicateOverload()
        {
            var Link = Ie.Link(t => t.Id == "testlinkid");

            Assert.That(Link.Id, Is.EqualTo("testlinkid"));
        }

        [Test]
        public void TestParaPredicateOverload()
        {
            var Para = Ie.Para(t => t.Id == "links");

            Assert.That(Para.Id, Is.EqualTo("links"));
        }

        [Test]
        public void TestRadioButtonPredicateOverload()
        {
            var RadioButton = Ie.RadioButton(t => t.Id == "Radio1");

            Assert.That(RadioButton.Id, Is.EqualTo("Radio1"));
        }

        [Test]
        public void TestSelectListPredicateOverload()
        {
            var SelectList = Ie.SelectList(t => t.Id == "Select1");

            Assert.That(SelectList.Id, Is.EqualTo("Select1"));
        }

        [Test]
        public void TestTablePredicateOverload()
        {
            var Table = Ie.Table(t => t.Id == "table1");

            Assert.That(Table.Id, Is.EqualTo("table1"));
        }

        [Test]
        public void TestTableCellPredicateOverload()
        {
            var TableCell = Ie.TableCell(t => t.Id == "td2");

            Assert.That(TableCell.Id, Is.EqualTo("td2"));
        }

        [Test]
        public void TestTableRowPredicateOverload()
        {
            var TableRow = Ie.TableRow(t => t.Id == "row0");

            Assert.That(TableRow.Id, Is.EqualTo("row0"));
        }

        [Test, Ignore("TODO")]
        public void TestTableBodyPredicateOverload()
        {
            var TableBody = Ie.TableBody(t => t.Id == "readonlytext");

            Assert.That(TableBody.Id, Is.EqualTo("readonlytext"));
        }

        [Test]
        public void TestTextFieldPredicateOverload()
        {
            var textField = Ie.TextField(t => t.Id == "readonlytext");

            Assert.That(textField.Id, Is.EqualTo("readonlytext"));
        }

        [Test]
        public void TestSpanPredicateOverload()
        {
            var Span = Ie.Span(t => t.Id == "Span1");

            Assert.That(Span.Id, Is.EqualTo("Span1"));
        }

        [Test]
        public void TestDivPredicateOverload()
        {
            var Div = Ie.Div(t => t.Id == "NextAndPreviousTests");

            Assert.That(Div.Id, Is.EqualTo("NextAndPreviousTests"));
        }

        [Test, Ignore("TODO")]
        public void TestImagePredicateOverload()
        {
            var Image = Ie.Image(t => t.Id == "readonlytext");

            Assert.That(Image.Id, Is.EqualTo("readonlytext"));
        }
	}

    public class TestDocument : Document
    {
        public TestDocument(DomContainer container, INativeDocument document) : base(container, document) {}
    }
}