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
using mshtml;
using NUnit.Framework;
using SHDocVw;

namespace WatiN.Core.UnitTests.ResearchTests
{
    [TestFixture]
    public class JavascriptErrorDetection
    {
        [Test, Ignore("Example code")]
        public void Should_detect_error()
        {
            using (var ie = new IE())
            {
                 var ieClass = (InternetExplorerClass) ie.InternetExplorer;
                 var doc = (IHTMLDocument2) ieClass.Document;
                 var window = (HTMLWindowEvents_Event) doc.parentWindow;
                 window.onerror += (description, url, line) => Console.WriteLine(@"{0}: '{1}' on line {2}", url, description, line);
                 ie.GoTo(@"D:\Projects\WatiN\Support\ErrorInJavascript\Test.html");
                 ie.GoTo("google.com");
                 ie.GoTo(@"D:\Projects\WatiN\Support\ErrorInJavascript\Test.html");

            }
        }
    }
}
