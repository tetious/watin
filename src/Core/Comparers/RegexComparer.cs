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

namespace WatiN.Core.Comparers
{
	/// <summary>
	/// This class supports matching a regular expression with a string value.
	/// </summary>
	public class RegexComparer : Comparer<string>
	{
		private readonly Regex regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexComparer"/> class.
        /// </summary>
        /// <param name="regex">The regex to be used by the <see cref="Compare(string)"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regex"/> is null</exception>
		public RegexComparer(Regex regex)
		{
			if (regex == null)
				throw new ArgumentNullException("regex");

			this.regex = regex;
		}

        /// <summary>
        /// Gets the regular expression to compare against.
        /// </summary>
        protected Regex Regex
        {
            get { return regex; }
        }

        /// <summary>
        /// Matches the given value with the regex. You can override this method
        /// and provide your own implementation for the comparison with the
        /// given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Should return <c>true</c> or <c>false</c>, which is the default.
        /// </returns>
		public override bool Compare(string value)
		{
			if (value == null)
                return false;

			return regex.IsMatch(value);
		}

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("matches '{0}'", regex);
        }
    }
}