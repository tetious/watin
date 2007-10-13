namespace WatiN.Core.Exceptions
{
  using System;

  /// <summary>
  /// Thrown if a (java) script failed to run. The innerexception returns the actual exception.
  /// </summary>
  public class RunScriptException : WatiNException
  {
    public RunScriptException(Exception innerException) : base("RunScript failed", innerException)
    {}
  }
}