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

// This constraint class is kindly donated by Seven Simple Machines

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
    /// <summary>
    /// Use this class to find a form field by text on the page that is 'nearby' the field.
    /// This constraint class is kindly donated by Seven Simple Machines.
    /// </summary>
    /// <example>
    /// This shows how to find a text field near the text "User name:".
    /// <code>
    /// ie.TextField(Find.Near("User name:")).TypeText("jsmythe")
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>In building web applications, often the form elements and the text used to label them is 
    /// not intrinsically connected (with a &lt;label&gt; tag for instance). In addition the HTML
    /// that is rendered in ASP.NET can have changing id/name at each change to the ASPX page. 
    /// This makes it hard to find form elements and keep the test cases effective without a lot
    /// of re-coding and fixing.</para>
    /// <para>As a human we can look at a web page and (usually) know what information should go into
    /// a form field based on the label. This is because we visually associate nearby text to the
    /// field. This class uses the same concept by measuring proximity of the text to field elements
    /// and giving a "best guess" to the element desired.</para>
    /// <para>Some caveats:</para>
    /// <list type="number">
    /// <item>Currently this class assume left-to-right layout. A future enhancement could look at 
    /// the current CultureInfo or support setting a culture on the constructor.</item>
    /// <item>This will <em>always</em> find a form element (if any exist on the page) for the given text
    /// if the text can be found. This isn't exactly what we as humans do. A future enhancement could
    /// change the algorithm to identify the closest text that appears to label the field for all fields.</item>
    /// <item>This only supports &lt;input&gt; and &lt;textarea&gt; elements (text fields, check box, 
    /// radio button, etc.)</item>
    /// <item>The text to look for must be only text - it may not contain HTML elements. If it does,
    /// the search method will throw an exception to warn you.</item>
    /// </list>
    /// </remarks>
    public class ProximityTextConstraint : Constraint
    {
        private readonly string labelText;

        // No point in including the whole System.Drawing lib just for this.		

        /// <summary>
        /// Initializes a new promixity constraint.
        /// </summary>
        /// <param name="labelText">The text that represents the label for the form element.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="labelText"/> is null</exception>
        public ProximityTextConstraint(string labelText)
        {
            if (labelText == null)
                throw new ArgumentNullException("labelText");

            this.labelText = labelText;
        }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            var element = attributeBag.GetAdapter<Element>();
            if (element == null)
                throw new WatiNException("This constraint class can only be used to compare against an element");

            var cache = (ProximityCache) context.GetData(this);
            if (cache == null)
            {
                cache = new ProximityCache(labelText);
                context.SetData(this, cache);
            }

            return cache.IsMatch(element);
        }

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("Near Text '{0}'", labelText);
        }

        private sealed class ProximityCache
        {
            private readonly string labelText;
            private string nearestElementId;
            private bool populated;

            public ProximityCache(string labelText)
            {
                this.labelText = labelText;
            }

            public bool IsMatch(Element element)
            {
                if (!populated)
                {
                    var nearestElement = FindNearestElement(element.DomContainer);
                    if (nearestElement != null)
                        nearestElementId = nearestElement.Id;

                    populated = true;
                }

                return nearestElementId != null && nearestElementId == element.Id;
            }

            private Element FindNearestElement(Document container)
            {
                var labelBounds = new List<Rectangle>(container.NativeDocument.GetTextBounds(labelText));

                var elementBounds = new List<KeyValuePair<Element, Rectangle>>();
                AddElementBounds(elementBounds, container.Buttons);
                AddElementBounds(elementBounds, container.CheckBoxes);
                AddElementBounds(elementBounds, container.FileUploads);
                AddElementBounds(elementBounds, container.RadioButtons);
                AddElementBounds(elementBounds, container.SelectLists);
                AddElementBounds(elementBounds, container.TextFields);

                // Here's a description of the approach taken:
                // This method calculates and tallies the distance between the nearest edges of the rectangles
                // for each form element and each text node's bounding box
                // It records the form element that is closest to a text node (shortest distance) as it traveses the sets
                // The logic here is that the distance between midpoints is going to determine the nearest item

                var shortestDistance = int.MaxValue;
                Element nearestElement = null;

                foreach (var labelRect in labelBounds)
                {
                    foreach (var elementPair in elementBounds)
                    {
                        var element = elementPair.Key;
                        var elementRect = elementPair.Value;

                        /* We need to look at distance between nearest faces to handle cases like this:
                         * 
                         * Long field label: [====================================================================]
                         * 
                         * Short: [=]
                         * 
                         * Incidentally, in almost all cases this should be normal to the faces (and therefore
                         * a faster calcultaion) because labels and fields are laid out in rows and columns.
                         * 
                         */

                        var distance = DistanceBetweenRectangles(labelRect, elementRect);

                        /*
                         * However, may not win in this situation[1]:
                         * 
                         * User Name:
                         * [=====================]
                         * Password:
                         * [=====================]
                         * Email:
                         * [=====================]
                         * 
                         * We need to pull out a rule based on the culture that forces the field to be after (right of
                         * or below) the label. This applies to LTR languages and should reverse the 'right of' 
                         * constraint for RTL languages. I don't know if there are any culture that need to reverse the 
                         * 'below' constraint.
                         * 
                         * Also, for radio buttons and check boxes the 'right' constraint should be dropped. The labels
                         * are usually right of the button, but not always:
                         * 
                         * I am: (o) Male  ( ) Female  ( ) Other
                         * I am looking for: [ ] Men  [ ] Women  [ ] Long walks on the beach
                         * 
                         * [1] There is a test case in the UnitTest\ProximityTests for this case and it passes
                         * 
                        */

                        if (distance >= shortestDistance) continue;
                        
                        shortestDistance = distance;
                        nearestElement = element;
                    }
                }

                return nearestElement;
            }

            private static void AddElementBounds<TElement>(ICollection<KeyValuePair<Element, Rectangle>> elementBounds, IEnumerable<TElement> elements)
                where TElement : Element
            {
                foreach (var element in elements)
                    elementBounds.Add(new KeyValuePair<Element, Rectangle>(element, element.NativeElement.GetElementBounds()));
            }

            /// <summary>
            /// Quick method to calculate squared distance between two points.
            /// </summary>
            /// <param name="x1">X-coordinate of the first point</param>
            /// <param name="y1">Y-coordinate of the first point</param>
            /// <param name="x2">X-coordinate of the second point</param>
            /// <param name="y2">Y-coordinate of the second point</param>
            /// <returns></returns>
            private static int CalculateSquaredDistance(int x1, int y1, int x2, int y2)
            {
                var width = x1 - x2;
                var height = y1 - y2;
                return width * width + height * height;
            }

            /// <summary>
            /// Returns the shortest distance between two rectangles.
            /// </summary>
            /// <param name="r1">The first rectangle</param>
            /// <param name="r2">The seconed rectangle</param>
            /// <returns>The shoutest distance between the nearest faces or vetices</returns>
            private static int DistanceBetweenRectangles(Rectangle r1, Rectangle r2)
            {
                /*
                 * Because the rectangles are right, all faces will either be perpendicular or parallel.
                 * 
                 * If the rectangles overlap horizontally or vertically, then the nearest distance is the 
                 * length of the normal between the two nearest parallel faces:
                 * 
                 *    +--------------+
                 *    |              |========+-------------+
                 *    |              |        |             |
                 *    +--------------+        |             |
                 *                            +-------------+
                 * 
                 * If the rectangles do not overlap at all then the distance is not a normal but
                 * merely the direct distance between the two nearest vertices:
                 * 
                 *    +--------------+
                 *    |              |
                 *    +--------------+****
                 *                        ****+-------------+
                 *                            |             |
                 *                            +-------------+
                 * 
                 */

                // Are rectangles overlapped at all?
                if ((r1.Top > r2.Top && r1.Top < r2.Bottom)
                    || (r2.Top > r1.Top && r2.Top < r1.Bottom)
                        || (r1.Left > r2.Left && r1.Left < r2.Right)
                            || (r2.Left > r1.Left && r2.Left < r1.Right))
                {
                    // Normal distance between nearest parallel faces
                    var shortestDistance = int.MaxValue;

                    var distance = Math.Abs(r1.Left - r2.Left);
                    if (distance < shortestDistance) shortestDistance = distance;

                    distance = Math.Abs(r1.Left - r2.Right);
                    if (distance < shortestDistance) shortestDistance = distance;

                    distance = Math.Abs(r1.Right - r2.Left);
                    if (distance < shortestDistance) shortestDistance = distance;

                    distance = Math.Abs(r1.Right - r2.Right);
                    if (distance < shortestDistance) shortestDistance = distance;

                    distance = Math.Abs(r1.Top - r2.Top);
                    if (distance < shortestDistance) shortestDistance = distance;

                    distance = Math.Abs(r1.Top - r2.Bottom);
                    if (distance < shortestDistance) shortestDistance = distance;

                    distance = Math.Abs(r1.Bottom - r2.Top);
                    if (distance < shortestDistance) shortestDistance = distance;

                    distance = Math.Abs(r1.Bottom - r2.Bottom);
                    if (distance < shortestDistance) shortestDistance = distance;

                    return shortestDistance;
                }
                
                // Distance between nearest vertices
                var shortestSquaredDistance = int.MaxValue;

                var squaredDistance = CalculateSquaredDistance(r1.Left, r1.Top, r2.Left, r2.Top);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Left, r1.Top, r2.Right, r2.Top);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Left, r1.Top, r2.Left, r2.Bottom);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Left, r1.Top, r2.Right, r2.Bottom);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;


                squaredDistance = CalculateSquaredDistance(r1.Right, r1.Top, r2.Left, r2.Top);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Right, r1.Top, r2.Right, r2.Top);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Right, r1.Top, r2.Left, r2.Bottom);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Right, r1.Top, r2.Right, r2.Bottom);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;


                squaredDistance = CalculateSquaredDistance(r1.Left, r1.Bottom, r2.Left, r2.Top);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Left, r1.Bottom, r2.Right, r2.Top);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Left, r1.Bottom, r2.Left, r2.Bottom);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Left, r1.Bottom, r2.Right, r2.Bottom);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;


                squaredDistance = CalculateSquaredDistance(r1.Right, r1.Bottom, r2.Left, r2.Top);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Right, r1.Bottom, r2.Right, r2.Top);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Right, r1.Bottom, r2.Left, r2.Bottom);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                squaredDistance = CalculateSquaredDistance(r1.Right, r1.Bottom, r2.Right, r2.Bottom);
                if (squaredDistance < shortestSquaredDistance) shortestSquaredDistance = squaredDistance;

                return (int) Math.Ceiling(Math.Sqrt(shortestSquaredDistance));
            }
        }
    }
}