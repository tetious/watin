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

using System;

namespace WatiN.Core.Exceptions
{
  /// <summary>
  /// Base class for Exceptions thrown by WatiN.
  /// </summary>
  public class WatiNException : Exception
  {
    private string message = "";

    public WatiNException() : base()
    {
    }

    public WatiNException(string message) : base()
    {
      this.message = message;
    }

    public override string Message
    {
      get { return message; }
    }
  }

  /// <summary>
  /// Thrown if no more alerts are available when calling PopUpWatcher.PopAlert.
  /// </summary>
  public class MissingAlertException : WatiNException
  {}

  /// <summary>
  /// Thrown if the searched for element can't be found.
  /// </summary>
  public class ElementNotFoundException : WatiNException
  {
    public ElementNotFoundException(string tagName, string attributeName, string value) : 
      base("Could not find a '" + UtilityClass.ToString(tagName) + "' tag containing attribute " + attributeName + " with value '" + value + "'")
    {}
  }

  /// <summary>
  /// Thrown if the searched for frame can't be found.
  /// </summary>
  public class FrameNotFoundException : WatiNException
  {
    public FrameNotFoundException(string attributeName, string value) : 
      base("Could not find a Frame or IFrame by " + attributeName + " with value '" + value + "'")
    {}
  }

  /// <summary>
  /// Thrown if the searched for HtmlDialog can't be found.
  /// </summary>
  public class HtmlDialogNotFoundException : WatiNException
  {
    public HtmlDialogNotFoundException(string attributeName, string value, int waitTimeInSeconds) : 
      base("Could not find a HTMLDialog by " + attributeName + " with value '" + value + "'. (Search expired after '" + waitTimeInSeconds.ToString() + "' seconds)")
    {}
  }

  /// <summary>
  /// Thrown if the searched for internet explorer (IE) can't be found.
  /// </summary>
  public class IENotFoundException : WatiNException
  {
    public IENotFoundException(string findBy, string value, int waitTimeInSeconds) : 
      base("Could not find an IE window by " + findBy + " with value '" + value + "'. (Search expired after '" + waitTimeInSeconds.ToString() + "' seconds)")
    {}
  }

  /// <summary>
  /// Thrown if an element is disabled and the current action (like clicking a
  /// disabled link) is not allowed.
  /// </summary>
  public class ElementDisabledException : WatiNException
  {
    public ElementDisabledException(string elementId) : 
      base("Element with Id:" + elementId + " is disabled")
    {}
  }

  /// <summary>
  /// Thrown if an element is readonly and the current action (like TextField.TypeText) a
  /// is not allowed.
  /// </summary>
  public class ElementReadOnlyException : WatiNException
  {
    public ElementReadOnlyException(string elementId) : 
      base("Element with Id:" + elementId + " is readonly")
    {}
  }

  /// <summary>
  /// Thrown if the searched for selectlist item (option) can't be found.
  /// </summary>
  public class SelectListItemNotFoundException : WatiNException
  {
    public SelectListItemNotFoundException(string value) : 
      base("No item with text or value '" + value + "' was found in the selectlist")
    {}
  }

  /// <summary>
  /// Thrown if waiting for a webpage or element times out.
  /// </summary>
  public class TimeoutException : WatiNException
  {
    public TimeoutException(string value) : base("Timeout while '" + value + "'")
    {}
  }
  
  /// <summary>
  /// Thrown if the specified attribute isn't a valid attribute of the element. 
  /// For example doing <c>TextField.GetAttribute("src")</c> will throw 
  /// this exception.
  /// </summary>
  public class InvalidAttributException : WatiNException
  {
    public InvalidAttributException(string atributeName, string elementTag) : 
      base("Invalid attribute '" +atributeName + "' for element '" + elementTag +"'" )
    {}
  }
  
  public class ReferenceCountException : WatiNException
  {
  }
}
