namespace WatiN.Core.UnitTests
{
  using NUnit.Framework;
  using WatiN.Core.Comparers;
  using WatiN.Core.Interfaces;

  [TestFixture, Ignore("work in progress")]
  public class ModalPopupExtenderTests
  {
    [Test]
    public void ModalPopupExtenderTest()
    {
      using (IE ie = new MyIE("http://www.asp.net/AJAX/Control-Toolkit/Live/ModalPopup/ModalPopup.aspx"))
      {
        Div modalDialog = ie.Div("ctl00_SampleContent_Panel1");
        Assert.IsTrue(modalDialog.Parent.Style.Display == "none", "modaldialog should not be visible");

        // Show the modaldialog
        ie.Link("showModalPopupClientButton").Click();

        modalDialog.WaitUntil(new VisibleAttribute(true), 5);
        Assert.IsTrue(modalDialog.Style.Display != "none", "modaldialog should be visible");

        // Hide the modaldialog
        Link link = modalDialog.Link("ctl00_SampleContent_CancelButton");
        link.Click();
        modalDialog.WaitUntil(new VisibleAttribute(false), 5);
        Assert.IsTrue(modalDialog.Style.Display == "none", "modaldialog should be visible again");
      }
    }
  }

  public class VisibleAttribute : AttributeConstraint
  {
    public VisibleAttribute(bool visible) : base("visible", new BoolComparer(visible))
    {
    }

    protected override bool doCompare(IAttributeBag attributeBag)
    {
      ElementAttributeBag bag = (ElementAttributeBag) attributeBag;

      return comparer.Compare(IsVisible(new Element(null, bag.IHTMLElement)).ToString());
    }

    public bool IsVisible(Element element)
    {
      bool isVisible = true;
      if (element.Parent != null)
      {
        isVisible = IsVisible(element.Parent);
      }

      if (isVisible)
      {
        isVisible = (element.Style.Display != "none");
      }

      return isVisible;
    }
  }

  public class MyIE : IE
  {
    public MyIE(string url) : base(url)
    {
    }

    public override void WaitForComplete()
    {
      Link("showModalPopupClientButton").WaitUntilExists();
    }
  }
}