#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using System.Timers;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides a simple way to handle loops that have to time out after 
	/// a specified number of seconds.
	/// </summary>
	/// <example>
	/// This is an example how you could use this class in your code.
	/// <code>
	/// // timer should elapse after 30 seconds
	/// SimpleTimer timeoutTimer = new SimpleTimer(30);
	///
	/// do
	/// {
	///   // Your check logic goes here
	///   
	///   // wait 200 miliseconds
	///   Thread.Sleep(200);
	/// } while (!timeoutTimer.Elapsed);
	/// </code>
	/// </example>
	public class SimpleTimer
	{
		private Timer clock;

	    private int _timeout;

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleTimer"/> class.
		/// </summary>
		/// <param name="timeout">The timeout.</param>
		public SimpleTimer(int timeout)
		{
			if (timeout < 0)
			{
				throw new ArgumentOutOfRangeException("timeout", timeout, "Should be equal are greater then zero.");
			}

		    _timeout = timeout;

			if (timeout > 0)
			{
				clock = new Timer(timeout*1000);
				clock.AutoReset = false;
				clock.Elapsed += new ElapsedEventHandler(ElapsedEvent);
				clock.Start();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="SimpleTimer"/> is elapsed.
		/// </summary>
		/// <value><c>true</c> if elapsed; otherwise, <c>false</c>.</value>
		public bool Elapsed
		{
			get { return (clock == null); }
		}

        /// <summary>
        /// The number of seconds after which this timer times out. The time out can only be
        /// set through the constructor.
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
        }
        
        private void ElapsedEvent(object source, ElapsedEventArgs e)
		{
			clock.Stop();
			clock.Close();
			clock = null;
		}
	}
}