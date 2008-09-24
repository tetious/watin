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
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading;
using mshtml;
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.InternetExplorer;
using WatiN.Core.Logging;

namespace WatiN.Core
{
#if !NET11

	/// <summary>
	/// This is the base class for all other element types in this project, like
	/// Button, Checkbox etc.. It provides common functionality to all these elements
	/// </summary>
	public class Element<E> : Element where E : Element
	{
	    public Element(DomContainer domContainer, INativeElement nativeElement) : base(domContainer, nativeElement)
	    {}

        [Obsolete("Use the constructor accepting INativeElement instead")]
	    public Element(DomContainer domContainer, object element) : this(domContainer, domContainer.NativeBrowser.CreateElement(element))
	    {}

	    public Element(Element element, ArrayList elementTags) : base(element, elementTags)
	    {}

	    public Element(DomContainer domContainer, INativeElementFinder elementFinder) : base(domContainer, elementFinder)
	    {}

	    /// <summary>
        /// Waits until the given expression is <c>true</c>.
        /// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
        /// </summary>
        /// <param name="predicate">The expression to use.</param>
        public void WaitUntil(Predicate<E> predicate)
        {
            WaitUntil(Find.ByElement(predicate), Settings.WaitUntilExistsTimeOut);
        }

        /// <summary>
		/// Waits until the given expression is <c>true</c>.
		/// </summary>
        /// <param name="predicate">The expression to use.</param>
		/// <param name="timeout">The timeout.</param>
		public void WaitUntil(Predicate<E> predicate, int timeout)
		{
            WaitUntil(Find.ByElement(predicate), timeout);
        }

	}
#endif

    /// <summary>
	/// This is the base class for all other element types in this project, like
	/// Button, Checkbox etc.. It provides common functionality to all these elements
	/// </summary>
	public class Element : IAttributeBag
	{
		private DomContainer _domContainer;
		private INativeElement _nativeElement;
		private INativeElementFinder _nativeElementFinder;

		private Stack _originalcolor;

		/// <summary>
		/// This constructor is mainly used from within WatiN.
		/// </summary>
		/// <param name="domContainer"><see cref="DomContainer" /> this element is located in</param>
		/// <param name="element">The element</param>
        [Obsolete("Use the constructor accepting INativeElement instead")]
		public Element(DomContainer domContainer, object element)
		{
			initElement(domContainer, domContainer.NativeBrowser.CreateElement(element), null);
		}

		/// <summary>
		/// This constructor is mainly used from within WatiN.
		/// </summary>
		/// <param name="domContainer"><see cref="DomContainer" /> this element is located in</param>
		/// <param name="nativeElement">The element</param>
		public Element(DomContainer domContainer, INativeElement nativeElement)
		{
			initElement(domContainer,nativeElement, null);
		}

		/// <summary>
		/// This constructor is mainly used from within WatiN.
		/// </summary>
		/// <param name="domContainer"><see cref="DomContainer"/> this element is located in</param>
		/// <param name="elementFinder">The element finder.</param>
		public Element(DomContainer domContainer, INativeElementFinder elementFinder)
		{
			initElement(domContainer, null, elementFinder);
		}

		/// <summary>
		/// This constructor is mainly used from within WatiN.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="elementTags">The element tags the element should match with.</param>
		public Element(Element element, ArrayList elementTags)
		{
			if (ElementTag.IsValidElement(element.NativeElement, elementTags))
			{
				initElement(element._domContainer, element._nativeElement, element._nativeElementFinder);
			}
			else
			{
				throw new ArgumentException(String.Format("Expected element {0}", ElementTag.ElementTagsToString(elementTags)), "element");
			}
		}

		private void initElement(DomContainer domContainer, INativeElement nativeElement, INativeElementFinder elementFinder) 
		{
            if (domContainer == null) throw new ArgumentNullException("domContainer");

			_domContainer = domContainer;
			_nativeElement = nativeElement;
			_nativeElementFinder = elementFinder;
		}

