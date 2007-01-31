#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006-2007 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright

using System;
using System.Collections;
using System.Threading;
using mshtml;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
  public class ElementFinder
  {
    public static ElementFinder ButtonFinder(Attribute findBy, IHTMLElementCollection elements)
    {
      return new ElementFinder(Button.ElementTags, findBy, elements);
    }

    private ArrayList tagsToFind = new ArrayList();
    
    protected readonly Attribute findBy;
    protected readonly IHTMLElementCollection elementsCollection;

    public ElementFinder(ArrayList elementTags, Attribute findBy, IHTMLElementCollection elementsCollection)
    {
      if (findBy == null)
      {
        this.findBy = new AlwaysTrueAttribute();
      }
      else
      {
        this.findBy = findBy;
      }
      
      this.elementsCollection = elementsCollection;
      
      if (elementTags != null)
      {
        tagsToFind = elementTags;
      }
      else
      {
        AddElementTag(null, null);
      }
    }
    
    public ElementFinder(ArrayList elementTags, IHTMLElementCollection elementsCollection) : this(elementTags, null, elementsCollection)
    {}
    
    public ElementFinder(string tagName, string inputType, Attribute findBy, IHTMLElementCollection elementsCollection)
    {
      this.findBy = findBy;
      this.elementsCollection = elementsCollection;
      
      AddElementTag(tagName, inputType);
    }
    
    public ElementFinder(string tagName, string inputType, IHTMLElementCollection elementsCollection): this(tagName, inputType, new AlwaysTrueAttribute(), elementsCollection)
    {}

    internal static string GetExceptionMessage(ArrayList elementTags)
    {
      string message = String.Empty;

      foreach (ElementTag elementTag in elementTags)
      {
        if (message.Length > 0)
        {
          message = message + " or ";
        }
        message = message + elementTag.ToString();
      }

      return message;
    }

    public virtual IHTMLElement FindFirst(bool throwExceptionIfElementNotFound)
    {      
      foreach (ElementTag elementTag in tagsToFind)
      {
        ArrayList elements = findElementsByAttribute(elementTag, findBy, true);

        if (elements.Count > 0)
        {
          return (IHTMLElement)elements[0];
        }
      }

      if (throwExceptionIfElementNotFound)
      {
        throw new ElementNotFoundException(GetExceptionMessage(tagsToFind), findBy.AttributeName, findBy.Value);
      }
      
      return null;
    }

    public void AddElementTag(string tagName, string inputType)
    {
      tagsToFind.Add(new ElementTag(tagName, inputType));
    }
    
    public ArrayList FindAll()
    {
      return FindAll(findBy);
    }
    
    public ArrayList FindAll(Attribute findBy)
    {
      if (tagsToFind.Count == 1)
      {
        return findElementsByAttribute((ElementTag)tagsToFind[0], findBy, false);
      }
      else 
      {
        ArrayList elements = new ArrayList();
      
        foreach (ElementTag elementTag in tagsToFind)
        {
          elements.AddRange(findElementsByAttribute(elementTag, findBy, false));
        }
    
        return elements;
      }
    }
    
    private ArrayList findElementsByAttribute(ElementTag elementTag, Attribute findBy, bool returnAfterFirstMatch)
    {
      // Get elements with the tagname from the page
      ArrayList children = new ArrayList();
      IHTMLElementCollection elements = elementTag.GetElementCollection(elementsCollection);

      ElementAttributeBag attributeBag = new ElementAttributeBag();
      attributeBag.IgnoreInvalidAttributes = (elementTag.TagName == null);
      
      // Loop through each element and evaluate
      foreach (IHTMLElement element in elements)
      {
        waitUntilElementReadyStateIsComplete(element);

        attributeBag.IHTMLElement = element;
        
        if (findBy.Compare(attributeBag) && elementTag.Compare(element))
        {
          children.Add(element);
          if (returnAfterFirstMatch)
          {
            return children;
          }
        }
      }

      return children;
    }

    private static void waitUntilElementReadyStateIsComplete(IHTMLElement element)
    {
      //TODO: See if this method could be dropped, it seems to give
      //      more trouble (uninitialized state of elements)
      //      then benefits (I just introduced this method to be on 
      //      the save side)
      
      if (String.Compare(element.tagName, "img", true) == 0)
      {
        return;
      }
      
      //TODO: replace with SimpleTimer
      DateTime startTime = DateTime.Now;
      
      // Wait if the readystate of an element is BETWEEN
      // Uninitialized and Complete. If it's uninitialized,
      // it's quite probable that it will never reach Complete.
      // Like for elements that could not load an image or ico
      // or some other bits not part of the HTML page.
      int readyState = ((IHTMLElement2)element).readyStateValue;
      while (readyState != 0 && readyState !=4)
      { 
        if(DateTime.Now.Subtract(startTime).Seconds <= 30)
        {
          Thread.Sleep(100);
        }
        else
        {
          throw new WatiNException("Element didn't reach readystate = complete within 30 seconds: " + element.outerText);
        }

        readyState = ((IHTMLElement2)element).readyStateValue;
      }
    }

    internal static bool isInputElement(string tagName)
    {
      return String.Compare(tagName, ElementsSupport.InputTagName, true) == 0;
    }
  }

  public class ElementAttributeBag : IAttributeBag
  {
    private IHTMLElement element = null;
    private bool ignoreInvalidAttributes = false;

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

      object attribute;
      
      if (attributename.ToLower().StartsWith("style."))
      {
        attribute = Style.GetAttributeValue(attributename.Substring(6), element.style); 
      }
      else
      {
        attribute = element.getAttribute(attributename, 0);
      }

      if (attribute == DBNull.Value)
      {
        if (ignoreInvalidAttributes)
        {
          return null;
        }
        
        throw new InvalidAttributException(attributename, element.tagName);
      }

      if (attribute == null)
      {
        return null;
      }
      
      return attribute.ToString();
    }

    public bool IgnoreInvalidAttributes
    {
      set
      {
        ignoreInvalidAttributes = value;
      }
      get
      {
        return ignoreInvalidAttributes;
      }
    }
  }

  public class ElementTag
  {
    public readonly string TagName;
    public readonly string InputTypes;
    public readonly bool IsInputElement = false;
      
    public ElementTag(string tagName) : this(tagName, null)
    {}
    
    public ElementTag(string tagName, string inputTypes)
    {
      TagName = tagName;
      IsInputElement = ElementFinder.isInputElement(tagName);
        
      // Check arguments
      if (IsInputElement)
      {
        if (UtilityClass.IsNullOrEmpty(inputTypes))
        {
          throw new ArgumentNullException("inputTypes", String.Format("inputTypes must be set when tagName is '{0}'", tagName));
        }
          
        InputTypes = inputTypes.ToLower();
      }
    }
      
    public IHTMLElementCollection GetElementCollection(IHTMLElementCollection elements)
    {
      if (TagName == null)
      {
        return elements;
      }
      
      return (IHTMLElementCollection)elements.tags(TagName);
    }

    public bool Compare(object element)
    {
      IHTMLElement ihtmlElement = element as IHTMLElement;
            
      return Compare(ihtmlElement);
    }
      
    public bool Compare(IHTMLElement element)
    {
      if (element == null)
      {
        return false;
      }
      
      if (CompareTagName(element))
      {
        if (IsInputElement)
        {
          return CompareAgainstInputTypes(element);
        }
        else
        {
          return true;
        }
      }
        
      return false;
    }

    public override string ToString()
    {
      if (IsInputElement)
      {
        return String.Format("{0} ({1})", TagName.ToUpper(), InputTypes);
      }
      return TagName.ToUpper();
    }

    private bool CompareTagName(IHTMLElement element)
    {
      if (TagName == null)
      {
        return true;
      }
        
      return String.Compare(TagName, element.tagName, true) == 0;
    }

    private bool CompareAgainstInputTypes(IHTMLElement element)
    {
      IHTMLInputElement inputElement = (IHTMLInputElement)element;
      
      string inputElementType = inputElement.type.ToLower();
    
      if (InputTypes.IndexOf(inputElementType) >= 0)
      {
        return true;
      }
      
      return false;
    }

    public static bool IsValidElement(object element, ArrayList elementTags)
    {
      return IsValidElement(element as IHTMLElement, elementTags);
    }

    public static bool IsValidElement(IHTMLElement element, ArrayList elementTags)
    {
      if (element == null) return false;

      foreach (ElementTag elementTag in elementTags)
      {
        if (elementTag.Compare(element))
        {
          return true;      
        }
      }
      
      return false;
    }
  }
}