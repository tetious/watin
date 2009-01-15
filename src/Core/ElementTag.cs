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
using System.Collections.Generic;
using System.Globalization;
using WatiN.Core.Interfaces;
using WatiN.Core.UtilityClasses;
using StringComparer = WatiN.Core.Comparers.StringComparer;

namespace WatiN.Core
{
	/// <summary>
	/// This class is mainly used by WatiN internally and defines 
	/// the supported html tags for inheritors of <see cref="Element"/>.
	/// </summary>
	public class ElementTag
	{
		public readonly string TagName;
		public readonly string InputTypes;
		public readonly bool IsInputElement;

		public ElementTag(string tagName) : this(tagName, null) {}

        public ElementTag(INativeElement nativeElement) : this(nativeElement.TagName, getInputType(nativeElement)) {}

		public ElementTag(string tagName, string inputTypes)
		{
			if (tagName != null)
			{
				TagName = tagName.ToLower(CultureInfo.InvariantCulture);
			}

			IsInputElement = IsAnInputElement(tagName);

			// Check arguments
		    if (!IsInputElement) return;
		    
            if (UtilityClass.IsNullOrEmpty(inputTypes))
		    {
		        throw new ArgumentNullException("inputTypes", String.Format("inputTypes must be set when tagName is '{0}'", tagName));
		    }

		    InputTypes = inputTypes.ToLower(CultureInfo.InvariantCulture);
		}

		private static string getInputType(INativeElement ieNativeElement)
		{
		    return IsAnInputElement(ieNativeElement.TagName) ? ieNativeElement.GetAttributeValue("type") : null;
		}

		public bool Compare(INativeElement nativeElement)
		{
			if (nativeElement == null) return false;

			if (CompareTagName(nativeElement))
			{
			    return !IsInputElement || CompareInputTypes(nativeElement);
			}

		    return false;
		}

		public override string ToString()
		{
			if (TagName != null)
			{
				var tagName = TagName.ToUpper(CultureInfo.InvariantCulture);
				return IsInputElement ? String.Format("{0} ({1})", tagName, InputTypes) : tagName;
			}

			return string.Empty;
		}

		private bool CompareTagName(INativeElement nativeElement)
		{
		    return TagName == null || StringComparer.AreEqual(TagName, nativeElement.TagName, true);
		}

	    public bool CompareInputTypes(INativeElement element)
		{
			var inputElementType = element.GetAttributeValue("type").ToLower(CultureInfo.InvariantCulture);

			return (InputTypes.IndexOf(inputElementType) >= 0);
		}

		public override int GetHashCode()
		{
			return (TagName != null ? TagName.GetHashCode() : 0) + 29*(InputTypes != null ? InputTypes.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			
            var elementTag = obj as ElementTag;
			if (elementTag == null) return false;
			
            return Equals(TagName, elementTag.TagName) && Equals(InputTypes, elementTag.InputTypes);
		}

		public static bool IsValidElement(INativeElement nativeElement, List<ElementTag> elementTags)
		{
			if (nativeElement == null) return false;

			foreach (var elementTag in elementTags)
			{
				if (elementTag.Compare(nativeElement))
				{
					return true;
				}
			}

			return false;
		}

	    public static bool IsAnInputElement(string tagName)
		{
			return StringComparer.AreEqual(tagName, ElementsSupport.InputTagName, true);
		}

        public static string ElementTagsToString(List<ElementTag> elementTags)
		{
			var elementTagsString = String.Empty;

			foreach (var elementTag in elementTags)
			{
				if (elementTagsString.Length > 0)
				{
					elementTagsString = elementTagsString + " or ";
				}
				elementTagsString = elementTagsString + elementTag;
			}

			return elementTagsString;
		}
	}
}
