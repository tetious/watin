namespace WatiN.Core.Exceptions
{
  /// <summary>
  /// Thrown if an element is readonly and the current action (like TextField.TypeText) a
  /// is not allowed.
  /// </summary>
  public class ElementReadOnlyException : WatiNException
  {
    public ElementReadOnlyException(string elementId) : 
      base("Element with Id:" + elementId + " is readonly")
    {}
  }
}