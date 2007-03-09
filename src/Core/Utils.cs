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
  using WatiN.Core.Interfaces;
  using WatiN.Core.Logging;

  /// <summary>
	/// Class with some utility methods to explore the HTML of a <see cref="Document"/>.
	/// </summary>
	public sealed class UtilityClass
	{
    /// <summary>
    /// Prevent creating an instance of this class (contains only static members)
    /// </summary>
    private UtilityClass(){}

    /// <summary>
    /// Dumps all element ids to <see cref="DebugLogWriter"/>
    /// </summary>
    /// <param name="document">The document.</param>
    public static void DumpElements(Document document)
    {
      DumpElements(document, new DebugLogWriter());
    }

    /// <summary>
    /// Dumps the element ids.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="logWriter">The log writer.</param>
    public static void DumpElements(Document document, ILogWriter logWriter)
    {
      logWriter.LogAction("Dump:");
      IHTMLElementCollection elements = elementCollection(document);
      foreach (IHTMLElement e in elements)
      {
        logWriter.LogAction("id = " + e.id);
      }
    }

    /// <summary>
    /// Dumps the elements with HTML source to <see cref="DebugLogWriter"/>
    /// </summary>
    /// <param name="document">The document.</param>
    public static void DumpElementsWithHtmlSource(Document document)
    {
      DumpElementsWithHtmlSource(document, new DebugLogWriter());
    }

    /// <summary>
    /// Dumps the elements with HTML source.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="logWriter">The log writer.</param>
    public static void DumpElementsWithHtmlSource(Document document, ILogWriter logWriter)
    {
      logWriter.LogAction("Dump:==================================================");
      IHTMLElementCollection elements = elementCollection(document);
      foreach (IHTMLElement e in elements)
      {
        logWriter.LogAction("------------------------- " + e.id);
        logWriter.LogAction(e.outerHTML);
      }
    }

    /// <summary>
    /// Dumps frame info to <see cref="DebugLogWriter"/>
    /// </summary>
    /// <param name="document">The document.</param>
    public static void DumpFrames(Document document)
    {
      DumpFrames(document, new DebugLogWriter());
    }

    /// <summary>
    /// Dumps frame info.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="logWriter">The log writer.</param>
    public static void DumpFrames(Document document, ILogWriter logWriter)
    {
      FrameCollection frames = document.Frames;

      logWriter.LogAction("There are " + frames.Length.ToString() + " Frames");
      
      int index = 0;
      foreach(Frame frame in frames)
      {
        logWriter.LogAction("Frame index: " + index.ToString());
        logWriter.LogAction(" name: " + frame.Name);
        logWriter.LogAction(" scr: " + frame.Url);
        
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

    /// <summary>
    /// Returns the sring value of the <paramref name="theObject" />
    /// and returns an empty string if <paramref name="theObject" /> is <c>null</c>
    /// </summary>
    /// <param name="theObject">The object.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Converts the HTML element collection to an array list.
    /// </summary>
    /// <param name="elementCollection">The element collection.</param>
    /// <returns>an array list with all the elements found in the element collection</returns>
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
  
  /// <summary>
  /// This class provides a simple way to handle loops that have to time out after 
  /// a specified number of seconds.
  /// </summary>
  /// <example>
  /// This is an example how you could use this class in your code.
  /// <code>
  /// // timer should elapse after 30 seconds
  /// SimpleTimer timeoutTimer = new SimpleTimer(30);
  ///
  /// do
  /// {
  ///   // Your check logic goes here
  ///   
  ///   // wait 200 miliseconds
  ///   Thread.Sleep(200);
  /// } while (!timeoutTimer.Elapsed);
  /// </code>
  /// </example>
  public class SimpleTimer
  {
    Timer clock = null;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleTimer"/> class.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
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

    /// <summary>
    /// Gets a value indicating whether this <see cref="SimpleTimer"/> is elapsed.
    /// </summary>
    /// <value><c>true</c> if elapsed; otherwise, <c>false</c>.</value>
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
