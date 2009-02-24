#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using mshtml;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Logging;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.UtilityClasses
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
            var elements = elementCollection(document);
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
            var elements = elementCollection(document);
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
            var frames = document.Frames;

            logWriter.LogAction("There are " + frames.Count + " Frames");

            var index = 0;
            foreach (Frame frame in frames)
            {
                logWriter.LogAction("Frame index: " + index);
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
            return (string.IsNullOrEmpty(value));
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
            return ((IHTMLDocument2)document.NativeDocument.Object).all;
        }

        /// <summary>
        /// Compares the class names.
        /// </summary>
        /// <param name="hWnd">The hWND of the window if which the class name should be retrieved.</param>
        /// <param name="expectedClassName">Expected name of the class.</param>
        /// <returns></returns>
        public static bool CompareClassNames(IntPtr hWnd, string expectedClassName)
        {
            if (hWnd == IntPtr.Zero)
            {
//                Console.WriteLine("   skip: Hwnd = zero");
                return false;
            }

            var className = NativeMethods.GetClassName(hWnd);
//            Console.WriteLine("   ClassName: " + className);
            return className.Equals(expectedClassName);
        }

        /// <summary>
        /// Converts the HTML element collection to a list of elements.
        /// </summary>
        /// <param name="domContainer">The DOM container</param>
        /// <param name="elementCollection">The element collection.</param>
        /// <returns>an array list with all the elements found in the element collection</returns>
        internal static IList<Element> IHtmlElementCollectionToElementList(DomContainer domContainer, IHTMLElementCollection elementCollection)
        {
            var elements = new List<Element>();
            var length = elementCollection.length;

            for (var index = 0; index < length; index++)
            {
                var nativeElement = new IEElement(elementCollection.item(index, null));
                elements.Add(ElementFactory.CreateElement(domContainer, nativeElement));
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
                throw new RunScriptException(ex);
            }
        }

        /// <summary>
        /// Fires the given event on the given element.
        /// </summary>
        /// <param name="element">Element to fire the event on</param>
        /// <param name="eventName">Name of the event to fire</param>
        public static void FireEvent(DispHTMLBaseElement element, string eventName)
        {
            var collection = new NameValueCollection {{"button", "1"}};

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
            var scriptCode = CreateJavaScriptFireEventCode(eventObjectProperties, element, eventName);

            try
            {
                var window = ((IHTMLDocument2) element.document).parentWindow;
                RunScript(scriptCode.ToString(), window);
            }
            catch (RunScriptException)
            {
                // In a cross domain automation scenario a System.UnauthorizedAccessException 
                // is thrown. The following code doesn't seem to have any effect,
                // but maybe someday MicroSoft fixes the issue... so I wrote the code anyway.
                object dummyEvt = null;
                object parentEvt = ((IHTMLDocument4) element.document).CreateEventObject(ref dummyEvt);

                var eventObj = (IHTMLEventObj2) parentEvt;

                for (var index = 0; index < eventObjectProperties.Count; index++)
                {
                    var property = eventObjectProperties.GetKey(index);
                    var value = eventObjectProperties.GetValues(index)[0];

                    eventObj.setAttribute(property, value, 0);
                }

                element.FireEvent(eventName, ref parentEvt);
            }
        }

        public static StringBuilder CreateJavaScriptFireEventCode(NameValueCollection eventObjectProperties, DispHTMLBaseElement element, string eventName)
        {
            var scriptCode = new StringBuilder();
            scriptCode.Append("var newEvt = document.createEventObject();");

            CreateJavaScriptEventObject(scriptCode, eventObjectProperties);

            scriptCode.Append("document.getElementById('" + element.uniqueID + "').fireEvent('" + eventName + "', newEvt);");
            return scriptCode;
        }

        private static void CreateJavaScriptEventObject(StringBuilder scriptCode, NameValueCollection eventObjectProperties)
        {
            if (eventObjectProperties == null) return;

            for (var index = 0; index < eventObjectProperties.Count; index++)
            {
                if (eventObjectProperties.GetKey(index) == "charCode") continue;

                scriptCode.Append("newEvt.");
                scriptCode.Append(eventObjectProperties.GetKey(index));
                scriptCode.Append(" = ");
                scriptCode.Append(eventObjectProperties.GetValues(index)[0]);
                scriptCode.Append(";");
            }
        }

        public static void AsyncActionOnBrowser(ThreadStart action)
        {
            var clickButton = new Thread(action);
            clickButton.SetApartmentState(ApartmentState.STA);
            
            clickButton.Start();
            clickButton.Join(500);
        }

        public static string StringArrayToString(string[] inputtypes, string seperator)
        {
            var inputtypesString = "";
            if (inputtypes.Length > 0)
            {
                foreach (var inputtype in inputtypes)
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

                foreach (var c in value)
                {
                    if(sendKeysCharactersToBeEscaped.IndexOf(c) != -1)
                    {
                        // Escape sendkeys special characters
                        returnvalue = returnvalue + "{" + c + "}";
                    }
                    else
                    {
                        returnvalue = returnvalue + c;
                    }
                }
                return returnvalue;
            }		

            return value;
        }

        public static Uri CreateUri(string url)
        {
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (UriFormatException)
            {
                uri = new Uri("http://" + url);
            }
            return uri;
        }


        /// <summary>
        /// Formats the string in the same sense as string.Format but checks 
        /// if args is null before calling string.Format to prevent FormatException
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string StringFormat(string format, params object[] args)
        {
            if (args != null && args.Length > 0) return string.Format(format, args);
            return format;
        }

        public static void TryActionIgnoreException(TryAction action)
        {
            try
            {
                action.Invoke();
            }
            catch { }

        }

        public static T TryFuncIgnoreException<T>(TryFunc<T> action)
        {
            try
            {
                return action.Invoke();
            }
            catch
            {
                return default(T);
            }

        }

        /// <summary>
        /// Turns the style attribute into property syntax.
        /// </summary>
        /// <example>
        /// "font-size" will turn into "fontSize"
        /// </example>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static string TurnStyleAttributeIntoProperty(string attributeName)
        {
            if (attributeName.Contains("-"))
            {
                var parts = attributeName.Split(char.Parse("-"));

                attributeName = parts[0].ToLowerInvariant();

                for (var i = 1; i < parts.Length; i++)
                {
                    var part = parts[i];
                    attributeName += part.Substring(0, 1).ToUpperInvariant();
                    attributeName += part.Substring(1).ToLowerInvariant();
                }
            }
            return attributeName;
        }

        public static void MoveMousePoinerToTopLeft(bool shouldMoveMousePointer)
        {
            if (shouldMoveMousePointer)
            {
                Cursor.Position = new Point(0, 0);
            }
        }

    }
}