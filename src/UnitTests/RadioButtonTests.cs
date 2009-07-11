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
	public class RadioButtonTests : BaseWithBrowserTests
	{
		[Test]
		public void RadioButtonElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<RadioButton>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("input", elementTags[0].TagName);
			Assert.AreEqual("radio", elementTags[0].InputType);
		}

		[Test]
		public void RadioButtonExists()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.IsTrue(browser.RadioButton("Radio1").Exists);
		                        Assert.IsTrue(browser.RadioButton(new Regex("Radio1")).Exists);
		                        Assert.IsFalse(browser.RadioButton("nonexistingRadio1").Exists);
		                    });
		}

		[Test]
		public void RadioButtonTest()
		{
		    ExecuteTest(browser =>
		                    {
		                        var RadioButton1 = browser.RadioButton("Radio1");

		                        Assert.AreEqual("Radio1", RadioButton1.Id, "Found wrong RadioButton.");
		                        Assert.AreEqual("Radio1", RadioButton1.ToString(), "ToString didn't return the Id.");
		                    });
		}
		[Test]
		public void RadioButtonWhichIsCheckedOnPageLoadShouldReturnChecked()
		{
		    ExecuteTest(browser =>
		                    {
                                browser.GoTo(TestPageUri);
		                        var RadioButton1 = browser.RadioButton("Radio1");
		                        Assert.IsTrue(RadioButton1.Checked, "Should initially be checked");
		                    });
		}

        [Test]
		public void RadioButtonsWithSameNameShouldSwitchCheckedWhenClickedOn()
		{
		    ExecuteTest(browser =>
		                    {
		                        var RadioButton2 = browser.RadioButton("Radio2");
		                        var RadioButton3 = browser.RadioButton("Radio3");

		                        Assert.That(RadioButton2.Checked, Is.False, "Radio2 should not be checked");
		                        Assert.That(RadioButton3.Checked, Is.False, "Radio3 should not be checked");

		                        RadioButton2.Checked = true;
		                        Assert.That(RadioButton2.Checked, Is.True, "Radio2 Should be checked");
		                        Assert.That(RadioButton3.Checked, Is.False, "Radio3 Should not be checked (when radio2 is selected)");

                                RadioButton3.Checked = true;
                                Assert.That(RadioButton2.Checked, Is.False, "Radio2 Should not be checked (when radio2 is selected)");
		                        Assert.That(RadioButton3.Checked, Is.True, "Radio3 Should be checked");
		                    });
		}

		[Test]
		public void RadioButtons()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.AreEqual(3, browser.RadioButtons.Count, "Unexpected number of RadioButtons");

		                        var formRadioButtons = browser.Form("FormRadioButtons").RadioButtons;

                                Assert.AreEqual(2, formRadioButtons.Count, "Wrong number off RadioButtons");
		                        Assert.AreEqual("Radio2", formRadioButtons[0].Id);
		                        Assert.AreEqual("Radio3", formRadioButtons[1].Id);

		                        // Collection iteration and comparing the result with Enumerator
		                        IEnumerable radiobuttonEnumerable = formRadioButtons;
		                        var radiobuttonEnumerator = radiobuttonEnumerable.GetEnumerator();

		                        var count = 0;
		                        foreach (var radioButton in formRadioButtons)
		                        {
		                            radiobuttonEnumerator.MoveNext();
		                            var enumRadioButton = radiobuttonEnumerator.Current;

		                            Assert.IsInstanceOfType(radioButton.GetType(), enumRadioButton, "Types are not the same");
		                            Assert.AreEqual(radioButton.OuterHtml, ((RadioButton) enumRadioButton).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(radiobuttonEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(2, count);
		                    });
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}