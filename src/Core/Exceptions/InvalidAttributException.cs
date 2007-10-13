namespace WatiN.Core.Exceptions
{
  /// <summary>
  /// Thrown if the specified attribute isn't a valid attribute of the element. 
  /// For example doing <c>TextField.GetAttribute("src")</c> will throw 
  /// this exception.
  /// </summary>
  public class InvalidAttributException : WatiNException
  {
    public InvalidAttributException(string atributeName, string elementTag) : 
      base("Invalid attribute '" +atributeName + "' for element '" + elementTag +"'" )
    {}
  }
}