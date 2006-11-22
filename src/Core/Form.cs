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

using WatiN.Core.Logging;

namespace WatiN.Core
{
  /// <summary>
  /// This class provides specialized functionality for a HTML Form element.
  /// </summary>
  public class Form : ElementsContainer
  {
    public Form(DomContainer ie, HTMLFormElement htmlFormElement) : base(ie, (IHTMLElement) htmlFormElement)
    {}

    public Form(DomContainer ie, ElementFinder finder) : base(ie, finder)
    {}

    public void Submit()
    {
      Logger.LogAction("Submitting " + GetType().Name + " '" + ToString() + "'");

      HtmlFormElement.submit();
      WaitForComplete();
    }

    public override string ToString()
    {
      if (UtilityClass.IsNotNullOrEmpty(Title))
      {
        return Title;
      }
      if (UtilityClass.IsNotNullOrEmpty(Id))
      {
        return Id;
      }
      if (UtilityClass.IsNotNullOrEmpty(Name))
      {
        return Name;
      }
      return base.ToString ();
    }

    public string Name
    {
      get
      {
        return HtmlFormElement.name;
      }
    }

    private HTMLFormElement HtmlFormElement
    {
      get {
        return (HTMLFormElement)HTMLElement;
      }
    }
  }
}
