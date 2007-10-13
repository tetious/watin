namespace WatiN.Core.UnitTests
{
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;

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