using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SHDocVw;
using WatiN.Core.Native.InternetExplorer;

namespace WatiN.Core.UnitTests.IETests
{
    [TestFixture]
    public class ShellWindows2Tests
    {
        [Test]
        public void ShouldCount1IEInstance()
        {
            Process process = null;
            try
            {
                // GIVEN
                process = StartIE("about:blank");
                Assert.That(process, Is.Not.Null, "pre-condition: Expected an IE process");
                
                var browsers = new ShellWindows2();

                // WHEN
                var count = browsers.Count;

                foreach (IWebBrowser2 browser in browsers)
                {
                    Console.WriteLine(browser.LocationURL);
                }
                // THEN
                Assert.That(count, Is.EqualTo(1), "unexpected count");
            }
            finally
            {
                if (process != null) process.Kill();
            }
        }

        [Test]
        public void ShouldBeAbleToEnumerateFoundInstances()
        {
            Process process1 = null;
            Process process2 = null;

            try
            {
                // GIVEN
                process1 = StartIE("about:blank");
                Assert.That(process1, Is.Not.Null, "pre-condition 1: Expected an IE process");
                
                process2 = StartIE("about:blank");
                Assert.That(process2, Is.Not.Null, "pre-condition 2: Expected an IE process");

                var documents2 = new ShellWindows2();

                // WHEN
                var count = documents2.Count;

                // THEN
                Assert.That(count, Is.EqualTo(2), "unexpected count");
            }
            finally
            {
                if (process1 != null) process1.Kill();
                if (process2 != null) process2.Kill();
            }
        }

        private static Process StartIE(string url)
        {
            var m_Proc = Process.Start("IExplore.exe", url);
            if (m_Proc == null) return null;

            // This sleep is necesary to give IE time to fully instantiate.
            // Needed sleep time might differ from machine to machine..
            Thread.Sleep(2000);

            m_Proc.Refresh();

            return m_Proc;
        }
    }
}
