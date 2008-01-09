#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

namespace WatiN.Core.UnitTests
{
	[TestFixture, Category("InternetConnectionNeeded")]
	public class FrameCrossDomainTests : BaseWithIETests
	{
		public override Uri TestPageUri
		{
			get { return CrossDomainFramesetURI; }
		}

		[Test]
		public void GetGoogleFrameUsingFramesCollection()
		{
			try
			{
				ie.Frames[1].TextField(Find.ByName("q"));
			}
			catch (UnauthorizedAccessException)
			{
				Assert.Fail("UnauthorizedAccessException");
			}

			Assert.AreEqual("mainid", ie.Frames[1].Id, "Unexpected id");
			Assert.AreEqual("main", ie.Frames[1].Name, "Unexpected name");
		}

		[Test]
		public void GetGoogleFrameUsingFindById()
		{
			try
			{
				ie.Frame("mainid").TextField(Find.ByName("q"));
			}
			catch (UnauthorizedAccessException)
			{
				Assert.Fail("UnauthorizedAccessException");
			}

			Assert.AreEqual("mainid", ie.Frame("mainid").Id, "Unexpected Id");
			Assert.AreEqual("main", ie.Frame("mainid").Name, "Unexpected name");
		}

		[Test]
		public void GetContentsFrameUsingFindById()
		{
			try
			{
				ie.Frame("contentsid").Link("googlelink");
			}
			catch (UnauthorizedAccessException)
			{
				Assert.Fail("UnauthorizedAccessException");
			}

			Assert.AreEqual("contentsid", ie.Frame("contentsid").Id, "Unexpected Id");
			Assert.AreEqual("contents", ie.Frame("contentsid").Name, "Unexpected name");
		}

		[Test]
		public void GetGoogleFrameUsingFindByName()
		{
			try
			{
				ie.Frame(Find.ByName("main"));
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
				ie.Frame(Find.ByName("contents"));
			}
			catch (UnauthorizedAccessException)
			{
				Assert.Fail("UnauthorizedAccessException");
			}
		}
	}
}