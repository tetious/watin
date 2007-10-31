#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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

using System.Text.RegularExpressions;
using System.Threading;
using mshtml;
using NUnit.Framework;
using Rhino.Mocks;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class DocumentTests : WatiNTest
	{
		private MockRepository _mockRepository;
		private IHTMLDocument2 _mockHtmlDocument;
		private IHTMLWindow2 _mockHtmlWindow2;
	    private int _originalWaitUntilExistsTimeOut;

		[SetUp]
		public void Setup()
		{
			_mockRepository = new MockRepository();
			_mockHtmlDocument = (IHTMLDocument2) _mockRepository.CreateMock(typeof (IHTMLDocument2));
			_mockHtmlWindow2 = (IHTMLWindow2) _mockRepository.CreateMock(typeof (IHTMLWindow2));
            _originalWaitUntilExistsTimeOut = IE.Settings.WaitUntilExistsTimeOut;
		}

	    [TearDown]
	    public void TearDown()
	    {
	        IE.Settings.WaitUntilExistsTimeOut = _originalWaitUntilExistsTimeOut;
	    }


		[Test]
        public void DocumentIsIElementsContainer()
		{
			using (IE ie = new IE())
			{
				Assert.IsInstanceOfType(typeof (IElementsContainer), ie);
			}
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

        [Test, ExpectedException(typeof(TimeoutException))]
        public void WaitUntilContainsTextShouldThrowTimeOutException()
        {
            IE.Settings.WaitUntilExistsTimeOut = 1;
            using(IE ie = new IE())
            {
                HTMLInjector.Start(ie, "some text", 2);
                ie.WaitUntilContainsText("some text");
            }
        }

        [Test]
        public void WaitUntilContainsTextShouldReturn()
        {
            IE.Settings.WaitUntilExistsTimeOut = 2;
            using(IE ie = new IE())
            {
                HTMLInjector.Start(ie, "some text", 1);
                ie.WaitUntilContainsText("some text");
            }
        }

        [Test, ExpectedException(typeof(TimeoutException))]
        public void WaitUntilContainsTextRegexShouldThrowTimeOutException()
        {
            IE.Settings.WaitUntilExistsTimeOut = 1;
            using(IE ie = new IE())
            {
                HTMLInjector.Start(ie, "some text", 2);
                ie.WaitUntilContainsText(new Regex("me te"));
            }
        }

        [Test]
        public void WaitUntilContainsTextRegexShouldReturn()
        {
            IE.Settings.WaitUntilExistsTimeOut = 2;
            using(IE ie = new IE())
            {
                HTMLInjector.Start(ie, "some text", 1);
                ie.WaitUntilContainsText(new Regex("me te"));
            }
        }
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