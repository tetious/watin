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

using NUnit.Framework;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class Elements : BaseElementsTests
	{
		[Test]
		public void ButtonAndOthersFindByShouldNeverThrowInvalidAttributeException()
		{
			Button button = ie.Button(Find.By("noattribute", "novalue"));
			Assert.IsFalse(button.Exists);
		}

		[Test]
		public void LinkFindNonExistingElementWithoutElementNotFoundException()
		{
			ie.Link(Find.ById("noexistinglinkid"));
		}

		[Test]
		public void FindingElementByCustomAttribute()
		{
			Assert.IsTrue(ie.Table(Find.By("myattribute", "myvalue")).Exists);
		}

		[Test]
		public void FindBySourceIndex()
		{
			Button button = ie.Button(Find.By("sourceIndex", "13"));

			Assert.AreEqual("13", button.GetAttributeValue("sourceIndex"));
		}

		[Test]
		public void FindByIndex()
		{
			Assert.AreEqual("popupid", ie.Button(Find.ByIndex(0)).Id);
		}

		[Test]
		public void FindByShouldPassEvenIfAttributeValueOfHTMLELementIsNull()
		{
			Assert.IsFalse(ie.Button(Find.By("classname", "nullstring")).Exists);
		}
	}
}