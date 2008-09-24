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

using System;
using System.Collections;
using System.Text.RegularExpressions;
using mshtml;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
    /// <summary>
	/// Summary description for ElementsContainer.
	/// </summary>
#if NET11
    public class ElementsContainer : Element, IElementsContainer, IElementCollection
#else
    public class ElementsContainer<E> : Element<E>, IElementsContainer, IElementCollection where E : Element
#endif
	{
        [Obsolete("Use the constructor accepting INativeElement instead")]
        public ElementsContainer(DomContainer domContainer, object element) : this(domContainer, domContainer.NativeBrowser.CreateElement(element)) { }
		
		public ElementsContainer(DomContainer domContainer, INativeElement nativeElement) : base(domContainer, nativeElement) {}

		public ElementsContainer(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		public ElementsContainer(Element element, ArrayList elementTags) : base(element, elementTags) {}

		#region IElementsContainer


        public Area Area(string elementId)
        {
            return Area(Find.ByDefault(elementId));
        }

        public Area Area(Regex elementId)
        {
            return Area(Find.ByDefault(elementId));
        }

        public Area Area(BaseConstraint findBy)
        {
            return ElementsSupport.Area(DomContainer, findBy, this);
        }

#if !NET11
        public Area Area(Predicate<Area> predicate)
        {
            return Area(Find.ByElement(predicate));
        }
#endif

        public AreaCollection Areas
        {
            get { return ElementsSupport.Areas(DomContainer, this); }
        }

        public Button Button(string elementId)
        {
            return Button(Find.ByDefault(elementId));
        }

        public Button Button(Regex elementId)
        {
            return Button(Find.ByDefault(elementId));
        }

        public Button Button(BaseConstraint findBy)
        {
            return ElementsSupport.Button(DomContainer, findBy, this);
        }

#if !NET11
        public Button Button(Predicate<Button> predicate)
        {
            return Button(Find.ByElement(predicate));
        }
#endif
        public ButtonCollection Buttons
        {
            get { return ElementsSupport.Buttons(DomContainer, this); }
        }

        public CheckBox CheckBox(string elementId)
        {
            return CheckBox(Find.ByDefault(elementId));
        }

        public CheckBox CheckBox(Regex elementId)
        {
            return CheckBox(Find.ByDefault(elementId));
        }

        public CheckBox CheckBox(BaseConstraint findBy)
        {
            return ElementsSupport.CheckBox(DomContainer, findBy, this);
        }

#if !NET11
        public CheckBox CheckBox(Predicate<CheckBox> predicate)
        {
            return CheckBox(Find.ByElement(predicate));
        }
#endif

        public CheckBoxCollection CheckBoxes
        {
            get { return ElementsSupport.CheckBoxes(DomContainer, this); }
        }

        public Element Element(string elementId)
        {
            return Element(Find.ByDefault(elementId));
        }

        public Element Element(Regex elementId)
        {
            return Element(Find.ByDefault(elementId));
        }

        public Element Element(BaseConstraint findBy)
        {
            return ElementsSupport.Element(DomContainer, findBy, this);
        }


#if !NET11
        public Element Element(Predicate<Element> predicate)
        {
            return Element(Find.ByElement(predicate));
        }
#endif

        public Element Element(string tagname, BaseConstraint findBy, params string[] inputtypes)
        {
            return ElementsSupport.Element(DomContainer, tagname, findBy, this, inputtypes);
        }

        public ElementCollection Elements
        {
            get { return ElementsSupport.Elements(DomContainer, this); }
        }

        public FileUpload FileUpload(string elementId)
        {
            return FileUpload(Find.ByDefault(elementId));
        }

        public FileUpload FileUpload(Regex elementId)
        {
            return FileUpload(Find.ByDefault(elementId));
        }

        public FileUpload FileUpload(BaseConstraint findBy)
        {
            return ElementsSupport.FileUpload(DomContainer, findBy, this);
        }

#if !NET11
        public FileUpload FileUpload(Predicate<FileUpload> predicate)
        {
            return FileUpload(Find.ByElement(predicate));
        }
#endif

        public FileUploadCollection FileUploads
        {
            get { return ElementsSupport.FileUploads(DomContainer, this); }
        }

        public Form Form(string elementId)
        {
            return Form(Find.ByDefault(elementId));
        }

        public Form Form(Regex elementId)
        {
            return Form(Find.ByDefault(elementId));
        }

        public Form Form(BaseConstraint findBy)
        {
            return ElementsSupport.Form(DomContainer, findBy, this);
        }

#if !NET11
        public Form Form(Predicate<Form> predicate)
        {
            return Form(Find.ByElement(predicate));
        }
#endif

        public FormCollection Forms
        {
            get { return ElementsSupport.Forms(DomContainer, this); }
        }

        public Label Label(string elementId)
        {
            return Label(Find.ByDefault(elementId));
        }

        public Label Label(Regex elementId)
        {
            return Label(Find.ByDefault(elementId));
        }

        public Label Label(BaseConstraint findBy)
        {
            return ElementsSupport.Label(DomContainer, findBy, this);
        }

#if !NET11
        public Label Label(Predicate<Label> predicate)
        {
            return Label(Find.ByElement(predicate));
        }
#endif

        public LabelCollection Labels
        {
            get { return ElementsSupport.Labels(DomContainer, this); }
        }

        public Link Link(string elementId)
        {
            return Link(Find.ByDefault(elementId));
        }

        public Link Link(Regex elementId)
        {
            return Link(Find.ByDefault(elementId));
        }

        public Link Link(BaseConstraint findBy)
        {
            return ElementsSupport.Link(DomContainer, findBy, this);
        }

#if !NET11
        public Link Link(Predicate<Link> predicate)
        {
            return Link(Find.ByElement(predicate));
        }
#endif

        public LinkCollection Links
        {
            get { return ElementsSupport.Links(DomContainer, this); }
        }

        public Para Para(string elementId)
        {
            return Para(Find.ByDefault(elementId));
        }

        public Para Para(Regex elementId)
        {
            return Para(Find.ByDefault(elementId));
        }

        public Para Para(BaseConstraint findBy)
        {
            return ElementsSupport.Para(DomContainer, findBy, this);
        }

#if !NET11
        public Para Para(Predicate<Para> predicate)
        {
            return Para(Find.ByElement(predicate));
        }
#endif

        public ParaCollection Paras
        {
            get { return ElementsSupport.Paras(DomContainer, this); }
        }

        public RadioButton RadioButton(string elementId)
        {
            return RadioButton(Find.ByDefault(elementId));
        }

        public RadioButton RadioButton(Regex elementId)
        {
            return RadioButton(Find.ByDefault(elementId));
        }

        public RadioButton RadioButton(BaseConstraint findBy)
        {
            return ElementsSupport.RadioButton(DomContainer, findBy, this);
        }

#if !NET11
        public RadioButton RadioButton(Predicate<RadioButton> predicate)
        {
            return RadioButton(Find.ByElement(predicate));
        }
#endif

        public RadioButtonCollection RadioButtons
        {
            get { return ElementsSupport.RadioButtons(DomContainer, this); }
        }

        public SelectList SelectList(string elementId)
        {
            return SelectList(Find.ByDefault(elementId));
        }

        public SelectList SelectList(Regex elementId)
        {
            return SelectList(Find.ByDefault(elementId));
        }

        public SelectList SelectList(BaseConstraint findBy)
        {
            return ElementsSupport.SelectList(DomContainer, findBy, this);
        }

#if !NET11
        public SelectList SelectList(Predicate<SelectList> predicate)
        {
            return SelectList(Find.ByElement(predicate));
        }
#endif

        public SelectListCollection SelectLists
        {
            get { return ElementsSupport.SelectLists(DomContainer, this); }
        }

        public Table Table(string elementId)
        {
            return Table(Find.ByDefault(elementId));
        }

        public Table Table(Regex elementId)
        {
            return Table(Find.ByDefault(elementId));
        }

        public Table Table(BaseConstraint findBy)
        {
            return ElementsSupport.Table(DomContainer, findBy, this);
        }

#if !NET11
        public Table Table(Predicate<Table> predicate)
        {
            return Table(Find.ByElement(predicate));
        }
#endif

        public TableCollection Tables
        {
            get { return ElementsSupport.Tables(DomContainer, this); }
        }

        public TableBody TableBody(string elementId)
        {
            return TableBody(Find.ByDefault(elementId));
        }

        public TableBody TableBody(Regex elementId)
        {
            return TableBody(Find.ByDefault(elementId));
        }

        public virtual TableBody TableBody(BaseConstraint findBy)
        {
            return ElementsSupport.TableBody(DomContainer, findBy, this);
        }

#if !NET11
        public virtual TableBody TableBody(Predicate<TableBody> predicate)
        {
            return TableBody(Find.ByElement(predicate));
        }
#endif

        public virtual TableBodyCollection TableBodies
        {
            get { return ElementsSupport.TableBodies(DomContainer, this); }
        }

        public TableCell TableCell(string elementId)
        {
            return TableCell(Find.ByDefault(elementId));
        }

        public TableCell TableCell(Regex elementId)
        {
            return TableCell(Find.ByDefault(elementId));
        }

        public TableCell TableCell(BaseConstraint findBy)
        {
            return ElementsSupport.TableCell(DomContainer, findBy, this);
        }

#if !NET11
        public TableCell TableCell(Predicate<TableCell> predicate)
        {
            return TableCell(predicate);
        }
#endif

		/// <summary>
		/// Finds a TableCell by the n-th index of an id. 
		/// index counting is zero based.
		/// </summary>  
		/// <example>
		/// This example will get the Text of the third(!) tablecell 
		/// with "tablecellid" as it's id value. 
		/// <code>ie.TableCell("tablecellid", 2).Text</code>
		/// </example>
        public TableCell TableCell(string elementId, int index)
        {
            return ElementsSupport.TableCell(DomContainer, elementId, index, this);
        }

        public TableCell TableCell(Regex elementId, int index)
        {
            return ElementsSupport.TableCell(DomContainer, elementId, index, this);
        }

        public TableCellCollection TableCells
        {
            get { return ElementsSupport.TableCells(DomContainer, this); }
        }

        public TableRow TableRow(string elementId)
        {
            return TableRow(Find.ByDefault(elementId));
        }

        public TableRow TableRow(Regex elementId)
        {
            return TableRow(Find.ByDefault(elementId));
        }

        public virtual TableRow TableRow(BaseConstraint findBy)
        {
            return ElementsSupport.TableRow(DomContainer, findBy, this);
        }

#if !NET11
        public virtual TableRow TableRow(Predicate<TableRow> predicate)
        {
            return TableRow(Find.ByElement(predicate));
        }
#endif

        public virtual TableRowCollection TableRows
        {
            get { return ElementsSupport.TableRows(DomContainer, this); }
        }

        public TextField TextField(string elementId)
        {
            return TextField(Find.ByDefault(elementId));
        }

        public TextField TextField(Regex elementId)
        {
            return TextField(Find.ByDefault(elementId));
        }

        public TextField TextField(BaseConstraint findBy)
        {
            return ElementsSupport.TextField(DomContainer, findBy, this);
        }

#if !NET11
        public TextField TextField(Predicate<TextField> predicate)
        {
            return TextField(Find.ByElement(predicate));
        }
#endif
        public TextFieldCollection TextFields
        {
            get { return ElementsSupport.TextFields(DomContainer, this); }
        }

        public Span Span(string elementId)
        {
            return Span(Find.ByDefault(elementId));
        }

        public Span Span(Regex elementId)
        {
            return Span(Find.ByDefault(elementId));
        }

        public Span Span(BaseConstraint findBy)
        {
            return ElementsSupport.Span(DomContainer, findBy, this);
        }

#if !NET11
        public Span Span(Predicate<Span> predicate)
        {
            return Span(Find.ByElement(predicate));
        }
#endif

        public SpanCollection Spans
        {
            get { return ElementsSupport.Spans(DomContainer, this); }
        }

        public Div Div(string elementId)
        {
            return Div(Find.ByDefault(elementId));
        }

        public Div Div(Regex elementId)
        {
            return Div(Find.ByDefault(elementId));
        }

        public Div Div(BaseConstraint findBy)
        {
            return ElementsSupport.Div(DomContainer, findBy, this);
        }

#if !NET11
        public Div Div(Predicate<Div> predicate)
        {
            return Div(Find.ByElement(predicate));
        }
#endif

        public DivCollection Divs
        {
            get { return ElementsSupport.Divs(DomContainer, this); }
        }

        public Image Image(string elementId)
        {
            return Image(Find.ByDefault(elementId));
        }

        public Image Image(Regex elementId)
        {
            return Image(Find.ByDefault(elementId));
        }

        public Image Image(BaseConstraint findBy)
        {
            return ElementsSupport.Image(DomContainer, findBy, this);
        }

#if !NET11
        public Image Image(Predicate<Image> predicate)
        {
            return Image(Find.ByElement(predicate));
        }
#endif

        public ImageCollection Images
        {
            get { return ElementsSupport.Images(DomContainer, this); }
        }
        #endregion

		IHTMLElementCollection IElementCollection.Elements
		{
			get
			{
				try
				{
					if (Exists)
					{
						return (IHTMLElementCollection) htmlElement.all;
					}

					return null;
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
