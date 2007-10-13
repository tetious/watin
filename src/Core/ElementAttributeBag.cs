namespace WatiN.Core
{
  using System;
  using mshtml;
  using WatiN.Core.Interfaces;

  /// <summary>
  /// Wrapper around the <see cref="mshtml.IHTMLElement"/> object. Used by <see cref="AttributeConstraint.Compare"/>.
  /// </summary>
  public class ElementAttributeBag : IAttributeBag
  {
    private IHTMLElement element = null;

    public ElementAttributeBag()
    {}
    
    public ElementAttributeBag(IHTMLElement element)
    {
      IHTMLElement = element;
    }
    
    public IHTMLElement IHTMLElement
    {
      get
      {
        return element;
      }
      set 
      { 
        element = value;
      }
    }

    public string GetValue(string attributename)
    {
      if (string.Compare(attributename, "style", true) == 0 )
      {
        return element.style.cssText;
      }

      object attributeValue;
      
      if (attributename.ToLower().StartsWith("style."))
      {
        attributeValue = Style.GetAttributeValue(attributename.Substring(6), element.style); 
      }
      else
      {
        attributeValue = element.getAttribute(attributename, 0);
      }

      if (attributeValue == DBNull.Value)
      {
        return null;
      }
        
      if (attributeValue == null)
      {
        return null;
      }
      
      return attributeValue.ToString();
    }
  }
}