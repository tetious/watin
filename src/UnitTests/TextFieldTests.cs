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

namespace WatiN.Core.UnitTests
{
  using System;
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;
  using WatiN.Core.Exceptions;

  [TestFixture]
  public class TextFieldTests : BaseElementsTests
  {
    [Test]
    public void TextFieldElementTags()
    {
      Assert.AreEqual(2, TextField.ElementTags.Count, "2 elementtags expected");
      Assert.AreEqual("input", ((ElementTag) TextField.ElementTags[0]).TagName);
      Assert.AreEqual("text password textarea hidden", ((ElementTag) TextField.ElementTags[0]).InputTypes);
      Assert.AreEqual("textarea", ((ElementTag) TextField.ElementTags[1]).TagName);
    }

    [Test]
    public void CreateTextFieldFromElementInput()
    {
      Element element = ie.Element("name");
      TextField textField = new TextField(element);
      Assert.AreEqual("name", textField.Id);
    }

    [Test]
    public void CreateTextFieldFromElementTextArea()
    {
      Element element = ie.Element("Textarea1");
      TextField textField = new TextField(element);
      Assert.AreEqual("Textarea1", textField.Id);
    }

    [Test]
    public void TextFieldExists()
    {
      Assert.IsTrue(ie.TextField("name").Exists);
      Assert.IsTrue(ie.TextField(new Regex("name")).Exists);
      Assert.IsFalse(ie.TextField("nonexistingtextfield").Exists);
    }

    [Test]
    public void TextFieldTypTextShouldHandleNewLineInputCorrect()
    {
      TextField textfieldName = ie.TextField("Textarea1");
      string textWithNewLine = "Line1" + Environment.NewLine + "Line2";

      textfieldName.TypeText(textWithNewLine);
      Assert.AreEqual(textWithNewLine, textfieldName.Value);
    }

    [Test]
    public void TextFieldTypTextShouldHonourMaxLength()
    {
      TextField textField = ie.TextField("name");
      int maxLenght = textField.MaxLength;

      textField.TypeText("1".PadLeft(maxLenght));
      Assert.That(textField.Value.Trim(), NUnit.Framework.SyntaxHelpers.Is.EqualTo("1"));

      textField.TypeText("23".PadLeft(maxLenght + 1));
      Assert.That(textField.Value.Trim(), NUnit.Framework.SyntaxHelpers.Is.EqualTo("2"));
    }

    [Test]
    public void TextFieldTest()
    {
      const string value = "Hello world!";
      const string appendValue = " This is WatiN!";
      TextField textfieldName = ie.TextField("name");

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
      Assert.AreEqual("readonlytext", ie.TextField("readonlytext").ToString(), "Expected Id");
      Assert.AreEqual("disabledtext", ie.TextField(Find.ByName("disabledtext")).ToString(), "Expected Name");
    }

    [Test]
    public void TextFieldTypeTextEvents()
    {
      using (IE ie1 = new IE(TestEventsURI))
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
      using (IE ie1 = new IE(FramesetURI))
      {
        Frame frame = ie1.Frames[1];
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
      ie.GoTo(ie.Uri);

      const string value = "Hello world!";
      const string appendValue = " This is WatiN!";
      TextField textfieldName = ie.TextField("Textarea1");

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
      TextField textField = ie.TextField("readonlytext");
      textField.TypeText("This should go wrong");
    }

    [Test, ExpectedException(typeof (ElementDisabledException), ExpectedMessage = "Element with Id:disabledtext is disabled")]
    public void TextFieldDisabledException()
    {
      TextField textField = ie.TextField(Find.ByName("disabledtext"));
      textField.TypeText("This should go wrong");
    }

    [Test, ExpectedException(typeof (ElementNotFoundException), ExpectedMessage = "Could not find a 'INPUT (text password textarea hidden) or TEXTAREA' tag containing attribute id with value 'noneexistingtextfieldid'")]
    public void TextFieldElementNotFoundException()
    {
      IE.Settings.WaitUntilExistsTimeOut = 1;
      ie.TextField("noneexistingtextfieldid").TypeText("");
    }

    [Test]
    public void TextFields()
    {
      Assert.AreEqual(6, ie.TextFields.Length, "Unexpected number of TextFields");

      // Collection items by index
      Form mainForm = ie.Form("FormHiddens");
      Assert.AreEqual(2, mainForm.TextFields.Length, "Wrong number of textfields in collectionTestForm");
      Assert.AreEqual("first", mainForm.TextFields[0].Value);
      Assert.AreEqual("second", mainForm.TextFields[1].Value);

      Form form = ie.Form("Form");

      // Collection.length
      TextFieldCollection textfields = form.TextFields;
      Assert.AreEqual(1, textfields.Length);

      // Collection iteration and comparing the result with Enumerator
      IEnumerable textfieldEnumerable = textfields;
      IEnumerator textfieldEnumerator = textfieldEnumerable.GetEnumerator();

      int count = 0;
      foreach (TextField textField in textfields)
      {
        textfieldEnumerator.MoveNext();
        object enumTextfield = textfieldEnumerator.Current;

        Assert.IsInstanceOfType(textField.GetType(), enumTextfield, "Types are not the same");
        Assert.AreEqual(textField.OuterHtml, ((TextField) enumTextfield).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }

      Assert.IsFalse(textfieldEnumerator.MoveNext(), "Expected last item");

      Assert.AreEqual(1, count);
    }
  }
}