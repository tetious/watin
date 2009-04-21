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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Native;

namespace WatiN.Core.UnitTests.Native
{
    /// <summary>
    ///This is a NUnit TestFixture class
    ///</summary>
    [TestFixture]
    public class JSElementCollectionBaseTests
    {
        [Test]
        public void ShouldForceTagNameOnJSElementWithoutAskingTheBrowserWhenGetElementsByTagIsCalled()
        {
            // GIVEN
            var elementCollection = new WrappedJSElementCollection(new MockClientPort(), "container");

            // WHEN
            var elements = elementCollection.GetElementsByTag("myTestTagName");

            // THEN
            var element = (wrappedJSElement) elements.First();
            Assert.That(element.AttribCache.ContainsKey("tagName"), "Expected cached tagName");
            Assert.That(element.AttribCache.ContainsValue("myTestTagName"), "Expected tagName 'myTestTagName' in cache");
        }

        [Test]
        public void ShouldNotForceTagNameOnJSElementWhenTagNameIsAstrisk()
        {
            // GIVEN
            var elementCollection = new WrappedJSElementCollection(new MockClientPort(), "container");

            // WHEN
            var elements = elementCollection.GetElementsByTag("*");

            // THEN
            var element = (wrappedJSElement) elements.First();
            Assert.That(element.AttribCache.ContainsKey("tagName"), Is.False, "tagName shouldn't be in the cache");
        }
    }

    public class MockClientPort : ClientPortBase
    {
        public override string DocumentVariableName
        {
            get { return "document"; }
        }

        /// <summary>
        /// Gets the type of java script engine.
        /// </summary>
        /// <value>The type of java script engine.</value>
        public override JavaScriptEngineType JavaScriptEngine
        {
            get
            {
                return JavaScriptEngineType.Mozilla;
            }
        }

        /// <summary>
        /// Gets the name of the browser variable.
        /// </summary>
        /// <value>The name of the browser variable.</value>
        public override string BrowserVariableName
        {
            get { return "browser"; }
        }

        public override void InitializeDocument()
        {
            return;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            return;
        }

        /// <summary>
        /// Connects to the Chrome browser and navigates to the specified URL.
        /// </summary>
        /// <param name="url">The URL to connect to.</param>
        public override void Connect(string url)
        {
            return;
        }

        public override void ConnectToExisting()
        {
            throw new NotImplementedException();
        }

        protected override void SendAndRead(string data, bool resultExpected, bool checkForErrors, params object[] args)
        {
            return;
        }
    }

    public class WrappedJSElementCollection : JSElementCollectionBase
    {
        public WrappedJSElementCollection(ClientPortBase clientPort, string containerReference) : base(clientPort, containerReference)
        {
        }

        protected override IEnumerable<JSElement> GetElementArrayEnumerator(string command)
        {
            return new List<JSElement> {new wrappedJSElement(clientPort, "elementRef1")};
        }        
    }

    public class wrappedJSElement : JSElement
    {
        public wrappedJSElement(ClientPortBase clientPort, string elementReference) : base(clientPort, elementReference)
        {
        }
 
        public Dictionary<string, object> AttribCache
        {
            get { return AttributeCache; }
        }
    }
}