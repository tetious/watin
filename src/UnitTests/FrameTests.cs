#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright


using System.Collections;

using NUnit.Framework;

using WatiN.Core;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class FrameTests : WatiNTest
  {
    const string frameNameContents = "contents";
    const string frameNameMain = "main";

    private IE ie;

    [TestFixtureSetUp]
    public void Setup()
    {
      Logger.LogWriter = new DebugLogWriter();
            
      ie = new IE(FramesetURI);
    }

    [TestFixtureTearDown]
    public void tearDown()
    {
      ie.Close();
    }

    [Test]
    public void HasNoFrames()
    {
      using(IE iemain = new IE(MainURI))
      {
        Assert.AreEqual(0, iemain.Frames.Length);
      }
    }

    [Test, ExpectedException(typeof(FrameNotFoundException),"Could not find a frame by id with value 'NonExistingFrameID'")]
    public void ExpectFrameNotFoundException()
    {
      ie.Frame(Find.ById("NonExistingFrameID"));
    }

    [Test]
    public void Frame()
    {
      Frame mainFrame = ie.Frame(Find.ById("mainid"));
      Assert.IsNotNull(mainFrame, "Frame expected");
      Assert.AreEqual("main", mainFrame.Name);
      Assert.AreEqual("mainid", mainFrame.Id);

      AssertFindFrame(ie, Find.ByName(frameNameMain), frameNameMain);
      AssertFindFrame(ie, Find.ByUrl(IndexURI), frameNameContents);
    }

    [Test]
    public void Frames()
    {
      const int expectedFramesCount = 2;
      FrameCollection frames = ie.Frames;

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
      UtilityClass.ShowFrames(ie);
    }
    
    private static void AssertFindFrame(IE ie, Attribute findBy, string expectedFrameName)
    {
      Frame frame = null;
      if (findBy is Url)
      {
        frame = ie.Frame((Url)findBy);
      }
      else if (findBy is Name)
      {
        frame = ie.Frame((Name)findBy);
      }
      Assert.IsNotNull(frame, "Frame '" + findBy.Value + "' not found");
      Assert.AreEqual(expectedFrameName, frame.Name, "Incorrect frame for " + findBy.ToString() + ", " + findBy.Value);
    }
  }
  
  [TestFixture]
  public class CrossDomainTests : WatiNTest
  {
    [Test]
    public void CrossDomainTest()
    {
      using(IE ieframes = new IE(FramesetURI))
      {
        ieframes.Frames[0].Link("googlelink").Click();
        
        try
        {
          ieframes.Frames[1].TextField(Find.ByName("q"));
        }
        catch
        {
          Assert.Fail("Failed when using Document.FramesCollection");
        }
        
        try
        {
          ieframes.Frame("mainid").TextField(Find.ByName("q"));
        }
        catch
        {
          Assert.Fail("Failed when using Document.Frame");
        }
      }            
    }
  }
}