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

using System;
using System.Text.RegularExpressions;
using mshtml;

using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// Summary description for ElementsContainer.
	/// </summary>
	public class ElementsContainer : Element, IElementsContainer
	{
	  public ElementsContainer(DomContainer ie, object element): base(ie, element) 
		{}
	  
	  public ElementsContainer(DomContainer ie, ElementFinder finder): base(ie, finder) 
		{}

    #region IElementsContainer

    public Button Button(string elementId)
    {
      return Button(Find.ById(elementId));
    }

	  public Button Button(Regex elementId)
	  {
      return Button(Find.ById(elementId));
    }

	  public Button Button(Attribute findBy)
    {
      return ElementsSupport.Button(DomContainer, findBy, elementCollection);
    }
  
    public ButtonCollection Buttons
    {
      get { return ElementsSupport.Buttons(DomContainer, elementCollection); }
    }

    public CheckBox CheckBox(string elementId)
    {
      return CheckBox(Find.ById(elementId));
    }

	  public CheckBox CheckBox(Regex elementId)
	  {
      return CheckBox(Find.ById(elementId));
    }

	  public CheckBox CheckBox(Attribute findBy)
    {
      return ElementsSupport.CheckBox(DomContainer, findBy, elementCollection);
    }

    public CheckBoxCollection CheckBoxes
    {
      get { return ElementsSupport.CheckBoxes(DomContainer, elementCollection); }
    }

    public Element Element(string elementId)
    {
      return Element(Find.ById(elementId));
    }

	  public Element Element(Regex elementId)
	  {
      return Element(Find.ById(elementId));
    }

	  public Element Element(Attribute findBy)
    {
      return ElementsSupport.Element(DomContainer, findBy, elementCollection);
    }

    public ElementCollection Elements
    {
      get { return ElementsSupport.Elements(DomContainer, elementCollection); }
    }

    public FileUpload FileUpload(string elementId)
    {
      return FileUpload(Find.ById(elementId));
    }

	  public FileUpload FileUpload(Regex elementId)
	  {
      return FileUpload(Find.ById(elementId));
    }

	  public FileUpload FileUpload(Attribute findBy)
    {
      return ElementsSupport.FileUpload(DomContainer, findBy, elementCollection);
    }

    public FileUploadCollection FileUploads
    {
      get { return ElementsSupport.FileUploads(DomContainer, elementCollection); }
    }
	  
    public Form Form(string elementId)
    {
      return Form(Find.ById(elementId));
    }

	  public Form Form(Regex elementId)
	  {
      return Form(Find.ById(elementId));
    }

	  public Form Form(Attribute findBy)
    {
      return ElementsSupport.Form(DomContainer, findBy, elementCollection);
    }

	  public FormCollection Forms
	  {
	    get { return ElementsSupport.Forms(DomContainer, elementCollection); }
	  }

	  public Label Label(string elementId)
	  {
      return Label(Find.ById(elementId));
    }

	  public Label Label(Regex elementId)
	  {
      return Label(Find.ById(elementId));
    }

	  public Label Label(Attribute findBy)
	  {
      return ElementsSupport.Label(DomContainer, findBy, elementCollection);
    }

	  public LabelCollection Labels
	  {
      get { return ElementsSupport.Labels(DomContainer, elementCollection); }
    }

	  public Link Link(string elementId)
    {
      return Link(Find.ById(elementId));
    }

	  public Link Link(Regex elementId)
	  {
      return Link(Find.ById(elementId));
    }

	  public Link Link(Attribute findBy)
    {
      return ElementsSupport.Link(DomContainer, findBy, elementCollection);
    }

    public LinkCollection Links
    {
      get { return ElementsSupport.Links(DomContainer, elementCollection); }
    }

	  public Para Para(string elementId)
	  {
	    return Para(Find.ById(elementId));
	  }

	  public Para Para(Regex elementId)
	  {
      return Para(Find.ById(elementId));
    }

	  public Para Para(Attribute findBy)
	  {
	    return ElementsSupport.Para(DomContainer, findBy, elementCollection);
	  }

	  public ParaCollection Paras
	  {
	    get { return ElementsSupport.Paras(DomContainer, elementCollection); }
	  }

	  public RadioButton RadioButton(string elementId)
	  {
	    return RadioButton(Find.ById(elementId));
	  }

	  public RadioButton RadioButton(Regex elementId)
	  {
      return RadioButton(Find.ById(elementId));
    }

	  public RadioButton RadioButton(Attribute findBy)
	  {
      return ElementsSupport.RadioButton(DomContainer, findBy, elementCollection);
    }

	  public RadioButtonCollection RadioButtons
	  {
      get { return ElementsSupport.RadioButtons(DomContainer, elementCollection); }
    }

	  public SelectList SelectList(string elementId)
    {
      return SelectList(Find.ById(elementId));
    }

	  public SelectList SelectList(Regex elementId)
	  {
      return SelectList(Find.ById(elementId));
    }

	  public SelectList SelectList(Attribute findBy)
    {
      return ElementsSupport.SelectList(DomContainer, findBy, elementCollection);
    }

    public SelectListCollection SelectLists
    {
      get { return ElementsSupport.SelectLists(DomContainer, elementCollection); }
    }

    public Table Table(string elementId)
    {
      return Table(Find.ById(elementId));
    }

	  public Table Table(Regex elementId)
	  {
      return Table(Find.ById(elementId));
    }

	  public Table Table(Attribute findBy)
    {
      return ElementsSupport.Table(DomContainer, findBy, elementCollection);
    }

    public TableCollection Tables
    {
      get { return ElementsSupport.Tables(DomContainer, elementCollection); }
    }

    //    public TableSectionCollection TableSections
    //    {
    //      get { return SubElementsSupport.TableSections(Ie, elementCollection); }
    //    }

    public TableCell TableCell(string elementId)
    {
      return TableCell(Find.ById(elementId));
    }

	  public TableCell TableCell(Regex elementId)
	  {
      return TableCell(Find.ById(elementId));
    }

	  public TableCell TableCell(Attribute findBy)
    {
      return ElementsSupport.TableCell(DomContainer, findBy, elementCollection);
    }

    /// <summary>
    /// Finds a TableCell by the n-th occurrence of an id. 
    /// Occurrence counting is zero based.
    /// </summary>  
    /// <example>
    /// This example will get Text of the third(!) occurrence on the page of a
    /// TableCell element with "tablecellid" as it's id value. 
    /// <code>ie.TableCell(new IdAndOccurrence("tablecellid", 2)).Text</code>
    /// </example>
    public TableCell TableCell(string elementId, int occurrence)
    {
      return ElementsSupport.TableCell(DomContainer, elementId, occurrence, elementCollection);
    }

	  public TableCell TableCell(Regex elementId, int occurrence)
	  {
      return ElementsSupport.TableCell(DomContainer, elementId, occurrence, elementCollection);
    }

	  public TableCellCollection TableCells
    {
      get { return ElementsSupport.TableCells(DomContainer, elementCollection); }
    }

    public TableRow TableRow(string elementId)
    {
      return TableRow(Find.ById(elementId));
    }

	  public TableRow TableRow(Regex elementId)
	  {
      return TableRow(Find.ById(elementId));
    }

	  public TableRow TableRow(Attribute findBy)
    {
      return ElementsSupport.TableRow(DomContainer, findBy, elementCollection);
    }

    public virtual TableRowCollection TableRows
    {
      get { return ElementsSupport.TableRows(DomContainer,elementCollection); }
    }

    public TextField TextField(string elementId)
    {
      return TextField(Find.ById(elementId));
    }

	  public TextField TextField(Regex elementId)
	  {
      return TextField(Find.ById(elementId));
    }

	  public TextField TextField(Attribute findBy)
    {
      return ElementsSupport.TextField(DomContainer, findBy, elementCollection);
    }

    public TextFieldCollection TextFields
    {
      get { return ElementsSupport.TextFields(DomContainer, elementCollection); }
    }

	  public Span Span(string elementId)
	  {
	    return Span(Find.ById(elementId));
	  }

	  public Span Span(Regex elementId)
	  {
      return Span(Find.ById(elementId));
    }

	  public Span Span(Attribute findBy)
	  {
      return ElementsSupport.Span(DomContainer, findBy, elementCollection);
    }

	  public SpanCollection Spans
    {
      get { return ElementsSupport.Spans(DomContainer, elementCollection); }
    }

	  public Div Div(string elementId)
	  {
	    return Div(Find.ById(elementId));
	  }

	  public Div Div(Regex elementId)
	  {
      return Div(Find.ById(elementId));
    }

	  public Div Div(Attribute findBy)
	  {
      return ElementsSupport.Div(DomContainer, findBy, elementCollection);
    }

	  public DivCollection Divs
    {
      get { return ElementsSupport.Divs(DomContainer, elementCollection); }
    }

	  public Image Image(string elementId)
	  {
	    return Image(Find.ById(elementId));
	  }

	  public Image Image(Regex elementId)
	  {
      return Image(Find.ById(elementId));
    }

	  public Image Image(Attribute findBy)
	  {
      return ElementsSupport.Image(DomContainer, findBy, elementCollection);
    }

	  public ImageCollection Images
    {
      get { return ElementsSupport.Images(DomContainer, elementCollection); }
    }
    #endregion

    private IHTMLElementCollection elementCollection
    {
      get
      {
        return (IHTMLElementCollection)((IHTMLElement)HTMLElement).all;
      }
    }
	}
}
