namespace WatiN.Core
{
  using System;
  using System.Collections;
  using mshtml;

  /// <summary>
  /// This class is mainly used by WatiN internally and defines 
  /// the supported html tags for inheritors of <see cref="Element"/>.
  /// </summary>
  public class ElementTag
  {
    public readonly string TagName = null;
    public readonly string InputTypes;
    public readonly bool IsInputElement = false;
      
    public ElementTag(string tagName) : this(tagName, null)
    {}
    
    public ElementTag(string tagName, string inputTypes)
    {
      if (tagName != null)
      {
        TagName = tagName.ToLower();
      }
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
    
    public ElementTag(IHTMLElement element)
    {
      TagName = element.tagName.ToLower();
      IsInputElement = ElementFinder.isInputElement(TagName);
      if (IsInputElement)
      {
        IHTMLInputElement inputElement = (IHTMLInputElement)element;
      
        InputTypes = inputElement.type.ToLower();
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
      string returnValue = string.Empty;
      if (IsInputElement)
      {
        returnValue = String.Format("{0} ({1})", TagName.ToUpper(), InputTypes);
      }
      else if (TagName != null)
      {
        returnValue = TagName.ToUpper();
      }
      return returnValue;
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
    
      return (InputTypes.IndexOf(inputElementType) >= 0);
    }

    public override int GetHashCode()
    {
      return (TagName != null ? TagName.GetHashCode() : 0) + 29*(InputTypes != null ? InputTypes.GetHashCode() : 0);
    }

    public override bool Equals(object obj)
    {
      if (this == obj) return true;
      ElementTag elementTag = obj as ElementTag;
      if (elementTag == null) return false;
      if (!Equals(TagName, elementTag.TagName)) return false;
      if (!Equals(InputTypes, elementTag.InputTypes)) return false;
      return true;
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