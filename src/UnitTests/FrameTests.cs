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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Exceptions;
using WatiN.Core.Constraints;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class FramesetTests : BaseWithBrowserTests
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
			Assert.IsInstanceOfType(typeof (Frame), Ie.Frame("mainid"));
		}

		[Test]
		public void FrameIsADocument()
		{
			Assert.IsInstanceOfType(typeof (Document), Ie.Frame("mainid"));
		}

		[Test]
		public void ExpectFrameNotFoundException()
		{
		    ExecuteTest(browser =>
		                    {
		                        try
		                        {
		                            browser.Frame(Find.ById("NonExistingFrameID"));
                                    Assert.Fail("Expected " + typeof(FrameNotFoundException));
		                        }
		                        catch (Exception e)
		                        {
		                            Assert.That(e, Is.InstanceOfType(typeof(FrameNotFoundException)), "Unexpected exception");
                                    Assert.That(e.Message, Is.EqualTo("Could not find a Frame or IFrame matching constraint: Attribute 'id' equals 'NonExistingFrameID'"), "Unexpected message");
		                        }
		                    });
		}

		[Test]
		public void FramesCollectionExists()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.IsTrue(browser.Frames.Exists(Find.ByName("contents")));
                                Assert.IsFalse(browser.Frames.Exists(Find.ByName("nonexisting")));
		                    });
		}

		[Test]
		public void ContentsFrame()
		{
		    ExecuteTest(browser =>
		                    {
                                var contentsFrame = browser.Frame(Find.ByName("contents"));
		                        Assert.IsNotNull(contentsFrame, "Frame expected");
		                        Assert.AreEqual("contents", contentsFrame.Name);
		                        Assert.AreEqual(null, contentsFrame.Id);

                                AssertFindFrame(browser, Find.ByUrl(IndexURI), frameNameContents);
		                    });
		}

		[Test]
		public void MainFrame()
		{
		    ExecuteTest(browser =>
		                    {
		                        var mainFrame = browser.Frame(Find.ById("mainid"));
		                        Assert.IsNotNull(mainFrame, "Frame expected");
		                        Assert.AreEqual("main", mainFrame.Name);
		                        Assert.AreEqual("mainid", mainFrame.Id);

		                        AssertFindFrame(browser, Find.ByName(frameNameMain), frameNameMain);
		                    });
		}

		[Test]
		public void FrameHTMLShouldNotReturnParentHTML()
		{
		    ExecuteTest(browser =>
		                    {
                                var contentsFrame = browser.Frame(Find.ByName("contents"));
                                Assert.AreNotEqual(browser.Html, contentsFrame.Html, "Html should be from the frame page.");
		                    });
		}

        // TODO: Make multi browser test as soon as WatiN can handle multiple instances of FireFox
		[Test]
		public void DoesFrameCodeWorkIfTwoBrowsersWithFramesAreOpen()
		{
			using (var ie2 = new IE(FramesetURI))
			{
				var contentsFrame = ie2.Frame(Find.ByName("contents"));
				Assert.IsFalse(contentsFrame.Html.StartsWith("<FRAMESET"), "inner html of frame is expected");
			}
		}

		[Test]
		public void Frames()
		{
		    ExecuteTest(browser =>
		                    {
		                        const int expectedFramesCount = 2;
                                var frames = browser.Frames;

                                Assert.AreEqual(expectedFramesCount, frames.Count, "Unexpected number of frames");

		                        // Collection items by index
		                        Assert.AreEqual(frameNameContents, frames[0].Name);
		                        Assert.AreEqual(frameNameMain, frames[1].Name);

		                        IEnumerable frameEnumerable = frames;
		                        var frameEnumerator = frameEnumerable.GetEnumerator();

		                        // Collection iteration and comparing the result with Enumerator
		                        var count = 0;
		                        foreach (Frame frame in frames)
		                        {
		                            frameEnumerator.MoveNext();
		                            var enumFrame = frameEnumerator.Current;

		                            Assert.IsInstanceOfType(frame.GetType(), enumFrame, "Types are not the same");
		                            Assert.AreEqual(frame.Html, ((Frame) enumFrame).Html, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(frameEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(expectedFramesCount, count);
		                    });
		}

		[Test]
		public void ShouldBeAbleToAccessCustomAttributeInFrameSetElement()
		{
		    ExecuteTest(browser =>
		                    {
		                        var value = browser.Frame("mainid").GetAttributeValue("mycustomattribute");
		                        Assert.That(value, Is.EqualTo("WatiN"));
		                    });
		}

		[Test]
		public void ShouldBeAbleToFindFrameUsingCustomAttributeInFrameSetElement()
		{
		    ExecuteTest(browser =>
		                    {
		                        var frame = browser.Frame(Find.By("mycustomattribute","WatiN"));
		                        Assert.That(frame.Id, Is.EqualTo("mainid"));
		                    });
		}

		[Test]
		public void ShouldThrowFrameNotFoundExceptionWhenFindingFrameWithNonExistingAttribute()
		{
		    ExecuteTest(browser =>
		                    {
		                        try
		                        {
		                            browser.Frame(Find.By("nonexisting","something"));
                                    Assert.Fail("Expected " + typeof(FrameNotFoundException));
		                        }
		                        catch (Exception e)
		                        {
		                            Assert.That(e, Is.TypeOf(typeof(FrameNotFoundException)), "Unexpected exception");
		                        }
		                    });
		}

	    [Test]
	    public void ShouldBeAbleToSetFrameAttribute()
	    {
            ExecuteTest(browser =>
                            {
	                            // GIVEN
                                var mainFrame = browser.Frame("mainid");
                                Assert.That(mainFrame.GetAttributeValue("myAttrib"), Is.Null, "pre-condition failed");
                                
                                // WHEN
                                mainFrame.SetAttributeValue("myAttrib", "myValue");

                                //THEN
                                Assert.That(mainFrame.GetAttributeValue("myAttrib"), Is.EqualTo("myValue"));
                            }
                );
	    }


		private static void AssertFindFrame(Document document, AttributeConstraint findBy, string expectedFrameName)
		{
			Frame frame = null;
			var attributeName = findBy.AttributeName.ToLower();
			if (attributeName == "href" || attributeName == "name")
			{
				frame = document.Frame(findBy);
			}
            Assert.IsNotNull(frame, "Frame '" + findBy.Comparer + "' not found");
            Assert.AreEqual(expectedFrameName, frame.Name, "Incorrect frame for " + findBy + ", " + findBy.Comparer);
		}
	}
}