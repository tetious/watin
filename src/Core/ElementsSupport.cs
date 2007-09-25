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

using System.Text.RegularExpressions;

namespace WatiN.Core
{
  using System.Collections;
  using WatiN.Core.Interfaces;

  /// <summary>
  /// Summary description for SubElements.
  /// </summary>
  public class ElementsSupport
  {
    public const string FrameTagName = "FRAME";
    public const string InputTagName = "INPUT";
    public const string TableCellTagName = "TD";
    
    /// <summary>
    /// Prevent creating an instance of this class (contains only static members)
    /// </summary>
    private ElementsSupport(){}

		public static Area Area(DomContainer domContainer, AttributeConstraint findBy, IElementCollection elements)
		{
			return new Area(domContainer, new ElementFinder(Core.Area.ElementTags, findBy, elements));
		}

		public static AreaCollection Areas(DomContainer domContainer, IElementCollection elements)
		{
			return new AreaCollection(domContainer, new ElementFinder(Core.Area.ElementTags, elements));
		}    

		public static Button Button(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
		{
			return new Button(ie, new ElementFinder(Core.Button.ElementTags, findBy, elements));
		}

		public static ButtonCollection Buttons(DomContainer ie, IElementCollection elements)
		{
			return new ButtonCollection(ie, new ElementFinder(Core.Button.ElementTags, elements));
		}
    
    public static CheckBox CheckBox(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new CheckBox(ie, new ElementFinder(Core.CheckBox.ElementTags, findBy, elements));
    }

    public static CheckBoxCollection CheckBoxes(DomContainer ie, IElementCollection elements)
    {
      return new CheckBoxCollection(ie, new ElementFinder(Core.CheckBox.ElementTags, elements));
    }
    
    public static Element Element(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new ElementsContainer(ie, new ElementFinder(null, findBy, elements));
    }

    public static Element Element(DomContainer domContainer, string tagname, AttributeConstraint findBy, IElementCollection elements, params string[] inputtypes)
    {
      string inputtypesString = UtilityClass.StringArrayToString(inputtypes, ",");

      return new ElementsContainer(domContainer, new ElementFinder(tagname, inputtypesString, findBy, elements));
    }

    public static ElementCollection Elements(DomContainer ie, IElementCollection elements)
    {
      return new ElementCollection(ie, new ElementFinder(null, elements));
    }

    public static FileUpload FileUpload(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new FileUpload(ie, new ElementFinder(Core.FileUpload.ElementTags, findBy, elements));
    }

    public static FileUploadCollection FileUploads(DomContainer ie, IElementCollection elements)
    {
      return new FileUploadCollection(ie, new ElementFinder(Core.FileUpload.ElementTags, elements));
    }
    
    public static Form Form(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new Form(ie, new ElementFinder(Core.Form.ElementTags, findBy, elements));
    }

    public static FormCollection Forms(DomContainer ie, IElementCollection elements)
    {
      return new FormCollection(ie, new ElementFinder(Core.Form.ElementTags, elements));
    }

    public static Label Label(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new Label(ie, new ElementFinder(Core.Label.ElementTags, findBy, elements));
    }

    public static LabelCollection Labels(DomContainer ie, IElementCollection elements)
    {
      return new LabelCollection(ie, new ElementFinder(Core.Label.ElementTags, elements));
    }

    public static Link Link(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new Link(ie, new ElementFinder(Core.Link.ElementTags, findBy, elements));
    }

    public static LinkCollection Links(DomContainer ie, IElementCollection elements)
    {
      return new LinkCollection(ie, new ElementFinder(Core.Link.ElementTags, elements));
    }

    public static Option Option(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new Option(ie, new ElementFinder(Core.Option.ElementTags, findBy, elements));
    }

    public static OptionCollection Options(DomContainer ie, IElementCollection elements)
    {
      return new OptionCollection(ie, new ElementFinder(Core.Option.ElementTags, elements));
    }

    public static Para Para(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new Para(ie, new ElementFinder(Core.Para.ElementTags, findBy, elements));
    }

    public static ParaCollection Paras(DomContainer ie, IElementCollection elements)
    {
      return new ParaCollection(ie, new ElementFinder(Core.Para.ElementTags, elements));
    }

    public static RadioButton RadioButton(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new RadioButton(ie, new ElementFinder(Core.RadioButton.ElementTags, findBy, elements));
    }

    public static RadioButtonCollection RadioButtons(DomContainer ie, IElementCollection elements)
    {
      return new RadioButtonCollection(ie, new ElementFinder(Core.RadioButton.ElementTags, elements));
    }

    public static SelectList SelectList(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new SelectList(ie, new ElementFinder(Core.SelectList.ElementTags, findBy, elements));
    }

    public static SelectListCollection SelectLists(DomContainer ie, IElementCollection elements)
    {
      return new SelectListCollection(ie, new ElementFinder(Core.SelectList.ElementTags, elements));
    }

    public static Table Table(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new Table(ie, new ElementFinder(Core.Table.ElementTags, findBy, elements));
    }

    public static TableCollection Tables(DomContainer ie, IElementCollection elements)
    {
      return new TableCollection(ie, new ElementFinder(Core.Table.ElementTags, elements));
    }
      

//    public static TableSectionCollection TableSections(IDomContainer ie, IElementCollection elements)
//    {
//      return new TableSectionCollection(ie, elements);
//    }

    public static TableCell TableCell(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new TableCell(ie, new ElementFinder(Core.TableCell.ElementTags, findBy, elements));
    }

    public static TableCell TableCell(DomContainer ie, string elementId, int index, IElementCollection elements)
    {
      return new TableCell(ie, new ElementFinder(Core.TableCell.ElementTags, Find.ByIndex(index).And(Find.ById(elementId)), elements));
    }
    
    public static TableCell TableCell(DomContainer ie, Regex elementId, int index, IElementCollection elements)
    {
      return new TableCell(ie, new ElementFinder(Core.TableCell.ElementTags, Find.ByIndex(index).And(Find.ById(elementId)), elements));
    }

    public static TableCellCollection TableCells(DomContainer ie, IElementCollection elements)
    {
      return new TableCellCollection(ie, new ElementFinder(Core.TableCell.ElementTags, elements));
    }

    public static TableRow TableRow(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new TableRow(ie, new ElementFinder(Core.TableRow.ElementTags, findBy, elements));
    }

    public static TableRowCollection TableRows(DomContainer ie, IElementCollection elements)
    {
      return new TableRowCollection(ie, new ElementFinder(Core.TableRow.ElementTags, elements));
    }

    public static TableBody TableBody(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
        return new TableBody(ie, new ElementFinder(Core.TableBody.ElementTags, findBy, elements));
    }

    public static TableBodyCollection TableBodies(DomContainer ie, IElementCollection elements)
    {
        return new TableBodyCollection(ie, new ElementFinder(Core.TableBody.ElementTags, elements));
    }

    public static TextField TextField(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new TextField(ie, new ElementFinder(Core.TextField.ElementTags, findBy, elements));
    }

    public static TextFieldCollection TextFields(DomContainer ie, IElementCollection elements)
    {
      return new TextFieldCollection(ie, new ElementFinder(Core.TextField.ElementTags, elements));
    }

    public static Span Span(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new Span(ie, new ElementFinder(Core.Span.ElementTags, findBy, elements));
    }

    public static SpanCollection Spans(DomContainer ie, IElementCollection elements)
    {
      return new SpanCollection(ie, new ElementFinder(Core.Span.ElementTags, elements));
    }

    public static Div Div(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new Div(ie, new ElementFinder(Core.Div.ElementTags, findBy, elements));
    }

    public static DivCollection Divs(DomContainer ie, IElementCollection elements)
    {
      return new DivCollection(ie, new ElementFinder(Core.Div.ElementTags, elements));
    }

    public static Image Image(DomContainer ie, AttributeConstraint findBy, IElementCollection elements)
    {
      return new Image(ie, new ElementFinder(Core.Image.ElementTags, findBy, elements));
    }

    public static ImageCollection Images(DomContainer ie, IElementCollection elements)
    {
      return new ImageCollection(ie, new ElementFinder(Core.Image.ElementTags, elements));
    }
  }
}


