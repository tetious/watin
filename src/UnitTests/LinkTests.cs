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

using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class LinkTests : BaseElementsTests
	{
		[Test]
		public void LinkElementTags()
		{
			Assert.AreEqual(1, Link.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("a", ((ElementTag) Link.ElementTags[0]).TagName);
		}

		[Test]
		public void LinkFromElement()
		{
			Element element = ie.Element("testlinkid");
			Link link = new Link(element);
			Assert.AreEqual("testlinkid", link.Id);
		}

		[Test]
		public void LinkExists()
		{
			Assert.IsTrue(ie.Link("testlinkid").Exists);
			Assert.IsTrue(ie.Link(new Regex("testlinkid")).Exists);
			Assert.IsFalse(ie.Link("nonexistingtestlinkid").Exists);
		}

		[Test]
		public void LinkTest()
		{
			Assert.AreEqual(WatiNURI, ie.Link(Find.ById("testlinkid")).Url);
			Assert.AreEqual(WatiNURI, ie.Link("testlinkid").Url);
			Assert.AreEqual(WatiNURI, ie.Link(Find.ByName("testlinkname")).Url);
			Assert.AreEqual(WatiNURI, ie.Link(Find.ByUrl(WatiNURI)).Url);
			Assert.AreEqual("Microsoft", ie.Link(Find.ByText("Microsoft")).Text);
		}

		[Test]
		public void Links()
		{
			const int expectedLinkCount = 3;

			Assert.AreEqual(expectedLinkCount, ie.Links.Length, "Unexpected number of links");

			LinkCollection links = ie.Links;

			// Collection items by index
			Assert.AreEqual(expectedLinkCount, links.Length, "Wrong number off links");
			Assert.AreEqual("testlinkid", links[0].Id);
			Assert.AreEqual("testlinkid1", links[1].Id);

			// Collection iteration and comparing the result with Enumerator
			IEnumerable linksEnumerable = links;
			IEnumerator linksEnumerator = linksEnumerable.GetEnumerator();

			int count = 0;
			foreach (Link link in links)
			{
				linksEnumerator.MoveNext();
				object enumLink = linksEnumerator.Current;

				Assert.IsInstanceOfType(link.GetType(), enumLink, "Types are not the same");
				Assert.AreEqual(link.OuterHtml, ((Link) enumLink).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(linksEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(expectedLinkCount, count);
		}

	    [Test]
	    public void ClickingOnLinkWithJavaScriptInHrefShouldWork()
	    {
	        IE.Settings.MakeNewIeInstanceVisible = true;

	        using (IE ie = new IE(TestEventsURI))
	        {
	            Link link = ie.Link("hreftest");
	            link.Click();
                Assert.That(ie.TextField("hrefclickresult").Value, Is.EqualTo("success"));
	        }
	    }

	}
}