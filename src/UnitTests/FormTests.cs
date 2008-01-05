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

using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class FormTests : WatiNTest
	{
		private IE ie = new IE();

		[SetUp]
		public void TestSetup()
		{
			if (!ie.Uri.Equals(WatiNTest.FormSubmitURI))
			{
				ie.GoTo(FormSubmitURI);
			}
		}

		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			ie.Close();
		}

		[Test]
		public void FormElementTags()
		{
			Assert.AreEqual(1, Form.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("form", ((ElementTag) Form.ElementTags[0]).TagName);
		}

		[Test]
		public void ImageFromElementImage()
		{
			Element element = ie.Element("Form1");
			Form form = new Form(element);
			Assert.AreEqual("Form1", form.Id);
		}

		[Test]
		public void FormExists()
		{
			Assert.IsTrue(ie.Form("Form1").Exists);
			Assert.IsTrue(ie.Form(new Regex("Form1")).Exists);
			Assert.IsFalse(ie.Form("nonexistingForm").Exists);
		}

		[Test]
		public void FormSubmit()
		{
			ie.Form("Form1").Submit();

			Assert.AreEqual(ie.Uri, MainURI);
		}

		[Test]
		public void FormSubmitBySubmitButton()
		{
			ie.Button("submitbutton").Click();

			Assert.AreEqual(ie.Uri, MainURI);
		}

		[Test]
		public void FormTest()
		{
			Form form = ie.Form("Form2");

			Assert.IsInstanceOfType(typeof (ElementsContainer), form);
			Assert.AreEqual("Form2", form.Id, "Unexpected Id");
			Assert.AreEqual("form2name", form.Name, "Unexpected Name");
			Assert.AreEqual("Form title", form.Title, "Unexpected Title");
		}

		[Test]
		public void Forms()
		{
			ie.GoTo(MainURI);

			Assert.AreEqual(6, ie.Forms.Length, "Unexpected number of forms");

			FormCollection forms = ie.Forms;

			// Collection items by index
			Assert.AreEqual("Form", forms[0].Id);
			Assert.AreEqual("FormInputElement", forms[1].Id);
			Assert.AreEqual("FormHiddens", forms[2].Id);
			Assert.AreEqual("ReadyOnlyDisabledInputs", forms[3].Id);
			Assert.AreEqual("FormCheckboxes", forms[4].Id);
			Assert.AreEqual("FormRadioButtons", forms[5].Id);

			// Collection iteration and comparing the result with Enumerator
			IEnumerable checkboxEnumerable = forms;
			IEnumerator checkboxEnumerator = checkboxEnumerable.GetEnumerator();

			int count = 0;
			foreach (Form form in forms)
			{
				checkboxEnumerator.MoveNext();
				object enumCheckbox = checkboxEnumerator.Current;

				Assert.IsInstanceOfType(form.GetType(), enumCheckbox, "Types are not the same");
				Assert.AreEqual(form.OuterHtml, ((Form) enumCheckbox).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(checkboxEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(6, count);
		}

		[Test]
		public void FormToStringWithTitleIdAndName()
		{
			Assert.AreEqual("Form title", ie.Form("Form2").ToString(), "Title expected");
			Assert.AreEqual("Form3", ie.Form("Form3").ToString(), "Id expected");
			Assert.AreEqual("form4name", ie.Form(Find.ByName("form4name")).ToString(), "Name expected");
			Assert.AreEqual("This is a form with no ID, Title or name.", ie.Forms[4].ToString(), "Text expected");
		}
	}
}