using System;
using System.Collections;
using System.IO;

using NUnit.Framework;

using WatiN.Core;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class FrameTests
  {
    private static Uri htmlTestBaseURI ;
    private static Uri indexURI;
    private static Uri mainURI;

    const string frameNameContents = "contents";
    const string frameNameMain = "main";

    private IE ie;

    [TestFixtureSetUp]
    public void Setup()
    {
      System.Threading.Thread.CurrentThread.ApartmentState = System.Threading.ApartmentState.STA;

      Logger.LogWriter = new DebugLogWriter();

      string htmlLocation = new DirectoryInfo(System.Environment.CurrentDirectory).Parent.Parent.FullName + @"\html\";
            
      htmlTestBaseURI = new Uri(htmlLocation);
      indexURI = new Uri(htmlTestBaseURI, "Index.html");
      mainURI = new Uri(htmlTestBaseURI, "main.html");

      ie = new IE(htmlTestBaseURI + "frameset.html", true);
    }

    [TestFixtureTearDown]
    public void tearDown()
    {
      ie.Close();
    }

    [Test]
    public void HasNoFrames()
    {
      using(IE iemain = new IE(mainURI.ToString()))
      {
        Assert.AreEqual(0, iemain.MainDocument.Frames.Length);
      }
    }

    [Test, ExpectedException(typeof(FrameNotFoundException),"Could not find a frame by id with value 'contentsid'")]
    public void ExpectFrameNotFoundException()
    {
      ie.MainDocument.Frame(Find.ById("contentsid"));
    }

    [Test]
    public void Frame()
    {
      Frame mainFrame = ie.MainDocument.Frame(Find.ById("mainid"));
      Assert.IsNotNull(mainFrame, "Frame expected");
      Assert.AreEqual("main", mainFrame.Name);
      Assert.AreEqual("mainid", mainFrame.Id);

      AssertFindFrame(ie, Find.ByName(frameNameMain), frameNameMain);
      AssertFindFrame(ie, Find.ByUrl(indexURI.ToString()), frameNameContents);
    }

    [Test]
    public void Frames()
    {
      const int expectedFramesCount = 2;
      FrameCollection frames = ie.MainDocument.Frames;

      Assert.AreEqual(expectedFramesCount, frames.Length, "Unexpected number of frames");

      // Collection items by index
      Assert.AreEqual(frameNameContents, frames[0].Name);
      Assert.AreEqual(frameNameMain, frames[1].Name);

      IEnumerable frameEnumerable = frames;
      IEnumerator frameEnumerator = frameEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (Frame frame in frames)
      {
        frameEnumerator.MoveNext();
        object enumFrame = frameEnumerator.Current;
        
        Assert.IsInstanceOfType(frame.GetType(), enumFrame, "Types are not the same");
        Assert.AreEqual(frame.Html, ((Frame)enumFrame).Html, "foreach en IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(frameEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedFramesCount, count);
    }

    [Test]
    public void ShowFrames()
    {
      Core.Utils.Utils.ShowFrames(ie.MainDocument);
    }

    private static void AssertFindFrame(IE ie, AttributeValue findBy, string expectedFrameName)
    {
      Frame frame = null;
      if (findBy is UrlValue)
      {
        frame = ie.MainDocument.Frame((UrlValue)findBy);
      }
      else if (findBy is NameValue)
      {
        frame = ie.MainDocument.Frame((NameValue)findBy);
      }
      Assert.IsNotNull(frame, "Frame '" + findBy.Value + "' not found");
      Assert.AreEqual(expectedFrameName, frame.Name, "Incorrect frame for " + findBy.ToString() + ", " + findBy.Value);
    }
  }
}