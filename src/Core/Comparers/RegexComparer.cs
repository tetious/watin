namespace WatiN.Core.Comparers
{
  using System;
  using System.Text.RegularExpressions;

  /// <summary>
  /// Class that supports matching a regular expression with a string value.
  /// </summary>
  public class RegexComparer : BaseComparer
  {
    private Regex regexToUse;

    public RegexComparer(Regex regex)
    {
      if (regex == null)
      {
        throw new ArgumentNullException("regex");
      }
      regexToUse = regex;
    }

    public override bool Compare(string value)
    {
      if (value == null) return false;

      return regexToUse.IsMatch(value);
    }

    public override string ToString()
    {
      return GetType().ToString() + " matching against: " + regexToUse.ToString();
    }
  }
}