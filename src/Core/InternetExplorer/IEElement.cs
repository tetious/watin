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
using mshtml;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.InternetExplorer
{
	/// <summary>
	/// Summary description for IEElement.
	/// </summary>
	public class IEElement : INativeElement
	{
		private readonly object _element;
		private ElementAttributeBag _attributeBag;

		public IEElement(object element)
		{
            if (element == null) throw new ArgumentNullException("element");
            if (element is INativeElement) throw new Exception("INativeElement not allowed");
			_element = element;
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
		/// Gets the next sibling of this element in the Dom of the HTML page.
		/// </summary>
		/// <value>The next sibling.</value>
		public INativeElement NextSibling
		{
			get
			{
				var node = domNode.nextSibling;
				while (node != null)
				{
					var nextSibling = node as IHTMLElement;
					if (nextSibling != null)
					{
						return new IEElement(nextSibling);
					}
				    
                    node = node.nextSibling;
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the previous sibling of this element in the Dom of the HTML page.
		/// </summary>
		/// <value>The previous sibling.</value>
		public INativeElement PreviousSibling
		{
			get
			{
				var node = domNode.previousSibling;
				while (node != null)
				{
					var previousSibling = node as IHTMLElement;
					if (previousSibling != null)
					{
						return new IEElement(previousSibling);
					}
				    
                    node = node.previousSibling;
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
		public INativeElement Parent
		{
			get
			{
				var parentNode = domNode.parentNode as IHTMLElement;
				return parentNode != null ? new IEElement(parentNode) : null;
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
            if (attributeName.ToLowerInvariant() == "tagname") return TagName;

			var attributeValue = htmlElement.getAttribute(attributeName, 0);

			if (attributeValue == DBNull.Value || attributeValue == null)
			{
				return null;
			}

			return attributeValue.ToString();
		}

        public void SetAttributeValue(string attributeName, string value)
        {
            value = HandleAttributesWhichHaveNoValuePart(attributeName, value);
            
            htmlElement.setAttribute(attributeName, value, 0);
        }

	    private static string HandleAttributesWhichHaveNoValuePart(string attributeName, string value)
	    {
            // selected is attribute of Option
            // checked is attribute of RadioButton and CheckBox
	        if (attributeName == "selected" || attributeName == "checked")
	        {
	            value = bool.Parse(value) ? "true" : "";
	        }
	        return value;
	    }


	    public void ClickOnElement()
		{
			DispHtmlBaseElement.click();
		}

		public void SetFocus() 
		{
			DispHtmlBaseElement.focus();
		}

		public void FireEvent(string eventName, NameValueCollection eventProperties)
		{
		    if (eventProperties == null)
			{
				UtilityClass.FireEvent(DispHtmlBaseElement, eventName);
			}
			else
			{
				if (eventName == "onKeyPress")
				{
				    var addChar = eventProperties.GetValues("keyCode")[0];
				    var newValue = GetAttributeValue("value") + ((char) int.Parse(addChar));
				    SetAttributeValue("value", newValue);
				}

			    UtilityClass.FireEvent(DispHtmlBaseElement, eventName, eventProperties);
			}
		}

	    public object Objects
	    {
            get { return htmlElement.all; }
	    }

	    public void FireEventNoWait(string eventName, NameValueCollection eventProperties)
        {
            var scriptCode = UtilityClass.CreateJavaScriptFireEventCode(eventProperties, DispHtmlBaseElement, eventName);
            var window = ((IHTMLDocument2)DispHtmlBaseElement.document).parentWindow;

            var asyncScriptRunner = new AsyncScriptRunner(scriptCode.ToString(), window);

            UtilityClass.AsyncActionOnBrowser(asyncScriptRunner.FireEvent);
        }

	    public void Select()
	    {
	        var input = _element as IHTMLInputElement;
            if (input != null)
            {
                input.select();
                return;
            }
	        var textarea = _element as IHTMLTextAreaElement;
            if (textarea != null)
            {
                textarea.select();
                return;
            }

            throw new WatiNException("Select not supported on " + _element.GetType());
        }

	    public void SubmitForm()
	    {
            HtmlFormElement.submit();

	    }

	    public void SetFileUploadFile(Element element, string fileName)
	    {
			var uploadDialogHandler = new FileUploadDialogHandler(fileName);
            using (new UseDialogOnce(element.DomContainer.DialogWatcher, uploadDialogHandler))
            {
                element.Click();
            }
	    }

	    private IHTMLFormElement HtmlFormElement
        {
            get { return (IHTMLFormElement)_element; }
        }

	    /// <summary>
        /// This methode can be used if the attribute isn't available as a property of
        /// of this <see cref="Style"/> class.
        /// </summary>
        /// <param name="attributeName">The attribute name. This could be different then named in
        /// the HTML. It should be the name of the property exposed by IE on it's style object.</param>
        /// <returns>The value of the attribute if available; otherwise <c>null</c> is returned.</returns>
        public string GetStyleAttributeValue(string attributeName)
        {
            if (UtilityClass.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException("attributeName", "Null or Empty not allowed.");
            }

            var attributeValue = GetStyleAttributeValue(attributeName, htmlElement.style);

            if (attributeValue == DBNull.Value || attributeValue == null)
            {
                return null;
            }

	        var stringAttributeValue = attributeValue.ToString();

	        if (attributeName == "cssText" && !stringAttributeValue.TrimEnd(Char.Parse(" ")).EndsWith(";"))
	        {
	            stringAttributeValue = stringAttributeValue.ToLowerInvariant() + ";";
	        }

            return stringAttributeValue;
        }

        internal static object GetStyleAttributeValue(string attributeName, IHTMLStyle style)
        {
            attributeName = UtilityClass.TurnStyleAttributeIntoProperty(attributeName);

            return style.getAttribute(attributeName, 0);
        }

        public void SetStyleAttributeValue(string attributeName, string value)
        {
            attributeName = UtilityClass.TurnStyleAttributeIntoProperty(attributeName);

            htmlElement.style.setAttribute(attributeName, value, 0);
        }

		protected IHTMLElement htmlElement
		{
			get { return (IHTMLElement) _element; }
		}

		private IHTMLElement2 htmlElement2
		{
			get { return (IHTMLElement2) _element; }
		}

		private IHTMLDOMNode domNode
		{
			get { return (IHTMLDOMNode) _element; }
		}

		/// <summary>
		/// Gets the DispHtmlBaseElement />.
		/// </summary>
		/// <value>The DispHtmlBaseElement.</value>
		private DispHTMLBaseElement DispHtmlBaseElement
		{
			get { return (DispHTMLBaseElement) _element; }
		}

		public object Object
		{
			get { return _element; }
		}

	    public IAttributeBag GetAttributeBag(DomContainer domContainer)
	    {
	        if (_attributeBag == null)
	        {
	            _attributeBag = new ElementAttributeBag(domContainer);
	        }
	        _attributeBag.INativeElement = this;
	        return _attributeBag;
	    }

	    public bool IsElementReferenceStillValid()
		{
			try
			{
				if (htmlElement.sourceIndex < 0)
				{
					return false;
				}
			    
                return htmlElement.offsetParent != null;
			}
			catch
			{
				return false;
			}
		}

		public string TagName
		{
			get { return htmlElement.tagName; }
		}
	}

    public class AsyncScriptRunner
    {
        private readonly string _scriptCode;
        private readonly IHTMLWindow2 _window;

        public AsyncScriptRunner(string scriptCode, IHTMLWindow2 window)
        {
            _scriptCode = scriptCode;
            _window = window;
        }

        public void FireEvent()
        {
            UtilityClass.RunScript(_scriptCode, _window);
        }
    }
}
