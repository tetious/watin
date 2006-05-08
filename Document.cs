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

using mshtml;

namespace WatiN
{
  public class Document : ISubElements
  {
    protected DomContainer ie;
    private IHTMLDocument2 htmlDocument;

    public Document(DomContainer ie, IHTMLDocument2 htmlDocument)
    {
      ArgumentRequired(ie, "ie");
      ArgumentRequired(htmlDocument, "htmlDocument");

      this.ie = ie;
      this.htmlDocument = htmlDocument;
    }

    internal void Close()
    {
      ie = null;
      htmlDocument = null;
    }

    public IHTMLDocument2 HtmlDocument
    {
      get
      {
        return htmlDocument;
      }
    }

    public string Html
    {
      get
      {
        return HtmlDocument.body.outerHTML;
      }
    }

    public string Url
    {
      get { return HtmlDocument.url; }
    }
    public bool ContainsText(string text)
    {
      string innertext = HtmlDocument.body.innerText;
      
      if (innertext == null) return false;

      return (innertext.IndexOf(text) >= 0);

      // Also found this implementation which seems to work OK to.
      // TODO: test if innertext assert might return a wrong result when
      // using hidden fields/ fields which aren't visible for the user.
      //  		HTMLBody body = (HTMLBody)htmlDocument.getElementsByTagName("body").item(0, null);
      //			return (body.createTextRange().findText(text, 0, 0) == true);

    }

    public string Title
    {
      get { return htmlDocument.title; }
    }

    public Frame Frame(string elementID)
    {
      return Frame(Find.ByID(elementID));
    }
    
    public Frame Frame(NameValue findBy)
    {
      return WatiN.Frame.Find(Frames, findBy);
    }
    public Frame Frame(UrlValue findBy)
    {
      return WatiN.Frame.Find(Frames, findBy);
    }
    public Frame Frame(IDValue findBy)
    {
      return WatiN.Frame.Find(Frames, findBy);
    }

    public FrameCollection Frames
    {
      get
      {
        return new FrameCollection(ie, htmlDocument);
      }
    }

    #region ISubElements

    public Button Button(string elementID)
    {
      return Button(Find.ByID(elementID));
    }

    public Button Button(AttributeValue findBy)
    {
      return SubElementsSupport.Button(ie, findBy, elementCollection);
    }

    public ButtonCollection Buttons
    {
      get { return SubElementsSupport.Buttons(ie, elementCollection); }
    }

    public CheckBox CheckBox(string elementID)
    {
      return CheckBox(Find.ByID(elementID));
    }

    public CheckBox CheckBox(AttributeValue findBy)
    {
      return SubElementsSupport.CheckBox(ie, findBy, elementCollection);
    }

    public CheckBoxCollection CheckBoxs
    {
      get { return SubElementsSupport.Checkboxes(ie, elementCollection); }
    }

    public Form Form(string elementID)
    {
      return Form(Find.ByID(elementID));
    }

    public Form Form(AttributeValue findBy)
    {
      return SubElementsSupport.Form(ie, findBy, elementCollection);
    }

    public FormCollection Forms
    {
      get { return SubElementsSupport.Forms(ie, elementCollection); }
    }

    public Label Label(string elementID)
    {
      return Label(Find.ByID(elementID));
    }

    public Label Label(AttributeValue findBy)
    {
      return SubElementsSupport.Label(ie, findBy, elementCollection);
    }

    public LabelCollection Labels
    {
      get { return SubElementsSupport.Labels(ie, elementCollection); }
    }

    public Link Link(string elementID)
    {
      return Link(Find.ByID(elementID));
    }

    public Link Link(AttributeValue findBy)
    {
      return SubElementsSupport.Link(ie, findBy, elementCollection);
    }

    public LinkCollection Links
    {
      get { return SubElementsSupport.Links(ie, elementCollection); }
    }

    public Para Para(string elementID)
    {
      return Para(Find.ByID(elementID));
    }

    public Para Para(AttributeValue findBy)
    {
      return SubElementsSupport.Para(ie, findBy, elementCollection);
    }

