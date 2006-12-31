using System;
using System.Collections;
using System.Threading;
using mshtml;
using WatiN.Core.Exceptions;

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
        this.findBy = new NoAttributeCompare();
      }
      else
      {
        this.findBy = findBy;
      }
      
      this.elementsCollection = elementsCollection;
      
      tagsToFind = elementTags;
    }
    
    public ElementFinder(ArrayList elementTags, IHTMLElementCollection elementsCollection) : this(elementTags, null, elementsCollection)
    {}
    
    public ElementFinder(string tagName, string inputType, Attribute findBy, IHTMLElementCollection elementsCollection)
    {
      this.findBy = findBy;
      this.elementsCollection = elementsCollection;
      
      AddElementTag(tagName, inputType);
    }
    
    public ElementFinder(string tagName, string inputType, IHTMLElementCollection elementsCollection): this(tagName, inputType, new NoAttributeCompare(), elementsCollection)
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

      // Loop through each element and evaluate
      foreach (IHTMLElement element in elements)
      {
        waitUntilElementReadyStateIsComplete(element);

        if (findBy.Compare(element) && elementTag.Compare(element))
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

  public class ElementTag
  {
    public readonly string TagName;
    public readonly string InputTypes;
    public readonly bool IsInputElement = false;
      
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
      
      if (ihtmlElement == null)
      {
        return false;
      }
      
      return Compare(ihtmlElement);
    }
      
    public bool Compare(IHTMLElement element)
    {
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
  }
}