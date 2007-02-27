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

using System.Text.RegularExpressions;

using mshtml;

namespace WatiN.Core
{
    using System;

    /// <summary>
  /// Summary description for SubElements.
  /// </summary>
  public sealed class ElementsSupport
  {
    public const string FrameTagName = "FRAME";
    public const string InputTagName = "INPUT";
    public const string TableCellTagName = "TD";
    
    /// <summary>
    /// Prevent creating an instance of this class (contains only static members)
    /// </summary>
    private ElementsSupport(){}
        
    public static CheckBox CheckBox(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new CheckBox(ie, new ElementFinder(Core.CheckBox.ElementTags, findBy, elements));
    }

    public static CheckBoxCollection CheckBoxes(DomContainer ie, IHTMLElementCollection elements)
    {
      return new CheckBoxCollection(ie, new ElementFinder(Core.CheckBox.ElementTags, elements));
    }
    
    public static Element Element(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new ElementsContainer(ie, new ElementFinder(null, findBy, elements));
    }

    public static ElementCollection Elements(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ElementCollection(ie, new ElementFinder(null, elements));
    }

    public static FileUpload FileUpload(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new FileUpload(ie, new ElementFinder(Core.FileUpload.ElementTags, findBy, elements));
    }

    public static FileUploadCollection FileUploads(DomContainer ie, IHTMLElementCollection elements)
    {
      return new FileUploadCollection(ie, new ElementFinder(Core.FileUpload.ElementTags, elements));
    }
    
    public static Form Form(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Form(ie, new ElementFinder(Core.Form.ElementTags, findBy, elements));
    }

    public static FormCollection Forms(DomContainer ie, IHTMLElementCollection elements)
    {
      return new FormCollection(ie, new ElementFinder(Core.Form.ElementTags, elements));
    }

    public static Label Label(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Label(ie, new ElementFinder(Core.Label.ElementTags, findBy, elements));
    }

    public static LabelCollection Labels(DomContainer ie, IHTMLElementCollection elements)
    {
      return new LabelCollection(ie, new ElementFinder(Core.Label.ElementTags, elements));
    }

    public static Link Link(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Link(ie, new ElementFinder(Core.Link.ElementTags, findBy, elements));
    }

    public static LinkCollection Links(DomContainer ie, IHTMLElementCollection elements)
    {
      return new LinkCollection(ie, new ElementFinder(Core.Link.ElementTags, elements));
    }

    public static Option Option(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Option(ie, new ElementFinder(Core.Option.ElementTags, findBy, elements));
    }

    public static OptionCollection Options(DomContainer ie, IHTMLElementCollection elements)
    {
      return new OptionCollection(ie, new ElementFinder(Core.Option.ElementTags, elements));
    }

    public static Para Para(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Para(ie, new ElementFinder(Core.Para.ElementTags, findBy, elements));
    }

    public static ParaCollection Paras(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ParaCollection(ie, new ElementFinder(Core.Para.ElementTags, elements));
    }

    public static RadioButton RadioButton(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new RadioButton(ie, new ElementFinder(Core.RadioButton.ElementTags, findBy, elements));
    }

    public static RadioButtonCollection RadioButtons(DomContainer ie, IHTMLElementCollection elements)
    {
      return new RadioButtonCollection(ie, new ElementFinder(Core.RadioButton.ElementTags, elements));
    }

    public static SelectList SelectList(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new SelectList(ie, new ElementFinder(Core.SelectList.ElementTags, findBy, elements));
    }

    public static SelectListCollection SelectLists(DomContainer ie, IHTMLElementCollection elements)
    {
      return new SelectListCollection(ie, new ElementFinder(Core.SelectList.ElementTags, elements));
    }

    public static Table Table(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Table(ie, new ElementFinder(Core.Table.ElementTags, findBy, elements));
    }

    public static TableCollection Tables(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableCollection(ie, new ElementFinder(Core.Table.ElementTags, elements));
    }
      

//    public static TableSectionCollection TableSections(IDomContainer ie, IHTMLElementCollection elements)
//    {
//      return new TableSectionCollection(ie, elements);
//    }

    public static TableCell TableCell(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new TableCell(ie, new ElementFinder(Core.TableCell.ElementTags, findBy, elements));
    }

    public static TableCell TableCell(DomContainer ie, string elementId, int index, IHTMLElementCollection elements)
    {
      return new TableCell(ie, new ElementFinder(Core.TableCell.ElementTags, new Index(index).And(new Id(elementId)), elements));
    }
    
    public static TableCell TableCell(DomContainer ie, Regex elementId, int index, IHTMLElementCollection elements)
    {
      return new TableCell(ie, new ElementFinder(Core.TableCell.ElementTags, new Index(index).And(new Id(elementId)), elements));
    }

    public static TableCellCollection TableCells(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableCellCollection(ie, new ElementFinder(Core.TableCell.ElementTags, elements));
    }

    public static TableRow TableRow(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new TableRow(ie, new ElementFinder(Core.TableRow.ElementTags, findBy, elements));
    }

    public static TableRowCollection TableRows(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TableRowCollection(ie, new ElementFinder(Core.TableRow.ElementTags, elements));
    }
      public static TableBody TableBody(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
      {
          return new TableBody(ie, new ElementFinder(Core.TableBody.ElementTags, findBy, elements));
      }

    public static TableBodyCollection TableBodies(DomContainer ie, IHTMLElementCollection elements)
    {
        return new TableBodyCollection(ie, new ElementFinder(Core.TableBody.ElementTags, elements));
    }

      public static TextField TextField(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new TextField(ie, new ElementFinder(Core.TextField.ElementTags, findBy, elements));
    }

    public static TextFieldCollection TextFields(DomContainer ie, IHTMLElementCollection elements)
    {
      return new TextFieldCollection(ie, new ElementFinder(Core.TextField.ElementTags, elements));
    }

    public static Span Span(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Span(ie, new ElementFinder(Core.Span.ElementTags, findBy, elements));
    }

    public static SpanCollection Spans(DomContainer ie, IHTMLElementCollection elements)
    {
      return new SpanCollection(ie, new ElementFinder(Core.Span.ElementTags, elements));
    }

    public static Div Div(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Div(ie, new ElementFinder(Core.Div.ElementTags, findBy, elements));
    }

    public static DivCollection Divs(DomContainer ie, IHTMLElementCollection elements)
    {
      return new DivCollection(ie, new ElementFinder(Core.Div.ElementTags, elements));
    }

    public static Image Image(DomContainer ie, Attribute findBy, IHTMLElementCollection elements)
    {
      return new Image(ie, new ElementFinder(Core.Image.ElementTags, findBy, elements));
    }

    public static ImageCollection Images(DomContainer ie, IHTMLElementCollection elements)
    {
      return new ImageCollection(ie, new ElementFinder(Core.Image.ElementTags, elements));
    }

      
  }
}

