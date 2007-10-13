namespace WatiN.Core.DialogHandlers
{
  public abstract class BaseDialogHandler : IDialogHandler
  {
    public override bool Equals(object obj)
    {
      if (obj == null) return false;
      
      return (GetType().Equals(obj.GetType()));
    }

    public override int GetHashCode()
    {
      return GetType().ToString().GetHashCode();
    }
    #region IDialogHandler Members

    public abstract bool HandleDialog(Window window);

    #endregion
  }
}