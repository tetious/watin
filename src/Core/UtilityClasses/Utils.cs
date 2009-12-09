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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WatiN.Core.UtilityClasses
{
    /// <summary>
    /// Class with some utility methods to explore the HTML of a <see cref="Document"/>.
    /// </summary>
    public class UtilityClass
    {
        public delegate void DoAction();

        /// <summary>
        /// Prevent creating an instance of this class (contains only static members)
        /// </summary>
        private UtilityClass() {}

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

        public static void AsyncActionOnBrowser(ThreadStart action)
        {
            var clickButton = new Thread(action);
            clickButton.SetApartmentState(ApartmentState.STA);
            
            clickButton.Start();
            clickButton.Join(500);
        }

        public static string EscapeSendKeysCharacters(string value) 
        {
            const string sendKeysCharactersToBeEscaped = "~%^+{}[]()";

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

        public static void TryActionIgnoreException(DoAction action)
        {
            try
            {
                action.Invoke();
            }
            catch { }

        }

        public static T TryFuncIgnoreException<T>(DoFunc<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch
            {
                return default(T);
            }

        }

        public static T TryFuncFailOver<T>(DoFunc<T> func, int numberOfRetries, int sleepTime)
        {
            Exception lastException;
            do
            {
                try
                {
                    var result = func.Invoke();
                    return result;
                }
                catch (Exception e)
                {
                    lastException = e;
                }

                numberOfRetries -= 1;
                Thread.Sleep(sleepTime);
            } while (numberOfRetries!=0);

            throw lastException;
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