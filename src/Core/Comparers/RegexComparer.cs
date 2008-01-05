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

namespace WatiN.Core.Comparers
{
	/// <summary>
	/// Class that supports matching a regular expression with a string value.
	/// </summary>
	public class RegexComparer : BaseComparer
	{
		private Regex regexToUse;

		public RegexComparer(Regex regex)
		{
			if (regex == null)
			{
				throw new ArgumentNullException("regex");
			}
			regexToUse = regex;
		}

		public override bool Compare(string value)
		{
			if (value == null) return false;

			return regexToUse.IsMatch(value);
		}

		public override string ToString()
		{
			return GetType().ToString() + " matching against: " + regexToUse.ToString();
		}
	}
}