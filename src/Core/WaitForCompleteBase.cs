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
using System.Threading;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
    public abstract class WaitForCompleteBase : IWait
    {
        private SimpleTimer _timer;
        private readonly int _waitForCompleteTimeOut;

        protected WaitForCompleteBase(int waitForCompleteTimeOut)
        {
            _waitForCompleteTimeOut = waitForCompleteTimeOut;
            MilliSecondsTimeOut = Settings.SleepTime;
        }

        public int MilliSecondsTimeOut { get; set; }

        public SimpleTimer Timer
        {
            get
            {
                if (_timer == null)
                {
                    _timer = new SimpleTimer(TimeSpan.FromSeconds(_waitForCompleteTimeOut));
                }
                return _timer;
            }
            set { _timer = value; }
        }

        /// <summary>
        /// This method calls InitTimeOut and waits till IE is ready
        /// processing or the timeout period has expired.
        /// </summary>
        public virtual void DoWait()
        {
            InitialSleep();

            InitTimeout();
            WaitForCompleteOrTimeout();
        }

        /// <summary>
        /// Waits an initial small sleep time (10ms default) to allow the browser some
        /// time to perform any immediately pending asynchronous operations that might
        /// cause it to enter a busy state.
        /// </summary>
        protected virtual void InitialSleep()
        {
            Thread.Sleep(10);
        }

        /// <summary>
        /// This method waits till IE is ready processing 
        /// or the timeout period has expired. You should
        /// call InitTimeout prior to calling this method.
        /// </summary>
        protected abstract void WaitForCompleteOrTimeout();

        /// <summary>
        /// This method is called to initialise the start time for
        /// determining a time out. It's set to the current time.
        /// </summary>
        /// <returns></returns>
        protected virtual SimpleTimer InitTimeout()
        {
            _timer = null;
            return Timer;
        }

        /// <summary>
        /// This method evaluates the time between the last call to InitTimeOut
        /// and the current time. If the timespan is more than 30 seconds, the
        /// return value will be true.
        /// </summary>
        /// <returns>If the timespan is more than 30 seconds, the
        /// return value will be true</returns>
        protected virtual bool IsTimedOut()
        {
            return Timer.Elapsed;
        }

        /// <summary>
        /// This method checks the return value of IsTimedOut. When true, it will
        /// throw a TimeoutException with the timeoutMessage param as message.
        /// </summary>
        /// <param name="timeoutMessage">The message to present when the TimeoutException is thrown</param>
        protected virtual void ThrowExceptionWhenTimeout(string timeoutMessage)
        {
            if (IsTimedOut())
            {
                throw new Exceptions.TimeoutException(timeoutMessage);
            }
        }

        protected void WaitUntil(DoFunc<bool> waitWhile, BuildTimeOutExceptionMessage exceptionMessage)
        {
            if (Timer == null)
                throw new WatiNException("_waitForCompleteTimer not initialized");
            
            var timeOut = new TryFuncUntilTimeOut(Timer) {ExceptionMessage = exceptionMessage};
            timeOut.Try(waitWhile);
        }
    }
}