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
	public class DivTests : BaseWithBrowserTests
	{
		[Test]
		public void DivElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<Div>();
			Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("div", elementTags[0].TagName);
		}

		[Test]
		public void DivExists()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.IsTrue(browser.Div("divid").Exists);
		                        Assert.IsTrue(browser.Div(new Regex("divid")).Exists);
		                        Assert.IsFalse(browser.Div("noneexistingdivid").Exists);
		                    });
		}

		[Test]
		public void DivTest()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.AreEqual("divid", browser.Div(Find.ById("divid")).Id, "Find Div by Find.ById");
		                        Assert.AreEqual("divid", browser.Div("divid").Id, "Find Div by ie.Div()");
		                    });
		}

		[Test]
		public void Divs()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.AreEqual(4, browser.Divs.Count, "Unexpected number of Divs");

		                        var divs = browser.Divs;

		                        // Collection items by index
		                        Assert.AreEqual("divid", divs[0].Id);

		                        // Collection iteration and comparing the result with Enumerator
		                        IEnumerable divEnumerable = divs;
		                        var divEnumerator = divEnumerable.GetEnumerator();

		                        var count = 0;
		                        foreach (var div in divs)
		                        {
		                            divEnumerator.MoveNext();
		                            var enumDiv = divEnumerator.Current;

		                            Assert.IsInstanceOfType(div.GetType(), enumDiv, "Types are not the same");
		                            Assert.AreEqual(div.OuterHtml, ((Div) enumDiv).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(divEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(4, count);
		                    });
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}