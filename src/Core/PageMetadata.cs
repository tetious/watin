using System;
using System.Text.RegularExpressions;
using WatiN.Core.Properties;

namespace WatiN.Core
{
    /// <summary>
    /// Provides metadata about a page.
    /// </summary>
    public class PageMetadata
    {
        private readonly Type pageType;
        private readonly Regex urlRegex;
        private readonly bool isSecure;

        /// <summary>
        /// Creates a page metadata object about a particular subclass of <see cref="Page" />.
        /// </summary>
        /// <param name="pageType">The page type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pageType"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="pageType"/> is not a subclass
        /// of <see cref="Page" /></exception>
        public PageMetadata(Type pageType)
        {
            if (pageType == null)
                throw new ArgumentNullException("pageType");
            if (! pageType.IsSubclassOf(typeof(Page)))
                throw new ArgumentException(Resources.PageMetadata_PageTypeIsExpectedToBeASubclassOfPage, "pageType");

            this.pageType = pageType;

            PageAttribute pageAttrib = GetPageAttribute(pageType);
            if (pageAttrib != null)
            {
                urlRegex = pageAttrib.UrlRegex != null ? new Regex(pageAttrib.UrlRegex) : null;
                isSecure = pageAttrib.IsSecure;
            }
        }

        /// <summary>
        /// Gets the subclass of <see cref="Page" /> that describes the page.
        /// </summary>
        public Type PageType
        {
            get { return pageType; }
        }

        /// <summary>
        /// Gets a regular expression that is expected to match the Url of the page, or null if unknown.
        /// </summary>
        public Regex UrlRegex
        {
            get { return urlRegex; }
        }

        /// <summary>
        /// Returns true if the page is expected to always be accessed using HTTPS.
        /// </summary>
        public bool IsSecure
        {
            get { return isSecure; }
        }

        private static PageAttribute GetPageAttribute(Type pageType)
        {
            var attribs = pageType.GetCustomAttributes(typeof(PageAttribute), true);
            return attribs.Length != 0 ? (PageAttribute) attribs[0] : null;
        }
    }
}
