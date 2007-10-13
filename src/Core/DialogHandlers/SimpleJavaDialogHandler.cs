namespace WatiN.Core.DialogHandlers
{
  public class SimpleJavaDialogHandler : BaseDialogHandler
  {
    JavaDialogHandler dialogHandler;
    bool clickCancelButton = false;
    private bool hasHandledDialog = false;
    private string message;
    
    public SimpleJavaDialogHandler()
    {
      dialogHandler = new AlertDialogHandler();  
    }
    
    public SimpleJavaDialogHandler(bool clickCancelButton)
    {
      this.clickCancelButton = clickCancelButton;
      dialogHandler = new ConfirmDialogHandler();
    }

    public string Message
    {
      get { return message; }
    }

    public bool HasHandledDialog
    {
      get { return hasHandledDialog; }
    }

    public override bool HandleDialog(Window window)
    {
      if (dialogHandler.CanHandleDialog(window))
      {
        dialogHandler.window = window;

        message = dialogHandler.Message;

        ConfirmDialogHandler confirmDialogHandler = dialogHandler as ConfirmDialogHandler;

        // hasHandledDialog must be set before the Click and not
        // after because this code executes on a different Thread
        // and could lead to property HasHandledDialog returning false
        // while hasHandledDialog set had to be set.
        hasHandledDialog = true;

        if (confirmDialogHandler != null && clickCancelButton)
        {
          confirmDialogHandler.CancelButton.Click();
        }
        else
        {
          dialogHandler.OKButton.Click();
        }
      }
      
      return hasHandledDialog;
    }
  }
}