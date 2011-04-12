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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Native;

namespace WatiN.Core.UnitTests.Native.FireFoxTests
{
    [TestFixture]
    public class FireEventTests
    {
        [Test]
        public void Should_create_javascript_code_to_call_mouse_event_with_the_defaults()
        {
            // GIVEN
            var creator = new JSEventCreator("test", new ClientPortMock());

            // WHEN
            var command = creator.CreateEvent("onmousedown", new NameValueCollection{},true );

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"MouseEvents\");event.initMouseEvent('mousedown',true,true,null,0,0,0,0,0,false,false,false,false,0,null);var res = test.dispatchEvent(event); if(res){true;}else{false;};"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_javascript_code_to_call_click_event_with_the_defaults()
        {
            // GIVEN
            var creator = new JSEventCreator("test", new ClientPortMock());

            // WHEN
            var command = creator.CreateEvent("click", new NameValueCollection{},true );

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"MouseEvents\");event.initMouseEvent('click',true,true,null,0,0,0,0,0,false,false,false,false,0,null);var res = test.dispatchEvent(event); if(res){true;}else{false;};"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_mouse_event_with_params_set()
        {
            // GIVEN
            var eventParams = new NameValueCollection
                {
                    {"type", "type"},
                    {"bubbles", "bubbles"},
                    {"cancelable", "cancelable"},
                    {"windowObject", "windowObject"},
                    {"detail", "detail"},
                    {"screenX", "screenX"},
                    {"screenY", "screenY"},
                    {"clientX", "clientX"},
                    {"clientY", "clientY"},
                    {"ctrlKey", "ctrlKey"},
                    {"altKey", "altKey"},
                    {"shiftKey", "shiftKey"},
                    {"metaKey", "metaKey"},
                    {"button", "button"},
                    {"relatedTarget", "relatedTarget"}
                };

            var creator = new JSEventCreator("test", new ClientPortMock());

            // WHEN
            var command = creator.CreateMouseEventCommand("mousedown", eventParams);

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"MouseEvents\");event.initMouseEvent('mousedown',bubbles,cancelable,windowObject,detail,screenX,screenY,clientX,clientY,ctrlKey,altKey,shiftKey,metaKey,button,relatedTarget);"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_javascript_code_to_call_key_event_with_the_defaults()
        {
            // GIVEN
            var creator = new JSEventCreator("test", new ClientPortMock(JavaScriptEngineType.Mozilla));

            // WHEN
            var command = creator.CreateEvent("onkeydown", new NameValueCollection { }, true);

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"KeyboardEvent\");event.initKeyEvent('keydown',true,true,null,false,false,false,false,0,0);var res = test.dispatchEvent(event); if(res){true;}else{false;};"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_key_event_with_params_set()
        {
            // GIVEN
            var eventParams = new NameValueCollection
                {
                    {"type", "type"},
                    {"bubbles", "bubbles"},
                    {"cancelable", "cancelable"},
                    {"windowObject", "windowObject"},
                    {"ctrlKey", "ctrlKey"},
                    {"altKey", "altKey"},
                    {"shiftKey", "shiftKey"},
                    {"metaKey", "metaKey"},
                    {"keyCode", "keyCode"},
                    {"charCode", "charCode"}
                };

            var creator = new JSEventCreator("test", new ClientPortMock());

            // WHEN
            var command = creator.CreateKeyboardEventForMozilla("keydown", eventParams);

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"KeyboardEvent\");event.initKeyEvent('keydown',bubbles,cancelable,windowObject,ctrlKey,altKey,shiftKey,metaKey,keyCode,charCode);"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_javascript_code_to_call_any_non_key_mouse_event_with_params_set()
        {
            // GIVEN
            var eventParams = new NameValueCollection
                {
                    {"notused", "notused"},
                    {"bubbles", "bubbles"},
                    {"cancelable", "cancelable"},
                };
            
            var creator = new JSEventCreator("test", new ClientPortMock(JavaScriptEngineType.Mozilla));

            // WHEN
            var command = creator.CreateEvent("focus", eventParams, true);

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"HTMLEvents\");event.initEvent('focus',bubbles,cancelable);var res = test.dispatchEvent(event); if(res){true;}else{false;};"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_javascript_code_to_call_any_non_key_mouse_event_with_the_defaults()
        {
            // GIVEN
            var creator = new JSEventCreator("test", new ClientPortMock(JavaScriptEngineType.Mozilla));

            // WHEN
            var command = creator.CreateEvent("focus", new NameValueCollection { }, true);

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Console.WriteLine(command);
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"HTMLEvents\");event.initEvent('focus',true,true);var res = test.dispatchEvent(event); if(res){true;}else{false;};"), "Unexpected method signature");
        }
    }

    public class ClientPortMock : ClientPortBase
    {
        private readonly JavaScriptEngineType _javaScriptEngineType;

        public ClientPortMock(){}

        public ClientPortMock(JavaScriptEngineType javaScriptEngineType)
        {
            _javaScriptEngineType = javaScriptEngineType;
        }

        #region Overrides of ClientPortBase

        public override string DocumentVariableName
        {
            get { throw new NotImplementedException(); }
        }

        public override JavaScriptEngineType JavaScriptEngine
        {
            get { return _javaScriptEngineType; }
        }

        public override string BrowserVariableName
        {
            get { throw new NotImplementedException(); }
        }

        public override void InitializeDocument()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void Connect(string url)
        {
            throw new NotImplementedException();
        }

        protected override void SendAndRead(string data, bool resultExpected, bool checkForErrors, params object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
