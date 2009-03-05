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