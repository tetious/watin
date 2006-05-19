using System;
using System.IO;

using NUnit.Framework;

using WatiN.Core;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class Utils
  {
    private static Uri htmlTestBaseURI ;

    [TestFixtureSetUp]
    public void Setup()
    {
      System.Threading.Thread.CurrentThread.ApartmentState = System.Threading.ApartmentState.STA;

      string htmlTestLocation = new DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + @"\html\";
            
      htmlTestBaseURI = new Uri(htmlTestLocation);
    }

    [Test]
    public void DumpElements()
    {
      using (IE ie = new IE(htmlTestBaseURI + "main.html"))
      {
        WatiN.Core.UtilityClass.DumpElements(ie.MainDocument);
      }
    }

    [Test]
    public void DumpElementsElab()
    {
      using (IE ie = new IE(htmlTestBaseURI + "Frameset.html"))
      {
        WatiN.Core.UtilityClass.DumpElementsWithHtmlSource(ie.MainDocument);
      }
    }
  }
}