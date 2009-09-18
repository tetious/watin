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

using WatiN.Core.Interfaces;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Logging
{
	/// <summary>
	/// This class provides some basic means to write actions to a logger class.
	/// WatiN uses this class to log which actions are done while driving Internet
	/// Explorer.
	/// </summary>
	public static class Logger
	{
		private static ILogWriter mLogWriter = DefaultLogWriter();

		/// <summary>
		/// Logs the action. These should be messages about "user" actions done by WatiN. 
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
            Log(LogMessageType.Action, UtilityClass.StringFormat(message, args));
		}

		/// <summary>
		/// Logs the debug message. These should be technical messages from within WatiN.
		/// </summary>
		/// <param name="message">A message containing zero or more format items.</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <example>
		/// Call this function from your code like this:
		/// <code>
        /// Logger.LogDebug("Some message");
		/// </code>
		/// or
		/// <code>
        /// Logger.LogDebug("Some message with an {0} to {1}, "item", "format");
		/// </code>
		/// 
		/// </example>
		public static void LogDebug(string message, params object[] args)
		{
            Log(LogMessageType.Debug, UtilityClass.StringFormat(message, args));            
		}

        /// <summary>
        /// Logs the information message. These should be comments and information messages from within WatiN.
        /// </summary>
        /// <param name="message">A message containing zero or more format items.</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        /// <example>
        /// Call this function from your code like this:
        /// <code>
        /// Logger.LogInfo("Some message");
        /// </code>
        /// or
        /// <code>
        /// Logger.LogInfo("Some message with an {0} to {1}, "item", "format");
        /// </code>
        /// 
        /// </example>
        public static void LogInfo(string message, params object[] args)
        {
            Log(LogMessageType.Info, UtilityClass.StringFormat(message, args));
        }

        /// <summary>
        /// base logging method to send data
        /// </summary>
        /// <param name="logType">LogTypes enumeration item</param>
        /// <param name="message">A message containing zero or more format items.</param>
        /// <param name="args">An object array containing zero or more objects to format</param>
        public static void Log(LogMessageType logType, string message, params object[] args)
        {
            var formattedMessage = UtilityClass.StringFormat(message, args);

            if (LogMessage != null)
            {
                LogMessage(null, new LogMessageEventArgs(logType, formattedMessage));
            }

            switch (logType)
            {
                case LogMessageType.Action:
                    LogWriter.LogAction(UtilityClass.StringFormat(message, args));
                    break;
                case LogMessageType.Debug:
                    LogWriter.LogDebug(UtilityClass.StringFormat(message, args));
                    break;
                case LogMessageType.Info:
                    LogWriter.LogInfo(UtilityClass.StringFormat(message, args));
                    break;
            }
        }

		/// <summary>
		/// Gets or sets the log writer.
		/// </summary>
		/// <value>The log writer.</value>
		public static ILogWriter LogWriter
		{
			get { return mLogWriter; }
			set { mLogWriter = value ?? DefaultLogWriter(); }
		}

        private static NoLog DefaultLogWriter()
        {
            return new NoLog();
        }

        public delegate void EventHandle<LogMessageEventArgs>(object sender, LogMessageEventArgs e);

        public static event EventHandle<LogMessageEventArgs> LogMessage;
    }
}