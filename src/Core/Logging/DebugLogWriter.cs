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
			System.Diagnostics.Debug.WriteLine("[Action]: " + message);
		}

	    public void LogDebug(string message)
	    {
            System.Diagnostics.Debug.WriteLine("[Debug] : " + message);
        }

        public void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine("[Info] : " + message);
        }
	}
}