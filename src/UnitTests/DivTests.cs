namespace WatiN.Core.UnitTests
{
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;

  [TestFixture]
  public class DivTests : BaseElementsTests
  {
    [Test]
    public void DivElementTags()
    {
      Assert.AreEqual(1, Div.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("div", ((ElementTag) Div.ElementTags[0]).TagName);
    }

    [Test]
    public void CreateDivFromElement()
    {
      Element element = ie.Element("divid");
      Div div = new Div(element);
      Assert.AreEqual("divid", div.Id);
    }

    [Test]
    public void DivExists()
    {
      Assert.IsTrue(ie.Div("divid").Exists);
      Assert.IsTrue(ie.Div(new Regex("divid")).Exists);
      Assert.IsFalse(ie.Div("noneexistingdivid").Exists);
    }

    [Test]
    public void DivTest()
    {
      Assert.AreEqual("divid", ie.Div(Find.ById("divid")).Id, "Find Div by Find.ById");
      Assert.AreEqual("divid", ie.Div("divid").Id, "Find Div by ie.Div()");
    }

    [Test]
    public void Divs()
    {
      Assert.AreEqual(1, ie.Divs.Length, "Unexpected number of Divs");

      DivCollection divs = ie.Divs;

      // Collection items by index
      Assert.AreEqual("divid", divs[0].Id);

      // Collection iteration and comparing the result with Enumerator
      IEnumerable divEnumerable = divs;
      IEnumerator divEnumerator = divEnumerable.GetEnumerator();

      int count = 0;
      foreach (Div div in divs)
      {
        divEnumerator.MoveNext();
        object enumDiv = divEnumerator.Current;

        Assert.IsInstanceOfType(div.GetType(), enumDiv, "Types are not the same");
        Assert.AreEqual(div.OuterHtml, ((Div) enumDiv).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }

      Assert.IsFalse(divEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(1, count);
    }
  }
}