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