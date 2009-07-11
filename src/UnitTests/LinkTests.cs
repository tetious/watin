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
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class LinkTests : BaseWithBrowserTests
	{
		[Test]
		public void LinkElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<Link>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("a", elementTags[0].TagName);
		}

		[Test]
		public void LinkExists()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.IsTrue(browser.Link("testlinkid").Exists);
                                Assert.IsTrue(browser.Link(new Regex("testlinkid")).Exists);
                                Assert.IsFalse(browser.Link("nonexistingtestlinkid").Exists);
		                    });
		}

		[Test]
		public void LinkTest()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.AreEqual(WatiNURI, browser.Link(Find.ById("testlinkid")).Url);
                                Assert.AreEqual(WatiNURI, browser.Link("testlinkid").Url);
                                Assert.AreEqual(WatiNURI, browser.Link(Find.ByName("testlinkname")).Url);
                                Assert.AreEqual(WatiNURI, browser.Link(Find.ByUrl(WatiNURI)).Url);
                                Assert.AreEqual("Microsoft", browser.Link(Find.ByText("Microsoft")).Text);
		                    });
		}

		[Test]
		public void Links()
		{
			const int expectedLinkCount = 4;

		    ExecuteTest(browser =>
		                    {
                                Assert.AreEqual(expectedLinkCount, browser.Links.Count, "Unexpected number of links");

                                var links = browser.Links;

		                        // Collection items by index
		                        Assert.AreEqual(expectedLinkCount, links.Count, "Wrong number off links");
		                        Assert.AreEqual("testlinkid", links[0].Id);
		                        Assert.AreEqual("testlinkid1", links[1].Id);

		                        // Collection iteration and comparing the result with Enumerator
		                        IEnumerable linksEnumerable = links;
		                        var linksEnumerator = linksEnumerable.GetEnumerator();

		                        var count = 0;
		                        foreach (var link in links)
		                        {
		                            linksEnumerator.MoveNext();
		                            var enumLink = linksEnumerator.Current;

		                            Assert.IsInstanceOfType(link.GetType(), enumLink, "Types are not the same");
		                            Assert.AreEqual(link.OuterHtml, ((Link) enumLink).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(linksEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(expectedLinkCount, count);
		                    });
		}

	    [Test]
	    public void ClickingOnLinkWithJavaScriptInHrefShouldWork()
	    {
	        ExecuteTest(browser =>
	                        {
                                // GIVEN
	                            browser.GoTo(TestEventsURI);

                                var link = browser.Link("hreftest");
                                
                                // WHEN
                                link.Click();
                                
                                // THEN
                                Assert.That(browser.TextField("hrefclickresult").Value, Is.EqualTo("success"));
	                        });
	    }

	    [Test]
	    public void ShouldBePossibleToGetImageInsideLink()
	    {
	        ExecuteTest(browser =>
	                        {
                                // GIVEN
	                            browser.GoTo(ImagesURI);

                                var link = browser.Link("linkWithImage");
                                
                                // WHEN
	                            var images = link.Images.Count;

	                            // THEN
                                Assert.That(images, Is.EqualTo(1));
	                        });
	    }

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}