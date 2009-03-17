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
using System.IO;
using System.Text.RegularExpressions;
using WatiN.Core.Comparers;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Constraints
{
	/// <summary>
	/// This is the base class for finding elements by a specified attribute. Use
	/// this class or one of it's subclasses to implement your own comparison logic.
	/// </summary>
	/// <example>
	/// <code>ie.Link(new Attribute("id", "testlinkid")).Url</code>
	/// or use 
	/// <code>ie.Link(Find.By("id", "testlinkid")).Url</code>
	/// </example>
	public class AttributeConstraint : Constraint
	{
		private readonly string attributeName;
        private readonly Comparer<string> comparer;

	    /// <summary>
		/// Creates an attribute constraint to search for an exact match by string value.
		/// </summary>
		/// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
		/// <param name="comparisonValue">The value to look for</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attributeName"/>
        /// or <paramref name="comparisonValue"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="attributeName"/> is empty</exception>
		public AttributeConstraint(string attributeName, string comparisonValue)
            : this(attributeName, new Comparers.StringComparer(comparisonValue)) { }

        /// <summary>
        /// Creates an attribute constraint to search for a match by regular expression.
        /// </summary>
        /// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
        /// <param name="regex">The regular expression to look for</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attributeName"/>
        /// or <paramref name="regex"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="attributeName"/> is empty</exception>
        public AttributeConstraint(string attributeName, Regex regex)
            : this(attributeName, new RegexComparer(regex)) { }

        /// <summary>
        /// Creates an attribute constraint to search for a match using a custom comparer.
        /// </summary>
        /// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
        /// <param name="comparer">The attribute value comparer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attributeName"/>
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="attributeName"/> is empty</exception>
        public AttributeConstraint(string attributeName, Comparer<string> comparer)
        {
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("attributeName");
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            this.attributeName = attributeName;
            this.comparer = comparer;
        }

        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        /// <value>The name of the attribute.</value>
        public string AttributeName
        {
            get { return attributeName; }
        }

        /// <summary>
		/// Gets the comparer used to match the expected attribute value with 
		/// the actual attribute value of an html element on a webpage.
		/// </summary>
		/// <value>The comparer.</value>
		public Comparer<string> Comparer
		{
			get { return comparer; }
		}

        /// <inheritdoc />
        public override void WriteDescriptionTo(TextWriter writer)
        {
            writer.Write("Attribute '{0}' {1}", attributeName, comparer);
        }

        /// <inheritdoc />
        protected override bool MatchesImpl(IAttributeBag attributeBag, ConstraintContext context)
        {
            var attributeValue = attributeBag.GetAttributeValue(attributeName);
            return comparer.Compare(attributeValue);
        }
	}
}
