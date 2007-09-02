#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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

namespace WatiN.Core
{
  using mshtml;
  using System;
  using System.Text.RegularExpressions;
  using WatiN.Core.Interfaces;
  using WatiN.Core.Exceptions;

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
  public abstract class Document : IElementsContainer, IDisposable, IElementCollection 
  {
    private DomContainer domContainer;
    private IHTMLDocument2 htmlDocument;

    /// <summary>
    /// Initializes a new instance of the <see cref="Document"/> class.
    /// Mainly used by WatiN internally. You should override HtmlDocument
    /// and set DomContainer before accessing any method or property of 
    /// this class.
    /// </summary>
    public Document()
    {
    }

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

    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
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
      get { return htmlDocument; }
    }

    /// <summary>
    /// Gets the HTML of the Body part of the webpage.
    /// </summary>
    /// <value>The HTML.</value>
    public string Html
    {
      get
      {
        IHTMLElement body = HtmlDocument.body;
        
        if (body == null) return null;

        return body.outerHTML;
      }
    }

    /// <summary>
    /// Returns a System.Uri instance of the url displayed in the address bar of the browser, 
    /// of the currently displayed web page.
    /// </summary>
    /// <example>
    /// The following example creates a new Internet Explorer instances, navigates to
    /// the WatiN Project website on SourceForge and writes the Uri of the
    /// currently displayed webpage to the debug window.
    /// <code>
    /// using WatiN.Core;
    /// using System.Diagnostics;
    ///
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE("http://watin.sourceforge.net");
    ///        Debug.WriteLine(ie.Uri.ToString());
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public Uri Uri
    {
      get { return new Uri(HtmlDocument.url); }
    }

    /// <summary>
    /// Returns the url, as displayed in the address bar of the browser, of the currently
    /// displayed web page.
    /// </summary>
    /// <example>
    /// The following example creates a new Internet Explorer instances, navigates to
    /// the WatiN Project website on SourceForge and writes the Url of the
    /// currently displayed webpage to the debug window.
    /// <code>
    /// using WatiN.Core;
    /// using System.Diagnostics;
    ///
    /// namespace NewIEExample
    /// {
    ///    public class WatiNWebsite
    ///    {
    ///      public WatiNWebsite()
    ///      {
    ///        IE ie = new IE("http://watin.sourceforge.net");
    ///        Debug.WriteLine(ie.Url);
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public string Url
    {
      get { return HtmlDocument.url; }
    }

    /// <summary>
    /// Determines whether the text inside the HTML Body element contains the given <paramref name="text" />.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>
    ///     <c>true</c> if the specified text is contained in <see cref="Html"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsText(string text)
    {
      IHTMLElement body = HtmlDocument.body;
      
      if (body == null) return false;

      string innertext = body.innerText;

      if (innertext == null) return false;

      return (innertext.IndexOf(text) >= 0);
    }

