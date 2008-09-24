#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using mshtml;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace WatiN.Core
{
	/// <summary>
	/// Class with some utility methods to explore the HTML of a <see cref="Document"/>.
	/// </summary>
	public class UtilityClass
	{
		/// <summary>
		/// Prevent creating an instance of this class (contains only static members)
		/// </summary>
		private UtilityClass() {}

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
		    StringBuilder scriptCode = CreateJavaScriptFireEventCode(eventObjectProperties, element, eventName);

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

	    public static StringBuilder CreateJavaScriptFireEventCode(NameValueCollection eventObjectProperties, DispHTMLBaseElement element, string eventName)
	    {
	        StringBuilder scriptCode = new StringBuilder();
	        scriptCode.Append("var newEvt = document.createEventObject();");

	        CreateJavaScriptEventObject(scriptCode, eventObjectProperties);

	        scriptCode.Append("document.getElementById('" + element.uniqueID + "').fireEvent('" + eventName + "', newEvt);");
	        return scriptCode;
	    }

	    private static void CreateJavaScriptEventObject(StringBuilder scriptCode, NameValueCollection eventObjectProperties)
	    {
            if (eventObjectProperties == null) return;

            for (int index = 0; index < eventObjectProperties.Count; index++)
	        {
	            scriptCode.Append("newEvt.");
	            scriptCode.Append(eventObjectProperties.GetKey(index));
	            scriptCode.Append(" = ");
	            scriptCode.Append(eventObjectProperties.GetValues(index)[0]);
	            scriptCode.Append(";");
	        }
	    }

        public static void AsyncActionOnBrowser(ThreadStart action)
        {
            Thread clickButton = new Thread(action);
#if !NET11
            clickButton.SetApartmentState(ApartmentState.STA);
#else
            clickButton.ApartmentState = ApartmentState.STA;
#endif
            clickButton.Start();
            clickButton.Join(500);
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

		public static string EscapeSendKeysCharacters(string value) 
		{
			const string sendKeysCharactersToBeEscaped = "~%^+{}[]";

			if(value.IndexOfAny(sendKeysCharactersToBeEscaped.ToCharArray()) > -1)
			{
				string returnvalue = null;

				foreach (char c in value)
				{
					if(sendKeysCharactersToBeEscaped.IndexOf(c) != -1)
					{
						// Escape sendkeys special characters
						returnvalue = returnvalue + "{" + c.ToString() + "}";
					}
					else
					{
						returnvalue = returnvalue + c.ToString();
					}
				}
				return returnvalue;
			}		

			return value;
		}
	}
}