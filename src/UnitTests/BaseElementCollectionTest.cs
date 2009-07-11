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
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class BaseElementCollectionTest : BaseWithBrowserTests
	{
		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

		[Test]
		public void FirstWithAttributeConstraint()
		{
		    ExecuteTest(browser =>
		                    {
		                        var elements = browser.Elements;
		                        var element = elements.First(Find.ById("popupid"));
		                        Assert.That(element.Exists, Is.True);
		                        Assert.That(element, Is.TypeOf(typeof(Button)));

		                    });

		}

		[Test]
		public void First()
		{
		    ExecuteTest(browser =>
		                    {
		                        var buttons = browser.Buttons;
		                        Element element = buttons.First();

		                        Assert.That(element.Exists, Is.True);
		                        Assert.That(element.Id, Is.EqualTo("popupid"));
		                        Assert.That(element, Is.TypeOf(typeof(Button)));

		                    });

		}

        [Test]
        public void ExistUsingPredicateT()
        {
            ExecuteTest(browser => Assert.That(browser.Buttons.Exists(b => b.Id == "helloid")));

        }
	}
}