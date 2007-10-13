namespace WatiN.Core.Exceptions
{
  /// <summary>
  /// Thrown if an element is disabled and the current action (like clicking a
  /// disabled link) is not allowed.
  /// </summary>
  public class ElementDisabledException : WatiNException
  {
    public ElementDisabledException(string elementId) : 
      base("Element with Id:" + elementId + " is disabled")
    {}
  }
}