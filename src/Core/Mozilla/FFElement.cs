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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using WatiN.Core.Interfaces;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Mozilla
{
    public class FFElement : INativeElement
    {

        /// <summary>
        /// List of html attributes that have to retrieved as properties in order to get the correct value.
        /// I.e. for options myOption.getAttribute("selected"); returns nothing if it's selected. 
        /// However  myOption.selected returns true.
        /// </summary>
        private static readonly List<string> knownAttributeOverrides = new List<string>(
            new[]
                {
                    "selected", "textContent", "className", "disabled", "checked", "readOnly", "multiple", "value",
                    "nodeType", "innerHTML", "baseURI", "src"
                });

        private static readonly Dictionary<string, string> watiNAttributeMap = new Dictionary<string, string>();

        static FFElement()
        {
            watiNAttributeMap.Add("innertext", "textContent");
            watiNAttributeMap.Add("classname", "className");
            watiNAttributeMap.Add("htmlFor", "for");
        }

        private ElementAttributeBag _attributeBag;
        private string _tagName;
        private const int NodeType_Text = 3;

        public string ElementReference { get; private set; }
        public FireFoxClientPort ClientPort { get; private set; }

        public FFElement(object elementReference, FireFoxClientPort clientPort)
        {
            ElementReference = elementReference as string;
            if (string.IsNullOrEmpty(ElementReference)) throw new ArgumentException("should be of type string and not null or empty","elementReference");

            if (clientPort == null) throw new ArgumentNullException("clientPort");
            ClientPort = clientPort;
        }

        /// <summary>
        /// This not supported in FireFox
        /// </summary>
        public string TextAfter
        {
            get
            {
                var element = GetElementByProperty("nextSibling");

                if (element == null || !element.IsTextNodeType()) return string.Empty;

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

                if (element == null || !element.IsTextNodeType()) return string.Empty;

                return element.GetAttributeValue("textContent");
            }
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

            // remove all \n (newline) and any following spaces
            var newlineSpaces = new Regex("\n *");
            var returnValue = newlineSpaces.Replace(innerHtml, "");

            // that's it for text nodes
            if (IsTextNodeType()) return returnValue;

            // remove all but the last param tag by \r\n
            var param = new Regex("</p>");
            var matches = param.Matches(returnValue);
            if (matches.Count > 1)
            {
                returnValue = param.Replace(returnValue, "\r\n", matches.Count - 1);
            }

            // remove all br tags and following space with \r\n
            var br = new Regex("<br> *");
            returnValue = br.Replace(returnValue, "\r\n");

            // remove all hr tags and following space with \r\n
            var hr = new Regex("<hr> *");
            returnValue = hr.Replace(returnValue, "\r\n");

            // remove tags (beginning and end tags)
            var tag = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?> *");
            returnValue = tag.Replace(returnValue, "");

//            tag = new Regex(@"</\w> *");
//            returnValue = tag.Replace(returnValue, "");

            // remove comment
            tag = new Regex("<!--.*-->");
            returnValue = tag.Replace(returnValue, "");

            // replace multiple spaces by one space
            var moreThanOneSpace = new Regex(" +");
            returnValue = moreThanOneSpace.Replace(returnValue, " ");

            // remove spaces at the beginning of the text
            return returnValue.TrimStart();
        }

        public INativeElement NextSibling
        {
            get
            {
                var element = GetElementByProperty("nextSibling");

                while (true)
                {
                    if (element == null || !element.IsTextNodeType())
                    {
                        return element;
                    }

                    element = (FFElement)element.NextSibling;
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
                    if (element == null || !element.IsTextNodeType())
                    {
                        return element;
                    }

                    element = (FFElement)element.PreviousSibling;
                }
            }
        }

        public INativeElement Parent
        {
            get { return GetElementByProperty("parentNode"); }
        }

        public string GetAttributeValue(string attributeName)
        {
            // Special case
            var attribName = attributeName.ToLowerInvariant();
            if (attribName == "tagname") return TagName;
            if (attribName == "outerhtml") return GetOuterHtml();

            // Translate to FireFox html syntax
            if (watiNAttributeMap.ContainsKey(attributeName))
            {
                attributeName = watiNAttributeMap[attributeName];
            }

            // Handle properties different from attributes
            if (knownAttributeOverrides.Contains(attributeName))
            {
                var propertyName = attributeName;
                if (propertyName == "textContent" && !IsTextNodeType()) propertyName = "innerHTML";

                var propertyValue = GetProperty(propertyName);

                if (attributeName == "textContent") propertyValue = InnerHtmlToInnerText(propertyValue);
                return propertyValue;
            }

            var value = GetAttribute(attributeName);

            // Post processing
            if (attributeName.ToLowerInvariant() == "type") { value = value ?? "text"; }

            // return result
            return value;
        }

        public string GetAttribute(string attributeName)
        {
            var getAttributeWrite = string.Format("{0}.getAttribute(\"{1}\");", ElementReference, attributeName);

            return ClientPort.WriteAndRead(getAttributeWrite);
        }

        public void SetAttributeValue(string attributeName, string value)
        {
            // Ignores
            // Checked is set be the click event on RadioButton.Checked and CheckBox.Checked
            // for CheckBox.Checked it in resulted in setting and re-setting
            if (attributeName == "checked") return;

            // Translate to FireFox html syntax
            if (watiNAttributeMap.ContainsKey(attributeName))
            {
                attributeName = watiNAttributeMap[attributeName];
            }

            // Handle properties different from attributes
            if (knownAttributeOverrides.Contains(attributeName) || attributeName.StartsWith("style", StringComparison.OrdinalIgnoreCase))
            {
                SetProperty(attributeName, value);
                return;
            }

            SetAttribute(attributeName, value);
        }

        public void SetAttribute(string attributeName, string value)
        {
            ClientPort.Write("{0}.setAttribute(\"{1}\", \"{2}\");", ElementReference, attributeName, value);
        }

        public string GetStyleAttributeValue(string attributeName)
        {
            attributeName = UtilityClass.TurnStyleAttributeIntoProperty(attributeName);

            var attributeValue = GetProperty("style." + attributeName);
            return string.IsNullOrEmpty(attributeValue) ? null : attributeValue;
        }

        public void SetStyleAttributeValue(string attributeName, string value)
        {
            attributeName = UtilityClass.TurnStyleAttributeIntoProperty(attributeName);
            SetProperty("style." + attributeName, "'" + value + "'");
        }

        public void ClickOnElement()
        {
            FireEvent("click", null);
        }

        public void SetFocus()
        {
            ExecuteMethod("focus");
        }

        public void FireEvent(string eventName, NameValueCollection eventProperties)
        {
            // TODO: can FireFox handle eventProperties?
            ExecuteEvent(eventName, eventProperties, true);
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
            if (UtilityClass.IsNullOrEmpty(ElementReference)) return false;

            var command = string.Format("{0} != null && {0}.offsetParent != null; ", ElementReference);

            return ClientPort.WriteAndReadAsBool(command);
        }

        public string TagName
        {
            get
            {
                if (_tagName == null)
                {
                    _tagName = GetProperty("tagName");
                }

                return _tagName;
            }
        }

        public object Object
        {
            get { return ElementReference; }
        }

        public object Objects
        {
            get { return ElementReference; }
        }

        public void FireEventNoWait(string eventName, NameValueCollection eventProperties)
        {
            ExecuteEvent(eventName, eventProperties, false);
        }

        public void Select()
        {
            FireEvent("select", null);
        }

        public void SubmitForm()
        {
            FireEvent("submit", null);
        }

        public void SetFileUploadFile(Element element, string fileName)
        {
            fileName = fileName.Replace(@"\", @"\\");
            SetAttributeValue("value", fileName);
        }

        /// <summary>
        /// Executes the event.
        /// </summary>
        /// <param name="eventName">Name of the event to fire.</param>
        /// <param name="eventProperties"></param>
        /// <param name="WaitForEventToComplete"></param>
        private void ExecuteEvent(string eventName, NameValueCollection eventProperties, bool WaitForEventToComplete)
        {
            // See http://www.howtocreate.co.uk/tutorials/javascript/domevents
            // for more info about manually firing events

            var eventname = CleanupEventName(eventName);

            string command;

            if (eventname.Contains("mouse") || eventname == "click")
            {
                command = CreateMouseEventCommand(eventname);
            }
            else if (eventname.Contains("key"))
            {
                command = CreateKeyEventCommand(eventname, eventProperties);
            }
            else
            {
                command = CreateHTMLEventCommand(eventname);
            }


            command += "var res = " + ElementReference + ".dispatchEvent(event); if(res){true;}else{false;};";

            if (WaitForEventToComplete == false)
            {
                command = WrappCommandInTimer(command);
            }
            
            ClientPort.WriteAndReadAsBool(command);

        }

        private static string WrappCommandInTimer(string command)
        {
            return "window.setTimeout(function() {" + command + ";" +"}, 200);";

        }

        private static string CleanupEventName(string eventName)
        {
            var eventname = eventName.ToLowerInvariant();

            if (eventname.StartsWith("on"))
            {
                eventname = eventname.Substring(2);
            }
            return eventname;
        }

        private static string CreateHTMLEventCommand(string eventname)
        {
            return "var event = " + FireFoxClientPort.DocumentVariableName + ".createEvent(\"HTMLEvents\");" +
                   "event.initEvent(\"" + eventname + "\",true,true);";
        }

        private static string CreateMouseEventCommand(string eventname)
        {
            // Params for the initMouseEvent:
            // 'type', bubbles, cancelable, windowObject, detail, screenX, screenY, clientX, clientY, ctrlKey, altKey, shiftKey, metaKey, button, relatedTarget )

            return "var event = " + FireFoxClientPort.DocumentVariableName + ".createEvent(\"MouseEvents\");" +
                   "event.initMouseEvent('" + eventname + "', true, true, null, 0, 0, 0, 0, 0, false, false, false, false, 0, null );";
        }

        private static string CreateKeyEventCommand(string eventname, NameValueCollection eventProperties)
        {
            // Params for the initKeyEvent:
            // 'type', bubbles, cancelable, windowObject, ctrlKey, altKey, shiftKey, metaKey, keyCode, charCode

            var keyCode = GetEventPropertyValue(eventProperties, "keyCode", "0");
            var charCode = GetEventPropertyValue(eventProperties, "charCode", "0");
            
            // After a lot of searching it seems that keyCode is not supported in keypress event
            // found out wierd behavior cause keyCode = 116 (="t") resulted in a page refresh. 
            if (eventname == "keypress") keyCode = "0";
            
            return "var event = " + FireFoxClientPort.DocumentVariableName + ".createEvent(\"KeyboardEvent\");" +
                   "event.initKeyEvent('" + eventname + "', true, true, null, false, false, false, false, " + keyCode + ", " + charCode + " );";
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

        /// <summary>
        /// Executes a method with no parameters.
        /// </summary>
        /// <param name="methodName">Name of the method to execute.</param>
        private void ExecuteMethod(string methodName)
        {
            ClientPort.Write("{0}.{1}();", ElementReference, methodName);
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
            if (propertyName == "value") value = "'" + value + "'";
            
            var command = string.Format("{0}.{1} = {2};", ElementReference, propertyName, value);
            ClientPort.Write(command);
        }

        /// <summary>
        /// Gets the element by property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Returns the element that is returned by the specified property</returns>
        internal FFElement GetElementByProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            var elementvar = FireFoxClientPort.CreateVariableName();
            var command = string.Format("{0}={1}.{2}; {0}!=null;", elementvar, ElementReference, propertyName);
            var exists = ClientPort.WriteAndReadAsBool(command);

            return exists ? new FFElement(elementvar, ClientPort) : null;
        }

        // TODO: Can/should be cached
        private bool IsTextNodeType()
        {
            var nodeTypeValue = GetProperty("nodeType");
            return Convert.ToInt32(nodeTypeValue) == NodeType_Text;
        }

        private string GetOuterHtml()
        {
            var clone = FireFoxClientPort.CreateVariableName();
            var div = FireFoxClientPort.CreateVariableName();
            var outerHtml = FireFoxClientPort.CreateVariableName();

            var command = string.Format("{0}={1}.cloneNode(true);", clone, ElementReference);
            command += string.Format("{0}={1}.createElement('div');", div, FireFoxClientPort.DocumentVariableName);
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
}
