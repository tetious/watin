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

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class LabelTests : BaseWithBrowserTests
	{
		[Test]
		public void LabelElementTags()
		{
			Assert.AreEqual(1, Label.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("label", ((ElementTag) Label.ElementTags[0]).TagName);
		}

		[Test]
		public void LabelFromElement()
		{
			Element element = Ie.Element("label", Find.ByFor("Checkbox21"));
			Label label = new Label(element);
			Assert.AreEqual("Checkbox21", label.For);
		}

		[Test]
		public void LabelExists()
		{
			Assert.IsTrue(Ie.Label(Find.ByFor("Checkbox21")).Exists);
			Assert.IsTrue(Ie.Label(Find.ByFor(new Regex("Checkbox21"))).Exists);
			Assert.IsFalse(Ie.Label(Find.ByFor("nonexistingCheckbox21")).Exists);
		}

		[Test]
		public void LabelByFor()
		{
			Label label = Ie.Label(Find.ByFor("Checkbox21"));

			Assert.AreEqual("Checkbox21", label.For, "Unexpected label.For id");
			Assert.AreEqual("label for Checkbox21", label.Text, "Unexpected label.Text");
			Assert.AreEqual("C", label.AccessKey, "Unexpected label.AccessKey");
		}

		[Test]
		public void LabelByForWithElement()
		{
			CheckBox checkBox = Ie.CheckBox("Checkbox21");

			Label label = Ie.Label(Find.ByFor(checkBox));

			Assert.AreEqual("Checkbox21", label.For, "Unexpected label.For id");
			Assert.AreEqual("label for Checkbox21", label.Text, "Unexpected label.Text");
			Assert.AreEqual("C", label.AccessKey, "Unexpected label.AccessKey");
		}

		[Test]
		public void LabelWrapped()
		{
			LabelCollection labelCollection = Ie.Labels;
			Assert.AreEqual(2, Ie.Labels.Length, "Unexpected number of labels");

			Label label = labelCollection[1];

			Assert.AreEqual(null, label.For, "Unexpected label.For id");
			Assert.AreEqual("Test label before:  Test label after", label.Text, "Unexpected label.Text");

			// Element.TextBefore and Element.TextAfter are tested in test method Element
		}

		[Test]
		public void Labels()
		{
			const int expectedLabelCount = 2;

			Assert.AreEqual(expectedLabelCount, Ie.Labels.Length, "Unexpected number of labels");

			LabelCollection labelCollection = Ie.Labels;

			// Collection items by index
			Assert.AreEqual(expectedLabelCount, labelCollection.Length, "Wrong number of labels");

			// Collection iteration and comparing the result with Enumerator
			IEnumerable labelEnumerable = labelCollection;
			IEnumerator labelEnumerator = labelEnumerable.GetEnumerator();

			int count = 0;
			foreach (Label label in labelCollection)
			{
				labelEnumerator.MoveNext();
				object enumCheckbox = labelEnumerator.Current;

				Assert.IsInstanceOfType(label.GetType(), enumCheckbox, "Types are not the same");
				Assert.AreEqual(label.OuterHtml, ((Label) enumCheckbox).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(labelEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(expectedLabelCount, count);
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}