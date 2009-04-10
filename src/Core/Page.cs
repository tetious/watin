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
using System.Text;
using WatiN.Core.Exceptions;

namespace WatiN.Core
{
    /// <summary>
    /// A page class describes the content and behaviors of a single web page or frame within
    /// a web site.  Each application page can be modeled as a subclass of <see cref="Page" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A page class typically provides access to the content and behaviors of the page
    /// by defining properties and methods to represent them.  In the typical case, the properties
    /// of a page class provide access to its elements and state.  Likewise, the methods of a page
    /// class perform actions or transactions.  The page class encapsulates the mechanisms used
    /// to access and manipulate the underlying DOM elements which enables test cases to
    /// become more focused on verifying application functionality.
    /// </para>
    /// <para>
    /// For example, instead of referring to a text box by its HTML ID everywhere it is used,
    /// we can create a page class to describe how the text box is accessed and what it represents
    /// (semantically) in terms of the application.  In this way, we can create an appropriately
    /// named property to provide access to the text box and clearly indicate
    /// that it belongs to a given web page in our application.
    /// </para>
    /// <para>
    /// This feature can be used to create a model of a web site (a site map) which helps to
    /// improve readability and encourage reuse across tests.
    /// </para>
    /// <para>
    /// How to create a page class:
    /// <list type="bullet">
    /// <item>Create a subclass of Page.</item>
    /// <item>Add a [Page] attribute to the newly created page.  Set the UrlRegex
    /// property to a regular expression that is expected to match part of the page's Url.
    /// If the page is only accessible via HTTPS then also set the IsSecure property to true.</item>
    /// <item>Add properties to provide access to the sub-elements of the page.  When the page
    /// is used, the <see cref="Document" /> property will be set to the containing _document
    /// (eg. the web browser or a frame).  Use this property to locate the sub-elements of the page.</item>
    /// <item>Define additional properties and methods as desired to model the state and
    /// behaviors of the page.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Tips:
    /// <list type="bullet">
    /// <item>Multiple pages might share the same basic layout.  You can model this structure by
    /// using a super class to describe the "master page" or by defining one or more
    /// <see cref="Control{TElement}" /> to capture recurring elements such as the page header or
    /// footer.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Consider a web page that has a username text field, a password text field
    /// and a sign in button.  Before writing the test for this page, we
    /// create a new page class to describe how these controls are found.
    /// We can also add a convenient SignIn method that sets both text fields
    /// and clicks on the sign in button all at once.
    /// </para>
    /// <para>
    /// Here's how the page class will look:
    /// </para>
    /// <code>
    /// [Page(UrlRegex = "SignIn.aspx", IsSecure = true)]
    /// public class SignInPage : Page
    /// {
    ///     public TextField UserNameTextField { get { return Document.TextField(Find.ByName("username")); } }
    ///     public TextField PasswordNameTextField { get { return Document.TextField(Find.ByName("password")); } }
    ///     public TextField SignInButton { get { return Document.Button(Find.ByName("signin")); } }
    /// 
    ///     public void SignIn(string userName, string password)
    ///     {
    ///         // Fill in the username and password fields.
    ///         UserNameTextField.TypeText(userName);
    ///         PasswordTextField.TypeText(password);
    /// 
    ///         // Click the sign in button.
    ///         SignInButton.Click();
    ///     }
    /// }
    /// </code>
    /// <para>
    /// Within a test, we can use the functionality of the sign in page like this:
    /// </para>
    /// <code>
    /// browser.Page&lt;SignInPage&gt;>().SignIn("somebody", "letmein");
    /// </code>
    /// </example>
    /// <seealso cref="PageAttribute"/>
    public abstract class Page : Component
    {
        private PageMetadata _metadata;
        private Document _document;
 
        /// <summary>
        /// Creates an uninitialized page instance.
        /// </summary>
        protected Page()
        {
        }

        /// <summary>
        /// Gets declarative _metadata about the page.
        /// </summary>
        public PageMetadata Metadata
        {
            get
            {
                if (_metadata == null)
                    _metadata = new PageMetadata(GetType());
                return _metadata;
            }
        }

        /// <summary>
        /// Gets the _document or frame that holds the page content.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method calls <see cref="VerifyDocumentProperties" /> to ensure that
        /// the current _document represents the correct page (has the correct Url, etc.).
        /// If this verification fails then an exception is thrown.
        /// </para>
        /// </remarks>
        /// <exception cref="WatiNException">Thrown if the page object does not have a reference
        /// to a _document or if the _document's properties fail validation.</exception>
        public Document Document
        {
            get
            {
                if (_document == null)
                    throw new WatiNException("The Document is not available because the Page instance has not been fully initialized.");
                VerifyDocumentProperties(_document);
                return _document;
            }
        }

        /// <summary>
        /// Verifies that the _document represents the correct page (has the correct Url, etc.).
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation calls <see cref="VerifyDocumentUrl" /> to verify the <paramref name="document"/>'s Url.
        /// </para>
        /// <para>
        /// Subclasses can override this method to customize how _document verification takes place.
        /// </para>
        /// </remarks>
        /// <param name="document">The _document to verify, not null</param>
        /// <exception cref="WatiNException">Thrown if the _document's properties fail verification</exception>
        protected virtual void VerifyDocumentProperties(Document document)
        {
            VerifyDocumentUrl(document.Url);
            VerifyDocumentIsSecure(document.Url);
        }

        /// <summary>
        /// Verifies that the _document's represents the correct page.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation uses information from the associated <see cref="PageMetadata" />
        /// to validate the <paramref name="url"/>.
        /// </para>
        /// <para>
        /// Subclasses can override this method to customize how _document Url verification takes place.
        /// </para>
        /// </remarks>
        /// <param name="url">The _document url to verify, not null</param>
        /// <exception cref="WatiNException">Thrown if the url fails verification</exception>
        protected virtual void VerifyDocumentUrl(string url)
        {
            if (Metadata.UrlRegex != null && !Metadata.UrlRegex.IsMatch(url))
            {
                throw new WatiNException(String.Format(
                    "Page '{0}' expected the Url to match the regular expression pattern '{1}'.", this, Metadata.UrlRegex));
            }
        }

        protected virtual void VerifyDocumentIsSecure(string url)
        {
            if (Metadata.IsSecure && !url.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new WatiNException(String.Format("Page '{0}' expected the Url to begin with 'https:'.", this));
            }
        }

        /// <summary>
        /// Creates an initialized page object from a _document.
        /// </summary>
        /// <typeparam name="T">The page type</typeparam>
        /// <param name="document">The _document or frame represented by the page</param>
        /// <returns>The page object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="document"/> is null</exception>
        public static T CreatePage<T>(Document document)
            where T : Page, new()
        {
            if (document == null)
                throw new ArgumentNullException("document");

            var page = new T {_document = document};
            return page;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder description = new StringBuilder();
            description.Append(GetType().Name);

            // Note: Uses unvalidated _document to avoid throwing an exception if the _document is incorrect.
            if (_document != null)
            {
                description.Append(@" (");
                description.Append(_document.Url);
                description.Append(@")");
            }

            return description.ToString();
        }

        /// <inheritdoc />
        public sealed override T GetAdapter<T>()
        {
            if (typeof(Document).IsAssignableFrom(typeof(T)))
                return Document as T;

            return base.GetAdapter<T>();
        }

        /// <inheritdoc />
        protected sealed override string GetAttributeValueImpl(string attributeName)
        {
            return Document.GetAttributeValue(attributeName);
        }
    }
}
