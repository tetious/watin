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
using System.Drawing;
using mshtml;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native.InternetExplorer
{
	/// <summary>
	/// Summary description for IEElement.
	/// </summary>
    public class IEElement : INativeElement
    {
        private readonly object _element;

        public IEElement(object element)
        {
            if (element == null) throw new ArgumentNullException("element");
            if (element is INativeElement) throw new Exception("INativeElement not allowed");
            _element = element;
        }

        /// <summary>
        /// Gets the underlying <see cref="IHTMLElement" /> object.
        /// </summary>
        public IHTMLElement HtmlElement
        {
            get { return (IHTMLElement)_element; }
        }

        /// <inheritdoc />
        public INativeElementCollection Children
        {
            get { return new IEElementCollection((IHTMLElementCollection)HtmlElement.children); }
        }

        /// <inheritdoc />
        public INativeElementCollection AllDescendants
        {
            get { return new IEElementCollection((IHTMLElementCollection)HtmlElement.all); }
        }

        /// <inheritdoc />
        public INativeElementCollection TableRows
        {
            get
            {
                IHTMLTable htmlTable = HtmlElement as IHTMLTable;
                if (htmlTable != null)
                    return new IEElementCollection(htmlTable.rows);

                IHTMLTableSection htmlTableSection = HtmlElement as IHTMLTableSection;
                if (htmlTableSection != null)
                    return new IEElementCollection(htmlTableSection.rows);

                throw new InvalidOperationException("The element must be a TABLE or TBODY.");
            }
        }

        /// <inheritdoc />
        public INativeElementCollection TableBodies
        {
            get
            {
                IHTMLTable htmlTable = HtmlElement as IHTMLTable;
                if (htmlTable != null)
                    return new IEElementCollection(htmlTable.tBodies);

                throw new InvalidOperationException("The element must be a TABLE.");
            }
        }

        /// <inheritdoc />
        public INativeElementCollection TableCells
        {
            get
            {
                IHTMLTableRow htmlTableRow = HtmlElement as IHTMLTableRow;
                if (htmlTableRow != null)
                    return new IEElementCollection(htmlTableRow.cells);

                throw new InvalidOperationException("The element must be a TR.");
            }
        }

        /// <inheritdoc />
        public INativeElementCollection Options
        {
            get
            {
                var htmlSelectElement = HtmlElement as IHTMLSelectElement;
                if (htmlSelectElement != null)
                    return new IEElementCollection(htmlSelectElement);

                throw new InvalidOperationException("The element must be a SELECT.");
            }
        }

        /// <inheritdoc />
        public string TextAfter
        {
            get { return HtmlElement2.getAdjacentText("afterEnd"); }
        }

        /// <inheritdoc />
        public string TextBefore
        {
            get { return HtmlElement2.getAdjacentText("beforeBegin"); }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public INativeElement Parent
        {
            get
            {
                var parentNode = domNode.parentNode as IHTMLElement;
                return parentNode != null ? new IEElement(parentNode) : null;
            }
        }

        /// <inheritdoc />
        public string GetAttributeValue(string attributeName)
        {
            if (attributeName.ToLowerInvariant() == "tagname") return TagName;

            var attributeValue = HtmlElement.getAttribute(attributeName, 0);

            if (DidReturnObjectReference(attributeValue))
            {
                attributeValue = RetrieveNodeValue(attributeName);
            }

            if (attributeValue == DBNull.Value || attributeValue == null)
            {
                return null;
            }

            return attributeValue.ToString();
        }

        private object RetrieveNodeValue(string attributeName)
        {
            var ihtmlElement4 = HtmlElement as IHTMLElement4;
            return ihtmlElement4 == null ? null : ihtmlElement4.getAttributeNode(attributeName).nodeValue;
        }

        private static bool DidReturnObjectReference(object attributeValue)
        {
            if (attributeValue == null) return false;
            return attributeValue.GetType().ToString() == "System.__ComObject";
        }

        /// <inheritdoc />
        public void SetAttributeValue(string attributeName, string value)
        {
            value = HandleAttributesWhichHaveNoValuePart(attributeName, value);

            HtmlElement.setAttribute(attributeName, value, 0);
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

        /// <inheritdoc />
        public void ClickOnElement()
        {
            DispHtmlBaseElement.click();
        }

        /// <inheritdoc />
        public void SetFocus()
        {
            DispHtmlBaseElement.focus();
        }

        /// <inheritdoc />
        public void FireEvent(string eventName, NameValueCollection eventProperties)
        {
            if (eventProperties == null)
            {
                IEUtils.FireEvent(DispHtmlBaseElement, eventName);
            }
            else
            {
                if (eventName == "onKeyPress")
                {
                    var addChar = eventProperties.GetValues("keyCode")[0];
                    var newValue = GetAttributeValue("value") + ((char)int.Parse(addChar));
                    SetAttributeValue("value", newValue);
                }

                IEUtils.FireEvent(DispHtmlBaseElement, eventName, eventProperties);
            }
        }

        /// <inheritdoc />
        public void FireEventNoWait(string eventName, NameValueCollection eventProperties)
        {
            var scriptCode = IEUtils.CreateJavaScriptFireEventCode(eventProperties, DispHtmlBaseElement, eventName);
            var window = ((IHTMLDocument2)DispHtmlBaseElement.document).parentWindow;

            var asyncScriptRunner = new AsyncScriptRunner(scriptCode.ToString(), window);

            UtilityClass.AsyncActionOnBrowser(asyncScriptRunner.FireEvent);
        }

        /// <inheritdoc />
        public void Select()
        {
            var input = _element as IHTMLInputElement;
            if (input != null)
            {
                input.select();
                FireEvent("onSelect", null);
                
                return;
            }
            var textarea = _element as IHTMLTextAreaElement;
            if (textarea != null)
            {
                textarea.select();
                FireEvent("onSelect", null);

                return;
            }

            throw new WatiNException("Select not supported on " + _element.GetType());
        }

        /// <inheritdoc />
        public void SubmitForm()
        {
            HtmlFormElement.submit();
        }

        /// <inheritdoc />
        public void SetFileUploadFile(DialogWatcher dialogWatcher, string fileName)
        {
            var uploadDialogHandler = new FileUploadDialogHandler(fileName);
            using (new UseDialogOnce(dialogWatcher, uploadDialogHandler))
            {
                ClickOnElement();
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

            var attributeValue = GetStyleAttributeValue(attributeName, HtmlElement.style);

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

        /// <inheritdoc />
        public void SetStyleAttributeValue(string attributeName, string value)
        {
            attributeName = UtilityClass.TurnStyleAttributeIntoProperty(attributeName);

            HtmlElement.style.setAttribute(attributeName, value, 0);
        }

        private IHTMLElement2 HtmlElement2
        {
            get { return (IHTMLElement2)_element; }
        }

        private IHTMLDOMNode domNode
        {
            get { return (IHTMLDOMNode)_element; }
        }

        /// <summary>
        /// Gets the DispHtmlBaseElement />.
        /// </summary>
        /// <value>The DispHtmlBaseElement.</value>
        private DispHTMLBaseElement DispHtmlBaseElement
        {
            get { return (DispHTMLBaseElement)_element; }
        }

        /// <inheritdoc />
        public bool IsElementReferenceStillValid()
        {
            try
            {
                if (HtmlElement.sourceIndex < 0)
                {
                    return false;
                }

                return HtmlElement.offsetParent != null;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc />
        public string TagName
        {
            get { return HtmlElement.tagName; }
        }

        /// <inheritdoc />
        public void WaitUntilReady()
        {
            //TODO: See if this method could be dropped, it seems to give
            //      more trouble (uninitialized state of elements)
            //      then benefits (I just introduced this method to be on 
            //      the save side)

            if (ElementTag.IsMatch(ElementFactory.GetElementTags<Image>(), this))
            {
                return;
            }

            // Wait if the readystate of an element is BETWEEN
            // Uninitialized and Complete. If it's uninitialized,
            // it's quite probable that it will never reach Complete.
            // Like for elements that could not load an image or ico
            // or some other bits not part of the HTML page.     
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(30);
            var ihtmlElement2 = ((IHTMLElement2)_element);
            var success = tryActionUntilTimeOut.Try(() =>
            {
                var readyState = ihtmlElement2.readyStateValue;
                return (readyState == 0 || readyState == 4);
            });

            if (success) return;

            var ihtmlElement = ((IHTMLElement)_element);
            throw new WatiNException("Element didn't reach readystate = complete within 30 seconds: " + ihtmlElement.outerText);
        }

        /// <inheritdoc />
	    public Rectangle GetElementBounds()
	    {
            return GetHtmlElementBounds(HtmlElement);
	    }

        internal static Rectangle GetHtmlElementBounds(IHTMLElement element)
        {
            int left = element.offsetLeft;
            int top = element.offsetTop;

            IHTMLElement parentElement = element.parentElement;
            while (parentElement != null)
            {
                left += parentElement.offsetLeft;
                top += parentElement.offsetTop;
                parentElement = parentElement.parentElement;
            }

            int width = element.offsetWidth / 2; // n.b. not sure why we are dividing by 2 -- JB
            int height = element.offsetHeight / 2;

            return new Rectangle(left, top, width, height);
        }
    }

    internal class AsyncScriptRunner
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
            IEUtils.RunScript(_scriptCode, _window);
        }
    }
}
