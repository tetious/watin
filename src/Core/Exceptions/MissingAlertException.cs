namespace WatiN.Core.Exceptions
{
  /// <summary>
  /// Thrown if no more alerts are available when calling PopUpWatcher.PopAlert.
  /// </summary>
  public class MissingAlertException : WatiNException
  {}
}