    /// <summary>
    /// Determines whether the text inside the HTML Body element contains the given <paramref name="regex" />.
    /// </summary>
    /// <param name="regex">The regular expression to match with.</param>
    /// <returns>
    ///     <c>true</c> if the specified text is contained in <see cref="Html"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsText(Regex regex)
    {
      IHTMLElement body = HtmlDocument.body;
      
      if (body == null) return false;

      string innertext = body.innerText;

      if (innertext == null) return false;

      return (regex.Match(innertext).Success);
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
    /// Gets the active element in this webpage.
    /// </summary>
    /// <value>The active element or <c>null</c> if no element has the focus.</value>
    public Element ActiveElement
    {
      get
      {
        IHTMLElement activeElement = HtmlDocument.activeElement;
        if (activeElement != null)
        {
          return Core.Element.GetTypedElement(domContainer, activeElement);
        }
        return null;
      }
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
    /// Gets the specified frame by it's id.
    /// </summary>
    /// <param name="id">The regular expression to match with the id of the frame.</param>
    /// <exception cref="FrameNotFoundException">Thrown if the given <paramref name="id" /> isn't found.</exception>
    public Frame Frame(Regex id)
    {
      return Frame(Find.ById(id));
    }

    /// <summary>
    /// Gets the specified frame by it's name.
    /// </summary>
    /// <param name="findBy">The name of the frame.</param>
    /// <exception cref="FrameNotFoundException">Thrown if the given name isn't found.</exception>
    public Frame Frame(Attribute findBy)
    {
      return Core.Frame.Find(Frames, findBy);
    }

    /// <summary>
    /// Gets a typed collection of <see cref="WatiN.Core.Frame"/> opend within this <see cref="Document"/>.
    /// </summary>
    public FrameCollection Frames
    {
      get { return new FrameCollection(DomContainer, HtmlDocument); }
    }

    #region IElementsContainer

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

    public Button Button(Regex elementId)
    {
      return Button(Find.ById(elementId));
    }

    /// <summary>
    /// Gets the specified Button by using the given <see cref="Attribute" /> to find the Button.
    /// <seealso cref="Find" />
    /// </summary>
    /// <param name="findBy">The <see cref="Attribute"/> class or one of it's subclasses to find an element by. The <see cref="Find" /> class provides factory methodes to create specialized instances.</param>
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
    ///        Id textFieldId = new Id("textFieldComment");
    ///        ie.TextField(textFieldId).TypeText("This is a comment to submit");
    ///        ie.Button(Find.ByText("Submit")).Click;
    ///        ie.Close;
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </example>
    public Button Button(Attribute findBy)
    {
      return new Button(DomContainer, ElementFinder.ButtonFinder(findBy, this));
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
      get { return new ButtonCollection(DomContainer, ElementFinder.ButtonFinder(null, this)); }
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
      return ElementsSupport.CheckBox(DomContainer, findBy, this);
    }

    public CheckBoxCollection CheckBoxes
    {
      get { return ElementsSupport.CheckBoxes(DomContainer, this); }
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
      return ElementsSupport.Element(DomContainer, findBy, this);
    }

    public Element Element(string tagname, Attribute findBy, params string[] inputtypes)
    {
      return ElementsSupport.Element(DomContainer, tagname, findBy, this, inputtypes);
    }

    public ElementCollection Elements
    {
      get { return ElementsSupport.Elements(DomContainer, this); }
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
      return ElementsSupport.FileUpload(DomContainer, findBy, this);
    }

    public FileUploadCollection FileUploads
    {
      get { return ElementsSupport.FileUploads(DomContainer, this); }
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
      return ElementsSupport.Form(DomContainer, findBy, this);
    }

    public FormCollection Forms
    {
      get { return ElementsSupport.Forms(DomContainer, this); }
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
      return ElementsSupport.Label(DomContainer, findBy, this);
    }

    public LabelCollection Labels
    {
      get { return ElementsSupport.Labels(DomContainer, this); }
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
      return ElementsSupport.Link(DomContainer, findBy, this);
    }

    public LinkCollection Links
    {
      get { return ElementsSupport.Links(DomContainer, this); }
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
      return ElementsSupport.Para(DomContainer, findBy, this);
    }

    public ParaCollection Paras
    {
      get { return ElementsSupport.Paras(DomContainer, this); }
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
      return ElementsSupport.RadioButton(DomContainer, findBy, this);
    }

    public RadioButtonCollection RadioButtons
    {
      get { return ElementsSupport.RadioButtons(DomContainer, this); }
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
      return ElementsSupport.SelectList(DomContainer, findBy, this);
    }

    public SelectListCollection SelectLists
    {
      get { return ElementsSupport.SelectLists(DomContainer, this); }
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
      return ElementsSupport.Table(DomContainer, findBy, this);
    }

    public TableCollection Tables
    {
      get { return ElementsSupport.Tables(DomContainer, this); }
    }

    public TableBody TableBody (string elementId)
    {
      return TableBody(Find.ById(elementId));
    }

    public TableBody TableBody(Regex elementId)
    {
      return TableBody(Find.ById(elementId));
    }

    public TableBody TableBody(Attribute findBy)
    {
      return ElementsSupport.TableBody(DomContainer,findBy, this);
    }

    public TableBodyCollection TableBodies
    {
      get { return ElementsSupport.TableBodies(DomContainer, this); }
    }

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
      return ElementsSupport.TableCell(DomContainer, findBy, this);
    }

    /// <summary>
    /// Finds a TableCell by the n-th index of an id. 
    /// index counting is zero based.
    /// </summary>  
    /// <example>
    /// This example will get Text of the third(!) index on the page of a
    /// TableCell element with "tablecellid" as it's id value. 
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
      return TableRow(Find.ById(elementId));
    }

