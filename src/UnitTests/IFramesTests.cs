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

using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Exceptions;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class IFramesTests : WatiNTest
	{
		private IE ie;

		[TestFixtureSetUp]
		public void Setup()
		{
			ie = new IE(IFramesMainURI);
		}

		[TestFixtureTearDown]
		public void tearDown()
		{
			ie.Close();
		}

		[Test, ExpectedException(typeof (FrameNotFoundException), ExpectedMessage = "Could not find a Frame or IFrame matching constraint: Attribute 'id' with value 'NonExistingIFrameID'")]
		public void ExpectFrameNotFoundException()
		{
			ie.Frame(Find.ById("NonExistingIFrameID"));
		}

		[Test]
		public void LeftFrame()
		{
			Frame leftFrame = ie.Frame(Find.ByName("left"));
			Assert.IsNotNull(leftFrame, "Frame expected");
			Assert.AreEqual("left", leftFrame.Name);
			Assert.AreEqual(null, leftFrame.Id);

			leftFrame = ie.Frame(Find.ByUrl(IFramesLeftURI));
			Assert.AreEqual("left", leftFrame.Name);
		}

		[Test]
		public void MiddleFrame()
		{
			Frame middleFrame = ie.Frame(Find.ByName("middle"));
			Assert.IsNotNull(middleFrame, "Frame expected");
			Assert.AreEqual("middle", middleFrame.Name);
			Assert.AreEqual("iframe2", middleFrame.Id);

			middleFrame = ie.Frame(Find.ByUrl(IFramesMiddleURI));
			Assert.AreEqual("middle", middleFrame.Name);

			middleFrame = ie.Frame(Find.ById("iframe2"));
			Assert.AreEqual("middle", middleFrame.Name);
		}

		[Test]
		public void RightFrame()
		{
			Frame rightFrame = ie.Frame(Find.ByUrl(IFramesRightURI));
			Assert.IsNotNull(rightFrame, "Frame expected");
			Assert.AreEqual(null, rightFrame.Name);
			Assert.AreEqual(null, rightFrame.Id);
		}

		[Test]
		public void Frames()
		{
			const int expectedFramesCount = 3;
			FrameCollection frames = ie.Frames;

			Assert.AreEqual(expectedFramesCount, frames.Length, "Unexpected number of frames");

			// Collection items by index
			Assert.AreEqual("left", frames[0].Name);
			Assert.AreEqual("middle", frames[1].Name);
			Assert.AreEqual(null, frames[2].Name);

			IEnumerable frameEnumerable = frames;
			IEnumerator frameEnumerator = frameEnumerable.GetEnumerator();

			// Collection iteration and comparing the result with Enumerator
			int count = 0;
			foreach (Frame frame in frames)
			{
				frameEnumerator.MoveNext();
				object enumFrame = frameEnumerator.Current;

				Assert.IsInstanceOfType(frame.GetType(), enumFrame, "Types are not the same");
				Assert.AreEqual(frame.Html, ((Frame) enumFrame).Html, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(frameEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(expectedFramesCount, count);
		}

		[Test]
		public void ShouldBeAbleToAccessCustomAttributeInIFrameElement()
		{
			string value = ie.Frame("iframe2").GetAttributeValue("mycustomattribute");
			Assert.That(value, Is.EqualTo("WatiN"));
		}

		[Test]
		public void ShouldBeAbleToFindFrameUsingCustomAttributeInIFrameElement()
		{
			Frame frame = ie.Frame(Find.By("mycustomattribute","WatiN"));
			Assert.That(frame.Id, Is.EqualTo("iframe2"));
		}

		[Test] //, Ignore("An Ebay IFrame never reaches complete status"), Category("InternetConnectionNeeded")]
			public void EbayTest()
		{
			// ebay seems to embed some active-x(?) component
			// which crashed the code in NativeMethods.EnumIWebBrowser2Interfaces
			// Changed the code to release the pUnk.
			// This test should ensure propper working untill I'm able to 
			// create a better test.
			using (new IE(EbayUrl)) {}
		}
	}
}