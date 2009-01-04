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
using System.Text.RegularExpressions;
using NUnit.Framework;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class SpanTests : BaseWithBrowserTests
	{
		[Test]
		public void SpanExists()
		{
			Assert.IsTrue(Ie.Span("spanid1").Exists);
			Assert.IsTrue(Ie.Span(new Regex("spanid1")).Exists);
			Assert.IsFalse(Ie.Span("nonexistingspanid1").Exists);
		}

		[Test]
		public void SpanElementTags()
		{
			Assert.AreEqual(1, Span.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("span", ((ElementTag) Span.ElementTags[0]).TagName);
		}

		[Test]
		public void CreateSpanFromElement()
		{
			Element element = Ie.Element("spanid1");
			Span span = new Span(element);
			Assert.AreEqual("spanid1", span.Id);
		}

		[Test]
		public void SpanTest()
		{
			Span Span = Ie.Span("spanid1");

			Assert.IsInstanceOfType(typeof (IElementsContainer), Span);
            Assert.IsInstanceOfType(typeof (ElementsContainer<Span>), Span);

			Assert.IsNotNull(Span, "Span should bot be null");
			Assert.AreEqual("spanid1", Span.Id, "Unexpected id");
		}

		[Test]
		public void Spans()
		{
			const int expectedSpansCount = 3;
			Assert.AreEqual(expectedSpansCount, Ie.Spans.Length, "Unexpected number of Spans");

			// Collection.Length
			SpanCollection formSpans = Ie.Spans;

			// Collection items by index
			Assert.AreEqual("spanid1", Ie.Spans[0].Id);
			Assert.AreEqual("Span1", Ie.Spans[1].Id);

			IEnumerable SpanEnumerable = formSpans;
			IEnumerator SpanEnumerator = SpanEnumerable.GetEnumerator();

			// Collection iteration and comparing the result with Enumerator
			int count = 0;
			foreach (Span inputSpan in formSpans)
			{
				SpanEnumerator.MoveNext();
				object enumSpan = SpanEnumerator.Current;

				Assert.IsInstanceOfType(inputSpan.GetType(), enumSpan, "Types are not the same");
				Assert.AreEqual(inputSpan.OuterHtml, ((Span) enumSpan).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(SpanEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(expectedSpansCount, count);
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}