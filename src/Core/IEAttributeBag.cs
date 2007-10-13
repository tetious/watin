namespace WatiN.Core
{
  using mshtml;
  using SHDocVw;
  using WatiN.Core.Exceptions;
  using WatiN.Core.Interfaces;

  /// <summary>
  /// Wrapper around the <see cref="SHDocVw.InternetExplorer"/> object. Used by <see cref="AttributeConstraint.Compare"/>.
  /// </summary>
  public class IEAttributeBag: IAttributeBag
  {
    private InternetExplorer internetExplorer = null;

    public InternetExplorer InternetExplorer
    {
      get
      {
        return internetExplorer;
      }
      set
      {
        internetExplorer = value;
      }
    }

    public string GetValue(string attributename)
    {
      string name = attributename.ToLower();
      string value = null;
      
      if (name.Equals("href"))
      {
        try
        {
          value = InternetExplorer.LocationURL;
        }
        catch{}
      }
      else if (name.Equals("title"))
      {
        try
        {
          value = ((HTMLDocument) InternetExplorer.Document).title;
        }
        catch{}
      }
      else if (name.Equals("hwnd"))
      {
        try
        {
          value = InternetExplorer.HWND.ToString();
        }
        catch{}
      }
      else
      {
        throw new InvalidAttributException(attributename, "IE");
      }
      
      return value;
    }
  }
}