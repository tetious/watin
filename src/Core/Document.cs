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
			var innertext = Text;

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
                ExceptionMessage = () => string.Format("waiting {0} seconds for document to contain text '{1}'.", Settings.WaitUntilExistsTimeOut, text)
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
                ExceptionMessage = () => string.Format("waiting {0} seconds for document to contain regex '{1}'.", Settings.WaitUntilExistsTimeOut, regex)
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
			    if (activeElement == null) return null;
			    
                return ElementFactory.CreateElement(domContainer, activeElement);
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

        internal virtual NativeElementCollectionAdapter All
        {
            get { return new NativeElementCollectionAdapter(DomContainer, () => NativeDocument.AllElements); }
        }

        #region IElementsContainer

        public virtual Area Area(string elementId)
        {
            return All.Area(elementId);
        }

        public virtual Area Area(Regex elementId)
        {
            return All.Area(elementId);
        }

        public virtual Area Area(Constraint findBy)
        {
            return All.Area(findBy);
        }

        public virtual Area Area(Predicate<Area> predicate)
        {
            return All.Area(predicate);
        }

        public virtual AreaCollection Areas
        {
            get { return All.Areas; }
        }

        public virtual Button Button(string elementId)
        {
            return All.Button(elementId);
        }

        public virtual Button Button(Regex elementId)
        {
            return All.Button(elementId);
        }

        public virtual Button Button(Predicate<Button> predicate)
        {
            return All.Button(predicate);
        }

        public virtual Button Button(Constraint findBy)
        {
            return All.Button(findBy);
        }

        public virtual ButtonCollection Buttons
        {
            get { return All.Buttons; }
        }

        public virtual CheckBox CheckBox(string elementId)
        {
            return All.CheckBox(elementId);
        }

        public virtual CheckBox CheckBox(Regex elementId)
        {
            return All.CheckBox(elementId);
        }

        public virtual CheckBox CheckBox(Predicate<CheckBox> predicate)
        {
            return All.CheckBox(predicate);
        }

        public virtual CheckBox CheckBox(Constraint findBy)
        {
            return All.CheckBox(findBy);
        }

        public virtual CheckBoxCollection CheckBoxes
        {
            get { return All.CheckBoxes; }
        }

        public virtual Element Element(string elementId)
        {
            return All.Element(elementId);
        }

        public virtual Element Element(Regex elementId)
        {
            return All.Element(elementId);
        }

        public virtual Element Element(Constraint findBy)
        {
            return All.Element(findBy);
        }

        public virtual Element Element(Predicate<Element> predicate)
        {
            return All.Element(predicate);
        }

        public virtual ElementCollection Elements
        {
            get { return All.Elements; }
        }

        public virtual Element ElementWithTag(string tagName, Constraint findBy, params string[] inputTypes)
        {
            return All.ElementWithTag(tagName, findBy, inputTypes);
        }

        public virtual ElementCollection ElementsWithTag(string tagName, params string[] inputTypes)
        {
            return All.ElementsWithTag(tagName, inputTypes);
        }

	    public ElementCollection ElementsWithTag(IList<ElementTag> elementTags)
	    {
	        return All.ElementsWithTag(elementTags);
	    }

	    public virtual TElement ElementOfType<TElement>(string elementId) where TElement : Element
        {
            return All.ElementOfType<TElement>(elementId);
        }

        public virtual TElement ElementOfType<TElement>(Regex elementId) where TElement : Element
        {
            return All.ElementOfType<TElement>(elementId);
        }

        public virtual TElement ElementOfType<TElement>(Constraint findBy) where TElement : Element
        {
            return All.ElementOfType<TElement>(findBy);
        }

        public virtual TElement ElementOfType<TElement>(Predicate<TElement> predicate) where TElement : Element
        {
            return All.ElementOfType(predicate);
        }

        public virtual ElementCollection<TElement> ElementsOfType<TElement>() where TElement : Element
        {
            return All.ElementsOfType<TElement>();
        }

        public virtual FileUpload FileUpload(string elementId)
        {
            return All.FileUpload(elementId);
        }

        public virtual FileUpload FileUpload(Regex elementId)
        {
            return All.FileUpload(elementId);
        }

        public virtual FileUpload FileUpload(Constraint findBy)
        {
            return All.FileUpload(findBy);
        }

        public virtual FileUpload FileUpload(Predicate<FileUpload> predicate)
        {
            return All.FileUpload(predicate);
        }

        public virtual FileUploadCollection FileUploads
        {
            get { return All.FileUploads; }
        }

        public virtual Form Form(string elementId)
        {
            return All.Form(elementId);
        }

        public virtual Form Form(Regex elementId)
        {
            return All.Form(elementId);
        }

        public virtual Form Form(Constraint findBy)
        {
            return All.Form(findBy);
        }

        public virtual Form Form(Predicate<Form> predicate)
        {
            return All.Form(predicate);
        }

        public virtual FormCollection Forms
        {
            get { return All.Forms; }
        }

        public virtual Label Label(string elementId)
        {
            return All.Label(elementId);
        }

        public virtual Label Label(Regex elementId)
        {
            return All.Label(elementId);
        }

        public virtual Label Label(Constraint findBy)
        {
            return All.Label(findBy);
        }

        public virtual Label Label(Predicate<Label> predicate)
        {
            return All.Label(predicate);
        }

        public virtual LabelCollection Labels
        {
            get { return All.Labels; }
        }

        public virtual Link Link(string elementId)
        {
            return All.Link(elementId);
        }

        public virtual Link Link(Regex elementId)
        {
            return All.Link(elementId);
        }

        public virtual Link Link(Constraint findBy)
        {
            return All.Link(findBy);
        }

        public virtual Link Link(Predicate<Link> predicate)
        {
            return All.Link(predicate);
        }

        public virtual LinkCollection Links
        {
            get { return All.Links; }
        }

        public virtual Para Para(string elementId)
        {
            return All.Para(elementId);
        }

        public virtual Para Para(Regex elementId)
        {
            return All.Para(elementId);
        }

        public virtual Para Para(Constraint findBy)
        {
            return All.Para(findBy);
        }

        public virtual Para Para(Predicate<Para> predicate)
        {
            return All.Para(predicate);
        }

        public virtual ParaCollection Paras
        {
            get { return All.Paras; }
        }

        public virtual RadioButton RadioButton(string elementId)
        {
            return All.RadioButton(elementId);
        }

        public virtual RadioButton RadioButton(Regex elementId)
        {
            return All.RadioButton(elementId);
        }

        public virtual RadioButton RadioButton(Constraint findBy)
        {
            return All.RadioButton(findBy);
        }

        public virtual RadioButton RadioButton(Predicate<RadioButton> predicate)
        {
            return All.RadioButton(predicate);
        }

        public virtual RadioButtonCollection RadioButtons
        {
            get { return All.RadioButtons; }
        }

        public virtual SelectList SelectList(string elementId)
        {
            return All.SelectList(elementId);
        }

        public virtual SelectList SelectList(Regex elementId)
        {
            return All.SelectList(elementId);
        }

        public virtual SelectList SelectList(Constraint findBy)
        {
            return All.SelectList(findBy);
        }

        public virtual SelectList SelectList(Predicate<SelectList> predicate)
        {
            return All.SelectList(predicate);
        }

        public virtual SelectListCollection SelectLists
        {
            get { return All.SelectLists; }
        }

        public virtual Table Table(string elementId)
        {
            return All.Table(elementId);
        }

        public virtual Table Table(Regex elementId)
        {
            return All.Table(elementId);
        }

        public virtual Table Table(Constraint findBy)
        {
            return All.Table(findBy);
        }

        public virtual Table Table(Predicate<Table> predicate)
        {
            return All.Table(predicate);
        }

        public virtual TableCollection Tables
        {
            get { return All.Tables; }
        }

        public virtual TableBody TableBody(string elementId)
        {
            return All.TableBody(elementId);
        }

        public virtual TableBody TableBody(Regex elementId)
        {
            return All.TableBody(elementId);
        }

        public virtual TableBody TableBody(Constraint findBy)
        {
            return All.TableBody(findBy);
        }

        public virtual TableBody TableBody(Predicate<TableBody> predicate)
        {
            return All.TableBody(predicate);
        }

        public virtual TableBodyCollection TableBodies
        {
            get { return All.TableBodies; }
        }

        public virtual TableCell TableCell(string elementId)
        {
            return All.TableCell(elementId);
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
            return All.TableCell(elementId);
        }

        public virtual TableCell TableCell(Constraint findBy)
        {
            return All.TableCell(findBy);
        }

        public virtual TableCell TableCell(Predicate<TableCell> predicate)
        {
            return All.TableCell(predicate);
        }

        public virtual TableCellCollection TableCells
        {
            get { return All.TableCells; }
        }

        public virtual TableRow TableRow(string elementId)
        {
            return All.TableRow(elementId);
        }

        public virtual TableRow TableRow(Regex elementId)
        {
            return All.TableRow(elementId);
        }

        public virtual TableRow TableRow(Constraint findBy)
        {
            return All.TableRow(findBy);
        }

        public virtual TableRow TableRow(Predicate<TableRow> predicate)
        {
            return All.TableRow(predicate);
        }

        public virtual TableRowCollection TableRows
        {
            get { return All.TableRows; }
        }

        public virtual TextField TextField(string elementId)
        {
            return All.TextField(elementId);
        }

        public virtual TextField TextField(Regex elementId)
        {
            return All.TextField(elementId);
        }

        public virtual TextField TextField(Constraint findBy)
        {
            return All.TextField(findBy);
        }

        public virtual TextField TextField(Predicate<TextField> predicate)
        {
            return All.TextField(predicate);
        }

        public virtual TextFieldCollection TextFields
        {
            get { return All.TextFields; }
        }

        public virtual Span Span(string elementId)
        {
            return All.Span(elementId);
        }

        public virtual Span Span(Regex elementId)
        {
            return All.Span(elementId);
        }

        public virtual Span Span(Constraint findBy)
        {
            return All.Span(findBy);
        }

        public virtual Span Span(Predicate<Span> predicate)
        {
            return All.Span(predicate);
        }

        public virtual SpanCollection Spans
        {
            get { return All.Spans; }
        }

        public virtual Div Div(string elementId)
        {
            return All.Div(elementId);
        }

        public virtual Div Div(Regex elementId)
        {
            return All.Div(elementId);
        }

        public virtual Div Div(Constraint findBy)
        {
            return All.Div(findBy);
        }

        public virtual Div Div(Predicate<Div> predicate)
        {
            return All.Div(predicate);
        }

        public virtual DivCollection Divs
        {
            get { return All.Divs; }
        }

        public virtual Image Image(string elementId)
        {
            return All.Image(elementId);
        }

        public virtual Image Image(Regex elementId)
        {
            return All.Image(elementId);
        }

        public virtual Image Image(Constraint findBy)
        {
            return All.Image(findBy);
        }

        public virtual Image Image(Predicate<Image> predicate)
        {
            return All.Image(predicate);
        }

        public virtual ImageCollection Images
        {
            get { return All.Images; }
        }

        public virtual TControl Control<TControl>() where TControl : Control, new()
	    {
            return All.Control<TControl>();
	    }

        public virtual TControl Control<TControl>(string elementId) where TControl : Control, new()
	    {
            return All.Control<TControl>(elementId);
	    }

        public virtual TControl Control<TControl>(Regex elementId) where TControl : Control, new()
	    {
            return All.Control<TControl>(elementId);
        }

        public virtual TControl Control<TControl>(Constraint findBy) where TControl : Control, new()
	    {
            return All.Control<TControl>(findBy);
	    }

        public virtual TControl Control<TControl>(Predicate<TControl> predicate) where TControl : Control, new()
	    {
            return All.Control(predicate);
	    }

        public virtual ControlCollection<TControl> Controls<TControl>() where TControl : Control, new()
	    {
            return All.Controls<TControl>();
	    }

	    #endregion
    }
}