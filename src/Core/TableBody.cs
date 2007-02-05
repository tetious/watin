namespace WatiN.Core
{
  using System.Collections;
  using mshtml;

  public class TableBody : ElementsContainer
  {
    private static ArrayList elementTags;

    public TableBody(DomContainer ie, ElementFinder finder) : base(ie, finder)
    {
    }

    public TableBody(DomContainer ie, IHTMLTableSection element) : base(ie, (IHTMLElement) element)
    {
    }

    public TableBody(Element element) : base(element, elementTags)
    {
    }

    public override TableRowCollection TableRows
    {
      get
      {
        return ElementsSupport.TableRows(DomContainer, ((IHTMLTableSection) htmlElement).rows);
      }
    }

    public static ArrayList ElementTags
    {
      get
      {
        if (elementTags == null)
        {
          elementTags = new ArrayList();
          elementTags.Add(new ElementTag("tbody"));
        }

        return elementTags;
      }
    }
  }
}
