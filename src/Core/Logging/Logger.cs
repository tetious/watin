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
		/// Logs the action. It replaces the format item(s) in the 
		/// <paramref name="message"/> with the text equivalent  of the value
		/// of a corresponding Object instance in the <paramref name="args"/>
		/// array.
		/// </summary>
		/// <param name="message">A message containing zero or more format items.</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <example>
		/// Call this function from your code like this:
		/// <code>
		/// Logger.LogAction("Some message");
		/// </code>
		/// or
		/// <code>
		/// Logger.LogAction("Some message with an {0} to {1}, "item", "format");
		/// </code>
		/// 
		/// </example>
    public static void LogAction(string message, params object[] args)
    {
      LogWriter.LogAction(string.Format(message, args));
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
}
