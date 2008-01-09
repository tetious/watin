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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using WatiN.Core.Constraints;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class FramesetTests : BaseWithIETests
	{
		private const string frameNameContents = "contents";
		private const string frameNameMain = "main";

		public override Uri TestPageUri
		{
			get { return FramesetURI; }
		}

		[Test]
		public void IEFrameIsAFrame()
		{
			Assert.IsInstanceOfType(typeof (Frame), ie.Frame("mainid"));
		}

		[Test]
		public void FrameIsADocument()
		{
			Assert.IsInstanceOfType(typeof (Document), ie.Frame("mainid"));
		}

		[Test, ExpectedException(typeof (FrameNotFoundException), ExpectedMessage = "Could not find a Frame or IFrame matching constraint: Attribute 'id' with value 'NonExistingFrameID'")]
		public void ExpectFrameNotFoundException()
		{
			ie.Frame(Find.ById("NonExistingFrameID"));
		}

		[Test]
		public void FramesCollectionExists()
		{
			Assert.IsTrue(ie.Frames.Exists(Find.ByName("contents")));
			Assert.IsFalse(ie.Frames.Exists(Find.ByName("nonexisting")));
		}

		[Test]
		public void ContentsFrame()
		{
			Frame contentsFrame = ie.Frame(Find.ByName("contents"));
			Assert.IsNotNull(contentsFrame, "Frame expected");
			Assert.AreEqual("contents", contentsFrame.Name);
			Assert.AreEqual(null, contentsFrame.Id);

			AssertFindFrame(ie, Find.ByUrl(IndexURI), frameNameContents);
		}

		[Test]
		public void MainFrame()
		{
			Frame mainFrame = ie.Frame(Find.ById("mainid"));
			Assert.IsNotNull(mainFrame, "Frame expected");
			Assert.AreEqual("main", mainFrame.Name);
			Assert.AreEqual("mainid", mainFrame.Id);

			AssertFindFrame(ie, Find.ByName(frameNameMain), frameNameMain);
		}

		[Test]
		public void FrameHTMLShouldNotReturnParentHTML()
		{
			Frame contentsFrame = ie.Frame(Find.ByName("contents"));
			Assert.AreNotEqual(ie.Html, contentsFrame.Html, "Html should be from the frame page.");
		}

		[Test]
		public void DoesFrameCodeWorkIfToBrowsersWithFramesAreOpen()
		{
			using (IE ie2 = new IE(FramesetURI))
			{
				Frame contentsFrame = ie2.Frame(Find.ByName("contents"));
				Assert.IsFalse(contentsFrame.Html.StartsWith("<FRAMESET"), "inner html of frame is expected");
			}
		}

		[Test]
		public void Frames()
		{
			const int expectedFramesCount = 2;
			FrameCollection frames = ie.Frames;

			Assert.AreEqual(expectedFramesCount, frames.Length, "Unexpected number of frames");

			// Collection items by index
			Assert.AreEqual(frameNameContents, frames[0].Name);
			Assert.AreEqual(frameNameMain, frames[1].Name);

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
		public void ShouldBeAbleToAccessCustomAttributeInFrameSetElement()
		{
			string value = ie.Frame("mainid").GetAttributeValue("mycustomattribute");
			Assert.That(value, Is.EqualTo("WatiN"));
		}

		[Test]
		public void ShouldBeAbleToFindFrameUsingCustomAttributeInFrameSetElement()
		{
			Frame frame = ie.Frame(Find.By("mycustomattribute","WatiN"));
			Assert.That(frame.Id, Is.EqualTo("mainid"));
		}

		[Test, ExpectedException(typeof(FrameNotFoundException))]
		public void ShouldThrowFrameNotFoundExceptionWhenFindingFrameWithNonExistingAttribute()
		{
			ie.Frame(Find.By("nonexisting","something"));
		}

		[Test]
		public void ShowFrames()
		{
			UtilityClass.DumpFrames(ie);
		}

		private static void AssertFindFrame(IE ie, AttributeConstraint findBy, string expectedFrameName)
		{
			Frame frame = null;
			string attributeName = findBy.AttributeName.ToLower();
			if (attributeName == "href" || attributeName == "name")
			{
				frame = ie.Frame(findBy);
			}
			Assert.IsNotNull(frame, "Frame '" + findBy.Value + "' not found");
			Assert.AreEqual(expectedFrameName, frame.Name, "Incorrect frame for " + findBy.ToString() + ", " + findBy.Value);
		}
	}

	[TestFixture]
	public class FramesetWithinFrameSetTests : BaseWithIETests
	{
		[Test]
		public void FramesLength()
		{
			Assert.AreEqual(2, ie.Frames.Length);
			Assert.AreEqual(2, ie.Frames[1].Frames.Length);
		}

		public override Uri TestPageUri
		{
			get { return FramesetWithinFramesetURI; }
		}
	}

	[TestFixture]
	public class NoFramesTest : BaseWithIETests
	{
		[Test]
		public void HasNoFrames()
		{
			Assert.AreEqual(0, ie.Frames.Length);
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}