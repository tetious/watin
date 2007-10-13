namespace WatiN.Core.Comparers
{
  using WatiN.Core.Interfaces;

  public class AlwaysTrueComparer : ICompare
  {
    public bool Compare(string value)
    {
      return true;
    }
  }
}