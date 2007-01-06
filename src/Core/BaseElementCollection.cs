using System.Collections;
using System.Text.RegularExpressions;
using mshtml;

namespace WatiN.Core
{
  public delegate Element CreateElementInstance(DomContainer domContainer, IHTMLElement element);
  public delegate Element CreateElementCollectionInstance(DomContainer domContainer, ArrayList elements);
  
  public abstract class BaseElementCollection : IEnumerable
  {
    protected DomContainer domContainer;
    
    private ArrayList elements;
    private CreateElementInstance createElementInstance;
    private ElementFinder finder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonCollection"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The DOM container.</param>
    /// <param name="finder">The finder.</param>
    /// <param name="createElementInstance">The create element instance.</param>
    public BaseElementCollection(DomContainer domContainer, ElementFinder finder, CreateElementInstance createElementInstance) : 
           this(domContainer, (ArrayList)null, createElementInstance)
    {
      this.finder = finder;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonCollection"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The DOM container.</param>
    /// <param name="elements">The elements.</param>
    /// <param name="createElementInstance">The create element instance.</param>
    public BaseElementCollection(DomContainer domContainer, ArrayList elements, CreateElementInstance createElementInstance)
    {
      this.elements = elements;
      this.domContainer = domContainer;
      this.createElementInstance = createElementInstance;
    }

    /// <summary>
    /// Gets the length.
    /// </summary>
    /// <value>The length.</value>
    public int Length { get { return Elements.Count; } }

    protected ArrayList Elements
    {
      get
      {
        if (elements == null)
        {
          if (finder != null)
          {
            elements = finder.FindAll();
          }
          else
          {
            elements = new ArrayList();
          }
        }
        
        return elements;
      }
    }

    protected virtual bool IgnoreInvalidAttributes
    {
      get { return false; }
    }

    public bool Exists(string elementId)
    {
      return Exists(new Id(elementId));
    }

    public bool Exists(Regex elementId)
    {
      return Exists(new Id(elementId));
    }
    
    public bool Exists(Attribute findBy)
    {
      ElementAttributeBag attributeBag = new ElementAttributeBag();
      attributeBag.IgnoreInvalidAttributes = IgnoreInvalidAttributes;
      
      foreach (IHTMLElement element in Elements)
      {
        attributeBag.IHTMLElement = element;
        if (findBy.Compare(attributeBag))
        {
          return true;
        }
      }
      
      return false;
    }
    
    protected ArrayList DoFilter(Attribute findBy)
    {
      ArrayList returnElements;
      
      if (elements == null)
      {
        if (finder != null)
        {
          returnElements = finder.FindAll(findBy);
        }
        else
        {
          returnElements = new ArrayList();
        }
      }
      else
      {
        returnElements = new ArrayList();
        ElementAttributeBag attributeBag = new ElementAttributeBag();
        attributeBag.IgnoreInvalidAttributes = IgnoreInvalidAttributes;
        
        foreach (IHTMLElement element in Elements)
        {
          attributeBag.IHTMLElement = element;
          
          if (findBy.Compare(attributeBag))
          {
            returnElements.Add(element);
          }
        }
      }
      
      return returnElements;
    }
    
    /// <exclude />
    public Enumerator GetEnumerator() 
    {
      return new Enumerator(domContainer, Elements, createElementInstance);
    }
    
    IEnumerator IEnumerable.GetEnumerator() 
    {
      return GetEnumerator();
    }

    /// <exclude />
    public class Enumerator: IEnumerator 
    {
      ArrayList children;
      DomContainer domContainer;
      CreateElementInstance createElementInstance;
      int index;
      
      /// <exclude />
      public Enumerator(DomContainer domContainer, ArrayList children, CreateElementInstance createElementInstance) 
      {
        this.children = children;
        this.domContainer = domContainer;
        this.createElementInstance = createElementInstance;
        
        Reset();
      }

      /// <exclude />
      public void Reset() 
      {
        index = -1;
      }

      /// <exclude />
      public bool MoveNext() 
      {
        ++index;
        return index < children.Count;
      }

      /// <exclude />
      public virtual object Current 
      {
        get
        {
          return createElementInstance(domContainer,(IHTMLElement)children[index]);
        }
      }

      /// <exclude />
      object IEnumerator.Current { get { return Current; } }
    }
  }
}