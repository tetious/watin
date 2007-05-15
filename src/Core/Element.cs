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
using System.Collections;
using System.Threading;

using mshtml;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.Core
{
  using System.Reflection;
  using System.Text.RegularExpressions;

  /// <summary>
  /// This is the base class for all other element types in this project, like
  /// Button, Checkbox etc.. It provides common functionality to all these elements
  /// </summary>
  public class Element
  {
    private DomContainer domContainer;
    private object element;
    private ElementFinder elementFinder;
    
    private string originalcolor;

    /// <summary>
    /// This constructor is mainly used from within WatiN.
    /// </summary>
    /// <param name="domContainer"><see cref="DomContainer" /> this element is located in</param>
    /// <param name="element">The element</param>
    public Element(DomContainer domContainer, object element)
    {
      init(domContainer, element, null);
    }
    
    /// <summary>
    /// This constructor is mainly used from within WatiN.
    /// </summary>
    /// <param name="domContainer"><see cref="DomContainer"/> this element is located in</param>
    /// <param name="elementFinder">The element finder.</param>
    public Element(DomContainer domContainer, ElementFinder elementFinder)
    {
      init(domContainer, null, elementFinder);
    }
    
    /// <summary>
    /// This constructor is mainly used from within WatiN.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="elementTags">The element tags the element should match with.</param>
    public Element(Element element, ArrayList elementTags)
    {
      if (ElementTag.IsValidElement(element.htmlElement, elementTags))
      {
        init(element.domContainer, element.element, element.elementFinder);
      }
      else
      {
        throw new ArgumentException(String.Format("Expected element {0}", ElementFinder.GetExceptionMessage(elementTags)), "element");
      }
    }

    private void init(DomContainer domContainer, object element, ElementFinder elementFinder)
    {
      this.domContainer = domContainer;
      this.element = element;
      this.elementFinder = elementFinder;
    }

    /// <summary>
    /// Gets the name of the stylesheet class assigned to this ellement (if any).
    /// </summary>
    /// <value>The name of the class.</value>
    public string ClassName
    {
      get { return htmlElement.className; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Element"/> is completely loaded.
    /// </summary>
    /// <value><c>true</c> if complete; otherwise, <c>false</c>.</value>
    public bool Complete
    {
      get { return htmlElement2.readyStateValue == 4; }
    }
    
    /// <summary>
    /// Gets a value indicating whether this <see cref="Element"/> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    public bool Enabled
    {
      get { return !htmlElement3.disabled; }
    }

    /// <summary>
    /// Gets the id of this element as specified in the HTML.
    /// </summary>
    /// <value>The id.</value>
    public string Id
    {
      get { return htmlElement.id; }
    }

    /// <summary>
    /// Gets the innertext of this element (and the innertext of all the elements contained
    /// in this element).
    /// </summary>
    /// <value>The innertext.</value>
    public virtual string Text
    {
      get { return htmlElement.innerText; }
    }

    /// <summary>
    /// Returns the text displayed after this element when it's wrapped
    /// in a Label element; otherwise it returns <c>null</c>.
    /// </summary>
    public string TextAfter
    {
      get { return htmlElement2.getAdjacentText("afterEnd"); }
    }

    /// <summary>
    /// Returns the text displayed before this element when it's wrapped
    /// in a Label element; otherwise it returns <c>null</c>.
    /// </summary>
    public string TextBefore
    {
      get { return htmlElement2.getAdjacentText("beforeBegin"); }
    }

    /// <summary>
    /// Gets the inner HTML of this element.
    /// </summary>
    /// <value>The inner HTML.</value>
    public string InnerHtml
    {
      get { return htmlElement.innerHTML; }
    }

    /// <summary>
    /// Gets the outer text.
    /// </summary>
    /// <value>The outer text.</value>
    public string OuterText
    {
      get { return htmlElement.outerText; }
    }

    /// <summary>
    /// Gets the outer HTML.
    /// </summary>
    /// <value>The outer HTML.</value>
    public string OuterHtml
    {
      get { return htmlElement.outerHTML; }
    }

    /// <summary>
    /// Gets the tag name of this element.
    /// </summary>
    /// <value>The name of the tag.</value>
    public string TagName
    {
      get { return htmlElement.tagName; }
    }

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public string Title
    {
      get { return htmlElement.title; }
    }

    /// <summary>
    /// Gets the next sibling of this element in the Dom of the HTML page.
    /// </summary>
    /// <value>The next sibling.</value>
    public Element NextSibling
    {
      get
      {
        IHTMLElement nextSibling = domNode.nextSibling as IHTMLElement;
        if (nextSibling != null)
        {
          return GetTypedElement(domContainer, nextSibling);
        }
        return null;
      }
    }
    
    /// <summary>
    /// Gets the previous sibling of this element in the Dom of the HTML page.
    /// </summary>
    /// <value>The previous sibling.</value>
    public Element PreviousSibling
    {
      get
      {
        IHTMLElement previousSibling = domNode.previousSibling as IHTMLElement;
        if (previousSibling != null)
        {
          return GetTypedElement(domContainer, previousSibling);
        }
        return null;
      }
    }

    /// <summary>
    /// Gets the parent element of this element.
    /// If the parent type is known you can cast it to that type.
    /// </summary>
    /// <value>The parent.</value>
    /// <example>
    /// The following example shows you how to use the parent property.
    /// Assume your web page contains a bit of html that looks something
    /// like this:
    /// 
    /// div
    ///   a id="watinlink" href="http://watin.sourceforge.net" /
    ///   a href="http://sourceforge.net/projects/watin" /
    /// /div
    /// div
    ///   a id="watinfixturelink" href="http://watinfixture.sourceforge.net" /
    ///   a href="http://sourceforge.net/projects/watinfixture" /
    /// /div
    /// Now you want to click on the second link of the watin project. Using the 
    /// parent property the code to do this would be:
    /// 
    /// <code>
    /// Div watinDiv = (Div) ie.Link("watinlink").Parent;
    /// watinDiv.Links[1].Click();
    /// </code>
    /// </example>
    public Element Parent
    {
      get
      {
        IHTMLElement parentNode = domNode.parentNode as IHTMLElement;
        if (parentNode != null)
        {
          return GetTypedElement(domContainer, parentNode);
        }
        return null;
      }
    }

    public Style Style
    {
      get
      {
        return new Style(htmlElement.style);
      }
    }
    
    /// <summary>
    /// This methode can be used if the attribute isn't available as a property of
    /// Element or a subclass of Element.
    /// </summary>
    /// <param name="attributeName">The attribute name. This could be different then named in
    /// the HTML. It should be the name of the property exposed by IE on it's element object.</param>
    /// <returns>The value of the attribute if available; otherwise <c>null</c> is returned.</returns>
    public string GetAttributeValue(string attributeName)
    {
      if (UtilityClass.IsNullOrEmpty(attributeName))
      {
        throw new ArgumentNullException("attributeName", "Null or Empty not allowed.");
      }
      
      if (string.Compare(attributeName, "style", true) == 0 )
      {
        return htmlElement.style.cssText;
      }
      
      object attribute = htmlElement.getAttribute(attributeName, 0);

      if (attribute == DBNull.Value || attribute == null)
      {
        return null;
      }

      return attribute.ToString();
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      if (UtilityClass.IsNotNullOrEmpty(Title))
      {
        return Title;
      }
      return Text;
    }

    /// <summary>
    /// Clicks this element and waits till the event is completely finished (page is loaded 
    /// and ready) .
    /// </summary>
    public void Click()
    {
      if (!Enabled) { throw new ElementDisabledException(Id); }

      Logger.LogAction("Clicking " + GetType().Name + " '" + ToString() + "'");
      Highlight(true);

      ClickDispHtmlBaseElement();

      try
      {
        WaitForComplete();
      }
      finally
      {
        Highlight(false);
      }
    }

    private void ClickDispHtmlBaseElement()
    {
      DispHtmlBaseElement.click();
    }

    /// <summary>
    /// Clicks this instance and returns immediately. Use this method when you want to continue without waiting
    /// for the click event to be finished. Mostly used when a 
    /// HTMLDialog is displayed after clicking the element.
    /// </summary>
    public void ClickNoWait()
    {
      if (!Enabled) { throw new ElementDisabledException(Id); }

      Logger.LogAction("Clicking (no wait) " + GetType().Name + " '" + ToString() + "'");

      Highlight(true);

      Thread clickButton = new Thread(new ThreadStart(ClickDispHtmlBaseElement));
      clickButton.Start();
      clickButton.Join(500);

      Highlight(false);
    }

    /// <summary>
    /// Gives the (input) focus to this element.
    /// </summary>
    public void Focus()
    {
      if (!Enabled) { throw new ElementDisabledException(Id); }

      DispHtmlBaseElement.focus();
      FireEvent("onFocus");
    }

    /// <summary>
    /// Doubleclicks this element.
    /// </summary>
    public void DoubleClick()
    {
      if (!Enabled) { throw new ElementDisabledException(Id); }

      Logger.LogAction("Double clicking " + GetType().Name + " '" + ToString() + "'");

      FireEvent("onDblClick");
    }

    /// <summary>
    /// Does a keydown on this element.
    /// </summary>
    public void KeyDown()
    {
      FireEvent("onKeyDown");
    }

    /// <summary>
    /// Does a keyspress on this element.
    /// </summary>
    public void KeyPress()
    {
      FireEvent("onKeyPress");
    }

    /// <summary>
    /// Does a keyup on this element.
    /// </summary>
    public void KeyUp()
    {
      FireEvent("onKeyUp");
    }

    /// <summary>
    /// Fires the blur event on this element.
    /// </summary>
    public void Blur()
    {
      FireEvent("onBlur");
    }

    /// <summary>
    /// Fires the change event on this element.
    /// </summary>
    public void Change()
    {
      FireEvent("onChange");
    }

    /// <summary>
    /// Fires the mouseenter event on this element.
    /// </summary>
    public void MouseEnter()
    {
      FireEvent("onMouseEnter");
    }
    
    /// <summary>
    /// Fires the mousedown event on this element.
    /// </summary>
    public void MouseDown()
    {
      FireEvent("onmousedown");
    }

    /// <summary>
    /// Fires the mouseup event on this element.
    /// </summary>
    public void MouseUp()
    {
      FireEvent("onmouseup");
    }

    /// <summary>
    /// Fires the specified <paramref name="eventName"/> on this element
    /// and waits for it to complete.
    /// </summary>
    public void FireEvent(string eventName)
    {
      fireEvent(eventName, true);
    }

    /// <summary>
    /// Only fires the specified <paramref name="eventName"/> on this element.
    /// </summary>
    public void FireEventNoWait(string eventName)
    {
      fireEvent(eventName, false);
    }

    private void fireEvent(string eventName, bool waitForComplete)
    {
      if (!Enabled) { throw new ElementDisabledException(Id); }

      Highlight(true);
      UtilityClass.FireEvent(DispHtmlBaseElement, eventName);
      if (waitForComplete) {WaitForComplete();}
      Highlight(false);
    }

    /// <summary>
    /// Flashes this element 5 times.
    /// </summary>
    public void Flash()
    {
      Flash(5);
    }
    
    /// <summary>
    /// Flashes this element the specified number of flashes.
    /// </summary>
    /// <param name="numberOfFlashes">The number of flashes.</param>
    public void Flash(int numberOfFlashes)
    {
      for (int counter = 0; counter < numberOfFlashes; counter++)
      {
        Highlight(true);
        Thread.Sleep(250);
        Highlight(false);
        Thread.Sleep(250);
      }
    }

    /// <summary>
    /// Highlights this element.
    /// </summary>
    /// <param name="doHighlight">if set to <c>true</c> the element is highlighted; otherwise it's not.</param>
    protected void Highlight(bool doHighlight)
    {
      if (IE.Settings.HighLightElement)
      {
        if (doHighlight)
        {
          try
          {
            originalcolor = (string)htmlElement.style.backgroundColor;
            htmlElement.style.backgroundColor = IE.Settings.HighLightColor;
          }
          catch
          {
            originalcolor = null;
          }
        }
        else
        {
          try
          {
            if (originalcolor != null)
            {
              htmlElement.style.backgroundColor = originalcolor;
            }
            else
            {
              htmlElement.style.backgroundColor = "";
            }
          }
          catch {}
          finally
          {
            originalcolor = null;
          }
        }
      }
    }

    protected IHTMLElement htmlElement
    {
      get { return (IHTMLElement) HTMLElement; }
    }

    private IHTMLElement2 htmlElement2
    {
      get { return (IHTMLElement2) HTMLElement; }
    }

    private IHTMLElement3 htmlElement3
    {
      get { return (IHTMLElement3) HTMLElement; }
    }

    private IHTMLDOMNode domNode
    {
      get { return (IHTMLDOMNode) HTMLElement; }
    }

    /// <summary>
    /// Gets the DispHtmlBaseElement by casting <see cref="HTMLElement" />.
    /// </summary>
    /// <value>The DispHtmlBaseElement.</value>
    protected DispHTMLBaseElement DispHtmlBaseElement
    {
      get { return (DispHTMLBaseElement) HTMLElement; }
    }

    /// <summary>
    /// Gets the DOMcontainer for this element.
    /// </summary>
    /// <value>The DOM container.</value>
    public DomContainer DomContainer
    {
      get { return domContainer; }
    }

    /// <summary>
    /// Gets the DOM HTML element for this instance as an object. Cast it to 
    /// the interface you need. Most of the time the object supports IHTMLELement, 
    /// IHTMLElement2 and IHTMLElement3 but you can also cast it to a more
    /// specific interface. You should reference the microsoft.MSHTML.dll 
    /// assembly to cast it to a valid type.
    /// </summary>
    /// <value>The DOM element.</value>
    public object HTMLElement
    {
      get
      {
        if (!ElementAvailable())
        {
          try
          {
            WaitUntilExists();
          }
          catch(WatiN.Core.Exceptions.TimeoutException)
          {
            throw elementFinder.CreateElementNotFoundException();
          }
        }

        return element;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Element"/> exists.
    /// </summary>
    /// <value><c>true</c> if exists; otherwise, <c>false</c>.</value>
    public virtual bool Exists
    {
      get
      {
        object elementExists = getElement();

        if (elementExists == null)
        {
          return false;
        }

        return true;
      }
    }

    /// <summary>
    /// Waits until the element exists or will time out after 30 seconds.
    /// To change the default time out, set <see cref="P:WatiN.Core.IE.Settings.WaitUntilExistsTimeOut"/>
    /// </summary>
    public void WaitUntilExists()
    {
      // Wait 30 seconds max
      WaitUntilExists(IE.Settings.WaitUntilExistsTimeOut);
    }
    
    /// <summary>
    /// Waits until the element exists. Wait will time out after <paramref name="timeout"/> seconds.
    /// </summary>
    /// <param name="timeout">The timeout in seconds.</param>
    public void WaitUntilExists(int timeout)
    {
      waitUntilExistsOrNot(timeout, true);
    }

    /// <summary>
    /// Waits until the element no longer exists or will time out after 30 seconds.
    /// To change the default time out, set <see cref="P:WatiN.Core.IE.Settings.WaitUntilExistsTimeOut"/>
    /// </summary>
    public void WaitUntilRemoved()
    {
      // Wait 30 seconds max
      WaitUntilRemoved(IE.Settings.WaitUntilExistsTimeOut);
    }
    
    /// <summary>
    /// Waits until the element no longer exists. Wait will time out after <paramref name="timeout"/> seconds.
    /// </summary>
    /// <param name="timeout">The timeout in seconds.</param>
    public void WaitUntilRemoved(int timeout)
    {
      waitUntilExistsOrNot(timeout, false);
    }

    /// <summary>
    /// Waits until the given <paramref name="attributename" /> matches <paramref name="expectedValue" />.
    /// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
    /// </summary>
    /// <param name="attributename">The attributename.</param>
    /// <param name="expectedValue">The expected value.</param>
    public void WaitUntil(string attributename, string expectedValue)
    {
      WaitUntil(attributename, expectedValue, IE.Settings.WaitUntilExistsTimeOut);
    }

    /// <summary>
    /// Waits until the given <paramref name="attributename" /> matches <paramref name="expectedValue" />.
    /// Wait will time out after <paramref name="timeout"/> seconds.
    /// </summary>
    /// <param name="attributename">The attributename.</param>
    /// <param name="expectedValue">The expected value.</param>
    /// <param name="timeout">The timeout.</param>
    public void WaitUntil(string attributename, string expectedValue, int timeout)
    {
      WaitUntil(new Attribute(attributename, expectedValue), timeout);
    }

    /// <summary>
    /// Waits until the given <paramref name="attributename" /> matches <paramref name="expectedValue" />.
    /// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
    /// </summary>
    /// <param name="attributename">The attributename.</param>
    /// <param name="expectedValue">The expected value.</param>
    public void WaitUntil(string attributename, Regex expectedValue)
    {
      WaitUntil(attributename, expectedValue, IE.Settings.WaitUntilExistsTimeOut);
    }

    /// <summary>
    /// Waits until the given <paramref name="attributename" /> matches <paramref name="expectedValue" />.
    /// Wait will time out after <paramref name="timeout"/> seconds.
    /// </summary>
    /// <param name="attributename">The attributename.</param>
    /// <param name="expectedValue">The expected value.</param>
    /// <param name="timeout">The timeout.</param>
    public void WaitUntil(string attributename, Regex expectedValue, int timeout)
    {
      WaitUntil(new Attribute(attributename, expectedValue), timeout);
    }

    /// <summary>
    /// Waits until the given <paramref name="attribute" /> matches.
    /// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    public void WaitUntil(Attribute attribute)
    {
      WaitUntil(attribute, IE.Settings.WaitUntilExistsTimeOut);
    }

    /// <summary>
    /// Waits until the given <paramref name="attribute" /> matches.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <param name="timeout">The timeout.</param>
    public void WaitUntil(Attribute attribute, int timeout)
    {
      Exception lastException;

      ElementAttributeBag attributeBag = new ElementAttributeBag();

      SimpleTimer timeoutTimer = new SimpleTimer(timeout);

      do
      {
        lastException = null;

        try
        {
          // Calling Exists will refresh the reference to the html element
          // so the compare is against the current html element (and not 
          // against some cached reference.
          if (Exists)
          {
            attributeBag.IHTMLElement = htmlElement;
            
            if (attribute.Compare(attributeBag))
            {
              return;
            }
            
          }
        }
        catch (Exception e)
        {
          lastException = e;
        }        

        Thread.Sleep(200);
      } while (!timeoutTimer.Elapsed);

      ThrowTimeOutException(lastException, string.Format("waiting {0} seconds for element attribute '{1}' to change to '{2}'.", timeout, attribute.AttributeName, attribute.Value));
    }

    private static void ThrowTimeOutException(Exception lastException, string message)
    {
      if (lastException != null)
      {
        throw new WatiN.Core.Exceptions.TimeoutException(message, lastException);
      }
      else
      {
        throw new WatiN.Core.Exceptions.TimeoutException(message);
      }
    }

    private void waitUntilExistsOrNot(int timeout, bool waitUntilExists)
    {
      // Does it make sense to go into the do loop?
      if (waitUntilExists)
      {
        if(ElementAvailable()) { return; }
        else if(elementFinder == null)
        {
          throw new WatiNException("It's not possible to find the element because no elementFinder is available.");
        }
      }
      else
      {
        if (!ElementAvailable()) { return; }
      }

      Exception lastException;
      SimpleTimer timeoutTimer = new SimpleTimer(timeout);
      
      do
      {
        lastException = null;
        
        try
        {
          if (Exists == waitUntilExists)
          {
            return;
          }
        }
        catch (Exception e)
        {
          lastException = e;
        }
        
        Thread.Sleep(200);
      } while (!timeoutTimer.Elapsed);

      ThrowTimeOutException(lastException,string.Format("waiting {0} seconds for element to {1}.", timeout, waitUntilExists ? "show up" : "disappear"));    
    }

    /// <summary>
    /// Call this method to make sure the cached reference to the html element on the web page
    /// is refreshed on the next call you make to a property or method of this element.
    /// When you want to check if an element still <see cref="Exists"/> you don't need 
    /// to call the <see cref="Refresh"/> method since <see cref="Exists"/> calls it internally.
    /// </summary>
    /// <example>
    /// The following code shows in which situation you can make use of the refresh mode.
    /// The code selects an option of a selectlist and then checks if this option
    /// is selected.
    /// <code>
    /// SelectList select = ie.SelectList(id);
    /// 
    /// // Lets assume calling Select causes a postback, 
    /// // which would invalidate the reference to the html selectlist.
    /// select.Select(val);
    /// 
    /// // Refresh will clear the cached reference to the html selectlist.
    /// select.Refresh(); 
    /// 
    /// // B.t.w. executing:
    /// // select = ie.SelectList(id); 
    /// // would have the same effect as calling: 
    /// // select.Refresh().
    /// 
    /// // Assert against a freshly fetched reference to the html selectlist.
    /// Assert.AreEqual(val, select.SelectedItem);
    /// </code>
    /// If you need to use refresh on an element retrieved from a collection of elements you 
    /// need to rewrite your code a bit.
    /// <code>
    /// SelectList select = ie.Spans[1].SelectList(id);
    /// select.Refresh(); // this will not have the expected effect
    /// </code>
    /// Rewrite your code as follows:
    /// <code>
    /// SelectList select = ie.Span(Find.ByIndex(1)).SelectList(id);
    /// select.Refresh(); // this will have the expected effect
    /// </code>
    /// </example>
    public void Refresh()
    {
      if (elementFinder != null)
      {
        element = null;
      }
    }

    /// <summary>
    /// Calls <see cref="Refresh"/> and returns the element.
    /// </summary>
    /// <returns></returns>
    protected object getElement()
    {
      if (elementFinder != null)
      {
        element = elementFinder.FindFirst();
      }
      else
      {
        // This will set element to null if some specific properties are
        // a certain value. These values indicate that the element is no longer
        // on the/a valid web page.
        // These checks are only necessary if element field
        // is set during the construction of an ElementCollection
        // or a more specialized element collection (like TextFieldCollection)
        IHTMLElement ihtmlElement = element as IHTMLElement;

        if (ihtmlElement != null)
        {
          try
          {
            if(ihtmlElement.sourceIndex < 0)
            {
              element = null;
            }
            else
            {
              if(ihtmlElement.offsetParent == null)
              {
                element = null;
              }
            }
          }
          catch
          {
            element = null;
          }
        }
      }

      return element;
    }

    private bool ElementAvailable()
    {
      return element != null;
    }

    /// <summary>
    /// Waits till the page load is complete. This should only be used on rare occasions
    /// because WatiN calls this function for you when it handles events (like Click etc..)
    /// To change the default time out, set <see cref="P:WatiN.Core.IE.Settings.WaitForCompleteTimeOut"/>
    /// </summary>
    public void WaitForComplete()
    {
      domContainer.WaitForComplete();
    }

    internal static Element GetTypedElement(DomContainer domContainer, IHTMLElement element)
    {
      Assembly assembly = Assembly.Load("WatiN.Core");

      Element returnElement = new ElementsContainer(domContainer, element);

      foreach (Type type in assembly.GetTypes())
      {
        if (type.IsSubclassOf(typeof(Element)))
        {
          PropertyInfo property = type.GetProperty("ElementTags");
          if (property != null)
          {
            ArrayList elementTags = (ArrayList)property.GetValue(type, null);

            if (ElementTag.IsValidElement(element, elementTags))
            {
              ConstructorInfo constructor = type.GetConstructor(new Type[] {typeof (Element)});
              if (constructor != null)
              {
                returnElement = (Element)constructor.Invoke(new object[] {returnElement});
                break;
              }
            }
          }
        }
      }

      return returnElement;
    }
  }
}
