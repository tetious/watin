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
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
  /// <summary>
  /// This class gives access to all contained elements of the webpage or the
  /// frames within this webpage.
  /// </summary>
  ///     /// <example>
  /// This example opens a webpage, types some text and submits it by clicking
  /// the submit button.
  /// <code>
  /// using WatiN.Core;
  /// 
  /// namespace NewIEExample
  /// {
  ///    public class WatiNWebsite
  ///    {
  ///      public WatiNWebsite()
  ///      {
  ///        IE ie = new IE("http://www.example.net");
  /// 
  ///        ie.TextField(Find.ById("textFieldComment")).TypeText("This is a comment to submit");
  ///        ie.Button(Find.ByText("Submit")).Click;
  /// 
  ///        ie.Close;
  ///      }
  ///    }
  ///  }
  /// </code>
  /// </example>
  public abstract class Document : ISubElements
  {
    private DomContainer domContainer;
    private IHTMLDocument2 htmlDocument;

    /// <summary>
    /// Initializes a new instance of the <see cref="Document"/> class.
    /// Mainly used by WatiN internally. You should override HtmlDocument
    /// and set DomContainer before accessing any method or property of 
    /// this class.
    /// </summary>
    public Document(){}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Document"/> class.
    /// Mainly used by WatiN internally.
    /// </summary>
    /// <param name="domContainer">The DOM container.</param>
    /// <param name="htmlDocument">The HTML document.</param>
    public Document(DomContainer domContainer, IHTMLDocument2 htmlDocument)
    {
      ArgumentRequired(domContainer, "domContainer");
      ArgumentRequired(htmlDocument, "htmlDocument");

      DomContainer = domContainer;
      this.htmlDocument = htmlDocument;
    }

    internal virtual void Dispose()
    {
      DomContainer = null;
      htmlDocument = null;
    }

    /// <summary>
    /// Gives access to the wrapped IHTMLDocument2 interface. This makes it
    /// possible to get even more control of the webpage by using the MSHTML
    /// Dom objectmodel.
    /// </summary>
    /// <value>The HTML document.</value>
    public virtual IHTMLDocument2 HtmlDocument
    {
      get
      {
        return htmlDocument;
      }
    }

    /// <summary>
    /// Gets the HTML of the Body part of the webpage.
    /// </summary>
    /// <value>The HTML.</value>
    public string Html
    {
      get
      {
        return HtmlDocument.body.outerHTML;
      }
    }

    /// <summary>
    /// Gets the URL of the displayed webpage.
    /// </summary>
    /// <value>The URL.</value>
    public string Url
    {
      get { return HtmlDocument.url; }
    }
    
    /// <summary>
    /// Determines whether the specified text contains the given <paramref name="text" />.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>
    /// 	<c>true</c> if the specified text is contained in <see cref="Html"/>; otherwise, <c>false</c>.
    /// </returns>
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

    /// <summary>
    /// Gets the title of the webpage.
    /// </summary>
    /// <value>The title.</value>
    public string Title
    {
      get { return HtmlDocument.title; }
    }

    /// <summary>
    /// Gets the specified frame by it's id.
    /// </summary>
    /// <param name="id">The id of the frame.</param>
    /// <exception cref="FrameNotFoundException">Thrown if the given <paramref name="id" /> isn't found.</exception>
    public Frame Frame(string id)
    {
      return Frame(Find.ById(id));
    }
    
    /// <summary>
    /// Gets the specified frame by it's name.
    /// </summary>
    /// <param name="findBy">The name of the frame.</param>
    /// <exception cref="FrameNotFoundException">Thrown if the given name isn't found.</exception>
    public Frame Frame(NameValue findBy)
    {
      return Core.Frame.Find(Frames, findBy);
    }
    
    /// <summary>
    /// Gets the specified frame by it's Url.
    /// </summary>
    /// <param name="findBy">The Url of the frame.</param>
    /// <exception cref="FrameNotFoundException">Thrown if the given Url isn't found.</exception>
    public Frame Frame(UrlValue findBy)
    {
      return Core.Frame.Find(Frames, findBy);
    }
    
    /// <summary>
    /// Gets the specified frame by it's id.
    /// </summary>
    /// <param name="findBy">The id of the frame.</param>
    /// <exception cref="FrameNotFoundException">Thrown if the given id isn't found.</exception>
    public Frame Frame(IdValue findBy)
    {
      return Core.Frame.Find(Frames, findBy);
    }

    /// <summary>
    /// Gets a typed collection of <see cref="WatiN.Core.Frame"/> opend within this <see cref="Document"/>.
    /// </summary>
    public FrameCollection Frames
    {
      get
      {
        return new FrameCollection(DomContainer, HtmlDocument);
      }
    }

    #region ISubElements

    /// <summary>
    /// Gets the specified Button by it's id.
    /// </summary>
    /// <param name="elementId">The id of the element.</param>
    /// <exception cref="ElementNotFoundException">Thrown if the given <paramref name="elementId"/> isn't found.</exception>
    /// <example>
    /// This example opens a webpage, types some text and submits it by clicking
    /// the submit button.
    /// <code>
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE("http://www.example.net");
    ///        ie.TextField(Find.ById("textFieldComment")).TypeText("This is a comment to submit");
    ///        ie.Button("buttonSubmit").Click;
    ///        ie.Close;
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public Button Button(string elementId)
    {
      return Button(Find.ById(elementId));
    }

    /// <summary>
    /// Gets the specified Button by using the given <see cref="AttributeValue" /> to find the Button.
    /// <seealso cref="Find" />
    /// </summary>
    /// <param name="findBy">The <see cref="AttributeValue"/> class or one of it's subclasses to find an element by. The <see cref="Find" /> class provides factory methodes to create specialized instances.</param>
    /// <exception cref="ElementNotFoundException">Thrown if the given <paramref name="findBy"/> doesn't match an element in the webpage.</exception>
    /// <example>
    /// This example opens a webpage, types some text and submits it by clicking
    /// the submit button.
    /// <code>
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE("http://www.example.net");
    ///        IdValue textFieldId = new IdValue("textFieldComment");
    ///        ie.TextField(textFieldId).TypeText("This is a comment to submit");
    ///        ie.Button(Find.ByText("Submit")).Click;
    ///        ie.Close;
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public Button Button(AttributeValue findBy)
    {
      return SubElementsSupport.Button(DomContainer, findBy, elementCollection);
    }

    /// <summary>
    /// Gets a typed collection of <see cref="WatiN.Core.Button" /> instances within this <see cref="Document"/>.
    /// </summary>
    ///     /// <example>
    /// This example opens a webpage and writes out the text of each button to the
    /// debug window.
    /// <code>
    /// using WatiN.Core;
    /// 
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE("http://www.example.net");
    ///       
    ///        ButtonCollection buttons = ie.Buttons;
    /// 
    ///        foreach (Button button in buttons)
    ///        {
    ///          System.Diagnostics.Debug.Writeline(button.Text);
    ///        }
    /// 
    ///        ie.Close;
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public ButtonCollection Buttons
    {
      get { return SubElementsSupport.Buttons(DomContainer, elementCollection); }
    }

    public CheckBox CheckBox(string elementId)
    {
      return CheckBox(Find.ById(elementId));
    }

    public CheckBox CheckBox(AttributeValue findBy)
    {
      return SubElementsSupport.CheckBox(DomContainer, findBy, elementCollection);
    }

    public CheckBoxCollection CheckBoxes
    {
      get { return SubElementsSupport.CheckBoxes(DomContainer, elementCollection); }
    }

    public Form Form(string elementId)
    {
      return Form(Find.ById(elementId));
    }

    public Form Form(AttributeValue findBy)
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

    public Label Label(AttributeValue findBy)
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

    public Link Link(AttributeValue findBy)
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

    public Para Para(AttributeValue findBy)
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

    public RadioButton RadioButton(AttributeValue findBy)
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

    public SelectList SelectList(AttributeValue findBy)
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

    public Table Table(AttributeValue findBy)
    {
      return SubElementsSupport.Table(DomContainer, findBy, elementCollection);
    }

    public TableCollection Tables
    {
      get { return SubElementsSupport.Tables(DomContainer, elementCollection); }
    }

    //    public TableSectionCollection TableSections
    //    {
    //      get { return SubElementsSupport.TableSections(ie, elementCollection); }
    //    }

    public TableCell TableCell(string elementId)
    {
      return TableCell(Find.ById(elementId));
    }

    public TableCell TableCell(AttributeValue findBy)
    {
      return SubElementsSupport.TableCell(DomContainer, findBy, elementCollection);
    }

    public TableCell TableCell(string elementId, int occurence)
    {
      return SubElementsSupport.TableCell(DomContainer, elementId, occurence, elementCollection);
    }

    public TableCellCollection TableCells
    {
      get { return SubElementsSupport.TableCells(DomContainer, elementCollection); }
    }

    public TableRow TableRow(string elementId)
    {
      return TableRow(Find.ById(elementId));
    }

    public TableRow TableRow(AttributeValue findBy)
    {
      return SubElementsSupport.TableRow(DomContainer, findBy, elementCollection);
    }

    public TableRowCollection TableRows
    {
      get { return SubElementsSupport.TableRows(DomContainer,elementCollection); }
    }

    public TextField TextField(string elementId)
    {
      return TextField(Find.ById(elementId));
    }

    public TextField TextField(AttributeValue findBy)
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

    public Span Span(AttributeValue findBy)
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

    public Div Div(AttributeValue findBy)
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

    public Image Image(AttributeValue findBy)
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
        return HtmlDocument.all;
      }
    }

    protected DomContainer DomContainer
    {
      get { return domContainer; }
      set { domContainer = value; }
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