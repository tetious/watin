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

namespace WatiN.Core.Interfaces
{
  public interface ISubElements
  {
    Button Button(string elementId);
    Button Button(Attribute findBy);
    ButtonCollection Buttons { get; }

    CheckBox CheckBox(string elementId);
    CheckBox CheckBox(Attribute findBy);
    CheckBoxCollection CheckBoxes { get; }

    Form Form(string elementId);
    Form Form(Attribute findBy);
    FormCollection Forms { get; }

    Label Label(string elementId);
    Label Label(Attribute findBy);
    LabelCollection Labels { get; }

    Link Link(string elementId);
    Link Link(Attribute findBy);
    LinkCollection Links { get; }

    Para Para(string elementId);
    Para Para(Attribute findBy);
    ParaCollection Paras { get; }

    RadioButton RadioButton(string elementId);
    RadioButton RadioButton(Attribute findBy);
    RadioButtonCollection RadioButtons { get; }

    SelectList SelectList(string elementId);
    SelectList SelectList(Attribute findBy);
    SelectListCollection SelectLists { get; }

    Table Table(string elementId);
    Table Table(Attribute findBy);
    TableCollection Tables { get; }
//    TableSectionCollection TableSections { get; }

    TableCell TableCell(string elementId);
    TableCell TableCell(Attribute findBy);
    TableCell TableCell(string elementId, int occurrence);
    TableCellCollection TableCells { get; }

    TableRow TableRow(string elementId);
    TableRow TableRow(Attribute findBy);
    TableRowCollection TableRows { get; }
    
    TextField TextField(string elementId);
    TextField TextField(Attribute findBy);
    TextFieldCollection TextFields { get; }

    Span Span(string elementId);
    Span Span(Attribute findBy);
    SpanCollection Spans { get; }

    Div Div(string elementId);
    Div Div(Attribute findBy);
    DivCollection Divs { get; }

    Image Image(string elementId);
    Image Image(Attribute findBy);
    ImageCollection Images { get; }
  }
}