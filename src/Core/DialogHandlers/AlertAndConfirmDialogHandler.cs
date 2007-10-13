namespace WatiN.Core.DialogHandlers
{
  using System;
  using System.Collections;
  using WatiN.Core.Exceptions;

  public class AlertAndConfirmDialogHandler : BaseDialogHandler
  {
    private Queue alertQueue;

    public AlertAndConfirmDialogHandler()
    {
      alertQueue = new Queue();
    }

    /// <summary>
    /// Gets the count of the messages in the que of displayed alert and confirm windows.
    /// </summary>
    /// <value>The count of the alert and confirm messages in the que.</value>
    public int Count
    {
      get
      {
        return alertQueue.Count;
      }
    }

    /// <summary>
    /// Pops the most recent message from a que of displayed alert and confirm windows.
    /// Use this method to get the displayed message.
    /// </summary>
    /// <returns>The displayed message.</returns>
    public string Pop()
    {
      if (alertQueue.Count == 0)
      {
        throw new MissingAlertException();
      }

      return (string) alertQueue.Dequeue();
    }
    
    /// <summary>
    /// Gets the alert and confirm messages in the que of displayed alert and confirm windows.
    /// </summary>
    /// <value>The alert and confirm messages in the que.</value>
    public string[] Alerts
    {
      get
      {
        string[] result = new string[alertQueue.Count];
        Array.Copy(alertQueue.ToArray(), result, alertQueue.Count);
        return result;
      }
    }

    /// <summary>
    /// Clears all the messages from the que of displayed alert and confirm windows.
    /// </summary>
    public void Clear()
    {
      alertQueue.Clear();
    }

    public override bool HandleDialog(Window window)
    {
      // See if the dialog has a static control with a controlID 
      // of 0xFFFF. This is unique for alert and confirm dialogboxes.
      IntPtr handle = NativeMethods.GetDlgItem(window.Hwnd, 0xFFFF);

      if (handle != IntPtr.Zero)
      {
        alertQueue.Enqueue(NativeMethods.GetWindowText(handle));
        
        window.ForceClose();
      
        return true;
      }

      return false;
    }
  }
}