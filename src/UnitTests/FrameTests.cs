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

using System;
using System.Collections;
using NUnit.Framework;

using WatiN.Core;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using Attribute=WatiN.Core.Attribute;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class FramesetTests : WatiNTest
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
    public void IEFrameIsAFrame()
    {
      Assert.IsInstanceOfType(typeof(Frame),ie.Frame("mainid"));
    }
    
    [Test]
    public void FrameIsADocument()
    {
      Assert.IsInstanceOfType(typeof(Document),ie.Frame("mainid"));
    }
  
    [Test, ExpectedException(typeof(FrameNotFoundException),"Could not find a Frame or IFrame by id with value 'NonExistingFrameID'")]
    public void ExpectFrameNotFoundException()
    {
      ie.Frame(Find.ById("NonExistingFrameID"));
    }

    [Test]
    public void ContentsFrame()
    {
      Frame contentsFrame = ie.Frame(Find.ByName("contents"));
      Assert.IsNotNull(contentsFrame, "Frame expected");
      Assert.AreEqual("contents", contentsFrame.Name);
      Assert.AreEqual(null, contentsFrame.Id);

      AssertFindFrame(ie, Find.ByUrl(IndexURI), frameNameContents);
    }
    
    [Test]
    public void MainFrame()
    {
      Frame mainFrame = ie.Frame(Find.ById("mainid"));
      Assert.IsNotNull(mainFrame, "Frame expected");
      Assert.AreEqual("main", mainFrame.Name);
      Assert.AreEqual("mainid", mainFrame.Id);

      AssertFindFrame(ie, Find.ByName(frameNameMain), frameNameMain);
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
        Assert.AreEqual(frame.Html, ((Frame)enumFrame).Html, "foreach and IEnumator don't act the same.");
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
    
    private static void AssertFindFrame
      
      (IE ie, Attribute findBy, string expectedFrameName)
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
  public class FramesetWithinFrameSetTests : WatiNTest
  {   
    [Test]
    public void FramesLength()
    {
      using (IE ieframes = new IE(FramesetWithinFramesetURI))
      {
        Assert.AreEqual(2, ieframes.Frames.Length);
        Assert.AreEqual(2, ieframes.Frames[1].Frames.Length);
      }
    }
  }
  
  [TestFixture]
  public class NoFramesTest : WatiNTest
  {   
    [Test]
    public void HasNoFrames()
    {
      using(IE iemain = new IE(MainURI))
      {
        Assert.AreEqual(0, iemain.Frames.Length);
      }
    }
  }
  
  [TestFixture]
  public class CrossDomainTests : WatiNTest
  {
    private IE ieframes;
    
    [TestFixtureSetUp]
    public void Setup()
    {
      ieframes = new IE(CrossDomainFramesetURI);
    }
    
    [TestFixtureTearDown]
    public void Teardown()
    {
      ieframes.Close();
    }
    
    [Test]
    public void GetGoogleFrameUsingFramesCollection()
    {
      try
      {
        ieframes.Frames[1].TextField(Find.ByName("q"));
      }
      catch (UnauthorizedAccessException)
      {
        Assert.Fail("UnauthorizedAccessException");
      }
      
      Assert.AreEqual("mainid", ieframes.Frames[1].Id, "Unexpected id");
      Assert.AreEqual("main", ieframes.Frames[1].Name, "Unexpected name");
    }
    
    [Test]
    public void GetGoogleFrameUsingFindById()
    {
      try
      {
        ieframes.Frame("mainid").TextField(Find.ByName("q"));
      }
      catch (UnauthorizedAccessException)
      {
        Assert.Fail("UnauthorizedAccessException");
      }

      Assert.AreEqual("mainid", ieframes.Frame("mainid").Id, "Unexpected Id");
      Assert.AreEqual("main", ieframes.Frame("mainid").Name, "Unexpected name");
    }
    
    [Test]
    public void GetContentsFrameUsingFindById()
    {
      try
      {
        ieframes.Frame("contentsid").Link("googlelink");
      }
      catch (UnauthorizedAccessException)
      {
        Assert.Fail("UnauthorizedAccessException");
      }

      Assert.AreEqual("contentsid", ieframes.Frame("contentsid").Id, "Unexpected Id");
      Assert.AreEqual("contents", ieframes.Frame("contentsid").Name, "Unexpected name");
    }
    
    [Test]
    public void GetGoogleFrameUsingFindByName()
    {
      try
      {
        ieframes.Frame(Find.ByName("main"));
      }
      catch (UnauthorizedAccessException)
      {
        Assert.Fail("UnauthorizedAccessException");
      }
    }
    
    [Test]
    public void GetContentsFrameUsingFindByName()
    {
      try
      {
        ieframes.Frame(Find.ByName("contents"));
      }
      catch (UnauthorizedAccessException)
      {
        Assert.Fail("UnauthorizedAccessException");
      }
    }
  }
  
  [TestFixture]
  public class IFramesTests : WatiNTest
  {
    private IE ie;

    [TestFixtureSetUp]
    public void Setup()
    {
      ie = new IE(IFramesMainURI);
    }

    [TestFixtureTearDown]
    public void tearDown()
    {
      ie.Close();
    }

    [Test, ExpectedException(typeof(FrameNotFoundException),"Could not find a Frame or IFrame by id with value 'NonExistingIFrameID'")]
    public void ExpectFrameNotFoundException()
    {
      ie.Frame(Find.ById("NonExistingIFrameID"));
    }

    [Test]
    public void LeftFrame()
    {
      Frame leftFrame = ie.Frame(Find.ByName("left"));
      Assert.IsNotNull(leftFrame, "Frame expected");
      Assert.AreEqual("left", leftFrame.Name);
      Assert.AreEqual(null, leftFrame.Id);

      leftFrame = ie.Frame(Find.ByUrl(iframesLeftURI));
      Assert.AreEqual("left", leftFrame.Name);
    }
    
    [Test]
    public void MiddleFrame()
    {
      Frame middleFrame = ie.Frame(Find.ByName("middle"));
      Assert.IsNotNull(middleFrame, "Frame expected");
      Assert.AreEqual("middle", middleFrame.Name);
      Assert.AreEqual("iframe2", middleFrame.Id);

      middleFrame = ie.Frame(Find.ByUrl(iframesMiddleURI));
      Assert.AreEqual("middle", middleFrame.Name);
      
      middleFrame = ie.Frame(Find.ById("iframe2"));
      Assert.AreEqual("middle", middleFrame.Name);
    }
    
    [Test]
    public void RightFrame()
    {
      Frame rightFrame = ie.Frame(Find.ByUrl(iframesRightURI));
      Assert.IsNotNull(rightFrame, "Frame expected");
      Assert.AreEqual(null, rightFrame.Name);
      Assert.AreEqual(null, rightFrame.Id);
    }

    [Test]
    public void Frames()
    {
      const int expectedFramesCount = 3;
      FrameCollection frames = ie.Frames;

      Assert.AreEqual(expectedFramesCount, frames.Length, "Unexpected number of frames");

      // Collection items by index
      Assert.AreEqual("left", frames[0].Name);
      Assert.AreEqual("middle", frames[1].Name);
      Assert.AreEqual(null, frames[2].Name);

      IEnumerable frameEnumerable = frames;
      IEnumerator frameEnumerator = frameEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (Frame frame in frames)
      {
        frameEnumerator.MoveNext();
        object enumFrame = frameEnumerator.Current;
        
        Assert.IsInstanceOfType(frame.GetType(), enumFrame, "Types are not the same");
        Assert.AreEqual(frame.Html, ((Frame)enumFrame).Html, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(frameEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedFramesCount, count);
    }
  }
}