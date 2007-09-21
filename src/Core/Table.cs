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

using System.Collections;
using System.Text.RegularExpressions;
using mshtml;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core
{
  using System;

  /// <summary>
  /// This class provides specialized functionality for a HTML table element.
  /// </summary>
  public class Table : ElementsContainer
  {
    private static ArrayList elementTags;

    public static ArrayList ElementTags
    {
      get
      {
        if (elementTags == null)
        {
          elementTags = new ArrayList();
          elementTags.Add(new ElementTag("table"));
        }

        return elementTags;
      }
    }

    public Table(DomContainer ie, IHTMLTable htmlTable) : base(ie, (IHTMLElement) htmlTable)
    {}
    
    public Table(DomContainer ie, ElementFinder finder) : base(ie, finder)
    {}

    /// <summary>
    /// Initialises a new instance of the <see cref="Table"/> class based on <paramref name="element"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    public Table(Element element) : base(element, ElementTags)
    {}

	/// <summary>
	/// Returns all rows in the first TBODY section of a table. If no
	/// explicit sections are defined in the table (like THEAD, TBODY 
	/// and/or TFOOT sections), it will return all the rows in the table.
	/// This method also returns rows from nested tables.
	/// </summary>
	/// <value>The table rows.</value>
    public override TableRowCollection TableRows
    {
      get 
      {
        return ElementsSupport.TableRows(DomContainer, TableBodies[0]); 
      }
    }

    /// <summary>
    /// Returns the table body sections belonging to this table (not including table body sections 
    /// from tables nested in this table).
    /// </summary>
    /// <value>The table bodies.</value>
    public override TableBodyCollection TableBodies
    {
      get
      {
        return new TableBodyCollection(DomContainer, UtilityClass.IHtmlElementCollectionToArrayList(HTMLTable.tBodies));
      }
    }

    /// <summary>
    /// Returns the table body section belonging to this table (not including table body sections 
    /// from tables nested in this table).
    /// </summary>
    /// <param name="findBy">The find by.</param>
    /// <returns></returns>
    public override TableBody TableBody(AttributeConstraint findBy)
    {
      return ElementsSupport.TableBody(DomContainer, findBy, new TBodies(this) );
    }

    private IHTMLElement GetFirstTBody()
    {
      return (IHTMLElement)HTMLTable.tBodies.item(0,null);
    }

    private IHTMLTable HTMLTable
    {
      get { return (IHTMLTable) HTMLElement; }
    }

    /// <summary>
    /// Finds te first row that matches findText in inColumn defined as a TD html element.
    /// If no match is found, null is returned.
    /// </summary>
    /// <param name="findText">The text to find.</param>
    /// <param name="inColumn">Index of the column to find the text in.</param>
    /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
    public TableRow FindRow(string findText, int inColumn)
    {
      Logger.LogAction("Searching for '" + findText + "' in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

      TableRowFinder finder = new TableRowFinder(findText, inColumn);
      
      return findRow(finder);
    }

    /// <summary>
    /// Finds te first row that matches findTextRegex in inColumn defined as a TD html element.
    /// If no match is found, null is returned.
    /// </summary>
    /// <param name="findTextRegex">The regular expression the cell text must match.</param>
    /// <param name="inColumn">Index of the column to find the text in.</param>
    /// <returns>The searched for <see cref="TableRow"/>; otherwise <c>null</c>.</returns>
    public TableRow FindRow(Regex findTextRegex, int inColumn)
    {
      Logger.LogAction("Matching regular expression'" + findTextRegex + "' with text in column " + inColumn + " of " + GetType().Name + " '" + Id + "'");

      TableRowFinder finder = new TableRowFinder(findTextRegex, inColumn);

      return FindRow(finder);
    }

    private TableRow findRow(TableRowFinder finder)
    {
      string innertext = GetFirstTBody().innerText;
      
      if (innertext != null && finder.IsTextContainedIn(innertext))
      {
        return FindRow(finder);
      }
      
      return null;
    }

    public override string ToString()
    {
      return Id;
    }
    
    public TableRow FindRow(TableRowFinder findBy)
    {
      TableRow row = ElementsSupport.TableRow(DomContainer, findBy, new ElementsInFirstTBody(this));
      
      if (row.Exists)
      {
        return row;
      }

      return null;
    }

    public abstract class TableElementCollectionsBase : IElementCollection
    {
      protected Table table;

      public TableElementCollectionsBase(Table table)
      {
        this.table = table;
      }

      public abstract IHTMLElementCollection Elements { get; }
    }

    public class TBodies : TableElementCollectionsBase
    {
      public TBodies(Table table) : base(table) {}

      public override IHTMLElementCollection Elements
      {
        get
        {
          return table.HTMLTable.tBodies;
        }
      }
    }

    public class ElementsInFirstTBody : TableElementCollectionsBase
    {
      public ElementsInFirstTBody(Table table): base(table) {}

      public override IHTMLElementCollection Elements
      {
        get
        {
          return (IHTMLElementCollection)table.GetFirstTBody().all;
        }
      }
    }
  }

  /// <summary>
  /// Use this class to find a row which contains a particular value
  /// in a table cell contained in a table column.
  /// </summary>
  public class TableRowFinder : Text
  {
    private int columnIndex;
    private ICompare containsText;
      
    /// <summary>
    /// Initializes a new instance of the <see cref="TableRowFinder"/> class.
    /// </summary>
    /// <param name="findText">The text to find (exact match but case insensitive).</param>
    /// <param name="inColumn">The column index in which to look for the value.</param>
    public TableRowFinder(string findText, int inColumn): base(new StringEqualsAndCaseInsensitiveComparer(findText))
    {
      columnIndex = inColumn;
      containsText = new StringContainsAndCaseInsensitiveComparer(findText);
    }
      
    /// <summary>
    /// Initializes a new instance of the <see cref="TableRowFinder"/> class.
    /// </summary>
    /// <param name="findTextRegex">The regular expression to match with.</param>
    /// <param name="inColumn">The column index in which to look for the value.</param>
    public TableRowFinder(Regex findTextRegex, int inColumn): base(findTextRegex)
    {
      columnIndex = inColumn;
      containsText = new AlwaysTrueComparer();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableRowFinder"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    /// <param name="inColumn">The column index in which to look for the value.</param>
    public TableRowFinder(ICompare comparer, int inColumn) : base(comparer)
    {
      columnIndex = inColumn;
      containsText = new AlwaysTrueComparer();
    }
  
    public override bool Compare(IAttributeBag attributeBag)
    {
      IHTMLElement element = ((ElementAttributeBag)attributeBag).IHTMLElement;

      if (IsTextContainedIn(element.innerText))
      {
        // Get all elements and filter this for TableCells
        IHTMLElementCollection allElements = (IHTMLElementCollection)element.all;
        IHTMLElementCollection tableCellElements = (IHTMLElementCollection)allElements.tags(ElementsSupport.TableCellTagName);
        
        if (tableCellElements.length - 1 >= columnIndex)
        {
          IHTMLElement tableCell = (IHTMLElement)tableCellElements.item(columnIndex, null);
          return base.Compare(new ElementAttributeBag(tableCell));
        }
      }
        
      return false;
    }

    public bool IsTextContainedIn(string text)
    {
      return containsText.Compare(text);
    }
  }
}
