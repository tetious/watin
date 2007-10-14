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

namespace WatiN.Core.UnitTests
{
  using System;
  using System.Collections;
  using System.Text.RegularExpressions;
  using mshtml;
  using NUnit.Framework;
  using Rhino.Mocks;
  using WatiN.Core.Comparers;
  using WatiN.Core.Interfaces;

  [TestFixture]
  public class ElementTests : BaseElementsTests
  {
    private MockRepository mocks;
    private IHTMLDOMNode node;
    private Element element;

    [SetUp]
    public void Setup()
    {
      mocks = new MockRepository();
      node = (IHTMLDOMNode) mocks.CreateMock(typeof (IHTMLDOMNode));

      element = new Element(null, node);
    }

    [TearDown]
    public void TearDown()
    {
    }

    [Test]
    public void AncestorTypeShouldReturnTypedElement()
    {
      IHTMLDOMNode parentNode1 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      IHTMLDOMNode parentNode2 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.parentNode).Return(parentNode1);
      Expect.Call(((IHTMLElement) parentNode1).tagName).Return("table").Repeat.Any();

      Expect.Call(parentNode1.parentNode).Return(parentNode2);
      Expect.Call(((IHTMLElement) parentNode2).tagName).Return("div").Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof (Div), element.Ancestor(typeof (Div)));

