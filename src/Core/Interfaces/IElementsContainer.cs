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
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;

namespace WatiN.Core.Interfaces
{
	/// <summary>
	/// This interface is used by all classes which provide access to (sub)elements.
	/// </summary>
	public interface IElementsContainer
	{
		/// <summary>
		/// Finds an image map area by its id.
		/// </summary>
		/// <param name="elementId">The area id</param>
		/// <returns>The area</returns>
		Area Area(string elementId);

		/// <summary>
		/// Finds an image map area by its id using a regular expression.
		/// </summary>
		/// <param name="elementId">The area id regular expression</param>
		/// <returns>The area</returns>
		Area Area(Regex elementId);

		/// <summary>
		/// Finds an image map area by an BaseConstraint.
		/// </summary>
		/// <param name="findBy">The BaseConstraint</param>
		/// <returns>The area</returns>
		Area Area(BaseConstraint findBy);

#if !NET11
        Area Area(Predicate<Area> predicate);
#endif
		/// <summary>
		/// Gets the collection of areas.
		/// </summary>
		AreaCollection Areas { get; }

		/// <summary>
		/// Gets the specified Button by its id.
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
		Button Button(string elementId);

		Button Button(Regex elementId);

		/// <summary>
		/// Gets the specified Button by using the given <see cref="BaseConstraint" /> to find the Button.
		/// <seealso cref="Find" />
		/// </summary>
		/// <param name="findBy">The <see cref="BaseConstraint"/> class or one of it's subclasses to find an element by. The <see cref="Find" /> class provides factory methodes to create specialized instances.</param>
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
		Button Button(BaseConstraint findBy);

#if !NET11
		Button Button(Predicate<Button> predicate);
#endif
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
		ButtonCollection Buttons { get; }

		CheckBox CheckBox(string elementId);
		CheckBox CheckBox(Regex elementId);
		CheckBox CheckBox(BaseConstraint findBy);
#if !NET11
		CheckBox CheckBox(Predicate<CheckBox> predicate);
#endif
		CheckBoxCollection CheckBoxes { get; }

		Element Element(string elementId);
		Element Element(Regex elementId);
		Element Element(BaseConstraint findBy);
#if !NET11
		Element Element(Predicate<Element> predicate);
#endif
		Element Element(string tagname, BaseConstraint findBy, params string[] inputtypes);
		ElementCollection Elements { get; }

		FileUpload FileUpload(string elementId);
		FileUpload FileUpload(Regex elementId);
		FileUpload FileUpload(BaseConstraint findBy);
#if !NET11
		FileUpload FileUpload(Predicate<FileUpload> predicate);
#endif
        FileUploadCollection FileUploads { get; }

		Form Form(string elementId);
		Form Form(Regex elementId);
		Form Form(BaseConstraint findBy);
#if !NET11
		Form Form(Predicate<Form> predicate);
#endif
        FormCollection Forms { get; }

		Label Label(string elementId);
		Label Label(Regex elementId);
		Label Label(BaseConstraint findBy);
#if !NET11
		Label Label(Predicate<Label> predicate);
#endif
        LabelCollection Labels { get; }

		Link Link(string elementId);
		Link Link(Regex elementId);
		Link Link(BaseConstraint findBy);
#if !NET11
		Link Link(Predicate<Link> predicate);
#endif
        LinkCollection Links { get; }

		Para Para(string elementId);
		Para Para(Regex elementId);
		Para Para(BaseConstraint findBy);
#if !NET11
		Para Para(Predicate<Para> predicate);
#endif
        ParaCollection Paras { get; }

		RadioButton RadioButton(string elementId);
		RadioButton RadioButton(Regex elementId);
		RadioButton RadioButton(BaseConstraint findBy);
#if !NET11
		RadioButton RadioButton(Predicate<RadioButton> predicate);
#endif
        RadioButtonCollection RadioButtons { get; }

		SelectList SelectList(string elementId);
		SelectList SelectList(Regex elementId);
		SelectList SelectList(BaseConstraint findBy);
#if !NET11
		SelectList SelectList(Predicate<SelectList> predicate);
#endif
        SelectListCollection SelectLists { get; }

		Table Table(string elementId);
		Table Table(Regex elementId);
		Table Table(BaseConstraint findBy);
#if !NET11
		Table Table(Predicate<Table> predicate);
#endif
        TableCollection Tables { get; }

		TableCell TableCell(string elementId);
		TableCell TableCell(Regex elementId);
		TableCell TableCell(BaseConstraint findBy);
#if !NET11
        TableCell TableCell(Predicate<TableCell> predicate);
#endif
		/// <summary>
		/// Finds a TableCell by the n-th index of an id. 
		/// index counting is zero based.
		/// </summary>  
		/// <example>
		/// This example will get Text of the third(!) index on the page of a
		/// TableCell element with "tablecellid" as it's id value. 
		/// <code>ie.TableCell("tablecellid", 2).Text</code>
		/// </example>
		TableCell TableCell(string elementId, int index);

		TableCell TableCell(Regex elementId, int index);
		TableCellCollection TableCells { get; }

		TableRow TableRow(string elementId);
		TableRow TableRow(Regex elementId);
		TableRow TableRow(BaseConstraint findBy);
#if !NET11
        TableRow TableRow(Predicate<TableRow> predicate);
#endif
		TableRowCollection TableRows { get; }

		TableBody TableBody(string elementId);
		TableBody TableBody(Regex elementId);
		TableBody TableBody(BaseConstraint findBy);
#if !NET11
        TableBody TableBody(Predicate<TableBody> predicate);
#endif
		TableBodyCollection TableBodies { get; }

		TextField TextField(string elementId);
		TextField TextField(Regex elementId);
		TextField TextField(BaseConstraint findBy);
#if !NET11
	    TextField TextField(Predicate<TextField> predicate);
#endif
		TextFieldCollection TextFields { get; }

		Span Span(string elementId);
		Span Span(Regex elementId);
		Span Span(BaseConstraint findBy);
#if !NET11
        Span Span(Predicate<Span> predicate);
#endif
		SpanCollection Spans { get; }

		Div Div(string elementId);
		Div Div(Regex elementId);
		Div Div(BaseConstraint findBy);
#if !NET11
        Div Div(Predicate<Div> predicate);
#endif
		DivCollection Divs { get; }

		Image Image(string elementId);
		Image Image(Regex elementId);
		Image Image(BaseConstraint findBy);
#if !NET11
        Image Image(Predicate<Image> predicate);
#endif
		ImageCollection Images { get; }
	}
}