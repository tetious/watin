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
using WatiN.Core.Interfaces;

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
			Assert.AreEqual(1, Form.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("form", Form.ElementTags[0].TagName);
		}

		[Test]
		public void ImageFromElementImage()
		{
			var element = Ie.Element("Form1");
			var form = new Form(element);
			Assert.AreEqual("Form1", form.Id);
		}

		[Test]
		public void FormExists()
		{
			Assert.IsTrue(Ie.Form("Form1").Exists);
			Assert.IsTrue(Ie.Form(new Regex("Form1")).Exists);
			Assert.IsFalse(Ie.Form("nonexistingForm").Exists);
		}

		[Test]
		public void FormSubmit()
		{
			Ie.Form("Form1").Submit();

			Assert.AreEqual(Ie.Uri, MainURI);
		}

		[Test]
		public void FormSubmitBySubmitButton()
		{
			Ie.Button("submitbutton").Click();

			Assert.AreEqual(Ie.Uri, MainURI);
		}

		[Test]
		public void FormTest()
		{
			var form = Ie.Form("Form2");

			Assert.IsInstanceOfType(typeof (IElementsContainer), form);
			Assert.IsInstanceOfType(typeof (ElementsContainer<Form>), form);
            Assert.AreEqual("Form2", form.Id, "Unexpected Id");
			Assert.AreEqual("form2name", form.Name, "Unexpected Name");
			Assert.AreEqual("Form title", form.Title, "Unexpected Title");
		}

		[Test]
		public void Forms()
		{
			Ie.GoTo(MainURI);

			Assert.AreEqual(6, Ie.Forms.Length, "Unexpected number of forms");

			var forms = Ie.Forms;

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
		}

		[Test]
		public void FormToStringWithTitleIdAndName()
		{
			Assert.AreEqual("Form title", Ie.Form("Form2").ToString(), "Title expected");
			Assert.AreEqual("Form3", Ie.Form("Form3").ToString(), "Id expected");
			Assert.AreEqual("form4name", Ie.Form(Find.ByName("form4name")).ToString(), "Name expected");
			Assert.AreEqual("This is a form with no ID, Title or name.", Ie.Forms[4].ToString(), "Text expected");
		}

	}
}