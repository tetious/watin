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
                    "selected", "textContent", "className"
                });

        private static readonly Dictionary<string, string> watiNAttributeMap = new Dictionary<string, string>();

        static FFElement()
        {
            watiNAttributeMap.Add("innertext", "textContent");
            watiNAttributeMap.Add("classname", "className");
        }

        private ElementAttributeBag _attributeBag;
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

        public string TextAfter
        {
            get { throw new System.NotImplementedException(); }
        }

        public string TextBefore
        {
            get { throw new System.NotImplementedException(); }
        }

        public INativeElement NextSibling
        {
            get
            {
                var element = GetElementByProperty("nextSibling");

                while (true)
                {
                    if (element == null || element.IsTextNodeType())
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
                    if (element == null || element.IsTextNodeType())
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
            if (UtilityClass.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException("attributeName", "Null or Empty not allowed.");
            }

            if (string.IsNullOrEmpty(ElementReference))
            {
                throw new FireFoxException("Element does not exist, element variable was empty");
            }

            if (watiNAttributeMap.ContainsKey(attributeName))
            {
                attributeName = watiNAttributeMap[attributeName];
            }

            if (knownAttributeOverrides.Contains(attributeName) || attributeName.StartsWith("style", StringComparison.OrdinalIgnoreCase))
            {
                return GetProperty(attributeName);
            }

            var getAttributeWrite = string.Format("{0}.getAttribute(\"{1}\");", ElementReference, attributeName);
            ClientPort.Write(getAttributeWrite);

            return ClientPort.LastResponseIsNull ? null : ClientPort.LastResponse;
        }

        public void SetAttributeValue(string attributeName, string value)
        {
            throw new System.NotImplementedException();
        }

        public string GetStyleAttributeValue(string attributeName)
        {
            throw new System.NotImplementedException();
        }

        public void ClickOnElement()
        {
            throw new System.NotImplementedException();
        }

        public void SetFocus()
        {
            throw new System.NotImplementedException();
        }

        public void FireEvent(string eventName, NameValueCollection eventProperties)
        {
            // TODO: can FireFox handle eventProperties?
            ExecuteEvent(eventName);
        }

        public string BackgroundColor
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
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
            ClientPort.Write(command);

            return ClientPort.LastResponseAsBool;
        }

        public string TagName
        {
            get { return GetProperty("tagName"); }
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
            ExecuteEvent(eventName);
        }

        public void Select()
        {
            throw new System.NotImplementedException();
        }

        public void SubmitForm()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Executes the event.
        /// </summary>
        /// <param name="eventName">Name of the event to fire.</param>
        private void ExecuteEvent(string eventName)
        {
            ClientPort.Write(
                    "var event = " + FireFoxClientPort.DocumentVariableName + ".createEvent(\"MouseEvents\");\n" +
                    "event.initEvent(\"" + eventName + "\",true,true);\n" +
                    "var res = " + ElementReference + ".dispatchEvent(event); if(res){true;}else{false};");
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private string GetProperty(string propertyName)
        {
            if (UtilityClass.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName", "Null or Empty not allowed.");
            }

            var command = string.Format("{0}.{1};", ElementReference, propertyName);
            return ClientPort.WriteAndRead(command);
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
            ClientPort.Write(command);

            return !ClientPort.LastResponseIsNull ? new FFElement(elementvar, ClientPort) : null;
        }

        private bool IsTextNodeType()
        {
            var nodeTypeValue = GetProperty("nodeType");
            return Convert.ToInt32(nodeTypeValue) == NodeType_Text;
        }

    }
}
