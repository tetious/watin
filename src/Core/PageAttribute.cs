using System;
using System.Collections.Generic;
using System.Text;

namespace WatiN.Core
{
    /// <summary>
    /// Specifies metadata about a <see cref="Page" /> class.
    /// </summary>
    /// <seealso cref="Page"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class PageAttribute : Attribute
    {
        private string urlRegex;
        private bool isSecure;

        /// <summary>
        /// Gets or sets a regular expression that is expected to match the Url of the page.
        /// </summary>
        /// <remarks>
        /// The regular expression should generally exclude host name and domain information
        /// since it may vary between production and testing environments.  It only needs
        /// to match enough of the Url to reliably detect that page navigation has proceeded
        /// as planned.
        /// </remarks>
        /// <value>The url regular expression or null if none</value>
        public string UrlRegex
        {
            get { return urlRegex; }
            set { urlRegex = value; }
        }

        /// <summary>
        /// Gets or sets whether the web page url is always expected to be access using the HTTPS protocol.
        /// </summary>
        /// <remarks>
        /// When set to <c>true</c>, page url validation will check that the HTTPS protocol appears in
        /// the page url.
        /// </remarks>
        public bool IsSecure
        {
            get { return isSecure; }
            set { isSecure = value; }
        }
    }
}
