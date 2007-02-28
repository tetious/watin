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
        return new TableRowCollection(DomContainer, UtilityClass.IHtmlElementCollectionToArrayList(HtmlBody.rows));
      }
    }

    /// <summary>
    /// Returns the table row in this table body (not including table rows 
    /// from tables nested in this table body).
    /// </summary>
    /// <param name="findBy">The find by.</param>
    /// <returns></returns>
    public override TableRow TableRow(Attribute findBy)
    {
      return ElementsSupport.TableRow(DomContainer, findBy, HtmlBody.rows);
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

    private IHTMLTableSection HtmlBody
    {
      get { return (IHTMLTableSection) HTMLElement; }
    }
  }
}
