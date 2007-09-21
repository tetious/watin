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

using System.Text.RegularExpressions;
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
		/// Finds an image map area by an attribute.
		/// </summary>
		/// <param name="findBy">The attribute</param>
		/// <returns>The area</returns>
		Area Area(Attribute findBy);

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
    Button Button(Attribute findBy);

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
    CheckBox CheckBox(Attribute findBy);
    CheckBoxCollection CheckBoxes { get; }
    
    Element Element(string elementId);
    Element Element(Regex elementId);
    Element Element(Attribute findBy);
    Element Element(string tagname, Attribute findBy, params string[] inputtypes);
    ElementCollection Elements { get; }

    FileUpload FileUpload(string elementId);
    FileUpload FileUpload(Regex elementId);
    FileUpload FileUpload(Attribute findBy);
    FileUploadCollection FileUploads { get; }

    Form Form(string elementId);
    Form Form(Regex elementId);
    Form Form(Attribute findBy);
    FormCollection Forms { get; }

    Label Label(string elementId);
    Label Label(Regex elementId);
    Label Label(Attribute findBy);
    LabelCollection Labels { get; }

    Link Link(string elementId);
    Link Link(Regex elementId);
    Link Link(Attribute findBy);
    LinkCollection Links { get; }

    Para Para(string elementId);
    Para Para(Regex elementId);
    Para Para(Attribute findBy);
    ParaCollection Paras { get; }

    RadioButton RadioButton(string elementId);
    RadioButton RadioButton(Regex elementId);
    RadioButton RadioButton(Attribute findBy);
    RadioButtonCollection RadioButtons { get; }

    SelectList SelectList(string elementId);
    SelectList SelectList(Regex elementId);
    SelectList SelectList(Attribute findBy);
    SelectListCollection SelectLists { get; }

    Table Table(string elementId);
    Table Table(Regex elementId);
    Table Table(Attribute findBy);
    TableCollection Tables { get; }
//    TableSectionCollection TableSections { get; }

    TableCell TableCell(string elementId);
    TableCell TableCell(Regex elementId);
    TableCell TableCell(Attribute findBy);

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
    TableRow TableRow(Attribute findBy);
    TableRowCollection TableRows { get; }

    TableBody TableBody(string elementId);
    TableBody TableBody(Regex elementId);
    TableBody TableBody(Attribute findBy);
    TableBodyCollection TableBodies { get; }
    
    TextField TextField(string elementId);
    TextField TextField(Regex elementId);
    TextField TextField(Attribute findBy);
    TextFieldCollection TextFields { get; }

    Span Span(string elementId);
    Span Span(Regex elementId);
    Span Span(Attribute findBy);
    SpanCollection Spans { get; }

    Div Div(string elementId);
    Div Div(Regex elementId);
    Div Div(Attribute findBy);
    DivCollection Divs { get; }

    Image Image(string elementId);
    Image Image(Regex elementId);
    Image Image(Attribute findBy);
    ImageCollection Images { get; }
  }
}
