using System;
using System.Threading;

namespace WatiN.Core.UtilityClasses
{
    public delegate T TryFunc<T>();
    public delegate void TryAction();
    public delegate string BuildTimeOutExceptionMessage();

    /// <summary>
    /// This class provides an easy way of retrying an action for a given number of seconds.
    /// <example>
    /// The following code shows a basic usage:
    /// <code>
    /// var action = new TryActionUntilTimeOut(5);
    /// var result = action.Try(() => false == true);
    /// </code>
    /// </example>
    /// </summary>
    public class TryActionUntilTimeOut
    {
        /// <summary>
        /// Gets or sets the interval between retries of the action..
        /// </summary>
        /// <value>The sleep time in milliseconds.</value>
        public int SleepTime { get; set; }
        
        /// <summary>
        /// Returns the time out period in seconds.
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout { get; private set; }
        
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
        /// Initializes a new instance of the <see cref="TryActionUntilTimeOut"/> class.
        /// </summary>
        /// <param name="timeout">The timeout in seconds.</param>
        public TryActionUntilTimeOut(int timeout)
        {
            Timeout = timeout;
            SleepTime = Settings.SleepTime;
        }

        public static T Try<T>(int timeout, TryFunc<T> func)
        {
            var tryAction = new TryActionUntilTimeOut(timeout);
            return tryAction.Try(func);
        }

        /// <summary>
        /// Tries the specified action until the result of the action is not equal to <c>default(T)</c>
        /// or the time out is reached.
        /// </summary>
        /// <typeparam name="T">The result type of the action</typeparam>
        /// <param name="func">The action.</param>
        /// <returns>The result of the action of <c>default(T)</c> when time out occured.</returns>
        public T Try<T>(TryFunc<T> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            var defaultT = default(T);
            var timeoutTimer = new SimpleTimer(Timeout);

            do
            {
                LastException = null;

                try
                {
                    var result = func.Invoke();
                    if (!result.Equals(defaultT)) return result;
                }
                catch (Exception e)
                {
                    LastException = e;
                }

                Sleep(SleepTime);
            } while (!timeoutTimer.Elapsed);

            HandleTimeOut();

            return defaultT;
        }

        private void HandleTimeOut()
        {
            DidTimeOut = true;

            if (ExceptionMessage != null)
            {
                ThrowTimeOutException(LastException, ExceptionMessage.Invoke());
            }
        }

        protected virtual void Sleep(int sleepTime)
        {
            Thread.Sleep(sleepTime);
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
