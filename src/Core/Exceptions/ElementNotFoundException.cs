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
using System.Runtime.Serialization;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Exceptions
{
	/// <summary>
	/// Thrown if the searched for element can't be found.
	/// </summary>
    [Serializable]
	public class ElementNotFoundException : WatiNException
	{
		public ElementNotFoundException(string tagName, string criteria, string url) :
			base(CreateMessage(tagName, criteria, url, null)) {}

		public ElementNotFoundException(string tagName, string criteria, string url, Exception innerexception) :
			base(CreateMessage(tagName, criteria, url, innerexception.Message), innerexception) {}

        public ElementNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {}

		private static string CreateMessage(string tagName, string criteria, string url, string innerException)
		{
			var message = string.Format("Could not find {0} element tag", tagName ?? string.Empty);

            if (UtilityClass.IsNotNullOrEmpty(criteria))
			{
				message += " matching criteria: " + criteria;
			}

            if (UtilityClass.IsNotNullOrEmpty(url))
            {
                message += " at " + url;
            }
            
			if (UtilityClass.IsNotNullOrEmpty(innerException))
			{
				message += " (inner exception: "+ innerException + ")";
			}

			
			return message;
		}
	}
}