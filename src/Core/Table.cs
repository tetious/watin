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

using System.Text.RegularExpressions;
using mshtml;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core
{
  /// <summary>
  /// This class provides specialized functionality for a HTML table element.
  /// </summary>
  public class Table : ElementsContainer
  {
    public Table(DomContainer ie, HTMLTable htmlTable) : base(ie, (IHTMLElement) htmlTable)
    {}

    public override TableRowCollection TableRows
    {
      get 
      {
        IHTMLElementCollection bodyElements = GetBodyElements();
        return ElementsSupport.TableRows(DomContainer, bodyElements); 
      }
    }

    private IHTMLElementCollection GetBodyElements()
    {
      return (IHTMLElementCollection)(GetFirstTBody().all);
    }

    private IHTMLElement GetFirstTBody()
    {
      return (IHTMLElement)((IHTMLTable)DomElement).tBodies.item(0,null);
    }

    /// <summary>
    /// Finds te first row that matches findText in inColumn. If no match is found, null is returned.
    /// </summary>
    /// <param name="findText">The text to find.</param>
    /// <param name="inColumn">Index of the column to find the text in.</param>
    /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
    public TableRow FindRow(string findText, int inColumn)
    {
      Logger.LogAction("Searching for '" + findText + "' in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

      string innertext = GetFirstTBody().innerText.ToLower();
      
      if (innertext != null && innertext.IndexOf(findText.ToLower()) >= 0)
      {
        return FindRow(new TableRowFinder(findText, inColumn));
      }
      
      return null;
    }

    /// <summary>
    /// Finds te first row that matches findTextRegex in inColumn. If no match is found, null is returned.
    /// </summary>
    /// <param name="findTextRegex">The regular expression the cell text must match.</param>
    /// <param name="inColumn">Index of the column to find the text in.</param>
    /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
    public TableRow FindRow(Regex findTextRegex, int inColumn)
    {
      Logger.LogAction("Matching regular expression'" + findTextRegex + "' with text in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

      string innertext = GetFirstTBody().innerText;
      
      if (innertext != null)
      {
        return FindRow(new TableRowFinder(findTextRegex, inColumn));
      }
      
      return null;
    }

    public override string ToString()
    {
      return Id;
    }
    
    public TableRow FindRow(TableRowFinder findBy)
    {
      IHTMLElementCollection bodyElements = GetBodyElements();
      IHTMLElement element = ElementsSupport.FindFirstElement(ElementsSupport.TableRowTagName, ElementsSupport.InputNullType, findBy, bodyElements, false);
      
      if (element != null)
      {
        return new TableRow(DomContainer,(HTMLTableRow)element);
      }

      return null;
    }
    
    private class TextEqualsAndCaseInsensitive : Text
    {
      public   TextEqualsAndCaseInsensitive(string text) : base(text)
      {
        comparer = new StringEqualsAndCaseInsensitiveComparer(text);
      }
    }
    
    private class TextContainsAndCaseInsensitive : Text
    {
      public   TextContainsAndCaseInsensitive(string text) : base(text)
      {
        comparer = new StringContainsAndCaseInsensitiveComparer(text);
      }
    }

    private class TextAlwaysTrue : Text
    {
      public   TextAlwaysTrue() : base("")
      {
        comparer = new AlwaysTrueComparer();
      }

      private class AlwaysTrueComparer : ICompare
      {
        public bool Compare(string value)
        {
          return true;
        }
      }
    }
    
    /// <summary>
    /// Use this class to find a row which contains a particular value
    /// in a table cell contained in a table column.
    /// </summary>
    public class TableRowFinder : Attribute
    {
      private int columnIndex;
      private Text findByText;
      private Text containsText;
      
      /// <summary>
      /// Initializes a new instance of the <see cref="TableRowFinder"/> class.
      /// </summary>
      /// <param name="findText">The text to find (exact match but case insensitive).</param>
      /// <param name="inColumn">The column index in which to look for the value.</param>
      public TableRowFinder(string findText, int inColumn): base("noattribute","")
      {
        columnIndex = inColumn;
        findByText = new TextEqualsAndCaseInsensitive(findText);
        containsText = new TextContainsAndCaseInsensitive(findText);
      }
      
      /// <summary>
      /// Initializes a new instance of the <see cref="TableRowFinder"/> class.
      /// </summary>
      /// <param name="findTextRegex">The regular expression to match with.</param>
      /// <param name="inColumn">The column index in which to look for the value.</param>
      public TableRowFinder(Regex findTextRegex, int inColumn): base("noattribute","")
      {
        columnIndex = inColumn;
        findByText = new Text(findTextRegex);
        containsText = new TextAlwaysTrue();
      }
      
      /// <summary>
      /// Compares the given <paramref name = "value" /> with the regex or
      /// text this class was constructed with. This should be a table cell elements
      /// innertext.
      /// </summary>
      /// <param name="value">Value to compare with</param>
      /// <returns>
      /// 	<c>true</c> if the searched for value equals the given value
      /// </returns>
      public override bool Compare(string value)
      {
        return findByText.Compare(value);
      }
 
      public override bool Compare(object ihtmlelement)
      {
        IHTMLElement element = GetIHTMLElement(ihtmlelement);

        if (containsText.Compare(element.innerText))
        {
          // Get all elements and filter this for TableCells
          IHTMLElementCollection allElements = (IHTMLElementCollection)element.all;
          IHTMLElementCollection tableCellElements = ElementsSupport.getElementCollection(allElements, ElementsSupport.TableCellTagName);
        
          return findByText.Compare(tableCellElements.item(columnIndex, null));
        }
        
        return false;
      }
    }
  }
}