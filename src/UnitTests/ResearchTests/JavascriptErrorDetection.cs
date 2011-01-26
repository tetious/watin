using System;
using mshtml;
using NUnit.Framework;
using SHDocVw;

namespace WatiN.Core.UnitTests.ResearchTests
{
    [TestFixture]
    public class JavascriptErrorDetection
    {
        [Test]
        public void Should_detect_error()
        {
            using (var ie = new IE())
            {
                 var ieClass = (InternetExplorerClass) ie.InternetExplorer;
                 var doc = (IHTMLDocument2) ieClass.Document;
                 var window = (HTMLWindowEvents_Event) doc.parentWindow;
                 window.onerror += (description, url, line) => Console.WriteLine("{0}: '{1}' on line {2}", url, description, line);
                 ie.GoTo(@"D:\Projects\WatiN\Support\ErrorInJavascript\Test.html");
                 ie.GoTo("google.com");
                 ie.GoTo(@"D:\Projects\WatiN\Support\ErrorInJavascript\Test.html");

            }
        }
    }
}
