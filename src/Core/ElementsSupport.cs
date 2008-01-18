#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
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
		private ElementsSupport() {}

		public static Area Area(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Area(domContainer, new ElementFinder(Core.Area.ElementTags, findBy, elements));
		}

		public static AreaCollection Areas(DomContainer domContainer, IElementCollection elements)
		{
			return new AreaCollection(domContainer, new ElementFinder(Core.Area.ElementTags, elements));
		}

		public static Button Button(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Button(domContainer, new ElementFinder(Core.Button.ElementTags, findBy, elements));
		}

		public static ButtonCollection Buttons(DomContainer domContainer, IElementCollection elements)
		{
			return new ButtonCollection(domContainer, new ElementFinder(Core.Button.ElementTags, elements));
		}

		public static CheckBox CheckBox(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new CheckBox(domContainer, new ElementFinder(Core.CheckBox.ElementTags, findBy, elements));
		}

		public static CheckBoxCollection CheckBoxes(DomContainer domContainer, IElementCollection elements)
		{
			return new CheckBoxCollection(domContainer, new ElementFinder(Core.CheckBox.ElementTags, elements));
		}

		public static Element Element(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new ElementsContainer(domContainer, new ElementFinder(null, findBy, elements));
		}

		public static Element Element(DomContainer domContainer, string tagname, BaseConstraint findBy, IElementCollection elements, params string[] inputtypes)
		{
			string inputtypesString = UtilityClass.StringArrayToString(inputtypes, ",");

			return new ElementsContainer(domContainer, new ElementFinder(tagname, inputtypesString, findBy, elements));
		}

		public static ElementCollection Elements(DomContainer domContainer, IElementCollection elements)
		{
			return new ElementCollection(domContainer, new ElementFinder(null, elements));
		}

		public static FileUpload FileUpload(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new FileUpload(domContainer, new ElementFinder(Core.FileUpload.ElementTags, findBy, elements));
		}

		public static FileUploadCollection FileUploads(DomContainer domContainer, IElementCollection elements)
		{
			return new FileUploadCollection(domContainer, new ElementFinder(Core.FileUpload.ElementTags, elements));
		}

		public static Form Form(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Form(domContainer, new ElementFinder(Core.Form.ElementTags, findBy, elements));
		}

		public static FormCollection Forms(DomContainer domContainer, IElementCollection elements)
		{
			return new FormCollection(domContainer, new ElementFinder(Core.Form.ElementTags, elements));
		}

		public static Label Label(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Label(domContainer, new ElementFinder(Core.Label.ElementTags, findBy, elements));
		}

		public static LabelCollection Labels(DomContainer domContainer, IElementCollection elements)
		{
			return new LabelCollection(domContainer, new ElementFinder(Core.Label.ElementTags, elements));
		}

		public static Link Link(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Link(domContainer, new ElementFinder(Core.Link.ElementTags, findBy, elements));
		}

		public static LinkCollection Links(DomContainer domContainer, IElementCollection elements)
		{
			return new LinkCollection(domContainer, new ElementFinder(Core.Link.ElementTags, elements));
		}

		public static Option Option(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Option(domContainer, new ElementFinder(Core.Option.ElementTags, findBy, elements));
		}

		public static OptionCollection Options(DomContainer domContainer, IElementCollection elements)
		{
			return new OptionCollection(domContainer, new ElementFinder(Core.Option.ElementTags, elements));
		}

		public static Para Para(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Para(domContainer, new ElementFinder(Core.Para.ElementTags, findBy, elements));
		}

		public static ParaCollection Paras(DomContainer domContainer, IElementCollection elements)
		{
			return new ParaCollection(domContainer, new ElementFinder(Core.Para.ElementTags, elements));
		}

		public static RadioButton RadioButton(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new RadioButton(domContainer, new ElementFinder(Core.RadioButton.ElementTags, findBy, elements));
		}

		public static RadioButtonCollection RadioButtons(DomContainer domContainer, IElementCollection elements)
		{
			return new RadioButtonCollection(domContainer, new ElementFinder(Core.RadioButton.ElementTags, elements));
		}

		public static SelectList SelectList(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new SelectList(domContainer, new ElementFinder(Core.SelectList.ElementTags, findBy, elements));
		}

		public static SelectListCollection SelectLists(DomContainer domContainer, IElementCollection elements)
		{
			return new SelectListCollection(domContainer, new ElementFinder(Core.SelectList.ElementTags, elements));
		}

		public static Table Table(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Table(domContainer, new ElementFinder(Core.Table.ElementTags, findBy, elements));
		}

		public static TableCollection Tables(DomContainer domContainer, IElementCollection elements)
		{
			return new TableCollection(domContainer, new ElementFinder(Core.Table.ElementTags, elements));
		}


