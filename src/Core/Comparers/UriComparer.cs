namespace WatiN.Core.Comparers
{
  using System;

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
      return uriToCompareWith.ToString();
    }
  }
}