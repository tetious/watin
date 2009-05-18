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

namespace WatiN.Core
{
    /// <summary>
    /// This interface is used by all classes which provide access to (sub)elements.
    /// </summary>
    public interface IElementContainer
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
        /// Finds an image map area by an Constraint.
        /// </summary>
        /// <param name="findBy">The Constraint</param>
        /// <returns>The area</returns>
        Area Area(Constraint findBy);

        Area Area(Predicate<Area> predicate);

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
        /// Gets the specified Button by using the given <see cref="Constraint" /> to find the Button.
        /// <seealso cref="Find" />
        /// </summary>
        /// <param name="findBy">The <see cref="Constraint"/> class or one of it's subclasses to find an element by. The <see cref="Find" /> class provides factory methodes to create specialized instances.</param>
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
        Button Button(Constraint findBy);

        Button Button(Predicate<Button> predicate);

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
        CheckBox CheckBox(Constraint findBy);
        CheckBox CheckBox(Predicate<CheckBox> predicate);
        CheckBoxCollection CheckBoxes { get; }

        Element Element(string elementId);
        Element Element(Regex elementId);
        Element Element(Constraint findBy);
        Element Element(Predicate<Element> predicate);
        ElementCollection Elements { get; }

        Element ElementWithTag(string tagName, Constraint findBy, params string[] inputTypes);
        ElementCollection ElementsWithTag(string tagName, params string[] inputTypes);
        ElementCollection ElementsWithTag(IList<ElementTag> elementTags);

        FileUpload FileUpload(string elementId);
        FileUpload FileUpload(Regex elementId);
        FileUpload FileUpload(Constraint findBy);
        FileUpload FileUpload(Predicate<FileUpload> predicate);
        FileUploadCollection FileUploads { get; }

        Form Form(string elementId);
        Form Form(Regex elementId);
        Form Form(Constraint findBy);
        Form Form(Predicate<Form> predicate);
        FormCollection Forms { get; }

        Label Label(string elementId);
        Label Label(Regex elementId);
        Label Label(Constraint findBy);
        Label Label(Predicate<Label> predicate);
        LabelCollection Labels { get; }

        Link Link(string elementId);
        Link Link(Regex elementId);
        Link Link(Constraint findBy);
        Link Link(Predicate<Link> predicate);
        LinkCollection Links { get; }

        Para Para(string elementId);
        Para Para(Regex elementId);
        Para Para(Constraint findBy);
        Para Para(Predicate<Para> predicate);
        ParaCollection Paras { get; }

        RadioButton RadioButton(string elementId);
        RadioButton RadioButton(Regex elementId);
        RadioButton RadioButton(Constraint findBy);
        RadioButton RadioButton(Predicate<RadioButton> predicate);
        RadioButtonCollection RadioButtons { get; }

        SelectList SelectList(string elementId);
        SelectList SelectList(Regex elementId);
        SelectList SelectList(Constraint findBy);
        SelectList SelectList(Predicate<SelectList> predicate);
        SelectListCollection SelectLists { get; }

        Table Table(string elementId);
        Table Table(Regex elementId);
        Table Table(Constraint findBy);
        Table Table(Predicate<Table> predicate);
        TableCollection Tables { get; }

        TableCell TableCell(string elementId);
        TableCell TableCell(Regex elementId);
        TableCell TableCell(Constraint findBy);
        TableCell TableCell(Predicate<TableCell> predicate);
        TableCellCollection TableCells { get; }

        TableRow TableRow(string elementId);
        TableRow TableRow(Regex elementId);
        TableRow TableRow(Constraint findBy);
        TableRow TableRow(Predicate<TableRow> predicate);
        TableRowCollection TableRows { get; }

        TableBody TableBody(string elementId);
        TableBody TableBody(Regex elementId);
        TableBody TableBody(Constraint findBy);
        TableBody TableBody(Predicate<TableBody> predicate);
        TableBodyCollection TableBodies { get; }

        TextField TextField(string elementId);
        TextField TextField(Regex elementId);
        TextField TextField(Constraint findBy);
        TextField TextField(Predicate<TextField> predicate);
        TextFieldCollection TextFields { get; }

        Span Span(string elementId);
        Span Span(Regex elementId);
        Span Span(Constraint findBy);
        Span Span(Predicate<Span> predicate);
        SpanCollection Spans { get; }

        Div Div(string elementId);
        Div Div(Regex elementId);
        Div Div(Constraint findBy);
        Div Div(Predicate<Div> predicate);
        DivCollection Divs { get; }

        Image Image(string elementId);
        Image Image(Regex elementId);
        Image Image(Constraint findBy);
        Image Image(Predicate<Image> predicate);
        ImageCollection Images { get; }

        /// <summary>
        /// ElementOfTypes an element of the desired type with the specified id.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ordinarily you should call the element-type specific method such as <see cref="Div(string)"/>.
        /// This generic method is intended to be used in situations where the type of the element
        /// may vary and is specified by a type parameter in the calling code.
        /// </para>
        /// </remarks>
        /// <typeparam name="TElement">The element type</typeparam>
        /// <param name="elementId">The element id to match</param>
        /// <returns>The element</returns>
        /// <example>
        /// <code>
        /// ie.ElementOfType&lt;Div&gt;("id");
        /// </code>
        /// </example>
        TElement ElementOfType<TElement>(string elementId)
            where TElement : Element;

