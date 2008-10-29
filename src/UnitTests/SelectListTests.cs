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
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using NUnit.Framework;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class SelectListTests : BaseWithIETests
	{
		[Test]
		public void SupportedElementTags()
		{
			Assert.AreEqual(1, SelectList.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("select", ((ElementTag) SelectList.ElementTags[0]).TagName);
		}

		[Test]
		public void SelectListFromElement()
		{
			Element element = ie.Element("Select2");
			SelectList selectList = new SelectList(element);
			Assert.AreEqual("Select2", selectList.Id);
		}

		[Test]
		public void MultipleSelectListExists()
		{
			Assert.IsTrue(ie.SelectList("Select2").Exists);
			Assert.IsTrue(ie.SelectList(new Regex("Select2")).Exists);
			Assert.IsFalse(ie.SelectList("nonexistingSelect2").Exists);
		}

		[Test]
		public void MultipleSelectList()
		{
			SelectList selectList = ie.SelectList("Select2");

			Assert.IsNotNull(selectList, "SelectList niet aangetroffen");

			StringCollection items = selectList.AllContents;

			Assert.AreEqual(4, items.Count);
			Assert.AreEqual("First Listitem", items[0], "First Listitem not found");
			Assert.AreEqual("Second Listitem", items[1], "Second Listitem not found");
			Assert.AreEqual("Third Listitem", items[2], "Third Listitem not found");
			Assert.AreEqual("Fourth Listitem", items[3], "Fourth Listitem not found");

			Assert.IsTrue(selectList.Multiple, "'Select 2' must be allow multiple selection to pass the next tests");

			// Second Listitem is selected by default/loading the page
			Assert.AreEqual(1, selectList.SelectedItems.Count, "SelectedItem not selected on page load");

			selectList.ClearList();
			Assert.AreEqual(0, selectList.SelectedItems.Count, "After ClearList there are still items selected");
			Assert.IsFalse(selectList.HasSelectedItems, "No selected items expected");

			selectList.Select("Third Listitem");

			Assert.IsTrue(selectList.HasSelectedItems, "Selecteditems expected after first select");
			Assert.AreEqual(1, selectList.SelectedItems.Count, "Wrong number of items selected after Select Third Listitem");
			Assert.AreEqual("Third Listitem", selectList.SelectedItems[0], "Third Listitem not selected after Select");

			selectList.Select("First Listitem");

			Assert.IsTrue(selectList.HasSelectedItems, "Selecteditems expected after second select");
			Assert.AreEqual(2, selectList.SelectedItems.Count, "Wrong number of items selected after Select First Listitem");
			Assert.AreEqual("First Listitem", selectList.SelectedItems[0], "First Listitem not selected after Select");
			Assert.AreEqual("Third Listitem", selectList.SelectedItems[1], "Third Listitem not selected after Select");
		}

		[Test, ExpectedException(typeof (SelectListItemNotFoundException), ExpectedMessage = "No item was found in the selectlist matching constraint: Attribute 'innertext' with value 'None existing item'")]
		public void SelectItemNotFoundException()
		{
			SelectList selectList = ie.SelectList("Select1");
			selectList.Select("None existing item");
		}

		[Test, ExpectedException(typeof (SelectListItemNotFoundException))]
		public void SelectPartialTextMatchItemNotFoundException()
		{
			SelectList selectList = ie.SelectList("Select1");
			selectList.Select("Second");
		}

		[Test]
		public void SelectListCollection()
		{
			Assert.AreEqual(2, ie.SelectLists.Length);

			// Collections
			SelectListCollection selectLists = ie.SelectLists;

			Assert.AreEqual(2, selectLists.Length);

			// Collection items by index
			Assert.AreEqual("Select1", selectLists[0].Id);
			Assert.AreEqual("Select2", selectLists[1].Id);

			IEnumerable selectListEnumerable = selectLists;
			IEnumerator selectListEnumerator = selectListEnumerable.GetEnumerator();

			// Collection iteration and comparing the result with Enumerator
			int count = 0;
			foreach (SelectList selectList in selectLists)
			{
				selectListEnumerator.MoveNext();
				object enumSelectList = selectListEnumerator.Current;

				Assert.IsInstanceOfType(selectList.GetType(), enumSelectList, "Types are not the same");
				Assert.AreEqual(selectList.OuterHtml, ((SelectList) enumSelectList).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(selectListEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(2, count);
		}

		[Test]
		public void SingleSelectListExists()
		{
			Assert.IsTrue(ie.SelectList("Select1").Exists);
			Assert.IsTrue(ie.SelectList(new Regex("Select1")).Exists);
			Assert.IsFalse(ie.SelectList("nonexistingSelect1").Exists);
		}

		[Test]
		public void SingleSelectSelectList()
		{
			// Make sure the page is fresh so the selected item (after loading
			// the page) is the right one.
			ie.GoTo(ie.Url);

			SelectList selectList = ie.SelectList("Select1");

			Assert.IsNotNull(selectList, "SelectList niet aangetroffen");

			Assert.IsFalse(selectList.Multiple, "Select 1 must not allow multiple selection to pass the next tests");

			Assert.AreEqual(1, selectList.SelectedItems.Count, "Not one item selected on page load");
			// Test if the right item is selected after a page load
			Assert.AreEqual("First text", selectList.SelectedItem, "'First text' not selected on page load");

			selectList.ClearList();
			Assert.AreEqual(1, selectList.SelectedItems.Count, "SelectedItem should still be selected after ClearList");
			Assert.IsTrue(selectList.HasSelectedItems, "Selected item expected");

			selectList.Select("Second text");
			Assert.IsTrue(selectList.HasSelectedItems, "Selected item expected");
			Assert.AreEqual(1, selectList.SelectedItems.Count, "Unexpected count");
			Assert.AreEqual("Second text", selectList.SelectedItems[0], "Unexpected SelectedItem by index");
			Assert.AreEqual("Second text", selectList.SelectedItem, "Unexpected SelectedItem");

			selectList.SelectByValue("3");
			Assert.AreEqual("Third text", selectList.SelectedItem, "Unexpected SelectedItem");
		}

		[Test]
		public void SelectTextWithRegex()
		{
			SelectList selectList = ie.SelectList("Select1");

			selectList.Select(new Regex("cond te"));
			Assert.IsTrue(selectList.HasSelectedItems, "Selected item expected");
			Assert.AreEqual(1, selectList.SelectedItems.Count, "Unexpected count");
			Assert.AreEqual("Second text", selectList.SelectedItems[0], "Unexpected SelectedItem by index");
			Assert.AreEqual("Second text", selectList.SelectedItem, "Unexpected SelectedItem");
		}

		[Test]
		public void SelectValueWithRegex()
		{
			SelectList selectList = ie.SelectList("Select1");

			selectList.SelectByValue(new Regex("twee"));
			Assert.AreEqual("Second text", selectList.SelectedItem, "Unexpected SelectedItem");
		}

		[Test]
		public void OptionExists()
		{
			SelectList selectList = ie.SelectList("Select1");

			Assert.IsTrue(selectList.Options.Exists(Find.ByText("First text")));
			Assert.IsTrue(selectList.Options.Exists(Find.ByValue("tweede tekst")));

			Assert.IsTrue(selectList.Option("First text").Exists);
			Assert.IsTrue(selectList.Option(Find.ByValue("tweede tekst")).Exists);
		}

		[Test]
		public void OptionsInSingelSelectList()
		{
			SelectList selectList = ie.SelectList("Select1");

			Assert.IsFalse(selectList.Option("Third text").Selected);
			selectList.Option("Third text").Select();
			Assert.IsTrue(selectList.Option("Third text").Selected);
			selectList.Option("First text").SelectNoWait();
			ie.WaitForComplete();
			Assert.IsFalse(selectList.Option("Third text").Selected);
		}

		[Test]
		public void OptionsInMultiSelectList()
		{
			ie.GoTo(MainURI);

			SelectList selectList = ie.SelectList("Select2");

			Assert.IsFalse(selectList.Option("Third Listitem").Selected, "Third listitem is selected");
			selectList.Option("Third Listitem").Select();
			Assert.IsTrue(selectList.Option("Third Listitem").Selected, "Third listitem is not selected");
			selectList.Option("Third Listitem").Clear();
			ie.WaitForComplete();
			Assert.IsFalse(selectList.Option("Third Listitem").Selected, "Third listitem is selected #2");
		}

        [Test]
        public void Bug_1958882_SelectNoWait_is_waiting_somewhere()
        {
            ie.GoTo(TestEventsURI);

            ConfirmDialogHandler confirm = new ConfirmDialogHandler();
            using (new UseDialogOnce(ie.DialogWatcher, confirm))
            {
                ie.SelectList(Find.ById("selectList")).Option(Find.ByValue("2")).SelectNoWait();

                confirm.WaitUntilExists();
                confirm.OKButton.Click();
            }
        }

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

        [Test]
        public void FindOptionUsingPredicateT()
        {
            Option option = ie.SelectList("Select2").Option(o => o.Text == "Third Listitem");
            Assert.That(option.Exists);
        }
	}
}