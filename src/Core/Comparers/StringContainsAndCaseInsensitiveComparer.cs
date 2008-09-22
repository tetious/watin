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

namespace WatiN.Core.Comparers
{
	/// <summary>
	/// Class that supports a simple matching of two strings.
	/// </summary>
	public class StringContainsAndCaseInsensitiveComparer : StringComparer
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="StringContainsAndCaseInsensitiveComparer"/> class.
        /// </summary>
        /// <param name="value">The value used to compare against.</param>
		public StringContainsAndCaseInsensitiveComparer(string value) : base(value)
		{
			valueToCompareWith = value.ToLower();
		}

        /// <summary>
        /// Checks if the given <paramref name="value"/> contains the string to compare against.
        /// Comparison is done case insensitive.
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The result of the comparison.</returns>
		public override bool Compare(string value)
		{
			if (value == null)
			{
				return false;
			}

			if (valueToCompareWith == String.Empty & value != String.Empty)
			{
				return false;
			}

			return (value.ToLower().IndexOf(valueToCompareWith) >= 0);
		}
	}
}