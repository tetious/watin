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
	public class AttributeConstraint : BaseConstraint
	{
		private string attributeName;
		private string valueToLookFor;
		protected ICompare comparer;

		/// <summary>
		/// Initializes a new instance of the <see cref="AttributeConstraint"/> class.
		/// </summary>
		/// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
		/// <param name="value">The value to look for.</param>
		public AttributeConstraint(string attributeName, string value)
		{
			CheckArgumentNotNull("value", value);
			Init(attributeName, value, new WatiN.Core.Comparers.StringComparer(value));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AttributeConstraint"/> class.
		/// </summary>
		/// <param name="attributeName">Name of the AttributeConstraint as recognised by Internet Explorer.</param>
		/// <param name="regex">The regular expression to use.</param>
		public AttributeConstraint(string attributeName, Regex regex)
		{
			CheckArgumentNotNull("regex", regex);
			Init(attributeName, regex.ToString(), new RegexComparer(regex));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AttributeConstraint"/> class.
		/// </summary>
		/// <param name="attributeName">Name of the attribute as recognised by Internet Explorer.</param>
		/// <param name="comparer">The comparer.</param>
		public AttributeConstraint(string attributeName, ICompare comparer)
		{
			CheckArgumentNotNull("comparer", comparer);
			Init(attributeName, comparer.ToString(), comparer);
		}

		private void Init(string attributeName, string value, ICompare comparerInstance)
		{
			CheckArgumentNotNullOrEmpty("attributeName", attributeName);

			this.attributeName = attributeName;
			valueToLookFor = value;
			comparer = comparerInstance;
		}

		/// <summary>
		/// Gets the comparer used to match the expected attribute value with 
		/// the attribute value of an html element on a webpage.
		/// </summary>
		/// <value>The comparer.</value>
		public ICompare Comparer
		{
			get { return comparer; }
		}

		/// <summary>
		/// Gets the name of the attribute.
		/// </summary>
		/// <value>The name of the attribute.</value>
		public virtual string AttributeName
		{
			get { return attributeName; }
		}

		/// <summary>
		/// Gets the value to look for or the regex pattern that will be used.
		/// </summary>
		/// <value>The value.</value>
		public virtual string Value
		{
			get { return valueToLookFor; }
		}

		/// <summary>
		/// Does the compare without calling <see cref="BaseConstraint.LockCompare"/> and <see cref="BaseConstraint.UnLockCompare"/>.
		/// </summary>
		/// <param name="attributeBag">The attribute bag.</param>
		protected override bool DoCompare(IAttributeBag attributeBag)
		{
			return EvaluateAndOrAttributes(attributeBag, comparer.Compare(attributeBag.GetValue(attributeName)));
		}

        /// <summary>
        /// Use DoCompare instead.
        /// </summary>
		[Obsolete("Use DoCompare instead.")]
		protected bool doCompare(IAttributeBag attributeBag)
		{
			return DoCompare(attributeBag);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// The Value of this <see cref="AttributeConstraint" />
		/// </returns>
		public override string ToString()
		{
			return Value;
		}

        /// <summary>
        /// Writes out the constraint into a <see cref="string"/>.
        /// </summary>
        /// <returns>The constraint text</returns>
		public override string ConstraintToString()
		{
			return "Attribute '" + AttributeName + "' with value '" + Value +"'";
		}

		private static void CheckArgumentNotNullOrEmpty(string argumentName, string argumentValue)
		{
			if (UtilityClass.IsNullOrEmpty(argumentValue))
			{
				throw new ArgumentNullException(argumentName, "Null and Empty are not allowed.");
			}
		}

		private static void CheckArgumentNotNull(string argumentName, object argumentValue)
		{
			if (argumentValue == null)
			{
				throw new ArgumentNullException(argumentName, "Null is not allowed.");
			}
		}
	}
}
