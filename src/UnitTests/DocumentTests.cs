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
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Exceptions;
using WatiN.Core.Native;
using WatiN.Core.UnitTests.TestUtils;
using TimeoutException=WatiN.Core.Exceptions.TimeoutException;

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
		    // GIVEN
            var documentType = typeof(Document);
            
            // WHEN 
            var actual = documentType.GetInterface(typeof(IElementContainer).ToString());

            // THEN
		    Assert.That(actual, Is.Not.Null, "document should implement IElementsContainer");
		}

	    [Test]
		public void DocumentUrlandUri()
		{
		    ExecuteTest(browser =>
		                    {
		                        var uri = new Uri(browser.Url);
		                        Assert.AreEqual(MainURI, uri);
		                        Assert.AreEqual(browser.Uri, uri);
		                    });
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
		    ExecuteTest(browser =>
		                    {
		                        var documentVariableName = browser.NativeDocument.JavaScriptVariableName;
                                browser.RunScript(documentVariableName + ".getElementById('last').innerHTML = 'java script has run';");
                                Assert.That(browser.Div("last").Text, Is.EqualTo("java script has run"));

		                        try
		                        {
		                            browser.RunScript("a+1");
		                            Assert.Fail("Expected RunScriptException");
		                        }
		                        catch (RunScriptException)
		                        {
		                            // OK;
		                        }
		                    });
		}

		[Test]
		public void Text()
		{
            // GIVEN
		    var nativeDocumentMock = new Mock<INativeDocument>();
		    var nativeElementMock = new Mock<INativeElement>();
            nativeElementMock.Expect(x => x.IsElementReferenceStillValid()).Returns(true);
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
		    ExecuteTest(browser =>
            {
		                        var result = browser.Eval("2+5");
		                        Assert.That(result, Is.EqualTo("7"));

		                        result = browser.Eval("'te' + 'st'");
		                        Assert.That(result, Is.EqualTo("test"));


		                        try
		                        {
		                            browser.Eval("a+1");
		                            Assert.Fail("Expected JavaScriptException");
		                        }
		                        catch (JavaScriptException){}

		                        // Make sure a valid eval after a failed eval executes OK.
                                var documentVariableName = browser.NativeDocument.JavaScriptVariableName;
                                result = browser.Eval(documentVariableName + ".getElementById('last').innerHTML = 'java script has run';4+4;");

                                Assert.AreEqual("8", result);
                                Assert.That(browser.Div("last").Text, Is.EqualTo("java script has run"));

		                        browser.GoTo(TestPageUri);
		                    }
            );
		}


		[Test]
		public void RunScriptAndEval()
		{
		    ExecuteTest(browser =>
		                    {
		                        browser.RunScript("var myVar = 5;");
		                        var result = browser.Eval("myVar;");
		                        Assert.AreEqual("5", result);
		                    });
		}

        [Test]
        public void ContainsText()
        {
            ExecuteTest(browser =>
                            {
                                Assert.IsTrue(browser.ContainsText("Contains text in DIV"), "Text not found");
                                Assert.IsFalse(browser.ContainsText("abcde"), "Text incorrectly found");

                                Assert.IsTrue(browser.ContainsText(new Regex("Contains text in DIV")), "Regex: Text not found");
                                Assert.IsFalse(browser.ContainsText(new Regex("abcde")), "Regex: Text incorrectly found");
                            });
        }

        [Test]
        public void FindText()
        {
            ExecuteTest(browser =>
                            {
                                Assert.AreEqual("Contains text in DIV", browser.FindText(new Regex("Contains .* in DIV")), "Text not found");
                                Assert.IsNull(browser.FindText(new Regex("abcde")), "Text incorrectly found");
                            });
        }

        [Test] //, ExpectedException(typeof(Exceptions.TimeoutException))]
        public void WaitUntilContainsTextShouldThrowTimeOutException()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                Settings.WaitUntilExistsTimeOut = 1;
                                HtmlInjector.Start(browser, "some text 1", 2);

                                try
                                {
                                    browser.WaitUntilContainsText("some text 1");

                                    // THEN
                                    Assert.Fail("Expected " + typeof(TimeoutException));
                                }
                                catch (Exception e)
                                {
                                    Assert.That(e, Is.InstanceOfType(typeof(TimeoutException)));
                                }
                                finally
                                {
                                    browser.GoTo(TestPageUri);
                                }
                            });
        }

        [Test]
        public void WaitUntilContainsTextShouldReturn()
        {
            ExecuteTest(browser =>
                            {
                                Settings.WaitUntilExistsTimeOut = 2;

                                HtmlInjector.Start(browser, "some text 2", 1);
                                browser.WaitUntilContainsText("some text 2");
                                browser.GoTo(TestPageUri);
                            });
        }

        [Test] //, ExpectedException(typeof(Exceptions.TimeoutException))]
        public void WaitUntilContainsTextRegexShouldThrowTimeOutException()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                Settings.WaitUntilExistsTimeOut = 1;
                                HtmlInjector.Start(browser, "some text 3", 2);

                                // WHEN

                                try
                                {
                                    browser.WaitUntilContainsText(new Regex("me text 3"));
                                    
                                    // THEN
                                    Assert.Fail("Expected " + typeof(TimeoutException));
                                }
                                catch (Exception e)
                                {
                                    Assert.That(e, Is.InstanceOfType(typeof(TimeoutException)));
                                }
                                finally
                                {
                                    browser.GoTo(TestPageUri);
                                }

                            });
        }

        [Test]
        public void WaitUntilContainsTextRegexShouldReturn()
        {
            Settings.WaitUntilExistsTimeOut = 2;

            ExecuteTest(browser =>
                            {
                                HtmlInjector.Start(browser, "some text 4", 1);
                                browser.WaitUntilContainsText(new Regex("me text 4"));
                                browser.GoTo(TestPageUri);
                            });
        }

        [Test, Ignore("TODO")]
        public void TestAreaPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Area = browser.Area(t => t.Id == "readonlytext");

                                Assert.That(Area.Id, Is.EqualTo("readonlytext"));
                            });
        }

        [Test]
        public void TestButtonPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Button = browser.Button(t => t.Id == "popupid");

                                Assert.That(Button.Id, Is.EqualTo("popupid"));
                            });
        }

        [Test]
        public void TestCheckBoxPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var CheckBox = browser.CheckBox(t => t.Id == "Checkbox2");

                                Assert.That(CheckBox.Id, Is.EqualTo("Checkbox2"));
                            });
        }

        [Test]
        public void TestElementPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Element = browser.Element(t => t.Id == "Radio1");

                                Assert.That(Element.Id, Is.EqualTo("Radio1"));
                            });
        }

        [Test]
        public void TestFileUploadPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var FileUpload = browser.FileUpload(t => t.Id == "upload");

                                Assert.That(FileUpload.Id, Is.EqualTo("upload"));
                            });
        }

        [Test]
        public void TestFormPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Form = browser.Form(t => t.Id == "Form");

                                Assert.That(Form.Id, Is.EqualTo("Form"));
                            });
        }

        [Test]
        public void TestLabelPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Label = browser.Label(t => t.For == "Checkbox21");

                                Assert.That(Label.For, Is.EqualTo("Checkbox21"));
                            });
        }

        [Test]
        public void TestLinkPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Link = browser.Link(t => t.Id == "testlinkid");

                                Assert.That(Link.Id, Is.EqualTo("testlinkid"));
                            });
        }

        [Test]
        public void TestParaPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Para = browser.Para(t => t.Id == "links");

                                Assert.That(Para.Id, Is.EqualTo("links"));
                            });
        }

        [Test]
        public void TestRadioButtonPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var RadioButton = browser.RadioButton(t => t.Id == "Radio1");

                                Assert.That(RadioButton.Id, Is.EqualTo("Radio1"));
                            });
        }

        [Test]
        public void TestSelectListPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var SelectList = browser.SelectList(t => t.Id == "Select1");

                                Assert.That(SelectList.Id, Is.EqualTo("Select1"));
                            });
        }

        [Test]
        public void TestTablePredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Table = browser.Table(t => t.Id == "table1");

                                Assert.That(Table.Id, Is.EqualTo("table1"));
                            });
        }

        [Test]
        public void TestTableCellPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var TableCell = browser.TableCell(t => t.Id == "td2");

                                Assert.That(TableCell.Id, Is.EqualTo("td2"));
                            });
        }

        [Test]
        public void TestTableRowPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var TableRow = browser.TableRow(t => t.Id == "row0");

                                Assert.That(TableRow.Id, Is.EqualTo("row0"));
                            });
        }

        [Test, Ignore("TODO")]
        public void TestTableBodyPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var TableBody = browser.TableBody(t => t.Id == "readonlytext");

                                Assert.That(TableBody.Id, Is.EqualTo("readonlytext"));
                            });
        }

        [Test]
        public void TestTextFieldPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var textField = browser.TextField(t => t.Id == "readonlytext");

                                Assert.That(textField.Id, Is.EqualTo("readonlytext"));
                            });
        }

        [Test]
        public void TestSpanPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Span = browser.Span(t => t.Id == "Span1");

                                Assert.That(Span.Id, Is.EqualTo("Span1"));
                            });
        }

        [Test]
        public void TestDivPredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Div = browser.Div(t => t.Id == "NextAndPreviousTests");

                                Assert.That(Div.Id, Is.EqualTo("NextAndPreviousTests"));
                            });
        }

        [Test, Ignore("TODO")]
        public void TestImagePredicateOverload()
        {
            ExecuteTest(browser =>
                            {
                                var Image = browser.Image(t => t.Id == "readonlytext");

                                Assert.That(Image.Id, Is.EqualTo("readonlytext"));
                            });
        }
    
        [Test, Ignore("but it doesn't")]
        public void FFelementInnerHtmlToInnerTextShouldBeWorkingForThisAsWell()
        {
            // GIVEN
            var ieText = Ie.Text;

            // WHEN
            var ffText = Firefox.Text;

            // THEN
            Assert.That(ffText, Is.EqualTo(ieText));
        }

        [Test]
        public void TextShouldStartWith()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                var newlineSpaces = new Regex("[\n\r ]*");

                                // WHEN
                                var text = browser.Text;
                                // remove all newlines and spaces.... browser differences
                                text = newlineSpaces.Replace(text, "");

                                // THEN
                                Assert.That(text, NUnit.Framework.SyntaxHelpers.Text.StartsWith("col1col2a1a2b1b2"));
                            });
        }

        [Test]
        public void HtmlShouldStartWithBodyTag()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                // WHEN
                                var outerHtml = browser.Html.ToLowerInvariant();

                                // THEN
                                Assert.That(outerHtml, NUnit.Framework.SyntaxHelpers.Text.StartsWith("\r\n<body>"));
                            });
        }
    }

    public class TestDocument : Document
    {
        private readonly INativeDocument document;

        public TestDocument(DomContainer container, INativeDocument document)
            : base(container)
        {
            this.document = document;
        }

        public override INativeDocument NativeDocument
        {
            get { return document; }
        }

        protected override string GetAttributeValueImpl(string attributeName)
        {
            throw new NotImplementedException();
        }
    }
}
