#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
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

namespace WatiN.Core
{
  /// <summary>
  /// Summary description for SubElements.
  /// </summary>
  public sealed class ElementsSupport
  {
    public const string ButtonTagName = "BUTTON";
    public const string DivTagName = "DIV";
    public const string FormTagName = "FORM";
    public const string FrameTagName = "FRAME";
    public const string ImageTagName = "IMG";
    public const string InputTagName = "INPUT";
    public const string LabelTagName = "LABEL";
    public const string LinkTagName = "A";
    public const string ParaTagName = "P";
    public const string SelectListTagName = "SELECT";
    public const string SpanTagName = "SPAN";
    public const string TableTagName = "TABLE";
    public const string TableCellTagName = "TD";
    public const string TableRowTagName = "TR";
    public const string TextAreaTagName = "TEXTAREA";

    public const string InputNullType = null;
    public const string InputButtonType = "button submit image reset";
    public const string InputCheckBoxType = "checkbox";
    public const string InputFileType = "file";
    public const string InputRadioButtonType = "radio";
    public const string InputTextFieldType = "text password textarea hidden";

    /// <summary>
    /// Prevent creating an instance of this class (contains only static members)
    /// </summary>
    private ElementsSupport(){}
    
    public static Button Button(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      ElementFinder finder = new ElementFinder(InputTagName, InputButtonType, findBy, elements);
      finder.AddTagType(ButtonTagName, InputNullType);
      finder.ExceptionMessage = string.Format("{0} ({1}) or {2}", InputTagName, InputButtonType, ButtonTagName);

      return new Button(ie, finder);
    }

    public static ButtonCollection Buttons(DomContainer ie, IHTMLElementCollection elements)
    {
      ArrayList inputElements = FindAllElements(InputTagName, InputButtonType, elements);
      inputElements.AddRange(FindAllElements(ButtonTagName, InputNullType, elements));
      
      return new ButtonCollection(ie, inputElements);
    }

    public static CheckBox CheckBox(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new CheckBox(ie, new ElementFinder(InputTagName, InputCheckBoxType, findBy, elements));
    }

    public static CheckBoxCollection CheckBoxes(DomContainer ie, IHTMLElementCollection elements)
    {
      return new CheckBoxCollection(ie, FindAllElements(InputTagName, InputCheckBoxType, elements));
    }
    
    public static Element Element(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new ElementsContainer(ie, new ElementFinder(null, InputNullType, findBy, elements));
    }

    public static ElementCollection Elements(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ElementCollection(ie, FindAllElements(null, InputNullType, elements));
    }

    public static FileUpload FileUpload(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new FileUpload(ie, new ElementFinder(InputTagName, InputFileType, findBy, elements));
    }

    public static FileUploadCollection FileUploads(DomContainer ie, IHTMLElementCollection elements)
    {
      return new FileUploadCollection(ie, FindAllElements(InputTagName, InputFileType, elements));
    }
    
    public static Form Form(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Form(ie, new ElementFinder(FormTagName, InputNullType, findBy, elements));
    }

    public static FormCollection Forms(DomContainer ie, IHTMLElementCollection elements)
    {
      return new FormCollection(ie, FindAllElements(FormTagName, InputNullType, elements));
    }

    public static Label Label(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Label(ie, new ElementFinder(LabelTagName, InputNullType, findBy, elements));
    }

    public static LabelCollection Labels(DomContainer ie, IHTMLElementCollection elements)
    {
      return new LabelCollection(ie, FindAllElements(LabelTagName, InputNullType, elements));
    }

    public static Link Link(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Link(ie, new ElementFinder(LinkTagName, InputNullType, findBy, elements));
    }

    public static LinkCollection Links(DomContainer ie, IHTMLElementCollection elements)
    {
      return new LinkCollection(ie, FindAllElements(LinkTagName, InputNullType, elements));
    }

    public static Para Para(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Para(ie, new ElementFinder(ParaTagName, InputNullType, findBy, elements));
    }

    public static ParaCollection Paras(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ParaCollection(ie, FindAllElements(ParaTagName, InputNullType, elements));
    }

    public static RadioButton RadioButton(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new RadioButton(ie, new ElementFinder(InputTagName, InputRadioButtonType, findBy, elements));
    }

    public static RadioButtonCollection RadioButtons(DomContainer ie, IHTMLElementCollection elements)
    {
      return new RadioButtonCollection(ie, FindAllElements(InputTagName, InputRadioButtonType, elements));
    }

    public static SelectList SelectList(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new SelectList(ie, new ElementFinder(SelectListTagName, InputNullType, findBy, elements));
    }

    public static SelectListCollection SelectLists(DomContainer ie, IHTMLElementCollection elements)
    {
      return new SelectListCollection(ie, FindAllElements(SelectListTagName, InputNullType, elements));
    }

    public static Table Table(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Table(ie, new ElementFinder(TableTagName, InputNullType, findBy, elements));
    }

    public static TableCollection Tables(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableCollection(ie, FindAllElements(TableTagName, InputNullType, elements));
    }

//    public static TableSectionCollection TableSections(IDomContainer ie, IHTMLElementCollection elements)
//    {
//      return new TableSectionCollection(ie, elements);
//    }

