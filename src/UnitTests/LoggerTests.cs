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

using Moq;
using NUnit.Framework;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class LoggerTests
	{
		private const string LogMessage = "Call LogAction on mock";

		private ILogWriter originalLogWriter;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			originalLogWriter = Logger.LogWriter;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			Logger.LogWriter = originalLogWriter;
		}

		[Test]
		public void SettingLogWriterToNullShouldReturnNoLogClass()
		{
			Logger.LogWriter = null;
			Assert.IsInstanceOfType(typeof (NoLog), Logger.LogWriter);
		}

		[Test]
		public void SettingLogWriterShouldReturnThatLogWriter()
		{
			Logger.LogWriter = new DebugLogWriter();
			Assert.IsInstanceOfType(typeof (DebugLogWriter), Logger.LogWriter);
		}

		[Test]
		public void LogActionShouldCallLogActionOnLogWriterInstance()
		{
            // GIVEN
            var logWriterMock = new Mock<ILogWriter>();
			Logger.LogWriter = logWriterMock.Object;

            // WHEN
            Logger.LogAction(LogMessage);

            // THEN
            logWriterMock.Verify(writer => writer.LogAction(LogMessage));
        }

		[Test]
		public void LogActionWithParamsShouldCallLogActionOnLogWriterInstance()
		{
            // GIVEN
            var logWriterMock = new Mock<ILogWriter>();
            Logger.LogWriter = logWriterMock.Object;

            // WHEN
			Logger.LogAction("Test {0} and {1}", "this", "that");

            // THEN
            logWriterMock.Verify(writer => writer.LogAction("Test this and that"));
        }

		[Test]
		public void LogActionShouldCallLogActionOnLogWriterInstance2()
		{
            // GIVEN
            var logWriterMock = new Mock<ILogWriter>();
            Logger.LogWriter = logWriterMock.Object;

            // WHEN
			Logger.LogAction(LogMessage);

            // THEN
			logWriterMock.Verify(writer => writer.LogAction(LogMessage));
		}

        [Test]
        public void LogActionShouldWriteAFile()
        {
            const string filename = "logtest.txt";
            using (var writer = new FileLogWriter(filename))
            {
                Logger.LogWriter = writer;
                Logger.LogAction(LogMessage);
            }
            string filecontent = System.IO.File.ReadAllText(filename).Trim();
            System.IO.File.Delete(filename);

            Assert.AreEqual("[Action]: "+LogMessage, filecontent, "file did not contain expected string");
        }

        [Test]
        public void LogActionShouldAddText()
        {
            using (var writer = new StringLogWriter())
            {
                Logger.LogWriter = writer;
                Logger.LogAction(LogMessage);
                Assert.AreNotEqual(0,writer.LogString.Length, "string did not appear");
                Assert.AreEqual("[Action]: " + LogMessage, writer.LogString.Trim(), "string did not match");
            }
        }
	}
}
