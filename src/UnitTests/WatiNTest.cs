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
    public static Uri MainURI = new Uri(HtmlTestBaseURI, "main.html");
    public static Uri IndexURI = new Uri(HtmlTestBaseURI, "Index.html");
    public static Uri PopUpURI = new Uri(HtmlTestBaseURI, "popup.html");
    public static Uri FramesetURI = new Uri(HtmlTestBaseURI, "Frameset.html");
    public static Uri FramesetWithinFramesetURI = new Uri(HtmlTestBaseURI, "FramesetWithinFrameset.html");
    public static Uri CrossDomainFramesetURI = new Uri(HtmlTestBaseURI, "CrossDomainFrameset.html");
    public static Uri TestEventsURI = new Uri(HtmlTestBaseURI, "TestEvents.html");
    public static Uri WatiNURI = new Uri("http://watin.sourceforge.net");
    public static Uri ImagesURI = new Uri(HtmlTestBaseURI,"images.html");
    public static Uri IFramesMainURI = new Uri(HtmlTestBaseURI, "iframes\\main.html");
    public static Uri IFramesLeftURI = new Uri(HtmlTestBaseURI, "iframes\\leftpage.html");
    public static Uri IFramesMiddleURI = new Uri(HtmlTestBaseURI, "iframes\\middlepage.html");
    public static Uri IFramesRightURI = new Uri(HtmlTestBaseURI, "iframes\\rightpage.html");
    public static string GoogleUrl = "http://www.google.com";
    
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

    private static string GetHtmlTestFilesLocation()
    {
      DirectoryInfo baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
      
      // Search for the html directory in the current domains base directory
      // Valid when executing WatiN UnitTests in a deployed situation. 
      string htmlTestFilesLocation = baseDirectory.FullName + @"\html\";
      
      if (!Directory.Exists(htmlTestFilesLocation))
      {
        // If html directory not found, search two dirs up in the directory tree
        // Valid when executing WatiN UnitTests from within Visual Studio
        htmlTestFilesLocation = baseDirectory.Parent.Parent.FullName + @"\html\";
      }
      
      return htmlTestFilesLocation;
    }
  }
}