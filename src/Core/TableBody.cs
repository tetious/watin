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

    /// <summary>
    /// Returns the table rows of this table body (not including table rows 
    /// from tables nested in this table body).
    /// </summary>
    /// <value>The table rows.</value>
    public override TableRowCollection TableRows
    {
      get
      {
        IHTMLTableSection htmlBody = (IHTMLTableSection) HTMLElement;
        IHTMLElementCollection rows2 = htmlBody.rows;

        return new TableRowCollection(DomContainer, UtilityClass.IHtmlElementCollectionToArrayList(rows2));
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
