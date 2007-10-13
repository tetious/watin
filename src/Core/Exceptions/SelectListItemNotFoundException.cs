namespace WatiN.Core.Exceptions
{
  /// <summary>
  /// Thrown if the searched for selectlist item (option) can't be found.
  /// </summary>
  public class SelectListItemNotFoundException : WatiNException
  {
    public SelectListItemNotFoundException(string value) : 
      base("No item with text or value '" + value + "' was found in the selectlist")
    {}
  }
}