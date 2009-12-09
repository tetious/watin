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
using System.Text;
using mshtml;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native.InternetExplorer
{
	/// <summary>
	/// Summary description for IEElement.
	/// </summary>
    public class IEElement : INativeElement
    {
        private const string CSSTEXT = "cssText";

        private readonly object _element;
	    private string _javascriptElementReference;

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
                var htmlTable = AsHtmlElement as IHTMLTable;
                if (htmlTable != null)
                {
                    return new IEElementCollection(htmlTable.rows);
                }

                var htmlTableSection = AsHtmlElement as IHTMLTableSection;
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
                var htmlTable = AsHtmlElement as IHTMLTable;
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
                var htmlTableRow = AsHtmlElement as IHTMLTableRow;
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

            object attributeValue;
            try
            {
                attributeValue = GetWithFailOver(() => AsHtmlElement.getAttribute(attributeName, 0));
            }
            catch
            {
                Logger.LogDebug("Getting attribute: {0}", attributeName);
                throw;
            }
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

        private static T GetWithFailOver<T>(DoFunc<T> func)
        {
            return UtilityClass.TryFuncFailOver(func, 5, 50);
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

            var asyncScriptRunner = new AsyncScriptRunner(scriptCode.ToString(), ParentWindow);

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

            var attributeValue = GetStyleAttributeValueInternal(attributeName);

            if (attributeValue == DBNull.Value || attributeValue == null)
            {
                return null;
            }

            var stringAttributeValue = attributeValue.ToString();

            if (attributeName == CSSTEXT && !stringAttributeValue.TrimEnd(Char.Parse(" ")).EndsWith(";"))
            {
                stringAttributeValue = stringAttributeValue.ToLowerInvariant() + ";";
            }

            return stringAttributeValue;
        }

	    private object GetStyleAttributeValueInternal(string attributeName)
        {
            attributeName = UtilityClass.TurnStyleAttributeIntoProperty(attributeName);

            if (attributeName == CSSTEXT)
                return GetWithFailOver(() => AsHtmlElement.style.getAttribute(attributeName, 0));

            return GetWithFailOver(() => AsHtmlElement2.currentStyle.getAttribute(attributeName, 0));
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
                if (AsHtmlElement.sourceIndex < 0) return false;

                // Note: We exclude elements that might appear as root elements from this check since we cannot verify them.
                if (TagName == "!" || TagName == "HTML") return true;

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
            var tryFuncUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(Settings.WaitForCompleteTimeOut));
            var ihtmlElement2 = AsHtmlElement2;
            var success = tryFuncUntilTimeOut.Try(() =>
            {
                var readyState = ihtmlElement2.readyStateValue;
                return (readyState == 0 || readyState == 4);
            });

            if (success) return;

            throw new WatiNException(String.Format("Element didn't reach readystate = complete within {0} seconds: {1}", Settings.WaitForCompleteTimeOut, AsHtmlElement.outerText));
        }

        private void WaitUntilElementAvailable()
        {
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(Settings.WaitForCompleteTimeOut));
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

        /// <inheritdoc />
        public string GetJavaScriptElementReference()
	    {
            if (string.IsNullOrEmpty(_javascriptElementReference))
            {
                _javascriptElementReference = IEUtils.IEVariableNameHelper.CreateVariableName();
            }

            var originalId = GetWithFailOver(() => AsHtmlElement.id);
            AsHtmlElement.id = _javascriptElementReference;

            var scriptCode = new StringBuilder();
            scriptCode.Append(string.Format("var {0} = document.getElementById('{0}');", _javascriptElementReference));
            scriptCode.Append(string.Format("{0}.id = '{1}';", _javascriptElementReference, originalId));

            IEUtils.RunScript(scriptCode, ParentWindow);

	        return _javascriptElementReference;
	    }

	    private IHTMLWindow2 ParentWindow
	    {
            get { return ((IHTMLDocument2)AsHtmlElement.document).parentWindow; }
	    }

	    internal static Rectangle GetHtmlElementBounds(IHTMLElement element)
        {
            var left = element.offsetLeft;
            var top = element.offsetTop;

            var parentElement = element.parentElement;
            while (parentElement != null)
            {
                left += parentElement.offsetLeft;
                top += parentElement.offsetTop;
                parentElement = parentElement.parentElement;
            }

            var width = element.offsetWidth / 2; // n.b. not sure why we are dividing by 2 -- JB
            var height = element.offsetHeight / 2;

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
}
