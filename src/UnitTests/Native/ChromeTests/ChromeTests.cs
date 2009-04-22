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

#if IncludeChromeInUnitTesting

namespace WatiN.Core.UnitTests.Native.ChromeTests
{
    using System.Web;

    using Logging;

    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    /// <summary>
    /// Tests behaviour specific to the <see cref="Chrome"/> class.
    /// </summary>
    [TestFixture]
    public class ChromeTests : BaseWatiNTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeTests"/> class.
        /// </summary>
        public ChromeTests()
        {
            Logger.LogWriter = new DebugLogWriter();
        }

        [Test, Category("InternetConnectionNeeded")]
        public void AddProtocolToUrlIfOmmitted()
        {
            using (var chrome = new Chrome("www.google.com"))
            {
                Assert.That(chrome.Url, Text.StartsWith("http://"));
            }
        }

        [Test, Ignore("Known issue, requires solution")]
        public void InnerTextIsNotTruncated()
        {
            using (var chrome = new Chrome(MainURI))
            {
                Assert.That(chrome.Text, Text.DoesNotContain("... (length:"), "Body inner text was truncated, a known issue which requires a work around.");
            }
        }

        [Test]
        public void CheckFireFoxIsInheritingProperTypes()
        {
            using (var chrome = new Chrome())
            {
                Assert.IsInstanceOfType(typeof(Chrome), chrome, "Should be a Chrome instance");
                Assert.IsInstanceOfType(typeof(Browser), chrome, "Should be a Browser instance");
                Assert.IsInstanceOfType(typeof(DomContainer), chrome, "Should be a DomContainer instance");
            }
        }

        [Test, Category("InternetConnectionNeeded")]
        public void GoogleSearchWithEncodedQueryStringInConstructor()
        {
            var url = string.Format("http://www.google.com/search?q={0}", HttpUtility.UrlEncode("a+b"));

            using (var chrome = new Chrome(url))
            {
                Assert.That(chrome.TextField(Find.ByName("q")).Value, Is.EqualTo("a+b"));
            }
        }

        [Test]
        public void NewChromeWithoutUrlShouldStartAtAboutBlank()
        {
            using (var chrome = new Chrome())
            {
                Assert.AreEqual("about:blank", chrome.Url);
            }
        }

        [Test]
        public void NewChromeWithUri()
        {
            using (var chrome = new Chrome(MainURI))
            {
                Assert.AreEqual(MainURI, chrome.Uri);
            }
        }

        [Test]
        public void NewChromeWithUriShouldAutoClose()
        {
            Assert.That(Chrome.CurrentProcessCount, Is.EqualTo(0), "pre-condition: Expected no running chrome instances");
            using (new Chrome(MainURI)) { }

            Assert.That(Chrome.CurrentProcessCount, Is.EqualTo(0), "Expected no running chrome instances");
        }

        [Test]
        public void CanTypeTextIntoATextField()
        {
            using (Chrome chrome = new Chrome(MainURI))
            {
                chrome.TextField("name").TypeText("Hello world");
                Assert.That(chrome.TextField("name").Text, Is.EqualTo("Hello world"));
            }
        }
    }
}
#endif