        /// <summary>
        /// ElementOfTypes an element of the desired type with an id that matches the specified regular expression.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ordinarily you should call the element-type specific method such as <see cref="Div(Regex)"/>.
        /// This generic method is intended to be used in situations where the type of the element
        /// may vary and is specified by a type parameter in the calling code.
        /// </para>
        /// </remarks>
        /// <typeparam name="TElement">The element type</typeparam>
        /// <param name="elementId">The element id regular expression to match</param>
        /// <returns>The element</returns>
        /// <example>
        /// <code>
        /// ie.ElementOfType&lt;Div&gt;(new Regex("id"));
        /// </code>
        /// </example>
        TElement ElementOfType<TElement>(Regex elementId)
            where TElement : Element;

        /// <summary>
        /// ElementOfTypes an element of the desired type that matches the specified <see cref="Constraint" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ordinarily you should call the element-type specific method such as <see cref="Div(Constraint)"/>.
        /// This generic method is intended to be used in situations where the type of the element
        /// may vary and is specified by a type parameter in the calling code.
        /// </para>
        /// </remarks>
        /// <typeparam name="TElement">The element type</typeparam>
        /// <param name="findBy">The constraint to match</param>
        /// <returns>The element</returns>
        /// <example>
        /// <code>
        /// ie.ElementOfType&lt;Div&gt;(Find.ById("id"));
        /// </code>
        /// </example>
        TElement ElementOfType<TElement>(Constraint findBy)
            where TElement : Element;

        /// <summary>
        /// ElementOfTypes an element of the desired type that matches the specified <see cref="Predicate{T}" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ordinarily you should call the element-type specific method such as <see cref="Div(Predicate{Div})"/>.
        /// This generic method is intended to be used in situations where the type of the element
        /// may vary and is specified by a type parameter in the calling code.
        /// </para>
        /// </remarks>
        /// <typeparam name="TElement">The element type</typeparam>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>The element</returns>
        /// <example>
        /// <code>
        /// ie.ElementOfType&lt;Div&gt;(div => div.Id == "id");
        /// </code>
        /// </example>
        TElement ElementOfType<TElement>(Predicate<TElement> predicate)
            where TElement : Element;

        /// <summary>
        /// Gets a collection of all elements of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ordinarily you should call the element-type specific method such as <see cref="Divs"/>.
        /// This generic method is intended to be used in situations where the type of the element
        /// may vary and is specified by a type parameter in the calling code.
        /// </para>
        /// </remarks>
        /// <typeparam name="TElement">The element type</typeparam>
        /// <returns>The element collection</returns>
        /// <example>
        /// <code>
        /// ie.ElementsOfType&lt;Div&gt;();
        /// </code>
        /// </example>
        ElementCollection<TElement> ElementsOfType<TElement>()
            where TElement : Element;

        /// <summary>
        /// Gets a control object of the desired type that appears first within this element container.
        /// </summary>
        /// <typeparam name="TControl">The <see cref="Core.Control{TElement}" /> subclass</typeparam>
        /// <returns>The control object</returns>
        /// <example>
        /// <code>
        /// ie.Control&lt;Header&gt;().MyAccountTab.Click();
        /// </code>
        /// </example>
        TControl Control<TControl>() where TControl : Control, new();

        /// <summary>
        /// Gets a control object of the desired type with the specified id.
        /// </summary>
        /// <param name="elementId">The element id to match</param>
        /// <typeparam name="TControl">The <see cref="Core.Control{TElement}" /> subclass</typeparam>
        /// <returns>The control object</returns>
        /// <example>
        /// <code>
        /// ie.Control&lt;CalendarControl&gt;("fromDateCalendar").SetDate(DateTime.Date);
        /// </code>
        /// </example>
        TControl Control<TControl>(string elementId) where TControl : Control, new();

        /// <summary>
        /// Gets a control object of the desired type with an id that matches the specified regular expression.
        /// </summary>
        /// <param name="elementId">The element id regular expression to match</param>
        /// <typeparam name="TControl">The <see cref="Core.Control{TElement}" /> subclass</typeparam>
        /// <returns>The control object</returns>
        /// <example>
        /// <code>
        /// ie.Control&lt;CalendarControl&gt;("fromDateCalendar").SetDate(DateTime.Date);
        /// </code>
        /// </example>
        TControl Control<TControl>(Regex elementId) where TControl : Control, new();

        /// <summary>
        /// Gets a control object of the desired type that matches the specified <see cref="Constraint" />.
        /// </summary>
        /// <param name="findBy">The constraint to match</param>
        /// <typeparam name="TControl">The <see cref="Core.Control{TElement}" /> subclass</typeparam>
        /// <returns>The control object</returns>
        /// <example>
        /// <code>
        /// ie.Control&lt;CalendarControl&gt;(Find.ById("fromDateCalendar")).SetDate(DateTime.Date);
        /// </code>
        /// </example>
        TControl Control<TControl>(Constraint findBy) where TControl : Control, new();

        /// <summary>
        /// Gets a control object of the desired type that matches the specified <see cref="Predicate{T}" />.
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <typeparam name="TControl">The <see cref="Core.Control{TElement}" /> subclass</typeparam>
        /// <returns>The control</returns>
        /// <example>
        /// <code>
        /// ie.Control&lt;CalendarControl&gt;(control => control.Name == "SomeName").SetDate(DateTime.Date);
        /// </code>
        /// </example>
        TControl Control<TControl>(Predicate<TControl> predicate) where TControl : Control, new();

        /// <summary>
        /// Gets a collection of all controls of the desired type.
        /// </summary>
        /// <typeparam name="TControl">The <see cref="Core.Control{TElement}" /> subclass</typeparam>
        /// <returns>The control collection</returns>
        /// <example>
        /// <code>
        /// ie.Control&lt;CalendarControl&gt;(control => control.Name == "SomeName").SetDate(DateTime.Date);
        /// </code>
        /// </example>
        ControlCollection<TControl> Controls<TControl>() where TControl : Control, new();
    }
}