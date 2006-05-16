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
        Core.Utils.Utils.dumpElements(ie.MainDocument);
      }
    }

    [Test]
    public void DumpElementsElab()
    {
      using (IE ie = new IE(htmlTestBaseURI + "Frameset.html"))
      {
        Core.Utils.Utils.dumpElementsElab(ie.MainDocument);
      }
    }
  }
}