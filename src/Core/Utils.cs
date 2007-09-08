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

namespace WatiN.Core
{
  using System;
  using System.Collections;
  using System.Collections.Specialized;
  using System.Text;
  using System.Timers;
  using mshtml;
  using WatiN.Core.Exceptions;
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
    private UtilityClass()
    {
    }

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
      foreach (Frame frame in frames)
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

      for (int index = 0; index < length; index++)
      {
        elements.Add(elementCollection.item(index, null));
      }

      return elements;
    }

    /// <summary>
    /// Runs the javascript code in IE.
    /// </summary>
    /// <param name="scriptCode">The javascript code.</param>
    /// <param name="window">The parent window of the document.</param>
    public static void RunScript(string scriptCode, IHTMLWindow2 window)
    {
      RunScript(scriptCode, "javascript", window);
    }

    /// <summary>
    /// Runs the script code in IE.
    /// </summary>
    /// <param name="scriptCode">The script code.</param>
    /// <param name="language">The language.</param>
    /// <param name="window">The parent window of the document.</param>
    public static void RunScript(string scriptCode, string language, IHTMLWindow2 window)
    {
      try
      {
        window.execScript(scriptCode, language);
      }
      catch (Exception ex)
      {
        throw new WatiN.Core.Exceptions.RunScriptException(ex);
      }
    }

    /// <summary>
    /// Fires the given event on the given element.
    /// </summary>
    /// <param name="element">Element to fire the event on</param>
    /// <param name="eventName">Name of the event to fire</param>
    public static void FireEvent(DispHTMLBaseElement element, string eventName)
    {
      NameValueCollection collection = new NameValueCollection();
      collection.Add("button", "1");

      FireEvent(element, eventName, collection);
    }

    /// <summary>
    /// Fires the given event on the given element.
    /// </summary>
    /// <param name="element">Element to fire the event on</param>
    /// <param name="eventName">Name of the event to fire</param>
    /// <param name="eventObjectProperties">The event object properties.</param>
    public static void FireEvent(DispHTMLBaseElement element, string eventName, NameValueCollection eventObjectProperties)
    {
      // TODO: Passing the eventarguments in a new param of type array. This array
      //       holds 0 or more name/value pairs where the name is a property of the event object
      //       and the value is the value that's assigned to the property.

      // Execute the JScript to fire the event inside the Browser.
      StringBuilder scriptCode = new StringBuilder();
      scriptCode.Append("var newEvt = document.createEventObject();");

      for (int index = 0; index < eventObjectProperties.Count; index++)
      {
        scriptCode.Append("newEvt.");
        scriptCode.Append(eventObjectProperties.GetKey(index));
        scriptCode.Append(" = ");
        scriptCode.Append(eventObjectProperties.GetValues(index)[0]);
        scriptCode.Append(";");
      }

      scriptCode.Append("document.getElementById('" + element.uniqueID + "').fireEvent('" + eventName + "', newEvt);");

      try
      {
        IHTMLWindow2 window = ((IHTMLDocument2) element.document).parentWindow;
        RunScript(scriptCode.ToString(), window);
      }
      catch (RunScriptException)
      {
        // In a cross domain automation scenario a System.UnauthorizedAccessException 
        // is thrown. The following code doesn't seem to have any effect,
        // but maybe someday MicroSoft fixes the issue... so I wrote the code anyway.
        object dummyEvt = null;
        object parentEvt = ((IHTMLDocument4) element.document).CreateEventObject(ref dummyEvt);

        IHTMLEventObj2 eventObj = (IHTMLEventObj2) parentEvt;

        for (int index = 0; index < eventObjectProperties.Count; index++)
        {
          string property = eventObjectProperties.GetKey(index);
          string value = eventObjectProperties.GetValues(index)[0];

          eventObj.setAttribute(property, value, 0);
        }

        element.FireEvent(eventName, ref parentEvt);
      }
    }

    public static string StringArrayToString(string[] inputtypes, string seperator)
    {
      string inputtypesString = "";
      if (inputtypes.Length > 0)
      {
        foreach (string inputtype in inputtypes)
        {
          inputtypesString += inputtype + seperator;
        }
        inputtypesString.Remove(inputtypesString.Length - seperator.Length, 1);
      }
      return inputtypesString;
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
    private Timer clock = null;

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
        clock = new Timer(timeout*1000);
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