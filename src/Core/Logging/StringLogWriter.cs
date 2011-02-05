#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using System.Text;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Logging
{
    /// <summary>
    /// This logger class writes it's output to a string
    /// </summary>
    /// <example>
    /// The following code attaches the StringLogWriter to WatiN and writes a
    /// LogAction.
    /// <code>
    /// Logger.LogWriter = new StringLogWriter();
    /// Logger.LogAction("Attached StringLogWriter");
    /// </code>
    /// </example>
    public class StringLogWriter : BaseLogWriter, IDisposable
    {
        /// <summary>
        /// private StringBuilder keeping the log
        /// </summary>
        private readonly StringBuilder Builder = new StringBuilder();

        /// <summary>
        /// flag to indicate inclusion of a timestamp (in "yyyy-mm-ddThh:nn:ss" format)
        /// </summary>
        public bool IncludeTimestamp { get; set; }

        /// <summary>
        /// data string containing the log
        /// </summary>
        public string LogString
        {
            get { return Builder.ToString(); }
        }

        /// <summary>
        /// private method to write the log line
        /// </summary>
        /// <param name="message">message to write</param>
        private void WriteLogLine(string message)
        {
            var line = IncludeTimestamp ? DateTime.Now.ToString("s") + " " + message : message;
            Builder.AppendLine(line);
        }

        protected override void LogActionImpl(string message)
        {
            WriteLogLine("[Action]: " + message);
        }

        protected override void LogDebugImpl(string message)
        {
            WriteLogLine("[Debug ]: " + message);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Builder.Length = 0;
        }
    }
}