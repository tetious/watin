namespace WatiN.Core.UnitTests
{
  using System;
  using NUnit.Framework;

  [TestFixture, Category("InternetConnectionNeeded")]
  public class FrameCrossDomainTests : WatiNTest
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
}