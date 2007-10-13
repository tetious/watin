namespace WatiN.Core.DialogHandlers
{
  using System;

  public class Window
  {
    private IntPtr hwnd;
    
    public Window(IntPtr hwnd)
    {
      this.hwnd = hwnd;
    }
    
    public virtual IntPtr Hwnd
    {
      get
      {
        return hwnd;
      }
    }
    
    public virtual IntPtr ParentHwnd
    {
      get
      {
        return NativeMethods.GetParent(Hwnd);
      }
    }

    public virtual string Title
    {
      get
      {
        return NativeMethods.GetWindowText(Hwnd);
      }
    }

    public virtual string ClassName
    {
      get
      {
        return NativeMethods.GetClassName(Hwnd);
      }
    }

    public virtual Int64 Style
    {
      get
      {
        return NativeMethods.GetWindowStyle(Hwnd);
      }
    }
    public virtual string StyleInHex
    {
      get
      {
        return Style.ToString("X");
      }
    }
    
    public virtual bool IsDialog()
    {
      return (ClassName == "#32770");
    }

    public virtual void ForceClose()
    {
      NativeMethods.SendMessage(Hwnd, NativeMethods.WM_CLOSE, 0, 0);
    }
    
    public virtual bool Exists()
    {
      return NativeMethods.IsWindow(Hwnd);
    }
    
    public virtual bool Visible
    {
      get
      {
        return NativeMethods.IsWindowVisible(Hwnd);
      }
    }

    public virtual void ToFront()
    {
      NativeMethods.SetForegroundWindow(Hwnd);
    }

    public virtual void SetActivate()
    {
      NativeMethods.SetActiveWindow(Hwnd);
    }
  }
}