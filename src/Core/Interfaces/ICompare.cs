namespace WatiN.Core.Interfaces
{
  /// <summary>
  /// This interface is used by <see cref="AttributeConstraint"/> to compare a searched attribute
  /// with a given AttributeConstraint.
  /// </summary>
  public interface ICompare
  {
    bool Compare(string value);
  }
}