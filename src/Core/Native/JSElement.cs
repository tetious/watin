#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using WatiN.Core.DialogHandlers;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native
{
    using Mozilla;

    public class JSElement : INativeElement
    {
        public delegate T DoFuncWithValue<T>(string value);

        private const int NodeType_Text = 3;

        /// <summary>
        /// List of html attributes that have to be retrieved as properties in order to get the correct value.
        /// I.e. for options myOption.getAttribute("selected"); returns nothing if it's selected. 
        /// However  myOption.selected returns true.
        /// </summary>
        public static readonly IList<string> UsePropertyInsteadOfAttribute = new[]
                                                                                  {
                                                                                          "selected", "textContent", "className", "checked", "readOnly", "multiple", "value",
                                                                                          "nodeType", "innerHTML", "baseURI", "src", "href", "rowIndex", "cellIndex"
                                                                                  };

        /// <summary>
        /// Mappings from attributnames used by WatiN to attribute/property names used by FireFox
        /// </summary>
        public static readonly Dictionary<string, string> WatiNAttributeMap = new Dictionary<string, string>
                                                                                  {
                                                                                          {Find.innerTextAttribute, "textContent"}, {Find.forAttribute, "for"}
                                                                                  };

        /// <summary>
        /// Mappings from attributnames used by WatiN to attribute/property names used by FireFox
        /// </summary>
        public static readonly Dictionary<string, DoFuncWithValue<string>> SetPropertyTransformations = new Dictionary<string, DoFuncWithValue<string>>
                                                                                                            {
                                                                                                                    {"value", value =>
                                                                                                                          {
                                                                                                                              var escapedValue = value.Replace("'","\\'");
                                                                                                                              return "'" + escapedValue + "'";
                                                                                                                          }
                                                                                                                    }
                                                                                                            };

        private Dictionary<string, object> _attributeCache;

        public JSElement(ClientPortBase clientPort, string elementReference)
        {
            if (clientPort == null)
                throw new ArgumentNullException("clientPort");
            if (elementReference == null)
                throw new ArgumentNullException("elementReference");

            ClientPort = clientPort;
            ElementReference = elementReference;
        }

        /// <summary>
        /// Gets the FireFox client port.
        /// </summary>
        public ClientPortBase ClientPort { get; private set; }

        /// <summary>
        /// Gets the name of a variable that stores a reference to the element within FireFox.
        /// </summary>
        public string ElementReference { get; private set; }

        public string TextContent
        {
            get
            {
                var propertyName = "textContent";
                if (!IsTextNodeType) propertyName = "innerHTML";

                var propertyValue = GetProperty(propertyName);

                propertyValue = (propertyName == "innerHTML" ? InnerHtmlToInnerText(propertyValue) : NewLineCleanup(propertyValue));

                return string.IsNullOrEmpty(propertyValue) ? null : propertyValue;
            }
        }

        public string InputType
        {
            get
            {
                return GetFromAttributeCache("Type", () =>
                                                     {
                                                         var value = GetAttribute("type");
                                                         return value != null ? value.ToLower() : "text";
                                                     });
            }
        }

        public string OuterHtml
        {
            get
            {
                var clone = ClientPort.CreateVariableName();
                var div = ClientPort.CreateVariableName();
                var outerHtml = ClientPort.CreateVariableName();

                var command = string.Format("{0}={1}.cloneNode(true);", clone, ElementReference);
                command += string.Format("{0}={1}.ownerDocument.createElement('div');", div, ElementReference);
                command += string.Format("{0}.appendChild({1});", div, clone);
                command += string.Format("{0}={1}.innerHTML;", outerHtml, div);
                command += string.Format("{0}=null;{1}=null;", div, clone);
                command += string.Format("{0};", outerHtml);

                var result = ClientPort.WriteAndRead(command);

                // remove all \n (newline) and any following spaces
                var newlineSpaces = new Regex("\n *");
                result = newlineSpaces.Replace(result, "");

                return "\r\n" + result;
            }
        }

        public bool IsTextNodeType
        {
            get
            {
                return GetFromAttributeCache("IsTextNodeType", () =>
                                                               {
                                                                   var nodeTypeValue = GetProperty("nodeType");
                                                                   return Convert.ToInt32(nodeTypeValue) == NodeType_Text;
                                                               });
            }
        }

        #region INativeElement Members

        public INativeElementCollection Children
        {
            get { return new JSElementArray(ClientPort, FireFoxClientPort.GetChildElementsFunctionName + "(" + ElementReference +")"); }
        }

        public INativeElementCollection AllDescendants
        {
            get { return new FFElementCollection(ClientPort, ElementReference); }
        }

        public INativeElementCollection TableRows
        {
            get { return new JSElementArray(ClientPort, ElementReference + ".rows"); }
        }

        public INativeElementCollection TableBodies
        {
            get { return new JSElementArray(ClientPort, ElementReference + ".tBodies"); }
        }

        public INativeElementCollection TableCells
        {
            get { return new JSElementArray(ClientPort, ElementReference + ".cells"); }
        }

        public INativeElementCollection Options
        {
            get { return new JSElementArray(ClientPort, ElementReference + ".options"); }
        }

        /// <summary>
        /// This not supported in FireFox
        /// </summary>
        public string TextAfter
        {
            get
            {
                var element = GetElementByProperty("nextSibling");

                if (element == null || !element.IsTextNodeType) return string.Empty;

                return element.GetAttributeValue("textContent");
            }
        }

        /// <summary>
        /// This not supported in FireFox
        /// </summary>
        public string TextBefore
        {
            get
            {
                var element = GetElementByProperty("previousSibling");

                if (element == null || !element.IsTextNodeType) return string.Empty;

                return element.GetAttributeValue("textContent");
            }
        }

        public INativeElement NextSibling
        {
            get
            {
                var element = GetElementByProperty("nextSibling");

                while (true)
                {
                    if (element == null || !element.IsTextNodeType)
                    {
                        return element;
                    }

                    element = (JSElement)element.NextSibling;
                }
            }
        }

        public INativeElement PreviousSibling
        {
            get
            {
                var element = GetElementByProperty("previousSibling");

                while (true)
                {
                    if (element == null || !element.IsTextNodeType)
                    {
                        return element;
                    }

                    element = (JSElement)element.PreviousSibling;
                }
            }
        }

        public INativeElement Parent
        {
            get { return GetElementByProperty("parentNode"); }
        }

        public string GetAttributeValue(string attributeName)
        {
            // Translate to FireFox dom syntax
            if (WatiNAttributeMap.ContainsKey(attributeName))
            {
                attributeName = WatiNAttributeMap[attributeName];
            }

            // Special cases
            var attribName = attributeName.ToLowerInvariant();
            if (attribName == "tagname") return TagName;
            if (attribName == "type") return InputType;
            if (attribName == "outerhtml") return OuterHtml;
            if (attributeName == "textContent") return TextContent;
            if (attributeName == "disabled") return IsDisabled;

            // Return value
            return UsePropertyInsteadOfAttribute.Contains(attributeName) ? GetProperty(attributeName) : GetAttribute(attributeName);
        }

        public string IsDisabled
        {
            get 
            { 
                var result = GetAttribute("disabled");
                if (string.IsNullOrEmpty(result))
                {
                    result = GetProperty("disabled");
                }
                return result;
            }
        }

        public void SetAttributeValue(string attributeName, string value)
        {
            // Translate to FireFox html syntax
            if (WatiNAttributeMap.ContainsKey(attributeName))
            {
                attributeName = WatiNAttributeMap[attributeName];
            }

            // Handle properties different from attributes
            if (UsePropertyInsteadOfAttribute.Contains(attributeName))
            {
                SetProperty(attributeName, value);
                return;
            }

            SetAttribute(attributeName, value);
        }

        public string GetStyleAttributeValue(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException("attributeName");
            }

            attributeName = UtilityClass.TurnStyleAttributeIntoProperty(attributeName);

            string attributeValue;
            if (attributeName == "cssText")
            {
                attributeValue = GetProperty("style." + attributeName);
            }
            else
            {
                attributeValue = ClientPort.WriteAndRead("{2}.defaultView.getComputedStyle({0},null).{1};", ElementReference, attributeName, ClientPort.DocumentVariableName);
            }
            
            return string.IsNullOrEmpty(attributeValue) ? null : attributeValue;
        }

        public void SetStyleAttributeValue(string attributeName, string value)
        {
            attributeName = UtilityClass.TurnStyleAttributeIntoProperty(attributeName);
            SetProperty("style." + attributeName, "'" + value + "'");
        }

        public void SetFocus()
        {
            ExecuteMethod("focus");
        }

        public void FireEvent(string eventName, NameValueCollection eventProperties)
        {
            ExecuteEvent(eventName, eventProperties, true);
        }

        public void FireEventNoWait(string eventName, NameValueCollection eventProperties)
        {
            ExecuteEvent(eventName, eventProperties, false);
        }

        public bool IsElementReferenceStillValid()
        {
            if (UtilityClass.IsNullOrEmpty(ElementReference))
                return false;

            // Note: We exclude elements that might appear as root elements from this check since we cannot verify them.
            const string excludedTags = "! HTML HEAD BODY TITLE LINK META SCRIPT STYLE BASE";
            var command = string.Format("({0} != undefined && {0} != null) && ({0}.offsetParent != null || '{1}'.indexOf({0}.tagName.toUpperCase())> -1); ", ElementReference, excludedTags);

            return ClientPort.WriteAndReadAsBool(command);
        }

        public string TagName
        {
            get { return GetFromAttributeCache("tagName", () => GetProperty(Find.tagNameAttribute)); }
            internal set { AddToAttributeCache("tagName", value);}
        }

        public void Select()
        {
            FireEvent("select", null);
        }

        public void SubmitForm()
        {
            FireEvent("submit", null);
        }

        public void SetFileUploadFile(DialogWatcher dialogWatcher, string fileName)
        {
            fileName = fileName.Replace(@"\", @"\\");
            SetAttributeValue("value", fileName);
        }

        #endregion

        public string GetAttribute(string attributeName)
        {
            var getAttributeWrite = string.Format("{0}.getAttribute(\"{1}\");", ElementReference, attributeName);
            return ClientPort.WriteAndRead(getAttributeWrite);
        }

        public void SetAttribute(string attributeName, string value)
        {
            ClientPort.Write("{0}.setAttribute(\"{1}\", \"{2}\");", ElementReference, attributeName, value);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public string GetProperty(string propertyName)
        {
            var command = string.Format("{0}.{1};", ElementReference, propertyName);
            return ClientPort.WriteAndRead(command);
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void SetProperty(string propertyName, string value)
        {
            if (SetPropertyTransformations.ContainsKey(propertyName)) value = SetPropertyTransformations[propertyName].Invoke(value);
            
            var command = string.Format("{0}.{1} = {2};", ElementReference, propertyName, value);
            ClientPort.Write(command);
        }

        /// <summary>
        /// Executes a method with no parameters.
        /// </summary>
        /// <param name="methodName">Name of the method to execute.</param>
        public void ExecuteMethod(string methodName)
        {
            ClientPort.Write("{0}.{1}();", ElementReference, methodName);
        }

        /// <summary>
        /// Gets the element by property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Returns the element that is returned by the specified property</returns>
        public JSElement GetElementByProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            var elementvar = ClientPort.CreateVariableName();
            var command = string.Format("{0}={1}.{2}; {0}!=null;", elementvar, ElementReference, propertyName);
            var exists = ClientPort.WriteAndReadAsBool(command);

            return exists ? new JSElement(ClientPort, elementvar) : null;
        }

        public void WaitUntilReady()
        {
            // TODO: Is this needed for FireFox?
        }

        /// <inheritdoc />
        public Rectangle GetElementBounds()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string GetJavaScriptElementReference()
        {
            return ElementReference;
        }

        public override bool Equals(object obj)
        {
            var jsElement = obj as JSElement;
            return jsElement != null && IsReferenceEqual(jsElement.ElementReference, ElementReference);
        }

        private bool IsReferenceEqual(string left, string right)
        {
            return ClientPort.WriteAndReadAsBool("{0}=={1};", left, right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Makes innerHtml inner text (IE) look a like. It comes close but it seems not to cover all
        /// conversions cause comparing browser.body.innertext between a IE and FireFox instances will 
        /// certainly fail on newlines and maybe some spaces.
        /// </summary>
        /// <param name="innerHtml">The value.</param>
        /// <returns></returns>
        private string InnerHtmlToInnerText(string innerHtml)
        {
            if (string.IsNullOrEmpty(innerHtml)) return string.Empty;

            if (TagName.ToLowerInvariant() == "pre") return innerHtml;

            var returnValue = NewLineCleanup(innerHtml);

            // remove all but the last param tag by \r\n
            var param = new Regex("</p>");
            var matches = param.Matches(returnValue);
            if (matches.Count > 1)
            {
                returnValue = param.Replace(returnValue, "\r\n", matches.Count - 1);
            }

            // remove all br tags and following space with \r\n
            var br = new Regex(@"<br((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?> *");
            returnValue = br.Replace(returnValue, "\r\n");

            // remove all hr tags and following space with \r\n
            var hr = new Regex(@"<hr((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?> *");
            returnValue = hr.Replace(returnValue, "\r\n");

            // remove tags (beginning and end tags)
            var tag = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>");
            returnValue = tag.Replace(returnValue, "");

            // remove comment
            tag = new Regex("<!--.*-->");
            returnValue = tag.Replace(returnValue, "");

            // replace multiple spaces by one space
            var moreThanOneSpace = new Regex(" +");
            returnValue = moreThanOneSpace.Replace(returnValue, " ");

            //Decode HTML entities left in the string
            returnValue = System.Web.HttpUtility.HtmlDecode(returnValue);

            //Replace the nbsp character with regular spaces
            returnValue = returnValue.Replace((char)0xa0, (char)0x20);

            // remove spaces at the beginning of the text
            return returnValue.TrimStart();
        }

        private static string NewLineCleanup(string innerHtml)
        {
            // remove all \n (newline) and any following spaces
            var newlineSpaces = new Regex("\r\n *");
            var returnValue = newlineSpaces.Replace(innerHtml, "");
            
            // remove all \n (newline) and any following spaces
            var simpleNewlineSpaces = new Regex("\n *");
            returnValue = simpleNewlineSpaces.Replace(returnValue, "");
            return returnValue;
        }

        private T GetFromAttributeCache<T>(string key, DoFunc<T> function)
        {
            AddToAttributeCache(key, function);
            return (T)AttributeCache[key];
        }

        private void AddToAttributeCache<T>(string key, DoFunc<T> function)
        {
            if (!AttributeCache.ContainsKey(key))
            {
                AttributeCache.Add(key, function.Invoke());
            }
        }

        private void AddToAttributeCache(string key, object value)
        {
            if (!AttributeCache.ContainsKey(key))
            {
                AttributeCache.Add(key, value);
            }
        }

        protected Dictionary<string, object> AttributeCache
        {
            get
            {
                if (_attributeCache == null) _attributeCache = new Dictionary<string, object>();
                return _attributeCache;
            }
        }

        /// <summary>
        /// Executes the event.
        /// </summary>
        /// <param name="eventName">Name of the event to fire.</param>
        /// <param name="eventProperties"></param>
        /// <param name="WaitForEventToComplete"></param>
        private void ExecuteEvent(string eventName, NameValueCollection eventProperties, bool WaitForEventToComplete)
        {
            var creator = new JSEventCreator(ElementReference, ClientPort);
            var command = creator.CreateEvent(eventName, eventProperties, WaitForEventToComplete);

            ClientPort.WriteAndReadAsBool(command);
            Thread.Sleep(30);
        }

        /// <summary>
        /// This changes/pins the java script variable name <see cref="ElementReference"/> which is used to 
        /// execute commands on FireFox. 
        /// </summary>
        public void Pin()
        {
            var elementVariableName = ClientPort.CreateVariableName();
            ClientPort.Write("{0}={1};", elementVariableName, ElementReference);

            ElementReference = elementVariableName;
        }
    }

    public class JSEventCreator
    {
        private readonly string _elementReference;
        private readonly ClientPortBase _clientPortBase;

        public JSEventCreator(string elementReference, ClientPortBase clientPortBase)
        {
            _elementReference = elementReference;
            _clientPortBase = clientPortBase;
        }

        public string CreateEvent(string eventName, NameValueCollection eventProperties, bool WaitForEventToComplete)
        {
            // See http://www.howtocreate.co.uk/tutorials/javascript/domevents
            // for more info about manually firing events
            var eventname = CleanupEventName(eventName);

            string command;

            if (eventname.Contains("mouse") || eventname == "click")
            {
                command = CreateMouseEventCommand(eventname, eventProperties);
            }
            else if (eventname.Contains("key"))
            {
                command = CreateKeyEventCommand(eventname, eventProperties);
            }
            else
            {
                command = CreateHTMLEventCommand(eventname, eventProperties);
            }

            command += "var res = " + _elementReference + ".dispatchEvent(event); if(res){true;}else{false;};";

            if (WaitForEventToComplete == false)
            {
                command = JSUtils.WrapCommandInTimer(command);
            }
            return command;
        }

        private static string CleanupEventName(string eventName)
        {
            var eventname = eventName.ToLowerInvariant();

            return eventname.StartsWith("on")? eventname.Substring(2): eventname;
        }

        public string CreateHTMLEventCommand(string eventname, NameValueCollection eventProperties)
        {
            var eventParams = GetEventPropertyValue(eventProperties, "bubbles", "true") + ","
                              + GetEventPropertyValue(eventProperties, "cancelable", "true");

            return "var event = " + _elementReference + ".ownerDocument.createEvent(\"HTMLEvents\");" +
                   "event.initEvent('" + eventname + "',"+ eventParams + ");";
        }

        public string CreateMouseEventCommand(string eventname, NameValueCollection eventProperties)
        {
            // Params for the initMouseEvent:
            // 'type', bubbles, cancelable, windowObject, detail, screenX, screenY, clientX, clientY, ctrlKey, altKey, shiftKey, metaKey, button, relatedTarget )
            // '', true, true, null, 0, 0, 0, 0, 0, false, false, false, false, 0, null;
            var eventParams = GetEventPropertyValue(eventProperties, "bubbles", "true") + ","
                              + GetEventPropertyValue(eventProperties, "cancelable", "true") + ","
                              + GetEventPropertyValue(eventProperties, "windowObject", "null") + ","
                              + GetEventPropertyValue(eventProperties, "detail", "0") + ","
                              + GetEventPropertyValue(eventProperties, "screenX", "0") + ","
                              + GetEventPropertyValue(eventProperties, "screenY", "0") + ","
                              + GetEventPropertyValue(eventProperties, "clientX", "0") + ","
                              + GetEventPropertyValue(eventProperties, "clientY", "0") + ","
                              + GetEventPropertyValue(eventProperties, "ctrlKey", "false") + ","
                              + GetEventPropertyValue(eventProperties, "altKey", "false") + ","
                              + GetEventPropertyValue(eventProperties, "shiftKey", "false") + ","
                              + GetEventPropertyValue(eventProperties, "metaKey", "false") + ","
                              + GetEventPropertyValue(eventProperties, "button", "0") + ","
                              + GetEventPropertyValue(eventProperties, "relatedTarget", "null"); 

            return "var event = " + _elementReference + ".ownerDocument.createEvent(\"MouseEvents\");" +
                   "event.initMouseEvent('" + eventname + "'," + eventParams + ");";

        }

        private string CreateKeyEventCommand(string eventname, NameValueCollection eventProperties)
        {
            switch (_clientPortBase.JavaScriptEngine)
            {
                case JavaScriptEngineType.WebKit:
                    return CreateKeyEventForWebkit(eventProperties, eventname);
                case JavaScriptEngineType.Mozilla:
                    return CreateKeyboardEventForMozilla(eventname, eventProperties);
                default:
                    throw new NotImplementedException(string.Format("CreateKeyEventCommand not implemented for javascript engine {0}", this._clientPortBase.JavaScriptEngine));
            }
        }

        private string CreateKeyEventForWebkit(NameValueCollection eventProperties, string eventname)
        {
            // HACK: Webkit doesn't seem to support manually firing keyboard events, 
            // event listeners will get fired for the key events, but the event target seems to ignore it.
            // We get around this by manually appending the value to the element between keydown and keypress.
            var keyCode = GetEventPropertyValue(eventProperties, "keyCode", "0");

            var eventCommand = string.Empty;

            if (eventname == "keypress")
            {
                eventCommand = "if(" + _elementReference + ".type==\"text\"){" + _elementReference
                               + ".value+=\"" + Convert.ToChar(Convert.ToInt32(keyCode)) + "\";};";
            }

            eventCommand += "var event = " + _elementReference + ".ownerDocument.createEvent(\"Events\");" +
                            "event.initEvent('" + eventname + "', true, true);event.keyCode=" + keyCode + ";";
            return eventCommand;
        }

        public string CreateKeyboardEventForMozilla(string eventname, NameValueCollection eventProperties)
        {
            // After a lot of searching it seems that keyCode is not supported in keypress event
            // found out wierd behavior cause keyCode = 116 (="t") resulted in a page refresh. 

            var keyCode = GetEventPropertyValue(eventProperties, "keyCode", "0");
            if (eventname == "keypress") { keyCode = "0"; }

            // 'type', bubbles, cancelable, windowObject, ctrlKey, altKey, shiftKey, metaKey, keyCode, charCode
            var eventParams = GetEventPropertyValue(eventProperties, "bubbles", "true") + ","
                              + GetEventPropertyValue(eventProperties, "cancelable", "true") + ","
                              + GetEventPropertyValue(eventProperties, "windowObject", "null") + ","
                              + GetEventPropertyValue(eventProperties, "ctrlKey", "false") + ","
                              + GetEventPropertyValue(eventProperties, "altKey", "false") + ","
                              + GetEventPropertyValue(eventProperties, "shiftKey", "false") + ","
                              + GetEventPropertyValue(eventProperties, "metaKey", "false") + ","
                              + keyCode + ","
                              + GetEventPropertyValue(eventProperties, "charCode", "0");

            // Params for the initKeyEvent:
            return "var event = " + _elementReference + ".ownerDocument.createEvent(\"KeyboardEvent\");" +
                   "event.initKeyEvent('" + eventname + "'," + eventParams + ");";
        }

        private static string GetEventPropertyValue(NameValueCollection eventProperties, string propertyName, string defaultValue)
        {
            if (eventProperties != null)
            {
                var values = eventProperties.GetValues(propertyName);
                if (values != null && values.Length > 0)
                {
                    return values[0];
                }
            }

            return defaultValue;
        }

    }
}