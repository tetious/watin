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
        	Logger.LogAction((LogFunction log) => { log("Busy finding FireFox matching constraint {0}", findBy); });

            var action = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(timeout)) { SleepTime = TimeSpan.FromMilliseconds(500) };
            var fireFox = action.Try(() => FindFireFox(findBy));

            if (fireFox != null)
            {
                fireFox.FinishInitialization();
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
