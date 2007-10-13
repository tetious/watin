namespace WatiN.Core.Comparers
{
  using System;

  /// <summary>
  /// Class that supports a simple matching of two strings.
  /// </summary>
  public class StringContainsAndCaseInsensitiveComparer : StringComparer
  {
    public StringContainsAndCaseInsensitiveComparer(string value) : base(value)
    {
      valueToCompareWith = value.ToLower();
    }

    public override bool Compare(string value)
    {
      if (value == null)
      {
        return false;
      }

      if (valueToCompareWith == String.Empty & value != String.Empty)
      {
        return false;
      }

      return (value.ToLower().IndexOf(valueToCompareWith) >= 0);
    }
  }
}