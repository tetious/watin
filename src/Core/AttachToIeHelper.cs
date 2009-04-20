using System;
using SHDocVw;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
    public interface IAttachTo
    {
        Browser Find(Constraint findBy, int timeout, bool waitForComplete);
        bool Exists(Constraint constraint);
    }

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
            return new IE(browser, false);
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