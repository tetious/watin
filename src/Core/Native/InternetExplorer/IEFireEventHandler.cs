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
using System.Collections.Specialized;
using System.Text;
using mshtml;
using WatiN.Core.Exceptions;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native.InternetExplorer
{
    public class IEFireEventHandler
    {
        private readonly IEElement _ieElement;

        public IEFireEventHandler(IEElement ieElement)
        {
            _ieElement = ieElement;
        }

        /// <summary>
        /// Fires the event on the element but doesn't wait for it to complete
        /// </summary>
        /// <param name="eventName">Name of the event to fire</param>
        /// <param name="eventProperties">The event object properties.</param>
        public void FireEventNoWait(string eventName, NameValueCollection eventProperties)
        {
            FireEvent(eventName, eventProperties, true);
        }

        /// <summary>
        /// Fires the event on the given element.
        /// </summary>
        /// <param name="eventName">Name of the event to fire</param>
        /// <param name="eventProperties">The event object properties.</param>
        public void FireEvent(string eventName, NameValueCollection eventProperties)
        {
            FireEvent(eventName, eventProperties, false);
        }

        private void FireEvent(string eventName, NameValueCollection eventProperties, bool noWait)
        {
            if (HandledOnClick(eventName, noWait)) return;

            SetValueWhenOnKeyPress(eventName, eventProperties);

            var scriptCode = CreateJavaScriptFireEventCode(eventProperties, eventName);

            if (noWait) scriptCode = JSUtils.WrapCommandInTimer(scriptCode);

            try
            {
                ExecuteScript(scriptCode);
            }
            catch (RunScriptException)
            {
                // In a cross domain automation scenario a System.UnauthorizedAccessException 
                // is thrown.  This code does causes the registered client event handlers to be fired
                // but it does not deliver the event to the control itself.  Consequently the
                // control state may need to be updated directly (eg. as with key press event).

                var eventObj = CreateCOMEventObject(eventProperties);
                _ieElement.AsDispHTMLBaseElement.FireEvent(eventName, ref eventObj);
            }
        }

        private bool HandledOnClick(string eventname, bool noWait)
        {
            if (eventname.ToLower() == "onclick")
            {
                if (!noWait) 
                    _ieElement.ClickOnElement();
                else
                    UtilityClass.AsyncActionOnBrowser(_ieElement.ClickOnElement);

                return true;
            }
            return false;
        }

        private object CreateCOMEventObject(NameValueCollection eventProperties)
        {
            object prototypeEvent = null;
            object eventObj = ((IHTMLDocument4)_ieElement.AsHtmlElement.document).CreateEventObject(ref prototypeEvent);

            if (eventProperties == null)
            {
                for (var index = 0; index < eventProperties.Count; index++)
                {
                    var property = eventProperties.GetKey(index);
                    var value = eventProperties.GetValues(index)[0];

                    ((IHTMLEventObj2) eventObj).setAttribute(property, value, 0);
                }
            }
            return eventObj;
        }

        private string CreateJavaScriptFireEventCode(NameValueCollection eventObjectProperties, string eventName)
        {
            var scriptCode = new StringBuilder();
            scriptCode.Append("var newEvt = document.createEventObject();");

            CreateJavaScriptEventObject(scriptCode, eventObjectProperties);

            var reference = _ieElement.GetJavaScriptElementReference();
            scriptCode.Append(string.Format("{0}.fireEvent('{1}', newEvt);", reference, eventName));

            return scriptCode.ToString();
        }

        private void ExecuteScript(string code)
        {
            var window = _ieElement.ParentWindow;
            IEUtils.RunScript(code, window);
        }

        private static void CreateJavaScriptEventObject(StringBuilder scriptCode, NameValueCollection eventObjectProperties)
        {
            if (eventObjectProperties == null) return;

            for (var index = 0; index < eventObjectProperties.Count; index++)
            {
                if (eventObjectProperties.GetKey(index) == "charCode") continue;

                scriptCode.Append("newEvt.");
                scriptCode.Append(eventObjectProperties.GetKey(index));
                scriptCode.Append(" = ");
                scriptCode.Append(eventObjectProperties.GetValues(index)[0]);
                scriptCode.Append(";");
            }
        }

        private void SetValueWhenOnKeyPress(string eventName, NameValueCollection eventProperties)
        {
            if (eventName != "onKeyPress" || eventProperties == null) return;
            
            var keys = eventProperties.GetValues("keyCode");
            if (keys == null || keys.Length <= 0) return;
            
            var addChar = keys[0];
            var newValue = _ieElement.GetAttributeValue("value") + ((char) int.Parse(addChar));
            
            _ieElement.SetAttributeValue("value", newValue);
        }
    }
}
