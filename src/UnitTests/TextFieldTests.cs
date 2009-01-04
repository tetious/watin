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
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Exceptions;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class TextFieldTests : BaseWithBrowserTests
	{
		[Test]
		public void TextFieldElementTags()
		{
			Assert.AreEqual(2, TextField.ElementTags.Count, "2 elementtags expected");
			Assert.AreEqual("input", TextField.ElementTags[0].TagName);
			Assert.AreEqual("text password textarea hidden", TextField.ElementTags[0].InputTypes);
			Assert.AreEqual("textarea", TextField.ElementTags[1].TagName);
		}

		[Test]
		public void CreateTextFieldFromElementInput()
		{
			var element = Ie.Element("name");
			var textField = new TextField(element);
			Assert.AreEqual("name", textField.Id);
		}

		[Test]
		public void CreateTextFieldFromElementTextArea()
		{
			var element = Ie.Element("Textarea1");
			var textField = new TextField(element);
			Assert.AreEqual("Textarea1", textField.Id);
		}

		[Test]
		public void TextFieldExists()
		{
			Assert.IsTrue(Ie.TextField("name").Exists);
			Assert.IsTrue(Ie.TextField(new Regex("name")).Exists);
			Assert.IsFalse(Ie.TextField("nonexistingtextfield").Exists);
		}

		[Test]
		public void TextFieldTypTextShouldHandleNewLineInputCorrect()
		{
			var textfieldName = Ie.TextField("Textarea1");
			var textWithNewLine = "Line1" + Environment.NewLine + "Line2";

			textfieldName.TypeText(textWithNewLine);
			Assert.AreEqual(textWithNewLine, textfieldName.Value);
		}

		[Test]
		public void TextFieldTypTextShouldHonourMaxLength()
		{
			var textField = Ie.TextField("name");
			var maxLenght = textField.MaxLength;

			textField.TypeText("1".PadLeft(maxLenght));
			Assert.That(textField.Value.Trim(), Is.EqualTo("1"));

			textField.TypeText("23".PadLeft(maxLenght + 1));
			Assert.That(textField.Value.Trim(), Is.EqualTo("2"));
		}

		[Test]
		public void TextFieldTest()
		{
			const string value = "Hello world!";
			const string appendValue = " This is WatiN!";
			var textfieldName = Ie.TextField("name");

			Assert.AreEqual("name", textfieldName.Id);
			Assert.AreEqual("textinput1", textfieldName.Name);

			Assert.AreEqual(200, textfieldName.MaxLength, "Unexpected maxlenght");

			textfieldName.TypeText(value);
			Assert.AreEqual(value, textfieldName.Value, "TypeText not OK");

			textfieldName.AppendText(appendValue);
			Assert.AreEqual(value + appendValue, textfieldName.Value, "AppendText not OK");

			textfieldName.Clear();
			Assert.IsNull(textfieldName.Value, "After Clear value should by null");

			textfieldName.Value = value;
			Assert.AreEqual(value, textfieldName.Value, "Value not OK");
			Assert.AreEqual(value, textfieldName.Text, "Text not OK");

			Assert.AreEqual("Textfield title", textfieldName.ToString(), "Expected Title");
			Assert.AreEqual("readonlytext", Ie.TextField("readonlytext").ToString(), "Expected Id");
			Assert.AreEqual("disabledtext", Ie.TextField(Find.ByName("disabledtext")).ToString(), "Expected Name");
		}

		[Test]
		public void TextFieldTypeTextEvents()
		{
			using (var ie1 = new IE(TestEventsURI))
			{
				Assert.IsFalse(ie1.CheckBox("chkKeyDown").Checked, "KeyDown false expected");
				Assert.IsFalse(ie1.CheckBox("chkKeyPress").Checked, "KeyPress false expected");
				Assert.IsFalse(ie1.CheckBox("chkKeyUp").Checked, "KeyUp false expected");

				const string text = "test";
				ie1.TextField("textfieldid").TypeText(text);

				Assert.IsTrue(ie1.CheckBox("chkKeyDown").Checked, "KeyDown event expected");
				Assert.IsTrue(ie1.CheckBox("chkKeyPress").Checked, "KeyPress event expected");
				Assert.IsTrue(ie1.CheckBox("chkKeyUp").Checked, "KeyUp event expected");

				Assert.AreEqual(text, ie1.TextField("txtKeycodeId").Value, "KeyUp event expected");
			}
		}

		[Test]
		public void TextFieldTypeTextEventsInFrame()
		{
			using (var ie1 = new IE(FramesetURI))
			{
				var frame = ie1.Frames[1];
				frame.RunScript("window.document.location.href='TestEvents.html';");

				Assert.IsFalse(frame.CheckBox("chkKeyDown").Checked, "KeyDown false expected");
				Assert.IsFalse(frame.CheckBox("chkKeyPress").Checked, "KeyPress false expected");
				Assert.IsFalse(frame.CheckBox("chkKeyUp").Checked, "KeyUp false expected");

				frame.TextField("textfieldid").TypeText("test");

				Assert.IsTrue(frame.CheckBox("chkKeyDown").Checked, "KeyDown event expected");
				Assert.IsTrue(frame.CheckBox("chkKeyPress").Checked, "KeyPress event expected");
				Assert.IsTrue(frame.CheckBox("chkKeyUp").Checked, "KeyUp event expected");
			}
		}

		[Test]
		public void TextFieldTextAreaElement()
		{
			Ie.GoTo(Ie.Uri);

			const string value = "Hello world!";
			const string appendValue = " This is WatiN!";
			var textfieldName = Ie.TextField("Textarea1");

			Assert.AreEqual("Textarea1", textfieldName.Id);
			Assert.AreEqual("TextareaName", textfieldName.Name);

			Assert.AreEqual(0, textfieldName.MaxLength, "Unexpected maxlenght");

			Assert.IsNull(textfieldName.Value, "Initial value should be null");

			textfieldName.TypeText(value);
			Assert.AreEqual(value, textfieldName.Value, "TypeText not OK");

			textfieldName.AppendText(appendValue);
			Assert.AreEqual(value + appendValue, textfieldName.Value, "AppendText not OK");

			textfieldName.Clear();
			Assert.IsNull(textfieldName.Value, "After Clear value should by null");

			textfieldName.Value = value;
			Assert.AreEqual(value, textfieldName.Value, "Value not OK");
			Assert.AreEqual(value, textfieldName.Text, "Text not OK");
		}

		[Test, ExpectedException(typeof (ElementReadOnlyException), ExpectedMessage = "Element with Id:readonlytext is readonly")]
		public void TextFieldReadyOnlyException()
		{
			var textField = Ie.TextField("readonlytext");
			textField.TypeText("This should go wrong");
		}

		[Test, ExpectedException(typeof (ElementDisabledException), ExpectedMessage = "Element with Id:disabledtext is disabled")]
		public void TextFieldDisabledException()
		{
			var textField = Ie.TextField(Find.ByName("disabledtext"));
			textField.TypeText("This should go wrong");
		}

		[Test]
		public void TextFieldElementNotFoundException()
		{
		    try
		    {
		        Settings.WaitUntilExistsTimeOut = 1;
		        Ie.TextField("noneexistingtextfieldid").TypeText("");
                Assert.Fail("Expected ElementNotFoundException");
            }
		    catch (ElementNotFoundException e)
		    {
                Assert.That(e.Message, Text.StartsWith("Could not find INPUT (text password textarea hidden) or TEXTAREA element tag matching criteria: Attribute 'id' with value 'noneexistingtextfieldid' at file://"));
                Assert.That(e.Message, Text.EndsWith("main.html"));
		    }

		}

		[Test]
		public void TextFields()
		{
			Assert.AreEqual(6, Ie.TextFields.Length, "Unexpected number of TextFields");

			// Collection items by index
			var mainForm = Ie.Form("FormHiddens");
			Assert.AreEqual(2, mainForm.TextFields.Length, "Wrong number of textfields in collectionTestForm");
			Assert.AreEqual("first", mainForm.TextFields[0].Value);
			Assert.AreEqual("second", mainForm.TextFields[1].Value);

			var form = Ie.Form("Form");

			// Collection.length
			var textfields = form.TextFields;
			Assert.AreEqual(1, textfields.Length);

			// Collection iteration and comparing the result with Enumerator
			IEnumerable textfieldEnumerable = textfields;
			var textfieldEnumerator = textfieldEnumerable.GetEnumerator();

			var count = 0;
			foreach (TextField textField in textfields)
			{
				textfieldEnumerator.MoveNext();
				var enumTextfield = textfieldEnumerator.Current;

				Assert.IsInstanceOfType(textField.GetType(), enumTextfield, "Types are not the same");
				Assert.AreEqual(textField.OuterHtml, ((TextField) enumTextfield).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(textfieldEnumerator.MoveNext(), "Expected last item");

			Assert.AreEqual(1, count);
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

        [Test]
        public void TextFieldOfElementE()
        {
            var textField = Ie.TextField("name");
            textField.WaitUntil(t => t.Enabled);
        }
	}
}