    public static TableCell TableCell(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new TableCell(ie, new ElementFinder(TableCellTagName, InputNullType, findBy, elements));
    }

    public static TableCell TableCell(DomContainer ie, string elementId, int occurrence, IHTMLElementCollection elements)
    {
      return new TableCell(ie, new ElementFinder(TableCellTagName, InputNullType, new IdAndOccurrence(elementId,occurrence), elements));
    }

    public static TableCellCollection TableCells(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableCellCollection(ie, FindAllElements(TableCellTagName, InputNullType, elements));
    }

    public static TableRow TableRow(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new TableRow(ie, new ElementFinder(TableRowTagName, InputNullType, findBy, elements));
    }

    public static TableRowCollection TableRows(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableRowCollection(ie, FindAllElements(TableRowTagName, InputNullType, elements));
    }

    public static TextField TextField(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      ElementFinder finder = new ElementFinder(InputTagName, InputTextFieldType, findBy, elements);
      finder.AddTagType(TextAreaTagName, InputNullType);
      finder.ExceptionMessage = string.Format("{0} ({1}) or {2}", InputTagName, InputTextFieldType, TextAreaTagName);
      
      return new TextField(ie, finder);
    }

    public static TextFieldCollection TextFields(DomContainer ie, IHTMLElementCollection elements)
    {
      ArrayList inputElements = FindAllElements(InputTagName, InputTextFieldType, elements);
      ArrayList textAreaElements = FindAllElements(TextAreaTagName, InputNullType, elements);
      
      return new TextFieldCollection(ie, inputElements, textAreaElements);
    }

    public static Span Span(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Span(ie, new ElementFinder(SpanTagName, InputNullType, findBy, elements));
    }

    public static SpanCollection Spans(DomContainer ie, IHTMLElementCollection elements)
    {
      return new SpanCollection(ie, FindAllElements(SpanTagName, InputNullType, elements));
    }

    public static Div Div(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Div(ie, new ElementFinder(DivTagName, InputNullType, findBy, elements));
    }

    public static DivCollection Divs(DomContainer ie, IHTMLElementCollection elements)
    {
      return new DivCollection(ie, FindAllElements(DivTagName, InputNullType, elements));
    }

    public static Image Image(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Image(ie, new ElementFinder(ImageTagName, InputNullType, findBy, elements));
    }

    public static ImageCollection Images(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ImageCollection(ie, FindAllElements(ImageTagName, InputNullType, elements));
    }

    public static IHTMLElement FindFirstElement(string tagName, string inputType, Attribute findBy, IHTMLElementCollection elementsCollection, bool throwExceptionIfElementNotFound)
    {
      ArrayList elements = findElementsByAttribute(tagName, inputType, findBy, elementsCollection, true);

      if (elements.Count > 0)
      {
        return (IHTMLElement)elements[0];
      }
    
      if (throwExceptionIfElementNotFound)
      {
        throw new ElementNotFoundException(tagName, findBy.AttributeName, findBy.Value);
      }
      
      return null;
    }

    public static ArrayList FindAllElements(string tagName, string inputType, IHTMLElementCollection elementsCollection)
    {
      return findElementsByAttribute(tagName, inputType, new NoAttributeCompare(), elementsCollection, false);
    }
    
    public static ArrayList FindFilteredElements(string tagName, string inputType, Attribute findBy, IHTMLElementCollection elementsCollection)
    {
      return findElementsByAttribute(tagName, inputType, findBy, elementsCollection, false);
    }
   
    internal static ArrayList findElementsByAttribute(string tagName, string inputType, Attribute findBy, IHTMLElementCollection elementsCollection, bool returnAfterFirstMatch)
    {
      // Check arguments
      if (isInputElement(tagName) && UtilityClass.IsNullOrEmpty(inputType))
      {
        throw new ArgumentNullException("inputType", "inputType must be set when tagName is 'input'");
      }

      // Get elements with the tagname from the page
      ArrayList children = new ArrayList();
      IHTMLElementCollection elements = getElementCollection(elementsCollection, tagName);

      // Loop through each element and evaluate
      foreach (IHTMLElement element in elements)
      {
        waitUntilElementReadyStateIsComplete(element, tagName);

        if (doCompare(element, findBy, inputType))
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
      return String.Compare(tagName, InputTagName, true) == 0;
    }    
  }
  
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
      IHTMLElement element = findElement();

      if (element == null && throwExceptionIfElementNotFound)
      {
        throw new ElementNotFoundException(ExceptionMessage, findBy.AttributeName, findBy.Value);
      }
      
      return element;
    }

    public void AddTagType(string tagName, string inputType)
    {
      tagsToFind.Add(new TagType(tagName, inputType));
    }
    
    private IHTMLElement findElement()
    {
      foreach (TagType tagType in tagsToFind)
      {
        IHTMLElement element = ElementsSupport.FindFirstElement(tagType.tagName, tagType.inputTypes, findBy, elementsCollection, false);
        if (element != null)
        {
          return element;
        }
      }
      
      return null;
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

