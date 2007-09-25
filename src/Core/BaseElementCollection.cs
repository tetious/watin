#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System.Collections;
using System.Text.RegularExpressions;
using mshtml;

namespace WatiN.Core
{
  /// <summary>
  /// This delegate is mainly used by <see cref="BaseElementCollection"/> to 
  /// delegate the creation of a specialized element type. 
  /// </summary>
  public delegate Element CreateElementInstance(DomContainer domContainer, IHTMLElement element);
  
  /// <summary>
  /// This class is mainly used by Watin internally as the base class for all 
  /// of the element collections.
  /// </summary>
  public abstract class BaseElementCollection : IEnumerable
  {
    protected DomContainer domContainer;
    
    private ArrayList elements;
    private CreateElementInstance createElementInstance;
    protected ElementFinder finder;

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

    public bool Exists(string elementId)
    {
      return Exists(Find.ById(elementId));
    }

    public bool Exists(Regex elementId)
    {
      return Exists(Find.ById(elementId));
    }
    
    public bool Exists(AttributeConstraint findBy)
    {
      ElementAttributeBag attributeBag = new ElementAttributeBag();
      
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
    
    protected ArrayList DoFilter(AttributeConstraint findBy)
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