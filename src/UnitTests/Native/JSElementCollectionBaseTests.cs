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

        public override void InitializeDocument()
        {
            return;
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

        protected override IEnumerable<INativeElement> GetElementsByIdImpl(string id)
        {
            throw new NotImplementedException();
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