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
using System.Threading;

using mshtml;

using WatiN.Core.Exceptions;

namespace WatiN.Core
{
  /// <summary>
  /// Summary description for SubElements.
  /// </summary>
  public sealed class SubElementsSupport
  {
    /// <summary>
    /// Prevent creating an instance of this class (contains only static members)
    /// </summary>
    private SubElementsSupport(){}

    public static Button Button(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new Button(ie, (HTMLInputElement)FindElementByAttribute("input", "button submit image reset", findBy, elements));
    }

    public static ButtonCollection Buttons(DomContainer ie, IHTMLElementCollection elements)
    {
       return new ButtonCollection(ie, elements);
    }

    public static CheckBox CheckBox(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new CheckBox(ie, (IHTMLInputElement) FindElementByAttribute("input", "checkbox", findBy, elements));
    }

    public static CheckBoxCollection CheckBoxes(DomContainer ie, IHTMLElementCollection elements)
    {
      return new CheckBoxCollection(ie, elements); 
    }

    public static Form Form(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new Form(ie, (HTMLFormElement) FindElementByAttribute("form", null, findBy, elements));
    }

    public static FormCollection Forms(DomContainer ie, IHTMLElementCollection elements)
    {
      return new FormCollection(ie, elements);
    }

    public static Label Label(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new Label(ie, (HTMLLabelElement) FindElementByAttribute("label", null, findBy, elements));
    }

    public static LabelCollection Labels(DomContainer ie, IHTMLElementCollection elements)
    {
      return new LabelCollection(ie, elements);
    }

    public static Link Link(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new Link(ie, (HTMLAnchorElement) FindElementByAttribute("A", null, findBy, elements));
    }

    public static LinkCollection Links(DomContainer ie, IHTMLElementCollection elements)
    {
      return new LinkCollection(ie, elements);
    }

    public static Para Para(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new Para(ie, (HTMLParaElement) FindElementByAttribute("p", null, findBy, elements));
    }

    public static ParaCollection Paras(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ParaCollection(ie, elements); 
    }

    public static RadioButton RadioButton(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new RadioButton(ie, (IHTMLInputElement) FindElementByAttribute("input", "radio", findBy, elements));
    }

    public static RadioButtonCollection RadioButtons(DomContainer ie, IHTMLElementCollection elements)
    {
      return new RadioButtonCollection(ie, elements); 
    }

    public static SelectList SelectList(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new SelectList(ie, FindElementByAttribute("select", null, findBy, elements));
    }

    public static SelectListCollection SelectLists(DomContainer ie, IHTMLElementCollection elements)
    {
      return new SelectListCollection(ie, elements);
    }

    public static Table Table(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new Table(ie, (HTMLTable) FindElementByAttribute("table", null, findBy, elements));
    }

    public static TableCollection Tables(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableCollection(ie, elements); 
    }

//    public static TableSectionCollection TableSections(IDomContainer ie, IHTMLElementCollection elements)
//    {
//      return new TableSectionCollection(ie, elements);
//    }

    public static TableCell TableCell(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new TableCell(ie, (HTMLTableCell) FindElementByAttribute("TD", null, findBy, elements));
    }

    public static TableCell TableCell(DomContainer ie, string elementId, int occurrence, IHTMLElementCollection elementCollection)
    {
      int nr = -1;
      IHTMLElementCollection elements = (IHTMLElementCollection)elementCollection.tags("TD");
      foreach (IHTMLElement e in elements)
      {
        if (e.id == elementId)
        {
          ++nr;
          if (nr == occurrence)
          {
            return new TableCell(ie, (HTMLTableCell) e);
          }
        }
      }
      return null;
    }

    public static TableCellCollection TableCells(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableCellCollection(ie, elements); 
    }

    public static TableRow TableRow(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new TableRow(ie, (HTMLTableRow) FindElementByAttribute("TR", null, findBy, elements));
    }

    public static TableRowCollection TableRows(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableRowCollection(ie, elements);
    }

    public static TextField TextField(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      TextField textfield;

      try
      {
        textfield = new TextField(ie, (HTMLInputElement)FindElementByAttribute("input", "text password textarea hidden", findBy, elements));
      }
      catch (ElementNotFoundException)
      {
        try
        {
          textfield = new TextField(ie, (HTMLTextAreaElement)FindElementByAttribute("textarea", null, findBy, elements));
        }
        catch (ElementNotFoundException)
        {
          throw new ElementNotFoundException("input (text, password, textarea, hidden) or textarea", findBy.AttributeName, findBy.Value);
        }
      }

      return textfield;
    }

    public static TextFieldCollection TextFields(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TextFieldCollection(ie, elements);
    }

    public static Span Span(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new Span(ie, (HTMLSpanElement) FindElementByAttribute("span", null, findBy, elements));
    }

    public static SpanCollection Spans(DomContainer ie, IHTMLElementCollection elements)
    {
      return new SpanCollection(ie, elements);
    }

    public static Div Div(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new Div(ie, (HTMLDivElement) FindElementByAttribute("div", null, findBy, elements));
    }

    public static DivCollection Divs(DomContainer ie, IHTMLElementCollection elements)
    {
      return new DivCollection(ie, elements);
    }

    public static Image Image(DomContainer ie, AttributeValue findBy, IHTMLElementCollection elements)
    {
      return new Image(ie, (IHTMLImgElement) FindElementByAttribute("img", null, findBy, elements));
    }

    public static ImageCollection Images(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ImageCollection(ie, elements);
    }

    private static IHTMLElement FindElementByAttribute(string tagName, string inputType, AttributeValue findBy, IHTMLElementCollection elementsCollection)
    {
      if (IsInputElement(tagName) && (inputType == null || inputType.Length == 0))
      {
        throw new ArgumentNullException("inputType", "inputType must be set when tagName is 'input'");
      }

      IHTMLElementCollection elements = getElementCollectionByTagName(elementsCollection, tagName);

      foreach (IHTMLElement element in elements)
      {
        while (((IHTMLElement2)element).readyStateValue != 4 )
        {
          Thread.Sleep(100);
        }

        string compareValue = getAttributeValue(findBy, element);

        if (findBy.Compare(compareValue))
        {
          if (IsInputElement(tagName))
          {
            string inputElementType = ((IHTMLInputElement) element).type.ToLower();
            if (inputType.ToLower().IndexOf(inputElementType) >= 0)
            {
              return element;
            }
          }
          else
          {
            return element;
          }
        }
      }

      throw new ElementNotFoundException(tagName, findBy.AttributeName, findBy.Value);
    }

    private static IHTMLElementCollection getElementCollectionByTagName(IHTMLElementCollection elements, string tagName)
    {
      IHTMLElementCollection elementsWithTagName = (IHTMLElementCollection) elements.tags(tagName);

      return elementsWithTagName;
    }

    private static bool IsInputElement(string tagName)
    {
      return String.Compare(tagName, "input", true) == 0;
    }

    private static string getAttributeValue(AttributeValue findBy, IHTMLElement element)
    {
      if (findBy is IdValue)
      {
        return element.id;        
      }

      else if (findBy is TextValue)
      {
        if (element.innerText != null)
        {
          return element.innerText.Trim();
        }
      }

      else 
      {
        
        object attribute = element.getAttribute(findBy.AttributeName, 0);

        if (attribute == DBNull.Value)
        {
          throw new InvalidAttributException(findBy.AttributeName, element.tagName);
        }

        return (string) attribute;
      }

      return null;
    }
  }
}

