#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

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
      if (mLogWriter != null)
      {
        LogWriter.LogAction(message);
      }
    }

    /// <summary>
    /// Gets or sets the log writer.
    /// </summary>
    /// <value>The log writer.</value>
    public static ILogWriter LogWriter
    {
      get
      {
        return mLogWriter;
      }
      set
      {
        mLogWriter = value;
      }
    }
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
