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
using WatiN.Core.Constraints;
using System.Collections.Generic;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class CheckBoxTests : BaseWithBrowserTests
    {
        [Test]
        public void CheckBoxElementTags()
        {
            var elementTags = ElementFactory.GetElementTags<CheckBox>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
            Assert.AreEqual("input", elementTags[0].TagName);
            Assert.AreEqual("checkbox", elementTags[0].InputType);
        }

        [Test]
        public void CheckBoxExists()
        {
            ExecuteTest(browser =>
                            {
                                Assert.IsTrue(browser.CheckBox("Checkbox1").Exists);
                                Assert.IsTrue(browser.CheckBox(new Regex("Checkbox1")).Exists);
                                Assert.IsFalse(browser.CheckBox("noneexistingCheckbox1id").Exists);
                            });
        }

        [Test]
        public void CheckBoxTest()
        {
            ExecuteTest(browser =>
                            {
                                var checkbox1 = browser.CheckBox("Checkbox1");

                                Assert.AreEqual("Checkbox1", checkbox1.Id, "Found wrong checkbox");
                                Assert.AreEqual("Checkbox1", checkbox1.ToString(), "ToString didn't return Id");
                                Assert.IsTrue(checkbox1.Checked, "Should initially be checked");

                                checkbox1.Checked = true;
                                Assert.That(checkbox1.Checked, Is.True, "Should still be checked");

                                checkbox1.Checked = false;
                                Assert.IsFalse(checkbox1.Checked, "Should not be checked");

                                checkbox1.Checked = true;
                                Assert.IsTrue(checkbox1.Checked, "Should be checked");
                            });
        }

        [Test]
        public void CheckBoxes()
        {
            ExecuteTest(browser =>
                            {
                                Assert.AreEqual(5, browser.CheckBoxes.Count, "Unexpected number of checkboxes");

                                var formCheckBoxs = browser.Form("FormCheckboxes").CheckBoxes;

                                // Collection items by index
                                Assert.AreEqual(3, formCheckBoxs.Count, "Wrong number off checkboxes");
                                Assert.AreEqual("Checkbox1", formCheckBoxs[0].Id);
                                Assert.AreEqual("Checkbox2", formCheckBoxs[1].Id);
                                Assert.AreEqual("Checkbox4", formCheckBoxs[2].Id);

                                // Collection iteration and comparing the result with Enumerator
                                IEnumerable checkboxEnumerable = formCheckBoxs;
                                var checkboxEnumerator = checkboxEnumerable.GetEnumerator();

                                var count = 0;
                                foreach (CheckBox checkBox in formCheckBoxs)
                                {
                                    checkboxEnumerator.MoveNext();
                                    var enumCheckbox = checkboxEnumerator.Current;

                                    Assert.IsInstanceOfType(checkBox.GetType(), enumCheckbox, "Types are not the same");
                                    Assert.AreEqual(checkBox.OuterHtml, ((CheckBox)enumCheckbox).OuterHtml, "foreach and IEnumator don't act the same.");
                                    ++count;
                                }

                                Assert.IsFalse(checkboxEnumerator.MoveNext(), "Expected last item");
                                Assert.AreEqual(3, count);

                            });
        }

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        [Test]
        public void FindCheckboxByLabel()
        {
            ExecuteTest(browser =>
                            {
                                // The control to test against
                                var checkBox21a = browser.CheckBox("Checkbox21");
                                Assert.That(checkBox21a.Exists, "Checkbox21 missing.");

                                // The new way to do this
                                var checkBox21b = browser.CheckBox(new LabelTextConstraint("label for Checkbox21"));
                                Assert.AreEqual(checkBox21a.Id, checkBox21b.Id, "Checkbox attached to Label for Checkbox21 did not match CheckBox21.");

                                // The old way to do this
                                // Using this method is about 10% slower than the new one and much more complicated to read/write
                                var label = browser.Label(Find.ByText("label for Checkbox21"));
                                Assert.That(label.Exists, Is.True, "Missing label for Checkbox21 or text has changed.");
                                var checkBox21c = browser.CheckBox(label.For);
                                Assert.AreEqual(checkBox21c.Id, checkBox21b.Id, "Label for Checkbox21b did not attach to correct checkbox.");
                            });
        }
    }
}