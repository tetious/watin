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

using System;
using WatiN.Core.Interfaces;
using System.IO;

namespace WatiN.Core.Logging
{
	/// <summary>
	/// This logger class writes it's output to a file
	/// </summary>
	/// <example>
	/// The following code attaches the FileLogWriter to WatiN and writes a
	/// LogAction.
	/// <code>
	/// Logger.LogWriter = new FileLogWriter("LogFile.txt");
    /// Logger.LogAction("Attached FileLogWriter");
	/// </code>
	/// </example>
    public class FileLogWriter : ILogWriter, IDisposable
    {
        private readonly StreamWriter LogStream;

        /// <summary>
        /// Constructor method creating a text file for writing
        /// </summary>
        /// <param name="filename">full filepath to write the new log</param>
        public FileLogWriter(string filename)
        {
            LogStream = File.CreateText(filename);
        }

        public void Dispose()
        {
            LogStream.Close();
        }

        /// <summary>
        /// flag to indicate inclusion of a timestamp (in "yyyy-mm-ddThh:nn:ss" format)
        /// </summary>
        public bool IncludeTimestamp { get; set; }

        /// <summary>
        /// private method to write the log line
        /// </summary>
        /// <param name="message">message to write</param>
        private void WriteLogLine(string message)
        {
            var line = IncludeTimestamp ? DateTime.Now.ToString("s") + " " + message : message;
            LogStream.WriteLine(line);
            LogStream.Flush();
        }

        public void LogAction(string message)
        {
            WriteLogLine("[Action]: " + message);
        }

        public void LogDebug(string message)
        {
            WriteLogLine("[Debug ]: " + message);
        }

        public void LogInfo(string message)
        {
            WriteLogLine("[Info  ]: " + message);
        }
    }
}