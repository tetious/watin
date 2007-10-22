#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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
	[TestFixture]
	public class HTMLDialogFindByTests : WatiNTest
	{
		private IE ie = new IE(WatiNTest.MainURI);

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			ie.Button("modalid").ClickNoWait();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			foreach (HtmlDialog dialog in ie.HtmlDialogs)
			{
				dialog.Close();
			}

			ie.WaitForComplete();
			ie.Close();
		}

		[Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void HTMLDialogGettingWithNegativeTimeoutNotAllowed()
		{
			ie.HtmlDialog(Find.ByUrl(PopUpURI), -1);
		}

		[Test]
		public void HTMLDialogFindByTitle()
		{
			AssertHTMLDialog(ie.HtmlDialog(Find.ByTitle("PopUpTest")));
		}

		[Test]
		public void HTMLDialogFindByUrl()
		{
			AssertHTMLDialog(ie.HtmlDialog(Find.ByUrl(PopUpURI)));
		}

		[Test]
		public void HTMLDialogFindByTitleAndWithTimeout()
		{
			AssertHTMLDialog(ie.HtmlDialog(Find.ByTitle("PopUpTest"), 10));
		}

		[Test]
		public void HTMLDialogFindByUrlAndWithTimeout()
		{
			AssertHTMLDialog(ie.HtmlDialog(Find.ByUrl(PopUpURI), 10));
		}

		private static void AssertHTMLDialog(HtmlDialog htmlDialog)
		{
			Assert.IsNotNull(htmlDialog, "Dialog niet aangetroffen");
			Assert.AreEqual("PopUpTest", htmlDialog.Title, "Unexpected title");
		}
	}
}