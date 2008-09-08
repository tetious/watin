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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using System.Text.RegularExpressions;
using System.Threading;
using mshtml;
using WatiN.Core.Constraints;
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
		public Document() {}

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
		/// Gets the inner text of the Body part of the webpage.
		/// </summary>
		/// <value>The inner text.</value>
		public string Text
		{
			get
			{
				IHTMLElement body = HtmlDocument.body;

				if (body == null) return null;

				return body.innerText;
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
			string innertext = Text;

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
			string innertext = Text;

			if (innertext == null) return false;

			return (regex.Match(innertext).Success);
		}


        /// <summary>
        /// Waits until the text is inside the HTML Body element contains the given <paramref name="text" />.
        /// Will time out after <see name="Settings.WaitUntilExistsTimeOut" />
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///     <see name="TimeoutException"/> if the specified text is not found within the time out period.
        /// </returns>
        public void WaitUntilContainsText(string text)
        {
            WaitUntilContainsText(text, Settings.WaitUntilExistsTimeOut);
        }

	    /// <summary>
        /// Waits until the text is inside the HTML Body element contains the given <paramref name="text" />.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="timeOut">The number of seconds to wait</param>
        /// <returns>
        ///     <see name="TimeoutException"/> if the specified text is not found within the time out period.
        /// </returns>
        public void WaitUntilContainsText(string text, int timeOut)
        {
            SimpleTimer timer = new SimpleTimer(timeOut);

            do
            {
                if (ContainsText(text)) { return; }

                Thread.Sleep(50);
            } while (!timer.Elapsed);

            throw new WatiN.Core.Exceptions.TimeoutException(string.Format("waiting {0} seconds for document to contain text '{1}'.", Settings.WaitUntilExistsTimeOut, text));
        }

        /// <summary>
        /// Waits until the <paramref name="regex" /> matches some text inside the HTML Body element contains the given <paramref name="text" />.
        /// Will time out after <see name="Settings.WaitUntilExistsTimeOut" />
        /// </summary>
        /// <param name="regex">The regular expression to match with.</param>
        /// <returns>
        ///     <see name="TimeoutException"/> if the specified text is not found within the time out period.
        /// </returns>
        public void WaitUntilContainsText(Regex regex)
        {
            WaitUntilContainsText(regex, Settings.WaitUntilExistsTimeOut);
        }

	    /// <summary>
        /// Waits until the <paramref name="regex" /> matches some text inside the HTML Body element contains the given <paramref name="text" />.
        /// </summary>
        /// <param name="regex">The regular expression to match with.</param>
        /// <param name="timeOut">The number of seconds to wait</param>
        /// <returns>
        ///     <see name="TimeoutException"/> if the specified text is not found within the time out period.
        /// </returns>
        public void WaitUntilContainsText(Regex regex, int timeOut)
        {
            SimpleTimer timer = new SimpleTimer(timeOut);

            do
            {
                if (ContainsText(regex)) { return; }

                Thread.Sleep(50);
            } while (!timer.Elapsed);

            throw new WatiN.Core.Exceptions.TimeoutException(string.Format("waiting {0} seconds for document to contain regex '{1}'.", Settings.WaitUntilExistsTimeOut, regex));
        }

		/// <summary>
		/// Gets the text inside the HTML Body element that matches the regular expression.
		/// </summary>
		/// <param name="regex">The regular expression to match with.</param>
		/// <returns>The matching text, or null if none.</returns>
		public string FindText(Regex regex)
		{
			Match match = regex.Match(Text);

			return match.Success ? match.Value : null;
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
		/// Gets the active element in the webpage.
		/// </summary>
		/// <value>The active element or <c>null</c> if no element has the focus.</value>
		public Element ActiveElement
		{
			get
			{
				IHTMLElement activeElement = HtmlDocument.activeElement;
				if (activeElement != null)
				{
                    return TypedElementFactory.CreateTypedElement(domContainer, domContainer.NativeBrowser.CreateElement(activeElement));
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the specified frame by its id.
		/// </summary>
		/// <param name="id">The id of the frame.</param>
		/// <exception cref="FrameNotFoundException">Thrown if the given <paramref name="id" /> isn't found.</exception>
		public Frame Frame(string id)
		{
			return Frame(Find.ById(id));
		}

		/// <summary>
		/// Gets the specified frame by its id.
		/// </summary>
		/// <param name="id">The regular expression to match with the id of the frame.</param>
		/// <exception cref="FrameNotFoundException">Thrown if the given <paramref name="id" /> isn't found.</exception>
		public Frame Frame(Regex id)
		{
			return Frame(Find.ById(id));
		}

		/// <summary>
		/// Gets the specified frame by its name.
		/// </summary>
		/// <param name="findBy">The name of the frame.</param>
		/// <exception cref="FrameNotFoundException">Thrown if the given name isn't found.</exception>
		public Frame Frame(BaseConstraint findBy)
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

		public TableBody TableBody(BaseConstraint findBy)
		{
			return ElementsSupport.TableBody(DomContainer, findBy, this);
		}

#if !NET11
        public TableBody TableBody(Predicate<TableBody> predicate)
	    {
	        return TableBody(Find.ByElement(predicate));
	    }
#endif

	    public TableBodyCollection TableBodies
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
	        return TableCell(Find.ByElement(predicate));
	    }
#endif

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

		public TableRow TableRow(BaseConstraint findBy)
		{
			return ElementsSupport.TableRow(DomContainer, findBy, this);
		}

#if !NET11
        public TableRow TableRow(Predicate<TableRow> predicate)
	    {
	        return TableRow(Find.ByElement(predicate));
	    }
#endif

	    public TableRowCollection TableRows
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

		public DomContainer DomContainer
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
		/// <param name="javaScriptCode">The javascript code.</param>
		public void RunScript(string javaScriptCode)
		{
			RunScript(javaScriptCode, "javascript");
		}

		/// <summary>
		/// Runs the script code in IE.
		/// </summary>
		/// <param name="scriptCode">The script code.</param>
		/// <param name="language">The language.</param>
		public void RunScript(string scriptCode, string language)
		{
			UtilityClass.RunScript(scriptCode, language, HtmlDocument.parentWindow);
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

		/// <summary>
		/// Evaluates the specified JavaScript code within the scope of this
		/// document. Returns the value computed by the last expression in the
		/// code block after implicit conversion to a string.
		/// </summary>
		/// <example>
		/// The following example shows an alert dialog then returns the string "4".
		/// <code>
		/// Eval("window.alert('Hello world!'); 2 + 2");
		/// </code>
		/// </example>
		/// <param name="javaScriptCode">The JavaScript code</param>
		/// <returns>The result converted to a string</returns>
		/// <exception cref="JavaScriptException">Thrown when the JavaScript code cannot be evaluated
		/// or throws an exception during evaluation</exception>
		public string Eval(string javaScriptCode)
		{
			const string resultPropertyName = "___expressionResult___";
			const string errorPropertyName = "___expressionError___";

			string exprWithAssignment = "try {\n"
			                            + "document." + resultPropertyName + "= String(eval('" + javaScriptCode.Replace("'", "\\'") + "'))\n"
			                            + "} catch (error) {\n"
			                            + "document." + errorPropertyName + "= 'message' in error ? error.name + ': ' + error.message : String(error)\n"
			                            + "}";

			// Run the script.
			RunScript(exprWithAssignment);

			// See if an error occured.
			string error = GetPropertyValue(errorPropertyName);
			if (error != null)
			{
				throw new JavaScriptException(error);
			}

			// Return the result
			return GetPropertyValue(resultPropertyName);
		}

		private string GetPropertyValue(string propertyName)
		{
			IExpando domDocumentExpando = (IExpando) HtmlDocument;

			PropertyInfo errorProperty = domDocumentExpando.GetProperty(propertyName, BindingFlags.Default);
			if (errorProperty != null)
			{
				try
				{
					return (string) errorProperty.GetValue(domDocumentExpando, null);
				}
				catch (COMException)
				{
					return null;
				}
			}

			return null;
		}
	}
}