//    public static TableSectionCollection TableSections(IDomContainer domContainer, IElementCollection elements)
//    {
//      return new TableSectionCollection(ie, elements);
//    }

		public static TableCell TableCell(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TableCell(domContainer, new ElementFinder(Core.TableCell.ElementTags, findBy, elements));
		}

		public static TableCell TableCell(DomContainer domContainer, string elementId, int index, IElementCollection elements)
		{
			return new TableCell(domContainer, new ElementFinder(Core.TableCell.ElementTags, Find.ByIndex(index).And(Find.ById(elementId)), elements));
		}

		public static TableCell TableCell(DomContainer domContainer, Regex elementId, int index, IElementCollection elements)
		{
			return new TableCell(domContainer, new ElementFinder(Core.TableCell.ElementTags, Find.ByIndex(index).And(Find.ById(elementId)), elements));
		}

		public static TableCellCollection TableCells(DomContainer domContainer, IElementCollection elements)
		{
			return new TableCellCollection(domContainer, new ElementFinder(Core.TableCell.ElementTags, elements));
		}

		public static TableRow TableRow(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TableRow(domContainer, new ElementFinder(Core.TableRow.ElementTags, findBy, elements));
		}

		public static TableRowCollection TableRows(DomContainer domContainer, IElementCollection elements)
		{
			return new TableRowCollection(domContainer, new ElementFinder(Core.TableRow.ElementTags, elements));
		}

		public static TableBody TableBody(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TableBody(domContainer, new ElementFinder(Core.TableBody.ElementTags, findBy, elements));
		}

		public static TableBodyCollection TableBodies(DomContainer domContainer, IElementCollection elements)
		{
			return new TableBodyCollection(domContainer, new ElementFinder(Core.TableBody.ElementTags, elements));
		}

		public static TextField TextField(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TextField(domContainer, new ElementFinder(Core.TextField.ElementTags, findBy, elements));
		}

		public static TextFieldCollection TextFields(DomContainer domContainer, IElementCollection elements)
		{
			return new TextFieldCollection(domContainer, new ElementFinder(Core.TextField.ElementTags, elements));
		}

		public static Span Span(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Span(domContainer, new ElementFinder(Core.Span.ElementTags, findBy, elements));
		}

		public static SpanCollection Spans(DomContainer domContainer, IElementCollection elements)
		{
			return new SpanCollection(domContainer, new ElementFinder(Core.Span.ElementTags, elements));
		}

		public static Div Div(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Div(domContainer, new ElementFinder(Core.Div.ElementTags, findBy, elements));
		}

		public static DivCollection Divs(DomContainer domContainer, IElementCollection elements)
		{
			return new DivCollection(domContainer, new ElementFinder(Core.Div.ElementTags, elements));
		}

		public static Image Image(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Image(domContainer, new ElementFinder(Core.Image.ElementTags, findBy, elements));
		}

		public static ImageCollection Images(DomContainer domContainer, IElementCollection elements)
		{
			return new ImageCollection(domContainer, new ElementFinder(Core.Image.ElementTags, elements));
		}
	}
}