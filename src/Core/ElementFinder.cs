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

using System;
using System.Collections;
using System.Threading;
using mshtml;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Interfaces
{
  public interface IElementCollection
  {
    IHTMLElementCollection Elements{ get; }
  }
}

namespace WatiN.Core
{
  /// <summary>
  /// This class is mainly used internally by WatiN to find elements in
  /// an <see cref="IHTMLElementCollection"/> or <see cref="ArrayList"/> matching
  /// the given <see cref="AttributeConstraint"/>.
  /// </summary>
  public class ElementFinder
  {
    private ArrayList tagsToFind = new ArrayList();
    
    protected readonly AttributeConstraint findBy;
    protected readonly IElementCollection elementCollection;

    public ElementFinder(ArrayList elementTags, AttributeConstraint findBy, IElementCollection elementCollection)
    {
      if (elementCollection == null) { throw new ArgumentNullException("elementCollection"); }

      this.findBy = getFindBy(findBy);
      this.elementCollection = elementCollection;
      
      if (elementTags != null)
      {
        tagsToFind = elementTags;
      }
      else
      {
        AddElementTag(null, null);
      }
    }

    public ElementFinder(ArrayList elementTags, IElementCollection elementCollection) : this(elementTags, null, elementCollection)
    {}
    
    public ElementFinder(string tagName, string inputType, AttributeConstraint findBy, IElementCollection elementCollection)
    {
      if (elementCollection == null) { throw new ArgumentNullException("elementCollection"); }

      this.findBy = getFindBy(findBy);
      this.elementCollection = elementCollection;
      
      AddElementTag(tagName, inputType);
    }
    
    public ElementFinder(string tagName, string inputType, IElementCollection elementCollection): this(tagName, inputType, null, elementCollection)
    {}

    public virtual IHTMLElement FindFirst()
    {            
      return FindFirst(false);
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
        throw CreateElementNotFoundException();
      }
      
      return null;
    }

    internal ElementNotFoundException CreateElementNotFoundException()
    {
      return new ElementNotFoundException(GetExceptionMessage(tagsToFind), findBy.AttributeName, findBy.Value);
    }

    internal ElementNotFoundException CreateElementNotFoundException(Exception innerexception)
    {
      return new ElementNotFoundException(GetExceptionMessage(tagsToFind), findBy.AttributeName, findBy.Value, innerexception);
    }

    public void AddElementTag(string tagName, string inputType)
    {
      tagsToFind.Add(new ElementTag(tagName, inputType));
    }
    
    public ArrayList FindAll()
    {
      return FindAll(findBy);
    }
    
    public ArrayList FindAll(AttributeConstraint findBy)
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

    private static AttributeConstraint getFindBy(AttributeConstraint findBy)
    {
      if (findBy == null)
      {
        return new AlwaysTrueAttribute();
      }
      return findBy;
    }

    private ArrayList findElementsByAttribute(ElementTag elementTag, AttributeConstraint findBy, bool returnAfterFirstMatch)
    {
      // Get elements with the tagname from the page
      ArrayList children = new ArrayList();
      IHTMLElementCollection elements = elementTag.GetElementCollection(elementCollection.Elements);

      if (elements != null)
      {
        ElementAttributeBag attributeBag = new ElementAttributeBag();
      
        // Loop through each element and evaluate
        foreach (IHTMLElement element in elements)
        {
          waitUntilElementReadyStateIsComplete(element);

          attributeBag.IHTMLElement = element;
        
          if (elementTag.Compare(element) && findBy.Compare(attributeBag))
          {
            children.Add(element);
            if (returnAfterFirstMatch)
            {
              return children;
            }
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
      
      if (ElementTag.IsValidElement(element, Image.ElementTags))
      {
        return;
      }
      
      // Wait if the readystate of an element is BETWEEN
      // Uninitialized and Complete. If it's uninitialized,
      // it's quite probable that it will never reach Complete.
      // Like for elements that could not load an image or ico
      // or some other bits not part of the HTML page.     
      SimpleTimer timeoutTimer = new SimpleTimer(30);

      do
      {
        int readyState = ((IHTMLElement2)element).readyStateValue;

        if (readyState == 0 || readyState == 4)
        {
          return;
        }

        Thread.Sleep(100);

      } while (!timeoutTimer.Elapsed);

      throw new WatiNException("Element didn't reach readystate = complete within 30 seconds: " + element.outerText);
    }

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

    internal static bool isInputElement(string tagName)
    {
      return String.Compare(tagName, ElementsSupport.InputTagName, true) == 0;
    }
  }

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

  /// <summary>
  /// This class is mainly used by WatiN internally and defines 
  /// the supported html tags for inheritors of <see cref="Element"/>.
  /// </summary>
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
      if (elements == null) return null;
        
      if (TagName == null) return elements;
      
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
