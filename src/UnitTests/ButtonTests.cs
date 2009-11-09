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
using WatiN.Core.Exceptions;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ButtonTests : BaseWithBrowserTests
	{
		[Test]
		public void ButtonCollectionSecondFilterAndOthersShouldNeverThrowInvalidAttributeException()
		{
            ExecuteTest(browser =>
                            {
                                var buttons = browser.Buttons.Filter(Find.ById("testlinkid"));
                                var buttons2 = buttons.Filter(Find.ByFor("Checkbox21"));
                                Assert.AreEqual(0, buttons2.Count);
                                
                            });
		}

		[Test]
		public void ButtonDisabledException()
		{
		    ExecuteTest(browser =>
		                    {
                                // GIVEN
		                        var button = browser.Button("disabledid");
		                        
                                try
		                        {
                                    // WHEN
		                            button.Click();
                                    
                                    // THEN
                                    Assert.Fail("Expected ElementDisabledException");
		                        }
		                        catch (Exception e)
		                        {
		                            Assert.That(e, Is.InstanceOfType(typeof(ElementDisabledException)), "Unexpected exception");
		                        }
		                    });

		}

        [Test]
        public void ButtonClick()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                browser.GoTo(ButtonTestsUri);

                                // WHEN
                                browser.Button("button1").Click();

                                // THEN
                                var value = browser.TextField("report").Value;
                                Assert.That(value, Is.EqualTo("clicked"));
                            });
        }

		[Test]
		public void ButtonElementNotFoundException()
		{
		    ExecuteTest(browser =>
		                    {
		                        try
		                        {
		                            Settings.WaitUntilExistsTimeOut = 1;
		                            browser.Button("noneexistingbuttonid").Click();
		                            Assert.Fail("Expected ElementNotFoundException");
		                        }
		                        catch (ElementNotFoundException e)
		                        {
                                    Assert.That(e.Message, Text.StartsWith("Could not find BUTTON or INPUT (button) or INPUT (reset) or INPUT (submit) element tag matching criteria: Attribute 'id' equals 'noneexistingbuttonid' at file://"));
		                            Assert.That(e.Message, Text.EndsWith("main.html"));
		                        }

		                    });

		}

		[Test]
		public void ButtonExists()
		{
		    ExecuteTest(browser =>
		                    {
		                        // Test <input type=button />
		                        Assert.IsTrue(browser.Button("disabledid").Exists);
		                        // Test <Button />
                                Assert.IsTrue(browser.Button("buttonelementid").Exists);

                                Assert.IsTrue(browser.Button(b => b.Id == "buttonelementid").Exists);

                                Assert.IsFalse(browser.Button("noneexistingbuttonid").Exists);

		                    });

		}

		[Test]
		public void Buttons()
		{
		    ExecuteTest(browser =>
		                    {
		                        const int expectedButtonsCount = 6;
                                Assert.AreEqual(expectedButtonsCount, browser.Buttons.Count, "Unexpected number of buttons");

		                        const int expectedFormButtonsCount = 5;
		                        var form = browser.Form("Form");

		                        // Collection.Length
		                        var formButtons = form.Buttons;

                                Assert.AreEqual(expectedFormButtonsCount, formButtons.Count);

		                        // Collection items by index
		                        Assert.AreEqual("popupid", form.Buttons[0].Id);
		                        Assert.AreEqual("modalid", form.Buttons[1].Id);
		                        Assert.AreEqual("helloid", form.Buttons[2].Id);

		                        // Exists
		                        Assert.IsTrue(form.Buttons.Exists("modalid"));
		                        Assert.IsTrue(form.Buttons.Exists(new Regex("modalid")));
		                        Assert.IsFalse(form.Buttons.Exists("nonexistingid"));

		                        IEnumerable buttonEnumerable = formButtons;
		                        var buttonEnumerator = buttonEnumerable.GetEnumerator();

		                        // Collection iteration and comparing the result with Enumerator
		                        var count = 0;
		                        foreach (var inputButton in formButtons)
		                        {
		                            buttonEnumerator.MoveNext();
		                            var enumButton = buttonEnumerator.Current;

		                            Assert.IsInstanceOfType(inputButton.GetType(), enumButton, "Types are not the same");
		                            Assert.AreEqual(inputButton.OuterHtml, ((Button) enumButton).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(buttonEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(expectedFormButtonsCount, count);
		                    });

		}

		[Test]
		public void ButtonsFilterOnHTMLElementCollection()
		{
		    ExecuteTest(browser =>
		                    {
		                        var buttons = browser.Buttons.Filter(Find.ById(new Regex("le")));
                                Assert.AreEqual(2, buttons.Count);
		                        Assert.AreEqual("disabledid", buttons[0].Id);
		                        Assert.AreEqual("buttonelementid", buttons[1].Id);
		                    });

		}

		[Test]
		public void ButtonsFilterOnArrayListElements()
		{
		    ExecuteTest(browser =>
		                    {
		                        var buttons = browser.Buttons;
                                Assert.AreEqual(6, buttons.Count);

		                        buttons = browser.Buttons.Filter(Find.ById(new Regex("le")));
                                Assert.AreEqual(2, buttons.Count);
		                        Assert.AreEqual("disabledid", buttons[0].Id);
		                        Assert.AreEqual("buttonelementid", buttons[1].Id);
		                    });

		}

		[Test]
		public void ButtonElementTags()
		{
            var elementTags = ElementFactory.GetElementTags<Button>();
            Assert.AreEqual(4, elementTags.Count, "4 elementtags expected");

            Assert.That(elementTags[0], Is.EqualTo(new ElementTag("input", "button")));
            Assert.That(elementTags[1], Is.EqualTo(new ElementTag("input", "submit")));
            Assert.That(elementTags[2], Is.EqualTo(new ElementTag("input", "reset")));
            Assert.That(elementTags[3], Is.EqualTo(new ElementTag("button")));
		}

		[Test]
		public void ButtonFromInputElement()
		{
		    ExecuteTest(browser =>
		                    {
		                        const string popupValue = "Show modeless dialog";
		                        var button = browser.Button(Find.ById("popupid"));

		                        Assert.IsInstanceOfType(typeof (Element), button);
		                        Assert.IsInstanceOfType(typeof (Button), button);

		                        Assert.AreEqual(popupValue, button.Value);
		                        Assert.AreEqual(popupValue, browser.Button("popupid").Value);
		                        Assert.AreEqual(popupValue, browser.Button("popupid").ToString());
		                        Assert.AreEqual(popupValue, browser.Button(Find.ByName("popupname")).Value);

		                        var helloButton = browser.Button("helloid");
		                        Assert.AreEqual("Show allert", helloButton.Value);
		                        Assert.AreEqual(helloButton.Value, helloButton.Text);

		                        Assert.IsTrue(browser.Button(new Regex("popupid")).Exists);
		                    });

		}

		[Test]
		public void ButtonFromButtonElement()
		{
		    ExecuteTest(browser =>
		                    {
                                var Value = "Button Element";
                                if (browser.GetType().Equals(typeof(FireFox)))
                                {
                                    Value = "ButtonElementValue";
                                }

		                        var button = browser.Button(Find.ById("buttonelementid"));

		                        Assert.IsInstanceOfType(typeof (Element), button);
		                        Assert.IsInstanceOfType(typeof (Button), button);

		                        Assert.AreEqual(Value, browser.Button("buttonelementid").Value);
		                        Assert.AreEqual(Value, browser.Button("buttonelementid").ToString());
		                        Assert.AreEqual(Value, browser.Button(Find.ByName("buttonelementname")).Value);

		                        Assert.AreEqual(Value, browser.Button(Find.ByText("Button Element")).Value);
		                        
		                        Assert.IsTrue(browser.Button(new Regex("buttonelementid")).Exists);
		                    });

		}

	    [Test]
	    public void FindByValueBehavesDifferentlyForIEThenFireFox()
	    {
            // OK, this one is weird. The HTML says value="ButtonElementValue"
            // but the value attribute returns the innertext(!) in IE6, IE7 and IE8
            // But FireFox does return the value attribute.....
            // <button id="buttonelementid" name="buttonelementname" value="ButtonElementValue">Button Element</button>

            const string ieValue = "Button Element";
            const string actualValue = "ButtonElementValue";

        	Assert.That(Ie.Button(Find.ByValue(ieValue)).Exists, Is.True, "IE issue");
            Assert.That(Firefox.Button(Find.ByValue(actualValue)).Exists, Is.True, "Firefox issue");
	    }

	    [Test]
	    public void ValueReturnsDifferentValueForIEThenFireFoxAndIE8()
	    {
            // OK, this one is weird. The HTML says value="ButtonElementValue"
            // but the value attribute returns the innertext(!) in IE6, IE7 and IE8.
            // But FireFox does return the value attribute.....
            // <button id="buttonelementid" name="buttonelementname" value="ButtonElementValue">Button Element</button>

            const string ieValue = "Button Element";
            const string actualValue = "ButtonElementValue";

            Assert.AreEqual(ieValue, Ie.Button("buttonelementid").Value, "IE issue");
            Assert.AreEqual(actualValue, Firefox.Button("buttonelementid").Value, "FireFox issue");
	    }

        [Test]
        public void SimpleButtonTest()
        {
            ExecuteTest(browser =>
                            {
                                Assert.That(browser.Button("buttonelementid").Id, Is.EqualTo("buttonelementid"));
                                Assert.That(browser.Button(Find.ByName("buttonelementname")).Id, Is.EqualTo("buttonelementid"));
                            });

        }

	    [Test]
	    public void ShouldFindButtonInsideForm()
	    {
            ExecuteTest(browser => Assert.That(browser.Form("Form").Button("helloid").Exists, Is.True, "Expected button to be found"));
	    }


	    [Test]
        public void Should_not_throw_element_not_found_exception_when_referencing_a_collection_contained_in_a_non_existing_element()
        {
	        // GIVEN
	        ExecuteTestWithAnyBrowser(browser =>
              {
                  // GIVEN
                  Settings.WaitUntilExistsTimeOut = 3;

                  Assert.That(browser.Div("I_dont_exist").Exists, Is.False, "Pre-condition failed. Div should not exist.");

                  try
                  {
                      // WHEN
                      var buttons = browser.Div("I_dont_exist").Buttons;
                      
                      // THEN success

                  }
                  catch (ElementNotFoundException)
                  {
                      Assert.Fail("Should not have thrown an ElementNotFoundException for the div");
                  }
                  catch(Exception e)
                  {
                      Assert.Fail("Unexpected exception: " + e.ToString());
                  }
              });
	    }

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}
