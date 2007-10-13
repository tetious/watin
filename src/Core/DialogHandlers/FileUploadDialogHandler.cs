namespace WatiN.Core.DialogHandlers
{
  using System;

  public class FileUploadDialogHandler : BaseDialogHandler
  {
    private String fileName;
    
    public FileUploadDialogHandler(String fileName)
    {
      this.fileName = fileName;  
    }
    
    public override bool HandleDialog(Window window)
    {      
      if (IsFileUploadDialog(window))
      {
        IntPtr usernameControlHandle = NativeMethods.GetChildWindowHwnd(window.Hwnd, "Edit");

        NativeMethods.SetForegroundWindow(usernameControlHandle);
        NativeMethods.SetActiveWindow(usernameControlHandle);

        System.Windows.Forms.SendKeys.SendWait(fileName + "{ENTER}");
        return true;
      }
        
      return false;
    }

    public bool IsFileUploadDialog(Window window)
    {
      // "96CC20C4" is valid for Windows XP, Win 2000 and Win 2003
      // "96CC02C4" is valid for Windows Vista
      return (window.StyleInHex == "96CC20C4") || (window.StyleInHex == "96CC02C4");
    }
  }
}