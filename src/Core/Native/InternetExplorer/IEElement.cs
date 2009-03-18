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
        public IHTMLElement AsHtmlElement
        {
            get { return (IHTMLElement)_element; }
        }

        /// <inheritdoc />
        public INativeElementCollection Children
        {
            get { return new IEElementCollection((IHTMLElementCollection)AsHtmlElement.children); }
        }

        /// <inheritdoc />
        public INativeElementCollection AllDescendants
        {
            get { return new IEElementCollection((IHTMLElementCollection)AsHtmlElement.all); }
        }

        /// <inheritdoc />
        public INativeElementCollection TableRows
        {
            get
            {
                IHTMLTable htmlTable = AsHtmlElement as IHTMLTable;
                if (htmlTable != null)
                    return new IEElementCollection(htmlTable.rows);

                IHTMLTableSection htmlTableSection = AsHtmlElement as IHTMLTableSection;
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
                IHTMLTable htmlTable = AsHtmlElement as IHTMLTable;
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
                IHTMLTableRow htmlTableRow = AsHtmlElement as IHTMLTableRow;
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
                var htmlSelectElement = AsHtmlElement as IHTMLSelectElement;
                if (htmlSelectElement != null)
                    return new IEElementCollection(htmlSelectElement);

                throw new InvalidOperationException("The element must be a SELECT.");
            }
        }

        /// <inheritdoc />
        public string TextAfter
        {
            get { return AsHtmlElement2.getAdjacentText("afterEnd"); }
        }

        /// <inheritdoc />
        public string TextBefore
        {
            get { return AsHtmlElement2.getAdjacentText("beforeBegin"); }
        }

        /// <inheritdoc />
        public INativeElement NextSibling
        {
            get
            {
                var node = AsDomNode.nextSibling;
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
                var node = AsDomNode.previousSibling;
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
                var parentNode = AsDomNode.parentNode as IHTMLElement;
                return parentNode != null ? new IEElement(parentNode) : null;
            }
        }

        /// <inheritdoc />
        public string GetAttributeValue(string attributeName)
        {
            if (attributeName.ToLowerInvariant() == "tagname") return TagName;

            var attributeValue = AsHtmlElement.getAttribute(attributeName, 0);

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
            var ihtmlElement4 = AsHtmlElement as IHTMLElement4;
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

            AsHtmlElement.setAttribute(attributeName, value, 0);
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
            AsDispHTMLBaseElement.click();
        }

        /// <inheritdoc />
        public void SetFocus()
        {
            AsDispHTMLBaseElement.focus();
        }

        /// <inheritdoc />
        public void FireEvent(string eventName, NameValueCollection eventProperties)
        {
            if (eventProperties == null)
            {
                IEUtils.FireEvent(AsDispHTMLBaseElement, eventName);
            }
            else
            {
                if (eventName == "onKeyPress")
                {
                    var addChar = eventProperties.GetValues("keyCode")[0];
                    var newValue = GetAttributeValue("value") + ((char)int.Parse(addChar));
                    SetAttributeValue("value", newValue);
                }

                IEUtils.FireEvent(AsDispHTMLBaseElement, eventName, eventProperties);
            }
        }

        /// <inheritdoc />
        public void FireEventNoWait(string eventName, NameValueCollection eventProperties)
        {
            var scriptCode = IEUtils.CreateJavaScriptFireEventCode(eventProperties, AsDispHTMLBaseElement, eventName);
            var window = ((IHTMLDocument2)AsDispHTMLBaseElement.document).parentWindow;

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
            AsHtmlFormElement.submit();
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

        private IHTMLFormElement AsHtmlFormElement
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

            var attributeValue = GetStyleAttributeValue(attributeName, AsHtmlElement.style);

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

            AsHtmlElement.style.setAttribute(attributeName, value, 0);
        }

        private IHTMLElement2 AsHtmlElement2
        {
            get { return (IHTMLElement2)_element; }
        }

        private IHTMLDOMNode AsDomNode
        {
            get { return (IHTMLDOMNode)_element; }
        }

        /// <summary>
        /// Gets the DispHtmlBaseElement />.
        /// </summary>
        /// <value>The DispHtmlBaseElement.</value>
        private DispHTMLBaseElement AsDispHTMLBaseElement
        {
            get { return (DispHTMLBaseElement)_element; }
        }

        /// <inheritdoc />
        public bool IsElementReferenceStillValid()
        {
            try
            {
                if (AsHtmlElement.sourceIndex < 0)
                {
                    return false;
                }

                return AsHtmlElement.offsetParent != null;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc />
        public string TagName
        {
            get { return AsHtmlElement.tagName; }
        }

        /// <inheritdoc />
        public void WaitUntilReady()
        {
            WaitUntilElementAvailable();

            if (ElementTag.IsMatch(ElementFactory.GetElementTags<Image>(), this))
            {
                return;
            }

            // Wait as long as the readystate of an element is BETWEEN
            // Uninitialized and Complete. If it's uninitialized,
            // it's quite probable that it will never reach Complete.
            // Like for elements that could not load an image or icon
            // or some other bits not part of the HTML page.     
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(Settings.WaitForCompleteTimeOut);
            var ihtmlElement2 = AsHtmlElement2;
            var success = tryActionUntilTimeOut.Try(() =>
            {
                var readyState = ihtmlElement2.readyStateValue;
                return (readyState == 0 || readyState == 4);
            });

            if (success) return;

            throw new WatiNException(String.Format("Element didn't reach readystate = complete within {0} seconds: {1}", Settings.WaitForCompleteTimeOut, AsHtmlElement.outerText));
        }

        private void WaitUntilElementAvailable()
        {
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(Settings.WaitForCompleteTimeOut);
            var ihtmlElement = AsHtmlElement;
            var success = tryActionUntilTimeOut.Try(() =>
            {
                var tagName = ihtmlElement.tagName;
                return true;
            });

            if (success) return;

            throw new WatiNException(String.Format("Element wasn't available within {0} seconds.", Settings.WaitForCompleteTimeOut));
        }

        /// <inheritdoc />
	    public Rectangle GetElementBounds()
	    {
            return GetHtmlElementBounds(AsHtmlElement);
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

        public override bool Equals(object obj)
        {
            var ieElement = obj as IEElement;

            return ieElement != null && Equals(ieElement.AsDispHTMLBaseElement.uniqueNumber, AsDispHTMLBaseElement.uniqueNumber);
        }

        public override int GetHashCode()
        {
            return AsDispHTMLBaseElement.uniqueNumber;
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
