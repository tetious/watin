namespace WatiN.Core
{
  using System.Collections;
  using mshtml;

  /// <summary>
  /// A typed collection of <see cref="Option" /> elements within a <see cref="SelectList"/>.
  /// </summary>
  public class OptionCollection : BaseElementCollection
  {		
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionCollection"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The DOM container.</param>
    /// <param name="finder">The finder.</param>
    public OptionCollection(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder, new CreateElementInstance(New))
    {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionCollection"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The DOM container.</param>
    /// <param name="elements">The elements.</param>
    public OptionCollection(DomContainer domContainer, ArrayList elements) : base(domContainer, elements, new CreateElementInstance(New))
    {}

    /// <summary>
    /// Gets the <see cref="Span"/> at the specified index.
    /// </summary>
    /// <value></value>
    public Option this[int index] 
    {
      get
      {
        return new Option(domContainer,(IHTMLOptionElement)Elements[index]);
      } 
    }
    
    /// <summary>
    /// Filters this collection with the specified find by.
    /// </summary>
    /// <param name="findBy">The <see cref="Attribute"/> to filter this collection.</param>
    /// <returns>A filtered <see cref="OptionCollection"/></returns>
    public OptionCollection Filter(Attribute findBy)
    {      
      return new OptionCollection(domContainer, DoFilter(findBy));
    }
    
    private static Element New(DomContainer domContainer, IHTMLElement element)
    {
      return new Option(domContainer, (IHTMLOptionElement)element);
    }
  }
}