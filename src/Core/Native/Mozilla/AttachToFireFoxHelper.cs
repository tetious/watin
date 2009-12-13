using System;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native.Mozilla
{
    public class AttachToFireFoxHelper :IAttachTo
    {
        private FireFox FindFireFox(Constraint findBy)
        {
            var clientPort = FireFox.GetClientPort();
            clientPort.ConnectToExisting();
            
            var ffBrowser = new FFBrowser(clientPort);
            var windowCount = ffBrowser.WindowCount;

            for (var i = 0; i < windowCount; i++)
            {
                ((FireFoxClientPort)ffBrowser.ClientPort).DefineDefaultJSVariablesForWindow(i);
                ffBrowser.ClientPort.InitializeDocument();
                var firefox = CreateBrowserInstance(ffBrowser);
                if (firefox.Matches(findBy)) 
                    return firefox;
            }

            clientPort.CloseConnection();

            return null;
        }

        protected virtual FireFox CreateBrowserInstance(FFBrowser ffBrowser)
        {
            return new FireFox(ffBrowser);
        }

        public Browser Find(Constraint findBy, int timeout, bool waitForComplete)
        {
            Logger.LogAction("Busy finding FireFox matching constraint {0}", findBy);

            var action = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(timeout)) { SleepTime = TimeSpan.FromMilliseconds(500) };
            var fireFox = action.Try(() => FindFireFox(findBy));

            if (fireFox != null)
            {
                if (waitForComplete) fireFox.WaitForComplete();
                return fireFox;
            }

            throw new BrowserNotFoundException("FireFox", findBy.ToString(), timeout);
        }

        public bool Exists(Constraint constraint)
        {
            return FindFireFox(constraint) != null;
        }
    }
}
