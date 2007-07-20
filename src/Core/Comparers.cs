namespace WatiN.Core.Interfaces
{
  /// <summary>
  /// This interface is used by <see cref="Attribute"/> to compare a searched attribute
  /// with a given attribute.
  /// </summary>
  public interface ICompare
  {
    bool Compare(string value);
  }

  public interface IAttributeBag
  {
    string GetValue(string attributename);
  }
}

namespace WatiN.Core
{
  using System;
  using System.Text.RegularExpressions;
  using WatiN.Core.Interfaces;

  public abstract class BaseComparer : ICompare
  {
    public virtual bool Compare(string value)
    {
      return false;
    }
  }

  /// <summary>
  /// Class that supports comparing a <see cref="Uri"/> instance with a string value.
  /// </summary>
  public class UriComparer : BaseComparer
  {
    private Uri uriToCompareWith;
    private bool _ignoreQuery = false;

    /// <summary>
    /// Constructor, querystring will not be ignored in comparisons.
    /// </summary>
    /// <param name="uri">Uri for comparison.</param>
    public UriComparer(Uri uri) : this(uri, false)
    {
    }

    /// <summary>
    /// Constructor, querystring can be ignored or not ignored in comparisons.
    /// </summary>
    /// <param name="uri">Uri for comparison.</param>
    /// <param name="ignoreQuery">Set to true to ignore querystrings in comparison.</param>
    public UriComparer(Uri uri, bool ignoreQuery)
    {
      if (uri == null)
      {
        throw new ArgumentNullException("uri");
      }
      uriToCompareWith = uri;
      _ignoreQuery = ignoreQuery;
    }

    public override bool Compare(string value)
    {
      if (UtilityClass.IsNullOrEmpty(value)) return false;

      return Compare(new Uri(value));
    }

    /// <summary>
    /// Compares the specified Uri.
    /// </summary>
    /// <param name="url">The Uri.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c></returns>
    public virtual bool Compare(Uri url)
    {
      if (!_ignoreQuery)
      {
        // compare without modification
        return uriToCompareWith.Equals(url);
      }
      else
      {
        // trim querystrings
        string trimmedUrl = TrimQueryString(url);
        string trimmedCompareUrl = TrimQueryString(uriToCompareWith);
        // compare trimmed urls.
        return (string.Compare(trimmedUrl, trimmedCompareUrl, true) == 0);
      }
    }

    private static string TrimQueryString(Uri url)
    {
      return url.ToString().Split('?')[0];
    }

    public override string ToString()
    {
      return GetType().ToString() + " compares with: " + uriToCompareWith.ToString();
    }
  }

  /// <summary>
  /// Class that supports comparing a <see cref="bool"/> instance with a string value.
  /// </summary>
  public class BoolComparer : StringEqualsAndCaseInsensitiveComparer
  {
    public BoolComparer(bool value) : base(value.ToString())
    {
    }
  }

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
      return GetType().ToString() + " compares with: " + valueToCompareWith;
    }
  }

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