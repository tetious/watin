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
using System.IO;

namespace WatiN.Core.UnitTests
{
  using NUnit.Framework.SyntaxHelpers;

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
    public static Uri FormSubmitURI = new Uri(HtmlTestBaseURI, "formsubmit.html");
    public static Uri WatiNURI = new Uri("http://watin.sourceforge.net");
    public static Uri ImagesURI = new Uri(HtmlTestBaseURI,"images.html");
    public static Uri IFramesMainURI = new Uri(HtmlTestBaseURI, "iframes\\main.html");
    public static Uri IFramesLeftURI = new Uri(HtmlTestBaseURI, "iframes\\leftpage.html");
    public static Uri IFramesMiddleURI = new Uri(HtmlTestBaseURI, "iframes\\middlepage.html");
    public static Uri IFramesRightURI = new Uri(HtmlTestBaseURI, "iframes\\rightpage.html");
    public static Uri OnBeforeUnloadJavaDialogURI = new Uri(HtmlTestBaseURI, "OnBeforeUnloadJavaDialog.html");
    public static string GoogleUrl = "http://www.google.com";
    public static string EbayUrl = "http://www.ebay.com";
    
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