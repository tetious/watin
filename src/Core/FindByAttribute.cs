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
using System.Text.RegularExpressions;
using WatiN.Core.Constraints;

namespace WatiN.Core
{
    /// <summary>
    /// Finds a component based on its attributes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If multiple attributes are specified, then all of them must jointly match the component.
    /// If no attributes are specified, then the first component of the required type will be used.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public class HomePage : Page
    /// {
    ///     [FindBy(Name = "signIn")]
    ///     [Description("Sign in button.")]
    ///     public Button SignInButton;
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FindByAttribute : ComponentFinderAttribute
    {
        /// <summary>
        /// Finds a component based on its attributes.
        /// </summary>
        public FindByAttribute()
        {
            Index = -1;
        }

        /// <summary>
        /// Gets or sets the alt text to find.
        /// </summary>
        /// <seealso cref="Find.ByAlt(string)"/>
        public string Alt { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the alt text to find.
        /// </summary>
        /// <seealso cref="Find.ByAlt(Regex)"/>
        public string AltRegex { get; set; }

        /// <summary>
        /// Gets or sets the (CSS) class name to find.
        /// </summary>
        /// <seealso cref="Find.ByClass(string)"/>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the (CSS) class name to find.
        /// </summary>
        /// <seealso cref="Find.ByClass(Regex)"/>
        public string ClassRegex { get; set; }

        /// <summary>
        /// Gets or sets the id of the linked label element to find.
        /// </summary>
        /// <seealso cref="Find.ByFor(string)"/>
        public string For { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the id of the linked label to find.
        /// </summary>
        /// <seealso cref="Find.ByFor(Regex)"/>
        public string ForRegex { get; set; }

        /// <summary>
        /// Gets or sets the element id to find.
        /// </summary>
        /// <seealso cref="Find.ById(string)"/>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the element id to find.
        /// </summary>
        /// <seealso cref="Find.ById(Regex)"/>
        public string IdRegex { get; set; }

        /// <summary>
        /// Gets or sets the element name to find.
        /// </summary>
        /// <seealso cref="Find.ByName(string)"/>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the element name to find.
        /// </summary>
        /// <seealso cref="Find.ByName(Regex)"/>
        public string NameRegex { get; set; }

        /// <summary>
        /// Gets or sets the (inner) text to find.
        /// </summary>
        /// <seealso cref="Find.ByText(string)"/>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the (inner) text to find.
        /// </summary>
        /// <seealso cref="Find.ByText(Regex)"/>
        public string TextRegex { get; set; }

        /// <summary>
        /// Gets or sets the Url to find.
        /// </summary>
        /// <seealso cref="Find.ByUrl(string)"/>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the Url to find.
        /// </summary>
        /// <seealso cref="Find.ByUrl(Regex)"/>
        public string UrlRegex { get; set; }

        /// <summary>
        /// Gets or sets the title to find.
        /// </summary>
        /// <seealso cref="Find.ByTitle(string)"/>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the title to find.
        /// </summary>
        /// <seealso cref="Find.ByTitle(Regex)"/>
        public string TitleRegex { get; set; }

        /// <summary>
        /// Gets or sets the value to find.
        /// </summary>
        /// <seealso cref="Find.ByValue(string)"/>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the value to find.
        /// </summary>
        /// <seealso cref="Find.ByValue(Regex)"/>
        public string ValueRegex { get; set; }

        /// <summary>
        /// Gets or sets the source Url to find.
        /// </summary>
        /// <seealso cref="Find.BySrc(string)"/>
        public string Src { get; set; }

        /// <summary>
        /// Gets or sets the regular expression for the source Url to find.
        /// </summary>
        /// <seealso cref="Find.BySrc(Regex)"/>
        public string SrcRegex { get; set; }

        /// <summary>
        /// Gets or sets the zero-based index of the element to find, or -1 if not constrained by index.
        /// </summary>
        /// <seealso cref="Find.ByIndex(int)"/>
        public int Index { get; set; }

        /// <inheritdoc />
        public override Component FindComponent(Type componentType, IElementContainer container)
        {
            return ComponentFinder.FindComponent(componentType, container, GetConstraint());
        }

        /// <summary>
        /// Gets the constraint expressed by this attribute.
        /// </summary>
        /// <returns>The constraint</returns>
        protected virtual Constraint GetConstraint()
        {
            Constraint constraint = null;

            Combine(ref constraint, CreateStringConstraint(Find.ByAlt, Alt));
            Combine(ref constraint, CreateRegexConstraint(Find.ByAlt, AltRegex));
            Combine(ref constraint, CreateStringConstraint(Find.ByClass, Class));
            Combine(ref constraint, CreateRegexConstraint(Find.ByClass, ClassRegex));
            Combine(ref constraint, CreateStringConstraint(Find.ByFor, For));
            Combine(ref constraint, CreateRegexConstraint(Find.ByFor, ForRegex));
            Combine(ref constraint, CreateStringConstraint(Find.ById, Id));
            Combine(ref constraint, CreateRegexConstraint(Find.ById, IdRegex));
            Combine(ref constraint, CreateStringConstraint(Find.ByName, Name));
            Combine(ref constraint, CreateRegexConstraint(Find.ByName, NameRegex));
            Combine(ref constraint, CreateStringConstraint(Find.BySrc, Src));
            Combine(ref constraint, CreateRegexConstraint(Find.BySrc, SrcRegex));
            Combine(ref constraint, CreateStringConstraint(Find.ByText, Text));
            Combine(ref constraint, CreateRegexConstraint(Find.ByText, TextRegex));
            Combine(ref constraint, CreateStringConstraint(Find.ByTitle, Title));
            Combine(ref constraint, CreateRegexConstraint(Find.ByTitle, TitleRegex));
            Combine(ref constraint, CreateStringConstraint(Find.ByUrl, Url));
            Combine(ref constraint, CreateRegexConstraint(Find.ByUrl, UrlRegex));
            Combine(ref constraint, CreateStringConstraint(Find.ByValue, Value));
            Combine(ref constraint, CreateRegexConstraint(Find.ByValue, ValueRegex));

            if (Index != -1)
                Combine(ref constraint, Find.ByIndex(Index));

            return constraint ?? Find.Any;
        }

        private delegate Constraint StringConstraintFactory(string value);
        private delegate Constraint RegexConstraintFactory(Regex value);

        private static Constraint CreateStringConstraint(StringConstraintFactory factory, string value)
        {
            return value != null ? factory(value) : null;
        }

        private static Constraint CreateRegexConstraint(RegexConstraintFactory factory, string value)
        {
            return value != null ? factory(new Regex(value)) : null;
        }

        private static void Combine(ref Constraint constraint, Constraint otherConstraint)
        {
            if (otherConstraint != null)
            {
                constraint = constraint != null ? constraint & otherConstraint : otherConstraint;
            }
        }
    }
}
