namespace WatiN.Core.DialogHandlers
{
  using System;

  public class UseDialogOnce : IDisposable
  {
    private DialogWatcher dialogWatcher;
    private IDialogHandler dialogHandler;

    public UseDialogOnce(DialogWatcher dialogWatcher, IDialogHandler dialogHandler)
    {
      if (dialogWatcher == null)
      {
        throw new ArgumentNullException("dialogWatcher");
      }

      if (dialogHandler == null)
      {
        throw new ArgumentNullException("dialogHandler");
      }

      this.dialogWatcher = dialogWatcher;
      this.dialogHandler = dialogHandler;

      dialogWatcher.Add(dialogHandler);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
      return;
    }

    protected virtual void Dispose(bool managedAndNative)
    {
      dialogWatcher.Remove(dialogHandler);

      dialogWatcher = null;
      dialogHandler = null;
    }
  }
}