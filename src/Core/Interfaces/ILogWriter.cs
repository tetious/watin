namespace WatiN.Core.Interfaces
{
  /// <summary>
  /// Implement this interface if you create your own Logger class.
  /// For example <c>Logger.LogWriter = new MyLogWriter</c>.
  /// </summary>
  public interface ILogWriter
  {
    void LogAction(string message);
  }
}