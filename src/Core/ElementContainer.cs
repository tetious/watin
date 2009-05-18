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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Native;

namespace WatiN.Core
{
    /// <summary>
	/// Represents an element that can contain other elements.
	/// </summary>
    public class ElementContainer<TElement> : Element<TElement>, IElementContainer
        where TElement : Element
	{
		public ElementContainer(DomContainer domContainer, INativeElement nativeElement) : base(domContainer, nativeElement) {}

        public ElementContainer(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

        private NativeElementCollectionAdapter All
        {
            get
            {
                return new NativeElementCollectionAdapter(DomContainer,
                                                          CreateNativeElementCollectionFactory(
                                                              nativeElement => nativeElement.AllDescendants));
            }
        }

        #region IElementsContainer

        public Area Area(string elementId)
        {
            return All.Area(elementId);
        }

        public Area Area(Regex elementId)
        {
            return All.Area(elementId);
        }

        public Area Area(Constraint findBy)
        {
            return All.Area(findBy);
        }

        public Area Area(Predicate<Area> predicate)
        {
            return All.Area(predicate);
        }

        public AreaCollection Areas
        {
            get { return All.Areas; }
        }

        public Button Button(string elementId)
        {
            return All.Button(elementId);
        }

        public Button Button(Regex elementId)
        {
            return All.Button(elementId);
        }

        public Button Button(Predicate<Button> predicate)
        {
            return All.Button(predicate);
        }

        public Button Button(Constraint findBy)
        {
            return All.Button(findBy);
        }

        public ButtonCollection Buttons
        {
            get { return All.Buttons; }
        }

        public CheckBox CheckBox(string elementId)
        {
            return All.CheckBox(elementId);
        }

        public CheckBox CheckBox(Regex elementId)
        {
            return All.CheckBox(elementId);
        }

        public CheckBox CheckBox(Predicate<CheckBox> predicate)
        {
            return All.CheckBox(predicate);
        }

        public CheckBox CheckBox(Constraint findBy)
        {
            return All.CheckBox(findBy);
        }

        public CheckBoxCollection CheckBoxes
        {
            get { return All.CheckBoxes; }
        }

        public Element Element(string elementId)
        {
            return All.Element(elementId);
        }

        public Element Element(Regex elementId)
        {
            return All.Element(elementId);
        }

        public Element Element(Constraint findBy)
        {
            return All.Element(findBy);
        }

        public Element Element(Predicate<Element> predicate)
        {
            return All.Element(predicate);
        }

        public ElementCollection Elements
        {
            get { return All.Elements; }
        }

        public Element ElementWithTag(string tagName, Constraint findBy, params string[] inputTypes)
        {
            return All.ElementWithTag(tagName, findBy, inputTypes);
        }

        public ElementCollection ElementsWithTag(string tagName, params string[] inputTypes)
        {
            return All.ElementsWithTag(tagName, inputTypes);
        }

        public ElementCollection ElementsWithTag(IList<ElementTag> elementTags)
        {
            return All.ElementsWithTag(elementTags);
        }

        public FileUpload FileUpload(string elementId)
        {
            return All.FileUpload(elementId);
        }

        public FileUpload FileUpload(Regex elementId)
        {
            return All.FileUpload(elementId);
        }

        public FileUpload FileUpload(Constraint findBy)
        {
            return All.FileUpload(findBy);
        }

        public FileUpload FileUpload(Predicate<FileUpload> predicate)
        {
            return All.FileUpload(predicate);
        }

        public FileUploadCollection FileUploads
        {
            get { return All.FileUploads; }
        }

        public Form Form(string elementId)
        {
            return All.Form(elementId);
        }

        public Form Form(Regex elementId)
        {
            return All.Form(elementId);
        }

        public Form Form(Constraint findBy)
        {
            return All.Form(findBy);
        }

        public Form Form(Predicate<Form> predicate)
        {
            return All.Form(predicate);
        }

        public FormCollection Forms
        {
            get { return All.Forms; }
        }

        public Label Label(string elementId)
        {
            return All.Label(elementId);
        }

        public Label Label(Regex elementId)
        {
            return All.Label(elementId);
        }

        public Label Label(Constraint findBy)
        {
            return All.Label(findBy);
        }

        public Label Label(Predicate<Label> predicate)
        {
            return All.Label(predicate);
        }

        public LabelCollection Labels
        {
            get { return All.Labels; }
        }

        public Link Link(string elementId)
        {
            return All.Link(elementId);
        }

        public Link Link(Regex elementId)
        {
            return All.Link(elementId);
        }

        public Link Link(Constraint findBy)
        {
            return All.Link(findBy);
        }

        public Link Link(Predicate<Link> predicate)
        {
            return All.Link(predicate);
        }

        public LinkCollection Links
        {
            get { return All.Links; }
        }

        public Para Para(string elementId)
        {
            return All.Para(elementId);
        }

        public Para Para(Regex elementId)
        {
            return All.Para(elementId);
        }

        public Para Para(Constraint findBy)
        {
            return All.Para(findBy);
        }

        public Para Para(Predicate<Para> predicate)
        {
            return All.Para(predicate);
        }

        public ParaCollection Paras
        {
            get { return All.Paras; }
        }

        public RadioButton RadioButton(string elementId)
        {
            return All.RadioButton(elementId);
        }

        public RadioButton RadioButton(Regex elementId)
        {
            return All.RadioButton(elementId);
        }

        public RadioButton RadioButton(Constraint findBy)
        {
            return All.RadioButton(findBy);
        }

        public RadioButton RadioButton(Predicate<RadioButton> predicate)
        {
            return All.RadioButton(predicate);
        }

        public RadioButtonCollection RadioButtons
        {
            get { return All.RadioButtons; }
        }

        public SelectList SelectList(string elementId)
        {
            return All.SelectList(elementId);
        }

        public SelectList SelectList(Regex elementId)
        {
            return All.SelectList(elementId);
        }

        public SelectList SelectList(Constraint findBy)
        {
            return All.SelectList(findBy);
        }

        public SelectList SelectList(Predicate<SelectList> predicate)
        {
            return All.SelectList(predicate);
        }

        public SelectListCollection SelectLists
        {
            get { return All.SelectLists; }
        }

        public Table Table(string elementId)
        {
            return All.Table(elementId);
        }

        public Table Table(Regex elementId)
        {
            return All.Table(elementId);
        }

        public Table Table(Constraint findBy)
        {
            return All.Table(findBy);
        }

        public Table Table(Predicate<Table> predicate)
        {
            return All.Table(predicate);
        }

        public TableCollection Tables
        {
            get { return All.Tables; }
        }

        public TableBody TableBody(string elementId)
        {
            return All.TableBody(elementId);
        }

        public TableBody TableBody(Regex elementId)
        {
            return All.TableBody(elementId);
        }

        public TableBody TableBody(Constraint findBy)
        {
            return All.TableBody(findBy);
        }

        public TableBody TableBody(Predicate<TableBody> predicate)
        {
            return All.TableBody(predicate);
        }

        public TableBodyCollection TableBodies
        {
            get { return All.TableBodies; }
        }

        public TableCell TableCell(string elementId)
        {
            return All.TableCell(elementId);
        }

        [Obsolete("Use TableCell(Find.By(elementId) & Find.ByIndex(index)) instead, or possibly OwnTableCell(...).")]
        public TableCell TableCell(string elementId, int index)
        {
            return TableCell(Find.ById(elementId) & Find.ByIndex(index));
        }

        [Obsolete("Use TableCell(Find.By(elementId) & Find.ByIndex(index)) instead, or possibly OwnTableCell(...).")]
        public TableCell TableCell(Regex elementId, int index)
        {
            return TableCell(Find.ById(elementId) & Find.ByIndex(index));
        }

        public TableCell TableCell(Regex elementId)
        {
            return All.TableCell(elementId);
        }

        public TableCell TableCell(Constraint findBy)
        {
            return All.TableCell(findBy);
        }

        public TableCell TableCell(Predicate<TableCell> predicate)
        {
            return All.TableCell(predicate);
        }

        public TableCellCollection TableCells
        {
            get { return All.TableCells; }
        }

        public TableRow TableRow(string elementId)
        {
            return All.TableRow(elementId);
        }

        public TableRow TableRow(Regex elementId)
        {
            return All.TableRow(elementId);
        }

        public TableRow TableRow(Constraint findBy)
        {
            return All.TableRow(findBy);
        }

        public TableRow TableRow(Predicate<TableRow> predicate)
        {
            return All.TableRow(predicate);
        }

        public TableRowCollection TableRows
        {
            get { return All.TableRows; }
        }

        public TextField TextField(string elementId)
        {
            return All.TextField(elementId);
        }

        public TextField TextField(Regex elementId)
        {
            return All.TextField(elementId);
        }

        public TextField TextField(Constraint findBy)
        {
            return All.TextField(findBy);
        }

        public TextField TextField(Predicate<TextField> predicate)
        {
            return All.TextField(predicate);
        }

        public TextFieldCollection TextFields
        {
            get { return All.TextFields; }
        }

        public Span Span(string elementId)
        {
            return All.Span(elementId);
        }

        public Span Span(Regex elementId)
        {
            return All.Span(elementId);
        }

        public Span Span(Constraint findBy)
        {
            return All.Span(findBy);
        }

        public Span Span(Predicate<Span> predicate)
        {
            return All.Span(predicate);
        }

        public SpanCollection Spans
        {
            get { return All.Spans; }
        }

        public Div Div(string elementId)
        {
            return All.Div(elementId);
        }

        public Div Div(Regex elementId)
        {
            return All.Div(elementId);
        }

        public Div Div(Constraint findBy)
        {
            return All.Div(findBy);
        }

        public Div Div(Predicate<Div> predicate)
        {
            return All.Div(predicate);
        }

        public DivCollection Divs
        {
            get { return All.Divs; }
        }

        public Image Image(string elementId)
        {
            return All.Image(elementId);
        }

        public Image Image(Regex elementId)
        {
            return All.Image(elementId);
        }

        public Image Image(Constraint findBy)
        {
            return All.Image(findBy);
        }

        public Image Image(Predicate<Image> predicate)
        {
            return All.Image(predicate);
        }

        public ImageCollection Images
        {
            get { return All.Images; }
        }

        public TChildElement ElementOfType<TChildElement>(string elementId) where TChildElement : Element
        {
            return All.ElementOfType<TChildElement>(elementId);
        }

        public TChildElement ElementOfType<TChildElement>(Regex elementId) where TChildElement : Element
        {
            return All.ElementOfType<TChildElement>(elementId);
        }

        public TChildElement ElementOfType<TChildElement>(Constraint findBy) where TChildElement : Element
        {
            return All.ElementOfType<TChildElement>(findBy);
        }

        public TChildElement ElementOfType<TChildElement>(Predicate<TChildElement> predicate) where TChildElement : Element
        {
            return All.ElementOfType(predicate);
        }

        public ElementCollection<TChildElement> ElementsOfType<TChildElement>() where TChildElement : Element
        {
            return All.ElementsOfType<TChildElement>();
        }

        public TControl Control<TControl>() where TControl : Control, new()
        {
            return All.Control<TControl>();
        }

        public TControl Control<TControl>(string elementId) where TControl : Control, new()
        {
            return All.Control<TControl>(elementId);
        }

        public TControl Control<TControl>(Regex elementId) where TControl : Control, new()
        {
            return All.Control<TControl>(elementId);
        }

        public TControl Control<TControl>(Constraint findBy) where TControl : Control, new()
        {
            return All.Control<TControl>(findBy);
        }

        public TControl Control<TControl>(Predicate<TControl> predicate) where TControl : Control, new()
        {
            return All.Control(predicate);
        }

        public ControlCollection<TControl> Controls<TControl>() where TControl : Control, new()
        {
            return All.Controls<TControl>();
        }

        #endregion
	}
}
