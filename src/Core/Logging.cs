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

using WatiN.Core.Interfaces;

namespace WatiN.Core.Logging
{
  /// <summary>
  /// This class provides some basic means to write actions to a logger class.
  /// WatiN uses this class to log which actions are done while driving Internet
  /// Explorer.
  /// </summary>
  public sealed class Logger
  {
    private static ILogWriter mLogWriter;

    /// <summary>
    /// Prevent creating an instance of this class (contains only static members)
    /// </summary>
    private Logger(){}

    /// <summary>
    /// Logs the action.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void LogAction(string message)
    {
      LogWriter.LogAction(message);
    }

    /// <summary>
    /// Gets or sets the log writer.
    /// </summary>
    /// <value>The log writer.</value>
    public static ILogWriter LogWriter
    {
      get
      {
        if (mLogWriter == null)
        {
          mLogWriter = new NoLog();
        }
        return mLogWriter;
      }
      set
      {
        mLogWriter = value;
      }
    }

  }
  
  public class NoLog : ILogWriter
  {
    #region ILogWriter Members

    public void LogAction(string message)
    {}

    #endregion
  }

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
