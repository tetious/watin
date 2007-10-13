namespace WatiN.Core.Logging
{
  using WatiN.Core.Interfaces;

  /// <summary>
  /// This logger class writes it's output to the console.
  /// </summary>
  /// <example>
  /// The following code attaches the ConsoleLogWriter to WatiN and writes a
  /// LogAction.
  /// <code>
  /// Logger.LogWriter = new ConsoleLogWriter;
  /// Logger.LogAction("Attached ConsoleLogWriter");
  /// </code>
  /// </example>
  public class ConsoleLogWriter : ILogWriter
  {
    public void LogAction(string message)
    {
      System.Console.WriteLine(message);
    }
  }
}