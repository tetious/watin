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

using mshtml;

using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// Summary description for ElementsContainer.
	/// </summary>
	public class ElementsContainer : Element, ISubElements
	{
	  public ElementsContainer(DomContainer ie, object element): base(ie, element) 
		{}

    #region ISubElements

    public Button Button(string elementId)
    {
      return Button(Find.ById(elementId));
    }

    public Button Button(Attribute findBy)
    {
      return SubElementsSupport.Button(DomContainer, findBy, elementCollection);
    }
  
    public ButtonCollection Buttons
    {
      get { return SubElementsSupport.Buttons(DomContainer, elementCollection); }
    }

    public CheckBox CheckBox(string elementId)
    {
      return CheckBox(Find.ById(elementId));
    }

    public CheckBox CheckBox(Attribute findBy)
    {
      return SubElementsSupport.CheckBox(DomContainer, findBy, elementCollection);
    }

    public CheckBoxCollection CheckBoxes
    {
      get { return SubElementsSupport.CheckBoxes(DomContainer, elementCollection); }
    }

    public Element Element(string elementId)
    {
      return Element(Find.ById(elementId));
    }

    public Element Element(Attribute findBy)
    {
      return SubElementsSupport.Element(DomContainer, findBy, elementCollection);
    }

    public ElementCollection Elements
    {
      get { return SubElementsSupport.Elements(DomContainer, elementCollection); }
    }

    public FileUpload FileUpload(string elementId)
    {
      return FileUpload(Find.ById(elementId));
    }

    public FileUpload FileUpload(Attribute findBy)
    {
      return SubElementsSupport.FileUpload(DomContainer, findBy, elementCollection);
    }

    public FileUploadCollection FileUploads
    {
      get { return SubElementsSupport.FileUploads(DomContainer, elementCollection); }
    }
	  
    public Form Form(string elementId)
    {
      return Form(Find.ById(elementId));
    }

    public Form Form(Attribute findBy)
    {
      return SubElementsSupport.Form(DomContainer, findBy, elementCollection);
    }

	  public FormCollection Forms
	  {
	    get { return SubElementsSupport.Forms(DomContainer, elementCollection); }
	  }

	  public Label Label(string elementId)
	  {
      return Label(Find.ById(elementId));
    }

	  public Label Label(Attribute findBy)
	  {
      return SubElementsSupport.Label(DomContainer, findBy, elementCollection);
    }

	  public LabelCollection Labels
	  {
      get { return SubElementsSupport.Labels(DomContainer, elementCollection); }
    }

	  public Link Link(string elementId)
    {
      return Link(Find.ById(elementId));
    }

    public Link Link(Attribute findBy)
    {
      return SubElementsSupport.Link(DomContainer, findBy, elementCollection);
    }

    public LinkCollection Links
    {
      get { return SubElementsSupport.Links(DomContainer, elementCollection); }
    }

	  public Para Para(string elementId)
	  {
	    return Para(Find.ById(elementId));
	  }

	  public Para Para(Attribute findBy)
	  {
	    return SubElementsSupport.Para(DomContainer, findBy, elementCollection);
	  }

	  public ParaCollection Paras
	  {
	    get { return SubElementsSupport.Paras(DomContainer, elementCollection); }
	  }

	  public RadioButton RadioButton(string elementId)
	  {
	    return RadioButton(Find.ById(elementId));
	  }

	  public RadioButton RadioButton(Attribute findBy)
	  {
      return SubElementsSupport.RadioButton(DomContainer, findBy, elementCollection);
    }

	  public RadioButtonCollection RadioButtons
	  {
      get { return SubElementsSupport.RadioButtons(DomContainer, elementCollection); }
    }

	  public SelectList SelectList(string elementId)
    {
      return SelectList(Find.ById(elementId));
    }

    public SelectList SelectList(Attribute findBy)
    {
      return SubElementsSupport.SelectList(DomContainer, findBy, elementCollection);
    }

    public SelectListCollection SelectLists
    {
      get { return SubElementsSupport.SelectLists(DomContainer, elementCollection); }
    }

    public Table Table(string elementId)
    {
      return Table(Find.ById(elementId));
    }

    public Table Table(Attribute findBy)
    {
      return SubElementsSupport.Table(DomContainer, findBy, elementCollection);
    }

    public TableCollection Tables
    {
      get { return SubElementsSupport.Tables(DomContainer, elementCollection); }
    }

    //    public TableSectionCollection TableSections
    //    {
    //      get { return SubElementsSupport.TableSections(Ie, elementCollection); }
    //    }

    public TableCell TableCell(string elementId)
    {
      return TableCell(Find.ById(elementId));
    }

    public TableCell TableCell(Attribute findBy)
    {
      return SubElementsSupport.TableCell(DomContainer, findBy, elementCollection);
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
      return SubElementsSupport.TableCell(DomContainer, elementId, occurrence, elementCollection);
    }

    public TableCellCollection TableCells
    {
      get { return SubElementsSupport.TableCells(DomContainer, elementCollection); }
    }

    public TableRow TableRow(string elementId)
    {
      return TableRow(Find.ById(elementId));
    }

    public TableRow TableRow(Attribute findBy)
    {
      return SubElementsSupport.TableRow(DomContainer, findBy, elementCollection);
    }

    public virtual TableRowCollection TableRows
    {
      get { return SubElementsSupport.TableRows(DomContainer,elementCollection); }
    }

    public TextField TextField(string elementId)
    {
      return TextField(Find.ById(elementId));
    }

    public TextField TextField(Attribute findBy)
    {
      return SubElementsSupport.TextField(DomContainer, findBy, elementCollection);
    }

    public TextFieldCollection TextFields
    {
      get { return SubElementsSupport.TextFields(DomContainer, elementCollection); }
    }

	  public Span Span(string elementId)
	  {
	    return Span(Find.ById(elementId));
	  }

	  public Span Span(Attribute findBy)
	  {
      return SubElementsSupport.Span(DomContainer, findBy, elementCollection);
    }

	  public SpanCollection Spans
    {
      get { return SubElementsSupport.Spans(DomContainer, elementCollection); }
    }

	  public Div Div(string elementId)
	  {
	    return Div(Find.ById(elementId));
	  }

	  public Div Div(Attribute findBy)
	  {
      return SubElementsSupport.Div(DomContainer, findBy, elementCollection);
    }

	  public DivCollection Divs
    {
      get { return SubElementsSupport.Divs(DomContainer, elementCollection); }
    }

	  public Image Image(string elementId)
	  {
	    return Image(Find.ById(elementId));
	  }

	  public Image Image(Attribute findBy)
	  {
      return SubElementsSupport.Image(DomContainer, findBy, elementCollection);
    }

	  public ImageCollection Images
    {
      get { return SubElementsSupport.Images(DomContainer, elementCollection); }
    }
    #endregion

    private IHTMLElementCollection elementCollection
    {
      get
      {
        return (IHTMLElementCollection)((IHTMLElement)DomElement).all;
      }
    }
	}
}
