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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This struct is mainly used by WatiN internally and defines 
	/// the supported html tags for inheritors of <see cref="Element"/>.
	/// </summary>
	public struct ElementTag : IEquatable<ElementTag>, IComparable<ElementTag>
	{
        private readonly string tagName;
        private readonly string inputType;

        /// <summary>
        /// Creates an element tag.
        /// </summary>
        /// <param name="tagName">The tag name, or null to represent any element</param>
        public ElementTag(string tagName)
            : this(tagName, null)
        {
        }

        /// <summary>
        /// Creates an element tag with an input tag type qualifier.
        /// </summary>
        /// <param name="tagName">The tag name, or null to represent any element</param>
        /// <param name="inputType">The input tag type qualifier, or null if none</param>
	    public ElementTag(string tagName, string inputType)
		{
            this.tagName = tagName != null ? tagName.ToLowerInvariant() : null;
            this.inputType = inputType != null ? inputType.ToLowerInvariant() : null;

            if (IsAnInputElement(tagName))
            {
                if (inputType == null)
                    throw new ArgumentNullException("inputType", String.Format("inputType must be set when tagName is '{0}'", tagName));
            }
            else
            {
                if (inputType != null)
                    throw new ArgumentNullException("inputType", String.Format("inputType must be null when tagName is '{0}'", tagName));
            }
		}

        /// <summary>
        /// Gets the tag name, or null to represent any element.
        /// </summary>
        public string TagName
        {
            get { return tagName; }
        }

        /// <summary>
        /// Gets the input tag type qualifier, or null if none.
        /// </summary>
        public string InputType
        {
            get { return inputType; }
        }

        /// <summary>
        /// Returns true if the tag represents an input element.
        /// </summary>
        public bool IsInputElement
        {
            get { return IsAnInputElement(tagName); }
        }
        
        /// <summary>
        /// Returns true if the tag matches any element.
        /// </summary>
        public bool IsAny
        {
            get { return tagName == null; }
        }

        /// <summary>
        /// Returns a special tag object that can match any element.
        /// </summary>
        public static ElementTag Any
        {
            get { return new ElementTag(null, null); }
        }

        /// <summary>
        /// Creates an element tag object from a native element.
        /// </summary>
        /// <param name="nativeElement">The native element</param>
        /// <returns>The element tag object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeElement"/> is null</exception>
        public static ElementTag FromNativeElement(INativeElement nativeElement)
        {
            if (nativeElement == null)
                throw new ArgumentNullException("nativeElement");

            return new ElementTag(nativeElement.TagName, GetInputType(nativeElement));
        }

        /// <summary>
        /// Returns true if this tag object matches the specified element.
        /// </summary>
        /// <param name="nativeElement">The element to consider</param>
        /// <returns>True if the tag matches the specified element</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeElement"/> is null</exception>
        public bool IsMatch(INativeElement nativeElement)
        {
            if (nativeElement == null)
                throw new ArgumentNullException("nativeElement");

            return IsMatch(FromNativeElement(nativeElement));
        }

        /// <summary>
        /// Returns true if this tag object matches the specified tag.
        /// </summary>
        /// <param name="elementTag">The element tag to consider</param>
        /// <returns>True if the tag matches the specified element tag</returns>
        public bool IsMatch(ElementTag elementTag)
        {
            return IsAny
                || tagName == elementTag.tagName && inputType == elementTag.inputType;
        }

        /// <inheritdoc />
		public override int GetHashCode()
		{
			return (tagName != null ? tagName.GetHashCode() : 0)
                + 29 * (inputType != null ? inputType.GetHashCode() : 0);
		}

        /// <inheritdoc />
        public override bool Equals(object obj)
		{
            return obj is ElementTag && Equals((ElementTag)obj);
		}

        /// <inheritdoc />
        public bool Equals(ElementTag other)
        {
            return tagName == other.tagName && inputType == other.inputType;
        }

	    public int CompareTo(ElementTag other)
	    {
	        var compare = TagName.CompareTo(other.TagName);
            if (compare == 0 && InputType != null)
            {
                compare = InputType.CompareTo(other.InputType);
            }
	        return compare;
	    }

	    /// <summary>
        /// Returns a human-readable string representation of the tag.
        /// </summary>
        /// <returns>The tag as a string</returns>
        public override string ToString()
        {
            if (TagName != null)
            {
                var tagName = TagName.ToUpper(CultureInfo.InvariantCulture);
                return IsInputElement ? String.Format("{0} ({1})", tagName, InputType) : tagName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns true if any tag object in the list matches the specified element.
        /// </summary>
        /// <param name="tags">The tags against which to match</param>
        /// <param name="nativeElement">The element to consider</param>
        /// <returns>True if the tag matches the specified element</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tags"/> or <paramref name="nativeElement"/> is null</exception>
        public static bool IsMatch(IEnumerable<ElementTag> tags, INativeElement nativeElement)
        {
            if (nativeElement == null)
                throw new ArgumentNullException("nativeElement");

            return IsMatch(tags, FromNativeElement(nativeElement));
        }

        /// <summary>
        /// Returns true if any tag object in the list matches the specified tag.
        /// </summary>
        /// <param name="tags">The tags against which to match</param>
        /// <param name="elementTag">The element tag to consider</param>
        /// <returns>True if the tag matches the specified element tag</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tags"/> is null</exception>
        public static bool IsMatch(IEnumerable<ElementTag> tags, ElementTag elementTag)
        {
            if (tags == null)
                throw new ArgumentNullException("tags");

            foreach (var tag in tags)
                if (tag.IsMatch(elementTag))
                    return true;

            return false;
        }

        /// <summary>
        /// Converts a list of tags to a human-readable string.
        /// </summary>
        /// <param name="elementTags">The list of element tags</param>
        /// <returns>The element tags as a string</returns>
        public static string ElementTagsToString(IList<ElementTag> elementTags)
		{
			var elementTagsString = String.Empty;
            var sortedElementTags = new List<ElementTag>(elementTags);
            sortedElementTags.Sort();

			foreach (var elementTag in sortedElementTags)
			{
				if (elementTagsString.Length > 0)
				{
					elementTagsString = elementTagsString + " or ";
				}
				elementTagsString = elementTagsString + elementTag;
			}

			return elementTagsString;
		}

        private static string GetInputType(INativeElement nativeElement)
        {
            return IsAnInputElement(nativeElement.TagName) ? nativeElement.GetAttributeValue("type") : null;
        }

        private static bool IsAnInputElement(string tagName)
        {
            return string.Compare(tagName, "input", StringComparison.InvariantCultureIgnoreCase) == 0;
        }

	    public static IEnumerable<string> ElementTagNames(IEnumerable<ElementTag> elementTags)
	    {
            var tagNames = new List<string>();
            foreach (var elementTag in elementTags)
            {
                if (tagNames.Contains(elementTag.TagName)) continue;
                tagNames.Add(elementTag.TagName);
            }
            return tagNames;
	    }

	    public static IEnumerable<ElementTag> ToElementTags(string tagName, params string[] inputTypes)
	    {
            if (inputTypes != null && ((ICollection)inputTypes).Count != 0)
            {
                foreach (var inputType in inputTypes)
                     yield return new ElementTag(tagName, inputType);
            }
            else
            {
                yield return new ElementTag(tagName);
            }
	        
	    }
	}
}
