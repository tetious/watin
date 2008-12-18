using System;
using System.Threading;

namespace WatiN.Core.UtilityClasses
{
    public delegate T TryAction<T>();
    public delegate string BuildTimeOutExceptionMessage();
    
    public class TryActionUntilTimeOut
    {
        public int SleepTime { get; set; }
        public int Timeout { get; private set; }
        public Exception LastException { get; private set; }
        public bool DidTimeOut { get; private set; }
        public BuildTimeOutExceptionMessage ExceptionMessage { get; set; }

        public TryActionUntilTimeOut(int timeout)
        {
            Timeout = timeout;
            SleepTime = Settings.SleepTime;
        }

        public T Try<T>(TryAction<T> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            var timeoutTimer = new SimpleTimer(Timeout);

            var defaultT = default(T);

            do
            {
                LastException = null;

                try
                {
                    var result = action.Invoke();
                    if (!result.Equals(defaultT)) return result;
                }
                catch (Exception e)
                {
                    LastException = e;
                }

                Sleep(SleepTime);
            } while (!timeoutTimer.Elapsed);

            DidTimeOut = true;

            if (ExceptionMessage != null)
            {
                ThrowTimeOutException(LastException, ExceptionMessage.Invoke());
            }

            return defaultT;
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
