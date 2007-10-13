namespace WatiN.Core.DialogHandlers
{
  using System;
  using System.Threading;
  using WatiN.Core.Exceptions;

  public abstract class JavaDialogHandler : BaseDialogHandler
  {
    internal Window window;

    public string Title
    {
      get
      {
        ThrowExceptionIfDialogDoesNotExist();
        
        return window.Title;
      }
    }

    public string Message
    {
      get
      {
        ThrowExceptionIfDialogDoesNotExist();
        
        IntPtr messagehWnd = NativeMethods.GetDlgItem(window.Hwnd, 65535);
        return NativeMethods.GetWindowText(messagehWnd);
      }
    }

    public WinButton OKButton
    {
      get
      {
        ThrowExceptionIfDialogDoesNotExist();
        
        return new WinButton(getOKButtonID(), window.Hwnd );
      }
    }

    public override bool HandleDialog(Window window)
    {
      if (CanHandleDialog(window))
      {
        this.window = window;
      
        while(window.Exists())
        {
          Thread.Sleep(200);
        }
        return true;
        
      }
      return false;
    }

    public void WaitUntilExists()
    {
      WaitUntilExists(30);
    }
    
    public void WaitUntilExists(int waitDurationInSeconds)
    {
      SimpleTimer timeoutTimer = new SimpleTimer(waitDurationInSeconds);

      while (!Exists() && !timeoutTimer.Elapsed)
      {
        Thread.Sleep(200);
      }
      
      if (!Exists())
      {
        throw new WatiNException(string.Format("Dialog not available within {0} seconds.", waitDurationInSeconds.ToString()));
      }
    }

    public abstract bool CanHandleDialog(Window window);

    public bool Exists()
    {
      if (window == null) return false;
      
      return window.Exists();
    }

    protected abstract int getOKButtonID();

    protected bool ButtonWithId1Exists(IntPtr windowHwnd)
    {
      WinButton button = new WinButton(1, windowHwnd);
      return button.Exists();
    }

    protected WinButton createCancelButton(IntPtr windowHwnd)
    {
      return new WinButton(2, windowHwnd );
    }

    protected void ThrowExceptionIfDialogDoesNotExist()
    {
      if (!Exists())
      {
        throw new WatiNException("Operation not available. Dialog doesn't exist.");
      }
    }
  }
}