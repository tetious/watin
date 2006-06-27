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

    private static Uri googleURI;

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

    public static Uri GoogleURI
    {
      get
      {
        if(googleURI == null)
        {
          googleURI = new Uri("http://www.google.com");
        }
        return googleURI;
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