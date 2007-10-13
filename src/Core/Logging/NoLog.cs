namespace WatiN.Core.Logging
{
  using WatiN.Core.Interfaces;

  public class NoLog : ILogWriter
  {
    #region ILogWriter Members

    public void LogAction(string message)
    {}

    #endregion
  }
}