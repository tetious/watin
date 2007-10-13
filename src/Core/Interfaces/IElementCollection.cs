namespace WatiN.Core.Interfaces
{
  using mshtml;

  public interface IElementCollection
  {
    IHTMLElementCollection Elements{ get; }
  }
}