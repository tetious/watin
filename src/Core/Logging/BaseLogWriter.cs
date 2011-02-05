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

using WatiN.Core.Interfaces;

namespace WatiN.Core.Logging
{
	/// <summary>
	/// This logger class can be used as a base class for your specific log class.
	/// </summary>
	public abstract class BaseLogWriter : ILogWriter
	{
		public BaseLogWriter()
		{
			HandlesLogAction = true;
			HandlesLogDebug = true;
		}
		
	    public bool HandlesLogAction { get; set; }		
	    public bool HandlesLogDebug { get; set; }

	    
	    public void LogAction(string message)
		{
	    	if (HandlesLogAction) LogActionImpl(message);
		}

	    public void LogDebug(string message)
	    {
	    	if (HandlesLogDebug) LogDebugImpl(message);
        }
	    
	    protected abstract void LogActionImpl(string message);
	    protected abstract void LogDebugImpl(string message);
	}
}