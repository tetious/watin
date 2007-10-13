namespace WatiN.Core.Comparers
{
  using System;

  /// <summary>
  /// Class that supports an exact comparison of two string values.
  /// </summary>
  public class StringComparer : BaseComparer
  {
    protected string valueToCompareWith;

    public StringComparer(string value)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      valueToCompareWith = value;
    }

    public override bool Compare(string value)
    {
      if (value != null && valueToCompareWith.Equals(value))
      {
        return true;
      }
      return false;
    }

    public override string ToString()
    {
      return valueToCompareWith;
    }
  }
}