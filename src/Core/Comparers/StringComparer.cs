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

		public StringComparer(string value) : this(value, false) {}

		public StringComparer(string value, bool ignorecase)
		{
			_ignorecase = ignorecase;
			
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			valueToCompareWith = value;
		}

		public override bool Compare(string value)
		{
			if (value == null) return false;

			return (String.Compare(value, valueToCompareWith, _ignorecase, CultureInfo.InvariantCulture) == 0);
		}

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