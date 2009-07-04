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

namespace WatiN.Core.Logging
{
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
	    public ConsoleLogWriter()
	    {
	        IgnoreLogDebug = false;
	    }

		public void LogAction(string message)
		{
			System.Console.WriteLine("[Action]: " + message);
		}

	    public void LogDebug(string message)
	    {
            if (IgnoreLogDebug) return;
            System.Console.WriteLine("[Debug] : " + message);
        }

        public void LogInfo(string message)
        {
            System.Console.WriteLine("[Info] : " + message);
        }

        public bool IgnoreLogDebug { get; set; }
	}
}