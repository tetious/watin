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

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class RadioButtonTests : BaseWithBrowserTests
	{
		[Test]
		public void RadioButtonElementTags()
		{
			Assert.AreEqual(1, RadioButton.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("input", RadioButton.ElementTags[0].TagName);
			Assert.AreEqual("radio", RadioButton.ElementTags[0].InputTypes);
		}

		[Test]
		public void CreateRadioButtonFromElement()
		{
		    ExecuteTest(browser =>
		                    {
		                        var element = browser.Element("Radio1");
		                        var radioButton = new RadioButton(element);
		                        Assert.AreEqual("Radio1", radioButton.Id);
		                    });
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
		                        Assert.IsTrue(RadioButton1.Checked, "Should initially be checked");

		                        RadioButton1.Checked = false;
		                        Assert.IsFalse(RadioButton1.Checked, "Should not be checked");

		                        RadioButton1.Checked = true;
		                        Assert.IsTrue(RadioButton1.Checked, "Should be checked");
		                    });
		}

		[Test]
		public void RadioButtons()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.AreEqual(3, browser.RadioButtons.Length, "Unexpected number of RadioButtons");

		                        var formRadioButtons = browser.Form("FormRadioButtons").RadioButtons;

		                        Assert.AreEqual(2, formRadioButtons.Length, "Wrong number off RadioButtons");
		                        Assert.AreEqual("Radio2", formRadioButtons[0].Id);
		                        Assert.AreEqual("Radio3", formRadioButtons[1].Id);

		                        // Collection iteration and comparing the result with Enumerator
		                        IEnumerable radiobuttonEnumerable = formRadioButtons;
		                        var radiobuttonEnumerator = radiobuttonEnumerable.GetEnumerator();

		                        var count = 0;
		                        foreach (RadioButton radioButton in formRadioButtons)
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