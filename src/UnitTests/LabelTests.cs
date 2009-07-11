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
using System.Collections.Generic;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class LabelTests : BaseWithBrowserTests
	{
		[Test]
		public void LabelElementTags()
		{
            IList<ElementTag> elementTags = ElementFactory.GetElementTags<Label>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("label", elementTags[0].TagName);
		}

		[Test]
		public void LabelExists()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.IsTrue(browser.Label(Find.ByFor("Checkbox21")).Exists);
                                Assert.IsTrue(browser.Label(Find.ByFor(new Regex("Checkbox21"))).Exists);
                                Assert.IsFalse(browser.Label(Find.ByFor("nonexistingCheckbox21")).Exists);
		                    });
		}

		[Test]
		public void LabelByFor()
		{
		    ExecuteTest(browser =>
		                    {
                                var label = browser.Label(Find.ByFor("Checkbox21"));

		                        Assert.AreEqual("Checkbox21", label.For, "Unexpected label.For id");
		                        Assert.AreEqual("label for Checkbox21", label.Text, "Unexpected label.Text");
		                        Assert.AreEqual("C", label.AccessKey, "Unexpected label.AccessKey");
		                    });
		}

		[Test]
		public void LabelByForWithElement()
		{
		    ExecuteTest(browser =>
		                    {
                                var checkBox = browser.CheckBox("Checkbox21");

                                var label = browser.Label(Find.ByFor(checkBox));

		                        Assert.AreEqual("Checkbox21", label.For, "Unexpected label.For id");
		                        Assert.AreEqual("label for Checkbox21", label.Text, "Unexpected label.Text");
		                        Assert.AreEqual("C", label.AccessKey, "Unexpected label.AccessKey");
		                    });
		}

		[Test]
		public void LabelWrapped()
		{
		    ExecuteTest(browser =>
		                    {
                                var labelCollection = browser.Labels;
                                Assert.AreEqual(2, browser.Labels.Count, "Unexpected number of labels");

		                        var label = labelCollection[1];

		                        Assert.AreEqual(null, label.For, "Unexpected label.For id");
		                        var regex = new Regex("Test label before: +Test label after");
                                Assert.That(regex.IsMatch(label.Text), Is.True, "Unexpected label.Text: '" + label.Text + "'");
		                    });
		}

		[Test]
		public void Labels()
		{
		    ExecuteTest(browser =>
		                    {
		                        const int expectedLabelCount = 2;

                                Assert.AreEqual(expectedLabelCount, browser.Labels.Count, "Unexpected number of labels");

                                var labelCollection = browser.Labels;

		                        // Collection items by index
                                Assert.AreEqual(expectedLabelCount, labelCollection.Count, "Wrong number of labels");

		                        // Collection iteration and comparing the result with Enumerator
		                        IEnumerable labelEnumerable = labelCollection;
		                        var labelEnumerator = labelEnumerable.GetEnumerator();

		                        var count = 0;
		                        foreach (Label label in labelCollection)
		                        {
		                            labelEnumerator.MoveNext();
		                            var enumCheckbox = labelEnumerator.Current;

		                            Assert.IsInstanceOfType(label.GetType(), enumCheckbox, "Types are not the same");
		                            Assert.AreEqual(label.OuterHtml, ((Label) enumCheckbox).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(labelEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(expectedLabelCount, count);

		                    });
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}