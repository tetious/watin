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

namespace WatiN.Core
{
    /// <summary>
    /// Specifies the HTML tags associated with a given <see cref="Element" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class ElementTagAttribute : Attribute, IComparable<ElementTagAttribute>
    {
        /// <summary>
        /// Associates a tag with an <see cref="Element" /> class.
        /// </summary>
        /// <param name="tagName">The tag name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tagName"/> is null</exception>
        public ElementTagAttribute(string tagName)
        {
            if (tagName == null)
                throw new ArgumentNullException("tagName");

            TagName = tagName;
        }

        /// <summary>
        /// Gets the tag name.
        /// </summary>
        public string TagName { get; private set; }

        /// <summary>
        /// Gets or sets the "type" attribute value to qualify which variations of an INPUT tag are supported.
        /// </summary>
        public string InputType { get; set; }

        /// <summary>
        /// Gets or sets the "index" attribute value to force a specific order of the ElementTag in a list of ElementTags.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Creates an <see cref="ElementTag" /> object from the contents of the attribute.
        /// </summary>
        /// <returns>The element tag</returns>
        public ElementTag ToElementTag()
        {
            return new ElementTag(TagName, InputType);
        }

        public int CompareTo(ElementTagAttribute other)
        {
            return Index.CompareTo(other.Index);
        }
    }
}
