#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using NUnit.Framework;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture, Category("InternetConnectionNeeded")]
	public class FrameCrossDomainTests : BaseWithBrowserTests
	{
		public override Uri TestPageUri
		{
			get { return CrossDomainFramesetURI; }
		}

		[Test]
		public void GetGoogleFrameUsingFramesCollection()
		{
		    ExecuteTest(browser =>
		                    {
		                        try
		                        {
                                    Assert.That(browser.Frames[1].TextField(Find.ByName("q")).Exists, "Should find google input field");
		                        }
		                        catch (UnauthorizedAccessException)
		                        {
		                            Assert.Fail("UnauthorizedAccessException");
		                        }
                                catch(Exception e)
                                {
                                    Assert.Fail("Unexpected exception: " + e.GetType());
                                }

                                Assert.AreEqual("mainid", browser.Frames[1].Id, "Unexpected id");
                                Assert.AreEqual("main", browser.Frames[1].Name, "Unexpected name");
		                    });
		}

		[Test]
		public void GetGoogleFrameUsingFindById()
		{
		    ExecuteTest(browser =>
		                    {
		                        try
		                        {
		                            browser.Frame("mainid").TextField(Find.ByName("q")).TypeText("WatiN");
		                        }
		                        catch (UnauthorizedAccessException)
		                        {
		                            Assert.Fail("UnauthorizedAccessException");
		                        }
                                catch (Exception e)
                                {
                                    Assert.Fail("Unexpected exception: " + e.GetType());
                                }

		                        Assert.AreEqual("mainid", browser.Frame("mainid").Id, "Unexpected Id");
		                        Assert.AreEqual("main", browser.Frame("mainid").Name, "Unexpected name");

		                    });

		}

		[Test]
		public void GetContentsFrameUsingFindById()
		{
		    ExecuteTest(browser =>
		                    {
		                        try
		                        {
		                            Assert.That(browser.Frame("contentsid").Link("googlelink").Exists, "Should find link");
		                        }
		                        catch (UnauthorizedAccessException)
		                        {
		                            Assert.Fail("UnauthorizedAccessException");
		                        }
                                catch (Exception e)
                                {
                                    Assert.Fail("Unexpected exception: " + e.GetType());
                                }

                                Assert.AreEqual("contentsid", browser.Frame("contentsid").Id, "Unexpected Id");
                                Assert.AreEqual("contents", browser.Frame("contentsid").Name, "Unexpected name");

		                    });

		}

		[Test]
		public void GetGoogleFrameUsingFindByName()
		{
		    ExecuteTest(browser =>
		                    {
		                        try
		                        {
                                    browser.Frame(Find.ByName("main"));
		                        }
		                        catch (UnauthorizedAccessException)
		                        {
		                            Assert.Fail("UnauthorizedAccessException");
		                        }
		                    });
		}

		[Test]
		public void GetContentsFrameUsingFindByName()
		{
		    ExecuteTest(browser =>
		                    {
		                        try
		                        {
                                    browser.Frame(Find.ByName("contents"));
		                        }
		                        catch (UnauthorizedAccessException)
		                        {
		                            Assert.Fail("UnauthorizedAccessException");
		                        }
		                    });
		}
	}
}