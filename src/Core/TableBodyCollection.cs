using System;
using System.Collections;
using System.Text;

namespace WatiN.Core
{
  using System.Collections;
  using mshtml;

  public class TableBodyCollection:BaseElementCollection
  {
    public TableBodyCollection(DomContainer domContainer, ArrayList elements) : base(domContainer, elements, new CreateElementInstance(New))
    {
    }

    public TableBodyCollection(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder, new CreateElementInstance(New))
    {
    }

    public TableBody this[int index]
    {
      get { return new TableBody(domContainer, (IHTMLTableSection) Elements[index]);}
    }
        
    private static Element New(DomContainer domContainer, IHTMLElement element)
    {
      return new TableBody(domContainer, (IHTMLTableSection)element);
    }
  }
}
