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
using WatiN.Core.Exceptions;
using WatiN.Core.Native;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
	/// <summary>
	/// This class gives access to all contained elements of the webpage or the
	/// frames within this webpage.
	/// </summary>
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
	public abstract class Document : Component, IElementContainer, IDisposable
	{
	    private const string RESULT_PROPERTY_NAME = "watinExpressionResult";
        public const string ERROR_PROPERTY_NAME = "watinExpressionError";

        private DomContainer domContainer;

		/// <summary>
		/// Initializes a new instance of the <see cref="Document"/> class.
		/// Mainly used by WatiN internally. You should override NativeDocument
		/// and set DomContainer before accessing any method or property of 
		/// this class.
		/// </summary>
		protected Document() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="Document"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		protected Document(DomContainer domContainer)
		{
            if (domContainer == null)
                throw new ArgumentNullException("domContainer");

			DomContainer = domContainer;
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
		{
			Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			DomContainer = null;
		}

		/// <summary>
        /// Gives access to the wrapped INativeDocument interface. This makes it
		/// possible to get even more control of the webpage by using wrapped element.
		/// </summary>
		/// <value>The NativeDocument.</value>
		public abstract INativeDocument NativeDocument { get; }

		/// <summary>
		/// Gets the HTML of the Body part of the webpage.
		/// </summary>
		/// <value>The HTML.</value>
        public virtual string Html
		{
			get
			{
			    return Body != null ? Body.OuterHtml : null;
			}
		}

        /// <summary>
        /// Gets the Body element of the webpage, or null if none.
        /// </summary>
        /// <value>The body, or null if none.</value>
        public virtual Body Body
        {
            get
            {
                var body = NativeDocument.Body;
                return body != null ? new Body(DomContainer, body) : null;
            }
        }

		/// <summary>
		/// Gets the inner text of the Body part of the webpage.
		/// </summary>
		/// <value>The inner text.</value>
        public virtual string Text
		{
			get
			{
				return Body != null ? Body.Text : null;
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
        public virtual Uri Uri
		{
			get { return new Uri(NativeDocument.Url); }
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
        public virtual string Url
		{
			get { return NativeDocument.Url; }
		}

		/// <summary>
		/// Determines whether the text inside the HTML Body element contains the given <paramref name="text" />.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>
		///     <c>true</c> if the specified text is contained in <see cref="Html"/>; otherwise, <c>false</c>.
		/// </returns>
        public virtual bool ContainsText(string text)
		{
            return NativeDocument.ContainsText(text);
		}

		/// <summary>
		/// Determines whether the text inside the HTML Body element contains the given <paramref name="regex" />.
		/// </summary>
		/// <param name="regex">The regular expression to match with.</param>
		/// <returns>
		///     <c>true</c> if the specified text is contained in <see cref="Html"/>; otherwise, <c>false</c>.
		/// </returns>
        public virtual bool ContainsText(Regex regex)
		{
			var innertext = Text;

            return innertext != null && (regex.Match(innertext).Success);
		}


        /// <summary>
        /// Waits until the text is inside the HTML Body element contains the given <paramref name="text" />.
        /// Will time out after <see name="Settings.WaitUntilExistsTimeOut" />
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///     <see name="TimeoutException"/> if the specified text is not found within the time out period.
        /// </returns>
        public virtual void WaitUntilContainsText(string text)
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
        public virtual void WaitUntilContainsText(string text, int timeOut)
        {
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(timeOut))
            {
                SleepTime = TimeSpan.FromMilliseconds(50),
                ExceptionMessage = () => string.Format("waiting {0} seconds for document to contain text '{1}'.", timeOut, text)
            };
            tryActionUntilTimeOut.Try(() => ContainsText(text));
        }

        /// <summary>
        /// Waits until the <paramref name="regex" /> matches some text inside the HTML Body element.
        /// Will time out after <see name="Settings.WaitUntilExistsTimeOut" />
        /// </summary>
        /// <param name="regex">The regular expression to match with.</param>
        /// <returns>
        ///     <see name="TimeoutException"/> if the specified text is not found within the time out period.
        /// </returns>
        public virtual void WaitUntilContainsText(Regex regex)
        {
            WaitUntilContainsText(regex, Settings.WaitUntilExistsTimeOut);
        }

	    /// <summary>
        /// Waits until the <paramref name="regex" /> matches some text inside the HTML Body element.
        /// </summary>
        /// <param name="regex">The regular expression to match with.</param>
        /// <param name="timeOut">The number of seconds to wait</param>
        /// <returns>
        ///     <see name="TimeoutException"/> if the specified text is not found within the time out period.
        /// </returns>
        public virtual void WaitUntilContainsText(Regex regex, int timeOut)
        {
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(timeOut))
            {
                SleepTime = TimeSpan.FromMilliseconds(50),
                ExceptionMessage = () => string.Format("waiting {0} seconds for document to contain regex '{1}'.", timeOut, regex)
            };
            tryActionUntilTimeOut.Try(() => ContainsText(regex));
        }

		/// <summary>
		/// Gets the text inside the HTML Body element that matches the regular expression.
		/// </summary>
		/// <param name="regex">The regular expression to match with.</param>
		/// <returns>The matching text, or null if none.</returns>
        public virtual string FindText(Regex regex)
		{
			var match = regex.Match(Text);

			return match.Success ? match.Value : null;
		}

		/// <summary>
		/// Gets the title of the webpage.
		/// </summary>
		/// <value>The title.</value>
        public virtual string Title
		{
			get { return NativeDocument.Title; }
		}

		/// <summary>
		/// Gets the active element in the webpage.
		/// </summary>
		/// <value>The active element or <c>null</c> if no element has the focus.</value>
        public virtual Element ActiveElement
		{
			get
			{
				var activeElement = NativeDocument.ActiveElement;
			    return activeElement == null ? null : ElementFactory.CreateElement(domContainer, activeElement);
			}
		}

		/// <summary>
		/// Gets the specified frame by its id.
		/// </summary>
		/// <param name="id">The id of the frame.</param>
		/// <exception cref="FrameNotFoundException">Thrown if the given <paramref name="id" /> isn't found.</exception>
        public virtual Frame Frame(string id)
		{
			return Frame(Find.ById(id));
		}

		/// <summary>
		/// Gets the specified frame by its id.
		/// </summary>
		/// <param name="id">The regular expression to match with the id of the frame.</param>
		/// <exception cref="FrameNotFoundException">Thrown if the given <paramref name="id" /> isn't found.</exception>
        public virtual Frame Frame(Regex id)
		{
			return Frame(Find.ById(id));
		}

		/// <summary>
		/// Gets the specified frame by its name.
		/// </summary>
		/// <param name="findBy">The name of the frame.</param>
		/// <exception cref="FrameNotFoundException">Thrown if the given name isn't found.</exception>
        public virtual Frame Frame(Constraint findBy)
		{
		    var frame = Frames.First(findBy);
            if (frame == null)
            {
                throw new FrameNotFoundException(findBy.ToString());
            }
		    return frame;
		}

		/// <summary>
		/// Gets a typed collection of <see cref="WatiN.Core.Frame"/> opend within this <see cref="Document"/>.
		/// </summary>
        public virtual FrameCollection Frames
		{
			get { return new FrameCollection(DomContainer, NativeDocument); }
		}

        /// <summary>
        /// Gets the document's DOM container.
        /// </summary>
        public virtual DomContainer DomContainer
		{
			get { return domContainer; }
			set { domContainer = value; }
		}

		/// <summary>
		/// Runs the javascript code in IE.
		/// </summary>
		/// <param name="javaScriptCode">The javascript code.</param>
        public virtual void RunScript(string javaScriptCode)
		{
			RunScript(javaScriptCode, "javascript");
		}

		/// <summary>
		/// Runs the script code in IE.
		/// </summary>
		/// <param name="scriptCode">The script code.</param>
		/// <param name="language">The language.</param>
        public virtual void RunScript(string scriptCode, string language)
		{
            NativeDocument.RunScript(scriptCode, language);
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
        public virtual string Eval(string javaScriptCode)
		{
		    var documentVariableName = NativeDocument.JavaScriptVariableName;

		    var resultVar = documentVariableName + "." + RESULT_PROPERTY_NAME;
		    var errorVar = documentVariableName + "." + ERROR_PROPERTY_NAME;

			var exprWithAssignment = resultVar + " = '';" + errorVar + " = '';"
                                        + "try {"
			                            +  resultVar + " = String(eval('" + javaScriptCode.Replace("'", "\\'") + "'))"
			                            + "} catch (error) {"
                                        + errorVar + " = 'message' in error ? error.name + ': ' + error.message : String(error)"
			                            + "};";

			// Run the script.
			RunScript(exprWithAssignment);

			// See if an error occured.
            var error = NativeDocument.GetPropertyValue(ERROR_PROPERTY_NAME);
			if (!string.IsNullOrEmpty(error))
			{
				throw new JavaScriptException(error);
			}

			// Return the result
            return NativeDocument.GetPropertyValue(RESULT_PROPERTY_NAME);
		}

        /// <summary>
        /// Gets a page object of the desired type that wraps this document.
        /// </summary>
        /// <typeparam name="TPage">The <see cref="Page" /> subclass</typeparam>
        /// <returns>The page object</returns>
        /// <example>
        /// <code>
        /// browser.Page&lt;SignInPage&gt;>().SignIn("somebody", "letmein");
        /// </code>
        /// </example>
        public virtual TPage Page<TPage>() where TPage : Page, new()
        {
            return Core.Page.CreatePage<TPage>(this);
        }

        private NativeElementCollectionAdapter AllElements
        {
            get { return new NativeElementCollectionAdapter(DomContainer, () => NativeDocument.AllElements); }
        }

        private IElementContainer ChildElements
        {
            get { return new NativeElementCollectionAdapter(DomContainer, () => NativeDocument.Body.Children); }
        }

        #region IElementsContainer

        public virtual Area Area(string elementId)
        {
            return AllElements.Area(elementId);
        }

        public virtual Area Area(Regex elementId)
        {
            return AllElements.Area(elementId);
        }

        public virtual Area Area(Constraint findBy)
        {
            return AllElements.Area(findBy);
        }

        public virtual Area Area(Predicate<Area> predicate)
        {
            return AllElements.Area(predicate);
        }

        public virtual AreaCollection Areas
        {
            get { return AllElements.Areas; }
        }

        public virtual Button Button(string elementId)
        {
            return AllElements.Button(elementId);
        }

        public virtual Button Button(Regex elementId)
        {
            return AllElements.Button(elementId);
        }

        public virtual Button Button(Predicate<Button> predicate)
        {
            return AllElements.Button(predicate);
        }

        public virtual Button Button(Constraint findBy)
        {
            return AllElements.Button(findBy);
        }

        public virtual ButtonCollection Buttons
        {
            get { return AllElements.Buttons; }
        }

        public virtual CheckBox CheckBox(string elementId)
        {
            return AllElements.CheckBox(elementId);
        }

        public virtual CheckBox CheckBox(Regex elementId)
        {
            return AllElements.CheckBox(elementId);
        }

        public virtual CheckBox CheckBox(Predicate<CheckBox> predicate)
        {
            return AllElements.CheckBox(predicate);
        }

        public virtual CheckBox CheckBox(Constraint findBy)
        {
            return AllElements.CheckBox(findBy);
        }

        public virtual CheckBoxCollection CheckBoxes
        {
            get { return AllElements.CheckBoxes; }
        }

        public virtual Element Element(string elementId)
        {
            return AllElements.Element(elementId);
        }

        public virtual Element Element(Regex elementId)
        {
            return AllElements.Element(elementId);
        }

        public virtual Element Element(Constraint findBy)
        {
            return AllElements.Element(findBy);
        }

        public virtual Element Element(Predicate<Element> predicate)
        {
            return AllElements.Element(predicate);
        }

        public virtual ElementCollection Elements
        {
            get { return AllElements.Elements; }
        }

        public virtual Element ElementWithTag(string tagName, Constraint findBy, params string[] inputTypes)
        {
            return AllElements.ElementWithTag(tagName, findBy, inputTypes);
        }

        public virtual ElementCollection ElementsWithTag(string tagName, params string[] inputTypes)
        {
            return AllElements.ElementsWithTag(tagName, inputTypes);
        }

	    public ElementCollection ElementsWithTag(IList<ElementTag> elementTags)
	    {
	        return AllElements.ElementsWithTag(elementTags);
	    }

        /// <inheritdoc />
        public virtual TElement ElementOfType<TElement>(string elementId) where TElement : Element
        {
            return AllElements.ElementOfType<TElement>(elementId);
        }

        /// <inheritdoc />
        public virtual TElement ElementOfType<TElement>(Regex elementId) where TElement : Element
        {
            return AllElements.ElementOfType<TElement>(elementId);
        }

        /// <inheritdoc />
        public virtual TElement ElementOfType<TElement>(Constraint findBy) where TElement : Element
        {
            return AllElements.ElementOfType<TElement>(findBy);
        }

        /// <inheritdoc />
        public virtual TElement ElementOfType<TElement>(Predicate<TElement> predicate) where TElement : Element
        {
            return AllElements.ElementOfType(predicate);
        }

        /// <inheritdoc />
        public virtual ElementCollection<TElement> ElementsOfType<TElement>() where TElement : Element
        {
            return AllElements.ElementsOfType<TElement>();
        }

        public virtual FileUpload FileUpload(string elementId)
        {
            return AllElements.FileUpload(elementId);
        }

        public virtual FileUpload FileUpload(Regex elementId)
        {
            return AllElements.FileUpload(elementId);
        }

        public virtual FileUpload FileUpload(Constraint findBy)
        {
            return AllElements.FileUpload(findBy);
        }

        public virtual FileUpload FileUpload(Predicate<FileUpload> predicate)
        {
            return AllElements.FileUpload(predicate);
        }

        public virtual FileUploadCollection FileUploads
        {
            get { return AllElements.FileUploads; }
        }

        public virtual Form Form(string elementId)
        {
            return AllElements.Form(elementId);
        }

        public virtual Form Form(Regex elementId)
        {
            return AllElements.Form(elementId);
        }

        public virtual Form Form(Constraint findBy)
        {
            return AllElements.Form(findBy);
        }

        public virtual Form Form(Predicate<Form> predicate)
        {
            return AllElements.Form(predicate);
        }

        public virtual FormCollection Forms
        {
            get { return AllElements.Forms; }
        }

        public virtual Label Label(string elementId)
        {
            return AllElements.Label(elementId);
        }

        public virtual Label Label(Regex elementId)
        {
            return AllElements.Label(elementId);
        }

        public virtual Label Label(Constraint findBy)
        {
            return AllElements.Label(findBy);
        }

        public virtual Label Label(Predicate<Label> predicate)
        {
            return AllElements.Label(predicate);
        }

        public virtual LabelCollection Labels
        {
            get { return AllElements.Labels; }
        }

        public virtual Link Link(string elementId)
        {
            return AllElements.Link(elementId);
        }

        public virtual Link Link(Regex elementId)
        {
            return AllElements.Link(elementId);
        }

        public virtual Link Link(Constraint findBy)
        {
            return AllElements.Link(findBy);
        }

        public virtual Link Link(Predicate<Link> predicate)
        {
            return AllElements.Link(predicate);
        }

        public virtual LinkCollection Links
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

	    public virtual Para Para(string elementId)
        {
            return AllElements.Para(elementId);
        }

        public virtual Para Para(Regex elementId)
        {
            return AllElements.Para(elementId);
        }

        public virtual Para Para(Constraint findBy)
        {
            return AllElements.Para(findBy);
        }

        public virtual Para Para(Predicate<Para> predicate)
        {
            return AllElements.Para(predicate);
        }

        public virtual ParaCollection Paras
        {
            get { return AllElements.Paras; }
        }

        public virtual RadioButton RadioButton(string elementId)
        {
            return AllElements.RadioButton(elementId);
        }

        public virtual RadioButton RadioButton(Regex elementId)
        {
            return AllElements.RadioButton(elementId);
        }

        public virtual RadioButton RadioButton(Constraint findBy)
        {
            return AllElements.RadioButton(findBy);
        }

        public virtual RadioButton RadioButton(Predicate<RadioButton> predicate)
        {
            return AllElements.RadioButton(predicate);
        }

        public virtual RadioButtonCollection RadioButtons
        {
            get { return AllElements.RadioButtons; }
        }

        public virtual SelectList SelectList(string elementId)
        {
            return AllElements.SelectList(elementId);
        }

        public virtual SelectList SelectList(Regex elementId)
        {
            return AllElements.SelectList(elementId);
        }

        public virtual SelectList SelectList(Constraint findBy)
        {
            return AllElements.SelectList(findBy);
        }

        public virtual SelectList SelectList(Predicate<SelectList> predicate)
        {
            return AllElements.SelectList(predicate);
        }

        public virtual SelectListCollection SelectLists
        {
            get { return AllElements.SelectLists; }
        }

        public virtual Table Table(string elementId)
        {
            return AllElements.Table(elementId);
        }

        public virtual Table Table(Regex elementId)
        {
            return AllElements.Table(elementId);
        }

        public virtual Table Table(Constraint findBy)
        {
            return AllElements.Table(findBy);
        }

        public virtual Table Table(Predicate<Table> predicate)
        {
            return AllElements.Table(predicate);
        }

        public virtual TableCollection Tables
        {
            get { return AllElements.Tables; }
        }

        public virtual TableBody TableBody(string elementId)
        {
            return AllElements.TableBody(elementId);
        }

        public virtual TableBody TableBody(Regex elementId)
        {
            return AllElements.TableBody(elementId);
        }

        public virtual TableBody TableBody(Constraint findBy)
        {
            return AllElements.TableBody(findBy);
        }

        public virtual TableBody TableBody(Predicate<TableBody> predicate)
        {
            return AllElements.TableBody(predicate);
        }

        public virtual TableBodyCollection TableBodies
        {
            get { return AllElements.TableBodies; }
        }

        public virtual TableCell TableCell(string elementId)
        {
            return AllElements.TableCell(elementId);
        }

        [Obsolete("Use TableCell(Find.By(elementId) & Find.ByIndex(index)) instead, or possibly OwnTableCell(...).")]
        public virtual TableCell TableCell(string elementId, int index)
        {
            return TableCell(Find.ById(elementId) & Find.ByIndex(index) & Find.Any);
        }

        [Obsolete("Use TableCell(Find.By(elementId) & Find.ByIndex(index)) instead, or possibly OwnTableCell(...).")]
        public virtual TableCell TableCell(Regex elementId, int index)
        {
            return TableCell(Find.ById(elementId) & Find.ByIndex(index));
        }

        public virtual TableCell TableCell(Regex elementId)
        {
            return AllElements.TableCell(elementId);
        }

        public virtual TableCell TableCell(Constraint findBy)
        {
            return AllElements.TableCell(findBy);
        }

        public virtual TableCell TableCell(Predicate<TableCell> predicate)
        {
            return AllElements.TableCell(predicate);
        }

        public virtual TableCellCollection TableCells
        {
            get { return AllElements.TableCells; }
        }

        public virtual TableRow TableRow(string elementId)
        {
            return AllElements.TableRow(elementId);
        }

        public virtual TableRow TableRow(Regex elementId)
        {
            return AllElements.TableRow(elementId);
        }

        public virtual TableRow TableRow(Constraint findBy)
        {
            return AllElements.TableRow(findBy);
        }

        public virtual TableRow TableRow(Predicate<TableRow> predicate)
        {
            return AllElements.TableRow(predicate);
        }

        public virtual TableRowCollection TableRows
        {
            get { return AllElements.TableRows; }
        }

        public virtual TextField TextField(string elementId)
        {
            return AllElements.TextField(elementId);
        }

        public virtual TextField TextField(Regex elementId)
        {
            return AllElements.TextField(elementId);
        }

        public virtual TextField TextField(Constraint findBy)
        {
            return AllElements.TextField(findBy);
        }

        public virtual TextField TextField(Predicate<TextField> predicate)
        {
            return AllElements.TextField(predicate);
        }

        public virtual TextFieldCollection TextFields
        {
            get { return AllElements.TextFields; }
        }

        public virtual Span Span(string elementId)
        {
            return AllElements.Span(elementId);
        }

        public virtual Span Span(Regex elementId)
        {
            return AllElements.Span(elementId);
        }

        public virtual Span Span(Constraint findBy)
        {
            return AllElements.Span(findBy);
        }

        public virtual Span Span(Predicate<Span> predicate)
        {
            return AllElements.Span(predicate);
        }

        public virtual SpanCollection Spans
        {
            get { return AllElements.Spans; }
        }

        public virtual Div Div(string elementId)
        {
            return AllElements.Div(elementId);
        }

        public virtual Div Div(Regex elementId)
        {
            return AllElements.Div(elementId);
        }

        public virtual Div Div(Constraint findBy)
        {
            return AllElements.Div(findBy);
        }

        public virtual Div Div(Predicate<Div> predicate)
        {
            return AllElements.Div(predicate);
        }

        public virtual DivCollection Divs
        {
            get { return AllElements.Divs; }
        }

        public virtual Image Image(string elementId)
        {
            return AllElements.Image(elementId);
        }

        public virtual Image Image(Regex elementId)
        {
            return AllElements.Image(elementId);
        }

        public virtual Image Image(Constraint findBy)
        {
            return AllElements.Image(findBy);
        }

        public virtual Image Image(Predicate<Image> predicate)
        {
            return AllElements.Image(predicate);
        }

        public virtual ImageCollection Images
        {
            get { return AllElements.Images; }
        }

        /// <inheritdoc />
        public virtual TControl Control<TControl>() where TControl : Control, new()
	    {
            return AllElements.Control<TControl>();
	    }

        /// <inheritdoc />
        public virtual TControl Control<TControl>(string elementId) where TControl : Control, new()
	    {
            return AllElements.Control<TControl>(elementId);
	    }

        /// <inheritdoc />
        public virtual TControl Control<TControl>(Regex elementId) where TControl : Control, new()
	    {
            return AllElements.Control<TControl>(elementId);
        }

        /// <inheritdoc />
        public virtual TControl Control<TControl>(Constraint findBy) where TControl : Control, new()
	    {
            return AllElements.Control<TControl>(findBy);
	    }

        /// <inheritdoc />
        public virtual TControl Control<TControl>(Predicate<TControl> predicate) where TControl : Control, new()
	    {
            return AllElements.Control(predicate);
	    }

        /// <inheritdoc />
        public virtual ControlCollection<TControl> Controls<TControl>() where TControl : Control, new()
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