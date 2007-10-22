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

using System;
using System.Text.RegularExpressions;
using WatiN.Core.Comparers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core
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
	public class AttributeConstraint
	{
		private string attributeName;
		private string valueToLookFor;
		private bool busyComparing = false;

		protected ICompare comparer;
		protected AttributeConstraint andAttributeConstraint;
		protected AttributeConstraint orAttributeConstraint;
		protected AttributeConstraint lastAddedAttributeConstraint;
		protected AttributeConstraint lastAddedOrAttributeConstraint;


		// This makes the Find.ByName() & Find.By() syntax possible
		// and is needed for the && operator
		public static AttributeConstraint operator &(AttributeConstraint first, AttributeConstraint second)
		{
			return first.And(second);
		}

		// This makes the Find.ByName() | Find.By() syntax possible
		// and is needed for the || operator
		public static AttributeConstraint operator |(AttributeConstraint first, AttributeConstraint second)
		{
			return first.Or(second);
		}

		// This makes the Find.ByName() && Find.By() syntax possible
		public static bool operator true(AttributeConstraint attributeConstraint)
		{
			return false;
		}

		// This makes the Find.ByName() || Find.By() syntax possible
		public static bool operator false(AttributeConstraint attributeConstraint)
		{
			return false;
		}

		public static AttributeConstraint operator !(AttributeConstraint attributeConstraint)
		{
			return new NotAttributeConstraint(attributeConstraint);
		}

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
		/// This methode implements an exact match comparison. If you want
		/// different behaviour, inherit this class or one of its subclasses and 
		/// override Compare with a specific implementation. 
		/// <seealso cref="LockCompare"/>
		/// <seealso cref="UnLockCompare"/>
		/// </summary>
		/// <param name="attributeBag">Value to compare with</param>
		/// <returns><c>true</c> if the searched for value equals the given value</returns>
		/// <example>
		/// The following example shows the use of the LockCompare and UnlockCompare methods.
		/// <code>
		///   public override bool Compare(IAttributeBag attributeBag)
		///    {
		///      bool result = false;
		/// 
		///      // Call LockCompare if you don't call base.compare.
		///      base.LockCompare();
		///
		///      try
		///      {
		///         // Your compare code goes here.
		/// 
		///         // Don't call base.compare here because that will throw
		///         // a ReEntryException since you already locked the compare
		///         // method for recursive calls.
		///      }
		///      finally
		///      {
		///      // Call UnLockCompare if you called LockCompare.
		///        base.UnLockCompare();
		///      }      
		///
		///      return result;
		///    }
		/// </code>
		/// </example>
		public virtual bool Compare(IAttributeBag attributeBag)
		{
			LockCompare();
			bool returnValue;

			try
			{
				returnValue = doCompare(attributeBag);
			}
			finally
			{
				UnLockCompare();
			}

			return returnValue;
		}

		/// <summary>
		/// Does the compare without calling <see cref="LockCompare"/> and <see cref="UnLockCompare"/>.
		/// </summary>
		/// <param name="attributeBag">The attribute bag.</param>
		protected virtual bool doCompare(IAttributeBag attributeBag)
		{
			return EvaluateAndOrAttributes(attributeBag, comparer.Compare(attributeBag.GetValue(attributeName)));
		}

		/// <summary>
		/// Evaluates the and or attributes.
		/// </summary>
		/// <param name="attributeBag">The attribute bag.</param>
		/// <param name="initialReturnValue">if set to <c>false</c> it will skip the And evaluation.</param>
		/// <returns></returns>
		protected bool EvaluateAndOrAttributes(IAttributeBag attributeBag, bool initialReturnValue)
		{
			bool returnValue = initialReturnValue;

			if (returnValue && andAttributeConstraint != null)
			{
				returnValue = andAttributeConstraint.Compare(attributeBag);
			}

			if (returnValue == false && orAttributeConstraint != null)
			{
				returnValue = orAttributeConstraint.Compare(attributeBag);
			}

			return returnValue;
		}


		/// <summary>
		/// Call this method to unlock the compare method. Typically done at the
		/// end of your compare method. 
		/// <seealso cref="LockCompare"/>
		/// <seealso cref="Compare"/>
		/// </summary>
		protected virtual void UnLockCompare()
		{
			busyComparing = false;
		}

		/// <summary>
		/// Call this method if you override the Compare method and don't call base.compare.
		/// You should typically call this method as the first line of code in your
		/// compare method. Calling this will prevent unnoticed reentry problems, resulting
		/// in a StackOverflowException when your AttributeConstraint class is recursively used in a multiple
		/// findby scenario.
		/// <seealso cref="Compare"/>
		/// <seealso cref="UnLockCompare"/>
		/// </summary>
		/// <example>
		/// The following example shows the use of the LockCompare and UnlockCompare methods.
		/// <code>
		///   public override bool Compare(IAttributeBag attributeBag)
		///    {
		///      bool result = false;
		/// 
		///      base.LockCompare();
		///
		///      try
		///      {
		///         // Your compare code goes here.
		///         // Don't call base.compare here because that will throw
		///         // a ReEntryException since you already locked the compare
		///         // method for recursive calls.
		///      }
		///      finally
		///      {
		///        base.UnLockCompare();
		///      }      
		///
		///      return result;
		///    }
		/// </code>
		/// </example>
		protected virtual void LockCompare()
		{
			if (busyComparing)
			{
				UnLockCompare();
				throw new ReEntryException(this);
			}

			busyComparing = true;
		}

		/// <summary>
		/// Adds the specified AttributeConstraint to the And AttributeConstraint chain of a multiple <see cref="AttributeConstraint"/>
		/// element search. When calling And or using the operators, WatiN will always use
		/// ConditionAnd (&amp;&amp;) during the evaluation.
		/// <seealso cref="Or"/>
		/// </summary>
		/// <param name="attributeConstraint">The <see cref="AttributeConstraint"/> instance.</param>
		/// <returns>This <see cref="AttributeConstraint"/></returns>
		/// <example>
		/// If you want to find a Button by it's name and value this example shows you how to use
		/// the And method to do this:
		/// <code> 
		/// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
		/// 
		/// Button myButton = ie.Button(Find.ByName("buttonname").And(Find.ByValue("Button value")));
		/// </code>
		/// 
		/// You can also use the &amp; or &amp;&amp; operators, resulting in a bit more readable code.
		/// <code> 
		/// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
		/// 
		/// Button myButton = ie.Button(Find.ByName("buttonname") &amp; Find.ByValue("Button value"));
		/// </code>
		/// </example>
		public AttributeConstraint And(AttributeConstraint attributeConstraint)
		{
			if (andAttributeConstraint == null)
			{
				andAttributeConstraint = attributeConstraint;
			}
			else
			{
				lastAddedAttributeConstraint.And(attributeConstraint);
			}

			lastAddedAttributeConstraint = attributeConstraint;

			return this;
		}

		/// <summary>
		/// Adds the specified AttributeConstraint to the Or AttributeConstraint chain of a multiple <see cref="AttributeConstraint"/>
		/// element search. When calling Or or using the | or || operators, WatiN will always use
		/// ConditionOr (||) during the evaluation.
		/// <seealso cref="And"/>
		/// </summary>
		/// <param name="attributeConstraint">The <see cref="AttributeConstraint"/> instance.</param>
		/// <returns>This <see cref="AttributeConstraint"/></returns>
		/// <example>
		/// If you want to find a Button by it's English or Dutch value this example shows you how to use
		/// the Or method to do this:
		/// <code> 
		/// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
		/// 
		/// Button myButton = ie.Button(Find.ByValue("Cancel").Or(Find.ByValue("Annuleren")));
		/// </code>
		/// 
		/// You can also use the | or || operators, resulting in a bit more readable code.
		/// <code> 
		/// IE ie = new IE("www.yourwebsite.com/yourpage.htm");
		/// 
		/// Button myButton = ie.Button(Find.ByValue("Cancel") || Find.ByValue("Annuleren"));
		/// </code>
		/// </example>
		public AttributeConstraint Or(AttributeConstraint attributeConstraint)
		{
			if (orAttributeConstraint == null)
			{
				orAttributeConstraint = attributeConstraint;
			}
			else
			{
				lastAddedOrAttributeConstraint.Or(attributeConstraint);
			}

			lastAddedOrAttributeConstraint = attributeConstraint;
			lastAddedAttributeConstraint = attributeConstraint;

			return this;
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