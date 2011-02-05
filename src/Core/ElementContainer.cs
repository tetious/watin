#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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

        private NativeElementCollectionAdapter AllElements
        {
            get
            {
                return new NativeElementCollectionAdapter(DomContainer,
                                                          CreateNativeElementCollectionFactory(
                                                              nativeElement => nativeElement.AllDescendants));
            }
        }

        private IElementContainer ChildElements
        {
            get
            {
                return new NativeElementCollectionAdapter(DomContainer,
                                                        CreateNativeElementCollectionFactory(
                                                            nativeElement => nativeElement.Children));
            }
        }
        
        #region IElementsContainer

        public Area Area(string elementId)
        {
            return AllElements.Area(elementId);
        }

        public Area Area(Regex elementId)
        {
            return AllElements.Area(elementId);
        }

        public Area Area(Constraint findBy)
        {
            return AllElements.Area(findBy);
        }

        public Area Area(Predicate<Area> predicate)
        {
            return AllElements.Area(predicate);
        }

        public AreaCollection Areas
        {
            get { return AllElements.Areas; }
        }

        public Button Button(string elementId)
        {
            return AllElements.Button(elementId);
        }

        public Button Button(Regex elementId)
        {
            return AllElements.Button(elementId);
        }

        public Button Button(Predicate<Button> predicate)
        {
            return AllElements.Button(predicate);
        }

        public Button Button(Constraint findBy)
        {
            return AllElements.Button(findBy);
        }

        public ButtonCollection Buttons
        {
            get { return AllElements.Buttons; }
        }

        public CheckBox CheckBox(string elementId)
        {
            return AllElements.CheckBox(elementId);
        }

        public CheckBox CheckBox(Regex elementId)
        {
            return AllElements.CheckBox(elementId);
        }

        public CheckBox CheckBox(Predicate<CheckBox> predicate)
        {
            return AllElements.CheckBox(predicate);
        }

        public CheckBox CheckBox(Constraint findBy)
        {
            return AllElements.CheckBox(findBy);
        }

        public CheckBoxCollection CheckBoxes
        {
            get { return AllElements.CheckBoxes; }
        }

        public Element Element(string elementId)
        {
            return AllElements.Element(elementId);
        }

        public Element Element(Regex elementId)
        {
            return AllElements.Element(elementId);
        }

        public Element Element(Constraint findBy)
        {
            return AllElements.Element(findBy);
        }

        public Element Element(Predicate<Element> predicate)
        {
            return AllElements.Element(predicate);
        }

        public ElementCollection Elements
        {
            get { return AllElements.Elements; }
        }

        public Element ElementWithTag(string tagName, Constraint findBy, params string[] inputTypes)
        {
            return AllElements.ElementWithTag(tagName, findBy, inputTypes);
        }

        public ElementCollection ElementsWithTag(string tagName, params string[] inputTypes)
        {
            return AllElements.ElementsWithTag(tagName, inputTypes);
        }

        public ElementCollection ElementsWithTag(IList<ElementTag> elementTags)
        {
            return AllElements.ElementsWithTag(elementTags);
        }

        public FileUpload FileUpload(string elementId)
        {
            return AllElements.FileUpload(elementId);
        }

        public FileUpload FileUpload(Regex elementId)
        {
            return AllElements.FileUpload(elementId);
        }

        public FileUpload FileUpload(Constraint findBy)
        {
            return AllElements.FileUpload(findBy);
        }

        public FileUpload FileUpload(Predicate<FileUpload> predicate)
        {
            return AllElements.FileUpload(predicate);
        }

        public FileUploadCollection FileUploads
        {
            get { return AllElements.FileUploads; }
        }

        public Form Form(string elementId)
        {
            return AllElements.Form(elementId);
        }

        public Form Form(Regex elementId)
        {
            return AllElements.Form(elementId);
        }

        public Form Form(Constraint findBy)
        {
            return AllElements.Form(findBy);
        }

        public Form Form(Predicate<Form> predicate)
        {
            return AllElements.Form(predicate);
        }

        public FormCollection Forms
        {
            get { return AllElements.Forms; }
        }

        public Label Label(string elementId)
        {
            return AllElements.Label(elementId);
        }

        public Label Label(Regex elementId)
        {
            return AllElements.Label(elementId);
        }

        public Label Label(Constraint findBy)
        {
            return AllElements.Label(findBy);
        }

        public Label Label(Predicate<Label> predicate)
        {
            return AllElements.Label(predicate);
        }

        public LabelCollection Labels
        {
            get { return AllElements.Labels; }
        }

        public Link Link(string elementId)
        {
            return AllElements.Link(elementId);
        }

        public Link Link(Regex elementId)
        {
            return AllElements.Link(elementId);
        }

        public Link Link(Constraint findBy)
        {
            return AllElements.Link(findBy);
        }

        public Link Link(Predicate<Link> predicate)
        {
            return AllElements.Link(predicate);
        }

        public LinkCollection Links
        {
            get { return AllElements.Links; }
        }

        public List List(string elementId)
        {
            return AllElements.List(elementId);
        }

        public List List(Regex elementId)
        {
            return AllElements.List(elementId);
        }

        public List List(Constraint findBy)
        {
            return AllElements.List(findBy);
        }

        public List List(Predicate<List> predicate)
        {
            return AllElements.List(predicate);
        }

        public ListCollection Lists
        {
            get { return AllElements.Lists; }
        }

        public ListItem ListItem(string elementId)
        {
            return AllElements.ListItem(elementId);
        }

        public ListItem ListItem(Regex elementId)
        {
            return AllElements.ListItem(elementId);
        }

        public ListItem ListItem(Constraint findBy)
        {
            return AllElements.ListItem(findBy);
        }

        public ListItem ListItem(Predicate<ListItem> predicate)
        {
            return AllElements.ListItem(predicate);
        }

        public ListItemCollection ListItems
        {
            get { return AllElements.ListItems; }
        }

        public Para Para(string elementId)
        {
            return AllElements.Para(elementId);
        }

        public Para Para(Regex elementId)
        {
            return AllElements.Para(elementId);
        }

        public Para Para(Constraint findBy)
        {
            return AllElements.Para(findBy);
        }

        public Para Para(Predicate<Para> predicate)
        {
            return AllElements.Para(predicate);
        }

        public ParaCollection Paras
        {
            get { return AllElements.Paras; }
        }

        public RadioButton RadioButton(string elementId)
        {
            return AllElements.RadioButton(elementId);
        }

        public RadioButton RadioButton(Regex elementId)
        {
            return AllElements.RadioButton(elementId);
        }

        public RadioButton RadioButton(Constraint findBy)
        {
            return AllElements.RadioButton(findBy);
        }

        public RadioButton RadioButton(Predicate<RadioButton> predicate)
        {
            return AllElements.RadioButton(predicate);
        }

        public RadioButtonCollection RadioButtons
        {
            get { return AllElements.RadioButtons; }
        }

        public SelectList SelectList(string elementId)
        {
            return AllElements.SelectList(elementId);
        }

        public SelectList SelectList(Regex elementId)
        {
            return AllElements.SelectList(elementId);
        }

        public SelectList SelectList(Constraint findBy)
        {
            return AllElements.SelectList(findBy);
        }

        public SelectList SelectList(Predicate<SelectList> predicate)
        {
            return AllElements.SelectList(predicate);
        }

        public SelectListCollection SelectLists
        {
            get { return AllElements.SelectLists; }
        }

        public Table Table(string elementId)
        {
            return AllElements.Table(elementId);
        }

        public Table Table(Regex elementId)
        {
            return AllElements.Table(elementId);
        }

        public Table Table(Constraint findBy)
        {
            return AllElements.Table(findBy);
        }

        public Table Table(Predicate<Table> predicate)
        {
            return AllElements.Table(predicate);
        }

        public TableCollection Tables
        {
            get { return AllElements.Tables; }
        }

        public TableBody TableBody(string elementId)
        {
            return AllElements.TableBody(elementId);
        }

        public TableBody TableBody(Regex elementId)
        {
            return AllElements.TableBody(elementId);
        }

        public TableBody TableBody(Constraint findBy)
        {
            return AllElements.TableBody(findBy);
        }

        public TableBody TableBody(Predicate<TableBody> predicate)
        {
            return AllElements.TableBody(predicate);
        }

        public TableBodyCollection TableBodies
        {
            get { return AllElements.TableBodies; }
        }

        public TableCell TableCell(string elementId)
        {
            return AllElements.TableCell(elementId);
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
            return AllElements.TableCell(elementId);
        }

        public TableCell TableCell(Constraint findBy)
        {
            return AllElements.TableCell(findBy);
        }

        public TableCell TableCell(Predicate<TableCell> predicate)
        {
            return AllElements.TableCell(predicate);
        }

        public TableCellCollection TableCells
        {
            get { return AllElements.TableCells; }
        }

        public TableRow TableRow(string elementId)
        {
            return AllElements.TableRow(elementId);
        }

        public TableRow TableRow(Regex elementId)
        {
            return AllElements.TableRow(elementId);
        }

        public TableRow TableRow(Constraint findBy)
        {
            return AllElements.TableRow(findBy);
        }

        public TableRow TableRow(Predicate<TableRow> predicate)
        {
            return AllElements.TableRow(predicate);
        }

        public TableRowCollection TableRows
        {
            get { return AllElements.TableRows; }
        }

        public TextField TextField(string elementId)
        {
            return AllElements.TextField(elementId);
        }

        public TextField TextField(Regex elementId)
        {
            return AllElements.TextField(elementId);
        }

        public TextField TextField(Constraint findBy)
        {
            return AllElements.TextField(findBy);
        }

        public TextField TextField(Predicate<TextField> predicate)
        {
            return AllElements.TextField(predicate);
        }

        public TextFieldCollection TextFields
        {
            get { return AllElements.TextFields; }
        }

        public Span Span(string elementId)
        {
            return AllElements.Span(elementId);
        }

        public Span Span(Regex elementId)
        {
            return AllElements.Span(elementId);
        }

        public Span Span(Constraint findBy)
        {
            return AllElements.Span(findBy);
        }

        public Span Span(Predicate<Span> predicate)
        {
            return AllElements.Span(predicate);
        }

        public SpanCollection Spans
        {
            get { return AllElements.Spans; }
        }

        public Div Div(string elementId)
        {
            return AllElements.Div(elementId);
        }

        public Div Div(Regex elementId)
        {
            return AllElements.Div(elementId);
        }

        public Div Div(Constraint findBy)
        {
            return AllElements.Div(findBy);
        }

        public Div Div(Predicate<Div> predicate)
        {
            return AllElements.Div(predicate);
        }

        public DivCollection Divs
        {
            get { return AllElements.Divs; }
        }

        public Image Image(string elementId)
        {
            return AllElements.Image(elementId);
        }

        public Image Image(Regex elementId)
        {
            return AllElements.Image(elementId);
        }

        public Image Image(Constraint findBy)
        {
            return AllElements.Image(findBy);
        }

        public Image Image(Predicate<Image> predicate)
        {
            return AllElements.Image(predicate);
        }

        public ImageCollection Images
        {
            get { return AllElements.Images; }
        }

        /// <inheritdoc />
        public TChildElement ElementOfType<TChildElement>(string elementId) where TChildElement : Element
        {
            return AllElements.ElementOfType<TChildElement>(elementId);
        }

        /// <inheritdoc />
        public TChildElement ElementOfType<TChildElement>(Regex elementId) where TChildElement : Element
        {
            return AllElements.ElementOfType<TChildElement>(elementId);
        }

        /// <inheritdoc />
        public TChildElement ElementOfType<TChildElement>(Constraint findBy) where TChildElement : Element
        {
            return AllElements.ElementOfType<TChildElement>(findBy);
        }

        /// <inheritdoc />
        public TChildElement ElementOfType<TChildElement>(Predicate<TChildElement> predicate) where TChildElement : Element
        {
            return AllElements.ElementOfType(predicate);
        }

        /// <inheritdoc />
        public ElementCollection<TChildElement> ElementsOfType<TChildElement>() where TChildElement : Element
        {
            return AllElements.ElementsOfType<TChildElement>();
        }

        /// <inheritdoc />
        public TControl Control<TControl>() where TControl : Control, new()
        {
            return AllElements.Control<TControl>();
        }

        /// <inheritdoc />
        public TControl Control<TControl>(string elementId) where TControl : Control, new()
        {
            return AllElements.Control<TControl>(elementId);
        }

        /// <inheritdoc />
        public TControl Control<TControl>(Regex elementId) where TControl : Control, new()
        {
            return AllElements.Control<TControl>(elementId);
        }

        /// <inheritdoc />
        public TControl Control<TControl>(Constraint findBy) where TControl : Control, new()
        {
            return AllElements.Control<TControl>(findBy);
        }

        /// <inheritdoc />
        public TControl Control<TControl>(Predicate<TControl> predicate) where TControl : Control, new()
        {
            return AllElements.Control(predicate);
        }

        /// <inheritdoc />
        public ControlCollection<TControl> Controls<TControl>() where TControl : Control, new()
        {
            return AllElements.Controls<TControl>();
        }

        /// <inheritdoc />
        public Element Child(string elementId)
        {
            return ChildElements.Element(elementId);
        }

        /// <inheritdoc />
        public Element Child(Regex elementId)
        {
            return ChildElements.Element(elementId);
        }

        /// <inheritdoc />
        public Element Child(Constraint findBy)
        {
            return ChildElements.Element(findBy);
        }

        /// <inheritdoc />
        public Element Child(Predicate<Element> predicate)
        {
            return ChildElements.Element(predicate);
        }

        /// <inheritdoc />
        public ElementCollection Children()
        {
            return ChildElements.Elements;
        }

        /// <inheritdoc />
        public TChildElement ChildOfType<TChildElement>(string elementId) where TChildElement : Element
        {
            return ChildElements.ElementOfType<TChildElement>(elementId);
        }

        /// <inheritdoc />
        public TChildElement ChildOfType<TChildElement>(Regex elementId) where TChildElement : Element
        {
            return ChildElements.ElementOfType<TChildElement>(elementId);
        }

        /// <inheritdoc />
        public TChildElement ChildOfType<TChildElement>(Constraint findBy) where TChildElement : Element
        {
            return ChildElements.ElementOfType<TChildElement>(findBy);
        }

        /// <inheritdoc />
        public TChildElement ChildOfType<TChildElement>(Predicate<TChildElement> predicate) where TChildElement : Element
        {
            return ChildElements.ElementOfType(predicate);
        }

        /// <inheritdoc />
        public ElementCollection<TChildElement> ChildrenOfType<TChildElement>() where TChildElement : Element
        {
            return ChildElements.ElementsOfType<TChildElement>();
        }

        /// <inheritdoc />
        public Element ChildWithTag(string tagName, Constraint findBy, params string[] inputTypes)
        {
            return ChildElements.ElementWithTag(tagName, findBy, inputTypes);
        }

        /// <inheritdoc />
        public ElementCollection ChildrenWithTag(string tagName, params string[] inputTypes)
        {
            return ChildElements.ElementsWithTag(tagName, inputTypes);
        }

        /// <inheritdoc />
        public ElementCollection ChildrenWithTag(IList<ElementTag> elementTags)
        {
            return ChildElements.ElementsWithTag(elementTags);
        }

        #endregion
	}
}
