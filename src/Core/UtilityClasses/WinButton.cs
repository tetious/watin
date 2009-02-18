using System;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Logging;

namespace WatiN.Core.UtilityClasses
{
    public class WinButton
    {
        private readonly IHwnd _hWnd;

        public WinButton(IntPtr Hwnd) : this(new Hwnd(Hwnd))
        {}

        public WinButton(int buttonid, IntPtr parentHwnd) : this(buttonid, new Hwnd(parentHwnd))
        {}

        public WinButton(IHwnd Hwnd)
        {
            _hWnd = Hwnd;
        }

        public WinButton(int buttonid, IHwnd parentHwnd)
        {
            _hWnd = new Hwnd(parentHwnd.GetDlgItem(buttonid));
        }

        public void Click()
        {
            if (!Exists()) return;

            Logger.LogAction("Clicking on '{0}'", Title);

            _hWnd.SendMessage(NativeMethods.WM_ACTIVATE, NativeMethods.MA_ACTIVATE, 0);
            _hWnd.SendMessage(NativeMethods.BM_CLICK, 0, 0);
        }

        public bool Exists()
        {
            return _hWnd.IsWindow && _hWnd.ClassName.Equals("Button");
        }

        public string Title
        {
            get { return _hWnd.WindowText; }
        }

        public bool Enabled
        {
            get { return _hWnd.IsWindowEnabled; }
        }

        public bool Visible
        {
            get { return _hWnd.IsWindowVisible; }
        }
    }
}