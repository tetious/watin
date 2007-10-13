namespace WatiN.Core.Exceptions
{
  /// <summary>
  /// Thrown if the searched for HtmlDialog can't be found.
  /// </summary>
  public class HtmlDialogNotFoundException : WatiNException
  {
    public HtmlDialogNotFoundException(string attributeName, string value, int waitTimeInSeconds) : 
      base("Could not find a HTMLDialog by " + attributeName + " with value '" + value + "'. (Search expired after '" + waitTimeInSeconds.ToString() + "' seconds)")
    {}
  }
}