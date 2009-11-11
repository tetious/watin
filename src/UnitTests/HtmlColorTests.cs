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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class HtmlColorTests
    {
        [Test]
        public void Should_handle_colors_by_their_name()
        {
            // GIVEN
            var value = "yellow";

            // WHEN
            var color = new HtmlColor(value);

            // THEN
            Assert.That(color.ToName, Is.EqualTo("Yellow"), "ToName");
            Assert.That(color.ToHexString, Is.EqualTo("#ffff00"), "ToHexString");
            Assert.That(color.ToRgbString, Is.EqualTo("rgb(255,255,0)"), "ToRgbString");
            Assert.That(color.OriginalValue, Is.EqualTo("yellow"), "OriginalValue");
        }

        [Test]
        public void Should_handle_colors_by_their_hex_code()
        {
            // GIVEN
            var value = "#008080";

            // WHEN
            var color = new HtmlColor(value);

            // THEN
            Assert.That(color.ToName, Is.EqualTo("Teal"), "ToName");
            Assert.That(color.ToHexString, Is.EqualTo("#008080"), "ToHexString");
            Assert.That(color.ToRgbString, Is.EqualTo("rgb(0,128,128)"), "ToRgbString");
            Assert.That(color.OriginalValue, Is.EqualTo("#008080"), "OriginalValue");
        }

        [Test]
        public void Should_handle_colors_by_their_compact_hex_code()
        {
            // GIVEN
            var value = "#fff"; // = #ffffff

            // WHEN
            var color = new HtmlColor(value);

            // THEN
            Assert.That(color.ToName, Is.EqualTo("White"), "ToName");
            Assert.That(color.ToHexString, Is.EqualTo("#ffffff"), "ToHexString");
            Assert.That(color.ToRgbString, Is.EqualTo("rgb(255,255,255)"), "ToRgbString");
            Assert.That(color.OriginalValue, Is.EqualTo("#fff"), "OriginalValue");
        }

        [Test]
        public void Should_handle_colors_by_their_rgb_value()
        {
            // GIVEN
            var value = "rgb(128,128,0)";

            // WHEN
            var color = new HtmlColor(value);

            // THEN
            Assert.That(color.ToName, Is.EqualTo("Olive"), "ToName");
            Assert.That(color.ToHexString, Is.EqualTo("#808000"), "ToHexString");
            Assert.That(color.ToRgbString, Is.EqualTo("rgb(128,128,0)"), "ToRgbString");
            Assert.That(color.OriginalValue, Is.EqualTo("rgb(128,128,0)"), "OriginalValue");
        }

        [Test]
        public void Should_return_unknown_for_non_w3c_color_names()
        {
            // GIVEN
            var value = "rgb(127,127,0)";

            // WHEN
            var color = new HtmlColor(value);

            // THEN
            Assert.That(color.ToName, Is.EqualTo("unknown"));
            Assert.That(color.ToHexString, Is.EqualTo("#7f7f00"));
            Assert.That(color.ToRgbString, Is.EqualTo("rgb(127,127,0)"));
        }

        [Test, ExpectedException(ExceptionName = "System.FormatException", ExpectedMessage = "Input string was not in a supported color format.")]
        public void Should_throw_exception_if_not_a_valid_color_format()
        {
            // GIVEN
            var value = "iam not a color";

            // WHEN
            new HtmlColor(value);

            // THEN exception expected
        }

        [Test]
        public void Should_be_equal_with_same_instance()
        {
            // GIVEN
            var htmlColor1 = new HtmlColor("black");

            // WHEN
            var equals = htmlColor1.Equals(htmlColor1);

            // THEN
            Assert.That(equals, Is.True);
        }

        [Test]
        public void Should_be_equal_with_different_instance_but_same_color()
        {
            // GIVEN
            var htmlColor1 = new HtmlColor("black");
            var htmlColor2 = new HtmlColor("black");

            // WHEN
            var equals = htmlColor1.Equals(htmlColor2);

            // THEN
            Assert.That(equals, Is.True);
        }

        [Test]
        public void Should_not_be_equal_to_null()
        {
            // GIVEN
            var htmlColor1 = new HtmlColor("black");

            // WHEN
            var equals = htmlColor1.Equals((object)null);

            // THEN
            Assert.That(equals, Is.False);
        }

        [Test]
        public void Should_not_be_equal_to_different_type()
        {
            // GIVEN
            var htmlColor1 = new HtmlColor("black");

            // WHEN
            var equals = htmlColor1.Equals(new object());

            // THEN
            Assert.That(equals, Is.False);
        }

        [Test]
        public void Should_be_equal_to_color_name()
        {
            // GIVEN
            // WHEN
            var result = HtmlColor.Blue.Equals("blue");
            
            // THEN
            Assert.That(result, Is.True);
        }

        [Test]
        public void Should_be_equal_to_hexcode_representation_of_color()
        {
            // GIVEN
            // WHEN
            var result = HtmlColor.Yellow.Equals("#ffff00");
            
            // THEN
            Assert.That(result, Is.True);
        }

        [Test]
        public void Should_be_equal_to_rgb_representation_of_color()
        {
            // GIVEN
            // WHEN
            var result = HtmlColor.Yellow.Equals("rgb(255,255,0)");
            
            // THEN
            Assert.That(result, Is.True);
        }

        [Test]
        public void Should_not_be_equal_to_invalid_string_input()
        {
            // GIVEN
            // WHEN
            var result = HtmlColor.Yellow.Equals("$%@)*^");
            
            // THEN
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_provide_factories_for_w3c_color_names()
        {
            // GIVEN & WHEN & THEN
            Assert.That(HtmlColor.Aqua, Is.EqualTo(new HtmlColor("Aqua")), "Aqua");
            Console.WriteLine(HtmlColor.Blue.ToRgbString);
            Assert.That(HtmlColor.Black, Is.EqualTo(new HtmlColor("Black")), "Black");
            Assert.That(HtmlColor.Blue, Is.EqualTo(new HtmlColor("Blue")), "Blue");
            Assert.That(HtmlColor.Fuchsia, Is.EqualTo(new HtmlColor("Fuchsia")), "Fuchsia");
            Assert.That(HtmlColor.Gray, Is.EqualTo(new HtmlColor("Gray")), "Gray");
            Assert.That(HtmlColor.Green, Is.EqualTo(new HtmlColor("Green")), "Green");
            Assert.That(HtmlColor.Lime, Is.EqualTo(new HtmlColor("Lime")), "Lime");
            Assert.That(HtmlColor.Maroon, Is.EqualTo(new HtmlColor("Maroon")), "Maroon");
            Assert.That(HtmlColor.Navy, Is.EqualTo(new HtmlColor("Navy")), "Navy");
            Assert.That(HtmlColor.Olive, Is.EqualTo(new HtmlColor("Olive")), "Olive");
            Assert.That(HtmlColor.Purple, Is.EqualTo(new HtmlColor("Purple")), "Purple");
            Assert.That(HtmlColor.Red, Is.EqualTo(new HtmlColor("Red")), "Red");
            Assert.That(HtmlColor.Silver, Is.EqualTo(new HtmlColor("Silver")), "Silver");
            Assert.That(HtmlColor.Teal, Is.EqualTo(new HtmlColor("Teal")), "Teal");
            Assert.That(HtmlColor.White, Is.EqualTo(new HtmlColor("White")), "White");
            Assert.That(HtmlColor.Yellow, Is.EqualTo(new HtmlColor("Yellow")), "Yellow");
        }

        [Test]
        public void To_string_should_return_all_color_formats()
        {
            // GIVEN
            // WHEN
            var toString = HtmlColor.Blue.ToString();

            // THEN
            Assert.That(toString, Is.EqualTo("Blue, #0000ff, rgb(0,0,255)"));
        }
    }
}