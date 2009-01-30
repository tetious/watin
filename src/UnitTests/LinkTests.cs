#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

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
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class LinkTests : BaseWithBrowserTests
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
			Element element = Ie.Element("testlinkid");
			Link link = new Link(element);
			Assert.AreEqual("testlinkid", link.Id);
		}

		[Test]
		public void LinkExists()
		{
			Assert.IsTrue(Ie.Link("testlinkid").Exists);
			Assert.IsTrue(Ie.Link(new Regex("testlinkid")).Exists);
			Assert.IsFalse(Ie.Link("nonexistingtestlinkid").Exists);
		}

		[Test]
		public void LinkTest()
		{
			Assert.AreEqual(WatiNURI, Ie.Link(Find.ById("testlinkid")).Url);
			Assert.AreEqual(WatiNURI, Ie.Link("testlinkid").Url);
			Assert.AreEqual(WatiNURI, Ie.Link(Find.ByName("testlinkname")).Url);
			Assert.AreEqual(WatiNURI, Ie.Link(Find.ByUrl(WatiNURI)).Url);
			Assert.AreEqual("Microsoft", Ie.Link(Find.ByText("Microsoft")).Text);
		}

		[Test]
		public void Links()
		{
			const int expectedLinkCount = 3;

			Assert.AreEqual(expectedLinkCount, Ie.Links.Length, "Unexpected number of links");

			LinkCollection links = Ie.Links;

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
	        Settings.MakeNewIeInstanceVisible = true;

	        using (IE ie = new IE(TestEventsURI))
	        {
	            Link link = ie.Link("hreftest");
	            link.Click();
                Assert.That(ie.TextField("hrefclickresult").Value, Is.EqualTo("success"));
	        }
	    }

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}