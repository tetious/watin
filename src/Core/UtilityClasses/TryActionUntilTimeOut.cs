using System;
using System.Threading;

namespace WatiN.Core.UtilityClasses
{
    public delegate bool TryAction();
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

        public bool Try(TryAction action)
        {
            if (action == null) throw new ArgumentNullException("action");

            var timeoutTimer = new SimpleTimer(Timeout);

            do
            {
                LastException = null;

                try
                {
                    if (action.Invoke()) return true;
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
                ThrowTimeOutException(LastException, ExceptionMessage.Invoke() );
            }

            return false;
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
