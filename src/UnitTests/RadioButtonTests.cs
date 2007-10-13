namespace WatiN.Core.UnitTests
{
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;

  [TestFixture]
  public class RadioButtonTests : BaseElementsTests
  {
    [Test]
    public void RadioButtonElementTags()
    {
      Assert.AreEqual(1, RadioButton.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("input", ((ElementTag) RadioButton.ElementTags[0]).TagName);
      Assert.AreEqual("radio", ((ElementTag) RadioButton.ElementTags[0]).InputTypes);
    }

    [Test]
    public void CreateRadioButtonFromElement()
    {
      Element element = ie.Element("Radio1");
      RadioButton radioButton = new RadioButton(element);
      Assert.AreEqual("Radio1", radioButton.Id);
    }

    [Test]
    public void RadioButtonExists()
    {
      Assert.IsTrue(ie.RadioButton("Radio1").Exists);
      Assert.IsTrue(ie.RadioButton(new Regex("Radio1")).Exists);
      Assert.IsFalse(ie.RadioButton("nonexistingRadio1").Exists);
    }

    [Test]
    public void RadioButtonTest()
    {
      RadioButton RadioButton1 = ie.RadioButton("Radio1");

      Assert.AreEqual("Radio1", RadioButton1.Id, "Found wrong RadioButton.");
      Assert.AreEqual("Radio1", RadioButton1.ToString(), "ToString didn't return the Id.");
      Assert.IsTrue(RadioButton1.Checked, "Should initially be checked");

      RadioButton1.Checked = false;
      Assert.IsFalse(RadioButton1.Checked, "Should not be checked");

      RadioButton1.Checked = true;
      Assert.IsTrue(RadioButton1.Checked, "Should be checked");
    }

    [Test]
    public void RadioButtons()
    {
      Assert.AreEqual(3, ie.RadioButtons.Length, "Unexpected number of RadioButtons");

      RadioButtonCollection formRadioButtons = ie.Form("FormRadioButtons").RadioButtons;

      Assert.AreEqual(2, formRadioButtons.Length, "Wrong number off RadioButtons");
      Assert.AreEqual("Radio2", formRadioButtons[0].Id);
      Assert.AreEqual("Radio3", formRadioButtons[1].Id);

      // Collection iteration and comparing the result with Enumerator
      IEnumerable radiobuttonEnumerable = formRadioButtons;
      IEnumerator radiobuttonEnumerator = radiobuttonEnumerable.GetEnumerator();

      int count = 0;
      foreach (RadioButton radioButton in formRadioButtons)
      {
        radiobuttonEnumerator.MoveNext();
        object enumRadioButton = radiobuttonEnumerator.Current;

        Assert.IsInstanceOfType(radioButton.GetType(), enumRadioButton, "Types are not the same");
        Assert.AreEqual(radioButton.OuterHtml, ((RadioButton) enumRadioButton).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }

      Assert.IsFalse(radiobuttonEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(2, count);
    }
  }
}