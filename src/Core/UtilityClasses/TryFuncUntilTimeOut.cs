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

namespace WatiN.Core.UtilityClasses
{
    public delegate T DoFunc<T>();

    public delegate string BuildTimeOutExceptionMessage();

    /// <summary>
    /// This class provides an easy way of retrying an action for a given number of seconds.
    /// <example>
    /// The following code shows a basic usage:
    /// <code>
    /// var action = new TryFuncUntilTimeOut(5);
    /// var result = action.Try(() => false == true);
    /// </code>
    /// </example>
    /// </summary>
    public class TryFuncUntilTimeOut
    {
        private readonly SimpleTimer _timer;
        private readonly TimeSpan _timeout;

        /// <summary>
        /// Gets or sets the maximum interval between retries of the action.
        /// </summary>
        public TimeSpan SleepTime { get; set; }
        
        /// <summary>
        /// Returns the time out period.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout
        {
            get { return _timer != null ? _timer.Timeout : _timeout; }
        }
        
        /// <summary>
        /// Returns the last exception (thrown by the action) before the time out occured.
        /// </summary>
        /// <value>The last exception.</value>
        public Exception LastException { get; private set; }
        
        /// <summary>
        /// Returns a value indicating whether a time out occured.
        /// </summary>
        /// <value><c>true</c> if did time out; otherwise, <c>false</c>.</value>
        public bool DidTimeOut { get; private set; }
        
        /// <summary>
        /// Gets or sets the exception message. If set a <see cref="TimeoutException"/> will be thrown
        /// if the action did time out.
        /// </summary>
        /// <value>The exception message.</value>
        public BuildTimeOutExceptionMessage ExceptionMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TryFuncUntilTimeOut"/> class.
        /// </summary>
        /// <param name="timeout">The timeout in seconds.</param>
        public TryFuncUntilTimeOut(TimeSpan timeout)
            : this()
        {
            _timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TryFuncUntilTimeOut"/> class.
        /// </summary>
        /// <param name="timer">The timer instance which will be used when executing <see cref="Try{T}(DoFunc{T})"/>.</param>
        public TryFuncUntilTimeOut(SimpleTimer timer)
            : this()
        {
            _timer = timer;
        }

        private TryFuncUntilTimeOut()
        {
            SleepTime = TimeSpan.FromMilliseconds(Settings.SleepTime);
        }

        public static T Try<T>(TimeSpan timeout, DoFunc<T> func)
        {
            var tryFunc = new TryFuncUntilTimeOut(timeout);
            return tryFunc.Try(func);
        }

        /// <summary>
        /// Tries the specified action until the result of the action is not equal to <c>default{T}</c>
        /// or the time out is reached.
        /// </summary>
        /// <typeparam name="T">The result type of the action</typeparam>
        /// <param name="func">The action.</param>
        /// <returns>The result of the action of <c>default{T}</c> when time out occured.</returns>
        public T Try<T>(DoFunc<T> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            var timeoutTimer = GetTimer();

            var currentSleepTime = TimeSpan.FromMilliseconds(1);
            do
            {
                LastException = null;

                try
                {
                    var result = func.Invoke();
                    if (!Equals(result, default(T)))
                        return result;
                }
                catch (Exception e)
                {
                    LastException = e;
                }

                Sleep(currentSleepTime);

                currentSleepTime += currentSleepTime;
                if (currentSleepTime > SleepTime)
                    currentSleepTime = SleepTime;
            } while (!timeoutTimer.Elapsed);

            HandleTimeOut();

            return default(T);
        }

        protected virtual void Sleep(TimeSpan sleepTime)
        {
            Thread.Sleep(sleepTime);
        }

        private SimpleTimer GetTimer()
        {
            return _timer ?? new SimpleTimer(Timeout);
        }

        private void HandleTimeOut()
        {
            DidTimeOut = true;

            if (ExceptionMessage != null)
            {
                ThrowTimeOutException(LastException, ExceptionMessage.Invoke());
            }
        }

        private static void ThrowTimeOutException(Exception lastException, string message)
        {
            if (lastException != null)
            {
                throw new Exceptions.TimeoutException(message, lastException);
            }
            throw new Exceptions.TimeoutException(message);
        }
    }
}
