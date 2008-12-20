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
using WatiN.Core.Constraints;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class CheckBoxTests : BaseWithIETests
    {
        [Test]
        public void CheckBoxElementTags()
        {
            Assert.AreEqual(1, CheckBox.ElementTags.Count, "1 elementtags expected");
            Assert.AreEqual("input", CheckBox.ElementTags[0].TagName);
            Assert.AreEqual("checkbox", CheckBox.ElementTags[0].InputTypes);
        }

        [Test]
        public void CheckBoxFromElement()
        {
            var element = ie.Element("Checkbox1");
            var checkBox = new CheckBox(element);
            Assert.AreEqual("Checkbox1", checkBox.Id);
        }

        [Test]
        public void CheckBoxExists()
        {
            Assert.IsTrue(ie.CheckBox("Checkbox1").Exists);
            Assert.IsTrue(ie.CheckBox(new Regex("Checkbox1")).Exists);
            Assert.IsFalse(ie.CheckBox("noneexistingCheckbox1id").Exists);
        }

        [Test]
        public void CheckBoxTest()
        {
            var checkbox1 = ie.CheckBox("Checkbox1");

            Assert.AreEqual("Checkbox1", checkbox1.Id, "Found wrong checkbox");
            Assert.AreEqual("Checkbox1", checkbox1.ToString(), "ToString didn't return Id");
            Assert.IsTrue(checkbox1.Checked, "Should initially be checked");

            checkbox1.Checked = false;
            Assert.IsFalse(checkbox1.Checked, "Should not be checked");

            checkbox1.Checked = true;
            Assert.IsTrue(checkbox1.Checked, "Should be checked");
        }

        [Test]
        public void CheckBoxes()
        {
            Assert.AreEqual(5, ie.CheckBoxes.Length, "Unexpected number of checkboxes");

            var formCheckBoxs = ie.Form("FormCheckboxes").CheckBoxes;

            // Collection items by index
            Assert.AreEqual(3, formCheckBoxs.Length, "Wrong number off checkboxes");
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
        }

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }

        [Test]
        public void FindCheckboxByLabel()
        {
            // The control to test against
            Assert.IsTrue(ie.CheckBox("Checkbox21").Exists, "Checkbox21 missing.");
            var checkBox21a = ie.CheckBox("Checkbox21");

            // The new way to do this
            var checkBox21b = ie.CheckBox(new LabelTextConstraint("label for Checkbox21"));
            Assert.AreEqual(checkBox21a.Id, checkBox21b.Id, "Checkbox attached to Label for Checkbox21 did not match CheckBox21.");

            // The old way to do this
            // Using this method is about 10% slower than the new one and much more complicated to read/write
            var label = ie.Label(Find.ByText("label for Checkbox21"));
            Assert.IsNotNull(label, "Missing label for Checkbox21 or text has changed.");
            var checkBox21c = ie.CheckBox(label.For);
            Assert.AreEqual(checkBox21c.Id, checkBox21b.Id, "Label for Checkbox21b did not attach to correct checkbox.");

        }
    }
}