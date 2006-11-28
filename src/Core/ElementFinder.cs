using System;
using System.Collections;
using System.Threading;
using mshtml;
using WatiN.Core.Exceptions;

namespace WatiN.Core
{
  public class ElementFinder
  {
    private ArrayList tagsToFind = new ArrayList();
    
    protected readonly Attribute findBy;
    protected readonly IHTMLElementCollection elementsCollection;

    private string exceptionMessage = null;

    public ElementFinder(string tagName, string inputType, Attribute findBy, IHTMLElementCollection elementsCollection)
    {
      this.findBy = findBy;
      this.elementsCollection = elementsCollection;
      
      AddTagType(tagName, inputType);
    }
    
    public ElementFinder(string tagName, string inputType, IHTMLElementCollection elementsCollection): this(tagName, inputType, new NoAttributeCompare(), elementsCollection)
    {}

    public string ExceptionMessage
    {
      get
      {
        if (exceptionMessage == null && tagsToFind.Count > 0)
        {
          TagType tagType = (TagType)tagsToFind[0];
          return tagType.tagName;
        }
        return exceptionMessage;
      }
      set
      {
        exceptionMessage = value;
      }
    }

    public virtual IHTMLElement FindFirst(bool throwExceptionIfElementNotFound)
    {      
      foreach (TagType tagType in tagsToFind)
      {
        ArrayList elements = findElementsByAttribute(tagType, true);

        if (elements.Count > 0)
        {
          return (IHTMLElement)elements[0];
        }
      }

      if (throwExceptionIfElementNotFound)
      {
        throw new ElementNotFoundException(ExceptionMessage, findBy.AttributeName, findBy.Value);
      }
      
      return null;
    }

    public void AddTagType(string tagName, string inputType)
    {
      tagsToFind.Add(new TagType(tagName, inputType));
    }
    
    public ArrayList FindAll()
    {
      if (tagsToFind.Count == 1)
      {
        return findElementsByAttribute((TagType)tagsToFind[0], false);
      }
      else 
      {
        ArrayList elements = new ArrayList();
      
        foreach (TagType tagType in tagsToFind)
        {
          elements.AddRange(findElementsByAttribute(tagType, false));
        }
    
        return elements;
      }
    }
    
    public ArrayList FindAll(Attribute findBy)
    {
      if (tagsToFind.Count == 1)
      {
        return findElementsByAttribute((TagType)tagsToFind[0], findBy, false);
      }
      else 
      {
        ArrayList elements = new ArrayList();
      
        foreach (TagType tagType in tagsToFind)
        {
          elements.AddRange(findElementsByAttribute(tagType, findBy, false));
        }
    
        return elements;
      }
    }

    private ArrayList findElementsByAttribute(TagType tagType, bool returnAfterFirstMatch)
    {
      return findElementsByAttribute(tagType, findBy, returnAfterFirstMatch);
    }
    
    private ArrayList findElementsByAttribute(TagType tagType, Attribute findBy, bool returnAfterFirstMatch)
    {
      // Check arguments
      if (isInputElement(tagType.tagName) && UtilityClass.IsNullOrEmpty(tagType.inputTypes))
      {
        throw new ArgumentNullException("inputType", "inputType must be set when tagName is 'input'");
      }

      // Get elements with the tagname from the page
      ArrayList children = new ArrayList();
      IHTMLElementCollection elements = getElementCollection(elementsCollection, tagType.tagName);

      // Loop through each element and evaluate
      foreach (IHTMLElement element in elements)
      {
        waitUntilElementReadyStateIsComplete(element, tagType.tagName);

        if (doCompare(element, findBy, tagType.inputTypes))
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

    private static bool doCompare(IHTMLElement element, Attribute findBy, string inputType)
    {
      if (findBy.Compare(element))
      {
        return inputType == null ? true : isInputOfType(element, inputType);
      }
      
      return false;
    }

    private static bool isInputOfType(IHTMLElement element, string inputType)
    {
      IHTMLInputElement inputElement = element as IHTMLInputElement;
      
      if (inputElement != null)
      {
        string inputElementType = inputElement.type.ToLower();
      
        if (inputType.ToLower().IndexOf(inputElementType) >= 0)
        {
          return true;
        }
      }
      
      return false;
    }

    private static void waitUntilElementReadyStateIsComplete(IHTMLElement element, string tagName)
    {
      //TODO: See if this method could be dropped, it seems to give
      //      more troubles (uninitialized state of elements)
      //      then benefits (I just introduced this method to be on 
      //      the save side)
      
      if (String.Compare(tagName, "img", true) == 0)
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

    internal static IHTMLElementCollection getElementCollection(IHTMLElementCollection elements, string tagName)
    {
      if (tagName == null)
      {
        return elements;
      }
      
      return (IHTMLElementCollection)elements.tags(tagName);
    }

    internal static bool isInputElement(string tagName)
    {
      return String.Compare(tagName, ElementsSupport.InputTagName, true) == 0;
    }
    
    private class TagType
    {
      public string tagName;
      public string inputTypes;
      
      public TagType(string tagName, string inputTypes)
      {
        this.tagName = tagName;
        this.inputTypes = inputTypes;
      }
    }
  }
}