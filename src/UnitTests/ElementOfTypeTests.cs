#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using WatiN.Core;
using WatiN.Core.Native;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementOfTypeTests : BaseWithBrowserTests
	{
		[Test]
		public void Should_be_able_to_use_customized_element_with_ElementOfType()
		{
			ExecuteTestWithAnyBrowser(browser =>
			{
				var myButton = browser.ElementOfType<MyButton>("helloid");
				Assert.That(myButton.Exists, "MyButton doesn't exist");
				
				myButton.Click();
				
				Assert.That(myButton.ClickWasCalled, "Click wasn't called");
			});
		}

		[Test]
		public void Should_be_able_to_use_customized_element_with_ElementsOfType()
		{
			ExecuteTestWithAnyBrowser(browser =>
			{
				var myButtons = browser.ElementsOfType<MyButton>();
				Assert.That(myButtons[0].Exists, "MyButton doesn't exist");
				
				myButtons[0].Click();
				
				Assert.That(myButtons[0].ClickWasCalled, "Click wasn't called");
			});
		}
		
		[Test]
		public void Should_be_able_to_filter_on_customized_elements()
		{
			ExecuteTestWithAnyBrowser(browser =>
			{
			    // GIVEN
			    var myButtons = browser.ElementsOfType<MyButton>();
			    	
			    // WHEN
			    var myButtonsFiltered = myButtons.Filter(Find.ByIndex(2));
			    
			    // THEN
				Assert.That(myButtonsFiltered[0].Exists, "MyButton doesn't exist");
			});
		}

		[Test]
		public void Should_be_able_to_use_custom_element_as_base_element_for_a_control()
		{
			ExecuteTestWithAnyBrowser(browser =>
			{
				var myControls = browser.Controls<MyControl>();
				Assert.That(myControls.Count, Is.EqualTo(5));
			});
		}

        [Test]
        public void Should_implicitly_inherit_attribute_tags_when_none_are_specified_on_custom_element()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                var buttonCount = browser.Buttons.Count;
                Assert.That(buttonCount, Is.GreaterThan(0), "Pre-condition: Page should have buttons");

                // WHEN
                var myButtons = browser.ElementsOfType<MyButtonWithNoElementTag>();

                // THEN
                Assert.That(myButtons, Is.Not.Null, "Expected a collection");
                Assert.That(myButtons.Count, Is.EqualTo(buttonCount), "Expected buttons");
            });
        }

		public override Uri TestPageUri {
			get { return MainURI; }
		}
	}

    public class MyControl : Control<MyButton> { }
		
	[ElementTag("input", InputType = "button")]
	public class MyButton : Button
	{
		public bool ClickWasCalled { get; set; }
		
		public MyButton(DomContainer domContainer, INativeElement element) : base(domContainer, element) { }
        public MyButton(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }
			
		public override void Click()
		{
			ClickWasCalled = true;	
		}
	}

    public class MyButtonWithNoElementTag : Button
    {
        public bool ClickWasCalled { get; set; }

        public MyButtonWithNoElementTag(DomContainer domContainer, INativeElement element) : base(domContainer, element) { }
        public MyButtonWithNoElementTag(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

        public override void Click()
        {
            ClickWasCalled = true;
        }

    }
}
