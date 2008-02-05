using System;

namespace WatiN.Core.DialogHandlers
{
    public interface IHwnd
    {
        IntPtr hwnd { get; }

        string WindowText { get; }

        bool IsWindow { get; }
        
        bool IsWindowEnabled { get; }

        bool IsWindowVisible { get; }

        string ClassName { get; }

        void SendMessage(int msg, int wParam, int lParam);
        
        IntPtr GetDlgItem(int controlid);
    }
}