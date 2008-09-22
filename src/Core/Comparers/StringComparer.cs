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
using System.Globalization;

namespace WatiN.Core.Comparers
{
	/// <summary>
	/// Class that supports an exact comparison of two string values.
	/// </summary>
	public class StringComparer : BaseComparer
	{
		private readonly bool _ignorecase;
		protected string valueToCompareWith;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringComparer"/> class.
        /// The string comparison done by <see cref="Compare(string)"/> will ignore any case differences.
        /// </summary>
        /// <param name="value">The value used to compare against.</param>
        public StringComparer(string value) : this(value, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringComparer"/> class and allows
        /// to specify the behavior regarding case sensitive comparisons.
        /// </summary>
        /// <param name="value">The value used to compare against.</param>
        /// <param name="ignorecase">if set to <c>false</c> <see cref="Compare(string)"/>
        /// will also check the casing of the <see cref="string"/>.</param>
		public StringComparer(string value, bool ignorecase)
		{
			_ignorecase = ignorecase;
			
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			valueToCompareWith = value;
		}

        /// <summary>
        /// Compares the specified value. 
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The result of the comparison.</returns>
		public override bool Compare(string value)
		{
			if (value == null) return false;

			return (String.Compare(value, valueToCompareWith, _ignorecase, CultureInfo.InvariantCulture) == 0);
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
		public override string ToString()
		{
			return valueToCompareWith;
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
            if (lhs == null) return false;

			return new StringComparer(lhs, ignoreCase).Compare(rhs);
		}
	}
}