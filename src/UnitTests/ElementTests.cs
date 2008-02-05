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
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using mshtml;
using NUnit.Framework;
using Rhino.Mocks;
using WatiN.Core.Comparers;
using WatiN.Core.Interfaces;
using WatiN.Core.InternetExplorer;
using StringComparer=WatiN.Core.Comparers.StringComparer;
using Iz=NUnit.Framework.SyntaxHelpers.Is;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementTests : BaseWithIETests
	{
		private MockRepository mocks;
		private Element element;

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

		[SetUp]
		public override void TestSetUp()
		{
			base.TestSetUp();
		}

		[Test]
		public void AncestorTypeShouldReturnTypedElement()
		{
			TableCell tableCell = ie.TableCell(Find.ByText("Contains text in DIV"));
			Assert.IsInstanceOfType(typeof (Div), tableCell.Ancestor(typeof (Div)));
		}

		[Test]
		public void AncestorTagNameShouldReturnTypedElement()
		{
			TableCell tableCell = ie.TableCell(Find.ByText("Contains text in DIV"));
			Assert.IsInstanceOfType(typeof (Div), tableCell.Ancestor("Div"));
		}

		[Test]
		public void AncestorAttributeConstraintShouldReturnTypedElement()
		{
			TableCell tableCell = ie.TableCell(Find.ByText("Contains text in DIV"));
			Assert.IsInstanceOfType(typeof (Div), tableCell.Ancestor(Find.ById("divid")));
		}

		[Test]
		public void AncestorTypeAndAttributeConstraintShouldReturnTypedElement()
		{
			MockRepository mockRepository = new MockRepository();

			INativeElement nativeElement = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			INativeElement firstParentDiv = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			IAttributeBag firstAttributeBag = (IAttributeBag) mockRepository.CreateMock(typeof (IAttributeBag));
			INativeElement secondParentDiv = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			IAttributeBag secondAttributeBag = (IAttributeBag) mockRepository.CreateMock(typeof (IAttributeBag));

			element = new Element(null, nativeElement); 
			Expect.Call(nativeElement.Parent).Return(firstParentDiv).Repeat.Any();
			Expect.Call(firstParentDiv.TagName).Return("div").Repeat.Any();
			Expect.Call(firstParentDiv.AttributeBag).Return(firstAttributeBag);
			Expect.Call(firstAttributeBag.GetValue("innertext")).Return("first ancestor");

			Expect.Call(firstParentDiv.Parent).Return(secondParentDiv).Repeat.Any();
			Expect.Call(secondParentDiv.TagName).Return("div").Repeat.Any();
			Expect.Call(secondParentDiv.AttributeBag).Return(secondAttributeBag);
			Expect.Call(secondAttributeBag.GetValue("innertext")).Return("second ancestor");
			Expect.Call(secondParentDiv.GetAttributeValue("innertext")).Return("second ancestor");

			mockRepository.ReplayAll();

			Element ancestor = element.Ancestor(typeof (Div), Find.ByText("second ancestor"));

			Assert.IsInstanceOfType(typeof (Div), ancestor);
			Assert.That(ancestor.Text, NUnit.Framework.SyntaxHelpers.Is.EqualTo("second ancestor"));

			mockRepository.VerifyAll();
		}

		[Test]
		public void AncestorTagNameAndAttributeConstraintShouldReturnTypedElement()
		{
			MockRepository mockRepository = new MockRepository();

			INativeElement nativeElement = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			INativeElement firstParentDiv = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			IAttributeBag firstAttributeBag = (IAttributeBag) mockRepository.CreateMock(typeof (IAttributeBag));
			INativeElement secondParentDiv = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			IAttributeBag secondAttributeBag = (IAttributeBag) mockRepository.CreateMock(typeof (IAttributeBag));

			element = new Element(null, nativeElement); 
			Expect.Call(nativeElement.Parent).Return(firstParentDiv).Repeat.Any();
			Expect.Call(firstParentDiv.TagName).Return("div").Repeat.Any();
			Expect.Call(firstParentDiv.AttributeBag).Return(firstAttributeBag).Repeat.Any();
			Expect.Call(firstAttributeBag.GetValue("tagname")).Return("div").Repeat.Any();
			Expect.Call(firstAttributeBag.GetValue("innertext")).Return("first ancestor");

			Expect.Call(firstParentDiv.Parent).Return(secondParentDiv).Repeat.Any();
			Expect.Call(secondParentDiv.TagName).Return("div").Repeat.Any();
			Expect.Call(secondParentDiv.AttributeBag).Return(secondAttributeBag).Repeat.Any();
			Expect.Call(secondAttributeBag.GetValue("tagname")).Return("div").Repeat.Any();
			Expect.Call(secondAttributeBag.GetValue("innertext")).Return("second ancestor");
			Expect.Call(secondParentDiv.GetAttributeValue("innertext")).Return("second ancestor");

			mockRepository.ReplayAll();

			Element ancestor = element.Ancestor("Div", Find.ByText("second ancestor"));

			Assert.IsInstanceOfType(typeof (Div), ancestor);
			Assert.That(ancestor.Text, NUnit.Framework.SyntaxHelpers.Is.EqualTo("second ancestor"));

			mockRepository.VerifyAll();
		}

		[Test]
		public void ElementParentShouldReturnNullWhenRootElement()
		{
			MockRepository mockRepository = new MockRepository();

			INativeElement nativeElement = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));

			element = new Element(null, nativeElement); 
			Expect.Call(nativeElement.Parent).Return(null);

			mockRepository.ReplayAll();

			Assert.IsNull(element.Parent);

			mockRepository.VerifyAll();
		}

		[Test]
		public void ElementParentReturningTypedParent()
		{
			TableCell tableCell = ie.TableCell(Find.ByText("Contains text in DIV"));
			Assert.IsInstanceOfType(typeof (TableRow), tableCell.Parent);
		}

		[Test]
		public void ElementParentReturnsElementsContainerForUnknownElement()
		{
			Element parent = ie.Form("Form").Parent;
			Assert.IsTrue(parent.GetType().Equals(typeof (ElementsContainer)));
		}

		[Test]
		public void ElementPreviousSiblingShouldReturnNullWhenFirstSibling()
		{
			Assert.IsNull(ie.Div("NextAndPreviousTests").Div("first").PreviousSibling);
		}

		[Test]
		public void ElementPreviousSiblingReturningTypedParent()
		{
			Assert.IsTrue(ie.RadioButton("Radio1").PreviousSibling.GetType().Equals(typeof (CheckBox)));
		}

		[Test]
		public void ElementPreviousSiblingReturnsElementsContainerForUnknowElement()
		{
			Element previous = ie.Div("NextAndPreviousTests").Div("last").PreviousSibling;
			Assert.IsTrue(previous.GetType().Equals(typeof (ElementsContainer)));
		}

		[Test]
		public void ElementNextSiblingShouldReturnNullWhenLastSibling()
		{
			Element next = ie.Div("NextAndPreviousTests").Div("last").NextSibling;
			Assert.IsNull(next);
		}

		[Test]
		public void ElementNextSiblingReturningTypedParent()
		{
			Element next = ie.Div("NextAndPreviousTests").Div("first").NextSibling;
			Assert.IsTrue(next.GetType().Equals(typeof (Span)));
		}

		[Test]
		public void ElementNextSiblingReturnsElementsContainerForUnknowElement()
		{
			Element next = ie.Div("NextAndPreviousTests").Span("second").NextSibling;
			Assert.IsTrue(next.GetType().Equals(typeof (ElementsContainer)));
		}

		[Test]
		public void ElementRefresh()
		{
			mocks = new MockRepository();

			INativeElementFinder finder = (INativeElementFinder) mocks.CreateMock(typeof (INativeElementFinder));
			INativeElement nativeElement = (INativeElement) mocks.CreateMock(typeof (INativeElement));

			element = new Element(null, finder);

			Expect.Call(finder.FindFirst()).Return(nativeElement).Repeat.Twice();
			SetupResult.For(nativeElement.GetAttributeValue("tagName")).Return("mockedtag");

			mocks.ReplayAll();

			element = new Element(null, finder);

			Assert.AreEqual("mockedtag", element.TagName);

			element.Refresh();

			Assert.AreEqual("mockedtag", element.TagName);

			mocks.VerifyAll();
		}

		[Test, ExpectedException(typeof (ArgumentException))]
		public void AncestorTypeShouldOnlyExceptTypesInheritingElement()
		{
			element = ie.Form("Form");
			element.Ancestor(typeof (String));
		}

		[Test]
		public void Element()
		{
			element = ie.Element(Find.ById("table1"));

			Assert.IsAssignableFrom(typeof (ElementsContainer), element, "The returned object form ie.Element should be castable to ElementsContainer");

			Assert.IsNotNull(element, "Element not found");

			// check behavior for standard attribute
			Assert.AreEqual("table1", element.GetAttributeValue("id"), "GetAttributeValue id failed");
			// check behavior for non existing attribute
			Assert.IsNull(element.GetAttributeValue("watin"), "GetAttributeValue watin should return null");
			// check behavior for custom attribute
			Assert.AreEqual("myvalue", element.GetAttributeValue("myattribute"), "GetAttributeValue myattribute should return myvalue");

			Assert.AreEqual("table", element.TagName.ToLower(), "Invalid tagname");

			// Textbefore and TextAfter tests
			CheckBox checkBox = ie.CheckBox("Checkbox21");
			Assert.AreEqual("Test label before: ", checkBox.TextBefore, "Unexpected checkBox.TextBefore");
			Assert.AreEqual(" Test label after", checkBox.TextAfter, "Unexpected checkBox.TextAfter");
		}

		[Test]
		public void ElementByTagNameAndInputType()
		{
			element = ie.Element("input", Find.By("id", "name"), "text");
			Assert.IsTrue(element.Exists);
		}

		[Test]
		public void ElementByTagName()
		{
			element = ie.Element("a", Find.By("id", "testlinkid"));
			Assert.IsTrue(element.Exists);
		}

		[Test]
		public void FindHeadElementByTagName()
		{
			element = ie.Element("head", Find.ByIndex(0));
			Assert.IsTrue(element.Exists);
		}

		[Test]
		public void ElementFindByShouldNeverThrowInvalidAttributeException()
		{
			element = ie.Element(Find.ByFor("Checkbox21"));
			Assert.IsTrue(element.Exists);
		}

		[Test]
		public void ElementCollectionExistsShouldNeverThrowInvalidAttributeException()
		{
			Assert.IsTrue(ie.Elements.Exists(Find.ByFor("Checkbox21")));
		}

		[Test]
		public void ElementCollectionShouldReturnTypedElements()
		{
			ElementCollection elements = ie.Div("NextAndPreviousTests").Elements;
			Assert.IsTrue(elements[0].GetType().Equals(typeof (Div)), "Element 0 should be a div");
			Assert.IsTrue(elements[1].GetType().Equals(typeof (Span)), "Element 1 should be a span");
			Assert.IsTrue(elements[2].GetType().Equals(typeof (ElementsContainer)), "Element 2 should be an elementscontainer");
			Assert.IsTrue(elements[3].GetType().Equals(typeof (Div)), "Element 3 should be a div");
		}

		[Test]
		public void ElementCollectionSecondFilterShouldNeverThrowInvalidAttributeException()
		{
			ElementCollection elements = ie.Elements.Filter(Find.ById("testlinkid"));
			ElementCollection elements2 = elements.Filter(Find.ByFor("Checkbox21"));
			Assert.AreEqual(0, elements2.Length);
		}

		[Test]
		public void GetInvalidAttribute()
		{
			Element helloButton = ie.Button("helloid");
			Assert.IsNull(helloButton.GetAttributeValue("NONSENCE"));
		}

		[Test]
		public void GetValidButUndefiniedAttribute()
		{
			Element helloButton = ie.Button("helloid");
			Assert.IsNull(helloButton.GetAttributeValue("title"));
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void GetAttributeValueOfNullThrowsArgumentNullException()
		{
			Element helloButton = ie.Button("helloid");
			Assert.IsNull(helloButton.GetAttributeValue(null));
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void GetAttributeValueOfEmptyStringThrowsArgumentNullException()
		{
			Element helloButton = ie.Button("helloid");
			Assert.IsNull(helloButton.GetAttributeValue(String.Empty));
		}

		[Test]
		public void Flash()
		{
			ie.TextField("name").Flash();
		}

		[Test]
		public void ElementExists()
		{
			Assert.IsTrue(ie.Div("divid").Exists);
			Assert.IsTrue(ie.Div(new Regex("divid")).Exists);
			Assert.IsFalse(ie.Button("noneexistingelementid").Exists);
		}

		[Test]
		public void WaitUntilElementExistsTestElementAlreadyExists()
		{
			Button button = ie.Button("disabledid");

			Assert.IsTrue(button.Exists);
			button.WaitUntilExists();
			Assert.IsTrue(button.Exists);
		}

		[Test]
		public void WaitUntilElementExistsElementInjectionAfter3Seconds()
		{
			Assert.IsTrue(IE.Settings.WaitUntilExistsTimeOut > 3, "IE.Settings.WaitUntilExistsTimeOut must be more than 3 seconds");

			using (IE ie1 = new IE(TestEventsURI))
			{
				TextField injectedTextField = ie1.TextField("injectedTextField");
				TextField injectedDivTextField = ie1.Div("seconddiv").TextField("injectedTextField");

				Assert.IsFalse(injectedTextField.Exists);
				Assert.IsFalse(injectedDivTextField.Exists);

				ie1.Button("injectElement").ClickNoWait();

				Assert.IsFalse(injectedTextField.Exists);
				Assert.IsFalse(injectedDivTextField.Exists);

				// WatiN should wait until the element exists before
				// getting the text.
				string text = injectedTextField.Text;

				Assert.IsTrue(injectedTextField.Exists);
				Assert.AreEqual("Injection Succeeded", text);
				Assert.IsTrue(injectedDivTextField.Exists);
			}
		}

		[Test]
		public void WaitUntilElementRemovedAfter3Seconds()
		{
			const int indexTextFieldToRemove = 8;

			Assert.IsTrue(IE.Settings.WaitUntilExistsTimeOut > 3, "IE.Settings.WaitUntilExistsTimeOut must be more than 3 seconds");

			using (IE ie1 = new IE(TestEventsURI))
			{
				TextField textfieldToRemove = ie1.TextField("textFieldToRemove");
				TextFieldCollection textfields = ie1.TextFields;

				Assert.AreEqual("textFieldToRemove", textfields[indexTextFieldToRemove].Id);

				Assert.IsTrue(textfieldToRemove.Exists);
				Assert.IsTrue(textfields[indexTextFieldToRemove].Exists);

				ie1.Button("removeElement").ClickNoWait();

				Assert.IsTrue(textfieldToRemove.Exists);
				Assert.IsTrue(textfields[indexTextFieldToRemove].Exists);

				textfieldToRemove.WaitUntilRemoved();

				Assert.IsFalse(textfieldToRemove.Exists);
				Assert.IsFalse(textfields[indexTextFieldToRemove].Exists);
			}
		}

		[Test, ExpectedException(typeof (WatiN.Core.Exceptions.TimeoutException), ExpectedMessage = "Timeout while waiting 1 seconds for element to show up.")]
		public void WaitUntilElementExistsTimeOutException()
		{
			ie.Button("nonexistingbutton").WaitUntilExists(1);
		}

		[Test]
		public void WaitUntil()
		{
			MockRepository mockRepository = new MockRepository();

			INativeElement nativeElement = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			IAttributeBag attributeBag = (IAttributeBag) mockRepository.CreateMock(typeof (IAttributeBag));

			Expect.Call(nativeElement.AttributeBag).Return(attributeBag).Repeat.Times(2);
			Expect.Call(nativeElement.IsElementReferenceStillValid).Return(true).Repeat.Times(2);
			Expect.Call(attributeBag.GetValue("disabled")).Return(true.ToString()).Repeat.Once();
			Expect.Call(attributeBag.GetValue("disabled")).Return(false.ToString()).Repeat.Once();

			mockRepository.ReplayAll();

			Element element = new Element(null, nativeElement);

			// calls htmlelement.getAttribute twice (ones true and once false is returned)
			element.WaitUntil(new AttributeConstraint("disabled", new BoolComparer(false)), 1);

			mockRepository.VerifyAll();
		}

		[Test]
		public void WaitUntilShouldCallExistsToForceRefreshOfHtmlElement()
		{
			MockRepository mockRepository = new MockRepository();

			INativeElement nativeElement = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			IAttributeBag attributeBag = (IAttributeBag) mockRepository.CreateMock(typeof (IAttributeBag));

			Expect.Call(nativeElement.AttributeBag).Return(attributeBag).Repeat.Times(1);
			Expect.Call(attributeBag.GetValue("disabled")).Return(false.ToString()).Repeat.Once();

			element = (Element) mockRepository.DynamicMock(typeof (Element),(DomContainer) null, nativeElement);

			Expect.Call(element.Exists).Return(true);

			mockRepository.ReplayAll();

			element.WaitUntil(new AttributeConstraint("disabled", new BoolComparer(false)), 1);

			mockRepository.VerifyAll();
		}

		[Test]
		public void WaitUntilExistsShouldIgnoreExceptionsDuringWait()
		{
			MockRepository mockRepository = new MockRepository();

			INativeElement nativeElement = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			INativeElementFinder elementFinder = (INativeElementFinder) mockRepository.CreateMock(typeof (INativeElementFinder));

			element = new Element(null, elementFinder);

			Expect.Call(elementFinder.FindFirst()).Return(null).Repeat.Times(5);
			Expect.Call(elementFinder.FindFirst()).Throw(new UnauthorizedAccessException("")).Repeat.Times(4);
			Expect.Call(elementFinder.FindFirst()).Return(nativeElement);

			Expect.Call(nativeElement.GetAttributeValue("innertext")).Return("succeeded").Repeat.Once();

			mockRepository.ReplayAll();

			Assert.AreEqual("succeeded", element.Text);

			mockRepository.VerifyAll();
		}

		[Test]
		public void WaitUntilExistsTimeOutExceptionInnerExceptionNotSetToLastExceptionThrown()
		{
			MockRepository mockRepository = new MockRepository();

			IElementCollection elementCollection = (IElementCollection) mockRepository.CreateMock(typeof (IElementCollection));
			IEElementFinder finder = (IEElementFinder) mockRepository.CreateMock(typeof (IEElementFinder), null, elementCollection);

			Expect.Call(finder.FindFirst()).Throw(new UnauthorizedAccessException(""));
			Expect.Call(finder.FindFirst()).Return(null).Repeat.AtLeastOnce();

			mockRepository.ReplayAll();

			Element element = new Element(null, finder);

			WatiN.Core.Exceptions.TimeoutException timeoutException = null;

			try
			{
				element.WaitUntilExists(1);
			}
			catch (WatiN.Core.Exceptions.TimeoutException e)
			{
				timeoutException = e;
			}

			Assert.IsNotNull(timeoutException, "TimeoutException not thrown");
			Assert.IsNull(timeoutException.InnerException, "Unexpected innerexception");

			mockRepository.VerifyAll();
		}

		[Test]
		public void WaitUntilExistsTimeOutExceptionInnerExceptionSetToLastExceptionThrown()
		{
			MockRepository mockRepository = new MockRepository();

			IElementCollection elementCollection = (IElementCollection) mockRepository.CreateMock(typeof (IElementCollection));
			IEElementFinder finder = (IEElementFinder) mockRepository.DynamicMock(typeof (IEElementFinder), null, elementCollection);

			Expect.Call(finder.FindFirst()).Throw(new Exception(""));
			Expect.Call(finder.FindFirst()).Throw(new UnauthorizedAccessException("mockUnauthorizedAccessException")).Repeat.AtLeastOnce();

			mockRepository.ReplayAll();

			element = new Element(null, finder);

			WatiN.Core.Exceptions.TimeoutException timeoutException = null;

			try
			{
				element.WaitUntilExists(1);
			}
			catch (WatiN.Core.Exceptions.TimeoutException e)
			{
				timeoutException = e;
			}

			Assert.IsNotNull(timeoutException, "TimeoutException not thrown");
			Assert.IsInstanceOfType(typeof (UnauthorizedAccessException), timeoutException.InnerException, "Unexpected innerexception");
			Assert.AreEqual("mockUnauthorizedAccessException", timeoutException.InnerException.Message);

			mockRepository.VerifyAll();
		}

		[Test]
		public void WaitUntilExistsShouldReturnImmediatelyIfElementIsSet()
		{
			MockRepository mockRepository = new MockRepository();

			INativeElement nativeElement = (INativeElement) mockRepository.CreateMock(typeof (INativeElement));
			Element mockElement = (Element) mockRepository.DynamicMock(typeof (Element), null, nativeElement);

			Expect.Call(mockElement.Exists).Repeat.Never();

			mockRepository.ReplayAll();

			mockElement.WaitUntilExists(3);

			mockRepository.VerifyAll();
		}

		[Test, ExpectedException(typeof (WatiN.Core.Exceptions.TimeoutException), ExpectedMessage = "Timeout while waiting 1 seconds for element matching constraint: Attribute 'disabled' with value 'True'")]
		public void WaitUntilTimesOut()
		{
			element = ie.Form("Form");
			Assert.That(element.GetAttributeValue("disabled"), Iz.EqualTo(false.ToString()), "Expected enabled form");

			element.WaitUntil(new AttributeConstraint("disabled", true.ToString()), 1);
		}

		[Test]
		public void ElementShouldBeFoundAfterRedirect()
		{
			ie.GoTo(new Uri(HtmlTestBaseURI, "intro.html"));

			Assert.IsFalse(ie.TextField("TheTextBox").Exists);

			ie.TextField("TheTextBox").WaitUntilExists(10);

			Assert.IsTrue(ie.TextField("TheTextBox").Exists);
		}

		[Test]
		public void GetAttributeValueOfTypeInt()
		{
			Assert.AreEqual("10", ie.Form("Form").GetAttributeValue("sourceIndex"));
		}

		[Test]
		public void FireKeyDownEventOnElementWithNoId()
		{
			ie.GoTo(TestEventsURI);

			TextField report = ie.TextField("Report");
			Core.Button button = ie.Button(Find.ByValue("Button without id"));

			Assert.IsNull(button.Id, "Button id not null before click event");
			Assert.IsNull(report.Text, "Report not empty");

			button.KeyDown();

			Assert.IsNotNull(report.Text, "No keydown event fired (report is empty )");
			Assert.AreEqual("button.id = ", report.Text, "Report should start with 'button.id = '");

			Assert.IsNull(button.Id, "Button id not null after click event");
		}

		[Test]
		public void FireEventAlwaysSetsLeftMouseOnEventObject()
		{
			ie.GoTo(TestEventsURI);
			
			// test in standard IE window
			ie.Button(Find.ByValue("Button without id")).KeyDown();

			Assert.AreEqual("1", ie.TextField("eventButtonValue").Value, "Event.button not left");

			// test in HTMLDialog window
			ie.Button("modalid").ClickNoWait();

			using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByIndex(0)))
			{
				htmlDialog.Button(Find.ByValue("Button without id")).KeyDown();

				Assert.AreEqual("1", htmlDialog.TextField("eventButtonValue").Value, "Event.button not left on modal dialog");
			}
		}

		[Test, Ignore("Work in progress")] // Category("InternetConnectionNeeded")]
		public void PositionMousePointerInMiddleOfElement()
		{
			ie.GoTo(GoogleUrl);

			Button button = ie.Button(Find.ByName("btnG"));
			PositionMousePointerInMiddleOfElement(button, ie);
			button.Flash();
			MouseMove(50, 50, true);
			Thread.Sleep(2000);
		}

		[Test, Ignore("Doesn't work yet")]
		public void PositionMousePointerInMiddleOfElementInFrame()
		{
			IE.Settings.MakeNewIeInstanceVisible = true;
			IE.Settings.HighLightElement = true;

			using (IE ie = new IE(FramesetURI))
			{
				Link button = ie.Frames[1].Links[0];
				PositionMousePointerInMiddleOfElement(button, ie);
				button.Flash();
				MouseMove(50, 50, true);
				Thread.Sleep(2000);
			}
		}

		private static void PositionMousePointerInMiddleOfElement(Element button, IE ie) 
		{
			int left = position(button, "Left");
			int width = int.Parse(button.GetAttributeValue("clientWidth"));
			int top = position(button, "Top");
			int height = int.Parse(button.GetAttributeValue("clientHeight"));
			
			IHTMLWindow3 window = (IHTMLWindow3) ie.HtmlDocument.parentWindow;
			
			left = left + window.screenLeft;
			top = top + window.screenTop;

			System.Drawing.Point currentPt = new System.Drawing.Point(left + (width / 2), top + (height / 2));
			System.Windows.Forms.Cursor.Position = currentPt;
		}

		private static int position(Element element, string attributename)
		{
			int pos = 0;
			IHTMLElement offsetParent = ((IHTMLElement)element.HTMLElement).offsetParent;
			if (offsetParent != null)
			{
				pos = position(new Element(element.DomContainer, offsetParent), attributename);
			}

			if (StringComparer.AreEqual(element.TagName, "table", true))
			{
				pos = pos + int.Parse(element.GetAttributeValue("client" + attributename));
			}
			return pos + int.Parse(element.GetAttributeValue("offset" + attributename));
		}

		public void MouseMove(int X, int Y, bool Relative)
		{
			System.Drawing.Point currentPt = System.Windows.Forms.Cursor.Position;
			if (Relative)
			{
				currentPt.X += X;
				currentPt.Y += Y;
			}
			else
			{
				currentPt.X = X;
				currentPt.Y = Y;
			}

			System.Windows.Forms.Cursor.Position = currentPt;
		}

		[Test]
		public void FireEventAlwaysSetsSrcElementOnEventObject()
		{
			ie.GoTo(TestEventsURI);

			// test in standard IE window
			ie.Button(Find.ByValue("Button without id")).KeyDown();

			Assert.AreEqual("Button without id", ie.TextField("eventScrElement").Value, "Unexpected Event.scrElement.value");

			// test in HTMLDialog window
			ie.Button("modalid").ClickNoWait();

			using (HtmlDialog htmlDialog = ie.HtmlDialog(Find.ByIndex(0)))
			{
				htmlDialog.Button(Find.ByValue("Button without id")).KeyDown();

				Assert.AreEqual("Button without id", htmlDialog.TextField("eventScrElement").Value, "Unexpected Event.scrElement.value");
			}
		}

		[Test]
		public void HighlightShouldGoBackToTheOriginalBackGroundColor()
		{
			IE.Settings.HighLightElement = true;
			IE.Settings.HighLightColor = "red";

			TextField textField = ie.TextField("name");
			string _originalcolor = textField.Style.BackgroundColor;

			textField.Highlight(true);
			Assert.That(textField.Style.BackgroundColor, Iz.EqualTo("red"), "Unexpected background after Highlight(true)");

			// Invoke highlighting done by WatiN when typing text
			IE.Settings.HighLightColor = "yellow";
			textField.TypeText("abc");

			Assert.That(textField.Style.BackgroundColor, Iz.EqualTo("red"), "Unexpected background after TypeText");
		
			textField.Highlight(false);
			Assert.That(textField.Style.BackgroundColor, Iz.EqualTo(_originalcolor), "Unexpected background Highlight(false)");
		}

		[Test]
		public void HighlightShouldNotThrowExceptionWhenCalledToManyTimesWithParamFalse()
		{
			IE.Settings.HighLightElement = true;
			IE.Settings.HighLightColor = "red";

			TextField textField = ie.TextField("name");
			string _originalcolor = textField.Style.BackgroundColor;

			textField.Highlight(true);
			Assert.That(textField.Style.BackgroundColor, Iz.EqualTo("red"), "Unexpected background after Highlight(true)");
		
			textField.Highlight(false);
			Assert.That(textField.Style.BackgroundColor, Iz.EqualTo(_originalcolor), "Unexpected background Highlight(false)");

			textField.Highlight(false);
			Assert.That(textField.Style.BackgroundColor, Iz.EqualTo(_originalcolor), "Unexpected background Highlight(false)");
		}


