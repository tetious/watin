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

namespace WatiN.Core.Interfaces
{
  /// <summary>
  /// This interface is used by all classes which provide access to (sub)elements.
  /// </summary>
  public interface IElementsContainer
  {
    Button Button(string elementId);
    Button Button(Regex elementId);
    Button Button(Attribute findBy);
    ButtonCollection Buttons { get; }

    CheckBox CheckBox(string elementId);
    CheckBox CheckBox(Regex elementId);
    CheckBox CheckBox(Attribute findBy);
    CheckBoxCollection CheckBoxes { get; }
    
    Element Element(string elementId);
    Element Element(Regex elementId);
    Element Element(Attribute findBy);
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