#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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
using WatiN.Core.Exceptions;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ButtonTests : BaseElementsTests
	{
		[Test]
		public void ButtonCollectionSecondFilterAndOthersShouldNeverThrowInvalidAttributeException()
		{
			ButtonCollection buttons = ie.Buttons.Filter(Find.ById("testlinkid"));
			ButtonCollection buttons2 = buttons.Filter(Find.ByFor("Checkbox21"));
			Assert.AreEqual(0, buttons2.Length);
		}

		[Test, ExpectedException(typeof (ElementDisabledException))]
		public void ButtonDisabledException()
		{
			ie.Button("disabledid").Click();
		}

		[Test, ExpectedException(typeof (ElementNotFoundException), ExpectedMessage = "Could not find INPUT (button submit image reset) or BUTTON element tag matching criteria: Attribute 'id' with value 'noneexistingbuttonid'")]
		public void ButtonElementNotFoundException()
		{
			IE.Settings.WaitUntilExistsTimeOut = 1;
			ie.Button("noneexistingbuttonid").Click();
		}

		[Test]
		public void ButtonExists()
		{
			// Test <input type=button />
			Assert.IsTrue(ie.Button("disabledid").Exists);
			// Test <Button />
			Assert.IsTrue(ie.Button("buttonelementid").Exists);

			Assert.IsFalse(ie.Button("noneexistingbuttonid").Exists);
		}

		[Test]
		public void CreateButtonFromInputHTMLElement()
		{
			Element element = ie.Element("disabledid");
			Button button = new Button(element);
			Assert.AreEqual("disabledid", button.Id);
		}

		[Test]
		public void CreateButtonFromButtonHTMLElement()
		{
			Element element = ie.Element("buttonelementid");
			Button button = new Button(element);
			Assert.AreEqual("buttonelementid", button.Id);
		}

		[Test, ExpectedException(typeof (ArgumentException))]
		public void ButtonFromElementArgumentException()
		{
			Element element = ie.Element("Checkbox1");
			new Button(element);
		}

		[Test]
		public void Buttons()
		{
			const int expectedButtonsCount = 5;
			Assert.AreEqual(expectedButtonsCount, ie.Buttons.Length, "Unexpected number of buttons");

			const int expectedFormButtonsCount = 4;
			Form form = ie.Form("Form");

			// Collection.Length
			ButtonCollection formButtons = form.Buttons;

			Assert.AreEqual(expectedFormButtonsCount, formButtons.Length);

			// Collection items by index
			Assert.AreEqual("popupid", form.Buttons[0].Id);
			Assert.AreEqual("modalid", form.Buttons[1].Id);
			Assert.AreEqual("helloid", form.Buttons[2].Id);

			// Exists
			Assert.IsTrue(form.Buttons.Exists("modalid"));
			Assert.IsTrue(form.Buttons.Exists(new Regex("modalid")));
			Assert.IsFalse(form.Buttons.Exists("nonexistingid"));

			IEnumerable buttonEnumerable = formButtons;
			IEnumerator buttonEnumerator = buttonEnumerable.GetEnumerator();

			// Collection iteration and comparing the result with Enumerator
			int count = 0;
			foreach (Button inputButton in formButtons)
			{
				buttonEnumerator.MoveNext();
				object enumButton = buttonEnumerator.Current;

				Assert.IsInstanceOfType(inputButton.GetType(), enumButton, "Types are not the same");
				Assert.AreEqual(inputButton.OuterHtml, ((Button) enumButton).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(buttonEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(expectedFormButtonsCount, count);
		}

		[Test]
		public void ButtonsFilterOnHTMLElementCollection()
		{
			ButtonCollection buttons = ie.Buttons.Filter(Find.ById(new Regex("le")));
			Assert.AreEqual(2, buttons.Length);
			Assert.AreEqual("disabledid", buttons[0].Id);
			Assert.AreEqual("buttonelementid", buttons[1].Id);
		}

		[Test]
		public void ButtonsFilterOnArrayListElements()
		{
			ButtonCollection buttons = ie.Buttons;
			Assert.AreEqual(5, buttons.Length);

			buttons = ie.Buttons.Filter(Find.ById(new Regex("le")));
			Assert.AreEqual(2, buttons.Length);
			Assert.AreEqual("disabledid", buttons[0].Id);
			Assert.AreEqual("buttonelementid", buttons[1].Id);
		}

		[Test]
		public void ButtonElementTags()
		{
			Assert.AreEqual(2, Button.ElementTags.Count, "2 elementtags expected");
			Assert.AreEqual("input", ((ElementTag) Button.ElementTags[0]).TagName);
			Assert.AreEqual("button submit image reset", ((ElementTag) Button.ElementTags[0]).InputTypes);
			Assert.AreEqual("button", ((ElementTag) Button.ElementTags[1]).TagName);
		}

		[Test]
		public void ButtonFromInputElement()
		{
			const string popupValue = "Show modeless dialog";
			Button button = ie.Button(Find.ById("popupid"));

			Assert.IsInstanceOfType(typeof (Element), button);
			Assert.IsInstanceOfType(typeof (Button), button);

			Assert.AreEqual(popupValue, button.Value);
			Assert.AreEqual(popupValue, ie.Button("popupid").Value);
			Assert.AreEqual(popupValue, ie.Button("popupid").ToString());
			Assert.AreEqual(popupValue, ie.Button(Find.ByName("popupname")).Value);

			Button helloButton = ie.Button("helloid");
			Assert.AreEqual("Show allert", helloButton.Value);
			Assert.AreEqual(helloButton.Value, helloButton.Text);

			Assert.IsTrue(ie.Button(new Regex("popupid")).Exists);
		}

		[Test]
		public void ButtonFromButtonElement()
		{
			const string Value = "Button Element";

			Button button = ie.Button(Find.ById("buttonelementid"));

			Assert.IsInstanceOfType(typeof (Element), button);
			Assert.IsInstanceOfType(typeof (Button), button);

			Assert.AreEqual(Value, button.Value);
			Assert.AreEqual(Value, ie.Button("buttonelementid").Value);
			Assert.AreEqual(Value, ie.Button("buttonelementid").ToString());
			Assert.AreEqual(Value, ie.Button(Find.ByName("buttonelementname")).Value);

			Assert.AreEqual(Value, ie.Button(Find.ByText("Button Element")).Value);
			// OK, this one is weird. The HTML says value="ButtonElementValue"
			// but the value attribute seems to return the innertext(!)
			// <button id="buttonelementid" name="buttonelementname" value="ButtonElementValue">Button Element</button>
			Assert.AreEqual(Value, ie.Button(Find.ByValue("Button Element")).Value);

			Assert.IsTrue(ie.Button(new Regex("buttonelementid")).Exists);
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}