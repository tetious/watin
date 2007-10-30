#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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
using System.Collections;
using System.Globalization;
using mshtml;
using StringComparer = WatiN.Core.Comparers.StringComparer;

namespace WatiN.Core
{
	/// <summary>
	/// This class is mainly used by WatiN internally and defines 
	/// the supported html tags for inheritors of <see cref="Element"/>.
	/// </summary>
	public class ElementTag
	{
		public readonly string TagName = null;
		public readonly string InputTypes;
		public readonly bool IsInputElement = false;

		public ElementTag(string tagName) : this(tagName, null) {}

		public ElementTag(IHTMLElement element): this(element.tagName, getInputType(element)) {}

		public ElementTag(string tagName, string inputTypes)
		{
			if (tagName != null)
			{
				TagName = tagName.ToLower(CultureInfo.InvariantCulture);
			}

			IsInputElement = ElementFinder.isInputElement(tagName);

			// Check arguments
			if (IsInputElement)
			{
				if (UtilityClass.IsNullOrEmpty(inputTypes))
				{
					throw new ArgumentNullException("inputTypes", String.Format("inputTypes must be set when tagName is '{0}'", tagName));
				}

				InputTypes = inputTypes.ToLower(CultureInfo.InvariantCulture);
			}
		}

		private static string getInputType(IHTMLElement element)
		{
			IHTMLInputElement inputElement = element as IHTMLInputElement;
			if(inputElement != null)
			{
				return inputElement.type;
			}

			return null;
		}

		public IHTMLElementCollection GetElementCollection(IHTMLElementCollection elements)
		{
			if (elements == null) return null;

			if (TagName == null) return elements;

			return (IHTMLElementCollection) elements.tags(TagName);
		}

		public bool Compare(object element)
		{
			IHTMLElement ihtmlElement = element as IHTMLElement;

			return Compare(ihtmlElement);
		}

		public bool Compare(IHTMLElement element)
		{
			if (element == null) return false;

			if (CompareTagName(element))
			{
				if (IsInputElement)
				{
					return CompareAgainstInputTypes(element);
				}

				return true;
			}

			return false;
		}

		public override string ToString()
		{
			if (TagName != null)
			{
				string tagName = TagName.ToUpper(CultureInfo.InvariantCulture);
				if (IsInputElement)
				{
					return String.Format("{0} ({1})", tagName, InputTypes);
				}

				return tagName;
			}

			return string.Empty;
		}

		private bool CompareTagName(IHTMLElement element)
		{
			if (TagName == null) return true;

			return StringComparer.AreEqual(TagName, element.tagName, true);
		}

		private bool CompareAgainstInputTypes(IHTMLElement element)
		{
			IHTMLInputElement inputElement = (IHTMLInputElement) element;

			string inputElementType = inputElement.type.ToLower(CultureInfo.InvariantCulture);

			return (InputTypes.IndexOf(inputElementType) >= 0);
		}

		public override int GetHashCode()
		{
			return (TagName != null ? TagName.GetHashCode() : 0) + 29*(InputTypes != null ? InputTypes.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			ElementTag elementTag = obj as ElementTag;
			if (elementTag == null) return false;
			if (!Equals(TagName, elementTag.TagName)) return false;
			if (!Equals(InputTypes, elementTag.InputTypes)) return false;
			return true;
		}

		public static bool IsValidElement(object element, ArrayList elementTags)
		{
			return IsValidElement(element as IHTMLElement, elementTags);
		}

		public static bool IsValidElement(IHTMLElement element, ArrayList elementTags)
		{
			if (element == null) return false;

			foreach (ElementTag elementTag in elementTags)
			{
				if (elementTag.Compare(element))
				{
					return true;
				}
			}

			return false;
		}
	}
}