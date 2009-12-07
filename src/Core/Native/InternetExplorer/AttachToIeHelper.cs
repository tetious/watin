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
using SHDocVw;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native.InternetExplorer
{
    public class AttachToIeHelper : IAttachTo
    {
        internal IE FindIEPartiallyInitialized(Constraint findBy)
        {
            var allBrowsers = new ShellWindows2();

            var context = new ConstraintContext();
            foreach (IWebBrowser2 browser in allBrowsers)
            {
                var ie = CreateBrowserInstance(new IEBrowser(browser));
                if (ie.Matches(findBy, context))
                    return ie;
            }

            return null;
        }

        protected virtual IE CreateBrowserInstance(IEBrowser browser)
        {
            return new IE(browser.WebBrowser, false);
        }

        public Browser Find(Constraint findBy, int timeout, bool waitForComplete)
        {
            Logger.LogAction("Busy finding Internet Explorer matching constraint {0}", findBy);

            var timer = new SimpleTimer(TimeSpan.FromSeconds(timeout));

            var ie = TryFindIe(findBy, timer);
            
            if (ie != null)
            {
                return FinishInitializationAndWaitForComplete(ie, timer, waitForComplete);
            }

            throw new BrowserNotFoundException("IE", findBy.ToString(), timeout);
        }

        private Browser FinishInitializationAndWaitForComplete(IE ie, SimpleTimer timer, bool waitForComplete)
        {
            ie.FinishInitialization(null);

            if (waitForComplete)
            {
                var ieWaitForComplete = new IEWaitForComplete((IEBrowser) ie.NativeBrowser) { Timer = timer };
                ie.WaitForComplete(ieWaitForComplete);
            }

            return ie;
        }

        private IE TryFindIe(Constraint findBy, SimpleTimer timer)
        {
            var action = new TryFuncUntilTimeOut(timer)
            {
                SleepTime = TimeSpan.FromMilliseconds(500)
            };

            return action.Try(() => FindIEPartiallyInitialized(findBy));
        }

        public bool Exists(Constraint constraint)
        {
            return null != FindIEPartiallyInitialized(constraint);
        }
    }
}