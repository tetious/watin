namespace WatiN.Core.Exceptions
{
  /// <summary>
  /// Thrown if the searched for frame can't be found.
  /// </summary>
  public class FrameNotFoundException : WatiNException
  {
    public FrameNotFoundException(string attributeName, string value) : 
      base("Could not find a Frame or IFrame by " + attributeName + " with value '" + value + "'")
    {}
  }
}