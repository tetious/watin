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
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Comparers
{
	/// <summary>
	/// Class that supports comparing a <see cref="Uri"/> instance with a string value.
	/// </summary>
	public class UriComparer : Comparer<string>
	{
		private readonly Uri uriToCompareWith;
		private readonly bool _ignoreQuery;

		/// <summary>
		/// Constructor, querystring will not be ignored in comparisons.
		/// </summary>
		/// <param name="uri">Uri for comparison.</param>
		public UriComparer(Uri uri) : this(uri, false) {}

		/// <summary>
		/// Constructor, querystring can be ignored or not ignored in comparisons.
		/// </summary>
		/// <param name="uri">Uri for comparison.</param>
		/// <param name="ignoreQuery">Set to true to ignore querystrings in comparison.</param>
		public UriComparer(Uri uri, bool ignoreQuery)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			uriToCompareWith = uri;
			_ignoreQuery = ignoreQuery;
		}

        /// <summary>
        /// Compares the specified value. 
        /// </summary>
        /// <param name="value">The url to compare with.</param>
        /// <returns>
        /// Should return <c>true</c> or <c>false</c>, which is the default.
        /// </returns>
		public override bool Compare(string value)
        {
            return !UtilityClass.IsNullOrEmpty(value) && Compare(UtilityClass.CreateUri(value));
        }

	    /// <summary>
		/// Compares the specified Uri.
		/// </summary>
		/// <param name="url">The Uri.</param>
		/// <returns><c>true</c> when equal; otherwise <c>false</c></returns>
		public virtual bool Compare(Uri url)
		{
			if (!_ignoreQuery)
			{
				// compare without modification
				return uriToCompareWith.Equals(url);
			}
		
            // trim querystrings
		    var trimmedUrl = TrimQueryString(url);
		    var trimmedUrlToCompareWith = TrimQueryString(uriToCompareWith);
		    
            // compare trimmed urls.
		    return (string.Compare(trimmedUrl, trimmedUrlToCompareWith, true) == 0);
		}

		private static string TrimQueryString(Uri url)
		{
			return url.AbsoluteUri.Split('?')[0];
		}

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("equals uri '{0}'{1}", uriToCompareWith.AbsoluteUri, _ignoreQuery ? " ignoring query parameters" : "");
        }
    }
}