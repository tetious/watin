using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Native.InternetExplorer;

namespace WatiN.Core.UnitTests.IETests
{
    [TestFixture]
    public class ShellWindows2Tests
    {
        [Test]
        public void ShouldHaveACountProperty()
        {
            // GIVEN
            var documents2 = new ShellWindows2();

            // WHEN
            var count = documents2.Count;

            // THEN
            Assert.That(count, Is.EqualTo(0), "unexpected count");
        }

        [Test]
        public void ShouldCount1IEInstance()
        {
            var process = StartIE("about:blank");
            Assert.That(process, Is.Not.Null, "pre-condition: Expected an IE process");

            try
            {
                // GIVEN
                var documents2 = new ShellWindows2();

                // WHEN
                var count = documents2.Count;

                // THEN
                Assert.That(count, Is.EqualTo(1), "unexpected count");
            }
            finally
            {
                process.Kill();
            }
        }

        private static Process StartIE(string url)
        {
            var m_Proc = Process.Start("IExplore.exe", url);
            if (m_Proc == null) return null;

            var retry = 10;

            do
            {
                retry--;

                m_Proc.Refresh();
                var mainWindowHandle = (int)m_Proc.MainWindowHandle;

                if (mainWindowHandle != 0) { break; }

                Thread.Sleep(500);

            } while (retry >= 0);

            return retry == 0 ? null : m_Proc;
        }
    }
}