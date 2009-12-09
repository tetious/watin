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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using WatiN.Core.Actions;
using WatiN.Core.Comparers;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using WatiN.Core.Native;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{

    public delegate INativeElementCollection NativeElementCollectionFactory(INativeElement nativeElement);

	/// <summary>
	/// This is the base class for all other element types in this project, like
	/// Button, Checkbox etc.. It provides common functionality to all these elements
	/// </summary>
	public class Element<TElement> : Element, IEquatable<Element>
        where TElement : Element
	{
	    public Element(DomContainer domContainer, INativeElement nativeElement) : base(domContainer, nativeElement)
	    {}

        public Element(DomContainer domContainer, ElementFinder elementFinder)
            : base(domContainer, elementFinder)
	    {}

	    /// <summary>
        /// Waits until the given expression is <c>true</c>.
        /// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
        /// </summary>
        /// <param name="predicate">The expression to use.</param>
        public virtual void WaitUntil(Predicate<TElement> predicate)
        {
            WaitUntil(Find.ByElement(predicate), Settings.WaitUntilExistsTimeOut);
        }

        /// <summary>
		/// Waits until the given expression is <c>true</c>.
		/// </summary>
        /// <param name="predicate">The expression to use.</param>
		/// <param name="timeout">The timeout.</param>
        public virtual void WaitUntil(Predicate<TElement> predicate, int timeout)
		{
            WaitUntil(Find.ByElement(predicate), timeout);
        }
	}

    /// <summary>
	/// This is the base class for all other element types in this project, like
	/// Button, Checkbox etc.. It provides common functionality to all these elements
	/// </summary>
    [DebuggerDisplay("{DebuggerDisplayProxy()}")]
    public class Element : Component
	{
        private INativeElement _cachedNativeElement;
        private ElementFinder _elementFinder;

        private HighlightAction _highlightAction;

        /// <summary>
		/// This constructor is mainly used from within WatiN.
		/// </summary>
		/// <param name="domContainer"><see cref="DomContainer" /> this element is located in</param>
		/// <param name="nativeElement">The element</param>
		public Element(DomContainer domContainer, INativeElement nativeElement)
		{
			InitElement(domContainer,nativeElement, null);
		}

		/// <summary>
		/// This constructor is mainly used from within WatiN.
		/// </summary>
		/// <param name="domContainer"><see cref="DomContainer"/> this element is located in</param>
		/// <param name="elementFinder">The element finder.</param>
        public Element(DomContainer domContainer, ElementFinder elementFinder)
		{
			InitElement(domContainer, null, elementFinder);
		}

        private void InitElement(DomContainer domContainer, INativeElement nativeElement, ElementFinder elementFinder) 
		{
            if (domContainer == null) throw new ArgumentNullException("domContainer");
            if (nativeElement == null && elementFinder == null) throw new ArgumentException("Either nativeElement or elementFinder needs to be set");
			
            DomContainer = domContainer;
			_cachedNativeElement = nativeElement;
			_elementFinder = elementFinder;
        }

		/// <summary>
		/// Gets the name of the stylesheet class assigned to this ellement (if any).
		/// </summary>
		/// <value>The name of the class.</value>
        public virtual string ClassName
		{
			get { return GetAttributeValue("className"); }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Element"/> is completely loaded.
		/// </summary>
		/// <value><c>true</c> if complete; otherwise, <c>false</c>.</value>
        public virtual bool Complete
		{
			get
		{
				var readyStateValue = GetAttributeValue("readyStateValue"); 
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
        public virtual bool Enabled
		{
			get
			{
			    var value = GetAttributeValue("disabled");
                if (string.IsNullOrEmpty(value)) return true;
			    return !bool.Parse(value);
			}
		}

		/// <summary>
		/// Gets (or sets) the id of this element as specified in the HTML.
		/// </summary>
		/// <value>The id.</value>
        public virtual string Id
		{
			get { return GetAttributeValue("id"); }
            set { SetAttributeValue("id", value); }
		}


        /// <summary>
        /// Gets the javascript element reference for this <see cref="Element"/>.
        /// </summary>
        /// <returns></returns>
        public virtual string GetJavascriptElementReference()
        {
            return NativeElement.GetJavaScriptElementReference();
        }

        /// <summary>
        /// Gets (or sets) the name of this element as specified in the HTML.
        /// </summary>
        /// <value>The name</value>
        public virtual string Name
        {
            get { return GetAttributeValue("name"); }
            set { SetAttributeValue("name", value); }
        }

        /// <summary>
        /// Gets the id or name (id preferred) of this element as specified in the HTML.
        /// </summary>
        /// <value>The id.</value>
        public virtual string IdOrName
        {
            get { return UtilityClass.IsNotNullOrEmpty(Id) ? Id : Name; }
        }

		/// <summary>
		/// Gets the innertext of this element (and the innertext of all the elements contained
		/// in this element).
		/// </summary>
		/// <value>The innertext.</value>
		public virtual string Text
		{
            get { return GetAttributeValue(Find.innerTextAttribute); }
		}

		/// <summary>
		/// Returns the text displayed after this element when it's wrapped
		/// in a Label element; otherwise it returns <c>null</c>.
		/// </summary>
        public virtual string TextAfter
		{
			get { return NativeElement.TextAfter; }
		}

		/// <summary>
		/// Returns the text displayed before this element when it's wrapped
		/// in a Label element; otherwise it returns <c>null</c>.
		/// </summary>
        public virtual string TextBefore
		{
			get { return NativeElement.TextBefore; }
		}

		/// <summary>
		/// Gets the inner HTML of this element.
		/// </summary>
		/// <value>The inner HTML.</value>
        public virtual string InnerHtml
		{
			get { return GetAttributeValue("innerHTML"); }
		}

		/// <summary>
		/// Gets the outer text.
		/// </summary>
		/// <value>The outer text.</value>
        public virtual string OuterText
		{
			get { return GetAttributeValue("outerText"); }
		}

		/// <summary>
		/// Gets the outer HTML.
		/// </summary>
		/// <value>The outer HTML.</value>
        public virtual string OuterHtml
		{
			get { return GetAttributeValue("outerHTML"); }
		}

		/// <summary>
		/// Gets the tag name of this element.
		/// </summary>
		/// <value>The name of the tag.</value>
        public virtual string TagName
		{
			get { return NativeElement.TagName; }
		}

		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>The title.</value>
        public virtual string Title
		{
			get { return GetAttributeValue("title"); }
		}

		/// <summary>
		/// Gets the next sibling of this element in the Dom of the HTML page.
		/// </summary>
		/// <value>The next sibling.</value>
        public virtual Element NextSibling
		{
			get
			{
                return ElementFactory.CreateElement(DomContainer, NativeElement.NextSibling);
			}
        }

		/// <summary>
		/// Gets the previous sibling of this element in the Dom of the HTML page.
		/// </summary>
		/// <value>The previous sibling.</value>
        public virtual Element PreviousSibling
		{
			get
			{
                return ElementFactory.CreateElement(DomContainer, NativeElement.PreviousSibling);
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
        public virtual Element Parent
		{
			get
			{
                return ElementFactory.CreateElement(DomContainer, NativeElement.Parent);
            }
        }

        public virtual Style Style
		{
            get { return new Style(NativeElement); }
		}

        /// <summary>
        /// Wraps an element as a control of a particular type.
        /// </summary>
        /// <typeparam name="TControl">The <see cref="Control{TElement}" /> subclass</typeparam>
        /// <returns>The control</returns>
        public virtual TControl As<TControl>()
            where TControl : Control, new()
        {
            return Control.CreateControl<TControl>(this);
        }

        /// <inheritdoc />
        protected override string GetAttributeValueImpl(string attributeName)
        {
            if (UtilityClass.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException("attributeName", "Null or Empty not allowed.");
            }

            var toLowerInvariant = attributeName.ToLowerInvariant();

            if (toLowerInvariant == "style")
            {
                return NativeElement.GetStyleAttributeValue("cssText");
            }

            if (toLowerInvariant.StartsWith("style."))
            {
                return NativeElement.GetStyleAttributeValue(attributeName.Substring(6));
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
            return UtilityClass.IsNotNullOrEmpty(Description) ? Description : DefaultToString();
		}

        public virtual Element SetDescription(string value)
        {
            Description = value;
            return this;
        }

        /// <summary>
        /// Obtains a default description of the element to be used when <see cref="IHasDescription.Description" /> is null.
        /// </summary>
        protected virtual string DefaultToString()
        {
            return UtilityClass.IsNotNullOrEmpty(Title) ? Title : Text;
        }

        /// <summary>
		/// Clicks this element and waits till the event is completely finished (page is loaded 
		/// and ready) .
		/// </summary>
        public virtual void Click()
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(IdOrName, this);
			}

            Logger.LogAction("Clicking {0} '{1}', {2}", GetType().Name, IdOrName, Description);
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
        public virtual void ClickNoWait()
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(IdOrName, this);
			}

            Logger.LogAction("Clicking (no wait) {0} '{1}', {2}", GetType().Name, IdOrName, Description);

			Highlight(true);

			UtilityClass.AsyncActionOnBrowser(NativeElement.ClickOnElement);

		    Highlight(false);
		}

        /// <summary>
		/// Gives the (input) focus to this element.
		/// </summary>
        public virtual void Focus()
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(IdOrName, this);
			}

			NativeElement.SetFocus();
			FireEvent("onFocus");
		}

		/// <summary>
		/// Doubleclicks this element.
		/// </summary>
        public virtual void DoubleClick()
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(IdOrName, this);
			}

            Logger.LogAction("Double clicking {0} '{1}', {2}", GetType().Name, IdOrName, Description);

			FireEvent("onDblClick");
		}

		/// <summary>
		/// Does a keydown on this element.
		/// </summary>
        public virtual void KeyDown()
		{
			FireEvent("onKeyDown");
		}

        /// <summary>
        /// Does a keydown on this element and does not wait for the page to finish loading.
        /// </summary>
        public virtual void KeyDownNoWait()
        {
            FireEventNoWait("onKeyDown");
        }

		/// <summary>
		/// Does a keydown on this element.
		/// </summary>
        public virtual void KeyDown(char character)
		{
			FireEvent("onKeyDown", GetKeyCodeEventProperty(character));
		}

        /// <summary>
        /// Does a keydown on this element and does not wait for the page to finish loading.
        /// </summary>
        public virtual void KeyDownNoWait(char character)
        {
            FireEventNoWait("onKeyDown", GetKeyCodeEventProperty(character));
        }

		/// <summary>
		/// Does a keypress on this element.
		/// </summary>
        public virtual void KeyPress()
		{
			FireEvent("onKeyPress");
		}

        /// <summary>
        /// Does a keypress on this element and does not wait for the page to finish loading.
        /// </summary>
        public virtual void KeyPressNoWait()
        {
            FireEventNoWait("onKeyPress");
        }

        /// <summary>
        /// Does a keypress on this element.
        /// </summary>
        public virtual void KeyPress(char character)
		{
			FireEvent("onKeyPress", GetKeyCodeEventProperty(character));
		}

        /// <summary>
        /// Does a keypress on this element and does not wait for the page to finish loading.
        /// </summary>
        /// <param name="character">The character.</param>
        public virtual void KeyPressNoWait(char character)
        {
            FireEventNoWait("onKeyPress", GetKeyCodeEventProperty(character));
        }

        /// <summary>
        /// Does a keyup on this element.
        /// </summary>
        public virtual void KeyUp()
        {
            FireEvent("onKeyUp");
        }

        /// <summary>
        /// Does a keyup on this element and does not wait for the page to finish loading.
        /// </summary>
        public virtual void KeyUpNoWait()
        {
            FireEventNoWait("onKeyUp");
        }

        /// <summary>
        /// Does a keyup on this element.
        /// </summary>
        /// <param name="character">The character.</param>
        public virtual void KeyUp(char character)
        {
            FireEvent("onKeyUp", GetKeyCodeEventProperty(character));
        }

        /// <summary>
        /// Does a keyup on this element and does not wait for the page to finish loading.
        /// </summary>
        /// <param name="character">The character.</param>
        public virtual void KeyUpNoWait(char character)
        {
            FireEventNoWait("onKeyUp", GetKeyCodeEventProperty(character));
        }

		private static NameValueCollection GetKeyCodeEventProperty(char character)
		{
		    return new NameValueCollection
		               {
		                   {"keyCode", ((int) character).ToString()},
		                   {"charCode", ((int) character).ToString()}
		               };
		}

		/// <summary>
		/// Fires the blur event on this element.
		/// </summary>
        public virtual void Blur()
		{
			FireEvent("onBlur");
		}

		/// <summary>
		/// Fires the change event on this element.
		/// </summary>
        public virtual void Change()
		{
			FireEvent("onChange");
		}

		/// <summary>
		/// Fires the mouseenter event on this element.
		/// </summary>
        public virtual void MouseEnter()
		{
			FireEvent("onMouseEnter");
		}

		/// <summary>
		/// Fires the mousedown event on this element.
		/// </summary>
        public virtual void MouseDown()
		{
			FireEvent("onmousedown");
		}

		/// <summary>
		/// Fires the mouseup event on this element.
		/// </summary>
        public virtual void MouseUp()
		{
			FireEvent("onmouseup");
		}

		/// <summary>
		/// Fires the specified <paramref name="eventName"/> on this element
		/// and waits for it to complete.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
        public virtual void FireEvent(string eventName)
		{
			FireEvent(eventName, true, null);
		}

		/// <summary>
		/// Fires the event. The <paramref name="eventProperties" /> collection
		/// can be used to set values of the event object in the browser to 
		/// full fill the needs of javascript attached to the event handler.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="eventProperties">The event properties that need to be set.</param>
        public virtual void FireEvent(string eventName, NameValueCollection eventProperties)
		{
			FireEvent(eventName, true, eventProperties);
		}

		/// <summary>
		/// Only fires the specified <paramref name="eventName"/> on this element.
		/// </summary>
        public virtual void FireEventNoWait(string eventName)
		{
			FireEvent(eventName, false, null);
		}

        /// <summary>
        /// Only fires the event but doesn't wait for the action to complete. 
        /// The <paramref name="eventProperties" /> collection
        /// can be used to set values of the event object in the browser to 
        /// full fill the needs of javascript attached to the event handler.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="eventProperties">The event properties that need to be set.</param>
        public virtual void FireEventNoWait(string eventName, NameValueCollection eventProperties)
        {
            FireEvent(eventName, false, eventProperties);
        }
        
        private void FireEvent(string eventName, bool waitForComplete, NameValueCollection eventProperties)
		{
			if (!Enabled)
			{
				throw new ElementDisabledException(IdOrName, this);
			}

			Highlight(true);

            if (waitForComplete)
            {
                NativeElement.FireEvent(eventName, eventProperties);
                WaitForComplete();
            }
            else
            {
                NativeElement.FireEventNoWait(eventName, eventProperties);
            }

			Highlight(false);
		}

		/// <summary>
		/// Flashes this element 5 times.
		/// </summary>
        public virtual void Flash()
		{
			Flash(5);
		}

		/// <summary>
		/// Flashes this element the specified number of flashes.
		/// </summary>
		/// <param name="numberOfFlashes">The number of flashes.</param>
        public virtual void Flash(int numberOfFlashes)
		{
			for (var counter = 0; counter < numberOfFlashes; counter++)
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
        public virtual void Highlight(bool doHighlight)
		{
		    if (!Settings.HighLightElement) return;
		    
            if (_highlightAction == null)
                _highlightAction = new HighlightAction(this);

		    _highlightAction.Highlight(doHighlight);
		}

        /// <summary>
        /// Gets the DomContainer for this element.
        /// </summary>
        public virtual DomContainer DomContainer { get; private set; }

		/// <summary>
		/// Gets a reference to the wrapper which incapsulates a native element in the browser.
		/// </summary>
        public virtual INativeElement NativeElement
		{
			get
			{
                if (GetCachedNativeElement() != null) return GetCachedNativeElement();

                INativeElement nativeElement = null;
				try
				{
					WaitUntilExists();
                    nativeElement = GetCachedNativeElement();
                }
				catch (Exceptions.TimeoutException e)
				{
				    if(e.InnerException != null)
                        throw new ElementNotFoundException(_elementFinder.ElementTagsToString(), _elementFinder.ConstraintToString(), DomContainer.Url, e.InnerException, this);
				}

                if (nativeElement == null)
                    throw new ElementNotFoundException(_elementFinder.ElementTagsToString(), _elementFinder.ConstraintToString(), DomContainer.Url, this);

				return nativeElement;
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
				return FindNativeElement() != null;
			}
		}

		/// <summary>
		/// Waits until the element exists or will time out after 30 seconds.
		/// To change the default time out, set <see cref="P:WatiN.Core.Settings.WaitUntilExistsTimeOut"/>
		/// </summary>
        public virtual void WaitUntilExists()
		{
			// Wait 30 seconds max
			WaitUntilExists(Settings.WaitUntilExistsTimeOut);
		}

		/// <summary>
		/// Waits until the element exists. Wait will time out after <paramref name="timeout"/> seconds.
		/// </summary>
		/// <param name="timeout">The timeout in seconds.</param>
        public virtual void WaitUntilExists(int timeout)
		{
			WaitUntilExistsOrNot(timeout, true);
		}

		/// <summary>
		/// Waits until the element no longer exists or will time out after 30 seconds.
		/// To change the default time out, set <see cref="P:WatiN.Core.Settings.WaitUntilExistsTimeOut"/>
		/// </summary>
        public virtual void WaitUntilRemoved()
		{
			// Wait 30 seconds max
			WaitUntilRemoved(Settings.WaitUntilExistsTimeOut);
		}

		/// <summary>
		/// Waits until the element no longer exists. Wait will time out after <paramref name="timeout"/> seconds.
		/// </summary>
		/// <param name="timeout">The timeout in seconds.</param>
        public virtual void WaitUntilRemoved(int timeout)
		{
			WaitUntilExistsOrNot(timeout, false);
		}

		/// <summary>
		/// Waits until the given <paramref name="attributename" /> matches <paramref name="expectedValue" />.
		/// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
		/// </summary>
		/// <param name="attributename">The attributename.</param>
		/// <param name="expectedValue">The expected value.</param>
        public virtual void WaitUntil(string attributename, string expectedValue)
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
        public virtual void WaitUntil(string attributename, string expectedValue, int timeout)
		{
			WaitUntil(new AttributeConstraint(attributename, expectedValue), timeout);
		}

		/// <summary>
		/// Waits until the given <paramref name="attributename" /> matches <paramref name="expectedValue" />.
		/// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
		/// </summary>
		/// <param name="attributename">The attributename.</param>
		/// <param name="expectedValue">The expected value.</param>
        public virtual void WaitUntil(string attributename, Regex expectedValue)
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
        public virtual void WaitUntil(string attributename, Regex expectedValue, int timeout)
		{
			WaitUntil(new AttributeConstraint(attributename, expectedValue), timeout);
		}

		/// <summary>
		/// Waits until the given <paramref name="constraint" /> matches.
		/// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
		/// </summary>
		/// <param name="constraint">The Constraint.</param>
        public virtual void WaitUntil(Constraint constraint)
		{
			WaitUntil(constraint, Settings.WaitUntilExistsTimeOut);
		}

        /// <summary>
		/// Waits until the given <paramref name="predicate" /> matches.
		/// Wait will time out after <see cref="Settings.WaitUntilExistsTimeOut"/> seconds.
		/// </summary>
		/// <param name="predicate">The Constraint.</param>
        public virtual void WaitUntil<E>(Predicate<E> predicate) where E : Element
		{
			WaitUntil(Find.ByElement(predicate), Settings.WaitUntilExistsTimeOut);
		}

        /// <summary>
		/// Waits until the given <paramref name="constraint" /> matches.
		/// </summary>
		/// <param name="constraint">The Constraint.</param>
		/// <param name="timeout">The timeout in seconds.</param>
        public virtual void WaitUntil(Constraint constraint, int timeout)
		{
			// Calling Exists will refresh the reference to the html element
			// so the compare is against the current html element (and not 
			// against some cached reference.
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(timeout))
            {
                ExceptionMessage = () => string.Format("waiting {0} seconds for element matching constraint: {1}", timeout, constraint)
            };

            tryActionUntilTimeOut.Try(() => Exists && Matches(constraint));
		}

	    private void WaitUntilExistsOrNot(int timeout, bool waitUntilExists)
	    {
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(timeout))
                {
                    ExceptionMessage = () => string.Format("waiting {0} seconds for element to {1}.", timeout,
                                                  waitUntilExists ? "show up" : "disappear")
                };

            tryActionUntilTimeOut.Try(() => 
            {
                    return ! CanFindNativeElement() || Exists == waitUntilExists;
            });

            if (! CanFindNativeElement())
                throw new WatiNException("It's not possible to find the element because no element finder is available.");
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
        public virtual void Refresh()
		{
			ClearCachedNativeElement();
		}

        /// <summary>
        /// Finds the native element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method caches the native element unless its reference becomes invalid
        /// in which case it will search for the element again.
        /// </para>
        /// <para>
        /// Unlike the <see cref="NativeElement" /> property, this method does not wait
        /// for the native element to be found.  If the native element is not found then
        /// this method returns null.
        /// </para>
        /// </remarks>
        /// <returns>The native element, or null if not found.</returns>
        public virtual INativeElement FindNativeElement()
        {
            if (_cachedNativeElement != null)
            {
                if (_cachedNativeElement.IsElementReferenceStillValid())
                    return _cachedNativeElement;

                _cachedNativeElement = null;
            }

            if (_elementFinder != null)
            {
                var foundElement = _elementFinder.FindFirst();
                if (foundElement != null)
                    _cachedNativeElement = foundElement._cachedNativeElement;
            }

            return _cachedNativeElement;
        }

        /// <summary>
		/// Clears the cached native element and finds it again.
		/// </summary>
		/// <returns>The native element, or null if not found.</returns>
		public virtual INativeElement RefreshNativeElement()
		{
            ClearCachedNativeElement();
            return FindNativeElement();
		}

        /// <summary>
        /// Returns true if it is possible to find a native element.
        /// </summary>
        /// <remarks>
        /// This method may return false if the element does not have an associated element finder
        /// and no native element is currently cached.
        /// </remarks>
        /// <returns>True if an element can be found.</returns>
        public virtual bool CanFindNativeElement()
        {
            return _cachedNativeElement != null || _elementFinder != null;
        }

        /// <summary>
        /// Clears the cached native element.
        /// </summary>
        protected void ClearCachedNativeElement()
        {
            if (_elementFinder != null)
                _cachedNativeElement = null;
        }

        /// <summary>
        /// Gets the cached native element immediately.
        /// </summary>
        /// <returns></returns>
        protected INativeElement GetCachedNativeElement()
        {
            return _cachedNativeElement;
        }

        /// <summary>
		/// Waits till the page load is complete. This should only be used on rare occasions
		/// because WatiN calls this function for you when it handles events (like Click etc..)
		/// To change the default time out, set <see cref="P:WatiN.Core.Settings.WaitForCompleteTimeOut"/>
		/// </summary>
        public virtual void WaitForComplete()
		{
			DomContainer.WaitForComplete();
		}


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
        public virtual T Ancestor<T>() where T : Element
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
        public virtual T Ancestor<T>(Constraint findBy) where T : Element
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
        public virtual T Ancestor<T>(Predicate<T> predicate) where T : Element
        {
    	    return (T)Ancestor(typeof(T), Find.ByElement(predicate));
        }

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
        public virtual Element Ancestor(Type ancestorType)
		{
			return Ancestor(ancestorType, Find.Any);
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
        public virtual Element Ancestor(Constraint findBy)
		{
			var parentElement = Parent;

            if (parentElement == null)
            {
                return null;
            }
            
            return parentElement.Matches(findBy) ? parentElement : parentElement.Ancestor(findBy);
		}

		/// <summary>
		/// Gets the closest ancestor of the specified Type and Constraint.
		/// </summary>
		/// <param name="ancestorType">Type of the ancestor.</param>
		/// <param name="findBy">The Constraint to match with.</param>
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
        public virtual Element Ancestor(Type ancestorType, Constraint findBy)
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
		/// If there is no ancestor that matches Tag and Constraint, <code>null</code> is returned.</returns>
		/// </returns>
		/// <example>
		/// The following example returns the Div a textfield is located in.
		/// <code>
		/// IE ie = new IE("http://www.example.com");
		/// Div mainDiv = ie.TextField("firstname").Ancestor("Div", Find.ByText("First name"));
		/// </code>
		/// </example>
        public virtual Element Ancestor(string tagName, Constraint findBy)
		{
			var findAncestor = Find.By(Find.tagNameAttribute, new StringEqualsAndCaseInsensitiveComparer(tagName))
			                                   && findBy;

			return Ancestor(findAncestor);
		}

        /// <summary>
		/// Gets the closest ancestor of the specified Tag and AttributConstraint.
		/// </summary>
		/// <param name="tagName">The tag of the ancestor.</param>
        /// <param name="predicate">The constraint to match with.</param>
        /// <returns>
		/// <returns>An typed instance of the element matching the Tag and the AttributeConstriant.
		/// If no specific type is available, an element of type ElementContainer will be returned. 
		/// If there is no ancestor that matches Tag and Constraint, <code>null</code> is returned.</returns>
		/// </returns>
		/// <example>
		/// The following example returns the Div a textfield is located in.
		/// <code>
		/// IE ie = new IE("http://www.example.com");
		/// Div mainDiv = ie.TextField("firstname").Ancestor("Div", Find.ByText("First name"));
		/// </code>
		/// </example>
        public virtual Element Ancestor(string tagName, Predicate<Element> predicate)
		{
			var findAncestor = Find.By("tagname", new StringEqualsAndCaseInsensitiveComparer(tagName))
                                               && Find.ByElement(predicate);

			return Ancestor(findAncestor);
		}

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
        public virtual Element Ancestor(string tagName)
		{
			return Ancestor(tagName, Find.Any);
		}

        public virtual void SetAttributeValue(string attributeName, string value)
        {
            NativeElement.SetAttributeValue(attributeName, value);
        }

        sealed public override bool Equals(object obj)
        {
            return Equals(obj as Element);
        }

        public virtual bool Equals(Element element)
        {
            if (element == null)
                return false;

            if (! CanFindNativeElement() || ! element.CanFindNativeElement())
                return ReferenceEquals(element, this);

            return NativeElement.Equals(element.NativeElement);
        }

        public override int GetHashCode()
        {
            return CanFindNativeElement() ? NativeElement.GetHashCode() : 0;
        }

        /// <summary>
        /// Creates an element finder for elements within specialized collections.
        /// </summary>
        /// <typeparam name="TElement">The element type</typeparam>
        /// <param name="nativeElementCollection">The native element collection</param>
        /// <param name="findBy">The constraint, or null if none</param>
        /// <returns>The native element finder</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeElementCollection"/> is null</exception>
        protected ElementFinder CreateElementFinder<TElement>(NativeElementCollectionFactory nativeElementCollection, Constraint findBy)
            where TElement : Element
        {
            if (nativeElementCollection == null)
                throw new ArgumentNullException("nativeElementCollection");

            return new NativeElementFinder(CreateNativeElementCollectionFactory(nativeElementCollection), DomContainer, ElementFactory.GetElementTags<TElement>(), findBy);
        }

        protected NativeElementFinder.NativeElementCollectionFactory CreateNativeElementCollectionFactory(NativeElementCollectionFactory factory)
        {
            return () =>
                {
                    INativeElement nativeElement = FindNativeElement();
                    return nativeElement != null ? factory.Invoke(nativeElement) : null;
                };
        }

        protected virtual object DebuggerDisplayProxy()
        {
            // Don't display our value if it doesn't exist... it causes the debugger to time out.
            return String.Format(CultureInfo.InvariantCulture, "{0}@{1:X}[Constraint = \"{2}\", Exists = {3}]", 
                                 GetType().FullName, RuntimeHelpers.GetHashCode(this),
                                 _elementFinder != null ? _elementFinder.ConstraintToString() : "",
                                 Exists);
        }
	}
}
