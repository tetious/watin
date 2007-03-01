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
using System.Timers;
using mshtml;

namespace WatiN.Core
{
  using System.Collections;

  /// <summary>
	/// Class with some utility methods to explore the HTML of a <see cref="Document"/>.
	/// </summary>
	public sealed class UtilityClass
	{
    /// <summary>
    /// Prevent creating an instance of this class (contains only static members)
    /// </summary>
    private UtilityClass(){}

    public static void DumpElements(Document document)
    {
      System.Diagnostics.Debug.WriteLine("Dump:");
      IHTMLElementCollection elements = elementCollection(document);
      foreach (IHTMLElement e in elements)
      {
        System.Diagnostics.Debug.WriteLine("id = " + e.id);
      }
    }

    public static void DumpElementsWithHtmlSource(Document document)
    {
      System.Diagnostics.Debug.WriteLine("Dump:==================================================");
      IHTMLElementCollection elements = elementCollection(document);
      foreach (IHTMLElement e in elements)
      {
        System.Diagnostics.Debug.WriteLine("------------------------- " + e.id);
        System.Diagnostics.Debug.WriteLine(e.outerHTML);
      }
    }

    public static void ShowFrames(Document document)
    {
      FrameCollection frames = document.Frames;

      System.Diagnostics.Debug.WriteLine("There are " + frames.Length.ToString() + " Frames", "WatiN");
      
      int index = 0;
      foreach(Frame frame in frames)
      {
        System.Diagnostics.Debug.Write("Frame index: " + index.ToString());
        System.Diagnostics.Debug.Write(" name: " + frame.Name);
        System.Diagnostics.Debug.WriteLine(" scr: " + frame.Url);
        
        index++;
      }
    }

    /// <summary>
    /// Determines whether the specified <paramref name="value" /> is null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// 	<c>true</c> if the specified value is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty(string value)
    {
      return (value == null || value.Length == 0);
    }
	  
    /// <summary>
    /// Determines whether the specified <paramref name="value" /> is null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// 	<c>true</c> if the specified value is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNotNullOrEmpty(string value)
    {
      return !IsNullOrEmpty(value);
    }

	  public static string ToString(object theObject)
	  {
	    if (theObject == null)
	    {
	      return String.Empty;
	    }
	    
	    return theObject.ToString();
	  }
	  
    private static IHTMLElementCollection elementCollection(Document document)
    {
      return document.HtmlDocument.all;
    }

    /// <summary>
    /// Compares the class names.
    /// </summary>
    /// <param name="hWnd">The hWND of the window if which the class name should be retrieved.</param>
    /// <param name="expectedClassName">Expected name of the class.</param>
    /// <returns></returns>
	  public static bool CompareClassNames(IntPtr hWnd, string expectedClassName)
	  {
	    if (hWnd == IntPtr.Zero) return false;
	    
	    string className = NativeMethods.GetClassName(hWnd);
      
	    return className.Equals(expectedClassName);
	  }

    internal static ArrayList IHtmlElementCollectionToArrayList(IHTMLElementCollection elementCollection)
	  {
	    ArrayList elements = new ArrayList();
	    int length = elementCollection.length;

      for(int index = 0; index < length; index++)
      {
        elements.Add(elementCollection.item(index, null));
      }

	    return elements;
	  }
	}
  
  public class SimpleTimer
  {
    Timer clock = null;
    
    public SimpleTimer(int timeout)
    {
      if (timeout < 0)
      {
        throw new ArgumentOutOfRangeException("timeout", timeout, "Should be equal are greater then zero.");
      }

      if (timeout > 0)
      {
        clock = new Timer(timeout * 1000);
        clock.AutoReset = false;
        clock.Elapsed += new ElapsedEventHandler(ElapsedEvent);
        clock.Start();
      }
    }

    public bool Elapsed
    {
      get { return (clock == null); }
    }

    private void ElapsedEvent(object source, ElapsedEventArgs e)
    {
      clock.Stop();
      clock.Close();
      clock = null;
    }
  }
}
