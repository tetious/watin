using WatiN.Core.Native.Windows;

namespace WatiN.Core.DialogHandlers
{
    public class CloseIEDialogHandler : BaseDialogHandler
    {
        private const int okButtonId = 6;
        private const int cancelButtonId = 7;
        
        private readonly bool _clickOnOK;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseIEDialogHandler"/> class.
        /// </summary>
        /// <param name="clickOnOK">if set to <c>true</c> the OK button will be clicked on. Otherwise the Cancel button will be clicked.</param>
        public CloseIEDialogHandler(bool clickOnOK)
        {
            _clickOnOK = clickOnOK;
        }

        public override bool HandleDialog(Window window)
        {
            if (CanHandleDialog(window))
            {
                var buttonId = _clickOnOK ? okButtonId : cancelButtonId;

                var button = new WinButton(buttonId, window.Hwnd);
                button.Click();

                return true;
            }
            return false;
        }

        public virtual bool CanHandleDialog(Window window)
        {
            return (window.StyleInHex == "94C801C5");
        }
    }
}
