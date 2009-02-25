#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
	/// <summary>
	/// Summary description for SubElements.
	/// </summary>
	internal static class ElementsSupport
	{
		public const string FrameTagName = "FRAME";
		public const string IFrameTagName = "IFRAME";
		public const string InputTagName = "INPUT";

		public static Area Area(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Area(domContainer, CreateElementFinder<Area>(domContainer, findBy, elements));
		}

		public static AreaCollection Areas(DomContainer domContainer, IElementCollection elements)
		{
			return new AreaCollection(domContainer, CreateElementFinder<Area>(domContainer, elements));
		}

		public static Button Button(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Button(domContainer, CreateElementFinder<Button>(domContainer, findBy, elements));
		}

		public static ButtonCollection Buttons(DomContainer domContainer, IElementCollection elements)
		{
			return new ButtonCollection(domContainer, CreateElementFinder<Button>(domContainer, elements));
		}

		public static CheckBox CheckBox(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new CheckBox(domContainer, CreateElementFinder<CheckBox>(domContainer, findBy, elements));
		}

		public static CheckBoxCollection CheckBoxes(DomContainer domContainer, IElementCollection elements)
		{
			return new CheckBoxCollection(domContainer, CreateElementFinder<CheckBox>(domContainer, elements));
		}

		public static Element Element(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
            return new ElementsContainer<Element>(domContainer, domContainer.NativeBrowser.CreateElementFinder(null, findBy, elements));
        }

		public static Element Element(DomContainer domContainer, string tagName, BaseConstraint findBy, IElementCollection elements, params string[] inputTypes)
		{
            List<ElementTag> tags = new List<ElementTag>();
            if (inputTypes != null && inputTypes.Length != 0)
            {
                foreach (string inputType in inputTypes)
                    tags.Add(new ElementTag(tagName, inputType));
            }
            else
            {
                tags.Add(new ElementTag(tagName));
            }

		    return new ElementsContainer<Element>(domContainer, domContainer.NativeBrowser.CreateElementFinder(tags, findBy, elements));
		}

		public static ElementCollection Elements(DomContainer domContainer, IElementCollection elements)
		{
			return new ElementCollection(domContainer, domContainer.NativeBrowser.CreateElementFinder(null, null, elements));
		}

		public static FileUpload FileUpload(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new FileUpload(domContainer, CreateElementFinder<FileUpload>(domContainer, findBy, elements));
		}

		public static FileUploadCollection FileUploads(DomContainer domContainer, IElementCollection elements)
		{
			return new FileUploadCollection(domContainer, CreateElementFinder<FileUpload>(domContainer, elements));
		}

		public static Form Form(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Form(domContainer, CreateElementFinder<Form>(domContainer, findBy, elements));
		}

		public static FormCollection Forms(DomContainer domContainer, IElementCollection elements)
		{
			return new FormCollection(domContainer, CreateElementFinder<Form>(domContainer, elements));
		}

		public static Label Label(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Label(domContainer, CreateElementFinder<Label>(domContainer, findBy, elements));
		}

		public static LabelCollection Labels(DomContainer domContainer, IElementCollection elements)
		{
			return new LabelCollection(domContainer, CreateElementFinder<Label>(domContainer, elements));
		}

		public static Link Link(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Link(domContainer, CreateElementFinder<Link>(domContainer, findBy, elements));
		}

		public static LinkCollection Links(DomContainer domContainer, IElementCollection elements)
		{
			return new LinkCollection(domContainer, CreateElementFinder<Link>(domContainer, elements));
		}

		public static Option Option(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Option(domContainer, CreateElementFinder<Option>(domContainer, findBy, elements));
		}

		public static OptionCollection Options(DomContainer domContainer, IElementCollection elements)
		{
			return new OptionCollection(domContainer, CreateElementFinder<Option>(domContainer, elements));
		}

		public static Para Para(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Para(domContainer, CreateElementFinder<Para>(domContainer, findBy, elements));
		}

		public static ParaCollection Paras(DomContainer domContainer, IElementCollection elements)
		{
			return new ParaCollection(domContainer, CreateElementFinder<Para>(domContainer, elements));
		}

		public static RadioButton RadioButton(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new RadioButton(domContainer, CreateElementFinder<RadioButton>(domContainer, findBy, elements));
		}

		public static RadioButtonCollection RadioButtons(DomContainer domContainer, IElementCollection elements)
		{
			return new RadioButtonCollection(domContainer, CreateElementFinder<RadioButton>(domContainer, elements));
		}

		public static SelectList SelectList(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new SelectList(domContainer, CreateElementFinder<SelectList>(domContainer, findBy, elements));
		}

		public static SelectListCollection SelectLists(DomContainer domContainer, IElementCollection elements)
		{
			return new SelectListCollection(domContainer, CreateElementFinder<SelectList>(domContainer, elements));
		}

		public static Table Table(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Table(domContainer, CreateElementFinder<Table>(domContainer, findBy, elements));
		}

		public static TableCollection Tables(DomContainer domContainer, IElementCollection elements)
		{
			return new TableCollection(domContainer, CreateElementFinder<Table>(domContainer, elements));
		}

		public static TableCell TableCell(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TableCell(domContainer, CreateElementFinder<TableCell>(domContainer, findBy, elements));
		}

		public static TableCell TableCell(DomContainer domContainer, string elementId, int index, IElementCollection elements)
		{
			return new TableCell(domContainer, CreateElementFinder<TableCell>(domContainer, Find.ByIndex(index).And(Find.ById(elementId)), elements));
		}

		public static TableCell TableCell(DomContainer domContainer, Regex elementId, int index, IElementCollection elements)
		{
			return new TableCell(domContainer, CreateElementFinder<TableCell>(domContainer, Find.ByIndex(index).And(Find.ById(elementId)), elements));
		}

		public static TableCellCollection TableCells(DomContainer domContainer, IElementCollection elements)
		{
			return new TableCellCollection(domContainer, CreateElementFinder<TableCell>(domContainer, elements));
		}

		public static TableRow TableRow(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return TableRow(domContainer, CreateElementFinder<TableRow>(domContainer, findBy, elements));
		}

        public static TableRow TableRow(DomContainer domContainer, ElementFinder elementFinder)
        {
            return new TableRow(domContainer, elementFinder);
        }

		public static TableRowCollection TableRows(DomContainer domContainer, IElementCollection elements)
		{
			return new TableRowCollection(domContainer, CreateElementFinder<TableRow>(domContainer, elements));
		}

		public static TableBody TableBody(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return TableBody(domContainer, CreateElementFinder<TableBody>(domContainer, findBy, elements));
		}

        public static TableBody TableBody(DomContainer domContainer, ElementFinder elementFinder)
        {
            return new TableBody(domContainer, elementFinder);
        }

		public static TableBodyCollection TableBodies(DomContainer domContainer, IElementCollection elements)
		{
			return new TableBodyCollection(domContainer, CreateElementFinder<TableBody>(domContainer, elements));
		}

		public static TextField TextField(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new TextField(domContainer, CreateElementFinder<TextField>(domContainer, findBy, elements));
		}

		public static TextFieldCollection TextFields(DomContainer domContainer, IElementCollection elements)
		{
			return new TextFieldCollection(domContainer, CreateElementFinder<TextField>(domContainer, elements));
		}

		public static Span Span(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Span(domContainer, CreateElementFinder<Span>(domContainer, findBy, elements));
		}

		public static SpanCollection Spans(DomContainer domContainer, IElementCollection elements)
		{
			return new SpanCollection(domContainer, CreateElementFinder<Span>(domContainer, elements));
		}

		public static Div Div(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Div(domContainer, CreateElementFinder<Div>(domContainer, findBy, elements));
		}

		public static DivCollection Divs(DomContainer domContainer, IElementCollection elements)
		{
			return new DivCollection(domContainer, CreateElementFinder<Div>(domContainer, elements));
		}

		public static Image Image(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
		{
			return new Image(domContainer, CreateElementFinder<Image>(domContainer, findBy, elements));
		}

		public static ImageCollection Images(DomContainer domContainer, IElementCollection elements)
		{
			return new ImageCollection(domContainer, CreateElementFinder<Image>(domContainer, elements));
		}

        private static ElementFinder CreateElementFinder<TElement>(DomContainer domContainer, BaseConstraint findBy, IElementCollection elements)
        {
            return domContainer.NativeBrowser.CreateElementFinder(ElementFactory.GetElementTags<TElement>(), findBy, elements);
        }

        private static ElementFinder CreateElementFinder<TElement>(DomContainer domContainer, IElementCollection elements)
        {
            return CreateElementFinder<TElement>(domContainer, null, elements);
        }
	}
}
