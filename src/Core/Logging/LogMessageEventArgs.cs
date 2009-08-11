using System;

namespace WatiN.Core.Logging
{
    public class LogMessageEventArgs : EventArgs
    {
        public LogMessageEventArgs(LogMessageType logMessageType, string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            LogMessageType = logMessageType;
            Message = message;
        }

        public LogMessageType LogMessageType { get; private set; }
        public string Message { get; private set; }
    }
}
