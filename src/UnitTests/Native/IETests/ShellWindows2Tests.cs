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
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SHDocVw;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.UnitTests.TestUtils;

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

                process2 = StartIE(BaseWatiNTest.FramesetURI.AbsolutePath);
                Assert.That(process2, Is.Not.Null, "pre-condition 2: Expected an IE process");

                var browsers = new ShellWindows2();

                // WHEN
                var count = browsers.Count;

                foreach (IWebBrowser2 browser in browsers)
                {
                    Console.WriteLine(browser.LocationURL);
                }

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
