namespace WatiN.Core.Exceptions
{
  using System;

  /// <summary>
  /// Thrown if the searched for element can't be found.
  /// </summary>
  public class ElementNotFoundException : WatiNException
  {
    public ElementNotFoundException(string tagName, string attributeName, string value) : 
      base(createMessage(attributeName, tagName, value))
    {}

    public ElementNotFoundException(string tagName, string attributeName, string value, Exception innerexception) : 
      base(createMessage(attributeName, tagName, value), innerexception)
    {}

    private static string createMessage(string attributeName, string tagName, string value)
    {
      return "Could not find a '" + UtilityClass.ToString(tagName) + "' tag containing attribute " + attributeName + " with value '" + value + "'";
    }
  }
}