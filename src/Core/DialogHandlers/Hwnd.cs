using System;

namespace WatiN.Core.DialogHandlers
{
    public class Hwnd : IHwnd
    {
        private IntPtr _hwnd = IntPtr.Zero;

        public Hwnd(IntPtr Hwnd)
        {
            _hwnd = Hwnd;
        }

        public IntPtr hwnd
        {
            get { return _hwnd; }
        }

        public string WindowText
        {
            get { return NativeMethods.GetWindowText(_hwnd); }
        }

        public bool IsWindowEnabled
        {
            get { return NativeMethods.IsWindowEnabled(_hwnd); }
        }

        public bool IsWindow
        {
            get { return NativeMethods.IsWindow(_hwnd); }
        }

        public bool IsWindowVisible
        {
            get { return NativeMethods.IsWindowVisible(_hwnd); }
        }

        #region IHwnd Members

        public string ClassName
        {
            get { return NativeMethods.GetClassName(_hwnd); }
        }

        #endregion

        public void SendMessage(int msg, int wParam, int lParam)
        {
            NativeMethods.SendMessage(_hwnd, msg, wParam, lParam);
        }

        public IntPtr GetDlgItem(int controlid)
        {
            return NativeMethods.GetDlgItem(_hwnd, controlid);
        }
    }
}