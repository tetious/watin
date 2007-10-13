namespace WatiN.Core.Exceptions
{
  /// <summary>
  /// Thrown if the searched for internet explorer (IE) can't be found.
  /// </summary>
  public class IENotFoundException : WatiNException
  {
    public IENotFoundException(string findBy, string value, int waitTimeInSeconds) : 
      base("Could not find an IE window by " + findBy + " with value '" + value + "'. (Search expired after '" + waitTimeInSeconds.ToString() + "' seconds)")
    {}
  }
}