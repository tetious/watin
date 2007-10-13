namespace WatiN.Core.UnitTests
{
  using System;
  using NUnit.Framework;

  [TestFixture]
  public class HTMLDialogFindByTests : WatiNTest
  {
    private IE ie = new IE(WatiNTest.MainURI);

    [TestFixtureSetUp]
    public void FixtureSetUp()
    {
      ie.Button("modalid").ClickNoWait();
    }

    [TestFixtureTearDown]
    public void FixtureTearDown()
    {
      foreach (HtmlDialog dialog in ie.HtmlDialogs)
      {
        dialog.Close();
      }

      ie.WaitForComplete();
      ie.Close();
    }

    [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
    public void HTMLDialogGettingWithNegativeTimeoutNotAllowed()
    {
      ie.HtmlDialog(Find.ByUrl(PopUpURI), -1);
    }

    [Test]
    public void HTMLDialogFindByTitle()
    {
      AssertHTMLDialog(ie.HtmlDialog(Find.ByTitle("PopUpTest")));
    }

    [Test]
    public void HTMLDialogFindByUrl()
    {
      AssertHTMLDialog(ie.HtmlDialog(Find.ByUrl(PopUpURI)));
    }

    [Test]
    public void HTMLDialogFindByTitleAndWithTimeout()
    {
      AssertHTMLDialog(ie.HtmlDialog(Find.ByTitle("PopUpTest"), 10));
    }

    [Test]
    public void HTMLDialogFindByUrlAndWithTimeout()
    {
      AssertHTMLDialog(ie.HtmlDialog(Find.ByUrl(PopUpURI), 10));
    }

    private static void AssertHTMLDialog(HtmlDialog htmlDialog)
    {
      Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
      Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");
    }
  }
}