namespace WatiN.Core.Logging
{
  using WatiN.Core.Interfaces;

  /// <summary>
  /// This logger class writes it's output to the debug window.
  /// </summary>
  /// <example>
  /// The following code attaches the DebugLogWriter to WatiN and writes a
  /// LogAction. Open de output window in your debugger to see the result.
  /// <code>
  /// Logger.LogWriter = new DebugLogWriter;
  /// Logger.LogAction("Attached DebugLogWriter");
  /// </code>
  /// </example>
  public class DebugLogWriter : ILogWriter
  {
    public void LogAction(string message)
    {
      System.Diagnostics.Debug.WriteLine(message);
    }
  }
}