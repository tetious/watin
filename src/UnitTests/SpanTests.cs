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
using System.Text.RegularExpressions;
using NUnit.Framework;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class SpanTests : BaseWithBrowserTests
	{
		[Test]
		public void SpanExists()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.IsTrue(browser.Span("spanid1").Exists);
                                Assert.IsTrue(browser.Span(new Regex("spanid1")).Exists);
                                Assert.IsFalse(browser.Span("nonexistingspanid1").Exists);
		                    });
		}

		[Test]
		public void SpanElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<Span>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("span", elementTags[0].TagName);
		}

		[Test]
		public void SpanTest()
		{
		    ExecuteTest(browser =>
		                    {
                                var Span = browser.Span("spanid1");

			                    Assert.IsInstanceOfType(typeof (IElementContainer), Span);
                                Assert.IsInstanceOfType(typeof (ElementContainer<Span>), Span);

		                        Assert.IsNotNull(Span, "Span should bot be null");
		                        Assert.AreEqual("spanid1", Span.Id, "Unexpected id");
		                    });
		}

		[Test]
		public void Spans()
		{
			const int expectedSpansCount = 3;

		    ExecuteTest(browser =>
		                    {
                                Assert.AreEqual(expectedSpansCount, browser.Spans.Count, "Unexpected number of Spans");

		                        // Collection.Length
                                var formSpans = browser.Spans;

		                        // Collection items by index
                                Assert.AreEqual("spanid1", browser.Spans[0].Id);
                                Assert.AreEqual("Span1", browser.Spans[1].Id);

		                        IEnumerable SpanEnumerable = formSpans;
		                        var SpanEnumerator = SpanEnumerable.GetEnumerator();

		                        // Collection iteration and comparing the result with Enumerator
		                        var count = 0;
		                        foreach (var inputSpan in formSpans)
		                        {
		                            SpanEnumerator.MoveNext();
		                            var enumSpan = SpanEnumerator.Current;

		                            Assert.IsInstanceOfType(inputSpan.GetType(), enumSpan, "Types are not the same");
		                            Assert.AreEqual(inputSpan.OuterHtml, ((Span) enumSpan).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(SpanEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(expectedSpansCount, count);
		                    });
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}