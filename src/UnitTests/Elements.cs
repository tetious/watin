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

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using NUnit.Framework;

using WatiN.Core;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.UnitTests
{
  using mshtml;
  using Rhino.Mocks;

  [TestFixture]
  public class Elements : WatiNTest
  {
    private IE ie;
    const string tableId = "table1";
    private Settings defaultSettings;
    
    [TestFixtureSetUp]
    public void FixtureSetup()
    {
      defaultSettings = IE.Settings.Clone();
      ie = new IE(MainURI);
    }

    [TestFixtureTearDown]
    public void FixtureTearDown()
    {
      ie.Close();
    }

    [SetUp]
    public void TestSetUp()
    {
      IE.Settings = defaultSettings.Clone();
      if(!ie.Uri.Equals(MainURI))
      {
        ie.GoTo(MainURI);
      }
    }
    
    [Test]
    public void IEIsIE()
    {
      Assert.IsInstanceOfType(typeof(IE), ie);
    }

    [Test]
    public void IEIsDomContainer()
    {
      Assert.IsInstanceOfType(typeof(DomContainer), ie); 
    }
    
    [Test]
    public void DomContainerIsDocument()
    {
      Assert.IsInstanceOfType(typeof(Document), ie); 
    }   
    
    [Test]
    public void DocumentIsISubElement()
    {
      Assert.IsInstanceOfType(typeof(IElementsContainer), ie); 
    }   

    [Test]
    public void TableElementTags()
    {
      Assert.AreEqual(1, Table.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("table",((ElementTag) Table.ElementTags[0]).TagName);
    }

    [Test]
    public void TableFromElement()
    {
      Element element = ie.Element(tableId);  
      Table table = new Table(element);
      Assert.AreEqual(tableId, table.Id);
    }

    [Test]
    public void TableTableBodiesExcludesBodiesFromNestedTables()
    {
      ie.GoTo(new Uri(HtmlTestBaseURI, "Tables.html"));

      TableBodyCollection tableBodies = ie.Table("Table1").TableBodies;
      Assert.AreEqual(2, tableBodies.Length, "Unexpected number of tbodies");
      Assert.AreEqual("tbody1", tableBodies[0].Id, "Unexpected tbody[0].id");
      Assert.AreEqual("tbody3", tableBodies[1].Id, "Unexpected tbody[1].id");
    }

    [Test]
    public void TableBodyExcludesRowsFromNestedTables()
    {
      ie.GoTo(new Uri(HtmlTestBaseURI, "Tables.html"));

      TableBody tableBody = ie.Table("Table1").TableBodies[0];

      Assert.AreEqual(1, tableBody.Tables.Length, "Expected nested table");
      Assert.AreEqual(2, tableBody.TableRows.Length, "Expected 2 rows");
      Assert.AreEqual("1", tableBody.TableRows[0].Id, "Unexpected tablerows[0].id");
      Assert.AreEqual("3", tableBody.TableRows[1].Id, "Unexpected tablerows[1].id");
    }

    [Test]
    public void TableExists()
    {
      Assert.IsTrue(ie.Table(tableId).Exists);
      Assert.IsFalse(ie.Table("nonexistingtableid").Exists);
    }

    [Test]
    public void TableRowGetParentTable()
    {
      TableRow tableRow = ie.TableRow("row0");
      Assert.IsInstanceOfType(typeof(TableBody), tableRow.Parent, "Parent should be a TableBody Type");
      Assert.IsInstanceOfType(typeof(Table), tableRow.ParentTable, "Should be a Table Type");
      Assert.AreEqual("table1", tableRow.ParentTable.Id, "Unexpected id");
    }

    [Test]
    public void TableTest()
    {
      Assert.AreEqual(tableId,  ie.Table(Find.ById(tableId)).Id);
      
      Table table = ie.Table(tableId);
      Assert.AreEqual(tableId,  table.Id);
      Assert.AreEqual(tableId,  table.ToString());
      Assert.AreEqual(3, table.TableRows.Length, "Unexpected number of rows");

      TableRow row = table.FindRow("a1",0);
      Assert.IsNotNull(row, "Row with a1 expected");
      Assert.AreEqual("a1", row.TableCells[0].Text, "Unexpected text in cell");
      

      row = table.FindRow("b2",1);
      Assert.IsNotNull(row, "Row with b2 expected");
      Assert.AreEqual("b2", row.TableCells[1].Text, "Unexpected text in cell");

      row = table.FindRow("c1",0);
      Assert.IsNull(row, "No row with c1 expected");
    }

    [Test]
    public void TableFindRowShouldIgnoreTHtagsInTBody()
    {
      Table table = ie.Table(tableId);
      Assert.AreEqual("TH", table.TableRows[0].Elements[0].TagName.ToUpper(), "First tablerow should contain a TH element");
      
      TableRow row = table.FindRow(new Regex("a"), 0);
      Assert.IsNotNull(row, "row expected");
      Assert.AreEqual("a1", row.TableCells[0].Text);
    }
  
    [Test]
    public void TableFindRowWithTextIgnoreCase()
    {
      Table table = ie.Table(tableId);

      // test: ignore case of the text to find
      TableRow row = table.FindRow("A2",1);
      Assert.IsNotNull(row, "Row with a1 expected");
      Assert.AreEqual("a2", row.TableCells[1].Text, "Unexpected text in cell");
    }

    [Test]
    public void TableFindRowWithTextNoPartialMatch()
    {
      Table table = ie.Table(tableId);

      // test: ignore case of the text to find
      TableRow row = table.FindRow("a",1);
      Assert.IsNull(row, "No row expected");
    }

    [Test]
    public void TableFindRowWithRegex()
    {
      Table table = ie.Table(tableId);
      
      TableRow row = table.FindRow(new Regex("b"),1);
      
      Assert.IsNotNull(row, "Row with b1 expected");
      Assert.AreEqual("b2", row.TableCells[1].Text, "Unexpected text in cell");
    }
    
    [Test]
    public void Tables()
    {
      // Collection.length
      TableCollection tables = ie.Tables;
      
      Assert.AreEqual(2, tables.Length);

      // Collection items by index
      Assert.AreEqual("table1", tables[0].Id);
      Assert.AreEqual("table2", tables[1].Id);

      IEnumerable tableEnumerable = tables;
      IEnumerator tableEnumerator = tableEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (Table table in tables)
      {
        tableEnumerator.MoveNext();
        object enumTable = tableEnumerator.Current;
        
        Assert.IsInstanceOfType(table.GetType(), enumTable, "Types are not the same");
        Assert.AreEqual(table.OuterHtml, ((Table)enumTable).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(tableEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(2, count);
    }

    [Test]
    public void TableRowElementTags()
    {
      Assert.AreEqual(1, TableRow.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("tr",((ElementTag) TableRow.ElementTags[0]).TagName);
    }

    [Test]
    public void TableRowFromElement()
    {
      Element element = ie.Element("row0");  
      TableRow tableRow = new TableRow(element);
      Assert.AreEqual("row0", tableRow.Id);
    }

    [Test]
    public void TableRowExists()
    {
      Assert.IsTrue(ie.TableRow("row0").Exists);
      Assert.IsTrue(ie.TableRow(new Regex("row0")).Exists);
      Assert.IsFalse(ie.TableRow("nonexistingtr").Exists);
    }

    [Test]
    public void TableRows()
    {
      // Collection.Length
      TableRowCollection rows = ie.Table("table1").TableRows;
      
      Assert.AreEqual(3, rows.Length);

      // Collection items by index
      Assert.AreEqual("row0", rows[1].Id);
      Assert.AreEqual("row1", rows[2].Id);

      IEnumerable rowEnumerable = rows;
      IEnumerator rowEnumerator = rowEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (TableRow row in rows)
      {
        rowEnumerator.MoveNext();
        object enumTable = rowEnumerator.Current;
        
        Assert.IsInstanceOfType(row.GetType(), enumTable, "Types are not the same");
        Assert.AreEqual(row.OuterHtml, ((TableRow)enumTable).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(rowEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(3, count);
    }

    [Test]
    public void TableCellElementTags()
    {
      Assert.AreEqual(1, TableCell.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("td",((ElementTag) TableCell.ElementTags[0]).TagName);
    }

    [Test]
    public void TableCellFromElement()
    {
      Element element = ie.Element("td1");  
      TableCell tableCell = new TableCell(element);
      Assert.AreEqual("td1", tableCell.Id);
    }

    [Test]
    public void TableCellExists()
    {
      Assert.IsTrue(ie.TableCell("td1").Exists);
      Assert.IsTrue(ie.TableCell(new Regex("td1")).Exists);
      Assert.IsFalse(ie.Table("nonexistingtd1").Exists);
    }

    [Test]
    public void TableCellByIndexExists()
    {
      Assert.IsTrue(ie.TableCell("td1", 0).Exists);
      Assert.IsTrue(ie.TableCell(new Regex("td1"), 0).Exists);
      Assert.IsFalse(ie.TableCell("td1", 100).Exists);
    }
    
    [Test]
    public void TableCellByIndex()
    {
      // accessing several occurences with equal id
      Assert.AreEqual("a1", ie.TableCell("td1", 0).Text);
      Assert.AreEqual("b1", ie.TableCell("td1", 1).Text);
    }

    [Test]
    public void TableCellGetParentTableRow()
    {
      TableCell tableCell = ie.TableCell(Find.ByText("b1"));
      Assert.IsInstanceOfType(typeof(TableRow), tableCell.ParentTableRow, "Should be a TableRow Type");
      Assert.AreEqual("row1", tableCell.ParentTableRow.Id, "Unexpected id");
    }

    [Test]
    public void TableCellCellIndex()
    {
      Assert.AreEqual(0, ie.TableCell(Find.ByText("b1")).Index);
      Assert.AreEqual(1, ie.TableCell(Find.ByText("b2")).Index);
    }

    [Test]
    public void TableCells()
    {
      // Collection.Length
      TableCellCollection cells = ie.Table("table1").TableRows[1].TableCells;
      
      Assert.AreEqual(2, cells.Length);

      // Collection items by index
      Assert.AreEqual("td1", cells[0].Id);
      Assert.AreEqual("td2", cells[1].Id);

      IEnumerable cellEnumerable = cells;
      IEnumerator cellEnumerator = cellEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (TableCell cell in cells)
      {
        cellEnumerator.MoveNext();
        object enumTable = cellEnumerator.Current;
        
        Assert.IsInstanceOfType(cell.GetType(), enumTable, "Types are not the same");
        Assert.AreEqual(cell.OuterHtml, ((TableCell)enumTable).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(cellEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(2, count);
    }

    [Test]
    public void GetInvalidAttribute()
    {
      Button helloButton = ie.Button("helloid");
      Assert.IsNull(helloButton.GetAttributeValue("NONSENCE"));
    }
    
    [Test]
    public void GetValidButUndefiniedAttribute()
    {
      Button helloButton = ie.Button("helloid");
      Assert.IsNull(helloButton.GetAttributeValue("title"));
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void GetAttributeValueOfNullThrowsArgumentNullException()
    {
      Button helloButton = ie.Button("helloid");
      Assert.IsNull(helloButton.GetAttributeValue(null));
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void GetAttributeValueOfEmptyStringThrowsArgumentNullException()
    {
      Button helloButton = ie.Button("helloid");
      Assert.IsNull(helloButton.GetAttributeValue(String.Empty));
    }

    [Test]
    public void ButtonElementTags()
    {
      Assert.AreEqual(2, Button.ElementTags.Count, "2 elementtags expected");
      Assert.AreEqual("input",((ElementTag) Button.ElementTags[0]).TagName);
      Assert.AreEqual("button submit image reset",((ElementTag) Button.ElementTags[0]).InputTypes);
      Assert.AreEqual("button",((ElementTag) Button.ElementTags[1]).TagName);
    }
    
    [Test]
    public void ButtonFormInputElement()
    {
      const string popupValue = "Show modeless dialog";
      Button button = ie.Button(Find.ById("popupid"));
      
      Assert.IsInstanceOfType(typeof(Element), button);
      Assert.IsInstanceOfType(typeof(Button), button);

      Assert.AreEqual(popupValue, button.Value);
      Assert.AreEqual(popupValue, ie.Button("popupid").Value);
      Assert.AreEqual(popupValue, ie.Button("popupid").ToString());
      Assert.AreEqual(popupValue, ie.Button(Find.ByName("popupname")).Value);
      
      Button helloButton = ie.Button("helloid");
      Assert.AreEqual("Show allert", helloButton.Value);
      Assert.AreEqual(helloButton.Value, helloButton.Text);
      
      Assert.IsTrue(ie.Button(new Regex("popupid")).Exists);
    }
        
    [Test]
    public void ButtonFormButtonElement()
    {
      const string Value = "Button Element";
      
      Button button = ie.Button(Find.ById("buttonelementid"));
      
      Assert.IsInstanceOfType(typeof(Element), button);
      Assert.IsInstanceOfType(typeof(Button), button);

      Assert.AreEqual(Value, button.Value);
      Assert.AreEqual(Value, ie.Button("buttonelementid").Value);
      Assert.AreEqual(Value, ie.Button("buttonelementid").ToString());
      Assert.AreEqual(Value, ie.Button(Find.ByName("buttonelementname")).Value);

      Assert.AreEqual(Value, ie.Button(Find.ByText("Button Element")).Value);
      // OK, this one is weird. The HTML says value="ButtonElementValue"
      // but the value attribute seems to return the innertext(!)
      // <button id="buttonelementid" name="buttonelementname" value="ButtonElementValue">Button Element</button>
      Assert.AreEqual(Value, ie.Button(Find.ByValue("Button Element")).Value);
      
      Assert.IsTrue(ie.Button(new Regex("buttonelementid")).Exists);
    }

    [Test, ExpectedException(typeof(ElementDisabledException))]
    public void ButtonDisabledException()
    {
      ie.Button("disabledid").Click();
    }

    [Test, ExpectedException(typeof(ElementNotFoundException), "Could not find a 'INPUT (button submit image reset) or BUTTON' tag containing attribute id with value 'noneexistingbuttonid'")]
    public void ButtonElementNotFoundException()
    {
      IE.Settings.WaitUntilExistsTimeOut = 1;
      ie.Button("noneexistingbuttonid").Click();
    }

    [Test]
    public void ButtonExists()
    {
      // Test <input type=button />
      Assert.IsTrue(ie.Button("disabledid").Exists);
      // Test <Button />
      Assert.IsTrue(ie.Button("buttonelementid").Exists);

      Assert.IsFalse(ie.Button("noneexistingbuttonid").Exists);
    }

    [Test]
    public void ButtonFromInputElement()
    {
      Element element = ie.Element("disabledid");
      Button button = new Button(element);
      Assert.AreEqual("disabledid", button.Id);
    }

    [Test]
    public void ButtonFromButtonElement()
    {
      Element element = ie.Element("buttonelementid");
      Button button = new Button(element);
      Assert.AreEqual("buttonelementid", button.Id);
    }
    
    [Test, ExpectedException(typeof(ArgumentException))]
    public void ButtonFromElementArgumentException()
    {
      Element element = ie.Element("Checkbox1");
      new Button(element);
    }
    
    [Test]
    public void Buttons()
    {
      const int expectedButtonsCount = 5;
      Assert.AreEqual(expectedButtonsCount, ie.Buttons.Length, "Unexpected number of buttons");

      const int expectedFormButtonsCount = 4;
      Form form = ie.Form("Form");

      // Collection.Length
      ButtonCollection formButtons = form.Buttons;
      
      Assert.AreEqual(expectedFormButtonsCount, formButtons.Length);

      // Collection items by index
      Assert.AreEqual("popupid", form.Buttons[0].Id);
      Assert.AreEqual("modalid", form.Buttons[1].Id);
      Assert.AreEqual("helloid", form.Buttons[2].Id);

      // Exists
      Assert.IsTrue(form.Buttons.Exists("modalid"));
      Assert.IsTrue(form.Buttons.Exists(new Regex("modalid")));
      Assert.IsFalse(form.Buttons.Exists("nonexistingid"));
      
      IEnumerable buttonEnumerable = formButtons;
      IEnumerator buttonEnumerator = buttonEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (Button inputButton in formButtons)
      {
        buttonEnumerator.MoveNext();
        object enumButton = buttonEnumerator.Current;
        
        Assert.IsInstanceOfType(inputButton.GetType(), enumButton, "Types are not the same");
        Assert.AreEqual(inputButton.OuterHtml, ((Button)enumButton).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(buttonEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedFormButtonsCount, count);
    }

    [Test]
    public void ButtonsFilterOnHTMLElementCollection()
    {
      ButtonCollection buttons = ie.Buttons.Filter(Find.ById(new Regex("le")));
      Assert.AreEqual(2, buttons.Length);
      Assert.AreEqual("disabledid", buttons[0].Id);
      Assert.AreEqual("buttonelementid", buttons[1].Id);
    }
    
    [Test]
    public void ButtonsFilterOnArrayListElements()
    {
      ButtonCollection buttons = ie.Buttons;
      Assert.AreEqual(5, buttons.Length);
      
      buttons = ie.Buttons.Filter(Find.ById(new Regex("le")));
      Assert.AreEqual(2, buttons.Length);
      Assert.AreEqual("disabledid", buttons[0].Id);
      Assert.AreEqual("buttonelementid", buttons[1].Id);
    }
    
    [Test]
    public void CheckBoxElementTags()
    {
      Assert.AreEqual(1, CheckBox.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("input",((ElementTag) CheckBox.ElementTags[0]).TagName);
      Assert.AreEqual("checkbox",((ElementTag) CheckBox.ElementTags[0]).InputTypes);
    }

    [Test]
    public void CheckBoxFromElement()
    {
      Element element = ie.Element("Checkbox1");  
      CheckBox checkBox = new CheckBox(element);
      Assert.AreEqual("Checkbox1", checkBox.Id);
    }
    
    [Test]
    public void CheckBoxExists()
    {
      Assert.IsTrue(ie.CheckBox("Checkbox1").Exists);
      Assert.IsTrue(ie.CheckBox(new Regex("Checkbox1")).Exists);
      Assert.IsFalse(ie.CheckBox("noneexistingCheckbox1id").Exists);
    }

    [Test]
    public void CheckBoxTest()
    {
      CheckBox checkbox1 = ie.CheckBox("Checkbox1");

      Assert.AreEqual("Checkbox1", checkbox1.Id, "Found wrong checkbox");
      Assert.AreEqual("Checkbox1", checkbox1.ToString(), "ToString didn't return Id");
      Assert.IsTrue(checkbox1.Checked, "Should initially be checked");
      
      checkbox1.Checked = false;
      Assert.IsFalse(checkbox1.Checked, "Should not be checked");

      checkbox1.Checked = true;
      Assert.IsTrue(checkbox1.Checked, "Should be checked");
      
    }

    [Test]
    public void CheckBoxes()
    {
      Assert.AreEqual(5, ie.CheckBoxes.Length, "Unexpected number of checkboxes");

      CheckBoxCollection formCheckBoxs = ie.Form("FormCheckboxes").CheckBoxes;

      // Collection items by index
      Assert.AreEqual(3,formCheckBoxs.Length, "Wrong number off checkboxes");
      Assert.AreEqual("Checkbox1", formCheckBoxs[0].Id);
      Assert.AreEqual("Checkbox2", formCheckBoxs[1].Id);
      Assert.AreEqual("Checkbox4", formCheckBoxs[2].Id);      

      // Collection iteration and comparing the result with Enumerator
      IEnumerable checkboxEnumerable = formCheckBoxs;
      IEnumerator checkboxEnumerator = checkboxEnumerable.GetEnumerator();

      int count = 0;
      foreach (CheckBox checkBox in formCheckBoxs)
      {
        checkboxEnumerator.MoveNext();
        object enumCheckbox = checkboxEnumerator.Current;
        
        Assert.IsInstanceOfType(checkBox.GetType(), enumCheckbox, "Types are not the same");
        Assert.AreEqual(checkBox.OuterHtml, ((CheckBox)enumCheckbox).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(checkboxEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(3, count);
    }

    [Test]
    public void DivElementTags()
    {
      Assert.AreEqual(1, Div.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("div",((ElementTag) Div.ElementTags[0]).TagName);
    }

    [Test]
    public void DivFromElement()
    {
      Element element = ie.Element("divid");  
      Div div = new Div(element);
      Assert.AreEqual("divid", div.Id);
    }
    
    [Test]
    public void DivExists()
    {
      Assert.IsTrue(ie.Div("divid").Exists);
      Assert.IsTrue(ie.Div(new Regex("divid")).Exists);
      Assert.IsFalse(ie.Div("noneexistingdivid").Exists);
    }

    [Test]
    public void DivTest()
    {
      Assert.AreEqual("divid", ie.Div(Find.ById("divid")).Id, "Find Div by Find.ById");
      Assert.AreEqual("divid", ie.Div("divid").Id, "Find Div by ie.Div()");
    }

    [Test]
    public void Divs()
    {
      Assert.AreEqual(1, ie.Divs.Length, "Unexpected number of Divs");

      DivCollection divs = ie.Divs;

      // Collection items by index
      Assert.AreEqual("divid", divs[0].Id);

      // Collection iteration and comparing the result with Enumerator
      IEnumerable divEnumerable = divs;
      IEnumerator divEnumerator = divEnumerable.GetEnumerator();

      int count = 0;
      foreach (Div div in divs)
      {
        divEnumerator.MoveNext();
        object enumDiv = divEnumerator.Current;
        
        Assert.IsInstanceOfType(div.GetType(), enumDiv, "Types are not the same");
        Assert.AreEqual(div.OuterHtml, ((Div)enumDiv).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(divEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(1, count);
    }

    
    [Test]
    public void Element()
    {
      Element element = ie.Element(Find.ById(tableId));

      Assert.IsAssignableFrom(typeof(ElementsContainer), element, "The returned object form ie.Element should be castable to ElementsContainer");

      Assert.IsNotNull(element,  "Element not found");
      
      // check behavior for standard attribute
      Assert.AreEqual(tableId, element.GetAttributeValue("id"), "GetAttributeValue id failed");
      // check behavior for non existing attribute
      Assert.IsNull(element.GetAttributeValue("watin"), "GetAttributeValue watin should return null");
      // check behavior for custom attribute
      Assert.AreEqual("myvalue",element.GetAttributeValue("myattribute"), "GetAttributeValue myattribute should return myvalue");
      
      Assert.AreEqual("table", element.TagName.ToLower(), "Invalid tagname");

      // Textbefore and TextAfter tests
      CheckBox checkBox = ie.CheckBox("Checkbox21");
      Assert.AreEqual("Test label before: ", checkBox.TextBefore, "Unexpected checkBox.TextBefore");
      Assert.AreEqual(" Test label after", checkBox.TextAfter, "Unexpected checkBox.TextAfter");
    }
    
    [Test]
    public void ElementFindByShouldNeverThrowInvalidAttributeException()
    {
      Element element = ie.Element(Find.ByFor("Checkbox21"));
      Assert.IsTrue(element.Exists);
    }

    [Test]
    public void ButtonAndOthersFindByShouldNeverThrowInvalidAttributeException()
    {
      Button button = ie.Button(Find.ByCustom("noattribute", "novalue"));
      Assert.IsFalse(button.Exists);
    }

    [Test]
    public void ElementCollectionExistsShouldNeverThrowInvalidAttributeException()
    {
      Assert.IsTrue(ie.Elements.Exists(Find.ByFor("Checkbox21")));
    }
            
    [Test]
    public void ElementCollectionSecondFilterShouldNeverThrowInvalidAttributeException()
    {
      ElementCollection elements = ie.Elements.Filter(Find.ById("testlinkid"));
      ElementCollection elements2 = elements.Filter(Find.ByFor("Checkbox21"));
      Assert.AreEqual(0, elements2.Length);
    }
    
    [Test]
    public void ButtonCollectionSecondFilterAndOthersShouldThrowInvalidAttributeException()
    {
      ButtonCollection buttons = ie.Buttons.Filter(Find.ById("testlinkid"));
      ButtonCollection buttons2 = buttons.Filter(Find.ByFor("Checkbox21"));
      Assert.AreEqual(0, buttons2.Length);
    }
    
    [Test]
    public void LabelElementTags()
    {
      Assert.AreEqual(1, Label.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("label",((ElementTag) Label.ElementTags[0]).TagName);
    }

    [Test]
    public void LabelFromElement()
    {
      Element element = ie.Element(Find.ByFor("Checkbox21"));
      Label label = new Label(element);
      Assert.AreEqual("Checkbox21", label.For);
    }

    [Test]
    public void LabelExists()
    {
      Assert.IsTrue(ie.Label(Find.ByFor("Checkbox21")).Exists);
      Assert.IsTrue(ie.Label(Find.ByFor(new Regex("Checkbox21"))).Exists);
      Assert.IsFalse(ie.Label(Find.ByFor("nonexistingCheckbox21")).Exists);
    }

    [Test]
    public void LabelByFor()
    {
      Label label = ie.Label(Find.ByFor("Checkbox21"));

      Assert.AreEqual("Checkbox21", label.For, "Unexpected label.For id");
      Assert.AreEqual("label for Checkbox21", label.Text, "Unexpected label.Text");
      Assert.AreEqual("C", label.AccessKey, "Unexpected label.AccessKey");
    }
    
    [Test]
    public void LabelByForWithElement()
    {
      CheckBox checkBox = ie.CheckBox("Checkbox21");
      
      Label label = ie.Label(Find.ByFor(checkBox));

      Assert.AreEqual("Checkbox21", label.For, "Unexpected label.For id");
      Assert.AreEqual("label for Checkbox21", label.Text, "Unexpected label.Text");
      Assert.AreEqual("C", label.AccessKey, "Unexpected label.AccessKey");
    }

    [Test]
    public void LabelWrapped()
    {
      LabelCollection labelCollection = ie.Labels;
      Assert.AreEqual(2, ie.Labels.Length, "Unexpected number of labels");

      Label label = labelCollection[1];

      Assert.AreEqual(null, label.For, "Unexpected label.For id");
      Assert.AreEqual("Test label before:  Test label after", label.Text, "Unexpected label.Text");

      // Element.TextBefore and Element.TextAfter are tested in test method Element
    }

    [Test]
    public void Labels()
    {
      const int expectedLabelCount = 2;

      Assert.AreEqual(expectedLabelCount, ie.Labels.Length, "Unexpected number of labels");

      LabelCollection labelCollection = ie.Labels;

      // Collection items by index
      Assert.AreEqual(expectedLabelCount,labelCollection.Length, "Wrong number of labels");

      // Collection iteration and comparing the result with Enumerator
      IEnumerable labelEnumerable = labelCollection;
      IEnumerator labelEnumerator = labelEnumerable.GetEnumerator();

      int count = 0;
      foreach (Label label in labelCollection)
      {
        labelEnumerator.MoveNext();
        object enumCheckbox = labelEnumerator.Current;
        
        Assert.IsInstanceOfType(label.GetType(), enumCheckbox, "Types are not the same");
        Assert.AreEqual(label.OuterHtml, ((Label)enumCheckbox).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(labelEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedLabelCount, count);
    }

    [Test]
    public void LinkElementTags()
    {
      Assert.AreEqual(1, Link.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("a",((ElementTag) Link.ElementTags[0]).TagName);
    }

    [Test]
    public void LinkFromElement()
    {
      Element element = ie.Element("testlinkid");  
      Link link = new Link(element);
      Assert.AreEqual("testlinkid", link.Id);
    }

    [Test]
    public void LinkExists()
    {
      Assert.IsTrue(ie.Link("testlinkid").Exists);
      Assert.IsTrue(ie.Link(new Regex("testlinkid")).Exists);
      Assert.IsFalse(ie.Link("nonexistingtestlinkid").Exists);
    }
    
    [Test]
    public void LinkTest()
    {
      Assert.AreEqual(WatiNURI, ie.Link(Find.ById("testlinkid")).Url);
      Assert.AreEqual(WatiNURI, ie.Link("testlinkid").Url);
      Assert.AreEqual(WatiNURI, ie.Link(Find.ByName("testlinkname")).Url);
      Assert.AreEqual(WatiNURI, ie.Link(Find.ByUrl(WatiNURI)).Url);
      Assert.AreEqual("Microsoft", ie.Link(Find.ByText("Microsoft")).Text);
    }

    [Test]
    public void LinkFindNonExistingElementWithoutElementNotFoundException()
    {
      ie.Link(Find.ById("noexistinglinkid"));
    }

    [Test]
    public void Links()
    {
      const int expectedLinkCount = 3;

      Assert.AreEqual(expectedLinkCount, ie.Links.Length, "Unexpected number of links");

      LinkCollection links = ie.Links;

      // Collection items by index
      Assert.AreEqual(expectedLinkCount,links.Length, "Wrong number off links");
      Assert.AreEqual("testlinkid", links[0].Id);
      Assert.AreEqual("testlinkid1", links[1].Id);

      // Collection iteration and comparing the result with Enumerator
      IEnumerable linksEnumerable = links;
      IEnumerator linksEnumerator = linksEnumerable.GetEnumerator();

      int count = 0;
      foreach (Link link in links)
      {
        linksEnumerator.MoveNext();
        object enumLink = linksEnumerator.Current;
        
        Assert.IsInstanceOfType(link.GetType(), enumLink, "Types are not the same");
        Assert.AreEqual(link.OuterHtml, ((Link)enumLink).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(linksEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedLinkCount, count);
    }


    [Test]
    public void RadioButtonElementTags()
    {
      Assert.AreEqual(1, RadioButton.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("input",((ElementTag) RadioButton.ElementTags[0]).TagName);
      Assert.AreEqual("radio",((ElementTag) RadioButton.ElementTags[0]).InputTypes);
    }

    [Test]
    public void RadioButtonFromElement()
    {
      Element element = ie.Element("Radio1");  
      RadioButton radioButton = new RadioButton(element);
      Assert.AreEqual("Radio1", radioButton.Id);
    }

    [Test]
    public void RadioButtonExists()
    {
      Assert.IsTrue(ie.RadioButton("Radio1").Exists);
      Assert.IsTrue(ie.RadioButton(new Regex("Radio1")).Exists);
      Assert.IsFalse(ie.RadioButton("nonexistingRadio1").Exists);
    }

    [Test]
    public void RadioButtonTest()
    {
      RadioButton RadioButton1 = ie.RadioButton("Radio1");

      Assert.AreEqual("Radio1", RadioButton1.Id, "Found wrong RadioButton.");
      Assert.AreEqual("Radio1", RadioButton1.ToString(), "ToString didn't return the Id.");
      Assert.IsTrue(RadioButton1.Checked, "Should initially be checked");
      
      RadioButton1.Checked = false;
      Assert.IsFalse(RadioButton1.Checked, "Should not be checked");

      RadioButton1.Checked = true;
      Assert.IsTrue(RadioButton1.Checked, "Should be checked");
    }

    [Test]
    public void RadioButtons()
    {
      Assert.AreEqual(3, ie.RadioButtons.Length, "Unexpected number of RadioButtons");

      RadioButtonCollection formRadioButtons = ie.Form("FormRadioButtons").RadioButtons;

      Assert.AreEqual(2,formRadioButtons.Length, "Wrong number off RadioButtons");
      Assert.AreEqual("Radio2", formRadioButtons[0].Id);
      Assert.AreEqual("Radio3", formRadioButtons[1].Id);

      // Collection iteration and comparing the result with Enumerator
      IEnumerable radiobuttonEnumerable = formRadioButtons;
      IEnumerator radiobuttonEnumerator = radiobuttonEnumerable.GetEnumerator();

      int count = 0;
      foreach (RadioButton radioButton in formRadioButtons)
      {
        radiobuttonEnumerator.MoveNext();
        object enumRadioButton = radiobuttonEnumerator.Current;
        
        Assert.IsInstanceOfType(radioButton.GetType(), enumRadioButton, "Types are not the same");
        Assert.AreEqual(radioButton.OuterHtml, ((RadioButton)enumRadioButton).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(radiobuttonEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(2, count);

    }

    [Test]
    public void TextFieldElementTags()
    {
      Assert.AreEqual(2, TextField.ElementTags.Count, "2 elementtags expected");
      Assert.AreEqual("input",((ElementTag) TextField.ElementTags[0]).TagName);
      Assert.AreEqual("text password textarea hidden",((ElementTag) TextField.ElementTags[0]).InputTypes);
      Assert.AreEqual("textarea",((ElementTag) TextField.ElementTags[1]).TagName);
    }

    [Test]
    public void TextFieldFromElementInput()
    {
      Element element = ie.Element("name");  
      TextField textField = new TextField(element);
      Assert.AreEqual("name", textField.Id);
    }
    
    [Test]
    public void TextFieldFromElementTextArea()
    {
      Element element = ie.Element("Textarea1");  
      TextField textField = new TextField(element);
      Assert.AreEqual("Textarea1", textField.Id);
    }

    [Test]
    public void TextFieldExists()
    {
      Assert.IsTrue(ie.TextField("name").Exists);
      Assert.IsTrue(ie.TextField(new Regex("name")).Exists);
      Assert.IsFalse(ie.TextField("nonexistingtextfield").Exists);
    }

    [Test]
    public void TextFieldTypTextShouldHandleNewLineInputCorrect()
    {
      TextField textfieldName = ie.TextField("Textarea1");
      string textWithNewLine = "Line1" + Environment.NewLine + "Line2";
      
      textfieldName.TypeText(textWithNewLine);
      Assert.AreEqual(textWithNewLine, textfieldName.Value);
    }

    [Test]
    public void TextFieldTest()
    {
      const string value = "Hello world!";
      const string appendValue = " This is WatiN!";
      TextField textfieldName = ie.TextField("name");

      Assert.AreEqual("name", textfieldName.Id);
      Assert.AreEqual("textinput1", textfieldName.Name);

      Assert.AreEqual(200, textfieldName.MaxLength, "Unexpected maxlenght");

      Assert.IsNull(textfieldName.Value, "Initial value should be null");
      
      textfieldName.TypeText(value);
      Assert.AreEqual(value,textfieldName.Value, "TypeText not OK");

      textfieldName.AppendText(appendValue) ;
      Assert.AreEqual(value + appendValue,textfieldName.Value, "AppendText not OK");

      textfieldName.Clear();
      Assert.IsNull(textfieldName.Value, "After Clear value should by null");

      textfieldName.Value = value;
      Assert.AreEqual(value,textfieldName.Value, "Value not OK");
      Assert.AreEqual(value,textfieldName.Text, "Text not OK");
      
      Assert.AreEqual("Textfield title", textfieldName.ToString(), "Expected Title");
      Assert.AreEqual("readonlytext", ie.TextField("readonlytext").ToString(), "Expected Id");
      Assert.AreEqual("disabledtext", ie.TextField(Find.ByName("disabledtext")).ToString(), "Expected Name");
    }

    [Test]
    public void TextFieldTypeTextEvents()
    {
      using(IE ie1= new IE(TestEventsURI))
      {
        Assert.IsFalse(ie1.CheckBox("chkKeyDown").Checked, "KeyDown false expected");
        Assert.IsFalse(ie1.CheckBox("chkKeyPress").Checked, "KeyPress false expected");
        Assert.IsFalse(ie1.CheckBox("chkKeyUp").Checked, "KeyUp false expected");

        ie1.TextField("textfieldid").TypeText("test");
        
        Assert.IsTrue(ie1.CheckBox("chkKeyDown").Checked, "KeyDown event expected");
        Assert.IsTrue(ie1.CheckBox("chkKeyPress").Checked, "KeyPress event expected");
        Assert.IsTrue(ie1.CheckBox("chkKeyUp").Checked, "KeyUp event expected");
      }
    }
    
    [Test]
    public void TextFieldTextAreaElement()
    {
      ie.GoTo(ie.Uri);

      const string value = "Hello world!";
      const string appendValue = " This is WatiN!";
      TextField textfieldName = ie.TextField("Textarea1");

      Assert.AreEqual("Textarea1", textfieldName.Id);
      Assert.AreEqual("TextareaName", textfieldName.Name);

      Assert.AreEqual(0, textfieldName.MaxLength, "Unexpected maxlenght");

      Assert.IsNull(textfieldName.Value, "Initial value should be null");
      
      textfieldName.TypeText(value);
      Assert.AreEqual(value,textfieldName.Value, "TypeText not OK");

      textfieldName.AppendText(appendValue) ;
      Assert.AreEqual(value + appendValue,textfieldName.Value, "AppendText not OK");

      textfieldName.Clear();
      Assert.IsNull(textfieldName.Value, "After Clear value should by null");

      textfieldName.Value = value;
      Assert.AreEqual(value,textfieldName.Value, "Value not OK");
      Assert.AreEqual(value,textfieldName.Text, "Text not OK");
    }

    [Test]
    public void TextFieldFlash()
    {
      ie.TextField("name").Flash();
    }

    [Test, ExpectedException(typeof(ElementReadOnlyException),"Element with Id:readonlytext is readonly")]
    public void TextFieldReadyOnlyException()
    {
      TextField textField = ie.TextField("readonlytext");
      textField.TypeText("This should go wrong");
    }

    [Test, ExpectedException(typeof(ElementDisabledException),"Element with Id:disabledtext is disabled")]
    public void TextFieldDisabledException()
    {
      TextField textField = ie.TextField(Find.ByName("disabledtext"));
      textField.TypeText("This should go wrong");
    }

    [Test, ExpectedException(typeof(ElementNotFoundException),"Could not find a 'INPUT (text password textarea hidden) or TEXTAREA' tag containing attribute id with value 'noneexistingtextfieldid'")]
    public void TextFieldElementNotFoundException()
    {
      IE.Settings.WaitUntilExistsTimeOut = 1;
      ie.TextField("noneexistingtextfieldid").TypeText("");
    }
    
    [Test]
    public void TextFields()
    {
      Assert.AreEqual(6, ie.TextFields.Length, "Number of TextFields should by 4");

      // Collection items by index
      Form mainForm = ie.Form("FormHiddens");
      Assert.AreEqual(2, mainForm.TextFields.Length, "Wrong number of textfields in collectionTestForm");
      Assert.AreEqual("first", mainForm.TextFields[0].Value);
      Assert.AreEqual("second", mainForm.TextFields[1].Value);

      Form form = ie.Form("Form");

      // Collection.length
      TextFieldCollection textfields = form.TextFields;
      Assert.AreEqual(1, textfields.Length);

      // Collection iteration and comparing the result with Enumerator
      IEnumerable textfieldEnumerable = textfields;
      IEnumerator textfieldEnumerator = textfieldEnumerable.GetEnumerator();

      int count = 0;
      foreach (TextField textField in textfields)
      {
        textfieldEnumerator.MoveNext();
        object enumTextfield = textfieldEnumerator.Current;
        
        Assert.IsInstanceOfType(textField.GetType(), enumTextfield, "Types are not the same");
        Assert.AreEqual(textField.OuterHtml, ((TextField)enumTextfield).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(textfieldEnumerator.MoveNext(), "Expected last item");

      Assert.AreEqual(1, count);
    }
    
    [Test]
    public void FindingElementByCustomAttribute()
    {
      Assert.IsTrue(ie.Table(Find.ByCustom("myattribute", "myvalue")).Exists);
    }
    
    [Test]
    public void FileUploadElementTags()
    {
      Assert.AreEqual(1, FileUpload.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("input",((ElementTag) FileUpload.ElementTags[0]).TagName);
      Assert.AreEqual("file",((ElementTag) FileUpload.ElementTags[0]).InputTypes);
    }

    [Test]
    public void FileUploadFromElement()
    {
      Element element = ie.Element("upload");  
      FileUpload fileUpload = new FileUpload(element);
      Assert.AreEqual("upload", fileUpload.Id);
    }

    [Test]
    public void FileUploadExists()
    {
      Assert.IsTrue(ie.FileUpload("upload").Exists);
      Assert.IsTrue(ie.FileUpload(new Regex("upload")).Exists);
      Assert.IsFalse(ie.FileUpload("noneexistingupload").Exists);
    }

    [Test]
    public void FileUploadTest()
    {
      FileUpload fileUpload = ie.FileUpload("upload");
      
      Assert.IsNotNull(fileUpload);
      Assert.IsNull(fileUpload.FileName);
      
      fileUpload.Set(MainURI.LocalPath);
      
      Assert.AreEqual(MainURI.LocalPath, fileUpload.FileName);      
    }
    
    [Test, ExpectedException(typeof(System.IO.FileNotFoundException))]
    public void FileUploadFileNotFoundException()
    {
      FileUpload fileUpload = ie.FileUpload("upload");
      fileUpload.Set("nonexistingfile.nef");
    }
    
    [Test]
    public void FileUploads()
    {
      const int expectedFileUploadsCount = 1;
      Assert.AreEqual(expectedFileUploadsCount, ie.FileUploads.Length, "Unexpected number of FileUploads");

      // Collection.Length
      FileUploadCollection formFileUploads = ie.FileUploads;
      
      // Collection items by index
      Assert.AreEqual("upload", ie.FileUploads[0].Id);

      IEnumerable FileUploadEnumerable = formFileUploads;
      IEnumerator FileUploadEnumerator = FileUploadEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (FileUpload inputFileUpload in formFileUploads)
      {
        FileUploadEnumerator.MoveNext();
        object enumFileUpload = FileUploadEnumerator.Current;
        
        Assert.IsInstanceOfType(inputFileUpload.GetType(), enumFileUpload, "Types are not the same");
        Assert.AreEqual(inputFileUpload.OuterHtml, ((FileUpload)enumFileUpload).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(FileUploadEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedFileUploadsCount, count);
    }
    
    [Test]
    public void ParaElementTags()
    {
      Assert.AreEqual(1, Para.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("p",((ElementTag) Para.ElementTags[0]).TagName);
    }

    [Test]
    public void ParaFromElement()
    {
      Element element = ie.Element("links");  
      Para para = new Para(element);
      Assert.AreEqual("links", para.Id);
    }

    [Test]
    public void ParaExists()
    {
      Assert.IsTrue(ie.Para("links").Exists);
      Assert.IsTrue(ie.Para(new Regex("links")).Exists);
      Assert.IsFalse(ie.Para("nonexistinglinks").Exists);
    }
    
    [Test]
    public void ParaTest()
    {
      Para para = ie.Para("links");
      
      Assert.IsInstanceOfType(typeof(ElementsContainer), para);
      
      Assert.IsNotNull(para);
      Assert.AreEqual("links", para.Id);
    }
    
    [Test]
    public void Paras()
    {
      const int expectedParasCount = 4;
      Assert.AreEqual(expectedParasCount, ie.Paras.Length, "Unexpected number of Paras");

      // Collection.Length
      ParaCollection formParas = ie.Paras;
      
      // Collection items by index
      Assert.IsNull(ie.Paras[0].Id);
      Assert.AreEqual("links", ie.Paras[1].Id);
      Assert.IsNull(ie.Paras[2].Id);
      Assert.IsNull(ie.Paras[3].Id);

      IEnumerable ParaEnumerable = formParas;
      IEnumerator ParaEnumerator = ParaEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (Para inputPara in formParas)
      {
        ParaEnumerator.MoveNext();
        object enumPara = ParaEnumerator.Current;
        
        Assert.IsInstanceOfType(inputPara.GetType(), enumPara, "Types are not the same");
        Assert.AreEqual(inputPara.OuterHtml, ((Para)enumPara).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(ParaEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedParasCount, count);
    }
    
    [Test]
    public void SpanExists()
    {
      Assert.IsTrue(ie.Span("spanid1").Exists);
      Assert.IsTrue(ie.Span(new Regex("spanid1")).Exists);
      Assert.IsFalse(ie.Span("nonexistingspanid1").Exists);
    }

    [Test]
    public void SpanElementTags()
    {
      Assert.AreEqual(1, Span.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("span",((ElementTag) Span.ElementTags[0]).TagName);
    }

    [Test]
    public void SpanFromElement()
    {
      Element element = ie.Element("spanid1");  
      Span span = new Span(element);
      Assert.AreEqual("spanid1", span.Id);
    }

    [Test]
    public void SpanTest()
    {
      Span Span = ie.Span("spanid1");
      
      Assert.IsInstanceOfType(typeof(ElementsContainer), Span);
      
      Assert.IsNotNull(Span, "Span should bot be null");
      Assert.AreEqual("spanid1", Span.Id, "Unexpected id");
    }
    
    [Test]
    public void Spans()
    {
      const int expectedSpansCount = 2;
      Assert.AreEqual(expectedSpansCount, ie.Spans.Length, "Unexpected number of Spans");

      // Collection.Length
      SpanCollection formSpans = ie.Spans;
      
      // Collection items by index
      Assert.AreEqual("spanid1", ie.Spans[0].Id);
      Assert.AreEqual("Span1", ie.Spans[1].Id);

      IEnumerable SpanEnumerable = formSpans;
      IEnumerator SpanEnumerator = SpanEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (Span inputSpan in formSpans)
      {
        SpanEnumerator.MoveNext();
        object enumSpan = SpanEnumerator.Current;
        
        Assert.IsInstanceOfType(inputSpan.GetType(), enumSpan, "Types are not the same");
        Assert.AreEqual(inputSpan.OuterHtml, ((Span)enumSpan).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(SpanEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedSpansCount, count);
    }
        
    [Test]
    public void ElementExists()
    {
      Assert.IsTrue(ie.Div("divid").Exists);
      Assert.IsTrue(ie.Div(new Regex("divid")).Exists);
      Assert.IsFalse(ie.Button("noneexistingelementid").Exists);
    }
    
    [Test]
    public void WaitUntilElementExistsTestElementAlreadyExists()
    {
      Button button = ie.Button("disabledid");
      
      Assert.IsTrue(button.Exists);
      button.WaitUntilExists();
      Assert.IsTrue(button.Exists);
    }
    
    [Test]
    public void WaitUntilElementExistsElementInjectionAfter3Seconds()
    {
      Assert.IsTrue(IE.Settings.WaitUntilExistsTimeOut > 3, "IE.Settings.WaitUntilExistsTimeOut must be more than 3 seconds");

      using(IE ie1 = new IE(TestEventsURI))
      {
        TextField injectedTextField = ie1.TextField("injectedTextField");
        
        Assert.IsFalse(injectedTextField.Exists);
        
        ie1.Button("injectElement").ClickNoWait();

        Assert.IsFalse(injectedTextField.Exists);

        // WatiN should wait until the element exists before
        // getting the text.
        string text = injectedTextField.Text;

        Assert.IsTrue(injectedTextField.Exists);
        Assert.AreEqual("Injection Succeeded", text);
      }
    }

    [Test]
    public void WaitUntilElementRemovedAfter3Seconds()
    {
      Assert.IsTrue(IE.Settings.WaitUntilExistsTimeOut > 3, "IE.Settings.WaitUntilExistsTimeOut must be more than 3 seconds");

      using(IE ie1 = new IE(TestEventsURI))
      {
        TextField textfieldToRemove = ie1.TextField("textFieldToRemove");
        
        Assert.IsTrue(textfieldToRemove.Exists);

        ie1.Button("removeElement").ClickNoWait();

        Assert.IsTrue(textfieldToRemove.Exists);

        System.Diagnostics.Debug.WriteLine(textfieldToRemove.Text);
        
        textfieldToRemove.WaitUntilRemoved();

        Assert.IsFalse(textfieldToRemove.Exists);
      }
    }
    
    [Test, ExpectedException(typeof(WatiN.Core.Exceptions.TimeoutException), "Timeout while 'waiting 1 seconds for element to show up.'" )]
    public void WaitUntilElementExistsTimeOutException()
    {
      ie.Button("nonexistingbutton").WaitUntilExists(1);
    }
    
    [Test]
    public void WaitUntil()
    {
      MockRepository mocks = new MockRepository();

      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));
      
      Expect.Call(htmlelement.getAttribute("disabled", 0)).Return(true).Repeat.Twice();
      Expect.Call(htmlelement.getAttribute("disabled", 0)).Return(false).Repeat.Twice();

      mocks.ReplayAll();

      Element element = new Element(null, htmlelement);

      Assert.AreEqual(true.ToString(), element.GetAttributeValue("disabled"));

      // calls htmlelement.getAttribute twice (ones true and once false is returned)
      element.WaitUntil(new Core.Attribute("disabled", false.ToString()), 1);

      Assert.AreEqual(false.ToString(), element.GetAttributeValue("disabled"));

      mocks.VerifyAll();
    }

    [Test]
    public void WaitUntilExistsShouldIgnoreExceptionsDuringWait()
    {
      MockRepository mocks = new MockRepository();

      ElementFinder finder = (ElementFinder) mocks.DynamicMock(typeof (ElementFinder), null, null);
      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));

      Expect.Call(finder.FindFirst()).Return(null).Repeat.Times(5);
      Expect.Call(finder.FindFirst()).Throw(new UnauthorizedAccessException("")).Repeat.Times(4);
      Expect.Call(finder.FindFirst()).Return(htmlelement);

      Expect.Call(htmlelement.sourceIndex).Return(100);
      Expect.Call(htmlelement.offsetParent).Return(htmlelement);
      Expect.Call(htmlelement.innerText).Return("succeeded");
      
      mocks.ReplayAll();

      Element element = new Element(null, finder);

      Assert.AreEqual("succeeded", element.Text);

      mocks.VerifyAll();
    }

    [Test]
    public void WaitUntilExistsTimeOutExceptionInnerExceptionNotSetToLastExceptionThrown()
    {
      MockRepository mocks = new MockRepository();

      ElementFinder finder = (ElementFinder) mocks.DynamicMock(typeof (ElementFinder), null, null);

      Expect.Call(finder.FindFirst()).Throw(new UnauthorizedAccessException(""));
      Expect.Call(finder.FindFirst()).Return(null).Repeat.AtLeastOnce();
      
      mocks.ReplayAll();

      Element element = new Element(null, finder);

      try
      {
        element.WaitUntilExists(1);
      }
      catch (TimeoutException e)
      {
        Assert.IsNull(e.InnerException, "Unexpected innerexception");
      }

      mocks.VerifyAll();
    }

    [Test]
    public void WaitUntilExistsTimeOutExceptionInnerExceptionSetToLastExceptionThrown()
    {
      MockRepository mocks = new MockRepository();

      ElementFinder finder = (ElementFinder) mocks.DynamicMock(typeof (ElementFinder), null, null);

      Expect.Call(finder.FindFirst()).Throw(new Exception(""));
      Expect.Call(finder.FindFirst()).Throw(new UnauthorizedAccessException("mockUnauthorizedAccessException")).Repeat.AtLeastOnce();

      mocks.ReplayAll();

      Element element = new Element(null, finder);

      try
      {
        element.WaitUntilExists(1);
      }
      catch (TimeoutException e)
      {
        Assert.IsInstanceOfType(typeof(UnauthorizedAccessException), e.InnerException, "Unexpected innerexception");
        Assert.AreEqual("mockUnauthorizedAccessException", e.InnerException.Message);
      }

      mocks.VerifyAll();
    }

    [Test]
    public void WaitUntilExistsShouldReturnImmediatelyIfElementIsSet()
    {
      MockRepository mocks = new MockRepository();

      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));
      Element mockElement = (Element) mocks.DynamicMock(typeof (Element), null, htmlelement);

      Expect.Call(mockElement.Exists).Repeat.Never();
      
      mocks.ReplayAll();

      mockElement.WaitUntilExists(3);

      mocks.VerifyAll();
    }

    [Test, ExpectedException(typeof(TimeoutException), "Timeout while 'waiting 1 seconds for element attribute 'disabled' to change to 'False'.'")]
    public void WaitUntilTimesOut()
    {
      MockRepository mocks = new MockRepository();

      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));
      
      Expect.Call(htmlelement.getAttribute("disabled", 0)).Return(true).Repeat.AtLeastOnce();

      mocks.ReplayAll();

      Element element = new Element(null, htmlelement);

      Assert.AreEqual(true.ToString(), element.GetAttributeValue("disabled"));

      // calls htmlelement.getAttribute twice (ones true and once false is returned)
      element.WaitUntil(new Core.Attribute("disabled", false.ToString()), 1);
    }

    [Test]
    public void GetAttributeValueOfTypeInt()
    {
      MockRepository mocks = new MockRepository();

      IHTMLElement htmlelement = (IHTMLElement) mocks.CreateMock(typeof (IHTMLElement));
      
      Expect.Call(htmlelement.getAttribute("sourceIndex", 0)).Return(13);

      mocks.ReplayAll();

      Element element = new Element(null, htmlelement);

      Assert.AreEqual("13", element.GetAttributeValue("sourceIndex"));

      mocks.VerifyAll();
    } 
    
    [Test]
    public void FindBySourceIndex()
    {
      Button button = ie.Button(Find.ByCustom("sourceIndex", "13"));

      Assert.AreEqual("13", button.GetAttributeValue("sourceIndex"));
    }

    [Test]
    public void FindByIndex()
    {
      Assert.AreEqual("popupid", ie.Button(Find.ByIndex(0)).Id);
    }
    
    [Test]
    public void FindByShouldPassEvenIfAttributeValueOfHTMLELementIsNull()
    {
      Assert.IsFalse(ie.Button(Find.ByCustom("classname", "nullstring")).Exists);
    }
  }
  

  [TestFixture]
  public class ImageTests : WatiNTest
  {
    private IE ie = new IE(ImagesURI);

    private Uri watinwebsiteImage = new Uri(HtmlTestBaseURI, "images\\watinwebsite.jpg");
    private Uri watinwebsiteLogoImage = new Uri(HtmlTestBaseURI, "images\\watin.jpg");
    
    [TestFixtureTearDown]
    public void Teardown()
    {
      ie.Close();
    }
    
    [Test]
    public void ImageElementTags()
    {
      Assert.AreEqual(2, Image.ElementTags.Count, "2 elementtags expected");
      Assert.AreEqual("img",((ElementTag) Image.ElementTags[0]).TagName);
      Assert.AreEqual("input",((ElementTag) Image.ElementTags[1]).TagName);
      Assert.AreEqual("image",((ElementTag) Image.ElementTags[1]).InputTypes);
    }

    [Test]
    public void ImageFromElementInput()
    {
      AssertImageFromElement("Image4");
    }

    [Test]
    public void ImageFromElementImage()
    {
      AssertImageFromElement("Image2");
    }

    private void AssertImageFromElement(string id)
    {
      Element element = ie.Element(id);  
      Image image = new Image(element);
      Assert.AreEqual(id, image.Id);
    }

    [Test]
    public void ImageExists()
    {
      Assert.IsTrue(ie.Image("Image2").Exists);
      Assert.IsTrue(ie.Image(new Regex("Image2")).Exists);
      Assert.IsFalse(ie.Image("nonexistingImage").Exists);
    }

    [Test]
    public void ImageTag()
    {
      Image image = ie.Image("Image2");

      Assert.AreEqual("img", image.TagName.ToLower(), "Should be image element");
      Assert.AreEqual("Image2", image.Id, "Unexpected id");
      Assert.AreEqual("ImageName2", image.Name, "Unexpected name");
      Assert.AreEqual(watinwebsiteImage, new Uri(image.Src), "Unexpected Src");
      Assert.AreEqual("WatiN website", image.Alt, "Unexpected Alt");
    }
    
    [Test]
    public void ImageInputTag()
    {
      Image image = ie.Image("Image4");

      Assert.AreEqual("input", image.TagName.ToLower(), "Should be input element");
      Assert.AreEqual("Image4", image.Id, "Unexpected id");
      Assert.AreEqual("ImageName4", image.Name, "Unexpected name");
      Assert.AreEqual(watinwebsiteLogoImage, new Uri(image.Src), "Unexpected Src");
      Assert.AreEqual("WatiN logo in input element of type image", image.Alt, "Unexpected Alt");
    }
        
    [Test]
    public void ImageReadyStateUninitializedButShouldReturn()
    {
      Assert.IsFalse(ie.Image("Image3").Complete);
    }
    
    [Test]
    public void Images()
    {
      const int expectedImagesCount = 4;
      Assert.AreEqual(expectedImagesCount, ie.Images.Length, "Unexpected number of Images");

      // Collection.Length
      ImageCollection formImages = ie.Images;
      
      // Collection items by index
      Assert.AreEqual("Image1", ie.Images[0].Id);
      Assert.AreEqual("Image2", ie.Images[1].Id);
      Assert.AreEqual("Image3", ie.Images[2].Id);
      Assert.AreEqual("Image4", ie.Images[3].Id);

      IEnumerable ImageEnumerable = formImages;
      IEnumerator ImageEnumerator = ImageEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (Image inputImage in formImages)
      {
        ImageEnumerator.MoveNext();
        object enumImage = ImageEnumerator.Current;
        
        Assert.IsInstanceOfType(inputImage.GetType(), enumImage, "Types are not the same");
        Assert.AreEqual(inputImage.OuterHtml, ((Image)enumImage).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(ImageEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedImagesCount, count);
    }
    
    [Test]
    public void ButtonFromInputImage()
    {
      Button button = ie.Button(Find.BySrc(new Regex("images/watin.jpg")));
      
      Assert.IsTrue(button.Exists, "Button should exist");
      Assert.AreEqual("Image4", button.Id, "Unexpected id");
    }
  }
  
  [TestFixture]
  public class FormTests : WatiNTest
  {
    IE ie = new IE();
    
    [SetUp]
    public void TestSetup()
    {
      if (!ie.Uri.Equals(FormSubmitURI))
      {
        ie.GoTo(FormSubmitURI);
      }
    }
    
    [TestFixtureTearDown]
    public void FixtureTeardown()
    {
      ie.Close();
    }
    
    [Test]
    public void FormElementTags()
    {
      Assert.AreEqual(1, Form.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("form",((ElementTag) Form.ElementTags[0]).TagName);
    }

    [Test]
    public void ImageFromElementImage()
    {
      Element element = ie.Element("Form1");  
      Form form = new Form(element);
      Assert.AreEqual("Form1", form.Id);
    }

    [Test]
    public void FormExists()
    {
      Assert.IsTrue(ie.Form("Form1").Exists);
      Assert.IsTrue(ie.Form(new Regex("Form1")).Exists);
      Assert.IsFalse(ie.Form("nonexistingForm").Exists);
    }

    [Test]
    public void FormSubmit()
    {
      ie.Form("Form1").Submit();

      Assert.AreEqual(ie.Uri, MainURI);
    }
    
    [Test]
    public void FormSubmitBySubmitButton()
    {
      ie.Button("submitbutton").Click();

      Assert.AreEqual(ie.Uri, MainURI);
    }
    
    [Test]
    public void FormTest()
    {
      Form form = ie.Form("Form2");
      
      Assert.IsInstanceOfType(typeof(ElementsContainer), form);
      Assert.AreEqual("Form2", form.Id, "Unexpected Id");
      Assert.AreEqual("form2name", form.Name, "Unexpected Name");
      Assert.AreEqual("Form title", form.Title, "Unexpected Title");
    }
    
    [Test]
    public void Forms()
    {
      ie.GoTo(MainURI);

      Assert.AreEqual(6, ie.Forms.Length, "Unexpected number of forms");

      FormCollection forms = ie.Forms;

      // Collection items by index
      Assert.AreEqual("Form", forms[0].Id);
      Assert.AreEqual("FormInputElement", forms[1].Id);
      Assert.AreEqual("FormHiddens", forms[2].Id);      
      Assert.AreEqual("ReadyOnlyDisabledInputs", forms[3].Id);      
      Assert.AreEqual("FormCheckboxes", forms[4].Id);      
      Assert.AreEqual("FormRadioButtons", forms[5].Id);      

      // Collection iteration and comparing the result with Enumerator
      IEnumerable checkboxEnumerable = forms;
      IEnumerator checkboxEnumerator = checkboxEnumerable.GetEnumerator();

      int count = 0;
      foreach (Form form in forms)
      {
        checkboxEnumerator.MoveNext();
        object enumCheckbox = checkboxEnumerator.Current;
        
        Assert.IsInstanceOfType(form.GetType(), enumCheckbox, "Types are not the same");
        Assert.AreEqual(form.OuterHtml, ((Form)enumCheckbox).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(checkboxEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(6, count);
    }
    
    [Test]
    public void FormToStringWithTitleIdAndName()
    {
      Assert.AreEqual("Form title", ie.Form("Form2").ToString(), "Title expected");
      Assert.AreEqual("Form3", ie.Form("Form3").ToString(), "Id expected");
      Assert.AreEqual("form4name", ie.Form(Find.ByName("form4name")).ToString(), "Name expected");
      Assert.AreEqual("This is a form with no ID, Title or name.", ie.Forms[4].ToString(), "Text expected");
    }
  }
  
  [TestFixture]
  public class ElementStyleTests : WatiNTest
  {
    const string style = "FONT-SIZE: 12px; COLOR: white; FONT-STYLE: italic; FONT-FAMILY: Arial; HEIGHT: 50px; BACKGROUND-COLOR: blue";

    IE ie = new IE();
    TextField element;
    
    [SetUp]
    public void TestSetup()
    {
      if (!ie.Uri.Equals(MainURI))
      {
        ie.GoTo(MainURI);
        element = ie.TextField("Textarea1");
      }
    }
    
    [TestFixtureTearDown]
    public void FixtureTeardown()
    {
      ie.Close();
    }
    
    [Test]
    public void GetAttributeValueStyleAsString()
    {
      Assert.AreEqual(style, element.GetAttributeValue("style"));
    }

    [Test]
    public void ElementStyleToStringReturnsCssText()
    {
      Assert.AreEqual(style, element.Style.ToString());
    }

    [Test]
    public void ElementStyleCssText()
    {
      Assert.AreEqual(style, element.Style.CssText);
    }
    
    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void GetAttributeValueOfNullThrowsArgumenNullException()
    {
      element.Style.GetAttributeValue(null);
    }
    
    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void GetAttributeValueOfEmptyStringThrowsArgumenNullException()
    {
      element.Style.GetAttributeValue(String.Empty);
    }
    
    [Test]
    public void GetAttributeValueBackgroundColor()
    {
      Assert.AreEqual("blue", element.Style.GetAttributeValue("BackgroundColor"));
    }
    
    [Test]
    public void GetAttributeValueBackgroundColorByOriginalHTMLattribname()
    {
      Assert.AreEqual("blue", element.Style.GetAttributeValue("background-color"));
    }
    
    [Test]
    public void GetAttributeValueOfUndefiniedButValidAttribute()
    {
      Assert.IsNull(element.Style.GetAttributeValue("cursor"));
    }
    
    [Test]
    public void GetAttributeValueOfUndefiniedAndInvalidAttribute()
    {
      Assert.IsNull(element.Style.GetAttributeValue("nonexistingattrib"));
    }
    
    [Test]
    public void BackgroundColor()
    {
      Assert.AreEqual("blue", element.Style.BackgroundColor);
    }
    
    [Test]
    public void Color()
    {
      Assert.AreEqual("white", element.Style.Color);
    }

    [Test]
    public void FontFamily()
    {
      Assert.AreEqual("Arial", element.Style.FontFamily);
    }

    [Test]
    public void FontSize()
    {
      Assert.AreEqual("12px", element.Style.FontSize);
    }

    [Test]
    public void FontStyle()
    {
      Assert.AreEqual("italic", element.Style.FontStyle);
    }

    [Test]
    public void Height()
    {
      Assert.AreEqual("50px", element.Style.Height);
    }
  }

  [TestFixture]
  public class ElementTests
  {
    MockRepository mocks;
    IHTMLDOMNode node;
    Element element;

    [SetUp]
    public void Setup()
    {
      mocks = new MockRepository();
      node = (IHTMLDOMNode) mocks.CreateMock(typeof (IHTMLDOMNode));
      
      element = new Element(null, node);
    }

    [TearDown]
    public void TearDown()
    {
      mocks.VerifyAll();
    }

    [Test]
    public void ElementParentShouldReturnNullWhenRootElement()
    {
      Expect.Call(node.parentNode).Return(null);
      mocks.ReplayAll();

      Assert.IsNull(element.Parent);
    }

    [Test]
    public void ElementParentReturningTypedParent()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      string tagname = ((ElementTag) TableRow.ElementTags[0]).TagName;

      Expect.Call(node.parentNode).Return(parentNode);
      Expect.Call(((IHTMLElement)parentNode).tagName).Return(tagname).Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof(TableRow), element.Parent);
    }

    [Test]
    public void ElementParentReturnsElementsContainerForUnknowElement()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.parentNode).Return(parentNode);
      Expect.Call(((IHTMLElement)parentNode).tagName).Return("notag").Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof(ElementsContainer), element.Parent);
    }

    [Test]
    public void ElementPreviousSiblingShouldReturnWhenFirstSibling()
    {
      Expect.Call(node.previousSibling).Return(null);
      mocks.ReplayAll();

      Assert.IsNull(element.PreviousSibling);
    }

    [Test]
    public void ElementPreviousSiblingReturningTypedParent()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));
      string tagname = ((ElementTag) Link.ElementTags[0]).TagName;

      Expect.Call(node.previousSibling).Return(parentNode);
      Expect.Call(((IHTMLElement)parentNode).tagName).Return(tagname).Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof(Link), element.PreviousSibling);
    }

    [Test]
    public void ElementPreviousSiblingReturnsElementsContainerForUnknowElement()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.previousSibling).Return(parentNode);
      Expect.Call(((IHTMLElement)parentNode).tagName).Return("notag").Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof(ElementsContainer), element.PreviousSibling);
    }

    [Test]
    public void ElementNextSiblingShouldReturnWhenLastSibling()
    {
      Expect.Call(node.nextSibling).Return(null);
      mocks.ReplayAll();

      Assert.IsNull(element.NextSibling);
    }

    [Test]
    public void ElementNextSiblingReturningTypedParent()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement), typeof(IHTMLInputElement));
      ElementTag inputTextField = (ElementTag) TextField.ElementTags[0];

      Expect.Call(node.nextSibling).Return(parentNode);
      Expect.Call(((IHTMLElement)parentNode).tagName).Return(inputTextField.TagName).Repeat.Any();
      Expect.Call(((IHTMLInputElement)parentNode).type).Return(inputTextField.InputTypes).Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof(TextField), element.NextSibling);
    }

    [Test]
    public void ElementNextSiblingReturnsElementsContainerForUnknowElement()
    {
      IHTMLDOMNode parentNode = (IHTMLDOMNode) mocks.CreateMultiMock(typeof (IHTMLDOMNode), typeof (IHTMLElement));

      Expect.Call(node.nextSibling).Return(parentNode);
      Expect.Call(((IHTMLElement)parentNode).tagName).Return("notag").Repeat.Any();

      mocks.ReplayAll();

      Assert.IsInstanceOfType(typeof(ElementsContainer), element.NextSibling);
    }
  }

  [TestFixture]
  public class SelectListTests : WatiNTest
  {
    private IE ie;
    
    [TestFixtureSetUp]
    public void FixtureSetup()
    {
      ie = new IE(MainURI);
    }

    [TestFixtureTearDown]
    public void FixtureTearDown()
    {
      ie.Close();
    }

    [Test]
    public void SupportedElementTags()
    {
      Assert.AreEqual(1, SelectList.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("select",((ElementTag) SelectList.ElementTags[0]).TagName);
    }

    [Test]
    public void SelectListFromElement()
    {
      Element element = ie.Element("Select2");  
      SelectList selectList = new SelectList(element);
      Assert.AreEqual("Select2", selectList.Id);
    }

    [Test]
    public void MultipleSelectListExists()
    {
      Assert.IsTrue(ie.SelectList("Select2").Exists);
      Assert.IsTrue(ie.SelectList(new Regex("Select2")).Exists);
      Assert.IsFalse(ie.SelectList("nonexistingSelect2").Exists);
    }

    [Test]
    public void MultipleSelectList()
    {
      SelectList selectList = ie.SelectList("Select2");

      Assert.IsNotNull(selectList,"SelectList niet aangetroffen");

      StringCollection items = selectList.AllContents;

      Assert.AreEqual(4, items.Count);
      Assert.AreEqual("First Listitem",items[0],"First Listitem not found");
      Assert.AreEqual("Second Listitem",items[1],"Second Listitem not found");
      Assert.AreEqual("Third Listitem",items[2],"Third Listitem not found");
      Assert.AreEqual("Fourth Listitem",items[3],"Fourth Listitem not found");

      Assert.IsTrue(selectList.Multiple, "'Select 2' must be allow multiple selection to pass the next tests");
              
      // Second Listitem is selected by default/loading the page
      Assert.AreEqual(1,selectList.SelectedItems.Count, "SelectedItem not selected on page load");

      selectList.ClearList();
      Assert.AreEqual(0,selectList.SelectedItems.Count, "After ClearList there are still items selected");
      Assert.IsFalse(selectList.HasSelectedItems, "No selected items expected");

      selectList.Select("Third Listitem");

      Assert.IsTrue(selectList.HasSelectedItems, "Selecteditems expected after first select");
      Assert.AreEqual(1,selectList.SelectedItems.Count, "Wrong number of items selected after Select Third Listitem");
      Assert.AreEqual("Third Listitem",selectList.SelectedItems[0],"Third Listitem not selected after Select");

      selectList.Select("First Listitem");

      Assert.IsTrue(selectList.HasSelectedItems, "Selecteditems expected after second select");
      Assert.AreEqual(2,selectList.SelectedItems.Count, "Wrong number of items selected after Select First Listitem");
      Assert.AreEqual("First Listitem",selectList.SelectedItems[0],"First Listitem not selected after Select");
      Assert.AreEqual("Third Listitem",selectList.SelectedItems[1],"Third Listitem not selected after Select");
    }

    [Test, ExpectedException(typeof(SelectListItemNotFoundException), "No item with text or value 'None existing item' was found in the selectlist")]
    public void SelectItemNotFoundException()
    {
      SelectList selectList = ie.SelectList("Select1");
      selectList.Select("None existing item");
    }

    [Test, ExpectedException(typeof(SelectListItemNotFoundException))]
    public void SelectPartialTextMatchItemNotFoundException()
    {
      SelectList selectList = ie.SelectList("Select1");
      selectList.Select("Second");
    }

    [Test]
    public void SelectListCollection()
    {
      Assert.AreEqual(2, ie.SelectLists.Length);

      // Collections
      SelectListCollection selectLists = ie.SelectLists;

      Assert.AreEqual(2, selectLists.Length);

      // Collection items by index
      Assert.AreEqual("Select1", selectLists[0].Id);
      Assert.AreEqual("Select2", selectLists[1].Id);

      IEnumerable selectListEnumerable = selectLists;
      IEnumerator selectListEnumerator = selectListEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (SelectList selectList in selectLists)
      {
        selectListEnumerator.MoveNext();
        object enumSelectList = selectListEnumerator.Current;
        
        Assert.IsInstanceOfType(selectList.GetType(), enumSelectList, "Types are not the same");
        Assert.AreEqual(selectList.OuterHtml, ((SelectList)enumSelectList).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }
      
      Assert.IsFalse(selectListEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(2, count);
    }

    [Test]
    public void SingleSelectListExists()
    {
      Assert.IsTrue(ie.SelectList("Select1").Exists);
      Assert.IsTrue(ie.SelectList(new Regex("Select1")).Exists);
      Assert.IsFalse(ie.SelectList("nonexistingSelect1").Exists);
    }

    [Test]
    public void SingleSelectSelectList()
    {
      // Make sure the page is fresh so the selected item (after loading
      // the page) is the right one.
      ie.GoTo(ie.Url);
      
      SelectList selectList = ie.SelectList("Select1");

      Assert.IsNotNull(selectList,"SelectList niet aangetroffen");

      Assert.IsFalse(selectList.Multiple, "Select 1 must not allow multiple selection to pass the next tests");

      Assert.AreEqual(1,selectList.SelectedItems.Count, "Not one item selected on page load");
      // Test if the right item is selected after a page load
      Assert.AreEqual("First text", selectList.SelectedItem, "'First text' not selected on page load");
              
      selectList.ClearList();
      Assert.AreEqual(1,selectList.SelectedItems.Count, "SelectedItem should still be selected after ClearList");
      Assert.IsTrue(selectList.HasSelectedItems, "Selected item expected");

      selectList.Select("Second text");
      Assert.IsTrue(selectList.HasSelectedItems, "Selected item expected");
      Assert.AreEqual(1,selectList.SelectedItems.Count, "Unexpected count");
      Assert.AreEqual("Second text", selectList.SelectedItems[0], "Unexpected SelectedItem by index");
      Assert.AreEqual("Second text", selectList.SelectedItem, "Unexpected SelectedItem");

      selectList.SelectByValue("3");
      Assert.AreEqual("Third text", selectList.SelectedItem, "Unexpected SelectedItem");
    }

    [Test]
    public void SelectTextWithRegex()
    {
      SelectList selectList = ie.SelectList("Select1");

      selectList.Select(new Regex("cond te"));
      Assert.IsTrue(selectList.HasSelectedItems, "Selected item expected");
      Assert.AreEqual(1,selectList.SelectedItems.Count, "Unexpected count");
      Assert.AreEqual("Second text", selectList.SelectedItems[0], "Unexpected SelectedItem by index");
      Assert.AreEqual("Second text", selectList.SelectedItem, "Unexpected SelectedItem");
    }
    
    [Test]
    public void SelectValueWithRegex()
    {
      SelectList selectList = ie.SelectList("Select1");

      selectList.SelectByValue(new Regex("twee"));
      Assert.AreEqual("Second text", selectList.SelectedItem, "Unexpected SelectedItem");
    }

    [Test]
    public void OptionExists()
    {
      SelectList selectList = ie.SelectList("Select1");

      Assert.IsTrue(selectList.Options.Exists(Find.ByText("First text")));
      Assert.IsTrue(selectList.Options.Exists(Find.ByValue("tweede tekst")));

      Assert.IsTrue(selectList.Option("First text").Exists);
      Assert.IsTrue(selectList.Option(Find.ByValue("tweede tekst")).Exists);
    }

    [Test]
    public void OptionsInSingelSelectList()
    {
      SelectList selectList = ie.SelectList("Select1");
      
      Assert.IsFalse(selectList.Option("Third text").Selected);
      selectList.Option("Third text").Select();
      Assert.IsTrue(selectList.Option("Third text").Selected);
      selectList.Option("First text").SelectNoWait();
      ie.WaitForComplete();
      Assert.IsFalse(selectList.Option("Third text").Selected);
    }

    [Test]
    public void OptionsInMultiSelectList()
    {
      ie.GoTo(MainURI);

      SelectList selectList = ie.SelectList("Select2");
      
      Assert.IsFalse(selectList.Option("Third Listitem").Selected, "Third listitem is selected");
      selectList.Option("Third Listitem").Select();
      Assert.IsTrue(selectList.Option("Third Listitem").Selected, "Third listitem is not selected");
      selectList.Option("Third Listitem").Clear();
      ie.WaitForComplete();
      Assert.IsFalse(selectList.Option("Third Listitem").Selected, "Third listitem is selected #2");
    }
  }
}




