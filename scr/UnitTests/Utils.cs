using System;
using System.IO;

using NUnit.Framework;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class Utils
  {
    private static Uri testDataBaseURI ;

    [TestFixtureSetUp]
    public void Setup()
    {
      System.Threading.Thread.CurrentThread.ApartmentState = System.Threading.ApartmentState.STA;

      string testDataLocation = new DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + @"\testdata\";
            
      testDataBaseURI = new Uri(testDataLocation);
    }

    [Test]
    public void DumpElements()
    {
      using (IE ie = new IE(testDataBaseURI + "main.html"))
      {
        Core.Utils.Utils.dumpElements(ie.MainDocument);
      }
    }

    [Test]
    public void DumpElementsElab()
    {
      using (IE ie = new IE(testDataBaseURI + "Frameset.html"))
      {
        Core.Utils.Utils.dumpElementsElab(ie.MainDocument);
      }
    }
  }
}