    public TableRow TableRow(Regex elementId)
    {
      return TableRow(Find.ById(elementId));
    }

    public TableRow TableRow(Attribute findBy)
    {
      return ElementsSupport.TableRow(DomContainer, findBy, this);
    }

    public TableRowCollection TableRows
    {
      get { return ElementsSupport.TableRows(DomContainer, this); }
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
      return ElementsSupport.TextField(DomContainer, findBy, this);
    }

    public TextFieldCollection TextFields
    {
      get { return ElementsSupport.TextFields(DomContainer, this); }
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
      return ElementsSupport.Span(DomContainer, findBy, this);
    }

    public SpanCollection Spans
    {
      get { return ElementsSupport.Spans(DomContainer, this); }
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
      return ElementsSupport.Div(DomContainer, findBy, this);
    }

    public DivCollection Divs
    {
      get { return ElementsSupport.Divs(DomContainer, this); }
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
      return ElementsSupport.Image(DomContainer, findBy, this);
    }

    public ImageCollection Images
    {
      get { return ElementsSupport.Images(DomContainer, this); }
    }

    #endregion

    protected DomContainer DomContainer
    {
      get { return domContainer; }
      set { domContainer = value; }
    }

    IHTMLElementCollection IElementCollection.Elements
    {
      get 
      {
        try
        {
          return HtmlDocument.all;
        }
        catch
        {
          return null;
        }
      }
    }

    private static void ArgumentRequired(object requiredObject, string name)
    {
      if (requiredObject == null)
      {
        throw new ArgumentNullException(name);
      }
    }

    /// <summary>
    /// Runs the javascript code in IE.
    /// </summary>
    /// <param name="scriptCode">The javascript code.</param>
    public void RunScript(string scriptCode)
    {
      RunScript(scriptCode, "javascript");
    }

    /// <summary>
    /// Runs the script code in IE.
    /// </summary>
    /// <param name="scriptCode">The script code.</param>
    /// <param name="language">The language.</param>
    public void RunScript(string scriptCode, string language)
    {
      UtilityClass.RunScript(scriptCode, "javascript", HtmlDocument.parentWindow);
    }

    /// <summary>
    /// Fires the given event on the given element.
    /// </summary>
    /// <param name="element">Element to fire the event on</param>
    /// <param name="eventName">Name of the event to fire</param>
    public virtual void FireEvent(DispHTMLBaseElement element, string eventName)
    {
      UtilityClass.FireEvent(element, eventName);
    }

    public string Eval(string code)
    {
      string elementId = Guid.NewGuid().ToString();

      string createElement =string.Format(@"
            var elem = document.createElement('INPUT');
            elem.id = '{0}';
            elem.type = 'HIDDEN';
            elem.value = {1};
            document.body.appendChild(elem);"
        , elementId, code);

      RunScript(createElement);

      string result = TextField(elementId).Value;

      string removeElement = string.Format(@"
            var elem = document.getElementById('{0}');
            document.body.removeChild(elem);"
        , elementId);
      
      RunScript(removeElement);

      return result;
    }
  }
}
