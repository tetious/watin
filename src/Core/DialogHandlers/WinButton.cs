namespace WatiN.Core.DialogHandlers
{
  using System;

  public class WinButton
  {
    private IntPtr hWnd;
    
    public WinButton(IntPtr Hwnd)
    {
      hWnd = Hwnd;
    }

    public WinButton(int buttonid, IntPtr parentHwnd)
    {
      hWnd = NativeMethods.GetDlgItem(parentHwnd, buttonid);
    }
    
    public void Click()
    {
      if (Exists())
      {
        NativeMethods.SendMessage(hWnd, NativeMethods.WM_ACTIVATE, NativeMethods.MA_ACTIVATE, 0);
        NativeMethods.SendMessage(hWnd, NativeMethods.BM_CLICK, 0, 0);
      }
    }
    
    public bool Exists()
    {
      return NativeMethods.IsWindow(hWnd);
    }
    
    public string Title
    {
      get
      {
        return NativeMethods.GetWindowText(hWnd);
      }
    }
    
    public bool Enabled
    {
      get
      {
        return NativeMethods.IsWindowEnabled(hWnd);
      }
    }
    
    public bool Visible
    {
      get
      {
        return NativeMethods.IsWindowVisible(hWnd);
      }
    }
  }
}