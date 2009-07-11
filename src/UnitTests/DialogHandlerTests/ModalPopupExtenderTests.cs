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
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ModalPopupExtenderTests : BaseWithBrowserTests
	{
		[Test, Category("InternetConnectionNeeded")]
		public void ModalPopupExtenderTest()
		{
            var modalDialog = Ie.Div("ctl00_SampleContent_Panel1");
		    Assert.That(modalDialog.Parent.Style.Display, Is.EqualTo("none"), "modaldialog should not be visible");

			// Show the modaldialog
            Ie.Link("ctl00_SampleContent_LinkButton1").Click();

			modalDialog.WaitUntil(new VisibleConstraint(true), 5);
			Assert.IsTrue(modalDialog.Style.Display != "none", "modaldialog should be visible");

			// Hide the modaldialog
            var cancel = modalDialog.Button("ctl00_SampleContent_CancelButton");
			cancel.Click();
			modalDialog.WaitUntil(new VisibleConstraint(false), 5);
            Assert.That(modalDialog.Parent.Style.Display, Is.EqualTo("none"), "modaldialog should be visible again");
		}

	    public override Uri TestPageUri
	    {
            get { return new Uri("http://www.asp.net/AJAX/AjaxControlToolkit/Samples/ModalPopup/ModalPopup.aspx"); }
	    }
	}

	public class VisibleConstraint : AttributeConstraint
	{
		public VisibleConstraint(bool visible) : base("visible", new BoolComparer(visible)) {}

        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            var element = attributeBag.GetAdapter<Element>();
            return element != null && Comparer.Compare(IsVisible(element).ToString());
        }

		public bool IsVisible(Element element)
		{
			var isVisible = true;
		    var parent = element.Parent;

		    if (parent != null)
			{
				isVisible = IsVisible(parent);
			}

			if (isVisible)
			{
				isVisible = (element.Style.Display != "none");
			}

			return isVisible;
		}
	}
}