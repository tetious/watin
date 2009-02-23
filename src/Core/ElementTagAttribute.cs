using System;

namespace WatiN.Core
{
    /// <summary>
    /// Specifies the HTML tags associated with a given <see cref="Element" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class ElementTagAttribute : Attribute
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
        /// Creates an <see cref="ElementTag" /> object from the contents of the attribute.
        /// </summary>
        /// <returns>The element tag</returns>
        public ElementTag ToElementTag()
        {
            return new ElementTag(TagName, InputType);
        }
    }
}
