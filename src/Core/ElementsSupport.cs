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

using System.Text.RegularExpressions;

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
    public const string InputImageType = "image";
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
      ElementFinder finder = new ElementFinder(InputTagName, InputButtonType, elements);
      finder.AddTagType(ButtonTagName, InputNullType);

      return new ButtonCollection(ie, finder);
    }

    public static CheckBox CheckBox(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new CheckBox(ie, new ElementFinder(InputTagName, InputCheckBoxType, findBy, elements));
    }

    public static CheckBoxCollection CheckBoxes(DomContainer ie, IHTMLElementCollection elements)
    {
      return new CheckBoxCollection(ie, new ElementFinder(InputTagName, InputCheckBoxType, elements));
    }
    
    public static Element Element(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new ElementsContainer(ie, new ElementFinder(null, InputNullType, findBy, elements));
    }

    public static ElementCollection Elements(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ElementCollection(ie, new ElementFinder(null, InputNullType, elements));
    }

    public static FileUpload FileUpload(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new FileUpload(ie, new ElementFinder(InputTagName, InputFileType, findBy, elements));
    }

    public static FileUploadCollection FileUploads(DomContainer ie, IHTMLElementCollection elements)
    {
      return new FileUploadCollection(ie, new ElementFinder(InputTagName, InputFileType, elements));
    }
    
    public static Form Form(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Form(ie, new ElementFinder(FormTagName, InputNullType, findBy, elements));
    }

    public static FormCollection Forms(DomContainer ie, IHTMLElementCollection elements)
    {
      return new FormCollection(ie, new ElementFinder(FormTagName, InputNullType, elements));
    }

    public static Label Label(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Label(ie, new ElementFinder(LabelTagName, InputNullType, findBy, elements));
    }

    public static LabelCollection Labels(DomContainer ie, IHTMLElementCollection elements)
    {
      return new LabelCollection(ie, new ElementFinder(LabelTagName, InputNullType, elements));
    }

    public static Link Link(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Link(ie, new ElementFinder(LinkTagName, InputNullType, findBy, elements));
    }

    public static LinkCollection Links(DomContainer ie, IHTMLElementCollection elements)
    {
      return new LinkCollection(ie, new ElementFinder(LinkTagName, InputNullType, elements));
    }

    public static Para Para(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Para(ie, new ElementFinder(ParaTagName, InputNullType, findBy, elements));
    }

    public static ParaCollection Paras(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ParaCollection(ie, new ElementFinder(ParaTagName, InputNullType, elements));
    }

    public static RadioButton RadioButton(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new RadioButton(ie, new ElementFinder(InputTagName, InputRadioButtonType, findBy, elements));
    }

    public static RadioButtonCollection RadioButtons(DomContainer ie, IHTMLElementCollection elements)
    {
      return new RadioButtonCollection(ie, new ElementFinder(InputTagName, InputRadioButtonType, elements));
    }

    public static SelectList SelectList(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new SelectList(ie, new ElementFinder(SelectListTagName, InputNullType, findBy, elements));
    }

    public static SelectListCollection SelectLists(DomContainer ie, IHTMLElementCollection elements)
    {
      return new SelectListCollection(ie, new ElementFinder(SelectListTagName, InputNullType, elements));
    }

    public static Table Table(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Table(ie, new ElementFinder(TableTagName, InputNullType, findBy, elements));
    }

    public static TableCollection Tables(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableCollection(ie, new ElementFinder(TableTagName, InputNullType, elements));
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
    
    public static TableCell TableCell(DomContainer ie, Regex elementId, int occurrence, IHTMLElementCollection elements)
    {
      return new TableCell(ie, new ElementFinder(TableCellTagName, InputNullType, new IdAndOccurrence(elementId,occurrence), elements));
    }

    public static TableCellCollection TableCells(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableCellCollection(ie, new ElementFinder(TableCellTagName, InputNullType, elements));
    }

    public static TableRow TableRow(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new TableRow(ie, new ElementFinder(TableRowTagName, InputNullType, findBy, elements));
    }

    public static TableRowCollection TableRows(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableRowCollection(ie, new ElementFinder(TableRowTagName, InputNullType, elements));
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
      ElementFinder finder = new ElementFinder(InputTagName, InputTextFieldType, elements);
      finder.AddTagType(TextAreaTagName, InputNullType);
      
      return new TextFieldCollection(ie, finder);
    }

    public static Span Span(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Span(ie, new ElementFinder(SpanTagName, InputNullType, findBy, elements));
    }

    public static SpanCollection Spans(DomContainer ie, IHTMLElementCollection elements)
    {
      return new SpanCollection(ie, new ElementFinder(SpanTagName, InputNullType, elements));
    }

    public static Div Div(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Div(ie, new ElementFinder(DivTagName, InputNullType, findBy, elements));
    }

    public static DivCollection Divs(DomContainer ie, IHTMLElementCollection elements)
    {
      return new DivCollection(ie, new ElementFinder(DivTagName, InputNullType, elements));
    }

    public static Image Image(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      ElementFinder finder = new ElementFinder(ImageTagName, InputNullType, findBy, elements);
      finder.AddTagType(InputTagName, InputImageType);
      
      return new Image(ie, finder);
    }

    public static ImageCollection Images(DomContainer ie, IHTMLElementCollection elements)
    {
      ElementFinder finder = new ElementFinder(ImageTagName, InputNullType, elements);
      finder.AddTagType(InputTagName, InputImageType);
      
      return new ImageCollection(ie, finder);
    }
  }
}