      mocks.VerifyAll();
    }

    [Test]
    public void AncestorTagNameShouldReturnTypedElement()
    {
      IHTMLDOMNode parentNode1 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      IHTMLDOMNode parentNode2 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.parentNode).Return(parentNode1);
      IHTMLElement tableElement = (IHTMLElement) parentNode1;
      Expect.Call(tableElement.tagName).Return("table").Repeat.Any();
      Expect.Call(tableElement.getAttribute("tagname", 0)).Return("table");

      Expect.Call(parentNode1.parentNode).Return(parentNode2);
      IHTMLElement divElement = (IHTMLElement) parentNode2;
      Expect.Call(divElement.tagName).Return("div").Repeat.Any();
      Expect.Call(divElement.getAttribute("tagname", 0)).Return("div");

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof (Div), element.Ancestor("Div"));

      mocks.VerifyAll();
    }

    [Test]
    public void AncestorAttributeConstraintShouldReturnTypedElement()
    {
      IHTMLDOMNode parentNode1 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      IHTMLDOMNode parentNode2 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.parentNode).Return(parentNode1);
      IHTMLElement tableElement = (IHTMLElement) parentNode1;
      Expect.Call(tableElement.tagName).Return("table").Repeat.Any();
      Expect.Call(tableElement.getAttribute("innertext", 0)).Return("not ok");

      Expect.Call(parentNode1.parentNode).Return(parentNode2);
      IHTMLElement divElement = (IHTMLElement) parentNode2;
      Expect.Call(divElement.tagName).Return("div").Repeat.Any();
      Expect.Call(divElement.getAttribute("innertext", 0)).Return("ancestor");

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof (Div), element.Ancestor(Find.ByText("ancestor")));

      mocks.VerifyAll();
    }

    [Test]
    public void AncestorTypeAndAttributeConstraintShouldReturnTypedElement()
    {
      IHTMLDOMNode parentNode1 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      IHTMLDOMNode parentNode2 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.parentNode).Return(parentNode1);
      IHTMLElement divElement1 = (IHTMLElement) parentNode1;
      Expect.Call(divElement1.tagName).Return("div").Repeat.Any();
      Expect.Call(divElement1.getAttribute("innertext", 0)).Return("first ancestor");

      Expect.Call(parentNode1.parentNode).Return(parentNode2);
      IHTMLElement divElement2 = (IHTMLElement) parentNode2;
      Expect.Call(divElement2.tagName).Return("div").Repeat.Any();
      Expect.Call(divElement2.getAttribute("innertext", 0)).Return("second ancestor");
      Expect.Call(divElement2.innerText).Return("second ancestor");

      mocks.ReplayAll();

      Element ancestor = element.Ancestor(typeof (Div), Find.ByText("second ancestor"));

      Assert.IsInstanceOfType(typeof (Div), ancestor);
      Assert.That(ancestor.Text, NUnit.Framework.SyntaxHelpers.Is.EqualTo("second ancestor"));

      mocks.VerifyAll();
    }

    [Test]
    public void AncestorTagNameAndAttributeConstraintShouldReturnTypedElement()
    {
      IHTMLDOMNode parentNode1 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      IHTMLDOMNode parentNode2 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.parentNode).Return(parentNode1);
      IHTMLElement divElement1 = (IHTMLElement) parentNode1;
      Expect.Call(divElement1.tagName).Return("div").Repeat.Any();
      Expect.Call(divElement1.getAttribute("tagname", 0)).Return("div");
      Expect.Call(divElement1.getAttribute("innertext", 0)).Return("first ancestor");

      Expect.Call(parentNode1.parentNode).Return(parentNode2);
      IHTMLElement divElement2 = (IHTMLElement) parentNode2;
      Expect.Call(divElement2.tagName).Return("div").Repeat.Any();
      Expect.Call(divElement2.getAttribute("tagname", 0)).Return("div");
      Expect.Call(divElement2.getAttribute("innertext", 0)).Return("second ancestor");
      Expect.Call(divElement2.innerText).Return("second ancestor");

      mocks.ReplayAll();

      Element ancestor = element.Ancestor("Div", Find.ByText("second ancestor"));

      Assert.IsInstanceOfType(typeof (Div), ancestor);
      Assert.That(ancestor.Text, NUnit.Framework.SyntaxHelpers.Is.EqualTo("second ancestor"));

      mocks.VerifyAll();
    }

    [Test]
    public void ElementParentShouldReturnNullWhenRootElement()
    {
      Expect.Call(node.parentNode).Return(null);
      mocks.ReplayAll();

      Assert.IsNull(element.Parent);

      mocks.VerifyAll();
    }

    [Test]
    public void ElementParentReturningTypedParent()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      string tagname = ((ElementTag) TableRow.ElementTags[0]).TagName;

      Expect.Call(node.parentNode).Return(parentNode);
      Expect.Call(((IHTMLElement) parentNode).tagName).Return(tagname).Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof (TableRow), element.Parent);

      mocks.VerifyAll();
    }

    [Test]
    public void ElementParentReturnsElementsContainerForUnknowElement()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.parentNode).Return(parentNode);
      Expect.Call(((IHTMLElement) parentNode).tagName).Return("notag").Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof (ElementsContainer), element.Parent);

      mocks.VerifyAll();
    }

    [Test]
    public void ElementPreviousSiblingShouldReturnNullWhenFirstSibling()
    {
      Expect.Call(node.previousSibling).Return(null);
      mocks.ReplayAll();

      Assert.IsNull(element.PreviousSibling);

      mocks.VerifyAll();
    }

    [Test]
    public void ElementPreviousSiblingReturningTypedParent()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      string tagname = ((ElementTag) Link.ElementTags[0]).TagName;

      Expect.Call(node.previousSibling).Return(parentNode);
      Expect.Call(((IHTMLElement) parentNode).tagName).Return(tagname).Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof (Link), element.PreviousSibling);

      mocks.VerifyAll();
    }

    [Test]
    public void ElementPreviousSiblingReturnsElementsContainerForUnknowElement()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.previousSibling).Return(parentNode);
      Expect.Call(((IHTMLElement) parentNode).tagName).Return("notag").Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof (ElementsContainer), element.PreviousSibling);

      mocks.VerifyAll();
    }

    [Test]
    public void ElementNextSiblingShouldReturnNullWhenLastSibling()
    {
      Expect.Call(node.nextSibling).Return(null);
      mocks.ReplayAll();

      Assert.IsNull(element.NextSibling);

      mocks.VerifyAll();
    }

    [Test]
    public void ElementNextSiblingReturningTypedParent()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement), typeof (IHTMLInputElement));

      Expect.Call(node.nextSibling).Return(parentNode);
      Expect.Call(((IHTMLElement) parentNode).tagName).Return("input").Repeat.Any();
      Expect.Call(((IHTMLInputElement) parentNode).type).Return("text").Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof (TextField), element.NextSibling);

      mocks.VerifyAll();
    }

    [Test]
    public void ElementNextSiblingReturnsElementsContainerForUnknowElement()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.nextSibling).Return(parentNode);
      Expect.Call(((IHTMLElement) parentNode).tagName).Return("notag").Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof (ElementsContainer), element.NextSibling);

      mocks.VerifyAll();
    }

    [Test]
    public void ElementRefresh()
    {
      IElementCollection elementCollection = (IElementCollection) mocks.CreateMock(typeof (IElementCollection));
      ElementFinder finder = (ElementFinder) mocks.CreateMock(typeof (ElementFinder), new ArrayList(), null, elementCollection);
      IHTMLElement ihtmlElement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));

      Expect.Call(finder.FindFirst()).Return(ihtmlElement).Repeat.Twice();
      SetupResult.For(ihtmlElement.tagName).Return("mockedtag");

      mocks.ReplayAll();

      Element element = new Element(null, finder);

      Assert.AreEqual("mockedtag", element.TagName);

      element.Refresh();

      Assert.AreEqual("mockedtag", element.TagName);

      mocks.VerifyAll();
    }

    [Test, ExpectedException(typeof (ArgumentException))]
    public void AncestorTypeShouldOnlyExceptTypesInheritingElement()
    {
      mocks.ReplayAll();

      element.Ancestor(typeof (String));

      mocks.VerifyAll();
    }

    [Test]
    public void Element()
    {
      Element element = ie.Element(Find.ById("table1"));

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
      Element element = ie.Element("input", Find.By("id", "name"), "text");
      Assert.IsTrue(element.Exists);
    }

    [Test]
    public void ElementByTagName()
    {
      Element element = ie.Element("a", Find.By("id", "testlinkid"));
      Assert.IsTrue(element.Exists);
    }

    [Test]
    public void FindHeadElementByTagName()
    {
      Element element = ie.Element("head", Find.ByIndex(0));
      Assert.IsTrue(element.Exists);
    }

    [Test]
    public void ElementFindByShouldNeverThrowInvalidAttributeException()
    {
      Element element = ie.Element(Find.ByFor("Checkbox21"));
      Assert.IsTrue(element.Exists);
    }

    [Test]
    public void ElementCollectionExistsShouldNeverThrowInvalidAttributeException()
    {
      Assert.IsTrue(ie.Elements.Exists(Find.ByFor("Checkbox21")));
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
      const int indexTextFieldToRemove = 6;

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

    [Test, ExpectedException(typeof (WatiN.Core.Exceptions.TimeoutException), ExpectedMessage = "Timeout while 'waiting 1 seconds for element to show up.'")]
    public void WaitUntilElementExistsTimeOutException()
    {
      ie.Button("nonexistingbutton").WaitUntilExists(1);
    }

    [Test]
    public void WaitUntil()
    {
      MockRepository mocks = new MockRepository();

      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));

      SetupResult.For(htmlelement.sourceIndex).Return(1);
      SetupResult.For(htmlelement.offsetParent).Return(htmlelement);

      Expect.Call(htmlelement.getAttribute("disabled", 0)).Return(true).Repeat.Once();
      Expect.Call(htmlelement.getAttribute("disabled", 0)).Return(false).Repeat.Once();

      mocks.ReplayAll();

      Element element = new Element(null, htmlelement);

      // calls htmlelement.getAttribute twice (ones true and once false is returned)
      element.WaitUntil(new AttributeConstraint("disabled", new BoolComparer(false)), 1);

      mocks.VerifyAll();
    }

    [Test]
    public void WaitUntilShouldCallExistsToForceRefreshOfHtmlElement()
    {
      MockRepository mocks = new MockRepository();

      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));

      SetupResult.For(htmlelement.getAttribute("disabled", 0)).Return(false);

      Element element = (Element) mocks.DynamicMock(typeof (Element), null, htmlelement);

      Expect.Call(element.Exists).Return(true);

      mocks.ReplayAll();

      element.WaitUntil(new AttributeConstraint("disabled", new BoolComparer(false)), 1);

      mocks.VerifyAll();
    }

    [Test]
    public void WaitUntilExistsShouldIgnoreExceptionsDuringWait()
    {
      MockRepository mocks = new MockRepository();

      IElementCollection elementCollection = (IElementCollection) mocks.CreateMock(typeof (IElementCollection));
      ElementFinder finder = (ElementFinder) mocks.CreateMock(typeof (ElementFinder), null, elementCollection);
      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));

      Expect.Call(finder.FindFirst()).Return(null).Repeat.Times(5);
      Expect.Call(finder.FindFirst()).Throw(new UnauthorizedAccessException("")).Repeat.Times(4);
      Expect.Call(finder.FindFirst()).Return(htmlelement).Repeat.Once();

      Expect.Call(htmlelement.innerText).Return("succeeded");

      mocks.ReplayAll();

      Element element = new Element(null, finder);

      Assert.AreEqual("succeeded", element.Text);

      mocks.VerifyAll();
    }

    [Test]
    public void WaitUntilExistsTimeOutExceptionInnerExceptionNotSetToLastExceptionThrown()
    {
      MockRepository mocks = new MockRepository();

      IElementCollection elementCollection = (IElementCollection) mocks.CreateMock(typeof (IElementCollection));
      ElementFinder finder = (ElementFinder) mocks.CreateMock(typeof (ElementFinder), null, elementCollection);

      Expect.Call(finder.FindFirst()).Throw(new UnauthorizedAccessException(""));
      Expect.Call(finder.FindFirst()).Return(null).Repeat.AtLeastOnce();

      mocks.ReplayAll();

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

      mocks.VerifyAll();
    }

    [Test]
    public void WaitUntilExistsTimeOutExceptionInnerExceptionSetToLastExceptionThrown()
    {
      MockRepository mocks = new MockRepository();

      IElementCollection elementCollection = (IElementCollection) mocks.CreateMock(typeof (IElementCollection));
      ElementFinder finder = (ElementFinder) mocks.DynamicMock(typeof (ElementFinder), null, elementCollection);

      Expect.Call(finder.FindFirst()).Throw(new Exception(""));
      Expect.Call(finder.FindFirst()).Throw(new UnauthorizedAccessException("mockUnauthorizedAccessException")).Repeat.AtLeastOnce();

      mocks.ReplayAll();

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
      Assert.IsInstanceOfType(typeof (UnauthorizedAccessException), timeoutException.InnerException, "Unexpected innerexception");
      Assert.AreEqual("mockUnauthorizedAccessException", timeoutException.InnerException.Message);

      mocks.VerifyAll();
    }

    [Test]
    public void WaitUntilExistsShouldReturnImmediatelyIfElementIsSet()
    {
      MockRepository mocks = new MockRepository();

      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));
      Element mockElement = (Element) mocks.DynamicMock(typeof (Element), null, htmlelement);

      Expect.Call(mockElement.Exists).Repeat.Never();

      mocks.ReplayAll();

      mockElement.WaitUntilExists(3);

      mocks.VerifyAll();
    }

    [Test, ExpectedException(typeof (WatiN.Core.Exceptions.TimeoutException), ExpectedMessage = "Timeout while 'waiting 1 seconds for element attribute 'disabled' to change to 'False'.'")]
    public void WaitUntilTimesOut()
    {
      MockRepository mocks = new MockRepository();

      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));

      Expect.Call(htmlelement.getAttribute("disabled", 0)).Return(true).Repeat.AtLeastOnce();

      mocks.ReplayAll();

      Element element = new Element(null, htmlelement);

      Assert.AreEqual(true.ToString(), element.GetAttributeValue("disabled"));

      // calls htmlelement.getAttribute twice (ones true and once false is returned)
      element.WaitUntil(new AttributeConstraint("disabled", false.ToString()), 1);

      mocks.VerifyAll();
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
      MockRepository mocks = new MockRepository();

      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));

      Expect.Call(htmlelement.getAttribute("sourceIndex", 0)).Return(13);

      mocks.ReplayAll();

      Element element = new Element(null, htmlelement);

      Assert.AreEqual("13", element.GetAttributeValue("sourceIndex"));

      mocks.VerifyAll();
    }

    [Test]
    public void FireKeyDownEventOnElementWithNoId()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        TextField report = ie.TextField("Report");
        Core.Button button = ie.Button(Find.ByValue("Button without id"));

        Assert.IsNull(button.Id, "Button id not null before click event");
        Assert.IsNull(report.Text, "Report not empty");

        button.KeyDown();

        Assert.IsNotNull(report.Text, "No keydown event fired (report is empty )");
        Assert.AreEqual("button.id = ", report.Text, "Report should start with 'button.id = '");

        Assert.IsNull(button.Id, "Button id not null after click event");
      }
    }

    [Test]
    public void FireEventAlwaysSetsLeftMouseOnEventObject()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        // test in standard IE window
        ie.Button(Find.ByValue("Button without id")).KeyDown();

        Assert.AreEqual("1", ie.TextField("eventButtonValue").Value, "Event.button not left");

        // test in HTMLDialog window
        ie.Button("modalid").ClickNoWait();

        using (HtmlDialog htmlDialog = ie.HtmlDialogs[0])
        {
          htmlDialog.Button(Find.ByValue("Button without id")).KeyDown();

          Assert.AreEqual("1", htmlDialog.TextField("eventButtonValue").Value, "Event.button not left on modal dialog");
        }
      }
    }

    [Test]
    public void FireEventAlwaysSetsSrcElementOnEventObject()
    {
      using (IE ie = new IE(TestEventsURI))
      {
        // test in standard IE window
        ie.Button(Find.ByValue("Button without id")).KeyDown();

        Assert.AreEqual("Button without id", ie.TextField("eventScrElement").Value, "Unexpected Event.scrElement.value");

        // test in HTMLDialog window
        ie.Button("modalid").ClickNoWait();

        using (HtmlDialog htmlDialog = ie.HtmlDialogs[0])
        {
          htmlDialog.Button(Find.ByValue("Button without id")).KeyDown();

          Assert.AreEqual("Button without id", htmlDialog.TextField("eventScrElement").Value, "Unexpected Event.scrElement.value");
        }
      }
    }

