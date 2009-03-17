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
using System.Reflection;
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

            var pageAttrib = GetPageAttribute(pageType);
            if (pageAttrib == null) return;

            urlRegex = pageAttrib.UrlRegex != null ? new Regex(pageAttrib.UrlRegex) : null;
            isSecure = pageAttrib.IsSecure;
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

        private static PageAttribute GetPageAttribute(ICustomAttributeProvider pageType)
        {
            var attribs = pageType.GetCustomAttributes(typeof(PageAttribute), true);
            return attribs.Length != 0 ? (PageAttribute) attribs[0] : null;
        }
    }
}
