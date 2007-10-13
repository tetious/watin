namespace WatiN.Core.UnitTests
{
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;

  [TestFixture]
  public class ParaTests : BaseElementsTests
  {
    [Test]
    public void ParaElementTags()
    {
      Assert.AreEqual(1, Para.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("p", ((ElementTag) Para.ElementTags[0]).TagName);
    }

    [Test]
    public void CreateParaFromElement()
    {
      Element element = ie.Element("links");
      Para para = new Para(element);
      Assert.AreEqual("links", para.Id);
    }

    [Test]
    public void ParaExists()
    {
      Assert.IsTrue(ie.Para("links").Exists);
      Assert.IsTrue(ie.Para(new Regex("links")).Exists);
      Assert.IsFalse(ie.Para("nonexistinglinks").Exists);
    }

    [Test]
    public void ParaTest()
    {
      Para para = ie.Para("links");

      Assert.IsInstanceOfType(typeof (ElementsContainer), para);

      Assert.IsNotNull(para);
      Assert.AreEqual("links", para.Id);
    }

    [Test]
    public void Paras()
    {
      const int expectedParasCount = 4;
      Assert.AreEqual(expectedParasCount, ie.Paras.Length, "Unexpected number of Paras");

      // Collection.Length
      ParaCollection formParas = ie.Paras;

      // Collection items by index
      Assert.IsNull(ie.Paras[0].Id);
      Assert.AreEqual("links", ie.Paras[1].Id);
      Assert.IsNull(ie.Paras[2].Id);
      Assert.IsNull(ie.Paras[3].Id);

      IEnumerable ParaEnumerable = formParas;
      IEnumerator ParaEnumerator = ParaEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (Para inputPara in formParas)
      {
        ParaEnumerator.MoveNext();
        object enumPara = ParaEnumerator.Current;

        Assert.IsInstanceOfType(inputPara.GetType(), enumPara, "Types are not the same");
        Assert.AreEqual(inputPara.OuterHtml, ((Para) enumPara).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }

      Assert.IsFalse(ParaEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedParasCount, count);
    }
  }
}