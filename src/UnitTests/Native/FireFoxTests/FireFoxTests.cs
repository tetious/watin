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

using System.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests.FireFoxTests
{
    using Logging;

    [TestFixture]
    public class FireFoxTests : BaseWatiNTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FireFoxTests"/> class.
        /// </summary>
        public FireFoxTests()
        {
            Logger.LogWriter = new DebugLogWriter();
        }

        [Test]
        public void CheckFireFoxIsInheritingProperTypes()
        {
			using (var firefox = new FireFox())
			{
				Assert.IsInstanceOfType(typeof (FireFox), firefox, "Should be a FireFox instance");
				Assert.IsInstanceOfType(typeof (Browser), firefox, "Should be a Browser instance");
				Assert.IsInstanceOfType(typeof (DomContainer), firefox, "Should be a DomContainer instance");
			}
        }

        [Test, Category("InternetConnectionNeeded")]
        public void GoogleSearchWithEncodedQueryStringInConstructor()
        {
            var url = string.Format("http://www.google.com/search?q={0}", HttpUtility.UrlEncode("a+b"));

            using (var firefox = new FireFox(url))
            {
                Assert.That(firefox.TextField(Find.ByName("q")).Value, Is.EqualTo("a+b"));
            }
        }

        [Test, Category("InternetConnectionNeeded")]
        public void AddProtocolToUrlIfOmmitted()
        {
            using (var firefox = new FireFox("www.google.com"))
            {
                Assert.That(firefox.Url, Text.StartsWith("http://"));
            }
        }

        [Test]
        public void NewFireFoxInstanceShouldTake_Settings_MakeNewBrowserInstanceVisible_IntoAccount()
        {
            // TODO: make this work for firefox if possible. Do rename the setting from ...IeInstance... to ...BrowserInstance...
        }

        [Test]
        public void NewFireFoxWithoutUrlShouldStartAtAboutBlank()
        {
            using (var fireFox = new FireFox())
            {
                Assert.AreEqual("about:blank", fireFox.Url);
            }
        }

        [Test]
        public void NewFireFoxWithUri()
        {
            using (var fireFox = new FireFox(MainURI))
            {
                Assert.AreEqual(MainURI, fireFox.Uri);
            }
        }

        [Test]
        public void NewFireFoxWithUriShouldAutoClose()
        {
            Assert.That(FireFox.CurrentProcessCount, Is.EqualTo(0), "pre-condition: Expected no running firefox instances");
            using (new FireFox(MainURI)) { }

            Assert.That(FireFox.CurrentProcessCount, Is.EqualTo(0), "Expected no running firefox instances");
        }


    }
}