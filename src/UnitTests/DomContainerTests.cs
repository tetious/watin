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
using NUnit.Framework;
using WatiN.Core.Interfaces;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class DomContainerTests
	{
        private MyTestDomContainer myTestDomContainer;
		private Mock<IWait> _waitMock;

		[SetUp]
		public void Setup()
		{
			Settings.AutoStartDialogWatcher = false;

            myTestDomContainer = new MyTestDomContainer();

            _waitMock = new Mock<IWait>();

		}

        [Test]
		public void WaitForCompleteUsesGivenWaitClass()
		{
			_waitMock.Expect(wait => wait.DoWait());

			myTestDomContainer.WaitForComplete(_waitMock.Object);

			_waitMock.VerifyAll();
		}

	    [Test]
	    public void WaitForCompleteShouldUseTimeOutProvidedThroughtTheConstructor()
	    {
	        var waitForCompleteMock = new WaitForCompleteStub(333);

            waitForCompleteMock.DoWait();
            
            Assert.That(waitForCompleteMock.Timeout, NUnit.Framework.SyntaxHelpers.Is.EqualTo(333), "Unexpected timeout");
	    }

	    [Test]
		public void DomContainerIsDocument()
		{
			Assert.IsInstanceOfType(typeof (Document), myTestDomContainer);
		}

	    [Test]
	    public void NativeDocumentShouldCallOnGetNativeDocument()
	    {
	        // GIVEN
	        var nativeDocument = new Mock<INativeDocument>().Object;
	        myTestDomContainer.ReturnNativeDocument = nativeDocument;

	        // WHEN
	        var result = myTestDomContainer.NativeDocument;

	        // THEN
	        Assert.That(ReferenceEquals(nativeDocument, result), "Unexpected instance");
	    }


	    [TearDown]
		public virtual void TearDown()
		{
			myTestDomContainer.Dispose();
			Settings.Reset();
		}

        internal class MyTestDomContainer : DomContainer
        {
            public INativeDocument ReturnNativeDocument { get; set; }

            public override IntPtr hWnd
            {
                get { throw new NotImplementedException(); }
            }

            public override INativeDocument OnGetNativeDocument()
            {
                return ReturnNativeDocument;
            }

            protected override string GetAttributeValueImpl(string attributeName)
            {
                throw new NotImplementedException();
            }

            public override void WaitForComplete(int waitForCompleteTimeOut)
            {
                //                waitForCompleteTimeOut()
            }
        }
	}

    internal class WaitForCompleteStub : WaitForCompleteBase
    {
        public WaitForCompleteStub(int waitForCompleteTimeOut) : base(waitForCompleteTimeOut) {}

        protected override SimpleTimer InitTimeout()
        {
            Timeout = (int) base.InitTimeout().Timeout.TotalSeconds;
            return null;
        }

        protected override void WaitForCompleteOrTimeout()
        {
            // Finished;
        }

        public int Timeout { get; private set; }
    }
}