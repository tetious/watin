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
using System.Globalization;

namespace WatiN.Core.Comparers
{
	/// <summary>
	/// Class that supports an exact comparison of two string values.
	/// </summary>
	public class StringComparer : Comparer<string>
	{
		private readonly bool _ignoreCase;
		private readonly string _comparisonValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringComparer"/> class.
        /// The string comparison done by <see cref="Compare(string)"/> will ignore any case differences.
        /// </summary>
        /// <param name="expectedValue">The value used to compare against.</param>
        public StringComparer(string expectedValue) 
            : this(expectedValue, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringComparer"/> class and allows
        /// to specify the behavior regarding case sensitive comparisons.
        /// </summary>
        /// <param name="comparisonValue">The value used to compare against.</param>
        /// <param name="ignoreCase">if set to <c>false</c> <see cref="Compare(string)"/>
        /// will also check the casing of the <see cref="string"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparisonValue"/> is null</exception>
		public StringComparer(string comparisonValue, bool ignoreCase)
		{
            if (comparisonValue == null)
                throw new ArgumentNullException("comparisonValue");
           
            _ignoreCase = ignoreCase;			
			_comparisonValue = comparisonValue;
		}

        /// <summary>
        /// Gets the value to compare against.
        /// </summary>
        public string ComparisonValue
        {
            get { return _comparisonValue; }
        }

        /// <summary>
        /// Compares the specified value. 
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The result of the comparison.</returns>
		public override bool Compare(string value)
		{
			if (value == null)
                return false;

			return string.Compare(value, _comparisonValue,
                _ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture) == 0;
		}

		/// <summary>
		/// Compare the two values with <seealso cref="CultureInfo"/> set to InvariantCulture.
		/// </summary>
		/// <param name="lhs">The left hand side value.</param>
		/// <param name="rhs">The right hand side value.</param>
		/// <returns><c>true</c> or <c>false</c></returns>
		public static bool AreEqual(string lhs, string rhs)
		{
            return AreEqual(lhs, rhs, false);
		}

		/// <summary>
		/// Compare the two values with <seealso cref="CultureInfo"/> set to InvariantCulture.
		/// </summary>
		/// <param name="lhs">The left hand side value.</param>
		/// <param name="rhs">The right hand side value.</param>
		/// <param name="ignoreCase">if set to <c>true</c> it compares case insensitive.</param>
		/// <returns><c>true</c> or <c>false</c></returns>
		public static bool AreEqual(string lhs, string rhs, bool ignoreCase)
		{
            if (lhs == null)
                return false;

			return new StringComparer(lhs, ignoreCase).Compare(rhs);
		}

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("equals '{0}'{1}", _comparisonValue, _ignoreCase ? " ignoring case" : "");
        }
    }
}