		/// <summary>
		/// Gets the name of the stylesheet class assigned to this ellement (if any).
		/// </summary>
		/// <value>The name of the class.</value>
		public string ClassName
		{
			get { return GetAttributeValue("className"); }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Element"/> is completely loaded.
		/// </summary>
		/// <value><c>true</c> if complete; otherwise, <c>false</c>.</value>
		public bool Complete
		{
			get
		{
				string readyStateValue = GetAttributeValue("readyStateValue"); 
				if (readyStateValue != null)
				{
					return int.Parse(readyStateValue) == 4;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Element"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Enabled
		{
			get { return !bool.Parse(GetAttributeValue("disabled")); }
		}

		/// <summary>
		/// Gets the id of this element as specified in the HTML.
		/// </summary>
		/// <value>The id.</value>
		public string Id
		{
			get { return GetAttributeValue("id"); }
		}

		/// <summary>
		/// Gets the innertext of this element (and the innertext of all the elements contained
		/// in this element).
		/// </summary>
		/// <value>The innertext.</value>
		public virtual string Text
		{
			get { return GetAttributeValue("innertext"); }
		}

		/// <summary>
		/// Returns the text displayed after this element when it's wrapped
		/// in a Label element; otherwise it returns <c>null</c>.
		/// </summary>
		public string TextAfter
		{
			get { return NativeElement.TextAfter; }
		}

		/// <summary>
		/// Returns the text displayed before this element when it's wrapped
		/// in a Label element; otherwise it returns <c>null</c>.
		/// </summary>
		public string TextBefore
		{
			get { return NativeElement.TextBefore; }
		}

		/// <summary>
		/// Gets the inner HTML of this element.
		/// </summary>
		/// <value>The inner HTML.</value>
		public string InnerHtml
		{
			get { return GetAttributeValue("innerHTML"); }
		}

		/// <summary>
		/// Gets the outer text.
		/// </summary>
		/// <value>The outer text.</value>
		public string OuterText
		{
			get { return GetAttributeValue("outerText"); }
		}

		/// <summary>
		/// Gets the outer HTML.
		/// </summary>
		/// <value>The outer HTML.</value>
		public string OuterHtml
		{
			get { return GetAttributeValue("outerHTML"); }
		}

		/// <summary>
		/// Gets the tag name of this element.
		/// </summary>
		/// <value>The name of the tag.</value>
		public string TagName
		{
			get { return GetAttributeValue("tagName"); }
		}

		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return GetAttributeValue("title"); }
		}

		/// <summary>
		/// Gets the next sibling of this element in the Dom of the HTML page.
		/// </summary>
		/// <value>The next sibling.</value>
		public Element NextSibling
		{
			get
			{
                return TypedElementFactory.CreateTypedElement(_domContainer, NativeElement.NextSibling);
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
                return TypedElementFactory.CreateTypedElement(_domContainer, NativeElement.PreviousSibling);
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
                return TypedElementFactory.CreateTypedElement(_domContainer, NativeElement.Parent);
            }
        }

		public Style Style
		{
			//TODO: Style class should also delegate to a browser specific instance
			get { return NativeElement.Style; }
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

			if (string.Compare(attributeName, "style", true) == 0)
			{
				return Style.CssText;
			}

			return NativeElement.GetAttributeValue(attributeName);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
		    return UtilityClass.IsNotNullOrEmpty(Title) ? Title : Text;
		}

        /// <summary>
		/// Clicks this element and waits till the event is completely finished (page is loaded 
		/// and ready) .
		/// </summary>
		public void Click()
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(Id);
			}

			Logger.LogAction("Clicking " + GetType().Name + " '" + ToString() + "'");
			Highlight(true);

			NativeElement.ClickOnElement();

			try
			{
				WaitForComplete();
			}
			finally
			{
				Highlight(false);
			}
		}

		/// <summary>
		/// Clicks this instance and returns immediately. Use this method when you want to continue without waiting
		/// for the click event to be finished. Mostly used when a 
		/// HTMLDialog is displayed after clicking the element.
		/// </summary>
		public void ClickNoWait()
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(Id);
			}

			Logger.LogAction("Clicking (no wait) " + GetType().Name + " '" + ToString() + "'");

			Highlight(true);

			UtilityClass.AsyncActionOnBrowser(new ThreadStart(NativeElement.ClickOnElement));

