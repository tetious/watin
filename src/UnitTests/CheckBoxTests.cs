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
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;

  [TestFixture]
  public class CheckBoxTests : BaseElementsTests
  {
    [Test]
    public void CheckBoxElementTags()
    {
      Assert.AreEqual(1, CheckBox.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("input", ((ElementTag) CheckBox.ElementTags[0]).TagName);
      Assert.AreEqual("checkbox", ((ElementTag) CheckBox.ElementTags[0]).InputTypes);
    }

    [Test]
    public void CheckBoxFromElement()
    {
      Element element = ie.Element("Checkbox1");
      CheckBox checkBox = new CheckBox(element);
      Assert.AreEqual("Checkbox1", checkBox.Id);
    }

    [Test]
    public void CheckBoxExists()
    {
      Assert.IsTrue(ie.CheckBox("Checkbox1").Exists);
      Assert.IsTrue(ie.CheckBox(new Regex("Checkbox1")).Exists);
      Assert.IsFalse(ie.CheckBox("noneexistingCheckbox1id").Exists);
    }

    [Test]
    public void CheckBoxTest()
    {
      CheckBox checkbox1 = ie.CheckBox("Checkbox1");

      Assert.AreEqual("Checkbox1", checkbox1.Id, "Found wrong checkbox");
      Assert.AreEqual("Checkbox1", checkbox1.ToString(), "ToString didn't return Id");
      Assert.IsTrue(checkbox1.Checked, "Should initially be checked");

      checkbox1.Checked = false;
      Assert.IsFalse(checkbox1.Checked, "Should not be checked");

      checkbox1.Checked = true;
      Assert.IsTrue(checkbox1.Checked, "Should be checked");
    }

    [Test]
    public void CheckBoxes()
    {
      Assert.AreEqual(5, ie.CheckBoxes.Length, "Unexpected number of checkboxes");

      CheckBoxCollection formCheckBoxs = ie.Form("FormCheckboxes").CheckBoxes;

      // Collection items by index
      Assert.AreEqual(3, formCheckBoxs.Length, "Wrong number off checkboxes");
      Assert.AreEqual("Checkbox1", formCheckBoxs[0].Id);
      Assert.AreEqual("Checkbox2", formCheckBoxs[1].Id);
      Assert.AreEqual("Checkbox4", formCheckBoxs[2].Id);

      // Collection iteration and comparing the result with Enumerator
      IEnumerable checkboxEnumerable = formCheckBoxs;
      IEnumerator checkboxEnumerator = checkboxEnumerable.GetEnumerator();

      int count = 0;
      foreach (CheckBox checkBox in formCheckBoxs)
      {
        checkboxEnumerator.MoveNext();
        object enumCheckbox = checkboxEnumerator.Current;

        Assert.IsInstanceOfType(checkBox.GetType(), enumCheckbox, "Types are not the same");
        Assert.AreEqual(checkBox.OuterHtml, ((CheckBox) enumCheckbox).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }

      Assert.IsFalse(checkboxEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(3, count);
    }
  }
}