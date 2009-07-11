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
	public class ParaTests : BaseWithBrowserTests
	{
		[Test]
		public void ParaElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<Para>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("p", elementTags[0].TagName);
		}

		[Test]
		public void ParaExists()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.IsTrue(browser.Para("links").Exists);
                                Assert.IsTrue(browser.Para(new Regex("links")).Exists);
                                Assert.IsFalse(browser.Para("nonexistinglinks").Exists);
		                    });
		}

		[Test]
		public void ParaTest()
		{
		    ExecuteTest(browser =>
		                    {
                                var para = browser.Para("links");

			                    Assert.IsInstanceOfType(typeof (IElementContainer), para);
                                Assert.IsInstanceOfType(typeof (ElementContainer<Para>), para);

		                        Assert.IsNotNull(para);
		                        Assert.AreEqual("links", para.Id);
		                    });
		}

		[Test]
		public void Paras()
		{
			const int expectedParasCount = 4;
		    ExecuteTest(browser =>
		                    {
                                Assert.AreEqual(expectedParasCount, browser.Paras.Count, "Unexpected number of Paras");

		                        // Collection.Length
                                var formParas = browser.Paras;

		                        // Collection items by index
                                Assert.IsNull(browser.Paras[0].Id);
                                Assert.AreEqual("links", browser.Paras[1].Id);
                                Assert.IsNull(browser.Paras[2].Id);
                                Assert.IsNull(browser.Paras[3].Id);

		                        IEnumerable ParaEnumerable = formParas;
		                        var ParaEnumerator = ParaEnumerable.GetEnumerator();

		                        // Collection iteration and comparing the result with Enumerator
		                        var count = 0;
		                        foreach (var inputPara in formParas)
		                        {
		                            ParaEnumerator.MoveNext();
		                            var enumPara = ParaEnumerator.Current;

		                            Assert.IsInstanceOfType(inputPara.GetType(), enumPara, "Types are not the same");
		                            Assert.AreEqual(inputPara.OuterHtml, ((Para) enumPara).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(ParaEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(expectedParasCount, count);
		                    });
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}