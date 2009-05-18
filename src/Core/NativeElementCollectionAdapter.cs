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
    /// Wraps a <see cref="DomContainer" /> and <see cref="INativeElementCollection" /> as
    /// an implementation of <see cref="IElementContainer" />.
    /// </summary>
    internal class NativeElementCollectionAdapter : IElementContainer
    {
        private readonly DomContainer domContainer;
        private readonly NativeElementFinder.NativeElementCollectionFactory nativeElementCollectionFactory;

        /// <summary>
        /// Creates a new adapter.
        /// </summary>
        /// <param name="domContainer">The DOM container</param>
        /// <param name="factory">The native element collection</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="domContainer"/> or
        /// <paramref name="factory"/> is null</exception>
        public NativeElementCollectionAdapter(DomContainer domContainer,
            NativeElementFinder.NativeElementCollectionFactory factory)
        {
            if (domContainer == null)
                throw new ArgumentNullException("domContainer");
            if (factory == null)
                throw new ArgumentNullException("factory");

            this.domContainer = domContainer;
            nativeElementCollectionFactory = factory;
        }

        #region IElementsContainer Members

        public Area Area(string elementId)
        {
            return Area(Find.ByDefault(elementId));
        }

        public Area Area(Regex elementId)
        {
            return Area(Find.ByDefault(elementId));
        }

        public Area Area(Constraint findBy)
        {
            return new Area(domContainer, CreateElementFinder<Area>(findBy));
        }

        public Area Area(Predicate<Area> predicate)
        {
            return Area(Find.ByElement(predicate));
        }

        public AreaCollection Areas
        {
            get { return new AreaCollection(domContainer, CreateElementFinder<Area>(null)); }
        }

        public Button Button(string elementId)
        {
            return Button(Find.ByDefault(elementId));
        }

        public Button Button(Regex elementId)
        {
            return Button(Find.ByDefault(elementId));
        }

        public Button Button(Predicate<Button> predicate)
        {
            return Button(Find.ByElement(predicate));
        }

        public Button Button(Constraint findBy)
        {
            return new Button(domContainer, CreateElementFinder<Button>(findBy));
        }

        public ButtonCollection Buttons
        {
            get { return new ButtonCollection(domContainer, CreateElementFinder<Button>(null)); }
        }

        public CheckBox CheckBox(string elementId)
        {
            return CheckBox(Find.ByDefault(elementId));
        }

        public CheckBox CheckBox(Regex elementId)
        {
            return CheckBox(Find.ByDefault(elementId));
        }

        public CheckBox CheckBox(Predicate<CheckBox> predicate)
        {
            return CheckBox(Find.ByElement(predicate));
        }

        public CheckBox CheckBox(Constraint findBy)
        {
            return new CheckBox(domContainer, CreateElementFinder<CheckBox>(findBy));
        }

        public CheckBoxCollection CheckBoxes
        {
            get { return new CheckBoxCollection(domContainer, CreateElementFinder<CheckBox>(null)); }
        }

        public Element Element(string elementId)
        {
            return Element(Find.ByDefault(elementId));
        }

        public Element Element(Regex elementId)
        {
            return Element(Find.ByDefault(elementId));
        }

        public Element Element(Constraint findBy)
        {
            return new ElementContainer<Element>(domContainer, CreateElementFinder<Element>(findBy));
        }

        public Element Element(Predicate<Element> predicate)
        {
            return Element(Find.ByElement(predicate));
        }

        public ElementCollection Elements
        {
            get { return new ElementCollection(domContainer, CreateElementFinder<Element>(null)); }
        }

        public Element ElementWithTag(string tagName, Constraint findBy, params string[] inputTypes)
        {
            return new ElementContainer<Element>(domContainer, CreateElementFinder(findBy, tagName, inputTypes));
        }

        public ElementCollection ElementsWithTag(string tagName, params string[] inputTypes)
        {
            return new ElementCollection(domContainer, CreateElementFinder(null, tagName, inputTypes));
        }

        public ElementCollection ElementsWithTag(IList<ElementTag> elementTags)
        {
            return new ElementCollection(domContainer, CreateElementFinder(null, elementTags));
        }

        public FileUpload FileUpload(string elementId)
        {
            return FileUpload(Find.ByDefault(elementId));
        }

        public FileUpload FileUpload(Regex elementId)
        {
            return FileUpload(Find.ByDefault(elementId));
        }

        public FileUpload FileUpload(Constraint findBy)
        {
            return new FileUpload(domContainer, CreateElementFinder<FileUpload>(findBy));
        }

        public FileUpload FileUpload(Predicate<FileUpload> predicate)
        {
            return FileUpload(Find.ByElement(predicate));
        }

        public FileUploadCollection FileUploads
        {
            get { return new FileUploadCollection(domContainer, CreateElementFinder<FileUpload>(null)); }
        }

        public Form Form(string elementId)
        {
            return Form(Find.ByDefault(elementId));
        }

        public Form Form(Regex elementId)
        {
            return Form(Find.ByDefault(elementId));
        }

        public Form Form(Constraint findBy)
        {
            return new Form(domContainer, CreateElementFinder<Form>(findBy));
        }

        public Form Form(Predicate<Form> predicate)
        {
            return Form(Find.ByElement(predicate));
        }

        public FormCollection Forms
        {
            get { return new FormCollection(domContainer, CreateElementFinder<Form>(null)); }
        }

        public Label Label(string elementId)
        {
            return Label(Find.ByDefault(elementId));
        }

        public Label Label(Regex elementId)
        {
            return Label(Find.ByDefault(elementId));
        }

        public Label Label(Constraint findBy)
        {
            return new Label(domContainer, CreateElementFinder<Label>(findBy));
        }

        public Label Label(Predicate<Label> predicate)
        {
            return Label(Find.ByElement(predicate));
        }

        public LabelCollection Labels
        {
            get { return new LabelCollection(domContainer, CreateElementFinder<Label>(null)); }
        }

        public Link Link(string elementId)
        {
            return Link(Find.ByDefault(elementId));
        }

        public Link Link(Regex elementId)
        {
            return Link(Find.ByDefault(elementId));
        }

        public Link Link(Constraint findBy)
        {
            return new Link(domContainer, CreateElementFinder<Link>(findBy));
        }

        public Link Link(Predicate<Link> predicate)
        {
            return Link(Find.ByElement(predicate));
        }

        public LinkCollection Links
        {
            get { return new LinkCollection(domContainer, CreateElementFinder<Link>(null)); }
        }

        public Para Para(string elementId)
        {
            return Para(Find.ByDefault(elementId));
        }

        public Para Para(Regex elementId)
        {
            return Para(Find.ByDefault(elementId));
        }

        public Para Para(Constraint findBy)
        {
            return new Para(domContainer, CreateElementFinder<Para>(findBy));
        }

        public Para Para(Predicate<Para> predicate)
        {
            return Para(Find.ByElement(predicate));
        }

        public ParaCollection Paras
        {
            get { return new ParaCollection(domContainer, CreateElementFinder<Para>(null)); }
        }

        public RadioButton RadioButton(string elementId)
        {
            return RadioButton(Find.ByDefault(elementId));
        }

        public RadioButton RadioButton(Regex elementId)
        {
            return RadioButton(Find.ByDefault(elementId));
        }

        public RadioButton RadioButton(Constraint findBy)
        {
            return new RadioButton(domContainer, CreateElementFinder<RadioButton>(findBy));
        }

        public RadioButton RadioButton(Predicate<RadioButton> predicate)
        {
            return RadioButton(Find.ByElement(predicate));
        }

        public RadioButtonCollection RadioButtons
        {
            get { return new RadioButtonCollection(domContainer, CreateElementFinder<RadioButton>(null)); }
        }

        public SelectList SelectList(string elementId)
        {
            return SelectList(Find.ByDefault(elementId));
        }

        public SelectList SelectList(Regex elementId)
        {
            return SelectList(Find.ByDefault(elementId));
        }

        public SelectList SelectList(Constraint findBy)
        {
            return new SelectList(domContainer, CreateElementFinder<SelectList>(findBy));
        }

        public SelectList SelectList(Predicate<SelectList> predicate)
        {
            return SelectList(Find.ByElement(predicate));
        }

        public SelectListCollection SelectLists
        {
            get { return new SelectListCollection(domContainer, CreateElementFinder<SelectList>(null)); }
        }

        public Table Table(string elementId)
        {
            return Table(Find.ByDefault(elementId));
        }

        public Table Table(Regex elementId)
        {
            return Table(Find.ByDefault(elementId));
        }

        public Table Table(Constraint findBy)
        {
            return new Table(domContainer, CreateElementFinder<Table>(findBy));
        }

        public Table Table(Predicate<Table> predicate)
        {
            return Table(Find.ByElement(predicate));
        }

        public TableCollection Tables
        {
            get { return new TableCollection(domContainer, CreateElementFinder<Table>(null)); }
        }

        public TableBody TableBody(string elementId)
        {
            return TableBody(Find.ByDefault(elementId));
        }

        public TableBody TableBody(Regex elementId)
        {
            return TableBody(Find.ByDefault(elementId));
        }

        public TableBody TableBody(Constraint findBy)
        {
            return new TableBody(domContainer, CreateElementFinder<TableBody>(findBy));
        }

        public TableBody TableBody(Predicate<TableBody> predicate)
        {
            return TableBody(Find.ByElement(predicate));
        }

        public TableBodyCollection TableBodies
        {
            get { return new TableBodyCollection(domContainer, CreateElementFinder<TableBody>(null)); }
        }

        public TableCell TableCell(string elementId)
        {
            return TableCell(Find.ByDefault(elementId));
        }

        public TableCell TableCell(Regex elementId)
        {
            return TableCell(Find.ByDefault(elementId));
        }

        public TableCell TableCell(Constraint findBy)
        {
            return new TableCell(domContainer, CreateElementFinder<TableCell>(findBy));
        }

        public TableCell TableCell(Predicate<TableCell> predicate)
        {
            return TableCell(Find.ByElement(predicate));
        }

        public TableCellCollection TableCells
        {
            get { return new TableCellCollection(domContainer, CreateElementFinder<TableCell>(null)); }
        }

        public TableRow TableRow(string elementId)
        {
            return TableRow(Find.ByDefault(elementId));
        }

        public TableRow TableRow(Regex elementId)
        {
            return TableRow(Find.ByDefault(elementId));
        }

        public TableRow TableRow(Constraint findBy)
        {
            return new TableRow(domContainer, CreateElementFinder<TableRow>(findBy));
        }

        public TableRow TableRow(Predicate<TableRow> predicate)
        {
            return TableRow(Find.ByElement(predicate));
        }

        public TableRowCollection TableRows
        {
            get { return new TableRowCollection(domContainer, CreateElementFinder<TableRow>(null)); }
        }

        public TextField TextField(string elementId)
        {
            return TextField(Find.ByDefault(elementId));
        }

        public TextField TextField(Regex elementId)
        {
            return TextField(Find.ByDefault(elementId));
        }

        public TextField TextField(Constraint findBy)
        {
            return new TextField(domContainer, CreateElementFinder<TextField>(findBy));
        }

        public TextField TextField(Predicate<TextField> predicate)
        {
            return TextField(Find.ByElement(predicate));
        }

        public TextFieldCollection TextFields
        {
            get { return new TextFieldCollection(domContainer, CreateElementFinder<TextField>(null)); }
        }

        public Span Span(string elementId)
        {
            return Span(Find.ByDefault(elementId));
        }

        public Span Span(Regex elementId)
        {
            return Span(Find.ByDefault(elementId));
        }

        public Span Span(Constraint findBy)
        {
            return new Span(domContainer, CreateElementFinder<Span>(findBy));
        }

        public Span Span(Predicate<Span> predicate)
        {
            return Span(Find.ByElement(predicate));
        }

        public SpanCollection Spans
        {
            get { return new SpanCollection(domContainer, CreateElementFinder<Span>(null)); }
        }

        public Div Div(string elementId)
        {
            return Div(Find.ByDefault(elementId));
        }

        public Div Div(Regex elementId)
        {
            return Div(Find.ByDefault(elementId));
        }

        public Div Div(Constraint findBy)
        {
            return new Div(domContainer, CreateElementFinder<Div>(findBy));
        }

        public Div Div(Predicate<Div> predicate)
        {
            return Div(Find.ByElement(predicate));
        }

        public DivCollection Divs
        {
            get { return new DivCollection(domContainer, CreateElementFinder<Div>(null)); }
        }

        public Image Image(string elementId)
        {
            return Image(Find.ByDefault(elementId));
        }

        public Image Image(Regex elementId)
        {
            return Image(Find.ByDefault(elementId));
        }

        public Image Image(Constraint findBy)
        {
            return new Image(domContainer, CreateElementFinder<Image>(findBy));
        }

        public Image Image(Predicate<Image> predicate)
        {
            return Image(Find.ByElement(predicate));
        }

        public ImageCollection Images
        {
            get { return new ImageCollection(domContainer, CreateElementFinder<Image>(null)); }
        }

        public TElement ElementOfType<TElement>(string elementId) where TElement : Element
        {
            return ElementOfType<TElement>(Find.ByDefault(elementId));
        }

        public TElement ElementOfType<TElement>(Regex elementId) where TElement : Element
        {
            return ElementOfType<TElement>(Find.ByDefault(elementId));
        }

        public TElement ElementOfType<TElement>(Constraint findBy) where TElement : Element
        {
            return ElementFactory.CreateElement<TElement>(domContainer, CreateElementFinder<TElement>(findBy));
        }

        public TElement ElementOfType<TElement>(Predicate<TElement> predicate) where TElement : Element
        {
            return ElementOfType<TElement>(Find.ByElement(predicate));
        }

        public ElementCollection<TElement> ElementsOfType<TElement>() where TElement : Element
        {
            return new ElementCollection<TElement>(domContainer, CreateElementFinder<TElement>(null));
        }

        public TControl Control<TControl>() where TControl : Control, new()
        {
            return Control<TControl>(Find.Any);
        }

        public TControl Control<TControl>(string elementId) where TControl : Control, new()
        {
            return Control<TControl>(Find.ByDefault(elementId));
        }

        public TControl Control<TControl>(Regex elementId) where TControl : Control, new()
        {
            return Control<TControl>(Find.ByDefault(elementId));
        }

        public TControl Control<TControl>(Constraint findBy) where TControl : Control, new()
        {
            return Core.Control.FindControl<TControl>(this, findBy);
        }

        public TControl Control<TControl>(Predicate<TControl> predicate) where TControl : Control, new()
        {
            return Control<TControl>(new PredicateConstraint<TControl>(predicate));
        }

        public ControlCollection<TControl> Controls<TControl>() where TControl : Control, new()
        {
            return Core.Control.FindControls<TControl>(this);
        }

        #endregion

        private NativeElementFinder CreateElementFinder<TElement>(Constraint findBy)
            where TElement : Element
        {
            return new NativeElementFinder(nativeElementCollectionFactory, domContainer, ElementFactory.GetElementTags<TElement>(), findBy);
        }

        private NativeElementFinder CreateElementFinder(Constraint findBy, string tagName, params string[] inputTypes)
        {
            var tags = ElementTag.ToElementTags(tagName, inputTypes);

            return CreateElementFinder(findBy, tags);
        }

        private NativeElementFinder CreateElementFinder(Constraint findBy, IEnumerable<ElementTag> tags)
        {
            return new NativeElementFinder(nativeElementCollectionFactory, domContainer, new List<ElementTag>(tags), findBy);
        }
    }
}