    public ParaCollection Paras
    {
      get { return SubElementsSupport.Paras(ie, elementCollection); }
    }

    public RadioButton RadioButton(string elementID)
    {
      return RadioButton(Find.ByID(elementID));
    }

    public RadioButton RadioButton(AttributeValue findBy)
    {
      return SubElementsSupport.RadioButton(ie, findBy, elementCollection);
    }

    public RadioButtonCollection RadioButtons
    {
      get { return SubElementsSupport.RadioButtons(ie, elementCollection); }
    }

    public SelectList SelectList(string elementID)
    {
      return SelectList(Find.ByID(elementID));
    }

    public SelectList SelectList(AttributeValue findBy)
    {
      return SubElementsSupport.SelectList(ie, findBy, elementCollection);
    }

    public SelectListCollection SelectLists
    {
      get { return SubElementsSupport.SelectLists(ie, elementCollection); }
    }

    public Table Table(string elementID)
    {
      return Table(Find.ByID(elementID));
    }

    public Table Table(AttributeValue findBy)
    {
      return SubElementsSupport.Table(ie, findBy, elementCollection);
    }

    public TableCollection Tables
    {
      get { return SubElementsSupport.Tables(ie, elementCollection); }
    }

    //    public TableSectionCollection TableSections
    //    {
    //      get { return SubElementsSupport.TableSections(ie, elementCollection); }
    //    }

    public TableCell TableCell(string elementID)
    {
      return TableCell(Find.ByID(elementID));
    }

    public TableCell TableCell(AttributeValue findBy)
    {
      return SubElementsSupport.TableCell(ie, findBy, elementCollection);
    }

    public TableCell TableCell(string elementId, int occurence)
    {
      return SubElementsSupport.TableCell(ie, elementId, occurence, elementCollection);
    }

    public TableCellCollection TableCells
    {
      get { return SubElementsSupport.TableCells(ie, elementCollection); }
    }

    public TableRow TableRow(string elementID)
    {
      return TableRow(Find.ByID(elementID));
    }

    public TableRow TableRow(AttributeValue findBy)
    {
      return SubElementsSupport.TableRow(ie, findBy, elementCollection);
    }

    public TableRowCollection TableRows
    {
      get { return SubElementsSupport.TableRows(ie,elementCollection); }
    }

    public TextField TextField(string elementID)
    {
      return TextField(Find.ByID(elementID));
    }

    public TextField TextField(AttributeValue findBy)
    {
      return SubElementsSupport.TextField(ie, findBy, elementCollection);
    }

    public TextFieldCollection TextFields
    {
      get { return SubElementsSupport.TextFields(ie, elementCollection); }
    }

    public Span Span(string elementID)
    {
      return Span(Find.ByID(elementID));
    }

    public Span Span(AttributeValue findBy)
    {
      return SubElementsSupport.Span(ie, findBy, elementCollection);
    }

    public SpanCollection Spans
    {
      get { return SubElementsSupport.Spans(ie, elementCollection); }
    }

    public Div Div(string elementID)
    {
      return Div(Find.ByID(elementID));
    }

    public Div Div(AttributeValue findBy)
    {
      return SubElementsSupport.Div(ie, findBy, elementCollection);
    }

    public DivCollection Divs
    {
      get { return SubElementsSupport.Divs(ie, elementCollection); }
    }

    public Image Image(string elementID)
    {
      return Image(Find.ByID(elementID));
    }

    public Image Image(AttributeValue findBy)
    {
      return SubElementsSupport.Image(ie, findBy, elementCollection);
    }

    public ImageCollection Images
    {
      get { return SubElementsSupport.Images(ie, elementCollection); }
    }
    #endregion

    private IHTMLElementCollection elementCollection
    {
      get
      {
        return htmlDocument.all;
      }
    }

    private static void ArgumentRequired(object requiredObject, string name)
    {
      if (requiredObject == null)
      {
        throw new ArgumentNullException(name);
      }
    }
  }
}