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

namespace WatiN.Core.Native
{
    /// <summary>
    /// Provides access to native services offered by a web browser.
    /// </summary>
    public interface INativeBrowser 
    {
        /// <summary>
        /// Navigates to the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        void NavigateTo(Uri url);

        /// <summary>
        /// Navigates to the specified <paramref name="url"/> without waiting for the page to finish loading.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        void NavigateToNoWait(Uri url);

        /// <summary>
        /// Navigates the browser back to the previously display Url
        /// </summary>
        /// <returns><c>True</c> if succeded otherwise <c>false</c>.</returns>
        bool GoBack();

        /// <summary>
        /// Navigates the browser forward to the next displayed Url (like the forward
        /// button in Internet Explorer). 
        /// </summary>
        /// <returns><c>True</c> if succeded otherwise <c>false</c>.</returns>
        bool GoForward();

        /// <summary>
        /// Closes and then reopens the browser with a blank page.
        /// </summary>
        void Reopen();

        /// <summary>
        /// Reloads the currently displayed webpage (like the Refresh/reload button in 
        /// a browser).
        /// </summary>
        void Refresh();

        /// <summary>
        /// Gets the window handle of the current browser.
        /// </summary>
        /// <value>Window handle of the current browser.</value>
        IntPtr hWnd { get; }

        INativeDocument NativeDocument { get; }
    }
}