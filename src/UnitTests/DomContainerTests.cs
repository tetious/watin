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

using System.Text.RegularExpressions;
using mshtml;
using NUnit.Framework;
using Rhino.Mocks;
using SHDocVw;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
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
			IE.Settings.AutoStartDialogWatcher = false;
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
			_mockWait = (IWait) _mockRepository.CreateMock(typeof (IWait));
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
	    public void WaitForCompleteShouldUseTimeOutProvidedThroughtTheConstructor()
	    {
	        WaitForCompleteMock waitForCompleteMock = new WaitForCompleteMock(_ie, 333);

            Assert.That(waitForCompleteMock, NUnit.Framework.SyntaxHelpers.Is.InstanceOfType(typeof(WaitForComplete)),"Should inherit WaitForComplete");

            waitForCompleteMock.DoWait();
            
            Assert.That(waitForCompleteMock.Timeout, NUnit.Framework.SyntaxHelpers.Is.EqualTo(333), "Unexpected timeout");
	    }

	    [Test]
	    public void WaitForCompleteShouldUseWaitForCompleteTimeOutSetting()
	    {
	        int expectedWaitForCompleteTimeOut = IE.Settings.WaitForCompleteTimeOut;

	        WaitForCompleteMock waitForCompleteMock = new WaitForCompleteMock(_ie);

            Assert.That(waitForCompleteMock, NUnit.Framework.SyntaxHelpers.Is.InstanceOfType(typeof(WaitForComplete)),"Should inherit WaitForComplete");

            waitForCompleteMock.DoWait();

            Assert.That(waitForCompleteMock.Timeout, NUnit.Framework.SyntaxHelpers.Is.EqualTo(expectedWaitForCompleteTimeOut), "Unexpected timeout");
	    }

	    [Test]
		public void DomContainerIsDocument()
		{
			Assert.IsInstanceOfType(typeof (Document), _ie);
		}

		[Test]
		public void Text()
		{
			Assert.IsTrue(_ie.Text.IndexOf("Contains text in DIV") >= 0, "Text property did not return expected contents.");
		}

		[Test]
		public void ContainsText()
		{
			Assert.IsTrue(_ie.ContainsText("Contains text in DIV"), "Text not found");
			Assert.IsFalse(_ie.ContainsText("abcde"), "Text incorrectly found");

			Assert.IsTrue(_ie.ContainsText(new Regex("Contains text in DIV")), "Regex: Text not found");
			Assert.IsFalse(_ie.ContainsText(new Regex("abcde")), "Regex: Text incorrectly found");
		}

		[Test]
		public void FindText()
		{
			Assert.AreEqual("Contains text in DIV", _ie.FindText(new Regex("Contains .* in DIV")), "Text not found");
			Assert.IsNull(_ie.FindText(new Regex("abcde")), "Text incorrectly found");
		}

		[TearDown]
		public virtual void TearDown()
		{
			_ie.Dispose();
			IE.Settings.Reset();
		}
	}

    public class WaitForCompleteMock : WaitForComplete
    {
        private int _timeout;

        public WaitForCompleteMock(DomContainer domContainer) : base(domContainer) {}

        public WaitForCompleteMock(DomContainer domContainer, int waitForCompleteTimeOut) : base(domContainer, waitForCompleteTimeOut) {}

        protected override SimpleTimer InitTimeout()
        {
            _timeout = base.InitTimeout().Timeout;
            return null;
        }

        protected override void WaitForCompleteOrTimeout()
        {
            // Finished;
        }

        public int Timeout
        {
            get { return _timeout; }
        }
    }
}