namespace WatiN.Core.Comparers
{
  using WatiN.Core.Interfaces;

  public abstract class BaseComparer : ICompare
  {
    public virtual bool Compare(string value)
    {
      return false;
    }
  }
}