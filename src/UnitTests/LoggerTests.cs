#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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

namespace WatiN.Core.UnitTests
{
  using NUnit.Framework;
  using Rhino.Mocks;
  using WatiN.Core.Interfaces;
  using WatiN.Core.Logging;

  [TestFixture]
  public class LoggerTests
  {
    private const string LogMessage = "Call LogAction on mock";

    private MockRepository mocks;
    private ILogWriter mockLogWriter;
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

    [SetUp]
    public void SetUp()
    {
      mocks = new MockRepository();
      mockLogWriter = (ILogWriter) mocks.CreateMock(typeof (ILogWriter));
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
      mockLogWriter.LogAction(LogMessage);

      mocks.ReplayAll();

      Logger.LogWriter = mockLogWriter;
      Logger.LogAction(LogMessage);

      mocks.VerifyAll();
    }

    [Test]
    public void LogActionShouldCallLogActionOnLogWriterInstance2()
    {
      Logger.LogWriter = mockLogWriter;
      mockLogWriter.LogAction(LogMessage);

      mocks.ReplayAll();

      Logger.LogAction(LogMessage);

      mocks.VerifyAll();
    }
  }
}