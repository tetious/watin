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
using System.Timers;
using mshtml;

namespace WatiN.Core
{
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