#if NET20
        [Test]
        public void AncestorGenericType()
        {
            MockRepository mockRepository = new MockRepository();

            INativeElement nativeElement = (INativeElement)mockRepository.CreateMock(typeof(INativeElement));
            INativeElement firstParentDiv = (INativeElement)mockRepository.CreateMock(typeof(INativeElement));
            INativeElement secondParentDiv = (INativeElement)mockRepository.CreateMock(typeof(INativeElement));

            element = new Element(null, nativeElement);
            Expect.Call(nativeElement.Parent).Return(firstParentDiv).Repeat.Any();
            Expect.Call(firstParentDiv.TagName).Return("div").Repeat.Any();

            Expect.Call(firstParentDiv.Parent).Return(secondParentDiv).Repeat.Any();
            Expect.Call(secondParentDiv.TagName).Return("div").Repeat.Any();

            mockRepository.ReplayAll();

            Assert.That(element.Ancestor<Div>(), NUnit.Framework.SyntaxHelpers.Is.Not.Null);

        	mockRepository.VerifyAll();
        }

        [Test]
        public void AncestorGenericTypeAndAttributeConstraintShouldReturnTypedElement()
        {
            MockRepository mockRepository = new MockRepository();

            INativeElement nativeElement = (INativeElement)mockRepository.CreateMock(typeof(INativeElement));
            INativeElement firstParentDiv = (INativeElement)mockRepository.CreateMock(typeof(INativeElement));
            IAttributeBag firstAttributeBag = (IAttributeBag)mockRepository.CreateMock(typeof(IAttributeBag));
            INativeElement secondParentDiv = (INativeElement)mockRepository.CreateMock(typeof(INativeElement));
            IAttributeBag secondAttributeBag = (IAttributeBag)mockRepository.CreateMock(typeof(IAttributeBag));

            element = new Element(null, nativeElement);
            Expect.Call(nativeElement.Parent).Return(firstParentDiv).Repeat.Any();
            Expect.Call(firstParentDiv.TagName).Return("div").Repeat.Any();
            Expect.Call(firstParentDiv.AttributeBag).Return(firstAttributeBag);
            Expect.Call(firstAttributeBag.GetValue("innertext")).Return("first ancestor");

            Expect.Call(firstParentDiv.Parent).Return(secondParentDiv).Repeat.Any();
            Expect.Call(secondParentDiv.TagName).Return("div").Repeat.Any();
            Expect.Call(secondParentDiv.AttributeBag).Return(secondAttributeBag);
            Expect.Call(secondAttributeBag.GetValue("innertext")).Return("second ancestor");
            Expect.Call(secondParentDiv.GetAttributeValue("innertext")).Return("second ancestor");

            mockRepository.ReplayAll();

            Element ancestor = element.Ancestor<Div>(Find.ByText("second ancestor"));
          
            Assert.IsInstanceOfType(typeof (Div), ancestor);
            Assert.That(ancestor.Text, NUnit.Framework.SyntaxHelpers.Is.EqualTo("second ancestor"));
            
            mockRepository.VerifyAll();
        }

#endif
	}
}