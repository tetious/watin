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
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WatiN.Core
{
    /// <summary>
    /// This class helps converting all kinds of html color formats
    /// to one uniform <see cref="HtmlColor"/> object. IE and FireFox return
    /// differently formatted results when retrieving a color, for instance backgroundColor. This class
    /// provides a way to keep your tests browser agnostic when it comes to checking color values.
    /// </summary>
    /// <example>
    /// Following some examples of valid use of this class. These all create the same
    /// <see cref="HtmlColor"/> object:
    /// <code>
    /// var blue = new HtmlCode("blue");
    /// var blue = new HtmlCode("#0000FF");
    /// var blue = new HtmlCode("#00F");
    /// var blue = new HtmlCode("rgb(0,0,255)");
    /// </code>
    /// The class provides factory properties for creating the 16 color names defined
    /// by the W3C.
    /// <code>
    /// var blue = HtmlCode.Blue;
    /// </code>
    /// The power of this class lies in the fact that you can use it in your test
    /// expectations no matter what color format the browser returns.
    /// <code>
    /// var backgroundColor = browser.Div("with_background_color").Style.BackgroundColor;
    /// Assert.That(new HtmlColor(backgroundColor), Is.EqualTo(HtmlColor.Yellow));
    /// </code>
    /// </example>
    public class HtmlColor
    {
        private static readonly Regex RgbValuePattern = new Regex(@"(?<r>\d{1,3}) ?, ?(?<g>\d{1,3}) ?, ?(?<b>\d{1,3})",
                                                                  RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Dictionary<int, string> W3CColorNames = new Dictionary<int, string>
            {
                { -16711681, "Aqua" },      // #00FFFF
                { -16777216, "Black" },     // #000000
                { -16776961, "Blue" },      // #0000FF
                { -65281, "Fuchsia" },      // #FF00FF
                { -8355712, "Gray" },       // #808080
                { -16744448, "Green" },     // #008000
                { -16711936, "Lime" },      // #00FF00
                { -8388608, "Maroon" },     // #800000
                { -16777088, "Navy" },      // #000080
                { -8355840, "Olive" },      // #808000
                { -8388480, "Purple" },     // #800080
                { -65536, "Red" },          // #FF0000
                { -4144960, "Silver" },     // #C0C0C0
                { -16744320, "Teal" },      // #008080
                { -1, "White" },            // #FFFFFF
                { -256, "Yellow" },         // #FFFF00
            };

        public static HtmlColor Aqua { get { return new HtmlColor("Aqua"); } }
        public static HtmlColor Black { get { return new HtmlColor("Black"); } }
        public static HtmlColor Blue { get { return new HtmlColor("Blue"); } }
        public static HtmlColor Fuchsia { get { return new HtmlColor("Fuchsia"); } }
        public static HtmlColor Gray { get { return new HtmlColor("Gray"); } }
        public static HtmlColor Green { get { return new HtmlColor("Green"); } }
        public static HtmlColor Lime { get { return new HtmlColor("Lime"); } }
        public static HtmlColor Maroon { get { return new HtmlColor("Maroon"); } }
        public static HtmlColor Navy { get { return new HtmlColor("Navy"); } }
        public static HtmlColor Olive { get { return new HtmlColor("Olive"); } }
        public static HtmlColor Purple { get { return new HtmlColor("Purple"); } }
        public static HtmlColor Red { get { return new HtmlColor("Red"); } }
        public static HtmlColor Silver { get { return new HtmlColor("Silver"); } }
        public static HtmlColor Teal { get { return new HtmlColor("Teal"); } }
        public static HtmlColor White { get { return new HtmlColor("White"); } }
        public static HtmlColor Yellow { get { return new HtmlColor("Yellow"); } }

        /// <summary>
        /// Returns the <see cref="System.Drawing.Color"/> wrapped by this class.
        /// </summary>
        /// <value>The color.</value>
        public Color Color { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlColor"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="FormatException">Will be thrown if the value can't be converted
        /// to a color.</exception>
        /// <example>
        /// Following some examples of valid use of this class. These all create the same
        /// <see cref="HtmlColor"/> object:
        /// <code>
        /// var blue = new HtmlCode("blue");
        /// var blue = new HtmlCode("#0000FF");
        /// var blue = new HtmlCode("#00F");
        /// var blue = new HtmlCode("rgb(0,0,255)");
        /// </code>
        /// </example>
        public HtmlColor(string value)
        {
            OriginalValue = value;

            var match = RgbValuePattern.Match(value);

            if (match.Success)
            {
                var r = Int32.Parse(match.Groups["r"].Value, NumberFormatInfo.InvariantInfo);
                var g = Int32.Parse(match.Groups["g"].Value, NumberFormatInfo.InvariantInfo);
                var b = Int32.Parse(match.Groups["b"].Value, NumberFormatInfo.InvariantInfo);
                
                Color = Color.FromArgb(r, g, b);
            }
            else
            {
                try
                {
                    Color = ColorTranslator.FromHtml(value);
                }
                catch (Exception)
                {
                    throw new FormatException("Input string was not in a supported color format.");
                }
            }
        }

        /// <summary>
        /// Returns a descriptive W3C color name (Aqua, Black, Blue, Fuchsia, Gray, Green, Lime, 
        /// Maroon, Navy, Olive, Purple, Red, Silver, Teal, White or Yellow). If it's none
        /// of these colors, it will return "unknown". 
        /// </summary>
        /// <value>To name.</value>
        public string ToName
        {
            get { return W3CColorNames.ContainsKey(ToArgb) ? W3CColorNames[ToArgb] : "unknown"; }
        }

        private int ToArgb
        {
            get { return Color.ToArgb(); }
        }

        /// <summary>
        /// Returns the color in a html hex code formatted string
        /// </summary>
        /// <value>To hex string.</value>
        public string ToHexString
        {
            get { return "#" + ToArgb.ToString("x").Substring(2); }
        }

        /// <summary>
        /// Returns the color in a rgb formatted string.
        /// </summary>
        /// <value>To RGB string.</value>
        public string ToRgbString
        {
            get
            {
                var r = Convert.ToInt32(Color.R);
                var g = Convert.ToInt32(Color.G);
                var b = Convert.ToInt32(Color.B);
                
                return string.Format("rgb({0},{1},{2})", r, g, b);
            }
        }

        public string OriginalValue { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="HtmlColor"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="HtmlColor"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="HtmlColor"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            var thatAsHtmlColor = obj as HtmlColor;
            if (thatAsHtmlColor != null) return Equals(thatAsHtmlColor);

            var thatAsString = obj as string;
            return thatAsString != null && Equals(thatAsString);
        }

        public bool Equals(HtmlColor that)
        {
            if (that == null) return false;

            return ReferenceEquals(this, that) || ToArgb.Equals(that.ToArgb);
        }

        public bool Equals(string color)
        {
            try
            {
                var that = new HtmlColor(color);
                return Equals(that);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="HtmlColor"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="HtmlColor"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="HtmlColor"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", ToName, ToHexString, ToRgbString);
        }
    }
}