#if NET20
    [Test]
    public void AncestorGenericType()
    {
      IHTMLDOMNode parentNode1 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      IHTMLDOMNode parentNode2 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.parentNode).Return(parentNode1);
      Expect.Call(((IHTMLElement) parentNode1).tagName).Return("table").Repeat.Any();
      
      Expect.Call(parentNode1.parentNode).Return(parentNode2);
      Expect.Call(((IHTMLElement) parentNode2).tagName).Return("div").Repeat.Any();

      mocks.ReplayAll();

      Assert.That(element.Ancestor<Div>(), NUnit.Framework.SyntaxHelpers.Is.Not.Null);
    	
    }

    [Test]
    public void AncestorGenericTypeAndAttributeConstraintShouldReturnTypedElement()
    {
      IHTMLDOMNode parentNode1 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      IHTMLDOMNode parentNode2 = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.parentNode).Return(parentNode1);
      IHTMLElement divElement1 = (IHTMLElement) parentNode1;
      Expect.Call(divElement1.tagName).Return("div").Repeat.Any();
      Expect.Call(divElement1.getAttribute("innertext",0)).Return("first ancestor");
      
      Expect.Call(parentNode1.parentNode).Return(parentNode2);
      IHTMLElement divElement2 = (IHTMLElement) parentNode2;
      Expect.Call(divElement2.tagName).Return("div").Repeat.Any();
      Expect.Call(divElement2.getAttribute("innertext",0)).Return("second ancestor");
      Expect.Call(divElement2.innerText).Return("second ancestor");
      
      mocks.ReplayAll();

      Element ancestor = element.Ancestor<Div>(Find.ByText("second ancestor"));
      
      Assert.IsInstanceOfType(typeof (Div), ancestor);
      Assert.That(ancestor.Text, NUnit.Framework.SyntaxHelpers.Is.EqualTo("second ancestor"));
    }

#endif
  }
}