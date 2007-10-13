namespace WatiN.Core.Comparers
{
  using System;

  /// <summary>
  /// Class that supports a simple matching of two strings.
  /// </summary>
  public class StringEqualsAndCaseInsensitiveComparer : StringComparer
  {
    public StringEqualsAndCaseInsensitiveComparer(string value) : base(value)
    {
    }

    public override bool Compare(string value)
    {
      if (value == null) return false;

      return (String.Compare(value, valueToCompareWith, true) == 0);
    }

    public override string ToString()
    {
      return valueToCompareWith;
    }
  }
}