#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright

using System;
using System.Threading;

using mshtml;

using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.Core
{
  /// <summary>
  /// This is the base class for all other element types in this project, like
  /// Button, Checkbox etc.. It provides common functionality to all these elements
  /// </summary>
  public class Element
  {
    private DomContainer domContainer;
    private object element;
    
    private string originalcolor;

    /// <summary>
    /// This constructor is mainly used from within WatiN.
    /// </summary>
    /// <param name="domContainer"><see cref="DomContainer" /> this element is located in</param>
    /// <param name="element">The element</param>
    public Element(DomContainer domContainer, object element)
    {
      this.domContainer = domContainer;
      DomElement = element;
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
    /// Gets the text of this element (and the text of all the elements contained
    /// in this element).
    /// </summary>
    /// <value>The text.</value>
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
      get { return new Element(domContainer, domNode.nextSibling); }
    }
    
    /// <summary>
    /// Gets the previous sibling of this element in the Dom of the HTML page.
    /// </summary>
    /// <value>The previous sibling.</value>
    public Element PreviousSibling
    {
      get { return new Element(domContainer, domNode.previousSibling); }
    }

    /// <summary>
    /// Gets the parent element of this element.
    /// </summary>
    /// <value>The parent.</value>
    public Element Parent
    {
      get { return new Element(domContainer, domNode.parentNode); }
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
      object attribute = htmlElement.getAttribute(attributeName, 0);

      if (attribute == DBNull.Value)
      {
        return null;
      }

      return (string) attribute;
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
    /// Fires the specified <paramref name="eventName"/> on this element.
    /// </summary>
    public void FireEvent(string eventName)
    {
      if (!Enabled) { throw new ElementDisabledException(Id); }

      Highlight(true);
      domContainer.FireEvent(DispHtmlBaseElement, eventName);
      WaitForComplete();
      Highlight(false);
    }

    /// <summary>
    /// Flashes this element.
    /// </summary>
    public void Flash()
    {
      for (int counter = 0; counter < 5; counter++)
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
      if (doHighlight)
      {
        try
        {
          originalcolor = (string)htmlElement.style.backgroundColor;
          htmlElement.style.backgroundColor = "yellow";
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
            htmlElement.style.backgroundColor = "transparent";
          }
        }

        catch {}
        finally
        {
          originalcolor = null;
        }
      }
    }

    private IHTMLElement htmlElement
    {
      get { return (IHTMLElement) DomElement; }
    }

    private IHTMLElement2 htmlElement2
    {
      get { return (IHTMLElement2) DomElement; }
    }

    private IHTMLElement3 htmlElement3
    {
      get { return (IHTMLElement3) DomElement; }
    }

    private IHTMLDOMNode domNode
    {
      get { return (IHTMLDOMNode) DomElement; }
    }

    /// <summary>
    /// Gets the DispHtmlBaseElement by casting <see cref="DomElement" />.
    /// </summary>
    /// <value>The DispHtmlBaseElement.</value>
    protected DispHTMLBaseElement DispHtmlBaseElement
    {
      get { return (DispHTMLBaseElement) DomElement; }
    }

    /// <summary>
    /// Gets the DOM container for this element.
    /// </summary>
    /// <value>The DOM container.</value>
    protected DomContainer DomContainer
    {
      get { return domContainer; }
    }

    /// <summary>
    /// Gets or sets the DOM element for this instance.
    /// </summary>
    /// <value>The DOM element.</value>
    protected object DomElement
    {
      get { return element; }
      set { element = value; }
    }

    /// <summary>
    /// Waits till the page load is complete. This should only be used on rare occasions
    /// because WatiN calls this function for you when it handles events (like Click etc..)
    /// </summary>
    public void WaitForComplete()
    {
      domContainer.WaitForComplete();
    }
  }
}