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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WatiN.Examples.MsHtmlBrowser
{
    [TestClass]
    public class MsHtmlBrowserTests
    {
        [TestMethod]
        public void Should_open_google_results_page_for_watin()
        {
            // GIVEN
            var browser = new MsHtmlBrowser();
            browser.GoTo("http://www.google.com/search?q=watin");

            // WHEN
            var result = browser.ContainsText("WatiN");

            // THEN
            Assert.IsTrue(result);
        }
    }
}
