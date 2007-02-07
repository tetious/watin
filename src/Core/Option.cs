namespace WatiN.Core
{
  using System.Collections;
  using mshtml;

  public class Option : Element
  {
    private static ArrayList elementTags;

    public static ArrayList ElementTags
    {
      get
      {
        if (elementTags == null)
        {
          elementTags = new ArrayList();
          elementTags.Add(new ElementTag("option"));
        }

        return elementTags;
      }
    }

    public Option(DomContainer ie, IHTMLOptionElement optionElement) : base(ie, optionElement)
    {}
    
    public Option(DomContainer ie, ElementFinder finder) : base(ie, finder)
    {}
    
    /// <summary>
    /// Initialises a new instance of the <see cref="Span"/> class based on <paramref name="element"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    public Option(Element element) : base(element, ElementTags)
    {}

    /// <summary>
    /// Returns the value.
    /// </summary>
    /// <value>The value.</value>
    public string Value
    {
      get { return optionElement.value; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Option"/> is selected.
    /// </summary>
    /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
    public bool Selected
    {
      get { return optionElement.selected; }
    }

    /// <summary>
    /// Returns the index of this <see cref="Option"/> in the <see cref="SelectList"/>.
    /// </summary>
    /// <value>The index.</value>
    public int Index
    {
      get { return optionElement.index; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Option"/> is selected by default.
    /// </summary>
    /// <value><c>true</c> if selected by default; otherwise, <c>false</c>.</value>
    public bool DefaultSelected
    {
      get { return optionElement.defaultSelected; }
    }

    /// <summary>
    /// De-selects this option in the selectlist (if selected),
    /// fires the "onchange" event on the selectlist and waits for it
    /// to complete.
    /// </summary>
    public void Clear()
    {
      setSelected(false, true);
    }

    /// <summary>
    /// De-selects this option in the selectlist (if selected),
    /// fires the "onchange" event on the selectlist and does not wait for it
    /// to complete.
    /// </summary>
    public void ClearNoWait()
    {
      setSelected(false, false);
    }

    /// <summary>
    /// Selects this option in the selectlist (if not selected),
    /// fires the "onchange" event on the selectlist and waits for it
    /// to complete.
    /// </summary>
    public void Select()
    {
      setSelected(true, true);
    }

    /// <summary>
    /// Selects this option in the selectlist (if not selected),
    /// fires the "onchange" event on the selectlist and does not wait for it
    /// to complete.
    /// </summary>
    public void SelectNoWait()
    {
      setSelected(true,false);
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      return Text;
    }

    /// <summary>
    /// Gets the parent <see cref="SelectList"/>.
    /// </summary>
    /// <value>The parent <see cref="SelectList"/>.</value>
    public SelectList ParentSelectList
    {
      get
      {
        Element parentElement = Parent;
        
        while (parentElement != null)
        {
          if (parentElement is SelectList)
          {
            return (SelectList)parentElement;
          }
          parentElement = parentElement.Parent;
        }
 
        return null;
      }
    }

    private void setSelected(bool value, bool WaitForComplete)
    {
      if (optionElement.selected != value)
      {
        optionElement.selected = value;
        if (WaitForComplete)
        {
          ParentSelectList.FireEvent("onchange");
        }
        else
        {
          ParentSelectList.FireEventNoWait("onchange");
        }
      }
    }

    private IHTMLOptionElement optionElement
    {
      get { return (IHTMLOptionElement) HTMLElement; }
    }
  }
}