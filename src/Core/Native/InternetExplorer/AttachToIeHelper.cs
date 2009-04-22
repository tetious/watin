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
            return new IE(browser.AsIWebBrowser2, false);
        }

        public Browser Find(Constraint findBy, int timeout, bool waitForComplete)
        {
            Logger.LogAction("Busy finding Internet Explorer matching constriant " + findBy);

            var action = new TryFuncUntilTimeOut(timeout) { SleepTime = 500 };
            var ie = action.Try(() => FindIEPartiallyInitialized(findBy));
            if (ie != null)
            {
                ie.FinishInitialization(null);

                if (waitForComplete)
                    ie.WaitForComplete();

                return ie;
            }

            throw new IENotFoundException(findBy.ToString(), timeout);
        }

        public bool Exists(Constraint constraint)
        {
            return null != FindIEPartiallyInitialized(constraint);
        }
    }
}