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

namespace WatiN
{
  public interface ISubElements
  {
    Button Button(string elementID);
    Button Button(AttributeValue findBy);
    ButtonCollection Buttons { get; }

    CheckBox CheckBox(string elementID);
    CheckBox CheckBox(AttributeValue findBy);
    CheckBoxCollection CheckBoxs { get; }

    Form Form(string elementID);
    Form Form(AttributeValue findBy);
    FormCollection Forms { get; }

    Label Label(string elementID);
    Label Label(AttributeValue findBy);
    LabelCollection Labels { get; }

    Link Link(string elementID);
    Link Link(AttributeValue findBy);
    LinkCollection Links { get; }

    Para Para(string elementID);
    Para Para(AttributeValue findBy);
    ParaCollection Paras { get; }

    RadioButton RadioButton(string elementID);
    RadioButton RadioButton(AttributeValue findBy);
    RadioButtonCollection RadioButtons { get; }

    SelectList SelectList(string elementID);
    SelectList SelectList(AttributeValue findBy);
    SelectListCollection SelectLists { get; }

    Table Table(string elementID);
    Table Table(AttributeValue findBy);
    TableCollection Tables { get; }
//    TableSectionCollection TableSections { get; }

    TableCell TableCell(string elementID);
    TableCell TableCell(AttributeValue findBy);
    TableCell TableCell(string elementId, int occurence);
    TableCellCollection TableCells { get; }

    TableRow TableRow(string elementID);
    TableRow TableRow(AttributeValue findBy);
    TableRowCollection TableRows { get; }
    
    TextField TextField(string elementID);
    TextField TextField(AttributeValue findBy);
    TextFieldCollection TextFields { get; }

    Span Span(string elementID);
    Span Span(AttributeValue findBy);
    SpanCollection Spans { get; }

    Div Div(string elementID);
    Div Div(AttributeValue findBy);
    DivCollection Divs { get; }

    Image Image(string elementID);
    Image Image(AttributeValue findBy);
    ImageCollection Images { get; }
  }
}