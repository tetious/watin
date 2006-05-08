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

using mshtml;

using WatiN.Logging;

namespace WatiN
{
  public class Table : ElementsContainer
  {
    public Table(DomContainer ie, HTMLTable htmlTable) : base(ie, (IHTMLElement) htmlTable)
    {}

    public override TableRowCollection TableRows
    {
      get {
        IHTMLElement firstTBody = (IHTMLElement)((HTMLTable)element).tBodies.item(0,null);
        return new TableRowCollection(Ie, (IHTMLElementCollection)(firstTBody.all)); }
    }

    /// <summary>
    /// Finds te first row that matches findText in inColumn. If no match is found, null is returned.
    /// </summary>
    /// <param name="findText">The text to find</param>
    /// <param name="inColumn">Index of the column to find the text in</param>
    /// <returns></returns>
    public TableRow FindRow(string findText, int inColumn)
    {
      Logger.LogAction("Searching for '" + findText + "' in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

      foreach (TableRow tableRow in TableRows)
      {
        TableCellCollection tableCells = tableRow.TableCells;

        if (tableCells[inColumn].Text.ToLower() == findText.ToLower())
        {
          return tableRow;
        }
      }
      return null;
    }

    public override string ToString()
    {
      return Id;
    }

  }
}