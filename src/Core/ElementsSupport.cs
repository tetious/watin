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
		public const string LabelTagName = "LABEL";
		/// <summary>
		/// Prevent creating an instance of this class (contains only static members)
		/// </summary>
		private ElementsSupport() {}

		public static Area Area(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Area(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Area.ElementTags, findBy, elements));
		}

		public static AreaCollection Areas(DomContainer domContainer, IElementCollection elements)
		{
			return new AreaCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Area.ElementTags, elements));
		}

		public static Button Button(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Button(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Button.ElementTags, findBy, elements));
		}

		public static ButtonCollection Buttons(DomContainer domContainer, IElementCollection elements)
		{
			return new ButtonCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Button.ElementTags, elements));
		}

		public static CheckBox CheckBox(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new CheckBox(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.CheckBox.ElementTags, findBy, elements));
		}

		public static CheckBoxCollection CheckBoxes(DomContainer domContainer, IElementCollection elements)
		{
			return new CheckBoxCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.CheckBox.ElementTags, elements));
		}

		public static Element Element(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
#if NET11
			return new ElementsContainer(domContainer, domContainer.NativeBrowser.CreateElementFinder(null, findBy, elements));
#else
            return new ElementsContainer<Element>(domContainer, domContainer.NativeBrowser.CreateElementFinder(null, findBy, elements));
#endif
        }

		public static Element Element(DomContainer domContainer, string tagname, BaseConstraint findBy, IElementCollection elements, params string[] inputtypes)
		{
			string inputtypesString = UtilityClass.StringArrayToString(inputtypes, ",");

#if NET11
			return new ElementsContainer(domContainer, domContainer.NativeBrowser.CreateElementFinder(tagname, inputtypesString, findBy, elements));
#else
            return new ElementsContainer<Element>(domContainer, domContainer.NativeBrowser.CreateElementFinder(tagname, inputtypesString, findBy, elements));
#endif
		}

		public static ElementCollection Elements(DomContainer domContainer, IElementCollection elements)
		{
			return new ElementCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(null, elements));
		}

		public static FileUpload FileUpload(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new FileUpload(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.FileUpload.ElementTags, findBy, elements));
		}

		public static FileUploadCollection FileUploads(DomContainer domContainer, IElementCollection elements)
		{
			return new FileUploadCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.FileUpload.ElementTags, elements));
		}

		public static Form Form(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Form(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Form.ElementTags, findBy, elements));
		}

		public static FormCollection Forms(DomContainer domContainer, IElementCollection elements)
		{
			return new FormCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Form.ElementTags, elements));
		}

		public static Label Label(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Label(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Label.ElementTags, findBy, elements));
		}

		public static LabelCollection Labels(DomContainer domContainer, IElementCollection elements)
		{
			return new LabelCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Label.ElementTags, elements));
		}

		public static Link Link(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Link(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Link.ElementTags, findBy, elements));
		}

		public static LinkCollection Links(DomContainer domContainer, IElementCollection elements)
		{
			return new LinkCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Link.ElementTags, elements));
		}

		public static Option Option(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Option(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Option.ElementTags, findBy, elements));
		}

		public static OptionCollection Options(DomContainer domContainer, IElementCollection elements)
		{
			return new OptionCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Option.ElementTags, elements));
		}

		public static Para Para(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Para(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Para.ElementTags, findBy, elements));
		}

		public static ParaCollection Paras(DomContainer domContainer, IElementCollection elements)
		{
			return new ParaCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Para.ElementTags, elements));
		}

		public static RadioButton RadioButton(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new RadioButton(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.RadioButton.ElementTags, findBy, elements));
		}

		public static RadioButtonCollection RadioButtons(DomContainer domContainer, IElementCollection elements)
		{
			return new RadioButtonCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.RadioButton.ElementTags, elements));
		}

		public static SelectList SelectList(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new SelectList(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.SelectList.ElementTags, findBy, elements));
		}

		public static SelectListCollection SelectLists(DomContainer domContainer, IElementCollection elements)
		{
			return new SelectListCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.SelectList.ElementTags, elements));
		}

		public static Table Table(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Table(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Table.ElementTags, findBy, elements));
		}

		public static TableCollection Tables(DomContainer domContainer, IElementCollection elements)
		{
			return new TableCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Table.ElementTags, elements));
		}

		public static TableCell TableCell(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TableCell(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TableCell.ElementTags, findBy, elements));
		}

		public static TableCell TableCell(DomContainer domContainer, string elementId, int index, IElementCollection elements)
		{
			return new TableCell(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TableCell.ElementTags, Find.ByIndex(index).And(Find.ById(elementId)), elements));
		}

		public static TableCell TableCell(DomContainer domContainer, Regex elementId, int index, IElementCollection elements)
		{
			return new TableCell(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TableCell.ElementTags, Find.ByIndex(index).And(Find.ById(elementId)), elements));
		}

		public static TableCellCollection TableCells(DomContainer domContainer, IElementCollection elements)
		{
			return new TableCellCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TableCell.ElementTags, elements));
		}

		public static TableRow TableRow(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TableRow(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TableRow.ElementTags, findBy, elements));
		}

		public static TableRowCollection TableRows(DomContainer domContainer, IElementCollection elements)
		{
			return new TableRowCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TableRow.ElementTags, elements));
		}

		public static TableBody TableBody(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TableBody(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TableBody.ElementTags, findBy, elements));
		}

		public static TableBodyCollection TableBodies(DomContainer domContainer, IElementCollection elements)
		{
			return new TableBodyCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TableBody.ElementTags, elements));
		}

		public static TextField TextField(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TextField(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TextField.ElementTags, findBy, elements));
		}

		public static TextFieldCollection TextFields(DomContainer domContainer, IElementCollection elements)
		{
			return new TextFieldCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.TextField.ElementTags, elements));
		}

		public static Span Span(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Span(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Span.ElementTags, findBy, elements));
		}

		public static SpanCollection Spans(DomContainer domContainer, IElementCollection elements)
		{
			return new SpanCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Span.ElementTags, elements));
		}

		public static Div Div(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Div(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Div.ElementTags, findBy, elements));
		}

		public static DivCollection Divs(DomContainer domContainer, IElementCollection elements)
		{
			return new DivCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Div.ElementTags, elements));
		}

		public static Image Image(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Image(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Image.ElementTags, findBy, elements));
		}

		public static ImageCollection Images(DomContainer domContainer, IElementCollection elements)
		{
			return new ImageCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(Core.Image.ElementTags, elements));
		}
	}
}
