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

#if !NET11
using System;

namespace WatiN.Core.Comparers
{
    /// <summary>
    /// This class supports comparing string values using a <see cref="Predicate{String}"/>.
    /// </summary>
	public class PredicateStringComparer : BaseComparer
	{
		private readonly Predicate<string> _compareString;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateStringComparer"/> class.
        /// </summary>
        /// <param name="predicate">The string predicate which will be used for the comparision(s) done by <see cref="PredicateStringComparer.Compare(string)"/>.</param>
        public PredicateStringComparer(Predicate<string> predicate)
		{
			_compareString = predicate;	
		}

        /// <summary>
        /// Passes the given <paramref name="value"/> to the string predicate to do the actual comparison
        /// </summary>
        /// <param name="value">A string value</param>
        /// <returns>The result of the comparison done by the predicate</returns>
        public override bool Compare(string value)
		{
			return _compareString.Invoke(value);
		}
	}
}
#endif
