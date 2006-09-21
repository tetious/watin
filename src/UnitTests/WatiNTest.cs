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
using System.IO;

namespace WatiN.UnitTests
{
  public class WatiNTest
  {
    private static Uri htmlTestBaseURI = null;
    private static Uri mainURI = null;
    private static Uri indexURI = null;
    private static Uri popUpURI = null;
    private static Uri framesetURI = null;
    private static Uri framesetWithinFramesetURI = null;
    private static Uri crossDomainFramesetURI = null;
    private static Uri testEventsURI = null;
    private static Uri watinURI = null;

    public static Uri IFramesMainURI = new Uri(HtmlTestBaseURI, "iframes\\main.html");
    public static Uri iframesLeftURI = new Uri(HtmlTestBaseURI, "iframes\\leftpage.html");
    public static Uri iframesMiddleURI = new Uri(HtmlTestBaseURI, "iframes\\middlepage.html");
    public static Uri iframesRightURI = new Uri(HtmlTestBaseURI, "iframes\\rightpage.html");

    
    public static Uri HtmlTestBaseURI
    {
      get
      {
        if (htmlTestBaseURI == null)
        {
          htmlTestBaseURI = new Uri(GetHtmlTestFilesLocation());
        }
        return htmlTestBaseURI;
      }
    }

    public static Uri MainURI
    {
      get
      {
        if (mainURI == null)
        {
          mainURI = new Uri(HtmlTestBaseURI, "main.html");
        }
        return mainURI;
      }
    }

    public static Uri IndexURI
    {
      get
      {
        if (indexURI == null)
        {
          indexURI = new Uri(HtmlTestBaseURI, "Index.html");
        }

        return indexURI;
      }
    }

    public static Uri PopUpURI
    {
      get
      {
        if(popUpURI == null)
        {
          popUpURI = new Uri(HtmlTestBaseURI, "popup.html");
        }
        return popUpURI;
      }
    }
    
    public static Uri FramesetURI
    {
      get
      {
        if(framesetURI == null)
        {
          framesetURI = new Uri(HtmlTestBaseURI, "Frameset.html");
        }
        return framesetURI;
      }
    }
        
    public static Uri CrossDomainFramesetURI
    {
      get
      {
        if(crossDomainFramesetURI == null)
        {
          crossDomainFramesetURI = new Uri(HtmlTestBaseURI, "CrossDomainFrameset.html");
        }
        return crossDomainFramesetURI;
      }
    }
    
    public static Uri FramesetWithinFramesetURI
    {
      get
      {
        if(framesetWithinFramesetURI == null)
        {
          framesetWithinFramesetURI = new Uri(HtmlTestBaseURI, "FramesetWithinFrameset.html");
        }
        return framesetWithinFramesetURI;
      }
    }
    
    public static Uri TestEventsURI
    {
      get
      {
        if (testEventsURI == null)
        {
          testEventsURI = new Uri(HtmlTestBaseURI, "TestEvents.html");
        }

        return testEventsURI;
      }
    }
    
    public static Uri WatiNURI
    {
      get
      {
        if (watinURI == null)
        {
          watinURI = new Uri(HtmlTestBaseURI, "http://watin.sourceforge.net");
        }

        return watinURI;
      }
    }

    public static string googleUrl
    {
      get
      {
        return "http://www.google.com";
      }
    }

    public static string GetHtmlTestFilesLocation()
    {
      DirectoryInfo baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
      
      // Search for the html directory in the current domains base directory
      // Valid when executing WatiN UnitTests in a deployed situation. 
      string htmlTestFilesLocation = baseDirectory.FullName + @"\html\";
      
      if (!Directory.Exists(htmlTestFilesLocation))
      {
        // If html dirctory not found, search two dirs up in the directory tree
        // Valid when executing WatiN UnitTests from within Visual Studio
        htmlTestFilesLocation = baseDirectory.Parent.Parent.FullName + @"\html\";
      }
      
      return htmlTestFilesLocation;
    }
  }
}