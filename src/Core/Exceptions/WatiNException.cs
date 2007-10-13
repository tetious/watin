namespace WatiN.Core.Exceptions
{
  using System;

  /// <summary>
  /// Base class for Exceptions thrown by WatiN.
  /// </summary>
  public class WatiNException : Exception
  {
    public WatiNException() : base() {}
    public WatiNException(string message) : base(message) {}
    public WatiNException(string message, Exception innerexception) : base(message, innerexception) {}
  }
}