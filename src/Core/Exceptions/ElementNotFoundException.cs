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
using System.Text;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Exceptions
{
	/// <summary>
	/// Thrown if the searched for element can't be found.
	/// </summary>
    [Serializable]
	public class ElementNotFoundException : ElementExceptionBase
	{
        public ElementNotFoundException(string tagName, string criteria, string url, Element element) :
            base(CreateMessage(tagName, criteria, url, null)) { Element = element; }

        public ElementNotFoundException(string tagName, string criteria, string url, Exception innerexception, Element element) :
            base(CreateMessage(tagName, criteria, url, innerexception.Message), innerexception) { Element = element; }

        public ElementNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {}

		private static string CreateMessage(string tagName, string criteria, string url, string innerException)
		{
            var builder = new StringBuilder();
            builder.AppendFormat("Could not find {0} element tag", tagName ?? string.Empty);

            if (UtilityClass.IsNotNullOrEmpty(criteria))
			{
				builder.Append(" matching criteria: " + criteria);
			}

            if (UtilityClass.IsNotNullOrEmpty(url))
            {
                builder.Append(" at " + url);
            }
            
			if (UtilityClass.IsNotNullOrEmpty(innerException))
			{
				builder.Append(" (inner exception: "+ innerException + ")");
			}

			
			return builder.ToString();
		}
	}
}