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
using System.Text.RegularExpressions;
using System.Threading;
using Moq;
using mshtml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native;
using WatiN.Core.Native.Windows;
using StringComparer=WatiN.Core.Comparers.StringComparer;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementTests : BaseWithBrowserTests
	{
		private Element element;

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}

		[TearDown]
		public void TearDown()
		{
			Settings.Reset();
		}

        // TODO: This should be mocked cause there is no browser logic involved
		[Test]
		public void AncestorTypeShouldReturnTypedElement()
		{
            // GIVEN
			var tableCell = Ie.TableCell(Find.ByText("Contains text in DIV"));
		    
            // WHEN
            var ancestor = tableCell.Ancestor(typeof (Div));
		    
            //THEN
            Assert.IsInstanceOfType(typeof (Div), ancestor);
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void AncestorTagNameShouldReturnTypedElement()
		{
			var tableCell = Ie.TableCell(Find.ByText("Contains text in DIV"));
			Assert.IsInstanceOfType(typeof (Div), tableCell.Ancestor("Div"));
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void AncestorAttributeConstraintShouldReturnTypedElement()
		{
			var tableCell = Ie.TableCell(Find.ByText("Contains text in DIV"));
			Assert.IsInstanceOfType(typeof (Div), tableCell.Ancestor(Find.ById("divid")));
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void AncestorTypeAndAttributeConstraintShouldReturnTypedElement()
		{
		    var tableCell = Ie.TableCell(Find.ByText("Contains text in DIV"));
            var ancestor = tableCell.Ancestor(typeof(Div), Find.ById("divid"));

            Assert.IsInstanceOfType(typeof(Div), ancestor);
            Assert.That(ancestor.Id, Is.EqualTo("divid"));
		}

	    [Test]
		public void AncestorTagNameAndAttributeConstraintShouldReturnTypedElement()
		{
            // GIVEN
	        var nativeElementMock = new Mock<INativeElement>();
			var firstParentDivMock = new Mock<INativeElement>();
			var secondParentDivMock = new Mock<INativeElement>();
            var domContainerMock = new Mock<DomContainer> (new object[] { });

			element = new Element(domContainerMock.Object, nativeElementMock.Object);

            nativeElementMock.Expect(native => native.Parent).Returns(firstParentDivMock.Object);
			
            firstParentDivMock.Expect(first => first.TagName).Returns("div");
            firstParentDivMock.Expect(first => first.GetAttributeValue("tagName")).Returns("div");
            firstParentDivMock.Expect(first => first.GetAttributeValue("innertext")).Returns("first ancestor");

			firstParentDivMock.Expect(first => first.Parent).Returns(secondParentDivMock.Object);
			
            secondParentDivMock.Expect(second => second.TagName).Returns("div");
            secondParentDivMock.Expect(second => second.GetAttributeValue("tagName")).Returns("div");
            secondParentDivMock.Expect(second => second.GetAttributeValue("innertext")).Returns("second ancestor");

            // WHEN
			var ancestor = element.Ancestor("Div", Find.ByText("second ancestor"));

            // THEN
            nativeElementMock.VerifyAll();
            firstParentDivMock.VerifyAll();
            secondParentDivMock.VerifyAll();

			Assert.IsInstanceOfType(typeof (Div), ancestor);
			Assert.That(ancestor.Text, Is.EqualTo("second ancestor"));
		}

		[Test]
		public void ElementParentShouldReturnNullWhenRootElement()
		{
			var nativeElementMock = new Mock<INativeElement>();
            var domContainer = new Mock<DomContainer> (new object[] { }).Object;

			nativeElementMock.Expect(native => native.Parent).Returns((INativeElement) null);
			element = new Element(domContainer, nativeElementMock.Object); 

			Assert.IsNull(element.Parent);

			nativeElementMock.VerifyAll();
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void ElementParentReturningTypedParent()
		{
			var tableCell = Ie.TableCell(Find.ByText("Contains text in DIV"));
			
            Assert.IsInstanceOfType(typeof (TableRow), tableCell.Parent);
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void ElementParentReturnsElementsContainerForUnknownElement()
		{
			var parent = Ie.Body.Parent;
		    var container = parent as IElementContainer;
            
            Assert.That(container, Is.Not.Null, "Should implement IElementsContainer");
            Assert.That(parent.GetType(), Is.EqualTo(typeof(ElementContainer<Element>)), "Should be ElementsContainer<Element>");
        }

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void ElementWithTagReturnElementsContainer()
		{
			var elementWithTag = Ie.ElementWithTag("form", Find.ById("Form"));
		    var container = elementWithTag as IElementContainer;
            
            Assert.That(container, Is.Not.Null, "Should implement IElementsContainer");
            Assert.That(elementWithTag.GetType(), Is.EqualTo(typeof(ElementContainer<Element>)), "Should be ElementsContainer<Element>");
        }

		[Test]
		public void ElementPreviousSiblingShouldReturnNullWhenFirstSibling()
		{
		    ExecuteTest(browser =>
		                    {
                                var first = browser.Div("NextAndPreviousTests").Div("first").PreviousSibling;
		                        Assert.IsNull(first);
		                    });
		}

	    [Test]
		public void ElementPreviousSiblingReturningTypedParent()
		{
			Assert.IsTrue(Ie.RadioButton("Radio1").PreviousSibling.GetType().Equals(typeof (CheckBox)));
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void ElementPreviousSiblingReturnsElementsContainerForUnknowElement()
		{
			var previous = Ie.Div("NextAndPreviousTests").Div("last").PreviousSibling;
		    var container = previous as IElementContainer;
            
            Assert.That(container, Is.Not.Null, "Should implement IElementsContainer");
            Assert.IsTrue(previous.GetType().Equals(typeof(ElementContainer<Element>)), "Should be ElementsContainer<Element>");
        }

		[Test]
		public void ElementNextSiblingShouldReturnNullWhenLastSibling()
		{
		    ExecuteTest(browser =>
		                    {
		                        var next = browser.Div("NextAndPreviousTests").Div("last").NextSibling;
			
		                        Assert.IsNull(next);
		                    });
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void ElementNextSiblingReturningTypedParent()
		{
			var next = Ie.Div("NextAndPreviousTests").Div("first").NextSibling;
			
            Assert.IsTrue(next.GetType().Equals(typeof (Span)));
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void ElementNextSiblingReturnsElementsContainerForUnknowElement()
		{
			var next = Ie.Div("NextAndPreviousTests").Span("second").NextSibling;
		    var container = next as IElementContainer;
            
            Assert.That(container, Is.Not.Null, "Should implement IElementsContainer");
            Assert.IsTrue(next.GetType().Equals(typeof (ElementContainer<Element>)), "Should be ElementsContainer<Element>");
        }

		[Test]
		public void ElementRefresh()
		{
			var finderMock = new Mock<ElementFinder>();
			var nativeElementMock = new Mock<INativeElement>();
            var domContainer = new Mock<DomContainer>( new object[] { });

			finderMock.Expect(finder => finder.FindFirst()).Returns(new Element(domContainer.Object, nativeElementMock.Object)).AtMost(2);
			nativeElementMock.Expect(native => native.TagName).Returns("mockedtag");

            element = new Element(domContainer.Object, finderMock.Object);

			Assert.AreEqual("mockedtag", element.TagName);

			element.Refresh();

			Assert.AreEqual("mockedtag", element.TagName);

			finderMock.VerifyAll();
		}

		[Test, ExpectedException(typeof (ArgumentException))]
		public void AncestorTypeShouldOnlyExceptTypesInheritingElement()
		{
            // GIVEN
		    var element1 = new Element(new Mock<DomContainer>().Object, new  Mock<INativeElement>().Object);

            // WHEN
			element1.Ancestor(typeof (String));

            // THEN exception
		}

		[Test]
		public void Element()
		{
		    ExecuteTest(browser =>
		                    {
                                element = browser.Element(Find.ById("table1"));

		                        var container = element as IElementContainer;
		                        Assert.That(container, Is.Not.Null, "Should implement IElementsContainer");
		                        Assert.IsAssignableFrom(typeof(ElementContainer<Element>), element, "The returned object form ie.Element should be castable to ElementsContainer<Element>");

		                        Assert.IsNotNull(element, "Element not found");

		                        // check behavior for standard attribute
		                        Assert.AreEqual("table1", element.GetAttributeValue("id"), "GetAttributeValue id failed");
		                        // check behavior for non existing attribute
		                        Assert.IsNull(element.GetAttributeValue("watin"), "GetAttributeValue watin should return null");
		                        // check behavior for custom attribute
		                        Assert.AreEqual("myvalue", element.GetAttributeValue("myattribute"), "GetAttributeValue myattribute should return myvalue");

		                        Assert.AreEqual("table", element.TagName.ToLower(), "Invalid tagname");

		                        // Textbefore and TextAfter tests
                                var checkBox = browser.CheckBox("Checkbox21");

		                        var textBefore = checkBox.TextBefore;
		                        Assert.AreEqual("Test label before: ", textBefore, "Unexpected checkBox.TextBefore");
                                
                                var textAfter = checkBox.TextAfter;
                                Assert.AreEqual(" Test label after", textAfter, "Unexpected checkBox.TextAfter");

		                        var label = checkBox.Parent;
		                        Assert.That(label, Is.InstanceOfType(typeof(Label)), "Expected a label");
                                Assert.That(label.TextAfter.Trim(), Is.Empty, "Unexpected label.TextAfter");
		                    });
		}

		[Test]
		public void ElementByTagNameAndInputType()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.ElementWithTag("input", Find.By("id", "name"), "text");
		                        Assert.IsTrue(element.Exists);
		                    });
		}

		[Test]
		public void ElementByTagName()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.ElementWithTag("a", Find.By("id", "testlinkid"));
		                        Assert.IsTrue(element.Exists);
		                    });
		}

		[Test]
		public void FindHeadElementByTagName()
		{
		    ExecuteTest(browser =>
		                    {
                                element = browser.ElementWithTag("head", Find.ByIndex(0));
		                        Assert.IsTrue(element.Exists);
		                    });
		}

		[Test]
		public void ElementFindByShouldNeverThrowInvalidAttributeException()
		{
		    ExecuteTest(browser =>
		                    {
		                        element = browser.Element(Find.ByFor("Checkbox21"));
		                        Assert.IsTrue(element.Exists);
		                    });
		}

		[Test]
		public void ElementCollectionExistsShouldNeverThrowInvalidAttributeException()
		{
		    ExecuteTest(browser => Assert.IsTrue(browser.Elements.Exists(Find.ByFor("Checkbox21"))));
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void ElementCollectionShouldReturnTypedElements()
		{
			var elements = Ie.Div("NextAndPreviousTests").Elements;
			Assert.IsTrue(elements[0].GetType().Equals(typeof (Div)), "Element 0 should be a div");
			Assert.IsTrue(elements[1].GetType().Equals(typeof (Span)), "Element 1 should be a span");

		    var container = elements[2] as IElementContainer;
            Assert.That(container, Is.Not.Null, "Element 2 should be an IElementsContainer");
            Assert.IsTrue(elements[2].GetType().Equals(typeof(ElementContainer<Element>)), "Element 2 should be an ElementsContainer<Element>");
            Assert.IsTrue(elements[3].GetType().Equals(typeof (Div)), "Element 3 should be a div");
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void ElementWithTagShouldReturnTypedElements()
		{
			var elements = Ie.ElementsWithTag("div");
			Assert.IsTrue(elements[0].GetType().Equals(typeof (Div)), "Element 0 should be a div");
		}

		[Test]
		public void ElementCollectionSecondFilterShouldNeverThrowInvalidAttributeException()
		{
		    ExecuteTest(browser =>
		                    {
		                        var elements = browser.Elements.Filter(Find.ById("testlinkid"));
		                        var elements2 = elements.Filter(Find.ByFor("Checkbox21"));
                                Assert.AreEqual(0, elements2.Count);
		                    });
		}

		[Test]
		public void GetInvalidAttribute()
		{
		    ExecuteTest(browser =>
		                    {
		                        Element helloButton = browser.Button("helloid");
		                        Assert.IsNull(helloButton.GetAttributeValue("NONSENCE"));
		                    });
		}

		[Test]
		public void GetValidButUndefinedAttribute()
		{
		    ExecuteTest(browser =>
		                    {
		                        Element helloButton = browser.Button("helloid");
		                        Assert.IsNull(helloButton.GetAttributeValue("title"));
		                    });
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void GetAttributeValueOfNullThrowsArgumentNullException()
		{
            // GIVEN
			var helloButton = new Element(new Mock<DomContainer>().Object, new Mock<INativeElement>().Object);
			
            // WHEN
            helloButton.GetAttributeValue(null);

            // THEN exception
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void GetAttributeValueOfEmptyStringThrowsArgumentNullException()
		{
            // GIVEN
            var helloButton = new Element(new Mock<DomContainer>().Object, new Mock<INativeElement>().Object);

            // WHEN
            helloButton.GetAttributeValue(string.Empty);

            // THEN exception
        }

		[Test]
		public void Flash()
		{
		    ExecuteTest(browser => browser.TextField("name").Flash());
		}

		[Test]
		public void ElementExists()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.IsTrue(browser.Div("divid").Exists);
		                        Assert.IsTrue(browser.Div(new Regex("divid")).Exists);
		                        Assert.IsFalse(browser.Button("noneexistingelementid").Exists);
		                    });
		}

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
		public void WaitUntilElementExistsTestElementAlreadyExists()
		{
			var button = Ie.Button("disabledid");

			Assert.IsTrue(button.Exists);
			button.WaitUntilExists();
			Assert.IsTrue(button.Exists);
		}

		[Test]
		public void WaitUntilElementExistsElementInjectionAfter3Seconds()
		{
		    ExecuteTest(browser =>
		                    {
                                Assert.IsTrue(Settings.WaitUntilExistsTimeOut > 3, "Settings.WaitUntilExistsTimeOut must be more than 3 seconds");
                                
                                browser.GoTo(TestEventsURI);
		                        
                                var injectedTextField = browser.TextField("injectedTextField");
		                        var injectedDivTextField = browser.Div("seconddiv").TextField("injectedTextField");

		                        Assert.IsFalse(injectedTextField.Exists);
		                        Assert.IsFalse(injectedDivTextField.Exists);

		                        browser.Button("injectElement").ClickNoWait();

		                        Assert.IsFalse(injectedTextField.Exists);
		                        Assert.IsFalse(injectedDivTextField.Exists);

		                        // WatiN should wait until the element exists before
		                        // getting the text.
		                        var text = injectedTextField.Text;

		                        Assert.IsTrue(injectedTextField.Exists);
		                        Assert.AreEqual("Injection Succeeded", text);
		                        Assert.IsTrue(injectedDivTextField.Exists);
		                    });
		}

		[Test]
		public void WaitUntilElementRemovedAfter3Seconds()
		{
			const int indexTextFieldToRemove = 9;

		    ExecuteTest(browser =>
		                    {
		                        Assert.IsTrue(Settings.WaitUntilExistsTimeOut > 3,
		                                      "Settings.WaitUntilExistsTimeOut must be more than 3 seconds");

		                        browser.GoTo(TestEventsURI);

		                        var textfieldToRemove = browser.TextField("textFieldToRemove");
		                        var textfields = browser.TextFields;

		                        Assert.AreEqual("textFieldToRemove", textfields[indexTextFieldToRemove].Id);

		                        Assert.IsTrue(textfieldToRemove.Exists);
		                        Assert.IsTrue(textfields[indexTextFieldToRemove].Exists);

		                        browser.Button("removeElement").ClickNoWait();

		                        Assert.IsTrue(textfieldToRemove.Exists);
		                        Assert.IsTrue(textfields[indexTextFieldToRemove].Exists);

		                        textfieldToRemove.WaitUntilRemoved();

		                        Assert.IsFalse(textfieldToRemove.Exists);

                                Assert.AreEqual("textFieldToRemove", textfields[indexTextFieldToRemove].Id);
                                Assert.IsFalse(textfields[indexTextFieldToRemove].Exists);
		                    });
		}

		[Test, ExpectedException(typeof (Exceptions.TimeoutException), ExpectedMessage = "Timeout while waiting 1 seconds for element to show up.")]
		public void WaitUntilElementExistsTimeOutException()
		{
            // GIVEN
		    var elementFinderMock = new Mock<ElementFinder>();
            elementFinderMock.Expect(finder => finder.FindFirst()).Returns((Element)null);

            var element1 = new Element(new Mock<DomContainer>().Object, elementFinderMock.Object);
		    
            // WHEN
            element1.WaitUntilExists(1);

            // THEN exception
		}

		[Test]
		public void WaitUntil()
		{
			var nativeElementMock = new Mock<INativeElement>();
            var domContainerMock = new Mock<DomContainer>(new object[] { });

            nativeElementMock.Expect(native => native.IsElementReferenceStillValid()).Returns(true).AtMost(2);
            nativeElementMock.Expect(native => native.GetAttributeValue("disabled")).Returns(true.ToString()).AtMostOnce();
            nativeElementMock.Expect(native => native.GetAttributeValue("disabled")).Returns(false.ToString()).AtMostOnce();

			var element1 = new Element(domContainerMock.Object, nativeElementMock.Object);

			// calls htmlelement.getAttribute twice (ones true and once false is returned)
			element1.WaitUntil(new AttributeConstraint("disabled", new BoolComparer(false)), 1);

			nativeElementMock.VerifyAll();
		}

		[Test]
		public void WaitUntilShouldCallExistsToForceRefreshOfHtmlElement()
		{
			var nativeElementMock = new Mock<INativeElement>();
            var domContainerMock = new Mock<DomContainer>(new object[] { });

			nativeElementMock.Expect(native => native.GetAttributeValue("disabled")).Returns(false.ToString()); //.AtMostOnce();

			var elementMock = new Mock<Element>(domContainerMock.Object, nativeElementMock.Object);

		    elementMock.Expect(elem => elem.Exists).Returns(true);
		    var element1 = elementMock.Object;

			element1.WaitUntil(new AttributeConstraint("disabled", new BoolComparer(false)), 1);

            elementMock.VerifyAll();
		}

		[Test]
		public void WaitUntilExistsShouldIgnoreExceptionsDuringWait()
		{
			var nativeElementMock = new Mock<INativeElement>();
			var elementFinderMock = new Mock<ElementFinder>();
            var domContainerMock = new Mock<DomContainer>( new object[] { });

			element = new Element(domContainerMock.Object, elementFinderMock.Object);

			elementFinderMock.Expect(finder => finder.FindFirst()).Returns((Element) null).AtMost(5);
            elementFinderMock.Expect(finder => finder.FindFirst()).Throws(new UnauthorizedAccessException("")).AtMost(4);
            elementFinderMock.Expect(finder => finder.FindFirst()).Returns(new Element(domContainerMock.Object, nativeElementMock.Object));

			nativeElementMock.Expect(native => native.GetAttributeValue("innertext")).Returns("succeeded").AtMostOnce();

			Assert.AreEqual("succeeded", element.Text);

            nativeElementMock.VerifyAll();
            elementFinderMock.VerifyAll();
            domContainerMock.VerifyAll();
        }

		[Test]
		public void WaitUntilExistsTimeOutExceptionInnerExceptionNotSetToLastExceptionThrown()
		{
            var domContainerMock = new Mock<DomContainer>( new object[] { });
			var finderMock = new Mock<ElementFinder> (null, null);

			finderMock.Expect(finder => finder.FindFirst()).Throws(new UnauthorizedAccessException(""));
            finderMock.Expect(finder => finder.FindFirst()).Returns((Element) null); //.AtMostOnce();

			var element1 = new Element(domContainerMock.Object, finderMock.Object);

			Exceptions.TimeoutException timeoutException = null;

			try
			{
				element1.WaitUntilExists(1);
			}
			catch (Exceptions.TimeoutException e)
			{
				timeoutException = e;
			}

			Assert.IsNotNull(timeoutException, "TimeoutException not thrown");
			Assert.IsNull(timeoutException.InnerException, "Unexpected innerexception");

			domContainerMock.VerifyAll();
            finderMock.VerifyAll();
		}

		[Test]
		public void WaitUntilExistsTimeOutExceptionInnerExceptionSetToLastExceptionThrown()
		{
            var domContainerMock = new Mock<DomContainer>(new object[] { });
            var finderMock = new Mock<ElementFinder>(null, null);

			finderMock.Expect(finder => finder.FindFirst()).Throws(new Exception(""));
            finderMock.Expect(finder => finder.FindFirst()).Throws(new UnauthorizedAccessException("mockUnauthorizedAccessException")).AtMostOnce();

			element = new Element(domContainerMock.Object, finderMock.Object);

			Exceptions.TimeoutException timeoutException = null;

			try
			{
				element.WaitUntilExists(1);
			}
			catch (Exceptions.TimeoutException e)
			{
				timeoutException = e;
			}

			Assert.IsNotNull(timeoutException, "TimeoutException not thrown");
			Assert.IsInstanceOfType(typeof (UnauthorizedAccessException), timeoutException.InnerException, "Unexpected innerexception");
			Assert.AreEqual("mockUnauthorizedAccessException", timeoutException.InnerException.Message);

            domContainerMock.VerifyAll();
            finderMock.VerifyAll();
		}

		[Test]
		public void WaitUntilExistsShouldReturnImmediatelyIfElementIsSet()
		{
			var nativeElementMock = new Mock<INativeElement>();
            var domContainerMock = new Mock<DomContainer>(new object[] { });
            var elementMock = new Mock<Element>(domContainerMock.Object, nativeElementMock.Object);

			elementMock.Expect(elem => elem.Exists).Never();

			elementMock.Object.WaitUntilExists(3);

            elementMock.VerifyAll();
		}

        // TODO: This should be mocked cause there is no browser logic involved
		[Test, ExpectedException(typeof (Exceptions.TimeoutException), ExpectedMessage = "Timeout while waiting 1 seconds for element matching constraint: Attribute 'disabled' equals 'True'")]
		public void WaitUntilTimesOut()
		{
			element = Ie.Form("Form");
			Assert.That(element.GetAttributeValue("disabled"), Is.EqualTo(false.ToString()), "Expected enabled form");

			element.WaitUntil(new AttributeConstraint("disabled", true.ToString()), 1);
		}

		[Test]
		public void ElementShouldBeFoundAfterRedirect()
		{
		    ExecuteTest(browser =>
		                    {
		                        browser.GoTo(new Uri(HtmlTestBaseURI, "intro.html"));

		                        Assert.IsFalse(browser.TextField("TheTextBox").Exists);

		                        browser.TextField("TheTextBox").WaitUntilExists(10);

		                        Assert.IsTrue(browser.TextField("TheTextBox").Exists);
		                    });
		}

		[Test]
		public void GetAttributeValueOfTypeInt()
		{
		    ExecuteTest(browser => Assert.AreEqual("1", browser.Form("Form").GetAttributeValue("nodeType")));
		}

        // Ie specific test to see if the way the element is referenced to fire an event on works.
		[Test]
		public void FireKeyDownEventOnElementWithNoId()
		{
			Ie.GoTo(TestEventsURI);

			var report = Ie.TextField("Report");
			var button = Ie.Button(Find.ByValue("Button without id"));

			Assert.IsNull(button.Id, "Button id not null before click event");
			Assert.IsNull(report.Text, "Report not empty");

			button.KeyDown();

			Assert.IsNotNull(report.Text, "No keydown event fired (report is empty )");
			Assert.AreEqual("button.id = ", report.Text, "Report should start with 'button.id = '");

			Assert.IsNull(button.Id, "Button id not null after click event");
		}

        // TODO: make this a multi browser test when HtmlDialogs are implemented for firefox
		[Test]
		public void FireEventAlwaysSetsLeftMouseOnEventObject()
		{
			Ie.GoTo(TestEventsURI);
			
			// test in standard IE window
			Ie.Button(Find.ByValue("Button without id")).KeyDown();

			Assert.AreEqual("1", Ie.TextField("eventButtonValue").Value, "Event.button not left");

			// test in HTMLDialog window
			Ie.Button("modalid").ClickNoWait();

			using (var htmlDialog = Ie.HtmlDialog(Find.ByIndex(0), 2))
			{
				htmlDialog.Button(Find.ByValue("Button without id")).KeyDown();

				Assert.AreEqual("1", htmlDialog.TextField("eventButtonValue").Value, "Event.button not left on modal dialog");
			}
		}

		[Test, Ignore("Work in progress")] // Category("InternetConnectionNeeded")]
		public void PositionMousePointerInMiddleOfElement()
		{
			Ie.GoTo(GoogleUrl);

			var button = Ie.Button(Find.ByName("btnG"));
			PositionMousePointerInMiddleOfElement(button, Ie);
			button.Flash();
			MouseMove(50, 50, true);
			Thread.Sleep(2000);
		}

		[Test, Ignore("Doesn't work yet")]
		public void PositionMousePointerInMiddleOfElementInFrame()
		{
			Settings.MakeNewIeInstanceVisible = true;
			Settings.HighLightElement = true;

			using (var ie = new IE(FramesetURI))
			{
				var button = ie.Frames[1].Links[0];
				PositionMousePointerInMiddleOfElement(button, ie);
				button.Flash();
				MouseMove(50, 50, true);
				Thread.Sleep(2000);
			}
		}

		private static void PositionMousePointerInMiddleOfElement(Element button, Document ie) 
		{
			var left = position(button, "Left");
			var width = int.Parse(button.GetAttributeValue("clientWidth"));
			var top = position(button, "Top");
			var height = int.Parse(button.GetAttributeValue("clientHeight"));
			
			var window = (IHTMLWindow3) ((IEDocument)ie.NativeDocument).HtmlDocument.parentWindow;
			
			left = left + window.screenLeft;
			top = top + window.screenTop;

			var currentPt = new System.Drawing.Point(left + (width / 2), top + (height / 2));
			System.Windows.Forms.Cursor.Position = currentPt;
		}

		private static int position(Element element, string attributename)
		{
            IEElement ieElement = (IEElement)element.NativeElement;

			var pos = 0;
            var offsetParent = ieElement.AsHtmlElement.offsetParent;
			if (offsetParent != null)
			{
			    var domContainer = element.DomContainer;
			    pos = position(new Element(domContainer, new IEElement(offsetParent)), attributename);
			}

		    if (StringComparer.AreEqual(element.TagName, "table", true))
			{
				pos = pos + int.Parse(element.GetAttributeValue("client" + attributename));
			}
			return pos + int.Parse(element.GetAttributeValue("offset" + attributename));
		}

	    private void MouseMove(int X, int Y, bool Relative)
		{
			var currentPt = System.Windows.Forms.Cursor.Position;
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

        // Ie specific test
		[Test]
		public void FireEventAlwaysSetsSrcElementOnEventObject()
		{
			Ie.GoTo(TestEventsURI);

			// test in standard IE window
			Ie.Button(Find.ByValue("Button without id")).KeyDown();

			Assert.AreEqual("Button without id", Ie.TextField("eventScrElement").Value, "Unexpected Event.scrElement.value");

			// test in HTMLDialog window
			Ie.Button("modalid").ClickNoWait();

			using (var htmlDialog = Ie.HtmlDialog(Find.ByIndex(0), 2))
			{
				htmlDialog.Button(Find.ByValue("Button without id")).KeyDown();

				Assert.AreEqual("Button without id", htmlDialog.TextField("eventScrElement").Value, "Unexpected Event.scrElement.value");
			}
		}

		[Test]
		public void HighlightShouldGoBackToTheOriginalBackGroundColor()
		{

		    ExecuteTest(browser =>
		                    {
                                Settings.HighLightElement = true;
                                Settings.HighLightColor = "red";
                                
                                var textField = browser.TextField("name");
		                        var _originalcolor = textField.Style.BackgroundColor;

		                        textField.Highlight(true);
		                        Assert.That(textField.Style.BackgroundColor, Is.EqualTo("red"), "Unexpected background after Highlight(true)");

		                        // Invoke highlighting done by WatiN when typing text
		                        Settings.HighLightColor = "yellow";
		                        textField.TypeText("abc");

		                        Assert.That(textField.Style.BackgroundColor, Is.EqualTo("red"), "Unexpected background after TypeText");
		
		                        textField.Highlight(false);
		                        Assert.That(textField.Style.BackgroundColor, Is.EqualTo(_originalcolor), "Unexpected background Highlight(false)");
		                    });
		}

		[Test]
		public void HighlightShouldNotThrowExceptionWhenCalledToManyTimesWithParamFalse()
		{
		    ExecuteTest(browser =>
		                    {
		                        Settings.HighLightElement = true;
		                        Settings.HighLightColor = "red";

		                        var textField = browser.TextField("name");
		                        var _originalcolor = textField.Style.BackgroundColor;

		                        textField.Highlight(true);
		                        Assert.That(textField.Style.BackgroundColor, Is.EqualTo("red"), "Unexpected background after Highlight(true)");
		
		                        textField.Highlight(false);
		                        Assert.That(textField.Style.BackgroundColor, Is.EqualTo(_originalcolor), "Unexpected background Highlight(false)");

		                        textField.Highlight(false);
		                        Assert.That(textField.Style.BackgroundColor, Is.EqualTo(_originalcolor), "Unexpected background Highlight(false)");
		                    });
		}

		[Test, Ignore("NEED TO BE FIXED: Fails due to wrong mocking")]
		public void ElementNotFoundExceptionShouldHaveInnerExceptionIfTheTimeOutExceptionHadOne()
		{
			Settings.WaitUntilExistsTimeOut = 1;
			
			var elementFinderMock = new Mock<ElementFinder>();
			elementFinderMock.Expect(finder => finder.FindFirst()).Throws(new Exception("My innerexception"));
			
			elementFinderMock.Expect(finder => finder.ElementTagsToString()).Returns("button");
            elementFinderMock.Expect(finder => finder.ConstraintToString()).Returns("id=something");

            
            var domContainerMock = new Mock<DomContainer>(new object[] { });
		    var nativeDocumentMock = new Mock<INativeDocument>();
            nativeDocumentMock.Expect(doc => doc.Url).Returns("http://mocked.com");
            domContainerMock.Expect(container => container.NativeDocument).Returns(nativeDocumentMock.Object);
			element = new Element(domContainerMock.Object, elementFinderMock.Object);

			try
			{
				// kick off the elementFinder
				var nativeElement = element.NativeElement;
				Assert.Fail("ElementNotFoundException should be thrown");
			}
			catch(ElementNotFoundException e)
			{
				Assert.That(e.InnerException != null, "Expected an innerexception");
				Assert.That(e.Message, Text.EndsWith("(inner exception: My innerexception)"));
			}

            elementFinderMock.VerifyAll();
            nativeDocumentMock.VerifyAll();
            domContainerMock.VerifyAll();
		}

        [Test, Ignore("NEED TO BE FIXED: Fails due to wrong mocking")]
        public void ElementNotFoundExceptionShouldHaveNoInnerExceptionIfTheTimeOutExceptionHadNone()
		{
			Settings.WaitUntilExistsTimeOut = 1;
			
			var elementFinderMock = new Mock<ElementFinder>();
			elementFinderMock.Expect(finder => finder.FindFirst()).Returns((Element)null);

            elementFinderMock.Expect(finder => finder.ElementTagsToString()).Returns("button");
            elementFinderMock.Expect(finder => finder.ConstraintToString()).Returns("id=something");

            var domContainerMock = new Mock<DomContainer>( new object[] { });
			element = new Element(domContainerMock.Object, elementFinderMock.Object);

		    var nativeDocumentMock = new Mock<INativeDocument>();
		    domContainerMock.Expect(container => container.NativeDocument).Returns(nativeDocumentMock.Object);
            nativeDocumentMock.Expect(doc => doc.Url).Returns("http://mock.value.com");

			try
			{
				// kick off the elementFinder
				var nativeElement = element.NativeElement;
				Assert.Fail("ElementNotFoundException should be thrown");
			}
			catch(ElementNotFoundException e)
			{
				Assert.That(e.InnerException == null, "Expected an innerexception");
				Assert.That(e.Message, Text.DoesNotEndWith("(inner exception: My innerexception)"));
			}

            elementFinderMock.VerifyAll();
            nativeDocumentMock.VerifyAll();
            domContainerMock.VerifyAll();
		}

		// TODO: Make this test work when HtmlDialog support is added for FireFox
        [Test]
        public void Bug_1932065_FireEventNoWait_hangs_when_ModalWindow_opened()
        {
                Ie.GoTo(PopUpURI);
                Ie.ShowWindow(NativeMethods.WindowShowStyle.ShowNormal);
                Ie.Button(Find.ById("modalid")).FireEventNoWait("onclick");
                Ie.HtmlDialog(Find.First()).Close();

                	//            ExecuteTest(browser =>
//                            {
//                                browser.GoTo(PopUpURI);
//                                browser.ShowWindow(NativeMethods.WindowShowStyle.ShowNormal);
//                                browser.Button(Find.ById("modalid")).FireEventNoWait("onclick");
//                                browser.htmdi
//                            });
        }

        [Test]
        public void AncestorGenericType()
        {
            var nativeElementMock = new Mock<INativeElement>();
            var firstParentDivMock = new Mock<INativeElement>();
            var secondParentDivMock = new Mock<INativeElement>();
            var domContainerMock = new Mock<DomContainer>(new object[] { });

            element = new Element(domContainerMock.Object, nativeElementMock.Object);
            nativeElementMock.Expect(native => native.Parent).Returns(firstParentDivMock.Object);
            
            firstParentDivMock.Expect(first => first.TagName).Returns("a");
            firstParentDivMock.Expect(first => first.Parent).Returns(secondParentDivMock.Object);

            secondParentDivMock.Expect(second => second.TagName).Returns("div");

            Assert.That(element.Ancestor<Div>(), Is.Not.Null);

        	nativeElementMock.VerifyAll();
            firstParentDivMock.VerifyAll();
            secondParentDivMock.VerifyAll();
            domContainerMock.VerifyAll();
        }

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
        public void AncestorGenericTypeAndAttributeConstraintShouldReturnTypedElement()
        {
            Ie.GoTo(TablesUri);
            var tableRow = Ie.TableRow(Find.ById("2"));
            Element ancestor = tableRow.Ancestor<Table>(Find.ById("Table1"));
          
            Assert.IsInstanceOfType (typeof (Table), ancestor);
            Assert.That(ancestor.Id, Is.EqualTo("Table1"));
        }

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
        public void AncestorGenericTypeAndPredicateShouldReturnTypedElement()
        {
            Ie.GoTo(TablesUri);
            var tableRow = Ie.TableRow(Find.ById("2"));
            Element ancestor = tableRow.Ancestor<Table>(table => table.Id == "Table1");
          
            Assert.IsInstanceOfType (typeof (Table), ancestor);
            Assert.That(ancestor.Id, Is.EqualTo("Table1"));
        }

        // TODO: This should be mocked cause there is no browser logic involved
        [Test]
        public void TableOfElementE()
        {
            // Invoke with anonymous delegate
            Element table = Ie.Table("table1");
            table.WaitUntil((Table table1) => table1.Enabled);

            // Invoke with lambda
            ElementContainer<Table> table2 = Ie.Table("table1");
            table2.WaitUntil(t => t.Enabled);

            // Invoke with delegate method
            var table3 = Ie.Table("table1");
            table3.WaitUntil(IsEnabled);
        }

        private static bool IsEnabled(Table table)
        {
            return table.Enabled;
        }

	    [Test]
	    public void ShouldBeEnabled()
	    {
	        ExecuteTest(browser =>
	                        {
	                            // GIVEN
	                            browser.GoTo(MainURI);
	                            Assert.AreEqual(MainURI, new Uri(browser.Url));

	                            // WHEN
	                            var enabled = browser.Link(Find.ByUrl(IndexURI)).Enabled;

	                            // THEN
	                            Assert.That(enabled, Is.True);
	                        });
	    }

	    [Test]
	    public void ShouldReturnTextofEventValue()
	    {
	        ExecuteTest(browser =>
	                        {
	                            // GIVEN
	                            browser.GoTo(MainURI);

	                            // WHEN
                                var onClickText = browser.Button("modalid").GetAttributeValue("onclick");

	                            // THEN
                                Assert.That(onClickText, Is.EqualTo("fnOpen()"), "Unexpected text for onclick event");
	                        });
	    }

        [Test]
        public void StyleAttributeShouldReturnAsString()
        {
            // GIVEN
            const string cssText = "COLOR: white; FONT-STYLE: italic";

            var mockNativeElement = new Mock<INativeElement>();
            var mockDomContainer = new Mock<DomContainer>();
            mockNativeElement.Expect(x => x.GetStyleAttributeValue("cssText")).Returns(cssText);

            var element = new Element(mockDomContainer.Object, mockNativeElement.Object);

            // WHEN
            var value = element.GetAttributeValue("style");

            // THEN
            Assert.That(value, Is.EqualTo(cssText));
        }

        [Test]
        public void StyleDotStyleAttributeNameShouldReturnStyleAttribute()
        {
            // GIVEN
            const string styleAttributeValue = "white";

            var mockNativeElement = new Mock<INativeElement>();
            var mockDomContainer = new Mock<DomContainer>();
            mockNativeElement.Expect(x => x.GetStyleAttributeValue("color")).Returns(styleAttributeValue);

            var element = new Element(mockDomContainer.Object, mockNativeElement.Object);

            // WHEN
            var value = element.GetAttributeValue("style.color");

            // THEN
            Assert.AreEqual(styleAttributeValue, value);
        }

	    [Test]
	    public void ShouldBeEqual()
	    {
	        ExecuteTest(browser =>
	                        {
	                            // GIVEN
	                            var form = browser.Form("FormInputElement");
	                            
                                // WHEN
                                var areEqual = form.Equals(form);

	                            // THEN
	                            Assert.That(areEqual, Is.True, "Should be equal to its self");
	                        });
	    }

	    [Test]
	    public void ShouldBeEqualAlthoughDifferentInstance()
	    {
	        ExecuteTest(browser =>
	                        {
	                            // GIVEN
	                            var form = browser.Form("FormInputElement");
	                            var parentForm = browser.TextField("Textarea1").Parent;
	                            
                                // WHEN
	                            var areEqual = form.Equals(parentForm);

	                            // THEN
	                            Assert.That(areEqual, Is.True, "Should reference same (html) element");
	                        });
	    }

	    [Test]
	    public void ShouldNotBeEqual()
	    {
	        ExecuteTest(browser =>
	                        {
	                            // GIVEN
	                            var form = browser.Form("FormInputElement");
	                            var textField = browser.TextField("Textarea1");
	                            
                                // WHEN
                                var areEqual = form.Equals(textField);

	                            // THEN
	                            Assert.That(areEqual, Is.False, "Should not reference same (html) element");
	                        });
	    }

	    [Test]
	    public void ShouldNotBeEqualWithNull()
	    {
	        ExecuteTest(browser =>
	                        {
	                            // GIVEN
	                            var form = browser.Form("FormInputElement");
	                            
                                // WHEN
                                var areEqual = form.Equals(null);

	                            // THEN
	                            Assert.That(areEqual, Is.False, "Should not be equal with null");
	                        });
	    }

        [Test]
        public void ToStringWhenDescriptionIsNotSetShouldDescribeElement()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
                var form = browser.Form("FormInputElement");

                // WHEN
                var description = form.Description;
                var toString = form.ToString();

                // THEN
                Assert.That(description, Is.Null);
                Assert.That(toString, Is.EqualTo("FormInputElement"));
            });
        }

        [Test]
        public void ToStringWhenDescriptionIsSetShouldReturnDescription()
        {
            ExecuteTestWithAnyBrowser(browser =>
            {
                // GIVEN
#if !NET20
                var form = browser.Form("FormInputElement").WithDescription("foo");
#else
                var form = browser.Form("FormInputElement");
                form.Description = "foo";
#endif

                // WHEN
                var description = form.Description;
                var toString = form.ToString();

                // THEN
                Assert.That(description, Is.EqualTo("foo"));
                Assert.That(toString, Is.EqualTo("foo"));
            });
        }

        [Test]
        public void ShouldRenderInnerTextCorrectlyWhenTagContainsSubTags()
        {
            ExecuteTest(browser =>
            {
                var link = browser.Link("testlinkid3");
                Assert.That(link.Text, Is.EqualTo("Escaped url test"));
            });
        }

	}
}
