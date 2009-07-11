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
	[TestFixture]
	public class HTMLDialogFindByTests : BaseWithBrowserTests
	{
		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

		[TestFixtureSetUp]
		public override void FixtureSetup()
		{
			base.FixtureSetup();
			Ie.Button("modalid").ClickNoWait();
		}

		[TestFixtureTearDown]
		public override void FixtureTearDown()
		{
			Ie.HtmlDialogs.CloseAll();
			Ie.WaitForComplete();
		
			base.FixtureTearDown();		
		}

		[Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void HTMLDialogGettingWithNegativeTimeoutNotAllowed()
		{
			Ie.HtmlDialog(Find.ByUrl(PopUpURI), -1);
		}

		[Test]
		public void HTMLDialogFindByTitle()
		{
			AssertHTMLDialog(Ie.HtmlDialog(Find.ByTitle("PopUpTest")));
		}

		[Test]
		public void HTMLDialogFindByUrl()
		{
			AssertHTMLDialog(Ie.HtmlDialog(Find.ByUrl(PopUpURI)));
		}

		[Test]
		public void HTMLDialogFindByTitleAndWithTimeout()
		{
			AssertHTMLDialog(Ie.HtmlDialog(Find.ByTitle("PopUpTest"), 10));
		}

		[Test]
		public void HTMLDialogFindByUrlAndWithTimeout()
		{
			AssertHTMLDialog(Ie.HtmlDialog(Find.ByUrl(PopUpURI), 10));
		}

		private static void AssertHTMLDialog(HtmlDialog htmlDialog)
		{
			Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
			Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");
		}
	}
}