using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.DialogHandlers
{
    public abstract class WaitUntilHandledDialogHandler : BaseDialogHandler
    {
        public abstract override bool HandleDialog(Window window);
        public abstract override bool CanHandleDialog(Window window);

        public bool HasHandledDialog { get; protected set; }

        public bool WaitUntilHandled()
        {
            return WaitUntilHandled(Settings.WaitForCompleteTimeOut);
        }

        public bool WaitUntilHandled(int timeoutAfterSeconds)
        {
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(timeoutAfterSeconds);
            tryActionUntilTimeOut.Try(() => HasHandledDialog);

            return HasHandledDialog;
        }
    }
}