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
	public class FormTests : BaseWithBrowserTests
	{
		public override Uri TestPageUri
		{
			get { return FormSubmitURI; }
		}

		[Test]
		public void FormElementTags()
		{
            IList<ElementTag> elementTags = ElementFactory.GetElementTags<Form>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("form", elementTags[0].TagName);
		}

		[Test]
		public void FormExists()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.IsTrue(browser.Form("Form1").Exists);
		                        Assert.IsTrue(browser.Form(new Regex("Form1")).Exists);
		                        Assert.IsFalse(browser.Form("nonexistingForm").Exists);
		                    });
		}

		[Test]
		public void FormSubmit()
		{
		    ExecuteTest(browser =>
		                    {
		                        browser.Form("Form1").Submit();

		                        Assert.AreEqual(browser.Uri, MainURI);
		                    });
		}

		[Test]
		public void FormSubmitBySubmitButton()
		{
		    ExecuteTest(browser =>
		                    {
		                        browser.Button("submitbutton").Click();

		                        Assert.AreEqual(browser.Uri, MainURI);
		                    });
		}

		[Test]
		public void FormTest()
		{
		    ExecuteTest(browser =>
		                    {
		                        var form = browser.Form("Form2");

		                        Assert.IsInstanceOfType(typeof (IElementContainer), form);
		                        Assert.IsInstanceOfType(typeof (ElementContainer<Form>), form);
		                        Assert.AreEqual("Form2", form.Id, "Unexpected Id");
		                        Assert.AreEqual("form2name", form.Name, "Unexpected Name");
		                        Assert.AreEqual("Form title", form.Title, "Unexpected Title");
		                    });
		}

		[Test]
		public void Forms()
		{
		    ExecuteTest(browser =>
		                    {
		                        browser.GoTo(MainURI);

		                        Assert.AreEqual(6, browser.Forms.Count, "Unexpected number of forms");

		                        var forms = browser.Forms;

		                        // Collection items by index
		                        Assert.AreEqual("Form", forms[0].Id);
		                        Assert.AreEqual("FormInputElement", forms[1].Id);
		                        Assert.AreEqual("FormHiddens", forms[2].Id);
		                        Assert.AreEqual("ReadyOnlyDisabledInputs", forms[3].Id);
		                        Assert.AreEqual("FormCheckboxes", forms[4].Id);
		                        Assert.AreEqual("FormRadioButtons", forms[5].Id);

		                        // Collection iteration and comparing the result with Enumerator
		                        IEnumerable checkboxEnumerable = forms;
		                        var checkboxEnumerator = checkboxEnumerable.GetEnumerator();

		                        var count = 0;
		                        foreach (Form form in forms)
		                        {
		                            checkboxEnumerator.MoveNext();
		                            var enumCheckbox = checkboxEnumerator.Current;

		                            Assert.IsInstanceOfType(form.GetType(), enumCheckbox, "Types are not the same");
		                            Assert.AreEqual(form.OuterHtml, ((Form) enumCheckbox).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(checkboxEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(6, count);
		                    });
		}

		[Test]
		public void FormToStringWithTitleIdAndName()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.AreEqual("Form title", browser.Form("Form2").ToString(), "Title expected");
		                        Assert.AreEqual("Form3", browser.Form("Form3").ToString(), "Id expected");
		                        Assert.AreEqual("form4name", browser.Form(Find.ByName("form4name")).ToString(), "Name expected");
		                        Assert.AreEqual("This is a form with no ID, Title or name.", browser.Forms[4].ToString(), "Text expected");
		                    });
		}

	    [Test]
        public void ShouldFindFormWithGivenText()
	    {
	        ExecuteTest(browser =>
	                        {
	                            // GIVEN
                                var formWithText = "This is a form with no ID, Title or name.";

	                            // WHEN
	                            var exists = browser.Form(Find.ByText(formWithText)).Exists;

	                            // THEN
	                            Assert.That(exists, Is.True);
	                        });
	    }

	    [Test]
	    public void TextContentShouldMatchWithInnerText()
	    {
	        var divs = Ie.Divs.Filter(Find.ById(new Regex("^innerTextTest")));

	        foreach (Div div in divs)
	        {
                if (div.Id == "innerTextTest7") continue; // See also following Ignored test
	            Assert.That(Firefox.Div(div.Id).Text, Is.EqualTo(div.Text), "failed for div: " + div.Id);
	        }
        }

        // TODO: improve innerText simulation of FireFox
        [Test, Ignore("improve innerText simulation of FireFox")]
	    public void TextContent_not_the_same_between_ie_and_firefox_is_an_JSElement_issue()
	    {
	        var div = Ie.Div("innerTextTest7");
            Assert.That(Firefox.Div("innerTextTest7").Text, Is.Not.EqualTo(div.Text), "Congratulations you have fixed this issue! And yes because of that this test is failing :-)");
        }
	}
}