using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using WatiN.Core.Interfaces;

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
                    "selected", "textContent", "className", "disabled", "checked", "readOnly", "multiple", "value"
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

                return CleanUpText(element);
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

                return CleanUpText(element);
            }
        }

        private static string CleanUpText(INativeElement element)
        {
            var value = element.GetAttributeValue("textContent");

            return string.IsNullOrEmpty(value) ? string.Empty : value.Split(Convert.ToChar("\n"))[0];
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
            // Special casese
            if (attributeName.ToLowerInvariant() == "tagname") return TagName;
            
            // Translate to FireFox html syntax
            if (watiNAttributeMap.ContainsKey(attributeName))
            {
                attributeName = watiNAttributeMap[attributeName];
            }

            // Handle properties different from attributes
            if (knownAttributeOverrides.Contains(attributeName) || attributeName.StartsWith("style", StringComparison.OrdinalIgnoreCase))
            {
                return GetPropertyValue(attributeName);
            }

            // Retrieve attribute value
            var getAttributeWrite = string.Format("{0}.getAttribute(\"{1}\");", ElementReference, attributeName);
            var lastResponse = ClientPort.WriteAndRead(getAttributeWrite);

            // Post processing
            if (attributeName.ToLowerInvariant() == "type") { lastResponse = lastResponse ?? "text"; }

            // return result
            return lastResponse;
        }

        public void SetAttributeValue(string attributeName, string value)
        {
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

            ClientPort.Write("{0}.setAttribute(\"{1}\", \"{2}\");", ElementReference, attributeName, value);
        }

        public string GetStyleAttributeValue(string attributeName)
        {
            throw new System.NotImplementedException();
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
            ExecuteEvent(eventName, eventProperties);
        }

        // TODO: implement backgroundcolor
        public string BackgroundColor
        {
            get { return string.Empty; }
            set {  }
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

            var command = string.Format("{0} != null; ", ElementReference);

            return ClientPort.WriteAndReadAsBool(command);
        }

        public string TagName
        {
            get
            {
                if (_tagName == null)
                {
                    _tagName = GetPropertyValue("tagName");
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
            // TODO: Make it not wait
            // TODO: Can FireFox handle eventProperties?
            ExecuteEvent(eventName, eventProperties);
        }

        public void Select()
        {
            FireEvent("select", null);
        }

        public void SubmitForm()
        {
            FireEvent("submit", null);
        }

        /// <summary>
        /// Executes the event.
        /// </summary>
        /// <param name="eventName">Name of the event to fire.</param>
        private void ExecuteEvent(string eventName, NameValueCollection eventProperties)
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

            ClientPort.WriteAndReadAsBool
                (command + "var res = " + ElementReference + ".dispatchEvent(event); if(res){true;}else{false;};");

        }

        private string CleanupEventName(string eventName)
        {
            var eventname = eventName.ToLowerInvariant();

            if (eventname.StartsWith("on"))
            {
                eventname = eventname.Substring(2);
            }
            return eventname;
        }

        private string CreateHTMLEventCommand(string eventname)
        {
            return "var event = " + FireFoxClientPort.DocumentVariableName + ".createEvent(\"HTMLEvents\");" +
                   "event.initEvent(\"" + eventname + "\",true,true);";
        }

        private string CreateMouseEventCommand(string eventname)
        {
            // Params for the initMouseEvent:
            // 'type', bubbles, cancelable, windowObject, detail, screenX, screenY, clientX, clientY, ctrlKey, altKey, shiftKey, metaKey, button, relatedTarget )

            return "var event = " + FireFoxClientPort.DocumentVariableName + ".createEvent(\"MouseEvents\");" +
                   "event.initMouseEvent('" + eventname + "', true, true, null, 0, 0, 0, 0, 0, false, false, false, false, 0, null );";
        }

        private string CreateKeyEventCommand(string eventname, NameValueCollection eventProperties)
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

        private string GetEventPropertyValue(NameValueCollection eventProperties, string propertyName, string defaultValue)
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
        private string GetPropertyValue(string propertyName)
        {
            var command = string.Format("{0}.{1};", ElementReference, propertyName);
            return ClientPort.WriteAndRead(command);
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        protected void SetProperty(string propertyName, string value)
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
            var command = string.Format("{0}={1}.{2};", elementvar, ElementReference, propertyName);
            var result = ClientPort.WriteAndRead(command);

            return result != null ? new FFElement(elementvar, ClientPort) : null;
        }

        private bool IsTextNodeType()
        {
            var nodeTypeValue = GetPropertyValue("nodeType");
            return Convert.ToInt32(nodeTypeValue) == NodeType_Text;
        }

    }
}