		    Highlight(false);
		}

        /// <summary>
		/// Gives the (input) focus to this element.
		/// </summary>
		public void Focus()
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(Id);
			}

			NativeElement.SetFocus();
			FireEvent("onFocus");
		}

		/// <summary>
		/// Doubleclicks this element.
		/// </summary>
		public void DoubleClick()
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(Id);
			}

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
		/// Does a keydown on this element.
		/// </summary>
		public void KeyDown(char character)
		{
			FireEvent("onKeyDown", GetKeyCodeEventProperty(character));
		}

		/// <summary>
		/// Does a keyspress on this element.
		/// </summary>
		public void KeyPress()
		{
			FireEvent("onKeyPress");
		}

		public void KeyPress(char character)
		{
			FireEvent("onKeyPress", GetKeyCodeEventProperty(character));
		}

		private static NameValueCollection GetKeyCodeEventProperty(char character)
		{
			NameValueCollection eventProperties = new NameValueCollection(1);
			eventProperties.Add("keyCode", ((int) character).ToString());
			return eventProperties;
		}

		/// <summary>
		/// Does a keyup on this element.
		/// </summary>
		public void KeyUp()
		{
			FireEvent("onKeyUp");
		}

		/// <summary>
		/// Does a keyup on this element.
		/// </summary>
		/// <param name="character">The character.</param>
		public void KeyUp(char character)
		{
			FireEvent("onKeyUp", GetKeyCodeEventProperty(character));
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
		/// <param name="eventName">Name of the event.</param>
		public void FireEvent(string eventName)
		{
			fireEvent(eventName, true, null);
		}

		/// <summary>
		/// Fires the event. The <paramref name="eventProperties" /> collection
		/// can be used to set values of the event object in the browser to 
		/// full fill the needs of javascript attached to the event handler.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="eventProperties">The event properties that need to be set.</param>
		public void FireEvent(string eventName, NameValueCollection eventProperties)
		{
			fireEvent(eventName, true, eventProperties);
		}

		/// <summary>
		/// Only fires the specified <paramref name="eventName"/> on this element.
		/// </summary>
		public void FireEventNoWait(string eventName)
		{
			fireEvent(eventName, false, null);
		}

        /// <summary>
        /// Only fires the event but doesn't wait for the action to complete. 
        /// The <paramref name="eventProperties" /> collection
        /// can be used to set values of the event object in the browser to 
        /// full fill the needs of javascript attached to the event handler.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="eventProperties">The event properties that need to be set.</param>
        public void FireEventNoWait(string eventName, NameValueCollection eventProperties)
        {
            fireEvent(eventName, false, eventProperties);
        }
        
        private void fireEvent(string eventName, bool waitForComplete, NameValueCollection eventProperties)
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(Id);
			}

			Highlight(true);

            if (waitForComplete)
            {
                NativeElement.FireEvent(eventName, eventProperties);
                WaitForComplete();
            }
            else
            {
                NativeElement.FireEventAsync(eventName, eventProperties);
            }

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
		public void Highlight(bool doHighlight)
		{
			if (Settings.HighLightElement)
			{
				if (_originalcolor == null)
				{
					_originalcolor = new Stack();
				}

				if (doHighlight)
				{
					_originalcolor.Push(NativeElement.BackgroundColor);
					SetBackgroundColor(Settings.HighLightColor);
				}
				else
				{
					if(_originalcolor.Count > 0)
					{
						SetBackgroundColor(_originalcolor.Pop() as string);
					}
				}
			}
		}

		private void SetBackgroundColor(string color) 
		{
			try
			{
				if (color != null)
				{
					NativeElement.BackgroundColor = color;
					
				}
				else
				{
					NativeElement.BackgroundColor = "";
				}
			}
			catch{}
		}

		// TODO: Remove this property or move it to IEElement
		protected IHTMLElement htmlElement
		{
			get { return (IHTMLElement) HTMLElement; }
		}

		/// <summary>
		/// Gets the DOMcontainer for this element.
		/// </summary>
		/// <value>The DOM container.</value>
		public DomContainer DomContainer
		{
			get { return _domContainer; }
		}

		//TODO: Should return INativeElement instead of object

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
				return ((IEElement)NativeElement).NativeElement;
			}
		}

		/// <summary>
		/// Gets the DOM HTML element for this instance as an object. Cast it to 
		/// the interface you need. Most of the time the object supports IHTMLELement, 
		/// IHTMLElement2 and IHTMLElement3 but you can also cast it to a more
		/// specific interface. You should reference the microsoft.MSHTML.dll 
		/// assembly to cast it to a valid type.
		/// </summary>
		/// <value>The DOM element.</value>
		public INativeElement NativeElement
		{
			get
			{
				if (_nativeElement == null)
				{
					try
					{
						WaitUntilExists();
					}
					catch (WatiN.Core.Exceptions.TimeoutException e)
					{
					    if(e.InnerException == null)
						{
							throw new ElementNotFoundException(_nativeElementFinder.ElementTagsToString, _nativeElementFinder.ConstriantToString, _domContainer.Url);
						}
                        throw new ElementNotFoundException(_nativeElementFinder.ElementTagsToString, _nativeElementFinder.ConstriantToString, _domContainer.Url, e.InnerException);
					}
				}

				return _nativeElement;
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
				RefreshNativeElement();
				return _nativeElement != null;
			}
		}

		/// <summary>
		/// Waits until the element exists or will time out after 30 seconds.
		/// To change the default time out, set <see cref="P:WatiN.Core.Settings.WaitUntilExistsTimeOut"/>
		/// </summary>
		public void WaitUntilExists()
		{
			// Wait 30 seconds max
			WaitUntilExists(Settings.WaitUntilExistsTimeOut);
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
		/// To change the default time out, set <see cref="P:WatiN.Core.Settings.WaitUntilExistsTimeOut"/>
		/// </summary>
		public void WaitUntilRemoved()
		{
			// Wait 30 seconds max
			WaitUntilRemoved(Settings.WaitUntilExistsTimeOut);
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
			WaitUntil(attributename, expectedValue, Settings.WaitUntilExistsTimeOut);
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
			WaitUntil(new AttributeConstraint(attributename, expectedValue), timeout);
		}

		/// <summary>
		/// Waits until the given <paramref name="attributename" /> matches <paramref name="expectedValue" />.
		/// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
		/// </summary>
		/// <param name="attributename">The attributename.</param>
		/// <param name="expectedValue">The expected value.</param>
		public void WaitUntil(string attributename, Regex expectedValue)
		{
			WaitUntil(attributename, expectedValue, Settings.WaitUntilExistsTimeOut);
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
			WaitUntil(new AttributeConstraint(attributename, expectedValue), timeout);
		}

		/// <summary>
		/// Waits until the given <paramref name="constraint" /> matches.
		/// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
		/// </summary>
		/// <param name="constraint">The BaseConstraint.</param>
		public void WaitUntil(BaseConstraint constraint)
		{
			WaitUntil(constraint, Settings.WaitUntilExistsTimeOut);
		}

#if !NET11
        /// <summary>
		/// Waits until the given <paramref name="predicate" /> matches.
		/// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
		/// </summary>
		/// <param name="predicate">The BaseConstraint.</param>
		public void WaitUntil<E>(Predicate<E> predicate) where E : Element
		{
			WaitUntil(Find.ByElement(predicate), Settings.WaitUntilExistsTimeOut);
		}
#endif
		/// <summary>
		/// Waits until the given <paramref name="constraint" /> matches.
		/// </summary>
		/// <param name="constraint">The BaseConstraint.</param>
		/// <param name="timeout">The timeout.</param>
		public void WaitUntil(BaseConstraint constraint, int timeout)
		{
			Exception lastException;
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
						if (constraint.Compare(NativeElement.GetAttributeBag(_domContainer)))
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

			ThrowTimeOutException(lastException, string.Format("waiting {0} seconds for element matching constraint: {1}", timeout, constraint.ConstraintToString()));
		}

		private static void ThrowTimeOutException(Exception lastException, string message)
		{
		    if (lastException != null)
			{
				throw new WatiN.Core.Exceptions.TimeoutException(message, lastException);
			}
		    throw new WatiN.Core.Exceptions.TimeoutException(message);
		}

	    private void waitUntilExistsOrNot(int timeout, bool waitUntilExists)
	    {
	        // Does it make sense to go into the do loop?
			if (waitUntilExists)
			{
			    if (_nativeElement != null)
				{
					return;
				}

			    if (_nativeElementFinder == null)
			    {
			        throw new WatiNException("It's not possible to find the element because no elementFinder is available.");
			    }
			}
			else
			{
				if (_nativeElement == null)
				{
					return;
				}
			}

	        LoopUntilExistsEqualsWaitUntilExistsArgument(waitUntilExists, timeout);
	    }

        private void LoopUntilExistsEqualsWaitUntilExistsArgument(bool waitUntilExists, int timeout)
        {
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

            ThrowTimeOutException(lastException, string.Format("waiting {0} seconds for element to {1}.", timeout, waitUntilExists ? "show up" : "disappear"));
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
			if (_nativeElementFinder != null)
			{
				_nativeElement = null;
			}
		}

		/// <summary>
		/// Calls <see cref="Refresh"/> and returns the element.
		/// </summary>
		/// <returns></returns>
		protected INativeElement RefreshNativeElement()
		{
			if (_nativeElementFinder != null)
			{
				_nativeElement = _nativeElementFinder.FindFirst();
			}
			else
			{
				// This will set element to null if some specific properties are
				// a certain value. These values indicate that the element is no longer
				// on the/a valid web page.
				// These checks are only necessary if element field
				// is set during the construction of an ElementCollection
				// or a more specialized element collection (like TextFieldCollection)
				if (_nativeElement != null)
				{
					if (_nativeElement.IsElementReferenceStillValid() == false)
					{
						_nativeElement = null;
					}
				}
			}

			return _nativeElement;
		}

		/// <summary>
		/// Waits till the page load is complete. This should only be used on rare occasions
		/// because WatiN calls this function for you when it handles events (like Click etc..)
		/// To change the default time out, set <see cref="P:WatiN.Core.Settings.WaitForCompleteTimeOut"/>
		/// </summary>
		public void WaitForComplete()
		{
			_domContainer.WaitForComplete();
		}


#if !NET11
    /// <summary>
	/// Gets the closest ancestor of the specified type.
	/// </summary>
	/// <returns>An instance of the ancestorType. If no ancestor of ancestorType is found <code>null</code> is returned.</returns>
	///<example>
	/// The following example returns the Div a textfield is located in.
	/// <code>
	/// IE ie = new IE("http://www.example.com");
	/// Div mainDiv = ie.TextField("firstname").Ancestor&lt;Div&gt;;
	/// </code>
	/// </example>
   public T Ancestor<T>() where T : Element
    {
    	return (T)Ancestor(typeof(T));
    }

    /// <summary>
    /// Gets the closest ancestor of the specified Type and constraint.
    /// </summary>
    /// <param name="findBy">The constraint to match with.</param>
    /// <returns>
    /// An instance of the ancestorType. If no ancestor of ancestorType is found <code>null</code> is returned.
    /// </returns>
    /// <example>
    /// The following example returns the Div a textfield is located in.
    /// <code>
    /// IE ie = new IE("http://www.example.com");
    /// Div mainDiv = ie.TextField("firstname").Ancestor&lt;Div&gt;(Find.ByText("First name"));
    /// </code>
    /// </example>
    public T Ancestor<T>(BaseConstraint findBy) where T : Element
    {
    	return (T)Ancestor(typeof(T), findBy);
    }

    /// <summary>
    /// Gets the closest ancestor of the specified Type and constraint.
    /// </summary>
    /// <param name="predicate">The constraint to match with.</param>
    /// <returns>
    /// An instance of the ancestorType. If no ancestor of ancestorType is found <code>null</code> is returned.
    /// </returns>
    /// <example>
    /// The following example returns the Div a textfield is located in.
    /// <code>
    /// IE ie = new IE("http://www.example.com");
    /// Div mainDiv = ie.TextField("firstname").Ancestor&lt;Div&gt;(div => div.Text == "First name");
    /// </code>
    /// </example>
    public T Ancestor<T>(Predicate<T> predicate) where T : Element
    {
    	return (T)Ancestor(typeof(T), Find.ByElement(predicate));
    }
#endif

		/// <summary>
		/// Gets the closest ancestor of the specified type.
		/// </summary>
		/// <param name="ancestorType">The ancestorType.</param>
		/// <returns>An instance of the ancestorType. If no ancestor of ancestorType is found <code>null</code> is returned.</returns>
		///<example>
		/// The following example returns the Div a textfield is located in.
		/// <code>
		/// IE ie = new IE("http://www.example.com");
		/// Div mainDiv = ie.TextField("firstname").Ancestor(typeof(Div));
		/// </code>
		/// </example>
		public Element Ancestor(Type ancestorType)
		{
			return Ancestor(ancestorType, new AlwaysTrueConstraint());
		}

		/// <summary>
		/// Gets the closest ancestor of the specified AttributConstraint.
		/// </summary>
		/// <param name="findBy">The AttributConstraint to match with.</param>
		/// <returns>An Element. If no ancestor of ancestorType is found <code>null</code> is returned.</returns>
		///<example>
		/// The following example returns the Div a textfield is located in.
		/// <code>
		/// IE ie = new IE("http://www.example.com");
		/// Div mainDiv = ie.TextField("firstname").Ancestor(Find.ByText("First name"));
		/// </code>
		/// </example>
		public Element Ancestor(BaseConstraint findBy)
		{
			Element parentElement = Parent;

            if (parentElement == null)
            {
                return null;
            }
            
            if (findBy.Compare(parentElement))
            {
                return parentElement;
            }

            return parentElement.Ancestor(findBy);

		}

		/// <summary>
		/// Gets the closest ancestor of the specified Type and BaseConstraint.
		/// </summary>
		/// <param name="ancestorType">Type of the ancestor.</param>
		/// <param name="findBy">The BaseConstraint to match with.</param>
		/// <returns>
		/// An instance of the ancestorType. If no ancestor of ancestorType is found <code>null</code> is returned.
		/// </returns>
		/// <example>
		/// The following example returns the Div a textfield is located in.
		/// <code>
		/// IE ie = new IE("http://www.example.com");
		/// Div mainDiv = ie.TextField("firstname").Ancestor(typeof(Div), Find.ByText("First name"));
		/// </code>
		/// </example>
		public Element Ancestor(Type ancestorType, BaseConstraint findBy)
		{
			if (!ancestorType.IsSubclassOf(typeof (Element)) && (ancestorType != typeof (Element)))
			{
				throw new ArgumentException("Type should inherit from Element", "ancestorType");
			}

		    return Ancestor(Find.ByElement(new TypeComparer(ancestorType)) && findBy);
		}

		/// <summary>
		/// Gets the closest ancestor of the specified Tag and AttributConstraint.
		/// </summary>
		/// <param name="tagName">The tag of the ancestor.</param>
		/// <param name="findBy">The AttributConstraint to match with.</param>
		/// <returns>
		/// <returns>An typed instance of the element matching the Tag and the AttributeConstriant.
		/// If no specific type is available, an element of type ElementContainer will be returned. 
		/// If there is no ancestor that matches Tag and BaseConstraint, <code>null</code> is returned.</returns>
		/// </returns>
		/// <example>
		/// The following example returns the Div a textfield is located in.
		/// <code>
		/// IE ie = new IE("http://www.example.com");
		/// Div mainDiv = ie.TextField("firstname").Ancestor("Div", Find.ByText("First name"));
		/// </code>
		/// </example>
		public Element Ancestor(string tagName, BaseConstraint findBy)
		{
			BaseConstraint findAncestor = Find.By("tagname", new StringEqualsAndCaseInsensitiveComparer(tagName))
			                                   && findBy;

			return Ancestor(findAncestor);
		}

#if !NET11
        /// <summary>
		/// Gets the closest ancestor of the specified Tag and AttributConstraint.
		/// </summary>
		/// <param name="tagName">The tag of the ancestor.</param>
        /// <param name="predicate">The constraint to match with.</param>
        /// <returns>
		/// <returns>An typed instance of the element matching the Tag and the AttributeConstriant.
		/// If no specific type is available, an element of type ElementContainer will be returned. 
		/// If there is no ancestor that matches Tag and BaseConstraint, <code>null</code> is returned.</returns>
		/// </returns>
		/// <example>
		/// The following example returns the Div a textfield is located in.
		/// <code>
		/// IE ie = new IE("http://www.example.com");
		/// Div mainDiv = ie.TextField("firstname").Ancestor("Div", Find.ByText("First name"));
		/// </code>
		/// </example>
		public Element Ancestor(string tagName, Predicate<Element> predicate)
		{
			BaseConstraint findAncestor = Find.By("tagname", new StringEqualsAndCaseInsensitiveComparer(tagName))
                                               && Find.ByElement(predicate);

			return Ancestor(findAncestor);
		}
#endif
		/// <summary>
		/// Gets the closest ancestor of the specified Tag.
		/// </summary>
		/// <param name="tagName">The tag of the ancestor.</param>
		/// <returns>An typed instance of the element matching the Tag. If no specific type is
		/// available, an element of type ElementContainer will be returned. 
		/// If there is no ancestor that matches Tag, <code>null</code> is returned.</returns>
		///<example>
		/// The following example returns the Div a textfield is located in.
		/// <code>
		/// IE ie = new IE("http://www.example.com");
		/// Div mainDiv = ie.TextField("firstname").Ancestor("Div");
		/// </code>
		/// </example>
		public Element Ancestor(string tagName)
		{
			return Ancestor(tagName, new AlwaysTrueConstraint());
		}

		public string GetValue(string attributename)
		{
			return NativeElement.GetAttributeBag(_domContainer).GetValue(attributename);
		}

		internal static Element New(DomContainer domContainer, IHTMLElement element)
		{
            return TypedElementFactory.CreateTypedElement(domContainer, domContainer.NativeBrowser.CreateElement(element));
		}
	}
}
