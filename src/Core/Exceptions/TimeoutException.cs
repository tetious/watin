namespace WatiN.Core.Exceptions
{
  using System;

  /// <summary>
  /// Thrown if waiting for a webpage or element times out.
  /// </summary>
  public class TimeoutException : WatiNException
  {
    public TimeoutException(string value) : base("Timeout while '" + value + "'")
    {}
    public TimeoutException(string value, Exception innerException) : base("Timeout while '" + value + "'", innerException)
    {}
  }
}